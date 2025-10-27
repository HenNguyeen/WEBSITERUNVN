using System.Linq;
using System.Web.Mvc;
using DBStoreSport.Models;
using BCrypt.Net;
using System.Web;


namespace DBStoreSport.Controllers
{
    public class AccountController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities(); // Entity Framework context

        // 1. GET: Login
        public ActionResult Login()
        {
            return View();
        }

        // 2. POST: Login
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.AdminUsers.FirstOrDefault(u => u.UserName == model.Username && u.PasswordUser == model.Password);
                if (user != null)
                {
                    // Lấy IDCus từ Customer dựa vào UserName
                    var customer = db.Customers.FirstOrDefault(c => c.UserName == user.UserName);
                    if (customer != null)
                        Session["UserID"] = customer.IDCus;
                    else
                        Session["UserID"] = user.ID; // fallback nếu không tìm thấy
                    Session["UserName"] = user.UserName;
                    Session["RoleUser"] = user.RoleUser;

                    // Điều hướng theo role
                    if (user.RoleUser == "admin")
                        return RedirectToAction("Dashboard", "Admin");
                    else
                        return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng.");
            }
            return View(model);
        }

        // 3. GET: Register
        // GET: /Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = db.AdminUsers.FirstOrDefault(u => u.UserName == model.Username);
                var existingEmail = db.AdminUsers.FirstOrDefault(u => u.Email == model.Email);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }
                if (existingEmail != null)
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng.");
                    return View(model);
                }
                AdminUser newUser = new AdminUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    PasswordUser = model.Password,
                    RoleUser = "user" // Đừng quên gán role!
                };

                db.AdminUsers.Add(newUser);
                db.SaveChanges();

                // Thêm vào bảng Customer sau khi tạo AdminUser
                Customer newCustomer = new Customer
                {
                    NameCus = model.NameCus,
                    EmailCus = model.Email,
                    UserName = model.Username,
                    Password = model.Password,
                    PhoneCus = model.PhoneCus
                };
                db.Customers.Add(newCustomer);
                db.SaveChanges();

                TempData["Success"] = "Đăng ký thành công!";
                return RedirectToAction("Login");
            }

            return View(model);
        }
        // GET: /Account/ForgotPassword
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.AdminUsers.FirstOrDefault(u => u.Email == model.Email);
                if (user != null)
                {
                    user.PasswordUser = model.NewPassword; // Nên mã hóa mật khẩu ở đây
                    db.SaveChanges();
                    ViewBag.Message = "Mật khẩu đã được cập nhật.";
                }
                else
                {
                    ModelState.AddModelError("", "Không tìm thấy tài khoản với email này.");
                }
            }

            return View(model);
        }
        // 5. Logout
        public ActionResult Logout()
        {
            Session.Clear(); // Xoá session đăng nhập
            return RedirectToAction("Login");
        }
        public new ActionResult Profile()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login");

            int userId = (int)Session["UserID"];
            string role = Session["RoleUser"]?.ToString();

            if (role == "admin")
            {
                var user = db.AdminUsers.FirstOrDefault(u => u.ID == userId);
                if (user == null)
                    return RedirectToAction("Login");
                return View(user); // trả về view mặc định
            }
            else
            {
                var customer = db.Customers.FirstOrDefault(c => c.IDCus == userId);
                if (customer == null)
                    return RedirectToAction("Login");
                return View(customer); // trả về view mặc định
            }
        }
        public ActionResult EditProfile()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login");

            int userId = (int)Session["UserID"];
            var user = db.AdminUsers.Find(userId);

            if (user == null)
                return HttpNotFound();

            var model = new EditProfileViewModel
            {
                ID = user.ID,
                UserName = user.UserName,
                Email = user.Email
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.AdminUsers.Find(model.ID);

                if (user == null)
                    return HttpNotFound();

                user.UserName = model.UserName;
                user.Email = model.Email;

                if (!string.IsNullOrWhiteSpace(model.NewPassword))
                    user.PasswordUser = model.NewPassword;

                db.SaveChanges();
                TempData["Success"] = "Cập nhật thành công!";
                return RedirectToAction("Profile");
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateAvatar(HttpPostedFileBase file)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login");

            int userId = (int)Session["UserID"];
            string role = Session["RoleUser"]?.ToString();

            if (file != null && file.ContentLength > 0)
            {
                string folderPath = Server.MapPath("~/Content/avatars/");
                if (!System.IO.Directory.Exists(folderPath))
                    System.IO.Directory.CreateDirectory(folderPath);

                string fileName = System.IO.Path.GetFileName(file.FileName);
                string filePath = System.IO.Path.Combine(folderPath, fileName);
                file.SaveAs(filePath);

                string relativePath = "/Content/avatars/" + fileName;

                if (role == "admin")
                {
                    var user = db.AdminUsers.FirstOrDefault(u => u.ID == userId);
                    if (user != null)
                    {
                        user.Avatar = relativePath;
                        db.SaveChanges();
                    }
                }
                else
                {
                    var customer = db.Customers.FirstOrDefault(c => c.IDCus == userId);
                    if (customer != null)
                    {
                        customer.Avatar = relativePath;
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Profile");
        }
    }
}
