using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Core.Entities
{
    public class TBook
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }

        // Foreign key property
        public int AuthorId { get; set; }

        // Navigation property
        public TAuthor Author { get; set; } = null!;
    }
}

