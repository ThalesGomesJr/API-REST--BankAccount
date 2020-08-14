using System.Linq;
using AutoMapper;
using WeBank.API.DTOs;
using WeBank.Domain.Models;

namespace WeBank.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            
            CreateMap<User, UserDTO>().ReverseMap();

            CreateMap<Extract, ExtractDTO>().ReverseMap();

            CreateMap<User, UserLoginDTO>().ReverseMap();
        }
    }
}