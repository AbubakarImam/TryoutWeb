using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tryout.DataAccess.Repository.IRepository;
using Tryout.Models;
using Tryout.Models.ViewModels;

namespace Tryout.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();
            return View(objProductList);
        }

        public IActionResult Create()
        {
            
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };

            return View(productVM);
        }


        // POST: /Admin/Product/Create
        [HttpPost]
        public IActionResult Create(ProductVM productVM)
        {
            if (productVM.Product.Title == productVM.Product.Description)
            {
                ModelState.AddModelError("Title", "The description cannot exactly match the title.");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category
                    .GetAll().Select(u => new SelectListItem
                        {
                            Text = u.Name,
                            Value = u.Id.ToString()
                         });
                return View(productVM);
            }
        }

        // GET: /Admin/Product/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var product = _unitOfWork.Product.Get(p => p.Id == id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: /Admin/Product/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product)
        {
            if (product.Title == product.Description)
            {
                ModelState.AddModelError("Title", "The description cannot exactly match the title.");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(product);
                _unitOfWork.Save();
                TempData["success"] = "Product updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        // GET: /Admin/Product/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var product = _unitOfWork.Product.Get(p => p.Id == id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: /Admin/Product/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var product = _unitOfWork.Product.Get(p => p.Id == id);
            if (product == null)
                return NotFound();

            _unitOfWork.Product.Remove(product);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
