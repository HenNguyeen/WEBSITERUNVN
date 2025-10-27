using System;

namespace DBStoreSport.Models
{
    public class OrderHistory
    {
        public int OrderID { get; set; }
        public DateTime? OrderDate { get; set; }
        public string ProductName { get; set; }
        public int? Quantity { get; set; }
        public double? UnitPrice { get; set; }
        public double? Total => Quantity * UnitPrice;
        public string AddressDelivery { get; set; }
}
}