using System;
using System.Linq;
using System.Web.Mvc;
using DBStoreSport.Models;

namespace DBStoreSport.Controllers
{
    public class ProductReviewController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();


        public ActionResult Create(int productId, int rating, string comment)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["UserID"];
            var customer = db.Customers.FirstOrDefault(c => c.IDCus == userId);
            if (customer == null)
                return RedirectToAction("Login", "Account");

            var review = new ProductReview
            {
                ProductID = productId,
                IDCus = customer.IDCus,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.Now
            };
            db.ProductReviews.Add(review);
            db.SaveChanges();

            return RedirectToAction("Details", "Products", new { id = productId });
        }

    }
}