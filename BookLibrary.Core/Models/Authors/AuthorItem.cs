using BookLibrary.Core.Models.Books;
using System.ComponentModel;

namespace BookLibrary.Core.Models.Authors
{
    public class AuthorItem
    {
        [ReadOnly(true)]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime? BirthDate { get; set; }
        public ICollection<BookItem> Books { get; set; } = new List<BookItem>();
    }
}
