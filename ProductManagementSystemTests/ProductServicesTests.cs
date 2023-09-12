using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using ProductManagementSystem.Data;
using ProductManagementSystem.Models;
using ProductManagementSystem.Services.Product;
using Xunit;

namespace ProductManagementSystemTests
{
    public class ProductServicesTests : IDisposable
    {
        private readonly ApplicationDbContext _context;

        public ProductServicesTests()
        {
            // Load the SQL Server connection string from configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json") // Adjust the path as needed
                .Build();

            var connectionString = configuration.GetConnectionString("ProductConnectionString");

            // Create options for the DbContext using the SQL Server connection string
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            // Create the database and apply migrations
            _context = new ApplicationDbContext(dbContextOptions);
            _context.Database.Migrate();
        }

        [Fact]
        public void CreateAsync_ShouldAddProductToDatabase()
        {
            // Arrange
            var productServices = new ProductServices(_context);
            var product = new ProductModel
            {
                Name = "test",
                Description = "test",
                Category = "test",
                Price = 1
            };

            // Act
            productServices.CreateAsync(product);

            // Assert
            var addedProduct = _context.Products.Find(product.ProdId);
            Assert.NotNull(addedProduct);
            // Add more assertions for the product properties
        }

        [Fact]
        public void DetailsAsync_ShouldReturnProduct()
        {
            // Arrange
            var productServices = new ProductServices(_context);
            var productId = Guid.NewGuid();
            var product = new ProductModel
            {
                ProdId = productId,
                Name = "test"
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            // Act
            var result = productServices.DetailsAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ProdId);
            // Add more assertions for the product properties
        }

        [Fact]
        public void Get_ShouldReturnProduct()
        {
            // Arrange
            var productServices = new ProductServices(_context);
            var productId = Guid.NewGuid();
            var product = new ProductModel
            {
                ProdId = productId,
                Name= "test"
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            // Act
            var result = productServices.Get(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ProdId);
        }

        [Fact]
        public void Get_ShouldReturnEmptyProductForNonExistingId()
        {
            // Arrange
            var productServices = new ProductServices(_context);
            var nonExistingId = Guid.NewGuid(); // Generate a non-existing ID

            // Act
            var result = productServices.Get(nonExistingId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Guid.Empty, result.ProdId);
        }

        [Fact]
        public void GetAll_ShouldReturnListOfProducts()
        {
            // Arrange
            var productServices = new ProductServices(_context);

            // Act
            var result = productServices.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<ProductModel>>(result);
        }

        [Fact]
        public void Remove_ShouldRemoveProductFromDatabase()
        {
            // Arrange
            var productServices = new ProductServices(_context);
            var productId = Guid.NewGuid();
            var product = new ProductModel
            {
                ProdId = productId,
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            // Act
            productServices.Remove(productId);

            // Assert
            var removedProduct = _context.Products.Find(productId);
            Assert.Null(removedProduct);
        }

        private void SeedDatabase()
        {
            // Add some initial products to the in-memory database for testing
            var products = new[]
            {
                new ProductModel { ProdId = Guid.NewGuid()},
                new ProductModel { ProdId = Guid.NewGuid()},
                new ProductModel { ProdId = Guid.NewGuid()},
            };

            _context.Products.AddRange(products);
            _context.SaveChanges();
        }

        [Fact]
        public void Update_ShouldUpdateProductInDatabase()
        {
            SeedDatabase();
            // Arrange
            var productToUpdate = _context.Products.FirstOrDefault(); // Get the first product
            Assert.NotNull(productToUpdate); // Ensure there's a product to update

            var updatedProduct = new ProductModel
            {
                ProdId = productToUpdate.ProdId,
               Name = "test",
               Category = "Update"
            };

            var productServices = new ProductServices(_context);

            // Detach the tracked instance from the DbContext
            _context.Entry(productToUpdate).State = EntityState.Detached;

            // Act
            productServices.Update(productToUpdate.ProdId, updatedProduct);

            // Assert
            var retrievedProduct = _context.Products.Find(productToUpdate.ProdId);
            Assert.NotNull(retrievedProduct);
        }


        public void Dispose()
        {
            // Cleanup: Delete the test database
            _context.Database.EnsureDeleted();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
