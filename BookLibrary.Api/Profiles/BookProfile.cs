using AutoMapper;
using BookLibrary.Core.Entities;
using BookLibrary.Api.Models.Books;

namespace BookLibrary.Api.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<TBook, BookDTO>().ReverseMap();
        }
    }
}
