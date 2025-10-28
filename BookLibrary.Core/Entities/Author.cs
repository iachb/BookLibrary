using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Core.Entities
{
    public class Author
    {
        public int PkId { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }
        public DateTime? BirthDate { get; set; }
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
