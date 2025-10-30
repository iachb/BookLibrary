using BookLibrary.Core.Entities;
using BookLibrary.Infrastructure.Data;
using BookLibrary.Core.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Infrastructure.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;
        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Book?> GetBookByTitleAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Books.FirstOrDefaultAsync(b => b.Title == name, cancellationToken);
        }

        public async Task<Book?> GetBookByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Books.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public async Task<Book> AddBookAsync(Book book, CancellationToken cancellationToken = default)
        {
            await _context.Books.AddAsync(book, cancellationToken);
            return book;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}