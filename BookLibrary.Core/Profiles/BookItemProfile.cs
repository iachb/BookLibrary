using AutoMapper;
using BookLibrary.Core.Entities;
using BookLibrary.Core.Models.Books;

namespace BookLibrary.Core.Profiles
{
    public class BookItemProfile : Profile
    {
        public BookItemProfile()
        {
            CreateMap<TBook, BookItem>().ReverseMap();
        }
    }
}
