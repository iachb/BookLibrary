using AutoMapper;
using BookLibrary.Core.Entities;
using BookLibrary.Api.Models.Books;

namespace BookLibrary.Api.Mapper
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookDTO>().ReverseMap();
        }
    }
}
