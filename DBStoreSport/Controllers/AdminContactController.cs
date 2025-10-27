using System.Linq;
using System.Web.Mvc;
using DBStoreSport.Models;

namespace DBStoreSport.Controllers
{
    public class AdminContactController : Controller
    {
        DBSportStoreEntities db = new DBSportStoreEntities();

        // Danh sách liên hệ
        public ActionResult Index()
        {
            var list = db.ContactMessages.OrderByDescending(m => m.SentDate).ToList();
            return View(list);
        }

        // Xem chi tiết liên hệ
        public ActionResult Details(int id)
        {
            var contact = db.ContactMessages.Find(id);
            if (contact == null)
                return HttpNotFound();

            // Dùng cách an toàn
            if (!contact.IsRead.GetValueOrDefault())
            {
                contact.IsRead = true;
                db.SaveChanges();
            }

            return View(contact);
        }



        // Xóa liên hệ
        public ActionResult Delete(int id)
        {
            var contact = db.ContactMessages.Find(id);
            if (contact == null)
                return HttpNotFound();

            db.ContactMessages.Remove(contact);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
