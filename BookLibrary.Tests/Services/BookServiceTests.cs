using Xunit;
using Moq;
using BookLibrary.Infrastructure.Services;
using BookLibrary.Core.Interfaces.Repository;
using BookLibrary.Core.Models.Books;
using BookLibrary.Core.Entities;
using AutoMapper;

namespace BookLibrary.Tests.Services
{
    public class BookServiceTests
    {
        [Fact]
        public async Task GetAllBooks_ReturnsBookItem()
        {
            // Arrange 
            var repositoryBookMock = new Mock<IBookRepository>();
            var mapperMock = new Mock<IMapper>();
            var mockAuthorRepository = new Mock<IAuthorRepository>();
            var cancellationToken = new CancellationToken();

            var tBooks = new List<TBook>
            {
                new TBook { Id = 1, Title = "Test Book", AuthorId = 1 },
                new TBook { Id = 2, Title = "Another Book", AuthorId = 2 }
            };
            
            var bookItems = new List<BookItem>
            {
                new BookItem { Id = 1, Title = "Test Book", AuthorId = 1 },
                new BookItem { Id = 2, Title = "Another Book", AuthorId = 2 }
            } as IReadOnlyList<BookItem>;

            repositoryBookMock.Setup(r => r.GetAllBooksAsync(cancellationToken))
                                  .ReturnsAsync(tBooks);
            
            mapperMock.Setup(m => m.Map<IReadOnlyList<BookItem>>(tBooks))
                      .Returns(bookItems);

            var bookService = new BookService(
                repositoryBookMock.Object,
                mapperMock.Object,
                mockAuthorRepository.Object
            );

            // Act 
            var result = await bookService.GetAllBooksAsync(cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bookItems, result);
            
            repositoryBookMock.Verify(r => r.GetAllBooksAsync(cancellationToken), Times.Once);
            mapperMock.Verify(m => m.Map<IReadOnlyList<BookItem>>(tBooks), Times.Once);
        }

        [Fact]
        public async Task GetBookById_ReturnsBookItem()
        {
            // Arrange
            var repositoryBookMock = new Mock<IBookRepository>();
            var mapperMock = new Mock<IMapper>();
            var mockAuthorRepository = new Mock<IAuthorRepository>();
            var cancellationToken = new CancellationToken();

            var tBook = new TBook { Id = 1, Title = "Test Book", AuthorId = 1 };
            var bookItem = new BookItem { Id = 1, Title = "Test Book", AuthorId = 1 };

            repositoryBookMock.Setup(r => r.GetBookByIdAsync(1, cancellationToken))
                              .ReturnsAsync(tBook);

            mapperMock.Setup(m => m.Map<BookItem>(tBook)).Returns(bookItem);

            var bookService = new BookService(
                repositoryBookMock.Object,
                mapperMock.Object,
                mockAuthorRepository.Object
            );

            // Act
            var result = await bookService.GetBookByIdAsync(1, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bookItem, result);
            repositoryBookMock.Verify(r => r.GetBookByIdAsync(1, cancellationToken), Times.Once);
            mapperMock.Verify(m => m.Map<BookItem>(tBook), Times.Once);
        }

        [Fact]
        public async Task GetBookById_ReturnsNull()
        {
            // Arrange
            var repositoryBookMock = new Mock<IBookRepository>();
            var mapperMock = new Mock<IMapper>();
            var mockAuthorRepository = new Mock<IAuthorRepository>();
            var cancellationToken = new CancellationToken();

            var tBook = new TBook { Id = 1, Title = "Test Book", AuthorId = 1 };
            var bookItem = new BookItem { Id = 1, Title = "Test Book", AuthorId = 1 };

            repositoryBookMock.Setup(r => r.GetBookByIdAsync(1, cancellationToken))
                              .ReturnsAsync((TBook?)null);

            var bookService = new BookService(
                repositoryBookMock.Object,
                mapperMock.Object,
                mockAuthorRepository.Object
            );

            // Act
            var result = await bookService.GetBookByIdAsync(1, cancellationToken);

            // Assert
            Assert.Null(result);
            repositoryBookMock.Verify(r => r.GetBookByIdAsync(1, cancellationToken), Times.Once);
            mapperMock.Verify(m => m.Map<BookItem>(tBook), Times.Never);
        }

        [Fact]
        public async Task CreateBook_ReturnsBookItem()
        {
            // Arrange
            var repositoryBookMock = new Mock<IBookRepository>();
            var mapperMock = new Mock<IMapper>();
            var repositoryAuthorMock = new Mock<IAuthorRepository>();
            var cancellationToken = new CancellationToken();

            var bookItem = new BookItem { Title = "New Book", AuthorId = 1, PublishedDate = DateTime.UtcNow };
            var tAuthor = new TAuthor { Id = 1, Name = "Author Name", Books = new List<TBook>() };
            var tBook = new TBook { Id = 0, Title = "New Book", AuthorId = 1 };
            var createdTBook = new TBook { Id = 10, Title = "New Book", AuthorId = 1, Author = tAuthor };
            var resultBookItem = new BookItem { Id = 10, Title = "New Book", AuthorId = 1 };

            // Setup: Author exists
            repositoryAuthorMock.Setup(a => a.GetAuthorByIdAsync(bookItem.AuthorId, cancellationToken))
                                .ReturnsAsync(tAuthor);

            // Setup: No existing book with same title
            repositoryBookMock.Setup(r => r.GetBookByTitleAsync("New Book", cancellationToken))
                              .ReturnsAsync((TBook?)null);

            // Setup: Map BookItem to TBook
            mapperMock.Setup(m => m.Map<TBook>(bookItem)).Returns(tBook);

            // Setup: Add, Save, LoadAuthor
            repositoryBookMock.Setup(r => r.AddBookAsync(It.IsAny<TBook>(), cancellationToken))
                              .Returns(Task.CompletedTask);

            repositoryBookMock.Setup(r => r.SaveChangesAsync(cancellationToken))
                              .Returns(Task.CompletedTask);

            repositoryBookMock.Setup(r => r.LoadAuthorAsync(It.IsAny<TBook>(), cancellationToken))
                              .Returns(Task.CompletedTask)
                              .Callback<TBook, CancellationToken>((book, ct) => book.Author = tAuthor);

            // Setup: Map TBook back to BookItem
            mapperMock.Setup(m => m.Map<BookItem>(It.Is<TBook>(b => b.Title == "New Book")))
                      .Returns(resultBookItem);

            var bookService = new BookService(
                repositoryBookMock.Object,
                mapperMock.Object,
                repositoryAuthorMock.Object
            );

            // Act
            var result = await bookService.CreateBookAsync(bookItem, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(resultBookItem.Title, result.Title);
            Assert.Equal(resultBookItem.AuthorId, result.AuthorId);

            repositoryAuthorMock.Verify(a => a.GetAuthorByIdAsync(bookItem.AuthorId, cancellationToken), Times.Once);
            repositoryBookMock.Verify(r => r.GetBookByTitleAsync("New Book", cancellationToken), Times.Once);
            repositoryBookMock.Verify(r => r.AddBookAsync(It.IsAny<TBook>(), cancellationToken), Times.Once);
            repositoryBookMock.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
            repositoryBookMock.Verify(r => r.LoadAuthorAsync(It.IsAny<TBook>(), cancellationToken), Times.Once);
            mapperMock.Verify(m => m.Map<TBook>(bookItem), Times.Once);
            mapperMock.Verify(m => m.Map<BookItem>(It.IsAny<TBook>()), Times.Once);
        }

        [Fact]
        public async Task CreateBook_ThrowsArgumentNullException_WhenBookIsNull()
        {
            // Arrange
            var repositoryBookMock = new Mock<IBookRepository>();
            var mapperMock = new Mock<IMapper>();
            var repositoryAuthorMock = new Mock<IAuthorRepository>();

            var bookService = new BookService(
                repositoryBookMock.Object,
                mapperMock.Object,
                repositoryAuthorMock.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                bookService.CreateBookAsync(null!, CancellationToken.None));
        }

        [Fact]
        public async Task CreateBook_ThrowsArgumentException_WhenTitleIsEmpty()
        {
            // Arrange
            var repositoryBookMock = new Mock<IBookRepository>();
            var mapperMock = new Mock<IMapper>();
            var repositoryAuthorMock = new Mock<IAuthorRepository>();

            var bookItem = new BookItem { Title = "   ", AuthorId = 1 };

            var bookService = new BookService(
                repositoryBookMock.Object,
                mapperMock.Object,
                repositoryAuthorMock.Object
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                bookService.CreateBookAsync(bookItem, CancellationToken.None));
            
            Assert.Contains("Title is required", exception.Message);
        }

        [Fact]
        public async Task CreateBook_ThrowsInvalidOperationException_WhenAuthorNotFound()
        {
            // Arrange
            var repositoryBookMock = new Mock<IBookRepository>();
            var mapperMock = new Mock<IMapper>();
            var repositoryAuthorMock = new Mock<IAuthorRepository>();
            var cancellationToken = CancellationToken.None;

            var bookItem = new BookItem { Title = "New Book", AuthorId = 999 };

            repositoryAuthorMock.Setup(a => a.GetAuthorByIdAsync(999, cancellationToken))
                                .ReturnsAsync((TAuthor?)null);

            var bookService = new BookService(
                repositoryBookMock.Object,
                mapperMock.Object,
                repositoryAuthorMock.Object
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                bookService.CreateBookAsync(bookItem, cancellationToken));
            
            Assert.Contains("Author with Id 999 not found", exception.Message);
        }

        [Fact]
        public async Task CreateBook_ThrowsInvalidOperationException_WhenBookTitleAlreadyExists()
        {
            // Arrange
            var repositoryBookMock = new Mock<IBookRepository>();
            var mapperMock = new Mock<IMapper>();
            var repositoryAuthorMock = new Mock<IAuthorRepository>();
            var cancellationToken = CancellationToken.None;

            var bookItem = new BookItem { Title = "Existing Book", AuthorId = 1 };
            var tAuthor = new TAuthor { Id = 1, Name = "Author", Books = new List<TBook>() };
            var existingBook = new TBook { Id = 5, Title = "Existing Book", AuthorId = 1 };

            repositoryAuthorMock.Setup(a => a.GetAuthorByIdAsync(1, cancellationToken))
                                .ReturnsAsync(tAuthor);

            repositoryBookMock.Setup(r => r.GetBookByTitleAsync("Existing Book", cancellationToken))
                              .ReturnsAsync(existingBook);

            var bookService = new BookService(
                repositoryBookMock.Object,
                mapperMock.Object,
                repositoryAuthorMock.Object
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                bookService.CreateBookAsync(bookItem, cancellationToken));
            
            Assert.Contains("already exists", exception.Message);
        }

        [Fact]
        public async Task UpdateBook_ReturnsBookItem()
        {
            // Arrange
            var repositoryBookMock = new Mock<IBookRepository>();
            var mapperMock = new Mock<IMapper>();
            var repositoryAuthorMock = new Mock<IAuthorRepository>();
            var cancellationToken = CancellationToken.None;
            int bookId = 1;

            var existingTBook = new TBook { Id = bookId, Title = "Old Title", AuthorId = 1 };
            var updatedBookItem = new BookItem { Id = bookId, Title = "Updated Title", AuthorId = 2, PublishedDate = DateTime.UtcNow };
            var author = new TAuthor { Id = 2, Name = "New Author", Books = new List<TBook>() };
            var resultBookItem = new BookItem { Id = bookId, Title = "Updated Title", AuthorId = 2 };

            repositoryBookMock.Setup(r => r.GetBookByIdAsync(bookId, cancellationToken))
                              .ReturnsAsync(existingTBook);

            repositoryBookMock.Setup(r => r.GetBookByTitleAsync("Updated Title", cancellationToken))
                              .ReturnsAsync((TBook?)null);

            repositoryAuthorMock.Setup(a => a.GetAuthorByIdAsync(2, cancellationToken))
                                .ReturnsAsync(author);

            repositoryBookMock.Setup(r => r.SaveChangesAsync(cancellationToken))
                              .Returns(Task.CompletedTask)
                              .Callback(() =>
                              {
                                  // Simulate ID assignment after save
                                  existingTBook.Id = bookId;
                              });

            repositoryBookMock.Setup(r => r.LoadAuthorAsync(existingTBook, cancellationToken))
                              .Returns(Task.CompletedTask)
                              .Callback<TBook, CancellationToken>((book, ct) => book.Author = author);

            mapperMock.Setup(m => m.Map<BookItem>(It.Is<TBook>(b => b.Id == bookId && b.Title == "Updated Title")))
                      .Returns(resultBookItem);

            var bookService = new BookService(
                repositoryBookMock.Object,
                mapperMock.Object,
                repositoryAuthorMock.Object
            );

            // Act
            var result = await bookService.UpdateBookAsync(bookId, updatedBookItem, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(resultBookItem.Title, result.Title);
            Assert.Equal(resultBookItem.AuthorId, result.AuthorId);
            
            // Verify the entity was updated
            Assert.Equal("Updated Title", existingTBook.Title);
            Assert.Equal(2, existingTBook.AuthorId);
            Assert.Equal(updatedBookItem.PublishedDate, existingTBook.PublishedDate);

            repositoryBookMock.Verify(r => r.GetBookByIdAsync(bookId, cancellationToken), Times.Once);
            repositoryBookMock.Verify(r => r.GetBookByTitleAsync("Updated Title", cancellationToken), Times.Once);
            repositoryAuthorMock.Verify(a => a.GetAuthorByIdAsync(2, cancellationToken), Times.Once);
            repositoryBookMock.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
            repositoryBookMock.Verify(r => r.LoadAuthorAsync(existingTBook, cancellationToken), Times.Once);
            mapperMock.Verify(m => m.Map<BookItem>(It.IsAny<TBook>()), Times.Once);
        }

        [Fact]
        public async Task UpdateBook_ThrowsArgumentNullException_WhenBookIsNull()
        {
            // Arrange
            var repositoryBookMock = new Mock<IBookRepository>();
            var mapperMock = new Mock<IMapper>();
            var repositoryAuthorMock = new Mock<IAuthorRepository>();

            var bookService = new BookService(
                repositoryBookMock.Object,
                mapperMock.Object,
                repositoryAuthorMock.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                bookService.UpdateBookAsync(1, null!, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateBook_ThrowsInvalidOperationException_WhenBookNotFound()
        {
            // Arrange
            var repositoryBookMock = new Mock<IBookRepository>();
            var mapperMock = new Mock<IMapper>();
            var repositoryAuthorMock = new Mock<IAuthorRepository>();
            var cancellationToken = CancellationToken.None;

            var bookItem = new BookItem { Id = 999, Title = "Title", AuthorId = 1 };

            repositoryBookMock.Setup(r => r.GetBookByIdAsync(999, cancellationToken))
                              .ReturnsAsync((TBook?)null);

            var bookService = new BookService(
                repositoryBookMock.Object,
                mapperMock.Object,
                repositoryAuthorMock.Object
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                bookService.UpdateBookAsync(999, bookItem, cancellationToken));

            Assert.Contains("No book found with ID '999'", exception.Message);
        }

        [Fact]
        public async Task UpdateBook_ThrowsInvalidOperationException_WhenTitleAlreadyExists()
        {
            // Arrange
            var repositoryBookMock = new Mock<IBookRepository>();
            var mapperMock = new Mock<IMapper>();
            var repositoryAuthorMock = new Mock<IAuthorRepository>();
            var cancellationToken = CancellationToken.None;

            var existingTBook = new TBook { Id = 1, Title = "Old Title", AuthorId = 1 };
            var anotherTBook = new TBook { Id = 2, Title = "New Title", AuthorId = 1 };
            var updatedBookItem = new BookItem { Id = 1, Title = "New Title", AuthorId = 1 };

            repositoryBookMock.Setup(r => r.GetBookByIdAsync(1, cancellationToken))
                              .ReturnsAsync(existingTBook);

            repositoryBookMock.Setup(r => r.GetBookByTitleAsync("New Title", cancellationToken))
                              .ReturnsAsync(anotherTBook);

            var bookService = new BookService(
                repositoryBookMock.Object,
                mapperMock.Object,
                repositoryAuthorMock.Object
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                bookService.UpdateBookAsync(1, updatedBookItem, cancellationToken));

            Assert.Contains("already exists", exception.Message);
        }

        [Fact]
        public async Task UpdateBook_ThrowsInvalidOperationException_WhenAuthorNotFound()
        {
            // Arrange
            var repositoryBookMock = new Mock<IBookRepository>();
            var mapperMock = new Mock<IMapper>();
            var repositoryAuthorMock = new Mock<IAuthorRepository>();
            var cancellationToken = CancellationToken.None;

            var existingTBook = new TBook { Id = 1, Title = "Title", AuthorId = 1 };
            var updatedBookItem = new BookItem { Id = 1, Title = "Title", AuthorId = 999 };

            repositoryBookMock.Setup(r => r.GetBookByIdAsync(1, cancellationToken))
                              .ReturnsAsync(existingTBook);

            repositoryBookMock.Setup(r => r.GetBookByTitleAsync("Title", cancellationToken))
                              .ReturnsAsync((TBook?)null);

            repositoryAuthorMock.Setup(a => a.GetAuthorByIdAsync(999, cancellationToken))
                                .ReturnsAsync((TAuthor?)null);

            var bookService = new BookService(
                repositoryBookMock.Object,
                mapperMock.Object,
                repositoryAuthorMock.Object
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                bookService.UpdateBookAsync(1, updatedBookItem, cancellationToken));

            Assert.Contains("Author with Id 999 not found", exception.Message);
        }
    }
}
