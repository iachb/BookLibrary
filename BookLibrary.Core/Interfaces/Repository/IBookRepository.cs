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
        public Task<TBook?> GetBookByTitleAsync(string name, CancellationToken cancellationToken = default);
        public Task<TBook?> GetBookByIdAsync(int id, CancellationToken cancellationToken = default);
        public Task<TBook> AddBookAsync(TBook book, CancellationToken cancellationToken = default);
        public Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
