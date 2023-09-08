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
using System.Reflection;
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

        [Fact]
        public async Task AddUser_ModelStateInvalid_VerifyModelStateErrors()
        {
            // Arrange
            var mockAdminService = new Mock<IAdminServices>();
            var controller = new AdminController(mockAdminService.Object);
            var model = new AddUserModel(); // Create a model with invalid ModelState
            controller.ModelState.AddModelError("PropertyName", "Error Message");

            // Act
            var result = await controller.AddUser(model) as ViewResult;

            // Assert
            mockAdminService.Verify(service => service.AddUserAsync(model), Times.Never()); // Ensure AddUserAsync is not called
            mockAdminService.Verify(service => service.GetRoleList(), Times.Once()); // Ensure GetRoleList is called

            // Check if ModelState contains errors
            Assert.False(controller.ModelState.IsValid); // ModelState should still be invalid
            Assert.NotNull(result); // Ensure that the result is a ViewResult

            // Check if ModelState contains the expected error
            Assert.True(controller.ModelState.ContainsKey("PropertyName")); // Verify that the key is in ModelState
            var modelStateEntry = controller.ModelState["PropertyName"];
            Assert.NotNull(modelStateEntry); // Verify that the ModelStateEntry is not null
            Assert.Equal("Error Message", modelStateEntry.Errors[0].ErrorMessage);
            // Verify the error message

            // You should also check if the model contains the expected data
            Assert.Equal(model, result.Model); // Ensure that the model is passed back to the view
        }

        [Fact]
        public async Task AddUser_ModelStateInvalid_CallsAddErrors()
        {
            // Arrange
            var mockAdminService = new Mock<IAdminServices>();
            var controller = new AdminController(mockAdminService.Object);
            var model = new AddUserModel(); // Create a model with invalid ModelState
            controller.ModelState.AddModelError("PropertyName", "Error Message");

            // Act
            var result = await controller.AddUser(model) as ViewResult;

            // Assert
            mockAdminService.Verify(service => service.AddUserAsync(model), Times.Never()); // Ensure AddUserAsync is not called
            mockAdminService.Verify(service => service.GetRoleList(), Times.Once()); // Ensure GetRoleList is called

            // Use reflection to access the private AddErrors method
            MethodInfo? addErrorsMethod = typeof(AdminController).GetMethod("AddErrors", BindingFlags.NonPublic | BindingFlags.Instance);

            addErrorsMethod?.Invoke(controller, new object[] { new IdentityResult() }); // Call AddErrors with an empty IdentityResult if addErrorsMethod is not null




            // Check if ModelState contains the expected error
            Assert.True(controller.ModelState.ContainsKey("PropertyName")); // Verify that the key is in ModelState
            var modelStateEntry = controller.ModelState["PropertyName"];
            Assert.NotNull(modelStateEntry); // Verify that the ModelStateEntry is not null
            Assert.Equal("Error Message", modelStateEntry.Errors[0].ErrorMessage); // Verify the error message

            // You should also check if the model contains the expected data
            Assert.NotNull(result); // Ensure that result is not null
            Assert.NotNull(result.Model); // Ensure that the model is not null
            Assert.Equal(model, result.Model); // Ensure that the model is passed back to the view
        }

        [Fact]
        public void TestAddErrors()
        {
            // Arrange
            var mockAdminServices = new Mock<IAdminServices>(); // Create a mock for IAdminServices
            var controller = new AdminController(mockAdminServices.Object); // Pass the mock to the constructor
            var identityResult = IdentityResult.Failed(new IdentityError { Description = "Error Message" });

            // Act
            // Use reflection to call the public AddErrors method
            MethodInfo? addErrorsMethod = controller.GetType().GetMethod("AddErrors", BindingFlags.Public | BindingFlags.Instance);

            if (addErrorsMethod != null)
            {
                addErrorsMethod.Invoke(controller, new object[] { identityResult });

                // Assert
                Assert.True(controller.ModelState.ContainsKey(string.Empty)); // Verify that the key is in ModelState
                var modelStateEntry = controller.ModelState[string.Empty];
                Assert.NotNull(modelStateEntry); // Verify that the ModelStateEntry is not null
                Assert.Equal("Error Message", modelStateEntry.Errors[0].ErrorMessage); // Verify the error message
            }
            else
            {
                Assert.True(false, "AddErrors method not found"); // Handle case where methodInfo is null
            }
        }


        [Fact]
        public void AddErrors_AddsErrorsToModelState()
        {
            // Arrange
            var mockAdminServices = new Mock<IAdminServices>(); // Create a mock for IAdminServices
            var controller = new AdminController(mockAdminServices.Object); // Pass the mock to the constructor
            var identityResult = IdentityResult.Failed(new IdentityError { Code = "Error1", Description = "Error Description 1" },
                                                      new IdentityError { Code = "Error2", Description = "Error Description 2" });

            // Act
            controller.AddErrors(identityResult);

            // Assert
            Assert.False(controller.ModelState.IsValid);

            var errors = controller.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            Assert.Contains("Error Description 1", errors);
            Assert.Contains("Error Description 2", errors);
        }

    }
}
