using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using ProductManagementSystem.Controllers;
using ProductManagementSystem.Models;
using ProductManagementSystem.Models.DTO;
using ProductManagementSystem.Services.Admin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ProductManagementSystemTests
{
    public class AddUserTest
    {
        [Fact]
        public void AddUser_Get_ReturnsViewWithModel()
        {
            // Arrange
            var adminServiceMock = new Mock<IAdminServices>();
            var roleList = new List<SelectListItem>
    {
        new SelectListItem { Value = "User", Text = "User" },
        new SelectListItem { Value = "Admin", Text = "Admin" }
    };
            adminServiceMock.Setup(mock => mock.GetRoleList()).Returns(roleList);

            var controller = new AdminController(adminServiceMock.Object);

            // Act
            var result = controller.AddUser() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<AddUserModel>(result.Model);
            var model = result.Model as AddUserModel;
            Assert.NotNull(model);
            Assert.Equal(2, ((List<SelectListItem>)model.RoleList).Count); // Cast model.RoleList to List<SelectListItem>
        }


        [Fact]
        public async Task AddUser_ValidModel_RedirectsToGetUser()
        {
            // Arrange
            var adminServiceMock = new Mock<IAdminServices>();
            adminServiceMock.Setup(mock => mock.AddUserAsync(It.IsAny<AddUserModel>()))
                .ReturnsAsync(IdentityResult.Success);

            var controller = new AdminController(adminServiceMock.Object);
            var validModel = new AddUserModel();

            // Act
            var result = await controller.AddUser(validModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("GetUser", result.ActionName);
        }

        [Fact]
        public async Task AddUser_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var adminServiceMock = new Mock<IAdminServices>();
            adminServiceMock.Setup(mock => mock.AddUserAsync(It.IsAny<AddUserModel>()))
                .ReturnsAsync(IdentityResult.Failed());

            var controller = new AdminController(adminServiceMock.Object);
            var invalidModel = new AddUserModel();
            controller.ModelState.AddModelError("key", "error message");

            // Act
            var result = await controller.AddUser(invalidModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<AddUserModel>(result.Model);
            var model = result.Model as AddUserModel;
            Assert.NotNull(model);
        }
    }
}
