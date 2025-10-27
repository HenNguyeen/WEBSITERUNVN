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
    public class AdminVoucherController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        // Kiểm tra quyền admin
        private bool IsAdmin()
        {
            return Session["RoleUser"] != null && Session["RoleUser"].ToString() == "admin";
        }

        // GET: AdminVoucher
        public ActionResult Index()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var vouchers = db.Vouchers.OrderByDescending(v => v.ExpiryDate).ToList();
            return View(vouchers);
        }

        // GET: AdminVoucher/Details/5
        public ActionResult Details(int? id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voucher voucher = db.Vouchers.Find(id);
            if (voucher == null)
            {
                return HttpNotFound();
            }
            return View(voucher);
        }

        // GET: AdminVoucher/Create
        public ActionResult Create()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            return View();
        }

        // POST: AdminVoucher/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Code,DiscountValue,Type,ExpiryDate,IsUsed")] Voucher voucher)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            // Validation bổ sung
            if (voucher.ExpiryDate <= DateTime.Now)
            {
                ModelState.AddModelError("ExpiryDate", "Ngày hết hạn phải sau ngày hiện tại!");
            }

            if (voucher.Type == "Percent" && (voucher.DiscountValue < 1 || voucher.DiscountValue > 100))
            {
                ModelState.AddModelError("DiscountValue", "Phần trăm giảm giá phải từ 1% đến 100%!");
            }

            if (voucher.Type == "Fixed" && voucher.DiscountValue <= 0)
            {
                ModelState.AddModelError("DiscountValue", "Số tiền giảm phải lớn hơn 0!");
            }

            if (ModelState.IsValid)
            {
                // Kiểm tra mã voucher đã tồn tại chưa
                var existingVoucher = db.Vouchers.FirstOrDefault(v => v.Code == voucher.Code);
                if (existingVoucher != null)
                {
                    ModelState.AddModelError("Code", "Mã voucher này đã tồn tại!");
                    return View(voucher);
                }

                // Đặt giá trị mặc định cho IsUsed
                voucher.IsUsed = false;
                
                db.Vouchers.Add(voucher);
                db.SaveChanges();
                TempData["Success"] = "Tạo voucher thành công!";
                return RedirectToAction("Index");
            }

            return View(voucher);
        }

        // GET: AdminVoucher/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voucher voucher = db.Vouchers.Find(id);
            if (voucher == null)
            {
                return HttpNotFound();
            }
            return View(voucher);
        }

        // POST: AdminVoucher/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VoucherID,Code,DiscountValue,Type,ExpiryDate,IsUsed")] Voucher voucher)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            // Validation bổ sung
            if (voucher.ExpiryDate <= DateTime.Now)
            {
                ModelState.AddModelError("ExpiryDate", "Ngày hết hạn phải sau ngày hiện tại!");
            }

            if (voucher.Type == "Percent" && (voucher.DiscountValue < 1 || voucher.DiscountValue > 100))
            {
                ModelState.AddModelError("DiscountValue", "Phần trăm giảm giá phải từ 1% đến 100%!");
            }

            if (voucher.Type == "Fixed" && voucher.DiscountValue <= 0)
            {
                ModelState.AddModelError("DiscountValue", "Số tiền giảm phải lớn hơn 0!");
            }

            if (ModelState.IsValid)
            {
                // Kiểm tra mã voucher đã tồn tại chưa (trừ voucher hiện tại)
                var existingVoucher = db.Vouchers.FirstOrDefault(v => v.Code == voucher.Code && v.VoucherID != voucher.VoucherID);
                if (existingVoucher != null)
                {
                    ModelState.AddModelError("Code", "Mã voucher này đã tồn tại!");
                    return View(voucher);
                }

                db.Entry(voucher).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Cập nhật voucher thành công!";
                return RedirectToAction("Index");
            }
            return View(voucher);
        }

        // GET: AdminVoucher/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voucher voucher = db.Vouchers.Find(id);
            if (voucher == null)
            {
                return HttpNotFound();
            }
            return View(voucher);
        }

        // POST: AdminVoucher/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            Voucher voucher = db.Vouchers.Find(id);
            if (voucher != null)
            {
                db.Vouchers.Remove(voucher);
                db.SaveChanges();
                TempData["Success"] = "Xóa voucher thành công!";
            }
            return RedirectToAction("Index");
        }

        // Action để test encoding
        public ActionResult TestEncoding()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            return View();
        }

        // Action để tạo voucher mẫu (chỉ dùng cho development)
        public ActionResult CreateSampleVouchers()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            try
            {
                // Tạo một số voucher mẫu
                var sampleVouchers = new List<Voucher>
                {
                    new Voucher
                    {
                        Code = "SALE2024",
                        DiscountValue = 10,
                        Type = "Percent",
                        ExpiryDate = DateTime.Now.AddMonths(3),
                        IsUsed = false
                    },
                    new Voucher
                    {
                        Code = "GIAM50K",
                        DiscountValue = 50000,
                        Type = "Fixed",
                        ExpiryDate = DateTime.Now.AddMonths(1),
                        IsUsed = false
                    },
                    new Voucher
                    {
                        Code = "WELCOME20",
                        DiscountValue = 20,
                        Type = "Percent",
                        ExpiryDate = DateTime.Now.AddDays(30),
                        IsUsed = false
                    },
                    new Voucher
                    {
                        Code = "FREESHIP",
                        DiscountValue = 30000,
                        Type = "Fixed",
                        ExpiryDate = DateTime.Now.AddDays(15),
                        IsUsed = false
                    }
                };

                foreach (var voucher in sampleVouchers)
                {
                    // Kiểm tra xem voucher đã tồn tại chưa
                    var existingVoucher = db.Vouchers.FirstOrDefault(v => v.Code == voucher.Code);
                    if (existingVoucher == null)
                    {
                        db.Vouchers.Add(voucher);
                    }
                }

                db.SaveChanges();
                TempData["Success"] = "Đã tạo voucher mẫu thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
            }

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
    }
} 