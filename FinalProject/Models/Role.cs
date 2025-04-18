using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation properties
        public List<User> Users { get; set; }
        public List<RolePermission> RolePermissions { get; set; }
    }
}
