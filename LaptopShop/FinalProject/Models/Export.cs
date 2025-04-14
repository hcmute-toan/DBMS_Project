using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    class Export
    {
        public int ExportId { get; set; }
        public DateTime ExportDate { get; set; }
        public string Purpose { get; set; }
        public int? CustomerId { get; set; }
        public int? WarehouseId { get; set; }

        // Navigation properties
        public Customer Customer { get; set; }
        public Warehouse Warehouse { get; set; }
        public List<ExportDetail> ExportDetails { get; set; }
        public List<ExportUser> ExportUsers { get; set; }
    }
}
