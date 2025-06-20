using Pharmacy.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Pharmacy.Controllers
{
    public class HomeController : Controller
    {
        private PharmacyContext db = new PharmacyContext();

        private int GetCurrentUserId()
        {
            if (Session["UserId"] != null)
            {
                return (int)Session["UserId"];
            }
            return 0; 
        }


        public ActionResult Index(string selectedMedicine = null, string selectedManufacturer = null, string sortOrder = null, int page = 1)
        {
            int pageSize = 5;

            var query = db.Medicines.AsQueryable();

            if (!string.IsNullOrEmpty(selectedMedicine))
            {
                query = query.Where(m => m.Name == selectedMedicine);
            }

            if (!string.IsNullOrEmpty(selectedManufacturer))
            {
                query = query.Where(m => m.Manufacturer == selectedManufacturer);
            }

            switch (sortOrder)
            {
                case "price_asc":
                    query = query.OrderBy(m => m.Price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(m => m.Price);
                    break;
                default:
                    query = query.OrderBy(m => m.Id);
                    break;
            }

            var totalItems = query.Count();

            var medicines = query
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            var allNames = db.Medicines.Select(m => m.Name).Distinct().ToList();
            var allManufacturers = db.Medicines.Select(m => m.Manufacturer).Distinct().ToList();

            var model = new MedicinesListViewModel
            {
                Medicines = medicines,
                SelectedMedicine = selectedMedicine,
                SelectedManufacturer = selectedManufacturer,
                MedicineNames = new SelectList(allNames),
                Manufacturers = new SelectList(allManufacturers),
                PageInfo = new PageInfo
                {
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalItems = totalItems
                },
                SortOrder = sortOrder
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult Buy(int id)
        {
            var medicine = db.Medicines.Find(id);
            if (medicine == null)
            {
                TempData["ErrorMessage"] = "Препарат не знайдено.";
                return RedirectToAction("Index");
            }

            ViewBag.MedicineId = id;
            ViewBag.AvailableQuantity = medicine.Quantity; 

            return View();
        }

        [HttpPost]
        public ActionResult Buy(Purchase purchase)
        {
            var medicine = db.Medicines.Find(purchase.MedicineId);
            if (medicine == null)
            {
                ViewBag.ErrorMessage = "Помилка: препарат не знайдено.";
                ViewBag.MedicineId = purchase.MedicineId;
                ViewBag.AvailableQuantity = 0;
                return View();
            }

            if (purchase.Quantity <= 0)
            {
                ViewBag.ErrorMessage = "Помилка: кількість має бути більшою за 0.";
                ViewBag.MedicineId = purchase.MedicineId;
                ViewBag.AvailableQuantity = medicine.Quantity;
                return View();
            }

            if (medicine.Quantity < purchase.Quantity)
            {
                ViewBag.ErrorMessage = $"Помилка: недостатньо препарату в наявності. Доступно: {medicine.Quantity}";
                ViewBag.MedicineId = purchase.MedicineId;
                ViewBag.AvailableQuantity = medicine.Quantity;
                return View();
            }

            medicine.Quantity -= purchase.Quantity;

            purchase.Date = DateTime.Now;
            purchase.UserId = GetCurrentUserId();

            db.Purchases.Add(purchase);
            db.SaveChanges();

            ViewBag.PurchaseSuccess = true;
            ViewBag.Person = purchase.Person;
            ViewBag.Quantity = purchase.Quantity;
            ViewBag.MedicineName = medicine.Name;

            return View();
        }


        public ActionResult MedicineView(int id)
        {
            var medicine = db.Medicines.Find(id);
            if (medicine != null)
            {
                return View(medicine);
            }

            TempData["ErrorMessage"] = "Препарат не знайдено.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult EditMedicine(int id)
        {
            var medicine = db.Medicines.Find(id);
            if (medicine == null)
            {
                return HttpNotFound();
            }
            return View(medicine);
        }

        [HttpPost]
        public ActionResult EditMedicine(Medicine medicine)
        {
            if (ModelState.IsValid)
            {
                db.Entry(medicine).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(medicine);
        }

        public ActionResult CreateMedicine()
        {
            return View();
        }



        [HttpPost]
        public ActionResult CreateMedicine(Medicine medicine, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(medicine.AdditionalInfo))
                    {
                        medicine.AdditionalInfo = medicine.AdditionalInfo.Trim();
                    }

                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(imageFile.FileName);
                        var imagePath = Path.Combine(Server.MapPath("~/Images/"), fileName);
                        imageFile.SaveAs(imagePath);
                            
                        medicine.ImagePath = "/Images/" + fileName;
                    }

                    db.Medicines.Add(medicine);
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Препарат успішно додано!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Помилка при збереженні препарату: " + ex.Message);
                }
            }

            return View(medicine);
        }


        public ActionResult DeleteMedicine(int id)
        {
            var medicine = db.Medicines.Find(id);
            if (medicine != null)
            {

                var relatedPurchases = db.Purchases.Where(p => p.MedicineId == id).ToList();
                db.Purchases.RemoveRange(relatedPurchases);

                db.Medicines.Remove(medicine);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        public ActionResult PurchaseList()
        {
       
            int currentUserId = GetCurrentUserId();
            var purchases = db.Purchases
                .Include("Medicine")
                .Where(p => p.UserId == currentUserId)
                .ToList();
            return View(purchases);
        }

        public ActionResult Reports()
        {
            int currentUserId = GetCurrentUserId();

            var reportData = db.Purchases
                .Include(p => p.Medicine)
                .Where(p => p.UserId == currentUserId)
                .GroupBy(p => p.Medicine.Name)
                .Select(g => new MedicineReportViewModel
                {
                    MedicineName = g.Key,
                    PurchaseCount = g.Sum(p => p.Quantity), 
                    TotalAmount = g.Sum(p => p.Medicine.Price * p.Quantity) 
                })
                .OrderByDescending(r => r.TotalAmount)
                .ToList();

            return View(reportData);
        }


        public ActionResult Welcome()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}