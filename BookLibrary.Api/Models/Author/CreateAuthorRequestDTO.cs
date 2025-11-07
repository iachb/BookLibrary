using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Api.Models.Author
{
    public class CreateAuthorRequestDTO
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public DateTime? BirthDate { get; set; }
    }
}
