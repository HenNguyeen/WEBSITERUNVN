using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using System.IO;

namespace DBStoreSport.Controllers
{
    public class AdminController : Controller
    {
        // Trang quản trị (chỉ admin vào được)
        public ActionResult Dashboard()
        {
            // Kiểm tra quyền
            if (Session["RoleUser"] == null || Session["RoleUser"].ToString() != "admin")
            {
                return RedirectToAction("Login", "Account");
            }

            using (var db = new DBStoreSport.Models.DBSportStoreEntities())
            {
                ViewBag.UserName = Session["UserName"];
                ViewBag.TotalCustomers = db.Customers.Count();
                ViewBag.TotalProducts = db.Products.Count();
                ViewBag.TotalOrders = db.OrderProes.Count();
                var orderDetails = db.OrderDetails.ToList();
                ViewBag.TotalRevenue = orderDetails.Sum(x => (x.Quantity ?? 0) * Convert.ToDecimal(x.UnitPrice ?? 0));

                // Doanh thu từng tháng trong năm hiện tại
                var now = DateTime.Now;
                var orderDetailsWithOrder = db.OrderDetails.Include("OrderPro").ToList();
                var monthlyRevenue = new decimal[12];
                for (int i = 1; i <= 12; i++)
                {
                    monthlyRevenue[i - 1] = orderDetailsWithOrder
                        .Where(x => x.OrderPro != null && x.OrderPro.DateOrder.HasValue && x.OrderPro.DateOrder.Value.Month == i && x.OrderPro.DateOrder.Value.Year == now.Year)
                        .Sum(x => (x.Quantity ?? 0) * Convert.ToDecimal(x.UnitPrice ?? 0));
                }
                ViewBag.MonthlyRevenue = monthlyRevenue;

                // Lấy hoạt động gần đây
                ViewBag.RecentOrders = db.OrderProes.OrderByDescending(o => o.DateOrder).Take(3).ToList();
                ViewBag.RecentCustomers = db.Customers.OrderByDescending(c => c.IDCus).Take(1).ToList();
                ViewBag.RecentProducts = db.Products.OrderByDescending(p => p.ProductID).Take(1).ToList();
            }
            return View();
        }

        [HttpGet]
        public ActionResult ExportMonthlyRevenueToExcel()
        {
            var now = DateTime.Now;
            var orderDetailsWithOrder = new DBStoreSport.Models.DBSportStoreEntities().OrderDetails.Include("OrderPro").ToList();
            var monthlyRevenue = new decimal[12];
            for (int i = 1; i <= 12; i++)
            {
                monthlyRevenue[i - 1] = orderDetailsWithOrder
                    .Where(x => x.OrderPro != null && x.OrderPro.DateOrder.HasValue && x.OrderPro.DateOrder.Value.Month == i && x.OrderPro.DateOrder.Value.Year == now.Year)
                    .Sum(x => (x.Quantity ?? 0) * Convert.ToDecimal(x.UnitPrice ?? 0));
            }

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("DoanhThuTheoThang");
                ws.Cells[1, 1].Value = "Tháng";
                ws.Cells[1, 2].Value = "Doanh thu (VNĐ)";
                for (int i = 0; i < 12; i++)
                {
                    ws.Cells[i + 2, 1].Value = $"T{i + 1}";
                    ws.Cells[i + 2, 2].Value = monthlyRevenue[i];
                }
                ws.Cells[1, 1, 1, 2].Style.Font.Bold = true;
                ws.Cells.AutoFitColumns();
                var stream = new MemoryStream(package.GetAsByteArray());
                string fileName = $"DoanhThuTheoThang_{now:yyyy}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}