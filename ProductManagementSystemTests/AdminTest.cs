using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using ProductManagementSystem.Controllers;
using ProductManagementSystem.Models;
using ProductManagementSystem.Models.DTO;
using ProductManagementSystem.Services.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementSystemTests
{
    public class AdminTest
    {
        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Arrange
            var adminServiceMock = new Mock<IAdminServices>();
            var controller = new AdminController(adminServiceMock.Object);

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }


        [Fact]
        public void GetUser_ReturnsViewWithUsers()
        {
            // Arrange
            var adminServiceMock = new Mock<IAdminServices>();

            // Mock the GetUsers method to return a list of UserModel
            adminServiceMock.Setup(mock => mock.GetUser()).Returns(new List<UserModel>
            {
                new UserModel { Id = "1", FirstName = "User1" },
                new UserModel { Id = "2", FirstName = "User2" }
            });

            var controller = new AdminController(adminServiceMock.Object);

            // Act
            var result = controller.GetUser() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Model);
            Assert.IsType<List<UserModel>>(result.Model);
            var model = (List<UserModel>)result.Model;
            Assert.Equal(2, model.Count);
            Assert.Equal("User1", model[0].FirstName);
            Assert.Equal("User2", model[1].FirstName);
        }


        
        [Fact]
        public void GetUser_ReturnsViewWithEmptyUsers()
        {
            // Arrange
            var adminServiceMock = new Mock<IAdminServices>();
            adminServiceMock.Setup(mock => mock.GetUser()).Returns(new List<UserModel>());

            var controller = new AdminController(adminServiceMock.Object);

            // Act
            var result = controller.GetUser() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Model);
            Assert.IsType<List<UserModel>>(result.Model);
            var model = (List<UserModel>)result.Model;
            Assert.Empty(model);
        }

        [Fact]
        public void GetUser_ServiceReturnsNullUsers_ReturnsView()
        {
            // Arrange
            var adminServiceMock = new Mock<IAdminServices>();
            adminServiceMock.Setup(mock => mock.GetUser()).Returns((List<UserModel>?)null!);

            var controller = new AdminController(adminServiceMock.Object);

            // Act
            var result = controller.GetUser() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Model);
        }


    }
}
