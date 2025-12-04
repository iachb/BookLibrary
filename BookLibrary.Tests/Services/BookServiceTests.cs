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
    }
}
