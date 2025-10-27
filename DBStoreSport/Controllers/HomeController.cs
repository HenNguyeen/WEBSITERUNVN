using DBStoreSport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DBStoreSport.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
[HttpPost]
    public ActionResult Contact(string Name, string Email, string Message)
    {
        using (var db = new DBSportStoreEntities())
        {
            var contact = new ContactMessage
            {
                Name = Name,
                Email = Email,
                Message = Message,
                SentDate = DateTime.Now
            };

            db.ContactMessages.Add(contact);
            db.SaveChanges();
        }

        TempData["msg"] = "Cảm ơn bạn đã liên hệ. Chúng tôi sẽ phản hồi sớm nhất!";
        return RedirectToAction("Contact");
    }


}
}