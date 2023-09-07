using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagementSystem.Controllers;
using ProductManagementSystem.Models;
using ProductManagementSystem.Services.Product;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ProductManagementSystemTests
{
    public class DetailsProductTest
    {
        [Fact]
        public void DetailsProductExistsReturnsViewResult()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productServiceMock = new Mock<IProductServices>();
            productServiceMock.Setup(mock => mock.DetailsAsync(productId))
                .Returns(new ProductModel
                {
                    ProdId = productId,
                    Name = "Test",
                    Description = "Test description",
                    Price = 1000
                });

            var controller = new ProductController(productServiceMock.Object);

            // Act
            var result = controller.Details(productId) as ViewResult;

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
            Assert.IsType<ProductModel>(viewResult.Model);
        }

        [Fact]
        public void DetailsProductNotFoundReturnsNotFoundResult()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productServiceMock = new Mock<IProductServices>();
            productServiceMock.Setup(mock => mock.DetailsAsync(productId))
                .Returns((ProductModel)null!);

            var controller = new ProductController(productServiceMock.Object);

            // Act
            var result = controller.Details(productId) as NotFoundResult;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
