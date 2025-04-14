using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    class ExportUser
    {
        public int ExportId { get; set; }
        public int UserId { get; set; }

        // Navigation properties
        public Export Export { get; set; }
        public User User { get; set; }
    }
}
