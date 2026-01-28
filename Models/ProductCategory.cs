using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProductPriceCalculator.Models
{
    /// <summary>
    /// Product Category model
    /// </summary>
    public class ProductCategory : INotifyPropertyChanged
    {
        private long _id;
        private string _name;
        private string _description;

        public long Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
