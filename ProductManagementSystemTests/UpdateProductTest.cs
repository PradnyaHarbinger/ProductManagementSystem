using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagementSystem.Controllers;
using ProductManagementSystem.Models;
using ProductManagementSystem.Services.Product;
using System;
using System.Security.Claims;
using Xunit;

namespace ProductManagementSystemTests
{
    public class UpdateProductTest
    {
        [Fact]
        public void Update_AdminUser_ProductExists_ReturnsViewResult()
        {
            // Arrange
            var productServiceMock = new Mock<IProductServices>();
            var controller = new ProductController(productServiceMock.Object);

            // Mock an authorized user in "Admin" role
            var authorizedUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
        new Claim(ClaimTypes.Role, "Admin")
    }, "mock"));

            // Create a dummy product for testing
            var productId = Guid.NewGuid();
            var dummyProduct = new ProductModel
            {
                ProdId = productId, // Set the ID to match the generated Guid
                                    // Set other properties of the product as needed for your test
            };

            // Setup the mock to return the dummyProduct when Get is called with the matching ID
            productServiceMock.Setup(mock => mock.Get(productId)).Returns(dummyProduct);

            // Set the authorized user for the controller's HttpContext
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = authorizedUser
                }
            };

            // Act
            var result = controller.Update(productId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }


        [Fact]
        public void Update_AdminUser_ProductNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var productServiceMock = new Mock<IProductServices>();
            productServiceMock.Setup(mock => mock.Get(It.IsAny<Guid>())).Returns((ProductModel)null); // Return null for non-existent product
            var controller = new ProductController(productServiceMock.Object);

            // Mock an authorized user in "Admin" role
            var authorizedUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                         new Claim(ClaimTypes.Role, "Admin")
                                    }, "mock"));

            // Set the authorized user for the controller's HttpContext
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = authorizedUser
                }
            };

            // Act
            var result = controller.Update(Guid.NewGuid());

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public void Update_AdminUser_InvalidModelState_ReturnsViewResult()
        {
            // Arrange
            var productServiceMock = new Mock<IProductServices>();
            var controller = new ProductController(productServiceMock.Object);
            controller.ModelState.AddModelError("Name", "Name is required"); // Simulate invalid ModelState

            // Mock an authorized user in "Admin" role
            var authorizedUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                    new Claim(ClaimTypes.Role, "Admin")
                                }, "mock"));

            // Set the authorized user for the controller's HttpContext
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = authorizedUser
                }
            };

            // Create a valid ProductModel
            var validProduct = new ProductModel
            {
                // Setting valid properties here
                Name = "ValidProductName",
                Description = "ValidDescription",
                Category = "ValidCategory",
                Price = 3000
            };

            // Act
            var result = controller.Update(Guid.NewGuid(), validProduct);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }



        [Fact]
        public void Update_AdminUser_ValidModelState_ReturnsRedirectToActionResult()
        {
            // Arrange
            var productServiceMock = new Mock<IProductServices>();
            var controller = new ProductController(productServiceMock.Object);

            // Mock an authorized user in "Admin" role
            var authorizedUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                    new Claim(ClaimTypes.Role, "Admin")
                                }, "mock"));

            // Set the authorized user for the controller's HttpContext
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = authorizedUser
                }
            };

            // Create a valid ProductModel
            var validProduct = new ProductModel
            {
                ProdId = Guid.NewGuid(), // Existing Product ID
                Name = "ValidProductName",
                Description = "ValidDescription",
                Category = "ValidCategory",
                Price = 3000
            };

            // Act
            var result = controller.Update(validProduct.ProdId, validProduct);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
        }


    }
}
