using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagementSystem.Models;
using ProductManagementSystem.Services.Product;

namespace ProductManagementSystem.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductServices _services;

        public ProductController(IProductServices services)
        {
            _services = services;
        }

        [Authorize]
        public IActionResult Index()
        {
            var products = _services.GetAll();
            return View(products);
        }

        [Authorize(Roles ="Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(ProductModel product)
        {
            if (ModelState.IsValid)
            {
                _services.CreateAsync(product);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public IActionResult Details(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = _services.DetailsAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }
            _services.Remove(id);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Update(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = _services.Get(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Update(Guid id, ProductModel product)
        {
            if (id != product.ProdId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _services.Update(id, product);
                return RedirectToAction("Index");
            }
            return View(product);
        }


    }
}
