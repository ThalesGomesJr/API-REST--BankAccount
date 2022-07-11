using AutoMapper;
using BankAccount.API.DTOs;
using BankAccount.Domain.Models;

namespace BankAccount.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserRegisterDTO>().ReverseMap();
            
            CreateMap<User, UserDTO>().ReverseMap();
            
            CreateMap<User, UserBalanceDTO>().ReverseMap();
            
            CreateMap<User, UserLoginDTO>().ReverseMap();

            CreateMap<User, UserUpdateDTO>().ReverseMap();

            CreateMap<User, UserUpdatePasswordDTO>().ReverseMap();

            CreateMap<Extract, ExtractDTO>().ReverseMap();
 
        }
    }
}