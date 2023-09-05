using ProductManagementSystem.Models;
using System.Collections.Generic;

namespace ProductManagementSystem.Services.Product
{
    public interface IProductServices
    {

        void CreateAsync(ProductModel model);

        ProductModel DetailsAsync(Guid id);

        List<ProductModel> GetAll();

        ProductModel Get(Guid id);
        void Remove(Guid id);

        void Update(Guid id, ProductModel product);

    }
}
