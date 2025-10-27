using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DBSportStore.Models;
using DBStoreSport.Controllers;
using DBStoreSport.Models;

namespace DBSportStore.Controllers
{
    public class ShoppingCartController : Controller
    {
        DBSportStoreEntities db = new DBSportStoreEntities();
        // GET: ShoppingCart, chuẩn bị dữ liệu cho View
        public ActionResult ShowCart()
        {
            if (Session["Cart"] == null)
            {
                ViewBag.IsLoggedIn = Session["UserID"] != null;
                return View("ShowCart");
            }
            Cart _cart = Session["Cart"] as Cart;
            ViewBag.IsLoggedIn = Session["UserID"] != null;
            return View(_cart);
        }
        // Tạo mới giỏ hàng, nguồn được lấy từ Session
        public Cart GetCart()
        {
            Cart cart = Session["Cart"] as Cart;
            if (cart == null || Session["Cart"] == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;
        }
        [HttpGet]
        public JsonResult GetCartJson()
        {
            var cart = GetCart(); // Gọi lại phương thức bạn đang có
            var items = cart.Items.Select(i => new
            {
                ProductId = i._product.ProductID,
                Name = i._product.NamePro,
                Quantity = i._quantity,
                Price = i._product.Price,
                Total = i._product.Price * i._quantity
            }).ToList();

            return Json(new
            {
                TotalPrice = items.Sum(x => x.Total),
                Items = items
            }, JsonRequestBehavior.AllowGet);
        }

        // Thêm sản phẩm vào giỏ hàng
        // Thêm sản phẩm vào giỏ hàng
        public ActionResult AddToCart(int id)
        {
            var _pro = db.Products.SingleOrDefault(s => s.ProductID == id);
            if (_pro == null)
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại!";
                return RedirectToAction("ShowCart", "ShoppingCart");
            }

            // ✅ Kiểm tra hàng tồn
            if (_pro.Quantity <= 0)
            {
                TempData["ErrorMessage"] = "Sản phẩm đã hết hàng!";
                return Redirect(Request.UrlReferrer?.ToString() ?? Url.Action("ShowCart", "ShoppingCart"));
            }

            GetCart().Add_Product_Cart(_pro);
            TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ hàng!";
            return RedirectToAction("ShowCart", "ShoppingCart");
        }

        // Cập nhật số lượng và tính lại tổng tiền
        public ActionResult Update_Cart_Quantity(FormCollection form)
        {
            Cart cart = Session["Cart"] as Cart;
            int id_pro = int.Parse(Request.Form["idPro"]);
            int _quantity = int.Parse(Request.Form["carQuantity"]);
            cart.Update_quantity(id_pro, _quantity);

            return RedirectToAction("ShowCart", "ShoppingCart");
        }
        // Xóa dòng sản phẩm trong giỏ hàng
        public ActionResult RemoveCart(int id)
        {
            Cart cart = Session["Cart"] as Cart;
            cart.Remove_CartItem(id);

            return RedirectToAction("ShowCart", "ShoppingCart");
        }
        // Tính tổng tiền đơn hàng
        public PartialViewResult BagCart()
        {
            decimal total_money_item = 0;
            Cart cart = Session["Cart"] as Cart;
            if (cart != null)
                total_money_item = cart.Total_money();
            ViewBag.TotalCart = total_money_item;
            return PartialView("BagCart");
        }
        public ActionResult CheckOut(FormCollection form)
        {
            try
            {
                Cart cart = Session["Cart"] as Cart;
                
                // Kiểm tra đăng nhập
                int? customerId = Session["UserID"] as int?;
                if (customerId == null)
                {
                    TempData["ErrorMessage"] = "Vui lòng đăng nhập để đặt hàng!";
                    return RedirectToAction("Login", "Account");
                }

                OrderPro _order = new OrderPro();
                _order.DateOrder = DateTime.Now;
                _order.AddressDeliverry = form["AddressDeliverry"];
                _order.IDCus = customerId.Value; // Tự động lấy từ session
                db.OrderProes.Add(_order);
                foreach (var item in cart.Items)
                {
                    // lưu dòng sản phẩm vào chi tiết hóa đơn
                    OrderDetail _order_detail = new OrderDetail();
                    _order_detail.IDOrder = _order.ID;
                    _order_detail.IDProduct = item._product.ProductID;
                    _order_detail.UnitPrice = (double)item._product.Price;
                    _order_detail.Quantity = item._quantity;
                    db.OrderDetails.Add(_order_detail);
                }
                db.SaveChanges();
                cart.ClearCart();
                return RedirectToAction("CheckOut_Success", "ShoppingCart");
            }
            catch
            {
                return Content("Có sai sót! Xin kiểm tra lại thông tin"); ;
            }
        }
        public ActionResult CheckOut_Success()
        {
            return View();
        }
        public ActionResult ApplyVoucher(string voucherCode)
        {
            using (var db = new DBSportStoreEntities())
            {
                var voucher = db.Vouchers.FirstOrDefault(v => v.Code == voucherCode && v.ExpiryDate >= DateTime.Now && v.IsUsed == false);

                if (voucher != null)
                {
                    Session["Voucher"] = voucher;
                    ViewBag.VoucherMessage = "Áp dụng mã thành công!";
                }
                else
                {
                    Session["Voucher"] = null;
                    ViewBag.VoucherMessage = "Mã không hợp lệ hoặc đã hết hạn!";
                }
            }

            return RedirectToAction("ShowCart");
        }
        // Thêm vào file: Controllers/ShoppingCartController.cs

        [HttpPost]
        public ActionResult ProcessPayment(FormCollection form)
        {
            try
            {
                Cart cart = Session["Cart"] as Cart;
                if (cart == null || !cart.Items.Any())
                {
                    TempData["ErrorMessage"] = "Giỏ hàng trống!";
                    return RedirectToAction("ShowCart");
                }

                // Kiểm tra đăng nhập
                int? customerId = Session["UserID"] as int?;
                if (customerId == null)
                {
                    TempData["ErrorMessage"] = "Vui lòng đăng nhập để đặt hàng!";
                    return RedirectToAction("Login", "Account");
                }

                string paymentMethod = form["paymentMethod"];
                string address = form["AddressDeliverry"];

                // Validation
                if (string.IsNullOrEmpty(address))
                {
                    TempData["ErrorMessage"] = "Vui lòng nhập địa chỉ giao hàng!";
                    return RedirectToAction("ShowCart");
                }

                // Tạo đơn hàng
                OrderPro order = new OrderPro
                {
                    DateOrder = DateTime.Now,
                    AddressDeliverry = address,
                    IDCus = customerId.Value, // Tự động lấy từ session
                    PaymentMethod = paymentMethod,
                    PaymentStatus = paymentMethod == "cod" ? "Pending" : "Pending",
                    Status = "Chờ xác nhận", // 👈 Thêm giá trị mặc định để admin duyệt
                };

                db.OrderProes.Add(order);
                db.SaveChanges();

                // Thêm chi tiết đơn hàng
                foreach (var item in cart.Items)
                {
                    OrderDetail orderDetail = new OrderDetail
                    {
                        IDOrder = order.ID,
                        IDProduct = item._product.ProductID,
                        UnitPrice = (double)item._product.Price,
                        Quantity = item._quantity
                    };
                    db.OrderDetails.Add(orderDetail);

                    // ✅ Cập nhật tồn kho sản phẩm
                    var product = db.Products.FirstOrDefault(p => p.ProductID == item._product.ProductID);
                    if (product != null)
                    {
                        if (product.Quantity >= item._quantity)
                        {
                            product.Quantity -= item._quantity;
                        }
                        else
                        {
                            // Nếu số lượng tồn không đủ, set về 0 và có thể ghi log hoặc cảnh báo
                            product.Quantity = 0;
                        }
                    }
                    var log = new ProductStockHistory
                    {
                        ProductID = item._product.ProductID,
                        QuantityChange = -item._quantity,
                        ActionType = "Bán hàng",
                        Note = $"Khách hàng đặt đơn #{order.ID}",
                        AdminUser = "Hệ thống"
                    };
                    db.ProductStockHistories.Add(log);

                }
                db.SaveChanges();



                if (paymentMethod == "vnpay")
                {
                    // Chuyển đến VNPay
                    var paymentController = new PaymentController();
                    var result = paymentController.CreatePaymentUrl(order.ID);

                    if (result is JsonResult jsonResult)
                    {
                        var data = jsonResult.Data;
                        var response = (dynamic)data;
                        if (response.success)
                        {
                            return Redirect(response.paymentUrl);
                        }
                        else
                        {
                            TempData["ErrorMessage"] = response.message;
                            return RedirectToAction("ShowCart");
                        }
                    }
                }
                else if (paymentMethod == "cod")
                {
                    // COD - chuyển đến trang thành công
                    cart.ClearCart();
                    Session["Voucher"] = null;
                    return RedirectToAction("CheckOut_Success", "ShoppingCart");
                }

                return RedirectToAction("ShowCart");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("ShowCart");
            }
        }
    }
}
