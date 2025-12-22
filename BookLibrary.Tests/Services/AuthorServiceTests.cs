using AutoMapper;
using BookLibrary.Core.Entities;
using BookLibrary.Core.Interfaces.Repository;
using BookLibrary.Core.Models.Authors;
using BookLibrary.Core.Models.Books;
using BookLibrary.Infrastructure.Services;
using Moq;
using System.Net;
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
        public async Task GetAuthorByIdAsync_ReturnsAuthorItem()
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
        public async Task GetAuthorByIdAsync_ReturnsNull()
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
        public async Task CreateAuthorAsync_ReturnsAuthorItem()
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

        [Fact]
        public async Task CreateAuthorAsync_NullItem_ReturnsArgumentNullException()
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            var authorService = new AuthorService(_repositoryAuthorMock.Object, _mapperMock.Object);

            // Act & Assert
            var result = await Assert.ThrowsAsync<ArgumentNullException>(() => authorService.CreateAuthorAsync(item: null!, cancellationToken));

            _mapperMock.Verify(m => m.Map<TAuthor>(It.IsAny<AuthorItem>()), Times.Never);
            _repositoryAuthorMock.Verify(r => r.GetAuthorByNameAsync(It.IsAny<string>(), cancellationToken), Times.Never);
            _repositoryAuthorMock.Verify(r => r.CreateAuthorAsync(It.IsAny<TAuthor>(), cancellationToken), Times.Never);
            _repositoryAuthorMock.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Never);
            _mapperMock.Verify(m => m.Map<AuthorItem>(It.IsAny<TAuthor>()), Times.Never);
        }

        [Fact]
        public async Task CreateAuthorAsync_NameIsNullOrWhiteSpace_ReturnsArgumentException()
        {
            // Arrange
            var cancellationToken = new CancellationToken();

            var authorItem = new AuthorItem
            {
                Id = 1,
                Name = "",
                BirthDate = DateTime.UtcNow,
            };

            var authorService = new AuthorService(_repositoryAuthorMock.Object, _mapperMock.Object);

            // Act & Assert
            var result = await Assert.ThrowsAsync<ArgumentException>(() => authorService.CreateAuthorAsync(authorItem, cancellationToken));

            Assert.Contains("An author name is required.", result.Message);

            _mapperMock.Verify(m => m.Map<TAuthor>(It.IsAny<AuthorItem>()), Times.Never);
            _repositoryAuthorMock.Verify(r => r.GetAuthorByNameAsync(It.IsAny<string>(), cancellationToken), Times.Never);
            _repositoryAuthorMock.Verify(r => r.CreateAuthorAsync(It.IsAny<TAuthor>(), cancellationToken), Times.Never);
            _repositoryAuthorMock.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Never);
            _mapperMock.Verify(m => m.Map<AuthorItem>(It.IsAny<TAuthor>()), Times.Never);
        }

        [Fact]
        public async Task CreateAuthorAsync_AuthorExists_ReturnsInvalidOperationException()
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

            _mapperMock.Setup(m => m.Map<TAuthor>(authorItem))
                .Returns(tAuthor);
            _repositoryAuthorMock.Setup(a => a.GetAuthorByNameAsync(tAuthor.Name.Trim(), cancellationToken))
                .ReturnsAsync(tAuthor);

            var authorService = new AuthorService(_repositoryAuthorMock.Object, _mapperMock.Object);

            // Act & Assert
            var result = await Assert.ThrowsAsync<InvalidOperationException>(() => authorService.CreateAuthorAsync(authorItem));
            Assert.Contains("already exists", result.Message);

            _mapperMock.Verify(m => m.Map<TAuthor>(It.IsAny<AuthorItem>()), Times.Once);
            _repositoryAuthorMock.Verify(r => r.GetAuthorByNameAsync(It.IsAny<string>(), cancellationToken), Times.Once);
            _repositoryAuthorMock.Verify(r => r.CreateAuthorAsync(It.IsAny<TAuthor>(), cancellationToken), Times.Never);
            _repositoryAuthorMock.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Never);
            _mapperMock.Verify(m => m.Map<AuthorItem>(It.IsAny<TAuthor>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAuthor_ReturnsAuthorItem()
        {
            // Arrange 
            var cancellationToken = new CancellationToken();
            int authorId = 1;
            var authorItem = new AuthorItem
            {
                Id = 1,
                Name = "UpdatedAuthorName",
                BirthDate = DateTime.UtcNow
            };

            var tAuthor = new TAuthor
            {
                Id = 1,
                Name = "AuthorName",
                BirthDate = DateTime.UtcNow
            };

            _repositoryAuthorMock.Setup(a => a.GetAuthorByIdAsync(authorId, cancellationToken))
                .ReturnsAsync(tAuthor);
            _repositoryAuthorMock.Setup(a => a.GetAuthorByNameAsync(authorItem.Name, cancellationToken))
                .ReturnsAsync((TAuthor?)null);

            _repositoryAuthorMock.Setup(r => r.SaveChangesAsync(cancellationToken))
                              .Returns(Task.FromResult(1))
                              .Callback(() =>
                              {
                                  tAuthor.Name = authorItem.Name;
                                  tAuthor.BirthDate = authorItem.BirthDate;
                              });

            _mapperMock.Setup(m => m.Map<AuthorItem>(It.Is<TAuthor>(a => a.Id == authorId && a.Name == "UpdatedAuthorName")))
                      .Returns(authorItem);
            var authorService = new AuthorService(_repositoryAuthorMock.Object, _mapperMock.Object);

            // Act 
            var request = await authorService.UpdateAuthorAsync(authorId, authorItem, cancellationToken);

            // Assert
            Assert.NotNull(request);
            Assert.Equal(request.Name, authorItem.Name);
            Assert.Equal(request.Id, authorItem.Id);

            _repositoryAuthorMock.Verify(a => a.GetAuthorByIdAsync(authorId, cancellationToken), Times.Once);
            _repositoryAuthorMock.Verify(a => a.GetAuthorByNameAsync(authorItem.Name, cancellationToken), Times.Once);
            _repositoryAuthorMock.Verify(a => a.SaveChangesAsync(cancellationToken), Times.Once);
            _mapperMock.Verify(m => m.Map<AuthorItem>(It.IsAny<TAuthor>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAuthor_InvalidId_ThrowsArgumentOutOfRangeException()
        {
            var cancellationToken = new CancellationToken();
            var authorService = new AuthorService(_repositoryAuthorMock.Object, _mapperMock.Object);

            var ex = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                authorService.UpdateAuthorAsync(0, new AuthorItem { Id = 0, Name = "Name", BirthDate = DateTime.UtcNow }, cancellationToken));

            Assert.Contains("greater than zero", ex.Message);
            _repositoryAuthorMock.Verify(a => a.GetAuthorByIdAsync(It.IsAny<int>(), cancellationToken), Times.Never);
            _repositoryAuthorMock.Verify(a => a.GetAuthorByNameAsync(It.IsAny<string>(), cancellationToken), Times.Never);
            _repositoryAuthorMock.Verify(a => a.SaveChangesAsync(cancellationToken), Times.Never);
            _mapperMock.Verify(m => m.Map<AuthorItem>(It.IsAny<TAuthor>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAuthor_NullItem_ThrowsArgumentNullException()
        {
            var cancellationToken = new CancellationToken();
            var authorService = new AuthorService(_repositoryAuthorMock.Object, _mapperMock.Object);

            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                authorService.UpdateAuthorAsync(1, null!, cancellationToken));

            _repositoryAuthorMock.Verify(a => a.GetAuthorByIdAsync(It.IsAny<int>(), cancellationToken), Times.Never);
            _repositoryAuthorMock.Verify(a => a.GetAuthorByNameAsync(It.IsAny<string>(), cancellationToken), Times.Never);
            _repositoryAuthorMock.Verify(a => a.SaveChangesAsync(cancellationToken), Times.Never);
            _mapperMock.Verify(m => m.Map<AuthorItem>(It.IsAny<TAuthor>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAuthor_NotFound_ThrowsKeyNotFoundException()
        {
            var cancellationToken = new CancellationToken();
            int authorId = 5;
            var authorItem = new AuthorItem { Id = 5, Name = "SomeName", BirthDate = DateTime.UtcNow };

            _repositoryAuthorMock.Setup(a => a.GetAuthorByIdAsync(authorId, cancellationToken))
                .ReturnsAsync((TAuthor?)null);

            var authorService = new AuthorService(_repositoryAuthorMock.Object, _mapperMock.Object);

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                authorService.UpdateAuthorAsync(authorId, authorItem, cancellationToken));

            Assert.Contains("not found", ex.Message);
            _repositoryAuthorMock.Verify(a => a.GetAuthorByIdAsync(authorId, cancellationToken), Times.Once);
            _repositoryAuthorMock.Verify(a => a.GetAuthorByNameAsync(It.IsAny<string>(), cancellationToken), Times.Never);
            _repositoryAuthorMock.Verify(a => a.SaveChangesAsync(cancellationToken), Times.Never);
            _mapperMock.Verify(m => m.Map<AuthorItem>(It.IsAny<TAuthor>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAuthor_DuplicateName_ThrowsInvalidOperationException()
        {
            var cancellationToken = new CancellationToken();
            int authorId = 3;
            var authorItem = new AuthorItem { Id = 3, Name = "NewName", BirthDate = DateTime.UtcNow };
            var existingAuthor = new TAuthor { Id = 3, Name = "OldName", BirthDate = DateTime.UtcNow };
            var otherAuthor = new TAuthor { Id = 99, Name = "NewName", BirthDate = DateTime.UtcNow };

            _repositoryAuthorMock.Setup(a => a.GetAuthorByIdAsync(authorId, cancellationToken))
                .ReturnsAsync(existingAuthor);
            _repositoryAuthorMock.Setup(a => a.GetAuthorByNameAsync(authorItem.Name, cancellationToken))
                .ReturnsAsync(otherAuthor);

            var authorService = new AuthorService(_repositoryAuthorMock.Object, _mapperMock.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                authorService.UpdateAuthorAsync(authorId, authorItem, cancellationToken));

            Assert.Contains("already exists", ex.Message);
            _repositoryAuthorMock.Verify(a => a.GetAuthorByIdAsync(authorId, cancellationToken), Times.Once);
            _repositoryAuthorMock.Verify(a => a.GetAuthorByNameAsync(authorItem.Name, cancellationToken), Times.Once);
            _repositoryAuthorMock.Verify(a => a.SaveChangesAsync(cancellationToken), Times.Never);
            _mapperMock.Verify(m => m.Map<AuthorItem>(It.IsAny<TAuthor>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAuthor_InvalidId_ThrowsArgumentOutOfRangeException()
        {
            var cancellationToken = new CancellationToken();
            var authorService = new AuthorService(_repositoryAuthorMock.Object, _mapperMock.Object);

            var ex = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                authorService.DeleteAuthorAsync(0, cancellationToken));

            Assert.Contains("greater than zero", ex.Message);
            _repositoryAuthorMock.Verify(a => a.GetAuthorByIdAsync(It.IsAny<int>(), cancellationToken), Times.Never);
            _repositoryAuthorMock.Verify(a => a.DeleteAuthorByIdAsync(It.IsAny<int>(), cancellationToken), Times.Never);
            _repositoryAuthorMock.Verify(a => a.SaveChangesAsync(cancellationToken), Times.Never);
        }

        [Fact]
        public async Task DeleteAuthor_NotFound_ThrowsKeyNotFoundException()
        {
            var cancellationToken = new CancellationToken();
            int authorId = 10;
            _repositoryAuthorMock.Setup(a => a.GetAuthorByIdAsync(authorId, cancellationToken))
                .ReturnsAsync((TAuthor?)null);

            var authorService = new AuthorService(_repositoryAuthorMock.Object, _mapperMock.Object);

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                authorService.DeleteAuthorAsync(authorId, cancellationToken));

            Assert.Contains("not found", ex.Message);
            _repositoryAuthorMock.Verify(a => a.GetAuthorByIdAsync(authorId, cancellationToken), Times.Once);
            _repositoryAuthorMock.Verify(a => a.DeleteAuthorByIdAsync(It.IsAny<int>(), cancellationToken), Times.Never);
            _repositoryAuthorMock.Verify(a => a.SaveChangesAsync(cancellationToken), Times.Never);
        }

        [Fact]
        public async Task DeleteAuthor_Success_DeletesAndSaves()
        {
            var cancellationToken = new CancellationToken();
            int authorId = 7;
            var existingAuthor = new TAuthor { Id = authorId, Name = "Author", BirthDate = DateTime.UtcNow };

            _repositoryAuthorMock.Setup(a => a.GetAuthorByIdAsync(authorId, cancellationToken))
                .ReturnsAsync(existingAuthor);
            _repositoryAuthorMock.Setup(a => a.DeleteAuthorByIdAsync(authorId, cancellationToken))
                .Returns(Task.CompletedTask);
            _repositoryAuthorMock.Setup(a => a.SaveChangesAsync(cancellationToken))
                .ReturnsAsync(1);

            var authorService = new AuthorService(_repositoryAuthorMock.Object, _mapperMock.Object);

            await authorService.DeleteAuthorAsync(authorId, cancellationToken);

            _repositoryAuthorMock.Verify(a => a.GetAuthorByIdAsync(authorId, cancellationToken), Times.Once);
            _repositoryAuthorMock.Verify(a => a.DeleteAuthorByIdAsync(authorId, cancellationToken), Times.Once);
            _repositoryAuthorMock.Verify(a => a.SaveChangesAsync(cancellationToken), Times.Once);
        }
    }
}
