using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Api.Models.Books
{
    public sealed class BookDTO
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public DateTime PublishedDate { get; set; }
        [Required]
        public string? AuthorName { get; set; } = string.Empty;
    }
}
