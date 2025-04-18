using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    class Import
    {
        public int ImportId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime ImportDate { get; set; }
        public int? SupplierId { get; set; }
        public int? WarehouseId { get; set; }

        // Navigation properties
        public Supplier Supplier { get; set; }
        public Warehouse Warehouse { get; set; }
        public List<ImportDetail> ImportDetails { get; set; }
        public List<ImportUser> ImportUsers { get; set; }
    }
}
