using AutoMapper;
using ExchangeRateAPI.Entities;
using ExchangeRateAPI.Models;

namespace ExchangeRateAPI
{
    public class ExchangeMappingProfile : Profile
    {
        public ExchangeMappingProfile()
        {
            CreateMap<Cache, ExchangeRateViewModel>().ReverseMap();
        }
    }
}