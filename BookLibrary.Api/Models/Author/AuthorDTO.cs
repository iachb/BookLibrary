using BookLibrary.Core.Entities;

namespace BookLibrary.Api.Models.Author
{
    public class AuthorDTO
    {
        public int Id { get; set;  }
        public string Name { get; set; } = null!;
        public DateTime? BirthDate { get; set; }
        public List<string> BookTitles { get; set; } = new();
    }
}
