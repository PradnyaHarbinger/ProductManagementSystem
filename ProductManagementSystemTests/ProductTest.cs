using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ProductManagementSystem.Controllers;
using ProductManagementSystem.Models;
using ProductManagementSystem.Services.Product;
using Xunit;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ProductManagementSystemTests
{
    public class ProductTests
    {
        [Fact]
        public void Index_ReturnsViewWithProducts()
        {
            // Arrange
            var productServiceMock = new Mock<IProductServices>();
            productServiceMock.Setup(mock => mock.GetAll()).Returns(new List<ProductModel>
            {
                new ProductModel { Name = "Product 1", Category = "cat", Description = "Decs1",Price = 10.99 },
                new ProductModel { Name = "Product 2", Category = "cat", Description = "Decs1",Price = 19.99 }
            });

            var controller = new ProductController(productServiceMock.Object);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Model);
            Assert.IsType<List<ProductModel>>(result.Model);
            var model = (List<ProductModel>)result.Model;
            Assert.Equal(2, model.Count);
            // Add additional assertions based on your sample products
            Assert.Equal("Product 1", model[0].Name);
            Assert.Equal("Product 2", model[1].Name);
        }

        [Fact]
        public void Create_ValidProduct_RedirectsToIndex()
        {
            // Arrange
            var productServiceMock = new Mock<IProductServices>();
            productServiceMock.Setup(mock => mock.CreateAsync(It.IsAny<ProductModel>())).Verifiable();

            var controller = new ProductController(productServiceMock.Object);
            var validProduct = new ProductModel();

            // Act
            var result = controller.Create(validProduct) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            productServiceMock.Verify(mock => mock.CreateAsync(validProduct), Times.Once);
        }

        [Fact]
        public void Create_AuthorizedAdmin_ReturnsView()
        {
            // Arrange
            var httpContextMock = new Mock<HttpContext>();
            var controllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            httpContextMock.SetupGet(c => c.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, "Admin") // Simulate a user with Admin role
            })));

            var productServiceMock = new Mock<IProductServices>();
            var controller = new ProductController(productServiceMock.Object)
            {
                ControllerContext = controllerContext
            };

            // Act
            var result = controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName); // Assert that the view name is null (default view)
        }

        [Fact]
        public void Create_UnauthorizedUser_ReturnsViewResult()
        {
            // Arrange
            var productServiceMock = new Mock<IProductServices>();
            var controller = new ProductController(productServiceMock.Object);

            // Mock an unauthorized user (not in "Admin" role)
            var unauthorizedUser = new ClaimsPrincipal(new ClaimsIdentity(Array.Empty<Claim>(), "mock"));


            // Create a mock authorization service that returns a failure result
            var authorizationService = new Mock<IAuthorizationService>();
            authorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), null, "AdminOnly"))
                                .ReturnsAsync(AuthorizationResult.Failed());

            // Configure TempData and TempDataDictionaryFactory
            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempDataDictionaryFactory = new TempDataDictionaryFactory(tempDataProvider.Object);

            // Set the authorization service and TempData for the controller's HttpContext
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = unauthorizedUser,
                    RequestServices = new ServiceCollection()
                        .AddSingleton(authorizationService.Object)
                        .AddSingleton<ITempDataDictionaryFactory>(tempDataDictionaryFactory)
                        .BuildServiceProvider()
                }
            };

            // Act
            var result = controller.Create();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }


        [Fact]
        public void Delete_AdminUser_ReturnsRedirectToActionResult()
        {
            // Arrange
            var productServiceMock = new Mock<IProductServices>();
            productServiceMock.Setup(mock => mock.Remove(It.IsAny<Guid>())).Verifiable(); // Mocking the Remove method

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
            var result = controller.Delete(Guid.NewGuid());

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);

            // Verify that the Remove method was called
            productServiceMock.Verify(mock => mock.Remove(It.IsAny<Guid>()), Times.Once);
        }

    }
}
