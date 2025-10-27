using DBStoreSport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
namespace DBStoreSport.Controllers
{
    public class AdminReviewController : Controller
    {
        // GET: AdminReview
        private DBSportStoreEntities db = new DBSportStoreEntities();

        // Chỉ cho admin truy cập
        private bool IsAdmin()
        {
            return Session["RoleUser"] != null && Session["RoleUser"].ToString() == "admin";
        }

        public ActionResult Index()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var reviews = db.ProductReviews
             .Include(r => r.Product)
             .Include(r => r.Customer)
             .OrderByDescending(r => r.CreatedAt)
             .ToList();
            return View(reviews);
        }

        public ActionResult Delete(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var review = db.ProductReviews.Find(id);
            if (review != null)
            {
                db.ProductReviews.Remove(review);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Reply(int id, string reply)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var review = db.ProductReviews.Find(id);
            if (review != null)
            {
                review.AdminReply = reply;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}