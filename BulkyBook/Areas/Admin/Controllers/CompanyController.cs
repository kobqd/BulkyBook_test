using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;

        public CompanyController(IUnitOfWork unitOfWork,ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        //public IActionResult Upsert(int? id)
        //{
        //    Company company = new Company();
        //    if (id == null)
        //    {
        //        // create
        //        return View(company);
        //    }

        //    company = _unitOfWork.Company.Get(id.GetValueOrDefault());
        //    if(company == null)
        //    {
        //        return NotFound();
        //    }

        //    // edit
        //    return View(company);
        //}

        public IActionResult Upsert(int? id)
        {
            Company company = new Company();
            if (id == null)
            {
                // create
                return View(company);
            }

            //string query = "Select * From Companies Where Id = @id And Name = @name";
            //var param = new DynamicParameters();
            //param.AddDynamicParams(new { Id = id,Name="Action2" });
            company = _db.Companies.FirstOrDefault(c => c.Id == id);
            //company = _unitOfWork.SP_Call.CustomSql<Company>(query, param).FirstOrDefault();
            if (company == null)
            {
                return NotFound();
            }

            // edit
            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if(company.Id == 0)
                {
                    _unitOfWork.Company.Add(company);
                }
                else
                {
                    _unitOfWork.Company.Update(company);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }                
            return View(company);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Company.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Company.Get(id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.Company.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
