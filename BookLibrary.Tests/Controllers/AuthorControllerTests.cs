using AutoMapper;
using BookLibrary.Api.Controllers;
using BookLibrary.Api.Models.Author;
using BookLibrary.Core.Interfaces;
using BookLibrary.Core.Models.Authors;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BookLibrary.Tests.Controllers
{
    public class AuthorControllerTests
    {
        [Fact]
        public async Task GetAllAuthors_ReturnsAuthorDTO()
        {
            // Arrange 
            var serviceMock = new Mock<IAuthorService>();
            var mapperMock = new Mock<IMapper>();
            var cancellationToken = new CancellationToken();

            var authorItems = new List<AuthorItem>
            {
                new AuthorItem { Id = 1, Name = "Author 1" },
                new AuthorItem { Id = 2, Name = "Author 2" }
            };

            var authorsDTO = new List<AuthorDTO>
            {
                new AuthorDTO { Id = 1, Name = "Author 1" },
                new AuthorDTO { Id = 2, Name = "Author 2" }
            };  

            serviceMock.Setup(s => s.GetAllAuthorsAsync(cancellationToken))
                .ReturnsAsync(authorItems);

            mapperMock
                .Setup(m => m.Map<IReadOnlyList<AuthorDTO>>(authorItems))
                .Returns(authorsDTO);

            var controller = new AuthorController(serviceMock.Object, mapperMock.Object);

            // Act
            var request = await controller.GetAllAuthors(cancellationToken);

            // Assert
            var okAuthorDTO = Assert.IsType<ActionResult<IReadOnlyList<AuthorDTO>>>(request);
            var ok = Assert.IsType<OkObjectResult>(okAuthorDTO.Result);
            Assert.Equal(200, ok.StatusCode);
            Assert.Equal(authorsDTO, ok.Value);

            serviceMock.Verify(s => s.GetAllAuthorsAsync(cancellationToken), Times.Once);
            mapperMock.Verify(m => m.Map<IReadOnlyList<AuthorDTO>>(authorItems), Times.Once);
        }
    }
}
