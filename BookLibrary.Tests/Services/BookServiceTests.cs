using Xunit;
using Moq;
using BookLibrary.Infrastructure.Services;
using BookLibrary.Core.Interfaces.Repository;
using BookLibrary.Core.Models.Books;
using BookLibrary.Core.Entities;

namespace BookLibrary.Tests.Services
{
    public class BookServiceTests
    {
        [Fact]
        public async Task GetAllBooks_ReturnsBookItem()
        {
            // Arrange 
            var repositoryBookMock = new Mock<IBookRepository>();
            var mapperMock = new Mock<AutoMapper.IMapper>();
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
            var mapperMock = new Mock<AutoMapper.IMapper>();
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
    }
}
