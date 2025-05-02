using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopShopProject.Models
{
    public class PermissionLog
    {
        public int LogId { get; set; }
        public string Action { get; set; }
        public DateTime ActionDate { get; set; }
        public string PerformedBy { get; set; }
        public string? TargetRole { get; set; } // Added to match database schema
    }
}
