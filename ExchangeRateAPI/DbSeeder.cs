﻿using ExchangeRateAPI.Entities;
using ExchangeRateAPI.Services;

namespace ExchangeRateAPI
{
    public class DbSeeder
    {
        private readonly IExchangeRateService _exchangeRateService;
        private readonly ExchangeDbContext _dbContext;

        public DbSeeder(IExchangeRateService exchangeRateService,
            ExchangeDbContext dbContext)
        {
            _exchangeRateService = exchangeRateService;
            _dbContext = dbContext;
        }

        // Seeding database with most popular currency pairs for 10 years back, for future optimization.
        public async Task Seed()
        {
            if (await _dbContext.Database.CanConnectAsync())
            {
                if (!_dbContext.Caches.Any())
                {
                    await GetUsdToEur();
                    await GetGbpToEur();
                    await GetPlnToEur();
                }
            }
        }

        private async Task GetUsdToEur()
        {
            var currencyCodes = new KeyValuePair<string, string>("USD", "EUR");
            var startDate = DateTime.Now.AddYears(-10);
            var endDate = DateTime.Now;
            await _exchangeRateService.GetExchangeRate(currencyCodes, startDate, endDate);
        }

        private async Task GetGbpToEur()
        {
            var currencyCodes = new KeyValuePair<string, string>("GBP", "EUR");
            var startDate = DateTime.Now.AddYears(-10);
            var endDate = DateTime.Now;
            await _exchangeRateService.GetExchangeRate(currencyCodes, startDate, endDate);
        }

        private async Task GetPlnToEur()
        {
            var currencyCodes = new KeyValuePair<string, string>("PLN", "EUR");
            var startDate = DateTime.Now.AddYears(-10);
            var endDate = DateTime.Now;
            await _exchangeRateService.GetExchangeRate(currencyCodes, startDate, endDate);
        }
    }
}