using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    class ImportUser
    {
        public int ImportId { get; set; }
        public int UserId { get; set; }

        // Navigation properties
        public Import Import { get; set; }
        public User User { get; set; }
    }
}
