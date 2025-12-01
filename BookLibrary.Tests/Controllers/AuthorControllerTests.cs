using AutoMapper;
using BookLibrary.Api.Controllers;
using BookLibrary.Api.Models.Author;
using BookLibrary.Core.Interfaces;
using BookLibrary.Core.Models.Authors;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
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

            serviceMock.Verify(s => s.GetAllAuthorsAsync(cancellationToken), Times.Once);
            mapperMock.Verify(m => m.Map<IReadOnlyList<AuthorDTO>>(authorItems), Times.Once);
        }

        [Fact]
        public async Task GetAuthor_ReturnAuthorDTO()
        {
            // Arrange
            var serviceMock = new Mock<IAuthorService>();
            var mapperMock = new Mock<IMapper>();
            var cancellationToken = new CancellationToken();

            var authorId = 1;

            var authorItem = new AuthorItem { Id = authorId, Name = "Author 1" };
            var authorDTO = new AuthorDTO { Id = authorId, Name = "Author 1" };

            serviceMock.Setup(s => s.GetAuthorByIdAsync(authorId, cancellationToken))
                .ReturnsAsync(authorItem);
            mapperMock.Setup(m => m.Map<AuthorDTO>(authorItem)).Returns(authorDTO);

            var controller = new AuthorController(serviceMock.Object, mapperMock.Object);

            // Act
            var request = await controller.GetAuthor(authorId, cancellationToken);

            // Assert
            var okAuthorDTO = Assert.IsType<ActionResult<AuthorDTO>>(request);
            var ok = Assert.IsType<OkObjectResult>(okAuthorDTO.Result);
            Assert.Equal(200, ok.StatusCode);

            serviceMock.Verify(s => s.GetAuthorByIdAsync(authorId, cancellationToken), Times.Once);
            mapperMock.Verify(m => m.Map<AuthorDTO>(authorItem), Times.Once);
        }

        [Fact]
        public async Task GetAuthor_ReturnNotFound()
        {
            // Arrange
            var serviceMock = new Mock<IAuthorService>();
            var mapperMock = new Mock<IMapper>();
            var cancellationToken = new CancellationToken();
            var authorId = 1;

            serviceMock.Setup(s => s.GetAuthorByIdAsync(authorId, cancellationToken))
                .ReturnsAsync((AuthorItem?)null);

            var controller = new AuthorController(serviceMock.Object, mapperMock.Object);

            // Act
            var request = await controller.GetAuthor(authorId, cancellationToken);

            // Assert
            var notFoundResult = Assert.IsType<ActionResult<AuthorDTO>>(request);
            Assert.IsType<NotFoundResult>(notFoundResult.Result);
            serviceMock.Verify(s => s.GetAuthorByIdAsync(authorId, cancellationToken), Times.Once);
            mapperMock.Verify(m => m.Map<AuthorDTO>(It.IsAny<AuthorItem>()), Times.Never);
        }

        [Fact]
        public async Task CreateAuthor_ReturnCreatedAuthorDTO()
        {
            // Arrange
            var serviceMock = new Mock<IAuthorService>();
            var mapperMock = new Mock<IMapper>();
            var cancellationToken = new CancellationToken();

            var createRequestDTO = new CreateAuthorRequestDTO { Name = "New Author", BirthDate = DateTime.UtcNow };
            var authorItem = new AuthorItem { Id = 1, Name = "New Author", BirthDate = DateTime.UtcNow };
            var createdAuthorItem = new AuthorItem { Id = 1, Name = "New Author" };
            var createdAuthorDTO = new AuthorDTO { Id = 1, Name = "New Author" };

            mapperMock.Setup(m => m.Map<AuthorItem>(createRequestDTO)).Returns(authorItem);
            serviceMock.Setup(s => s.CreateAuthorAsync(authorItem, cancellationToken))
                .ReturnsAsync(createdAuthorItem);
            mapperMock.Setup(m => m.Map<AuthorDTO>(createdAuthorItem)).Returns(createdAuthorDTO);

            var controller = new AuthorController(serviceMock.Object, mapperMock.Object);

            // Act
            var request = await controller.CreateAuthor(createRequestDTO, cancellationToken);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(request.Result);
            Assert.Equal(201, createdAtActionResult.StatusCode);
            serviceMock.Verify(s => s.CreateAuthorAsync(authorItem, cancellationToken), Times.Once);
            mapperMock.Verify(m => m.Map<AuthorItem>(createRequestDTO), Times.Once);
            mapperMock.Verify(m => m.Map<AuthorDTO>(createdAuthorItem), Times.Once);
        }

        [Fact]
        public async Task CreateAuthor_InvalidModel_ReturnValidationProblem()
        {
            // Arrange
            var serviceMock = new Mock<IAuthorService>();
            var mapperMock = new Mock<IMapper>();
            var cancellationToken = new CancellationToken();

            var controller = new AuthorController(serviceMock.Object, mapperMock.Object);
            controller.ModelState.AddModelError("Name", "The Name field is required.");
            var createRequestDTO = new CreateAuthorRequestDTO { BirthDate = DateTime.UtcNow };

            // Act
            var request = await controller.CreateAuthor(createRequestDTO, cancellationToken);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(request.Result);
            var problem = Assert.IsType<ValidationProblemDetails>(objectResult.Value);
            serviceMock.Verify(s => s.CreateAuthorAsync(It.IsAny<AuthorItem>(), cancellationToken), Times.Never);
            mapperMock.Verify(m => m.Map<AuthorItem>(It.IsAny<CreateAuthorRequestDTO>()), Times.Never);
        }
    }
}
