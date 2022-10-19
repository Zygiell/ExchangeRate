using AutoMapper;
using ExchangeRateAPI.Entities;
using ExchangeRateAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
