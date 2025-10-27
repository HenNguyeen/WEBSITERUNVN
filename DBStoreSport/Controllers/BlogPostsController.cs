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
    public class BlogPostsController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        // GET: BlogPosts
       
        public ActionResult Index()
        {
            var blogs = db.BlogPosts.OrderByDescending(b => b.CreatedDate).ToList();
            var productList = db.Products.ToList(); // để dùng khi xử lý liên quan

            // Tạo dictionary chứa các sản phẩm liên quan cho từng blog
            var relatedDict = new Dictionary<int, List<Product>>();
            foreach (var blog in blogs)
            {
                if (!string.IsNullOrEmpty(blog.RelatedProductIds))
                {
                    var ids = blog.RelatedProductIds.Split(',').Select(Int32.Parse).ToList();
                    var related = productList.Where(p => ids.Contains(p.ProductID)).ToList();
                    relatedDict[blog.Id] = related;
                }
            }

            ViewBag.RelatedProducts = relatedDict;
            return View(db.BlogPosts.ToList());
        }
        public ActionResult Lietke()
        {
            var posts = db.BlogPosts.ToList();

            if (posts.Count == 0)
            {
                ViewBag.Message = "Không có bài viết nào.";
                return View(posts); // Dù trống, vẫn return view
            }

            return View(posts);
        }


        // GET: BlogPosts/Details/5
        public ActionResult Detail(int id)
        {
            var blog = db.BlogPosts.FirstOrDefault(x => x.Id == id && x.IsActive == true);
            if (blog == null)
                return HttpNotFound();

            // Load sản phẩm liên quan nếu có
            List<Product> relatedProducts = new List<Product>();
            if (!string.IsNullOrEmpty(blog.RelatedProductIds))
            {
                var ids = blog.RelatedProductIds.Split(',').Select(Int32.Parse).ToList();
                relatedProducts = db.Products.Where(p => ids.Contains(p.ProductID)).ToList();
            }

            ViewBag.RelatedProducts = relatedProducts;
            return View(blog);
        }


        // GET: BlogPosts/Create
        public ActionResult Create()
        {
            if (Session["RoleUser"]?.ToString() != "admin")
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase uploadImage, BlogPost blogPost)
        {
            if (Session["RoleUser"]?.ToString() != "admin")
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            if (ModelState.IsValid)
            {
                // Nếu có ảnh
                if (uploadImage != null && uploadImage.ContentLength > 0)
                {
                    // Tạo thư mục nếu chưa có
                    string folderPath = Server.MapPath("~/Content/images");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string fileName = Path.GetFileName(uploadImage.FileName);
                    string path = Path.Combine(folderPath, fileName);

                    uploadImage.SaveAs(path);

                    // Gán đường dẫn tương đối (dùng Url.Content sau này)
                    blogPost.Image = "/Content/images/" + fileName;
                }

                blogPost.CreatedDate = DateTime.Now;
                db.BlogPosts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blogPost);
        }


        // GET: BlogPosts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["RoleUser"]?.ToString() != "admin")
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Content,Image,CreatedDate,IsActive")] HttpPostedFileBase uploadImage, BlogPost blogPost)
        {
            if (ModelState.IsValid)
            {
                db.Entry(blogPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            if (uploadImage != null && uploadImage.ContentLength > 0)
            {
                string fileName = Path.GetFileName(uploadImage.FileName);
                string path = Path.Combine(Server.MapPath("~/Content/UploadImage"), fileName);
                uploadImage.SaveAs(path);

                // Lưu đường dẫn ảo vào CSDL để hiển thị trên web
                blogPost.Image = "~/Content/UploadImage/" + fileName;
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["RoleUser"]?.ToString() != "admin")
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["RoleUser"]?.ToString() != "admin")
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            BlogPost blogPost = db.BlogPosts.Find(id);
            db.BlogPosts.Remove(blogPost);
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
    }
}
