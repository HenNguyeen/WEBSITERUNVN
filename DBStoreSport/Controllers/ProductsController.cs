using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DBStoreSport.Models;

namespace DBStoreSport.Controllers
{
    public class ProductsController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        // GET: Products
        public ActionResult Index(string searchString, string categoryFilter, string priceFilter, string sortOrder)
        {
            var products = db.Products.Include(p => p.Category).AsQueryable();

            // Tìm kiếm theo tên sản phẩm
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.NamePro.Contains(searchString) || p.DecriptionPro.Contains(searchString));
            }

            // Lọc theo danh mục
            if (!string.IsNullOrEmpty(categoryFilter) && categoryFilter != "all")
            {
                products = products.Where(p => p.Category.NameCate == categoryFilter);
            }

            // Lọc theo giá
            if (!string.IsNullOrEmpty(priceFilter))
            {
                switch (priceFilter)
                {
                    case "under100k":
                        products = products.Where(p => p.Price < 100000);
                        break;
                    case "100k-500k":
                        products = products.Where(p => p.Price >= 100000 && p.Price <= 500000);
                        break;
                    case "500k-1m":
                        products = products.Where(p => p.Price > 500000 && p.Price <= 1000000);
                        break;
                    case "over1m":
                        products = products.Where(p => p.Price > 1000000);
                        break;
                }
            }

            // Sắp xếp
            switch (sortOrder)
            {
                case "name_asc":
                    products = products.OrderBy(p => p.NamePro);
                    break;
                case "name_desc":
                    products = products.OrderByDescending(p => p.NamePro);
                    break;
                case "price_asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                default:
                    products = products.OrderBy(p => p.ProductID);
                    break;
            }

            // Lấy danh sách danh mục cho dropdown
            ViewBag.Categories = db.Categories.OrderBy(c => c.NameCate).ToList();
            ViewBag.SearchString = searchString;
            ViewBag.CategoryFilter = categoryFilter;
            ViewBag.PriceFilter = priceFilter;
            ViewBag.SortOrder = sortOrder;

            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CateID = new SelectList(db.Categories, "IDCate", "NameCate");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NamePro,DecriptionPro,CateID,Price,Quantity")] Product product, HttpPostedFileBase imageFile)
        {
            try
            {
                // Xử lý file upload
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    // Kiểm tra loại file
                    var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                    if (!allowedTypes.Contains(imageFile.ContentType))
                    {
                        ModelState.AddModelError("ImagePro", "Chỉ chấp nhận file hình ảnh (JPG, PNG, GIF)");
                    }
                    else
                    {
                        // Tạo tên file unique
                        var fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss_") + Path.GetFileName(imageFile.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/images/"), fileName);
                        
                        // Lưu file
                        imageFile.SaveAs(path);
                        product.ImagePro = fileName;
                    }
                }

                // Validation bổ sung
                if (string.IsNullOrWhiteSpace(product.NamePro))
                {
                    ModelState.AddModelError("NamePro", "Tên sản phẩm không được để trống");
                }

                if (product.Price <= 0)
                {
                    ModelState.AddModelError("Price", "Giá sản phẩm phải lớn hơn 0");
                }

                if(product.Quantity <= 0)
                {
                    ModelState.AddModelError("Quantity", "Số tồn phải lớn hơn 0");
                }
                if (ModelState.IsValid)
                {
                    db.Products.Add(product);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
            }

            ViewBag.CateID = new SelectList(db.Categories, "IDCate", "NameCate", product.CateID);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CateID = new SelectList(db.Categories, "IDCate", "NameCate", product.CateID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,NamePro,DecriptionPro,CateID,Price,ImagePro,Quantity")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CateID = new SelectList(db.Categories, "IDCate", "NameCate", product.CateID);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
        // GET: Products
        public ActionResult ProductList(int? category, string SearchString, double min = double.MinValue, double max = double.MaxValue, string sortOrder = "")
        {
            // Lấy danh sách sản phẩm từ DB và bao gồm luôn Category
            var products = db.Products.Include(p => p.Category).AsQueryable();

            // Tìm kiếm theo chuỗi (tên sản phẩm)
            if (!String.IsNullOrEmpty(SearchString))
            {
                products = products.Where(s => s.NamePro.Contains(SearchString.Trim()));
            }

            // Lọc theo khoảng giá
            if (min >= 0 && max > 0)
            {
                products = products.Where(p => (double)p.Price >= min && (double)p.Price <= max);
            }

            // Lọc theo danh mục
            if (category != null)
            {
                products = products.Where(x => x.CateID == category);
            }

            // Sắp xếp (tăng/giảm theo tên sản phẩm)
            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(x => x.NamePro);
                    break;
                case "price":
                    products = products.OrderBy(x => x.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(x => x.Price);
                    break;
                default:
                    products = products.OrderBy(x => x.NamePro); // mặc định tăng dần theo tên
                    break;
            }

          

            // Truyền trạng thái sắp xếp hiện tại về View để giữ trạng thái
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentSearch = SearchString;
            ViewBag.CurrentCategory = category;
            ViewBag.CurrentMin = min;
            ViewBag.CurrentMax = max;

            return View(products.ToList());
        }


    }
}
