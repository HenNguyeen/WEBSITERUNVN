using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DBStoreSport.Models;

namespace DBStoreSport.Controllers
{
    public class CustomersController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        // GET: Customers
        public ActionResult Index()
        {
            
            return View(db.Customers.ToList());
        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDCus,NameCus,PhoneCus,EmailCus,UserName,Email,Password,PasswordUser")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                // 1. Thêm khách hàng
                db.Customers.Add(customer);
                db.SaveChanges();

                // 2. Đồng thời tạo tài khoản đăng nhập
                var newUser = new AdminUser
                {
                    UserName = customer.UserName,
                    Email = customer.EmailCus,
                    PasswordUser = customer.Password, // ⚠️ nếu project dùng mã hóa mật khẩu, cần hash ở đây
                    RoleUser = "user"
                };
                db.AdminUsers.Add(newUser);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDCus,NameCus,PhoneCus,EmailCus,UserName,Password")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Xem thông tin cá nhân
        public new ActionResult Profile()
        {
            if (Session["UserID"] == null)
                return Content("Chưa đăng nhập hoặc Session hết hạn");

            int id;
            try
            {
                id = Convert.ToInt32(Session["UserID"]);
            }
            catch
            {
                return Content("Session[\"UserID\"] không hợp lệ: " + Session["UserID"]);
            }

            Customer customer = db.Customers.Find(id);
            if (customer == null)
                return Content("Không tìm thấy khách hàng với IDCus = " + id);

            return View(customer);
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
