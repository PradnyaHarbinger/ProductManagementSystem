using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ProductManagementSystem.Controllers;
using ProductManagementSystem.Models;
using ProductManagementSystem.Services.Admin;
using Xunit;

namespace ProductManagementSystemTests
{
    public class UpdateUserTest
    {
        [Fact]
        public void Update_Get_ReturnsViewWithUser()
        {
            // Arrange
            var userId = "user123"; // Replace with a valid user ID.
            var adminServiceMock = new Mock<IAdminServices>();
            var userForUpdate = new UserModel
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com"
            };

            adminServiceMock.Setup(mock => mock.GetUserForUpdate(userId))
                .Returns(userForUpdate);

            var controller = new AdminController(adminServiceMock.Object);

            // Act
            var result = controller.Update(userId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<UserModel>(result.Model);
            var model = result.Model as UserModel;
            Assert.NotNull(model);
            Assert.Equal(userId, model.Id);
            Assert.Equal("John", model.FirstName);
            Assert.Equal("Doe", model.LastName);
            Assert.Equal("john@example.com", model.Email);
        }

        [Fact]
        public async Task Update_Post_ValidModel_SuccessfulUpdate()
        {
            // Arrange
            var adminServiceMock = new Mock<IAdminServices>();
            adminServiceMock.Setup(mock => mock.UpdateUserAsync(It.IsAny<UserModel>(), It.IsAny<string>()))
                .ReturnsAsync(true); // Simulate a successful update.

            var controller = new AdminController(adminServiceMock.Object);
            var user = new UserModel { Id = "user123", FirstName = "John" };

            // Create a new ControllerContext with HttpContext
            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.ControllerContext = controllerContext;

            // Mock ITempDataDictionary and set TempData["Success"]
            var tempDataMock = new Mock<ITempDataDictionary>();
            controller.TempData = tempDataMock.Object;
            tempDataMock.SetupSet(tempData => tempData["Success"] = "User has been edited successfully");

            // Act
            var result = await controller.Update(user) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("GetUser", result.ActionName); // Ensure it redirects to GetUser action.

            // Verify that UpdateUserAsync was called with the correct parameters
            adminServiceMock.Verify(mock => mock.UpdateUserAsync(user, It.IsAny<string>()), Times.Once);
        }



        [Fact]
        public async Task Update_Post_ValidModel_UnsuccessfulUpdate_ReturnsNotFound()
        {
            // Arrange
            var adminServiceMock = new Mock<IAdminServices>();
            adminServiceMock.Setup(mock => mock.UpdateUserAsync(It.IsAny<UserModel>(), It.IsAny<string>()))
                .ReturnsAsync(false); // Simulate an unsuccessful update.

            var controller = new AdminController(adminServiceMock.Object);
            var user = new UserModel { Id = "user123", FirstName = "John" };

            // Act
            var result = await controller.Update(user) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
        }

    }
}
