using CRUDApplicationMVC.Models;
using CRUDApplicationMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis;
using System.Drawing.Drawing2D;

namespace CRUDApplicationMVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index()
        {
            var product = context.Products.OrderByDescending(p => p.Id).ToList();
            return View(product);
        }

        public IActionResult ProductCreate()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ProductCreate(CreateProduct createProduct)
        {
            if (createProduct.ImageFileName == null)
            {
                ModelState.AddModelError("ImageFileName", "Image file is requred");
            }

            if (!ModelState.IsValid)
            {
                return View(createProduct);
            }

            string newFileName = DateTime.Now.ToString("yyyyMMddmmssfff");
            newFileName += Path.GetExtension(createProduct.ImageFileName!.FileName);

            string imageFullPath = environment.WebRootPath + "/Images/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                createProduct.ImageFileName.CopyTo(stream);
            }

            Product product = new Product()
            {
                Name = createProduct.Name,
                Brand = createProduct.Brand,
                Category = createProduct.Category,
                Price = createProduct.Price,
                Description = createProduct.Description,
                ImageFileName = newFileName,
                CreateDate = DateTime.Now,
            };

            context.Products.Add(product);
            context.SaveChanges();

            return RedirectToAction("Index", "Product");
        }

        public IActionResult Edit(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Product");
            }

            var CreateProduct = new CreateProduct()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description
            };

            ViewData["Id"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreateDate"] = product.CreateDate.ToString("MM/dd/yyyy");

            return View(CreateProduct);

        }

        [HttpPost]
        public IActionResult Edit(int id, CreateProduct createProduct)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Product");
            }

            if (!ModelState.IsValid)
            {

                ViewData["Id"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreateDate"] = product.CreateDate.ToString("MM/dd/yyyy");

                return View(createProduct);
            }

            string newFileName = product.ImageFileName;
            if (createProduct.ImageFileName != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddmmssfff");
                newFileName += Path.GetExtension(createProduct.ImageFileName!.FileName);

                string imageFullPath = environment.WebRootPath + "/Images/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    createProduct.ImageFileName.CopyTo(stream);
                }

                string oldImageFullPath = environment.WebRootPath + "/Images/" + product.ImageFileName;
                System.IO.File.Delete(oldImageFullPath);
            }
            
            product.Name = createProduct.Name;
            product.Brand = createProduct.Brand;
            product.Description = createProduct.Description;
            product.Price = createProduct.Price;
            product.Category = createProduct.Category;
            product.ImageFileName = newFileName;

            context.SaveChanges();

            return RedirectToAction("Index", "Product");
        }

        public IActionResult Delete(int id)
        {
            var product = context.Products.Find(id);
            if(product == null)
            {
                return RedirectToAction("Index", "Product");
            }

            string imageFullPath = environment.WebRootPath + "/Images/" + product.ImageFileName;
            System.IO.File.Create(imageFullPath);

            context.Products.Remove(product);
            context.SaveChanges(true);

            return RedirectToAction("Index", "Product");
        }
    }
}
