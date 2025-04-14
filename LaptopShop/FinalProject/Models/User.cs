using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // Theo biểu đồ
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Img { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? RoleId { get; set; } // Liên kết với bảng Role (phân quyền)

        // Navigation properties
        public Role Role { get; set; }
        public List<ImportUser> ImportUsers { get; set; }
        public List<ExportUser> ExportUsers { get; set; }
        public List<Notification> Notifications { get; set; }
    }
}
