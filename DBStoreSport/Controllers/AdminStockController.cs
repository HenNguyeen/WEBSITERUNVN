using DBStoreSport.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DBStoreSport.Controllers
{
    public class AdminStockController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        // GET: Danh sách tồn kho
        public ActionResult Index()
        {
            var products = db.Products.ToList();
            return View(products);
        }

        // POST: Nhập kho
        [HttpPost]
        public ActionResult ImportStock(int productId, int quantity)
        {
            var product = db.Products.Find(productId);
            if (product == null)
                return HttpNotFound();

            if (quantity <= 0)
            {
                TempData["Error"] = "Số lượng nhập phải lớn hơn 0!";
                return RedirectToAction("Index");
            }

            product.Quantity += quantity;

            // Lưu lịch sử nhập kho
            db.ProductStockHistories.Add(new ProductStockHistory
            {
                ProductID = productId,
                QuantityChange = quantity,
                ActionType = "Nhập kho",
                Note = "Nhập thêm hàng mới",
                AdminUser = Session["UserName"]?.ToString(),
                ActionDate = DateTime.Now
            });

            db.SaveChanges();
            TempData["Message"] = "✅ Nhập kho thành công!";
            return RedirectToAction("Index");
        }

        // POST: Xuất kho
        [HttpPost]
        public ActionResult ExportStock(int productId, int quantity)
        {
            var product = db.Products.Find(productId);
            if (product == null)
                return HttpNotFound();

            if (quantity <= 0)
            {
                TempData["Error"] = "Số lượng xuất phải lớn hơn 0!";
                return RedirectToAction("Index");
            }

            if (product.Quantity < quantity)
            {
                TempData["Error"] = "❌ Không đủ hàng trong kho để xuất!";
                return RedirectToAction("Index");
            }

            product.Quantity -= quantity;

            // Lưu lịch sử xuất kho
            db.ProductStockHistories.Add(new ProductStockHistory
            {
                ProductID = productId,
                QuantityChange = -quantity,
                ActionType = "Xuất kho",
                Note = "Xuất hàng ra cửa hàng",
                AdminUser = Session["UserName"]?.ToString(),
                ActionDate = DateTime.Now
            });

            db.SaveChanges();
            TempData["Message"] = "✅ Xuất kho thành công!";
            return RedirectToAction("Index");
        }
        public ActionResult History()
        {
            var history = db.ProductStockHistories
                            .OrderByDescending(h => h.ActionDate)
                            .ToList();
            return View("History", history);
        }
    }
}
