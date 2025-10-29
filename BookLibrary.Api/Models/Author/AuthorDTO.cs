using BookLibrary.Core.Entities;

namespace BookLibrary.Api.Models.Author
{
    public class AuthorDTO
    {
        public int Id { get; init;  }
        public string Name { get; init; } = null!;
        public DateTime? BirthDate { get; init; }
        public List<string> BookTitles { get; set; } = new();
    }
}
