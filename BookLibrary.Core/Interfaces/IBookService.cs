using BookLibrary.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Core.Interfaces
{
    public interface IBookService
    {
        public Task<TBook> CreateBookAsync(TBook book, CancellationToken cancellationToken = default);
    }
}
