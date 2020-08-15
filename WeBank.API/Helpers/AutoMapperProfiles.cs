using AutoMapper;
using WeBank.API.DTOs;
using WeBank.Domain.Models;

namespace WeBank.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserRegisterDTO>().ReverseMap();
            
            CreateMap<User, UserDTO>().ReverseMap();

            CreateMap<User, UserLoginDTO>().ReverseMap();
            
            CreateMap<User, UserMovimentDTO>().ReverseMap();
            
            CreateMap<Extract, ExtractDTO>().ReverseMap();
        }
    }
}