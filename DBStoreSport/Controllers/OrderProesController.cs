using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DBSportStore.Models;
using DBStoreSport.Models;

namespace DBStoreSport.Controllers
{
    public class OrderProesController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        // GET: OrderProes
        public ActionResult Index()
        {
            var orderProes = db.OrderProes.Include(o => o.Customer);
            return View(orderProes.ToList());
        }

        // GET: OrderProes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderPro orderPro = db.OrderProes.Find(id);
            if (orderPro == null)
            {
                return HttpNotFound();
            }
            return View(orderPro);
        }

        // GET: OrderProes/Create
        public ActionResult Create()
        {
            ViewBag.IDCus = new SelectList(db.Customers, "IDCus", "NameCus");
            return View();
        }

        // POST: OrderProes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,DateOrder,IDCus,AddressDeliverry")] OrderPro orderPro)
        {
            if (ModelState.IsValid)
            {
                db.OrderProes.Add(orderPro);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDCus = new SelectList(db.Customers, "IDCus", "NameCus", orderPro.IDCus);
            return View(orderPro);
        }

        // GET: OrderProes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderPro orderPro = db.OrderProes.Find(id);
            if (orderPro == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDCus = new SelectList(db.Customers, "IDCus", "NameCus", orderPro.IDCus);
            return View(orderPro);
        }

        // POST: OrderProes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,DateOrder,IDCus,AddressDeliverry")] OrderPro orderPro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderPro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDCus = new SelectList(db.Customers, "IDCus", "NameCus", orderPro.IDCus);
            return View(orderPro);
        }

        // GET: OrderProes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderPro orderPro = db.OrderProes.Find(id);
            if (orderPro == null)
            {
                return HttpNotFound();
            }
            return View(orderPro);
        }

        // POST: OrderProes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderPro orderPro = db.OrderProes.Find(id);
            db.OrderProes.Remove(orderPro);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult PendingOrders()
        {
            var pendingOrders = db.OrderProes
                .Include(o => o.Customer)
                .Where(o => o.Status == "Chờ xác nhận")
                .OrderByDescending(o => o.DateOrder)
                .ToList();
            return View(pendingOrders);
        }

        // Xác nhận đơn hàng -> chuyển sang "Đang giao"
        [HttpPost]
        public ActionResult ConfirmOrder(int id)
        {
            var order = db.OrderProes.Find(id);
            if (order != null)
            {
                order.Status = "Đang giao";
                db.SaveChanges();
            }
            return RedirectToAction("PendingOrders");
        }

        // Hủy đơn hàng
        [HttpPost]
        public ActionResult CancelOrder(int id)
        {
            var order = db.OrderProes.Find(id);
            if (order == null) return HttpNotFound();

            if (order.Status == "Chờ xác nhận")
            {
                order.Status = "Đã hủy";
                db.SaveChanges();
                TempData["Error"] = "❌ Đơn hàng đã bị hủy!";
            }
            return RedirectToAction("PendingOrders");
        }

        // Đánh dấu đơn hàng giao thành công
        [HttpPost]
        public ActionResult CompleteOrder(int id)
        {
            var order = db.OrderProes.Find(id);
            if (order == null) return HttpNotFound();

            if (order.Status == "Đang giao")
            {
                order.Status = "Giao thành công";
                db.SaveChanges();
                TempData["Message"] = "🎉 Đơn hàng đã giao thành công!";
            }
            return RedirectToAction("PendingOrders");
        }
        


    }
}
