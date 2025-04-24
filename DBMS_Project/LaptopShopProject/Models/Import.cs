using System;

namespace LaptopShopProject.Models
{
    internal class Import
    {
        public int ImportId { get; set; }
        public DateTime ImportDate { get; set; }
        public int SupplierId { get; set; } // Added for ComboBox selection
        public string SupplierName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
    }
}