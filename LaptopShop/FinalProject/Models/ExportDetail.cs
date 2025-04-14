using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    class ExportDetail
    {
        public int ExportDetailId { get; set; }
        public int ExportId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // Navigation properties
        public Export Export { get; set; }
        public Product Product { get; set; }
    }
}
