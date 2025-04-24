using System;

namespace LaptopShopProject.Models
{
    internal class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Gmail { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; } // Optional: For display purposes in CustomerForm (not in DB)
        public DateTime? ExportDate { get; set; } // Optional: For display purposes in CustomerForm
    }
}