using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Dto;
using Todo.Models;

namespace Todo.Profiles
{
    public class TodoListProfile : Profile
    {
        public TodoListProfile()
        {
            CreateMap<TodoList, TodoListResponseDto>();
            CreateMap<TodoListPostDto, TodoList>()
                .ForMember(m => m.InsertTime,
                s => s.MapFrom(src => DateTime.Now))
                .ForMember(m => m.UpdateTime,
                s => s.MapFrom(src => DateTime.Now));
                //.ForMember(m => m.InsertEmployeeId,
                //s => s.MapFrom(src => Guid.Parse("00000000-0000-0000-0000-000000000001")))
                //.ForMember(m => m.UpdateEmployeeId,
                //s => s.MapFrom(src => Guid.Parse("00000000-0000-0000-0000-000000000001")));

            CreateMap<TodoListPostUpDto,TodoList>()
                .ForMember(m => m.InsertTime,
                s => s.MapFrom(src => DateTime.Now))
                .ForMember(m => m.UpdateTime,
                s => s.MapFrom(src => DateTime.Now))
                .ForMember(m => m.InsertEmployeeId,
                s => s.MapFrom(src => Guid.Parse("00000000-0000-0000-0000-000000000001")))
                .ForMember(m => m.UpdateEmployeeId,
                s => s.MapFrom(src => Guid.Parse("00000000-0000-0000-0000-000000000001")));
        }
    }
}
