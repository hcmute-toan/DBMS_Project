using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    class Notification
    {
        public int NotificationId { get; set; }
        public string Message { get; set; }
        public DateTime NotificationDate { get; set; }
        public bool IsRead { get; set; }
        public int? UserId { get; set; }

        // Navigation properties
        public User User { get; set; }
    }
}
