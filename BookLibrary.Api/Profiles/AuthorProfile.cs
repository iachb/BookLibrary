using AutoMapper;
using BookLibrary.Api.Models.Author;
using BookLibrary.Core.Models.Authors;

namespace BookLibrary.Api.Profiles
{
    public class AuthorProfile : Profile
    {
        public AuthorProfile()
        {
            CreateMap<AuthorItem, AuthorDTO>().ReverseMap();
            CreateMap<AuthorItem, CreateAuthorRequestDTO>().ReverseMap();
        }
    }
}
