using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Api.Models.Books
{
    public class UpdateBookDTO
    {
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public DateTime PublishedDate { get; set; }
        [Required]
        public int? AuthorId { get; set; } = null!;
    }
}
