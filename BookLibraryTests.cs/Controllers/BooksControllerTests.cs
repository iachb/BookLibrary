using AutoMapper;
using BookLibrary.Api.Controllers;
using BookLibrary.Api.Models.Books;
using BookLibrary.Core.Interfaces;
using BookLibrary.Core.Models.Books;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BookLibrary.Tests.Controllers
{
    public class BooksControllerTests
    {
        [Fact]
        public async Task GetAllBooks_ReturnsMappedDTOs()
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
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, ok.StatusCode);
            Assert.Equal(mappedDtos, ok.Value);

            serviceMock.Verify(s => s.GetAllBooksAsync(cancellationToken), Times.Once);
            mapperMock.Verify(m => m.Map<IReadOnlyList<BookDTO>>(bookItems), Times.Once);
        }

        [Fact]
        public async Task GetBook_ReturnMappedDTO()
        {
            // Arrange
            var serviceMock = new Mock<IBookService>();
            var mapperMock = new Mock<IMapper>();
            var cancellationToken = CancellationToken.None;

            var bookItem = new BookItem { Id = 1, Title = "Title1" };
            var mappedDTO = new BookDTO { Id = 1, Title = "Title1" };

            serviceMock
                .Setup(s => s.GetBookByIdAsync(bookItem.Id, cancellationToken))
                .ReturnsAsync(bookItem);

            mapperMock
                .Setup(m => m.Map<BookDTO>(bookItem))
                .Returns(mappedDTO);

            var controller = new BooksController(serviceMock.Object, mapperMock.Object);

            // Act
            var result = await controller.GetBook(bookItem.Id, cancellationToken);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, ok.StatusCode);

            var dto = Assert.IsType<BookDTO>(ok.Value);
            Assert.Equal(mappedDTO.Id, dto.Id);
        }

        [Fact]
        public async Task GetBook_ReturnNotFound()
        {
            // Arrange
            var serviceMock = new Mock<IBookService>();
            var mapperMock = new Mock<IMapper>();
            var cancellationToken = CancellationToken.None;
            var bookIdInRoute = 2;

            serviceMock
                .Setup(s => s.GetBookByIdAsync(bookIdInRoute, cancellationToken))
                .ReturnsAsync((BookItem?)null);

            var controller = new BooksController(serviceMock.Object, mapperMock.Object);

            // Act
            var result = await controller.GetBook(bookIdInRoute, cancellationToken);

            // Assert
            var notFound = Assert.IsType<NotFoundResult>(result.Result);
            Assert.Equal(404, notFound.StatusCode);
        }
    }
}
