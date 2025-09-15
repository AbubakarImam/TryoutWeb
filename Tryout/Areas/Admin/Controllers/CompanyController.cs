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
                
                Company company = new();
                if(id == null || id == 0)
                {
                    return View(company);
                }
                else
                {
                    //Update
                    company = _unitOfWork.Company.Get(u => u.Id==id);
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

        //public IActionResult Create()
        //    {
        //        return View();
        //    }
        //    [HttpPost]
        //    public IActionResult Create(Company obj)
        //    {
                
        //        if (ModelState.IsValid)
        //        {
        //            _unitOfWork.Company.Add(obj);
        //            _unitOfWork.Save();
        //            TempData["success"] = "Company created succesfully";
        //            return RedirectToAction("Index");

        //        }
        //        return View();
        //    }
        //    public IActionResult Edit(int? id)
        //    {
        //        if (id == null || id == 0)
        //        {
        //            return NotFound();
        //        }
        //        Company? companyFromDb = _unitOfWork.Company.Get(u => u.Id==id);
        //        //Category categpryFromDb1 = _db.Categories.FirstOrDefault(u=>u.Id==id);
        //        //Category categpryFromDb2 = _db.Categories.Where(u=>u.Id==id).FirstOrDefault();
        //        if (companyFromDb == null)
        //        {
        //            return NotFound();
        //        }
        //        return View(companyFromDb);
        //    }
        //    [HttpPost]
        //    public IActionResult Edit(Company obj)
        //    {

        //        if (ModelState.IsValid)
        //        {
        //            _unitOfWork.Company.Update(obj);
        //            _unitOfWork.Save();
        //            TempData["success"] = "Company updated successfully";

        //            return RedirectToAction("Index");

        //        }
        //        return View();
        //    }
            //public IActionResult Delete(int? id)
            //{
            //    if (id == null || id == 0)
            //    {
            //        return NotFound();
            //    }
            //    Company? companyFromDb = _unitOfWork.Company.Get(u => u.Id==id);
            //    if (companyFromDb == null)
            //    {
            //        return NotFound();
            //    }
            //    return View(companyFromDb);
            //}
            //[HttpPost, ActionName("Delete")]
            //public IActionResult DeletePOst(int? id)
            //{
            //    Company? obj = _unitOfWork.Company.Get(u => u.Id==id);
            //    if (obj == null)
            //    {
            //        return NotFound();
            //    }
            //    _unitOfWork.Company.Remove(obj);
            //    _unitOfWork.Save();
            //    TempData["success"] = "Company deleted successfully";

            //    return RedirectToAction("Index");
            //}

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
