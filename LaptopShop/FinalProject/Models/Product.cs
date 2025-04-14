using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal WholesalePrice { get; set; }
        public decimal SellPrice { get; set; }
        public string EAN13 { get; set; }
        public string Brand { get; set; }
        public bool Status { get; set; }
        public string Img { get; set; }
        public decimal ImportPrice { get; set; }
        public int? WarehouseId { get; set; }

        // Navigation properties
        public Warehouse Warehouse { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
        public List<ProductSpecs> ProductSpecs { get; set; }
        public List<ImportDetail> ImportDetails { get; set; }
        public List<ExportDetail> ExportDetails { get; set; }
    }
}
