using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DBStoreSport.Models;

namespace DBStoreSport.Controllers
{
    public class PaymentControllerSimple : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        // GET: Payment
        public ActionResult Index()
        {
            return View();
        }

        // Trang thành công - đơn giản để test
        public ActionResult PaymentSuccess(int orderId)
        {
            var order = db.OrderProes.Find(orderId);
            if (order == null)
            {
                return RedirectToAction("Index", "Home");
            }
            
            ViewBag.OrderId = orderId;
            ViewBag.TotalAmount = order.OrderDetails.Sum(od => od.UnitPrice * od.Quantity);
            ViewBag.TransactionId = "TEST_" + DateTime.Now.Ticks;
            
            // Xóa giỏ hàng
            Session["Cart"] = null;
            Session["Voucher"] = null;
            
            return View();
        }

        // Trang thất bại
        public ActionResult PaymentFailed(string message = "")
        {
            ViewBag.ErrorMessage = message;
            return View();
        }

        // Hủy thanh toán
        public ActionResult PaymentCancelled()
        {
            ViewBag.Message = "Đã hủy thanh toán thành công";
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
