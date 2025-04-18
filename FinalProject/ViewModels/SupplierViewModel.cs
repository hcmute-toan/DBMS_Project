using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FinalProject.ViewModels
{
    public class SupplierViewModel
    {
        public ObservableCollection<SupplierViewModel> SupplierList { get; set; }
        public SupplierViewModel SelectedSupplier { get; set; }

        public string SupplierName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime EntryDate { get; set; }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand EditCommand { get; }
    }
}
