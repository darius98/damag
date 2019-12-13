using Damag.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Web;

namespace Damag.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var items = db.Items.Include("Category")
                                .Include("User")
                                .Where(item => item.Approved);

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            ViewBag.UserId = User.Identity.GetUserId();
            ViewBag.Items = items;

            return View();
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult NotApproved()
        {
            var items = db.Items.Include("Category")
                                .Include("User")
                                .Where(item => !item.Approved);

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            ViewBag.UserId = User.Identity.GetUserId();
            ViewBag.Items = items;

            return View();
        }

        [Authorize(Roles = "Collaborator")]
        public ActionResult Mine()
        {
            var currentUserId = User.Identity.GetUserId();
            var items = db.Items.Include("Category")
                                .Include("User")
                                .Where(item => item.UserId == currentUserId);
            ViewBag.UserId = currentUserId;
            ViewBag.Items = items;
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Approve(int id)
        {
            Item item = db.Items.Find(id);
            item.Approved = true;
            if (TryUpdateModel(item))
            {
                item.Approved = true;
                TempData["message"] = "Item approved.";
                db.SaveChanges();
            }
            return RedirectToAction("NotApproved");
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Reject(int id)
        {
            Item item = db.Items.Find(id);
            db.Items.Remove(item);
            db.SaveChanges();
            TempData["message"] = "Item rejected.";
            return RedirectToAction("NotApproved");
        }

        public ActionResult Show(int id)
        {
            Item item = db.Items.Find(id);

            return View(item);
        }

        [Authorize(Roles = "Collaborator,Administrator")]
        public ActionResult New()
        {
            return View(new Item
            {
                Categories = GetAllCategories(),
                UserId = User.Identity.GetUserId(),
                Approved = false,
            });

        }

        [HttpPost]
        [Authorize(Roles = "Collaborator,Administrator")]
        public ActionResult New(Item item)
        {
            item.Approved = false;
            item.Categories = GetAllCategories();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Items.Add(item);
                    db.SaveChanges();
                    TempData["message"] = "Item added successfully. Pending administrator approval.";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(item);
                }
            }
            catch (Exception)
            {
                return View(item);
            }
        }

        [Authorize(Roles = "Collaborator,Administrator")]
        public ActionResult Edit(int id)
        {
            Item item = db.Items.Find(id);
            ViewBag.Item = item;
            item.Categories = GetAllCategories();

            if (item.UserId == User.Identity.GetUserId() ||
                User.IsInRole("Administrator"))
            {
                return View(item);
            }
            else
            {
                TempData["message"] = "Cannot edit non-owned item.";
                return RedirectToAction("Index");
            }

        }

        [HttpPut]
        [Authorize(Roles = "Collaborator,Administrator")]
        public ActionResult Edit(int id, Item requestItem)
        {
            requestItem.Categories = GetAllCategories();
            try
            {
                if (ModelState.IsValid)
                {
                    Item item = db.Items.Find(id);
                    if (item.UserId == User.Identity.GetUserId() ||
                        User.IsInRole("Administrator"))
                    {
                        if (TryUpdateModel(item))
                        {
                            item.Name = requestItem.Name;
                            item.Description = requestItem.Description;
                            item.Price = requestItem.Price;
                            item.CategoryId = requestItem.CategoryId;
                            db.SaveChanges();
                            TempData["message"] = "Changes saved successfully.";
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["message"] = "Cannot edit non-owned item.";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    return View(requestItem);
                }
            }
            catch (Exception)
            {
                return View(requestItem);
            }
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            var selectList = new List<SelectListItem>();
            var categories = from category in db.Categories
                             orderby category.CategoryName
                             select category;
            foreach (var category in categories)
            {
                selectList.Add(new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }
            return selectList;
        }
    }
}