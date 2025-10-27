using System;
using System.ComponentModel.DataAnnotations;

namespace DBStoreSport.Models
{
    public partial class OrderPro
    {
        // Thêm các thuộc tính cho thanh toán online
        public string TransactionId { get; set; }
        public string PaymentStatus { get; set; } // Pending, Paid, Failed, Cancelled
        public string PaymentGateway { get; set; } // VNPay, MoMo, etc.
        public DateTime? PaymentTime { get; set; }
        public string PaymentResponseCode { get; set; }
        public string PaymentMessage { get; set; }
        public decimal? TotalAmount { get; set; }
        
        // Thuộc tính tính toán
        public bool IsPaid
        {
            get { return PaymentStatus == "Paid"; }
        }
        
        public bool IsPending
        {
            get { return PaymentStatus == "Pending"; }
        }
        
        public bool IsFailed
        {
            get { return PaymentStatus == "Failed"; }
        }
    }
}
