using System;

namespace LaptopShopProject.Models
{
    internal class Supplier
    {
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string Gmail { get; set; }
        public string Phone { get; set; }
        public DateTime? ImportDate { get; set; } // Optional: For display purposes in SupplierForm
    }
}