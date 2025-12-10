using AutoMapper;
using BookLibrary.Core.Entities;
using BookLibrary.Core.Interfaces.Repository;
using BookLibrary.Core.Models.Authors;
using BookLibrary.Core.Models.Books;
using BookLibrary.Infrastructure.Services;
using Moq;
using System;
using System.Xml.Linq;
using Xunit;
using static System.Reflection.Metadata.BlobBuilder;

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
    }
}
