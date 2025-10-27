using System;
using System.Collections.Generic;
using System.Linq;                  // ✅ Bắt buộc để dùng Where(), ToList()
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;          // ✅ Cần nếu bạn dùng .Include()
using DBStoreSport.Models;


namespace DBStoreSport.Controllers
{
    public class UserOrderController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        public ActionResult OrderHistory()
        {
            int? idUser = Session["UserID"] as int?;
            if (idUser == null) return RedirectToAction("Login", "Account");

            var orders = db.OrderProes
                           .Where(o => o.IDCus == idUser)
                           .OrderByDescending(o => o.DateOrder)
                           .ToList();

            return View(orders);
        }

        public ActionResult OrderDetail(int id)
        {
            var details = db.OrderDetails
                .Where(d => d.IDOrder == id)
                .Include("Product")
                .ToList();

            return View(details);
        }
    }
}