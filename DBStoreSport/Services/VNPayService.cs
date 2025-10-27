// Tạo file: Services/VNPayService.cs
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using DBStoreSport.Models;

namespace DBStoreSport.Services
{
    public class VNPayService
    {
        private readonly string _tmnCode;
        private readonly string _hashSecret;
        private readonly string _paymentUrl;
        private readonly string _returnUrl;

        public VNPayService()
        {
            _tmnCode = System.Configuration.ConfigurationManager.AppSettings["VNPay_TmnCode"];
            _hashSecret = System.Configuration.ConfigurationManager.AppSettings["VNPay_HashSecret"];
            _paymentUrl = System.Configuration.ConfigurationManager.AppSettings["VNPay_Url"];
            _returnUrl = System.Configuration.ConfigurationManager.AppSettings["VNPay_ReturnUrl"];
        }

        public string CreatePaymentUrl(VNPayRequest request)
        {
            // Tạo query string parameters
            var vnpayData = new Dictionary<string, string>
            {
                {"vnp_Version", request.vnp_Version},
                {"vnp_Command", request.vnp_Command},
                {"vnp_TmnCode", _tmnCode},
                {"vnp_Amount", (request.vnp_Amount * 100).ToString()}, // VNPay yêu cầu amount * 100
                {"vnp_CurrCode", request.vnp_CurrCode},
                {"vnp_TxnRef", request.vnp_TxnRef},
                {"vnp_OrderInfo", request.vnp_OrderInfo},
                {"vnp_OrderType", request.vnp_OrderType},
                {"vnp_Locale", request.vnp_Locale},
                {"vnp_ReturnUrl", _returnUrl},
                {"vnp_IpAddr", request.vnp_IpAddr},
                {"vnp_CreateDate", request.vnp_CreateDate}
            };

            // Sắp xếp và tạo query string
            var queryString = CreateQueryString(vnpayData);

            // Tạo secure hash
            var secureHash = CreateSecureHash(queryString);
            queryString += $"&vnp_SecureHash={secureHash}";

            return $"{_paymentUrl}?{queryString}";
        }

        public bool VerifyPaymentCallback(PaymentCallback callback)
        {
            var vnpayData = new Dictionary<string, string>
            {
                {"vnp_Amount", callback.vnp_Amount},
                {"vnp_BankCode", callback.vnp_BankCode},
                {"vnp_BankTranNo", callback.vnp_BankTranNo},
                {"vnp_CardType", callback.vnp_CardType},
                {"vnp_OrderInfo", callback.vnp_OrderInfo},
                {"vnp_PayDate", callback.vnp_PayDate},
                {"vnp_ResponseCode", callback.vnp_ResponseCode},
                {"vnp_TmnCode", callback.vnp_TmnCode},
                {"vnp_TransactionNo", callback.vnp_TransactionNo},
                {"vnp_TransactionStatus", callback.vnp_TransactionStatus},
                {"vnp_TxnRef", callback.vnp_TxnRef}
            };

            var queryString = CreateQueryString(vnpayData);
            var secureHash = CreateSecureHash(queryString);

            return secureHash.Equals(callback.vnp_SecureHash, StringComparison.OrdinalIgnoreCase);
        }

        private string CreateQueryString(Dictionary<string, string> data)
        {
            var sortedData = new SortedDictionary<string, string>(data);
            var queryString = new StringBuilder();

            foreach (var item in sortedData)
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    queryString.Append($"{item.Key}={HttpUtility.UrlEncode(item.Value)}&");
                }
            }

            return queryString.ToString().TrimEnd('&');
        }

        private string CreateSecureHash(string queryString)
        {
            var data = Encoding.UTF8.GetBytes(queryString + _hashSecret);
            var hash = SHA256.Create().ComputeHash(data);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        public string GetPaymentStatusMessage(string responseCode)
        {
            switch (responseCode)
            {
                case "00": return "Giao dịch thành công";
                case "07": return "Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, giao dịch bất thường).";
                case "09": return "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking của ngân hàng.";
                case "10": return "Xác thực thông tin thẻ/tài khoản không đúng quá 3 lần";
                case "11": return "Đã hết hạn chờ thanh toán. Xin vui lòng thực hiện lại giao dịch.";
                case "12": return "Giao dịch bị huỷ";
                case "24": return "Giao dịch không thành công do: Khách hàng hủy giao dịch";
                case "51": return "Giao dịch không thành công do: Tài khoản của quý khách không đủ số dư để thực hiện giao dịch.";
                case "65": return "Giao dịch không thành công do: Tài khoản của Quý khách đã vượt quá hạn mức giao dịch trong ngày.";
                case "75": return "Ngân hàng thanh toán đang bảo trì.";
                case "79": return "Nhập sai mật khẩu thanh toán quá số lần quy định. Xin vui lòng thực hiện lại giao dịch";
                default: return "Giao dịch không thành công";
            }
        }

    }
}