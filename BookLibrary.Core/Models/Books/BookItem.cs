using BookLibrary.Core.Models.Authors;
using System.ComponentModel;

namespace BookLibrary.Core.Models.Books
{
    public class BookItem
    {
        [ReadOnly(true)]
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public DateTime PublishedDate { get; set; }
        [ReadOnly(true)]
        public int AuthorId { get; set; }
        public AuthorItem AuthorName { get; set; } = null!;
    }
}
