using System;

namespace LaptopShopProject.Models
{
    internal class Export
    {
        public int ExportId { get; set; }
        public DateTime ExportDate { get; set; }
        public string CustomerName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
    }
}