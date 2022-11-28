﻿using AutoMapper;
using WebApiClient.DTOs;
using WebAppMVC.ViewModels;

namespace WebAppMVC.ViewModelProfiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<IEnumerable<OrderDto>, OrderIndexVM>()
            .ForMember(dest => dest.Orders, act => act.MapFrom(src => src));

            CreateMap<OrderDto, OrderDetailsVM>()
                .ReverseMap();

        }
    }
}
