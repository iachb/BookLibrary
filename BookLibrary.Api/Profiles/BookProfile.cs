using AutoMapper;
using BookLibrary.Api.Models.Books;
using BookLibrary.Core.Models.Books;

namespace BookLibrary.Api.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<BookItem, CreateBookRequestDTO>().ReverseMap();
            CreateMap<BookItem, BookDTO>().ReverseMap();
            CreateMap<BookItem, UpdateBookDTO>().ReverseMap();
        }
    }
}
