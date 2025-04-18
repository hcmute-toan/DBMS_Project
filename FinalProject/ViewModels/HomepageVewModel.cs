using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using FinalProject.Views;

namespace FinalProject.ViewModels
{
    public class HomepageViewModel : INotifyPropertyChanged
    {
        private UserControl _currentView;
        public UserControl CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        public ICommand ShowSupplierViewCommand { get; }

        public HomepageViewModel() => ShowSupplierViewCommand = new RelayCommand(_ => ShowSupplierView());

        private void ShowSupplierView()
        {
            CurrentView = new SupplierView();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
