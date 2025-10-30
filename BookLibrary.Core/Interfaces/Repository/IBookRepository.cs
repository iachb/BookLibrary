using BookLibrary.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Core.Interfaces.Repository
{
    public interface IBookRepository
    {
        public Task<Book?> GetBookByTitleAsync(string name, CancellationToken cancellationToken = default);
        public Task<Book?> GetBookByIdAsync(int id, CancellationToken cancellationToken = default);
        public Task<Book> AddBookAsync(Book book, CancellationToken cancellationToken = default);
        public Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
