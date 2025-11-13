using AutoMapper;
using BookLibrary.Api.Controllers;
using BookLibrary.Core.Interfaces;
using BookLibrary.Core.Models.Books;
using BookLibrary.Api.Models.Books;
using Moq;
using Xunit;

namespace BookLibrary.Tests.Controllers
{
    public class BooksControllerTests
    {
        [Fact]
        public async Task GetAllBooks_ReturnsMappedDtos()
        {
            // Arrange
            var serviceMock = new Mock<IBookService>();
            var mapperMock = new Mock<IMapper>();
            var cancellationToken = CancellationToken.None;

            var bookItems = new List<BookItem>
            {
                new BookItem { Id =1, Title = "Title1" },
                new BookItem { Id =2, Title = "Title2" }
            } as IReadOnlyList<BookItem>;

            var mappedDtos = new List<BookDTO>
            {
                new BookDTO { Id =1, Title = "Title1" },
                new BookDTO { Id =2, Title = "Title2" }
            } as IReadOnlyList<BookDTO>;

            serviceMock
                .Setup(s => s.GetAllBooksAsync(cancellationToken))
                .ReturnsAsync(bookItems);

            mapperMock
                .Setup(m => m.Map<IReadOnlyList<BookDTO>>(bookItems))
                .Returns(mappedDtos);

            var controller = new BooksController(serviceMock.Object, mapperMock.Object);

            // Act
            var result = await controller.GetAllBooks(cancellationToken);

            // Assert
            Assert.Equal(mappedDtos, result);
            serviceMock.Verify(s => s.GetAllBooksAsync(cancellationToken), Times.Once);
            mapperMock.Verify(m => m.Map<IReadOnlyList<BookDTO>>(bookItems), Times.Once);
        }
    }
}
