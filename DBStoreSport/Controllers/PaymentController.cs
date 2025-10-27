// Cập nhật file: Controllers/PaymentController.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DBStoreSport.Models;
using DBStoreSport.Services;

namespace DBStoreSport.Controllers
{
    public class PaymentController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();
        private readonly VNPayService _vnPayService = new VNPayService();

        // GET: Payment
        public ActionResult Index()
        {
            return View();
        }

        // Tạo URL thanh toán VNPay
        [HttpPost]
        public ActionResult CreatePaymentUrl(int orderId)
        {
            try
            {
                var order = db.OrderProes.Find(orderId);
                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                // Tính tổng tiền
                var totalAmount = order.OrderDetails.Sum(od => (decimal)(od.UnitPrice ?? 0) * (od.Quantity ?? 0));

                // Cập nhật thông tin đơn hàng
                order.PaymentMethod = "VNPay";
                order.PaymentStatus = "Pending";
                db.SaveChanges();

                var request = new VNPayRequest
                {
                    vnp_TmnCode = System.Configuration.ConfigurationManager.AppSettings["VNPay_TmnCode"],
                    vnp_Amount = totalAmount,
                    vnp_TxnRef = orderId.ToString(),
                    vnp_OrderInfo = $"Thanh toan don hang #{orderId}",
                    vnp_IpAddr = GetClientIPAddress(),
                    vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss")
                };

                var paymentUrl = _vnPayService.CreatePaymentUrl(request);

                return Json(new { success = true, paymentUrl = paymentUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Xử lý callback từ VNPay
        [HttpPost]
        public ActionResult VNPayCallback(FormCollection collection)
        {
            try
            {
                var callbackData = new PaymentCallback
                {
                    vnp_Amount = collection["vnp_Amount"],
                    vnp_BankCode = collection["vnp_BankCode"],
                    vnp_BankTranNo = collection["vnp_BankTranNo"],
                    vnp_CardType = collection["vnp_CardType"],
                    vnp_OrderInfo = collection["vnp_OrderInfo"],
                    vnp_PayDate = collection["vnp_PayDate"],
                    vnp_ResponseCode = collection["vnp_ResponseCode"],
                    vnp_TmnCode = collection["vnp_TmnCode"],
                    vnp_TransactionNo = collection["vnp_TransactionNo"],
                    vnp_TransactionStatus = collection["vnp_TransactionStatus"],
                    vnp_TxnRef = collection["vnp_TxnRef"],
                    vnp_SecureHash = collection["vnp_SecureHash"]
                };

                // Verify signature
                bool isValid = _vnPayService.VerifyPaymentCallback(callbackData);

                if (isValid && callbackData.vnp_ResponseCode == "00")
                {
                    // Thanh toán thành công
                    int orderId = int.Parse(callbackData.vnp_TxnRef);
                    var order = db.OrderProes.Find(orderId);

                    if (order != null)
                    {
                        // Cập nhật trạng thái thanh toán
                        order.PaymentStatus = "Paid";
                        order.TransactionId = callbackData.vnp_TransactionNo;
                        order.PaymentTime = DateTime.Now;
                        order.PaymentResponseCode = callbackData.vnp_ResponseCode;
                        order.PaymentMessage = _vnPayService.GetPaymentStatusMessage(callbackData.vnp_ResponseCode);

                        db.SaveChanges();

                        // Xóa giỏ hàng và voucher
                        Session["Cart"] = null;
                        Session["Voucher"] = null;

                        return RedirectToAction("PaymentSuccess", new { orderId = orderId });
                    }
                }

                // Thanh toán thất bại
                return RedirectToAction("PaymentFailed", new { message = "Giao dịch không thành công" });
            }
            catch (Exception ex)
            {
                return RedirectToAction("PaymentFailed", new { message = $"Lỗi xử lý thanh toán: {ex.Message}" });
            }
        }

        // Trang thành công
        public ActionResult PaymentSuccess(int orderId)
        {
            var order = db.OrderProes.Find(orderId);
            if (order == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.OrderId = orderId;
            ViewBag.TotalAmount = order.OrderDetails.Sum(od => od.UnitPrice * od.Quantity);
            ViewBag.TransactionId = order.TransactionId;

            return View();
        }

        // Trang thất bại
        public ActionResult PaymentFailed(string message = "")
        {
            ViewBag.ErrorMessage = message;
            return View();
        }

        // Kiểm tra trạng thái thanh toán
        public JsonResult CheckPaymentStatus(int orderId)
        {
            try
            {
                var order = db.OrderProes.Find(orderId);
                if (order != null)
                {
                    return Json(new
                    {
                        success = true,
                        status = order.PaymentStatus ?? "Pending",
                        message = order.PaymentMessage ?? "Đơn hàng đang chờ xử lý"
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new
                {
                    success = false,
                    message = "Không tìm thấy đơn hàng"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // Hủy thanh toán
        public ActionResult CancelPayment(int orderId)
        {
            try
            {
                var order = db.OrderProes.Find(orderId);
                if (order != null)
                {
                    order.PaymentStatus = "Cancelled";
                    order.PaymentTime = DateTime.Now;
                    order.PaymentMessage = "Khách hàng hủy thanh toán";

                    db.SaveChanges();

                    ViewBag.Message = "Đã hủy thanh toán thành công";
                    return View("PaymentCancelled");
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi hủy thanh toán: {ex.Message}";
                return View("PaymentFailed");
            }
        }

        private string GetClientIPAddress()
        {
            try
            {
                // Kiểm tra Request có null không
                if (Request == null)
                {
                    return "127.0.0.1"; // IP mặc định cho localhost
                }

                string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = Request.ServerVariables["REMOTE_ADDR"];
                }

                // Nếu vẫn null, trả về IP mặc định
                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = "127.0.0.1";
                }

                return ipAddress;
            }
            catch
            {
                return "127.0.0.1"; // IP mặc định nếu có lỗi
            }
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