using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tryout.DataAccess.Repository.IRepository;
using Tryout.Models;
using Tryout.Utility;

namespace Tryout.Areas.Admin.Controllers
{
        [Area("Admin")]
        [Authorize(Roles = SD.Role_Admin)]
        public class CompanyController : Controller
        {

            private readonly IUnitOfWork _unitOfWork;
            public CompanyController(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }
            public IActionResult Index()
            {
                List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();

                return View(objCompanyList);
            }

            public IActionResult Upsert(int? id)
            {
                
                if(id == null || id == 0)
                {
                Company company = new();

                return View(company);
                }
                else
                {
                    //Update
                   var  company = _unitOfWork.Company.Get(u => u.Id==id);
                    if (company == null)
                    {
                        return NotFound();
                    }
                    return View(company);
                }

        }
            // POST: /Admin/Company/Create
            [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company obj)
        {
            if (ModelState.IsValid)
            {
                if(obj.Id == 0)
                {
                    _unitOfWork.Company.Add(obj);
                    TempData["success"] = "Company created successfully";
                }
                else
                {
                    _unitOfWork.Company.Update(obj);
                    TempData["success"] = "Company updated successfully";
                }
                
                _unitOfWork.Save();
                
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        #region API CALLS
        [HttpGet]
            public IActionResult GetAll()
            {
                var companyList = _unitOfWork.Company.GetAll();
                return Json(new { data = companyList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitOfWork.Company.Get(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.Company.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}
