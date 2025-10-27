using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DBStoreSport.Models
{
    public class PaymentRequest
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string OrderInfo { get; set; }
        public string CustomerId { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string ReturnUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public string PaymentMethod { get; set; }
    }

    public class VNPayRequest
    {
        public string vnp_Version { get; set; } = "2.1.0";
        public string vnp_Command { get; set; } = "pay";
        public string vnp_TmnCode { get; set; }
        public decimal vnp_Amount { get; set; }
        public string vnp_CurrCode { get; set; } = "VND";
        public string vnp_TxnRef { get; set; }
        public string vnp_OrderInfo { get; set; }
        public string vnp_OrderType { get; set; } = "other";
        public string vnp_Locale { get; set; } = "vn";
        public string vnp_ReturnUrl { get; set; }
        public string vnp_IpAddr { get; set; }
        public string vnp_CreateDate { get; set; }
        public string vnp_SecureHash { get; set; }
    }

    public class PaymentCallback
    {
        public string vnp_Amount { get; set; }
        public string vnp_BankCode { get; set; }
        public string vnp_BankTranNo { get; set; }
        public string vnp_CardType { get; set; }
        public string vnp_OrderInfo { get; set; }
        public string vnp_PayDate { get; set; }
        public string vnp_ResponseCode { get; set; }
        public string vnp_TmnCode { get; set; }
        public string vnp_TransactionNo { get; set; }
        public string vnp_TransactionStatus { get; set; }
        public string vnp_TxnRef { get; set; }
        public string vnp_SecureHash { get; set; }
    }

    public class PaymentResponse
    {
        public bool Success { get; set; }
        public string PaymentUrl { get; set; }
        public string Message { get; set; }
        public string TransactionId { get; set; }
        public string ErrorCode { get; set; }
    }
}
