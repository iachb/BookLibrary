using AutoMapper;
using BookLibrary.Core.Entities;
using BookLibrary.Core.Models.Books;

namespace BookLibrary.Core.Profiles
{
    public class BooksProfile : Profile
    {
        public BooksProfile()
        {
            CreateMap<TBook, BookItem>().ReverseMap();
        }
    }
}
