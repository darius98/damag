using Damag.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Damag.Controllers
{

    [Authorize(Roles = "Administrator")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            var categories = from category in db.Categories
                             orderby category.CategoryName
                             select category;
            ViewBag.Categories = categories;
            return View();
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(Category cat)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Categories.Add(cat);
                    db.SaveChanges();
                    TempData["message"] = "Category added successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(cat);
                }
            }
            catch (Exception)
            {
                return View(cat);
            }
        }

        public ActionResult Edit(int id)
        {
            Category category = db.Categories.Find(id);
            return View(category);
        }

        [HttpPut]
        public ActionResult Edit(int id, Category requestCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Category category = db.Categories.Find(id);
                    if (TryUpdateModel(category))
                    {
                        category.CategoryName = requestCategory.CategoryName;
                        TempData["message"] = "Category changed successfully.";
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(requestCategory);
                }
            }
            catch (Exception)
            {
                return View(requestCategory);
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            TempData["message"] = "Category deleted successfully.";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}