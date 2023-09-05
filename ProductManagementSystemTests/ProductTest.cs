using System;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagementSystem.Controllers;
using ProductManagementSystem.Models;
using ProductManagementSystem.Services.Product;
using Xunit;

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

        // Add similar test cases for other controller methods (Details, Delete, Update)
    }
}
