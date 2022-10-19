using AutoMapper;
using ExchangeRateAPI.Entities;
using ExchangeRateAPI.Exceptions;
using ExchangeRateAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ExchangeRateAPI.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ExchangeDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<ExchangeRateService> _logger;

        public ExchangeRateService(IHttpClientFactory httpClientFactory, ExchangeDbContext dbContext,
            IMapper mapper, ILogger<ExchangeRateService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }


        public async Task<IEnumerable<ExchangeRateViewModel>> GetExchangeRate(KeyValuePair<string, string> currencyCodes,
            DateTime startDate, DateTime endDate)
        {
            _logger.LogWarning($"GetExchangeRate invoked for {currencyCodes.Key}:{currencyCodes.Value}" +
                $"from {startDate} to {endDate}");
            if (startDate > DateTime.Now ||
                endDate > DateTime.Now ||
                endDate<startDate)
            {
                throw new NotFoundException();
            }

            var ratesFromDb = await GetFromDb(currencyCodes, startDate, endDate);
            var daysInterval = (int)((endDate - startDate).TotalDays);
            var dates = Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days)
          .Select(offset => startDate.AddDays(offset))
          .ToList();

            foreach (var day in dates)
            {
                if (day.DayOfWeek == DayOfWeek.Sunday ||
                    day.DayOfWeek == DayOfWeek.Saturday)
                {
                    daysInterval--;
                }
            }

            if (ratesFromDb.Count() > daysInterval)
            {
                if (ratesFromDb.Count() == 0)
                {
                    return await GetExternalResponse(currencyCodes, startDate, endDate, ratesFromDb);
                }
                return _mapper.Map<List<ExchangeRateViewModel>>(ratesFromDb);
            }

            return await GetExternalResponse(currencyCodes, startDate, endDate, ratesFromDb);
        }



        private async Task<IEnumerable<Cache>> GetFromDb(KeyValuePair<string, string> currencyCodes,
            DateTime startDate, DateTime endDate)
        {
            var exchangeRatesList = await _dbContext.Caches                
                .Where(x => x.FirstCurrency == currencyCodes.Key && x.SecondCurrency == currencyCodes.Value &&
                x.Date >= startDate && x.Date <= endDate)
                .ToListAsync();
            return exchangeRatesList;
        }

        private async Task<IEnumerable<ExchangeRateViewModel>> GetExternalResponse(KeyValuePair<string, string> currencyCodes,
            DateTime startDate, DateTime endDate, IEnumerable<Cache> resultFromDb)
        {
            _logger.LogWarning($"Downloading exchange rates {currencyCodes.Key}:{currencyCodes.Value}" +
                $"from {startDate} to {endDate} from external api. ");
            var httpClient = _httpClientFactory.CreateClient("ECB_SDMX");

            var startDateString = startDate.ToString("yyyy-MM-dd");
            var endDateString = endDate.ToString("yyyy-MM-dd");

            var result = await httpClient.GetAsync($"D.{currencyCodes.Key}.{currencyCodes.Value}.SP00.A" +
                $"?startPeriod={startDateString}" +
                $"&endPeriod={endDateString}" +
                $"&detail=dataonly");
            var content = await result.Content.ReadAsStringAsync();
            if(content.Equals("No results found."))
            {
                throw new NotFoundException();
            }

            if (string.IsNullOrEmpty(content))
            {
                _logger.LogWarning($"No exchange rates found for {startDate}");
                var collectionWithClosestDay = new List<Cache>();
                collectionWithClosestDay.Add(await GetClosestAvaiablePreviousDayFromStartDate(currencyCodes, startDate));
                return _mapper.Map<List<ExchangeRateViewModel>>(collectionWithClosestDay);
            }

            var xmlSerializer = new XmlSerializer(typeof(GenericData));
            var genericData = (GenericData)xmlSerializer.Deserialize(new StringReader(content));
            var seriesKeys = genericData.DataSet.Series.SeriesKey;
            var obs = genericData.DataSet.Series.Obs;
            var cache = obs.Select(x => new Cache
            {
                FirstCurrency = seriesKeys.Single(x => x.id == "CURRENCY").value,
                SecondCurrency = seriesKeys.Single(x => x.id == "CURRENCY_DENOM").value,
                Date = x.ObsDimension.value,
                ExchangeRateValue = x.ObsValue.value
            }).ToList();

            if (!cache.Any(x=>x.Date == startDate))
            {
                _logger.LogWarning($"No exchange rates found for {startDate}");
                cache.Add(await GetClosestAvaiablePreviousDayFromStartDate(currencyCodes, startDate));
                cache = cache.OrderBy(x => x.Date).ToList();
            }

            // Upsert missing exchange rates to database.
            var newCache = new List<Cache>();
            foreach (var item in cache)
            {
                if (!resultFromDb.Any(x => x.Date == item.Date))
                {
                    newCache.Add(item);
                }
            }

            await _dbContext.AddRangeAsync(newCache);
            await _dbContext.SaveChangesAsync();


            return _mapper.Map<List<ExchangeRateViewModel>>(cache);

        }

        private async Task<Cache> GetClosestAvaiablePreviousDayFromStartDate(KeyValuePair<string, string> currencyCodes,
            DateTime startDate)
        {
            _logger.LogWarning($"Downloading recently avaiable record.");
            var startDateString = startDate.AddDays(-10).ToString("yyyy-MM-dd");
            var endDateString = startDate.ToString("yyyy-MM-dd");
            var httpClient = _httpClientFactory.CreateClient("ECB_SDMX");
            var result = await httpClient.GetAsync($"D.{currencyCodes.Key}.{currencyCodes.Value}.SP00.A" +
                $"?startPeriod={startDateString}" +
                $"&endPeriod={endDateString}" +
                $"&detail=dataonly");
            var content = await result.Content.ReadAsStringAsync();
            var xmlSerializer = new XmlSerializer(typeof(GenericData));
            var genericData = (GenericData)xmlSerializer.Deserialize(new StringReader(content));
            var seriesKeys = genericData.DataSet.Series.SeriesKey;
            var obs = genericData.DataSet.Series.Obs;
            var cache = obs.Select(x => new Cache
            {
                FirstCurrency = seriesKeys.Single(x => x.id == "CURRENCY").value,
                SecondCurrency = seriesKeys.Single(x => x.id == "CURRENCY_DENOM").value,
                Date = x.ObsDimension.value,
                ExchangeRateValue = x.ObsValue.value
            });
            cache = cache.OrderByDescending(x => x.Date);
            return cache.First();
        }


    }
}
