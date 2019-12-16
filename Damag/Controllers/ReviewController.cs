using Damag.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Damag.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult New(Review review)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Reviews.Add(review);
                    db.SaveChanges();
                    TempData["message"] = "Review added successfully.";
                    return Redirect("/Items/Show/" + review.ItemId.ToString());
                }
                else
                {
                    TempData["error"] = "An error occured. Review not sent.";
                    return Redirect("/Items/Show/" + review.ItemId.ToString());
                }
            }
            catch (Exception)
            {
                TempData["error"] = "An error occured. Review not sent.";
                return Redirect("/Items/Show/" + review.ItemId.ToString());
            }
        }

        public ActionResult Delete(int id)
        {
            Review review = db.Reviews.Find(id);
            int itemId = review.ItemId;
            db.Reviews.Remove(review);
            TempData["message"] = "Review deleted successfully.";
            db.SaveChanges();
            return Redirect("/Items/Show/" + itemId.ToString());
        }

        public ActionResult Edit(int id)
        {
            Review review = db.Reviews.Find(id);
            return View(review);
        }

        [HttpPut]
        public ActionResult Edit(int id, Review requestReview)
        {
            Review review = db.Reviews.Find(id);
            try
            {
                if (ModelState.IsValid)
                {
                    if (TryUpdateModel(review))
                    {
                        review.Text = requestReview.Text;
                        TempData["message"] = "Review changed successfully.";
                        db.SaveChanges();
                    }
                    return Redirect("/Items/Show/" + review.ItemId.ToString());
                }
                else
                {
                    TempData["error"] = "An error occured. Review not sent.";
                    return Redirect("/Items/Show/" + review.ItemId.ToString());
                }
            }
            catch (Exception)
            {
                TempData["error"] = "An error occured. Review not sent.";
                return Redirect("/Items/Show/" + review.ItemId.ToString());
            }
        }
    }
}
