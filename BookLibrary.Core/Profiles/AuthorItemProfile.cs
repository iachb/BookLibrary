using AutoMapper;
using BookLibrary.Core.Entities;
using BookLibrary.Core.Models.Authors;

namespace BookLibrary.Core.Profiles
{
    public class AuthorItemProfile: Profile
    {
        public AuthorItemProfile()
        {
            CreateMap<TAuthor, AuthorItem>().ReverseMap();
        }
    }
}
