using Microsoft.EntityFrameworkCore;
using ProductManagementSystem.Data;
using ProductManagementSystem.Models;

namespace ProductManagementSystem.Services.Product
{
    public class ProductServices : IProductServices
    {
        private readonly ApplicationDbContext _db;

        public ProductServices(ApplicationDbContext db)
        {
            _db = db;
        }

        public void CreateAsync(ProductModel model)
        {
            _db.Products.Add(model);
            _db.SaveChanges();
        }

        public ProductModel DetailsAsync(Guid id)
        {
            return _db.Products.First(p => p.ProdId == id);
        }

        public ProductModel Get(Guid id)
        {
            return _db.Products.Find(id);
        }

        public List<ProductModel> GetAll()
        {
                var products = _db.Products.ToList();
                return products;            
        }

        public void Remove(Guid id)
        {
            var product = _db.Products.FirstOrDefault(p => p.ProdId == id);
            if (product != null)
            {
                _db.Remove(product);
            }
            _db.SaveChanges();
        }

        public void Update(Guid id, ProductModel product)
        {
            _db.Update(product);
            _db.SaveChanges();
        }
    }
}
