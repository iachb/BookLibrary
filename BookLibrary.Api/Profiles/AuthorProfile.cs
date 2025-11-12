using AutoMapper;
using BookLibrary.Api.Models.Author;
using BookLibrary.Core.Models.Authors;

namespace BookLibrary.Api.Profiles
{
    public class AuthorProfile : Profile
    {
        public AuthorProfile()
        {
            CreateMap<AuthorItem, AuthorDTO>()
                .ForMember(dest => dest.BookTitles, opt => opt.MapFrom(src => src.Books.Select(b => b.Title).ToList()))
                .ReverseMap();
            CreateMap<AuthorItem, CreateAuthorRequestDTO>().ReverseMap();
            CreateMap<AuthorItem, UpdateAuthorDTO>().ReverseMap();
        }
    }
}
