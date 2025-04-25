using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopShopProject.Utilities
{
    public static class Helper
    {
        public static string FormatCurrency(decimal amount)
        {
            return amount.ToString("C", CultureInfo.CreateSpecificCulture("vi-VN"));
        }

        public static string FormatDate(DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

        public static bool IsAdmin(string role)
        {
            return role.Equals("admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
