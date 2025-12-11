using AutoMapper;
using BookLibrary.Core.Entities;
using BookLibrary.Core.Interfaces.Repository;
using BookLibrary.Core.Models.Authors;
using BookLibrary.Core.Models.Books;
using BookLibrary.Infrastructure.Services;
using Moq;
using System.Xml.Linq;
using Xunit;

namespace BookLibrary.Tests.Services
{
    public class AuthorServiceTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IAuthorRepository> _repositoryAuthorMock;
        public AuthorServiceTests()
        {
            _repositoryAuthorMock = new Mock<IAuthorRepository>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task GetAllAuthorsAsync_ReturnsIReadOnlyList()
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            var authorList = new List<TAuthor>
            {
                new TAuthor {
                    Id = 1, 
                    Name = "AuthorName", 
                    BirthDate = DateTime.Now, 
                    Books = {
                        new TBook { 
                            Id = 1, 
                            Title = "BookName" } 
                    } 
                },
                new TAuthor {
                    Id = 2,
                    Name = "AnotherAuthorName",
                    BirthDate = DateTime.Now,
                    Books = {
                        new TBook {
                            Id = 2,
                            Title = "AnotherBookName" }
                    }
                },
            };

            var authorListItem = new List<AuthorItem>
            {
                new AuthorItem {
                    Id = 1,
                    Name = "AuthorName",
                    BirthDate = DateTime.Now,
                    Books = {
                        new BookItem {
                            Id = 1,
                            Title = "BookName" }
                    }
                },
                new AuthorItem {
                    Id = 2,
                    Name = "AnotherAuthorName",
                    BirthDate = DateTime.Now,
                    Books = {
                        new BookItem {
                            Id = 2,
                            Title = "AnotherBookName" }
                    }
                },
            } as IReadOnlyList<AuthorItem>;

            _repositoryAuthorMock.Setup(a => a.GetAllAuthorsAsync(cancellationToken))
                .ReturnsAsync(authorList);
            _mapperMock.Setup(m => m.Map<IReadOnlyList<AuthorItem>>(authorList))
                .Returns(authorListItem);

            var authorService = new AuthorService(_repositoryAuthorMock.Object, _mapperMock.Object);

            // Act
            var result = await authorService.GetAllAuthorsAsync(cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(authorListItem, result);
            _repositoryAuthorMock.Verify(r => r.GetAllAuthorsAsync(cancellationToken), Times.Once);
            _mapperMock.Verify(m => m.Map<IReadOnlyList<AuthorItem>>(authorList), Times.Once);
        }

        [Fact]
        public async Task GetAuthorById_ReturnsAuthorItem()
        {
            // Arrange
            var cancellationToken = new CancellationToken();
            int authorId = 1;

            var authorItem = new AuthorItem
            {
                Id = 1,
                Name = "AuthorName",
                BirthDate = DateTime.Now,
                Books = new List<BookItem>
                {
                    new BookItem
                    {
                        Id = 1,
                        Title = "BookName"
                    },
                    new BookItem
                    {
                        Id = 2,
                        Title = "OtherBook"
                    }
                }
            };

            var tAuthor = new TAuthor
            {
                Id = 1,
                Name = "AuthorName",
                BirthDate = DateTime.Now,
                Books = new List<TBook>
                {
                    new TBook
                    {
                        Id = 1,
                        Title = "BookName"
                    },
                    new TBook
                    {
                        Id = 2,
                        Title = "OtherBook"
                    }
                }
            };

            _repositoryAuthorMock.Setup(a => a.GetAuthorByIdAsync(authorId, cancellationToken))
                .ReturnsAsync(tAuthor);
            _mapperMock.Setup(m => m.Map<AuthorItem>(tAuthor))
                .Returns(authorItem);

            var authorService = new AuthorService(_repositoryAuthorMock.Object, _mapperMock.Object);

            // Act
            var request = await authorService.GetAuthorByIdAsync(authorId, cancellationToken);

            // Assert
            Assert.NotNull(request);
            Assert.Equal(authorItem, request);
            _repositoryAuthorMock.Verify(r => r.GetAuthorByIdAsync(authorId, cancellationToken), Times.Once);
            _mapperMock.Verify(m => m.Map<AuthorItem>(tAuthor), Times.Once);
        }

        [Fact]
        public async Task GetAuthorById_ReturnsNull()
        {
            // Arrange
            var cancellationToken = new CancellationToken();
            var authorId = 999;

            _repositoryAuthorMock.Setup(a => a.GetAuthorByIdAsync(authorId, cancellationToken))
                .ReturnsAsync((TAuthor?)null);

            var authorService = new AuthorService(_repositoryAuthorMock.Object, _mapperMock.Object);

            // Act & Assert
            var request = await Assert.ThrowsAsync<KeyNotFoundException>(() => authorService.GetAuthorByIdAsync(authorId, cancellationToken));
            Assert.Contains("not found", request.Message);
            _repositoryAuthorMock.Verify(r => r.GetAuthorByIdAsync(authorId, cancellationToken), Times.Once);
            _mapperMock.Verify(m => m.Map<AuthorItem>(It.IsAny<TBook>()), Times.Never);
        }

        [Fact]
        public async Task CreateAuthor_ReturnsAuthorItem()
        {
            // Arrange 
            var cancellationToken = new CancellationToken();
            
            var authorItem = new AuthorItem
            {
                Name = "AuthorName",
                BirthDate = DateTime.Now
            };
            
            var tAuthor = new TAuthor
            {
                Id = 0,
                Name = "AuthorName",
                BirthDate = authorItem.BirthDate
            };
            
            var createdTAuthor = new TAuthor
            {
                Id = 1,
                Name = "AuthorName",
                BirthDate = authorItem.BirthDate
            };
            
            var resultAuthorItem = new AuthorItem
            {
                Id = 1,
                Name = "AuthorName",
                BirthDate = authorItem.BirthDate
            };

            _mapperMock.Setup(m => m.Map<TAuthor>(authorItem))
                .Returns(tAuthor);
            _repositoryAuthorMock.Setup(a => a.GetAuthorByNameAsync(tAuthor.Name, cancellationToken))
                .ReturnsAsync((TAuthor?)null);
            _repositoryAuthorMock.Setup(a => a.CreateAuthorAsync(tAuthor, cancellationToken))
                .ReturnsAsync(createdTAuthor);
            _repositoryAuthorMock.Setup(a => a.SaveChangesAsync(cancellationToken))
                .ReturnsAsync(0);
            _mapperMock.Setup(m => m.Map<AuthorItem>(createdTAuthor))
                .Returns(resultAuthorItem);

            var authorService = new AuthorService(_repositoryAuthorMock.Object, _mapperMock.Object);

            // Act
            var result = await authorService.CreateAuthorAsync(authorItem, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(resultAuthorItem.Id, result.Id);
            Assert.Equal(resultAuthorItem.Name, result.Name);
            
            _mapperMock.Verify(m => m.Map<TAuthor>(authorItem), Times.Once);
            _repositoryAuthorMock.Verify(r => r.GetAuthorByNameAsync(tAuthor.Name, cancellationToken), Times.Once);
            _repositoryAuthorMock.Verify(r => r.CreateAuthorAsync(tAuthor, cancellationToken), Times.Once);
            _repositoryAuthorMock.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
            _mapperMock.Verify(m => m.Map<AuthorItem>(createdTAuthor), Times.Once);
        }

    }
}
