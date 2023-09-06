using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using ProductManagementSystem.Controllers;
using ProductManagementSystem.Services.Admin;
using Xunit;

namespace ProductManagementSystemTests
{
    public class DeleteUserTest
    {
        [Fact]
        public IActionResult Delete_UserFound_DeletesUserAndRedirectsToGetUserWithSuccessMessage()
        {
            // Arrange
            var userId = "user123";
            var adminServiceMock = new Mock<IAdminServices>();
            adminServiceMock.Setup(mock => mock.Delete(userId)).Returns(true); // Simulate a successful delete.

            var controller = new AdminController(adminServiceMock.Object);

            // Create a new HttpContext and assign it to the controller
            var httpContext = new DefaultHttpContext();
            httpContext.Items["key"] = "value"; // Add any items needed for your test
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Set up TempData
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;

            // Act
            var result = controller.Delete(userId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("GetUser", result.ActionName); // Ensure it redirects to GetUser action.
            Assert.Equal("User deleted successfully.", controller.TempData["Success"]);

            // Verify that Delete method was called with the correct parameter
            adminServiceMock.Verify(mock => mock.Delete(userId), Times.Once);

            return result;
        }

        [Fact]
        public IActionResult Delete_UserNotFound_ReturnsErrorAndRedirectsToGetUser()
        {
            // Arrange
            var userId = "user123";
            var adminServiceMock = new Mock<IAdminServices>();
            adminServiceMock.Setup(mock => mock.Delete(userId)).Returns(false); // Simulate user not found or unable to delete.

            var controller = new AdminController(adminServiceMock.Object);

            // Create a new HttpContext and assign it to the controller
            var httpContext = new DefaultHttpContext();
            httpContext.Items["key"] = "value"; // Add any items needed for your test
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Set up TempData
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;

            // Act
            var result = controller.Delete(userId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("GetUser", result.ActionName); // Ensure it redirects to GetUser action.
            Assert.Equal("User not found or unable to delete.", controller.TempData["Error"]);

            // Verify that Delete method was called with the correct parameter
            adminServiceMock.Verify(mock => mock.Delete(userId), Times.Once);

            return result;
        }
    }
}
