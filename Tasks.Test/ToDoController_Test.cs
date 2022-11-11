using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Tasks.Controllers;
using Tasks.Domain;
using Tasks.Models;
using Tasks.Services;

namespace Tasks.Test
{
    public class ToDoController_Test
    {
        ToDoItem originalToDoItem;
        ToDoItem toDoItem;
        CreateToDoItemDto createToDoItemDto;
        Claim sidCalim;
        string userName = "Test User name";
        Mock<IRabbitMqClient> mockRabbitMqClient;
        Mock<IToDoRepository> mockToDoRepository;
        Mock<HttpContext> mockHttpContext;
        ControllerContext mockControllerContext;
        [SetUp]
        public void Setup()
        {
            originalToDoItem = new ToDoItem()
            {
                Id = 123,
                Title = "Test item title",
                Description = "Test item description",
                AccountId = "Test account Id",
                CreatedBy = "Test created by",
                ModifiedBy = "Test modified by",
                CreatedAt = new DateTime(2022, 11, 1, 1, 2, 3),
                ModifedOn = new DateTime(2022, 11, 2, 3, 4, 5),
            };
            toDoItem = new ToDoItem()
            {
                Id = 234,
                Title = "Test new item title",
                Description = "Test new item description",
                AccountId = "Test new account Id",
                CreatedBy = "Test new created by",
                ModifiedBy = "Test new modified by",
                CreatedAt = new DateTime(2021, 9, 5, 6, 7, 8),
                ModifedOn = new DateTime(2021, 8, 9, 8, 7, 6),
            };

            createToDoItemDto = new CreateToDoItemDto()
            {
                Title = "Create ToDo item Dto",
                Description = "Description of ToDoItem Dto",
            };

            sidCalim = new Claim("sid", "test sid");

            mockRabbitMqClient = new Mock<IRabbitMqClient>();
            mockToDoRepository = new Mock<IToDoRepository>();
            mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(repo => repo.User.Claims)
                .Returns(new List<Claim>() { sidCalim });
            mockHttpContext.Setup(repo => repo.User.Identity.Name)
                .Returns(userName);
            mockControllerContext = new ControllerContext();
            mockControllerContext.HttpContext = mockHttpContext.Object;

        }

        [Test(Description = "Should return Ok(ToDoItemDto)")]
        public void CreateToDoItem_Ok_Test()
        {
            // Arrange
            mockToDoRepository.Setup(repo => repo.CreateToDoItem(createToDoItemDto, userName, sidCalim.Value))
                .Returns(toDoItem);

            var controller = new ToDoController(mockRabbitMqClient.Object, mockToDoRepository.Object)
            {
                ControllerContext = mockControllerContext,
            };
            var itemId = 444;

            // Act
            var actionResult = controller.CreateToDoItem(createToDoItemDto);

            // Assert
            mockToDoRepository.Verify(x =>
                x.CreateToDoItem(createToDoItemDto, userName, sidCalim.Value), Times.Once);
            mockRabbitMqClient.Verify(x =>
                x.SendMessage("ToDoItem", It.IsAny<ToDoRabbitMessage>()), Times.Once);

            Assert.IsInstanceOf<CreatedAtActionResult>(actionResult);
            Assert.IsInstanceOf<ToDoItemDto>(((CreatedAtActionResult)actionResult).Value);
            var returnedToDoItemDto = (ToDoItemDto)((CreatedAtActionResult)actionResult).Value;
            Assert.That(returnedToDoItemDto.Id, Is.EqualTo(toDoItem.Id));
            Assert.That(returnedToDoItemDto.Title, Is.EqualTo(toDoItem.Title));
            Assert.That(returnedToDoItemDto.Description, Is.EqualTo(toDoItem.Description));
            Assert.That(returnedToDoItemDto.AccountId, Is.EqualTo(toDoItem.AccountId));
            Assert.That(returnedToDoItemDto.CreatedBy, Is.EqualTo(toDoItem.CreatedBy));
            Assert.That(returnedToDoItemDto.ModifiedBy, Is.EqualTo(toDoItem.ModifiedBy));
            Assert.That(returnedToDoItemDto.ModifedOn, Is.EqualTo(toDoItem.ModifedOn));
            Assert.That(returnedToDoItemDto.CreatedAt, Is.EqualTo(toDoItem.CreatedAt));
        }

        [Test(Description = "Should return Ok(ToDoItemDto)")]
        public void UpdateToDoItem_Ok_Test()
        {
            // Arrange
            mockToDoRepository.Setup(repo => repo.UpdateToDoItem(It.IsAny<CreateToDoItemDto>(), It.IsAny<string>(), It.IsAny<int>(), out originalToDoItem))
                .Returns(toDoItem);

            var controller = new ToDoController(mockRabbitMqClient.Object, mockToDoRepository.Object)
            {
                ControllerContext = mockControllerContext,
            };
            var itemId = 444;

            // Act
            var actionResult = controller.UpdateToDoItem(itemId, createToDoItemDto);

            // Assert
            ToDoItem outToDoItem;
            mockToDoRepository.Verify(x =>
                x.UpdateToDoItem(createToDoItemDto, userName, itemId, out outToDoItem), Times.Once);
            mockRabbitMqClient.Verify(x =>
                x.SendMessage("ToDoItem", It.IsAny<ToDoRabbitMessage>()), Times.Once);

            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            Assert.IsInstanceOf<ToDoItemDto>(((OkObjectResult)actionResult).Value);
            var returnedToDoItemDto = (ToDoItemDto)((OkObjectResult)actionResult).Value;
            Assert.That(returnedToDoItemDto.Id, Is.EqualTo(toDoItem.Id));
            Assert.That(returnedToDoItemDto.Title, Is.EqualTo(toDoItem.Title));
            Assert.That(returnedToDoItemDto.Description, Is.EqualTo(toDoItem.Description));
            Assert.That(returnedToDoItemDto.AccountId, Is.EqualTo(toDoItem.AccountId));
            Assert.That(returnedToDoItemDto.CreatedBy, Is.EqualTo(toDoItem.CreatedBy));
            Assert.That(returnedToDoItemDto.ModifiedBy, Is.EqualTo(toDoItem.ModifiedBy));
            Assert.That(returnedToDoItemDto.ModifedOn, Is.EqualTo(toDoItem.ModifedOn));
            Assert.That(returnedToDoItemDto.CreatedAt, Is.EqualTo(toDoItem.CreatedAt));
        }

        [Test(Description = "Should return BadRequest if Title is missing")]
        public void UpdateToDoItem_BadRequest_Test()
        {
            // Arrange
            mockToDoRepository.Setup(repo => repo.UpdateToDoItem(It.IsAny<CreateToDoItemDto>(), It.IsAny<string>(), It.IsAny<int>(), out originalToDoItem))
                .Returns(toDoItem);

            var controller = new ToDoController(mockRabbitMqClient.Object, mockToDoRepository.Object)
            {
                ControllerContext = mockControllerContext,
            };
            var itemId = 444;

            // Act
            var actionResult = controller.UpdateToDoItem(itemId, new CreateToDoItemDto() { Title = "", Description = null });

            // Assert
            ToDoItem outToDoItem;
            mockToDoRepository.Verify(x =>
                x.UpdateToDoItem(It.IsAny<CreateToDoItemDto>(), It.IsAny<string>(), It.IsAny<int>(), out outToDoItem), Times.Never);

            Assert.IsInstanceOf<BadRequestObjectResult>(actionResult);
        }

        [Test(Description = "Should return BadRequest if Title is missing")]
        public void DeleteToDoItem_Ok_Test()
        {
            // Arrange
            var controller = new ToDoController(mockRabbitMqClient.Object, mockToDoRepository.Object)
            {
                ControllerContext = mockControllerContext,
            };
            var itemId = 444;

            // Act
            var actionResult = controller.DeleteToDoItem(itemId);

            // Assert
            ToDoItem outToDoItem;
            mockToDoRepository.Verify(x =>
                x.DeleteToDoItem(itemId, out outToDoItem), Times.Once);
            mockRabbitMqClient.Verify(x =>
                x.SendMessage("ToDoItem", It.IsAny<ToDoRabbitMessage>()), Times.Once);

            Assert.IsInstanceOf<OkResult>(actionResult);
        }
    }
}