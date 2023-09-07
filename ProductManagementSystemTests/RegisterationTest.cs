﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using ProductManagementSystem.Controllers;
using ProductManagementSystem.Models.DTO;
using ProductManagementSystem.Services.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementSystemTests
{
    public class RegisterationTest
    {
        [Fact]
        public async Task Register_InvalidClientInput_ReturnsViewWithModelError()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthServices>();
            var controller = new AccountController(authServiceMock.Object);

            // Simulate an invalid POST request with client-side validation errors
            var invalidRegisterModel = new RegisterModel();

            // Add model errors to simulate client-side validation errors
            controller.ModelState.AddModelError("FirstName", "The FirstName field is required.");
            controller.ModelState.AddModelError("LastName", "The LastName field is required.");
            controller.ModelState.AddModelError("Email", "The Email field is required.");
            controller.ModelState.AddModelError("Password", "The Password field is required.");
            controller.ModelState.AddModelError("ConfirmPassword", "The ConfirmPassword field is required.");

            // Act
            var result = await controller.Register(invalidRegisterModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Check for model errors for each field
            Assert.Contains("The FirstName field is required.", controller.ModelState["FirstName"]!.Errors.Select(e => e.ErrorMessage));
            Assert.Contains("The LastName field is required.", controller.ModelState["LastName"]!.Errors.Select(e => e.ErrorMessage));
            Assert.Contains("The Email field is required.", controller.ModelState["Email"]!.Errors.Select(e => e.ErrorMessage));
            Assert.Contains("The Password field is required.", controller.ModelState["Password"]!.Errors.Select(e => e.ErrorMessage));
            Assert.Contains("The ConfirmPassword field is required.", controller.ModelState["ConfirmPassword"]!.Errors.Select(e => e.ErrorMessage));
        }





        [Fact]
        public async Task Register_ValidModel_RedirectsToHomePage()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthServices>();
            authServiceMock.Setup(mock => mock.RegisterAsync(It.IsAny<RegisterModel>()))
                           .ReturnsAsync(IdentityResult.Success);

            var controller = new AccountController(authServiceMock.Object);
            var validRegisterModel = new RegisterModel
            {
                FirstName = "Unit",
                LastName = "TestCase",
                Email = "unit@test.com",
                Password = "User@123",
                ConfirmPassword = "User@123"
            };

            // Act
            var result = await controller.Register(validRegisterModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
        }


        [Fact]
        public async Task Register_RegistrationFailure_ReturnViewResult()
        {
            var authServiceMock = new Mock<IAuthServices>();
            authServiceMock.Setup(mock => mock.RegisterAsync(It.IsAny<RegisterModel>()))
                            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error 1" }));
            
            var controller = new AccountController(authServiceMock.Object);

            var invalidRegisterModel = new RegisterModel
            {
                FirstName = "unit",
                LastName = "TestInvalid",
                Email = "demouser@gmail.com",
                Password = "User@123",
                ConfirmPassword = "User@123"
            };

            var result = await controller.Register(invalidRegisterModel) as ViewResult;
            Assert.NotNull(result);
            Assert.Equal(invalidRegisterModel, result.Model);
            Assert.False(controller.ModelState.IsValid);
            Assert.Contains(controller.ModelState.Values.SelectMany(v => v.Errors), e => e.ErrorMessage == "Error 1");
        }


        [Fact]
        public async Task Register_DuplicateEmail_ReturnsViewWithError()
        {
            var authServiceMock = new Mock<IAuthServices>();
            authServiceMock.Setup(mock => mock.RegisterAsync(It.IsAny<RegisterModel>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "DuplicateEmail", Description = "Email is already registered." }));

            var controller = new AccountController(authServiceMock.Object);

            var duplicateEmailModel = new RegisterModel
            {
                FirstName = "Unit",
                LastName = "TestCase",
                Email = "existing-email@test.com", // Email already exists
                Password = "User@123",
                ConfirmPassword = "User@123"
            };

            var result = await controller.Register(duplicateEmailModel) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Contains(controller.ModelState.Values.SelectMany(v => v.Errors), e => e.ErrorMessage == "Email is already registered.");
        }



    }
}
