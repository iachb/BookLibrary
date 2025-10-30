using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Core.Entities
{
    public class TAuthor
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime? BirthDate { get; set; }
        public ICollection<TBook> Books { get; set; } = new List<TBook>();
    }
}
