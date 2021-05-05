using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _db;
        private static string ConnectionString = "";

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment, ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
            _db = db;
            ConnectionString = db.Database.GetDbConnection().ConnectionString;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem {
                    Text = i.Name,
                    Value = i.Id.ToString(),
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString(),
                })
            };
            if (id == null)
            {
                // create
                return View(productVM);
            }

            productVM.Product = _unitOfWork.Product.Get(id.GetValueOrDefault());
            if (productVM.Product == null)
            {
                return NotFound();
            }

            // edit
            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var upload = Path.Combine(webRootPath, @"images\products");
                    var extention = Path.GetExtension(files[0].FileName);

                    //using (var reader = new StreamReader(files[0].OpenReadStream()))
                    //using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    //{
                    //    csv.Read();
                    //    csv.ReadHeader();
                    //    while (csv.Read())
                    //    {
                    //        var category = csv.GetRecord<Category>();
                    //        _unitOfWork.Category.Add(category);
                    //    }
                    //    _unitOfWork.Save();

                    //}



                    //var categories = _unitOfWork.Category.GetAll();
                    //using (var write = new StreamWriter(upload+"\\NewCategory.csv"))
                    //using (var csv = new CsvWriter(write, CultureInfo.InvariantCulture))
                    //{
                    //    csv.WriteRecords(categories);

                    //}



                    //DataTable tblcsv = new DataTable();
                    //tblcsv.Columns.Add("Id");
                    //tblcsv.Columns.Add("Name");
                    //using (var reader = new StreamReader(files[0].OpenReadStream()))
                    //using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    //{
                    //    reader.ReadLine();
                    //    string ReadCSV = reader.ReadToEnd();
                    //    //spliting row after new line  
                    //    foreach (string csvRow in ReadCSV.Split("\r\n"))
                    //    {
                    //        if (!string.IsNullOrEmpty(csvRow))
                    //        {
                    //            //Adding each row into datatable  
                    //            tblcsv.Rows.Add();
                    //            int count = 0;
                    //            foreach (string FileRec in csvRow.Split(','))
                    //            {
                    //                tblcsv.Rows[tblcsv.Rows.Count - 1][count] = FileRec;
                    //                count++;
                    //            }
                    //        }
                    //    }
                    //}
                    
                    //using (var connection = new MySqlConnection(ConnectionString+ ";AllowLoadLocalInfile=True;"))
                    //{
                    //    connection.Open();
                    //    var bulkCopy = new MySqlBulkCopy(connection);
                    //    bulkCopy.DestinationTableName = "categories";




                    //    bulkCopy.ColumnMappings.Add(new MySqlBulkCopyColumnMapping{ SourceOrdinal = 0,DestinationColumn = "Id"});
                    //    bulkCopy.ColumnMappings.Add(new MySqlBulkCopyColumnMapping { SourceOrdinal = 1, DestinationColumn = "Name" });
                    //    //bulkCopy.ColumnMappings.AddRange(GetMySqlColumnMapping(tblcsv));

                    //    bulkCopy.WriteToServer(tblcsv);
                            

                    //    connection.Close();

                    //}
                    //SET GLOBAL local_infile = 1


                        if (productVM.Product.ImageUrl != null)
                        {
                            // this is an edit and we need to remove old image
                            var imagePath = Path.Combine(webRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                            }
                        }
                    using (var filesStreams = new FileStream(Path.Combine(upload, fileName + extention), FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }
                    productVM.Product.ImageUrl = @"\images\products\" + fileName + extention;
                }
                else
                {
                    //update when they not change the image
                    if (productVM.Product.Id != 0)
                    {
                        Product objFromDb = _unitOfWork.Product.Get(productVM.Product.Id);
                        productVM.Product.ImageUrl = objFromDb.ImageUrl;
                    }
                }

                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString(),
                });
                productVM.CoverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString(),
                });
                if (productVM.Product.Id != 0)
                {
                    productVM.Product = _unitOfWork.Product.Get(productVM.Product.Id);
                }
            }
            return View(productVM);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Product.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            string webRootPath = _hostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRootPath, objFromDb.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _unitOfWork.Product.Remove(objFromDb);            
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }

}
