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
        public int UserId { get; set; }
        public string Action { get; set; }
        public string OldRole { get; set; }
        public string NewRole { get; set; }
        public DateTime ActionDate { get; set; }
        public int PerformedBy { get; set; }
    }
}
