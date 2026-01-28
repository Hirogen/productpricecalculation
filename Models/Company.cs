using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProductPriceCalculator.Models
{
    /// <summary>
    /// Company/Manufacturer model
    /// </summary>
    public class Company : INotifyPropertyChanged
    {
        private long _id;
        private string _name;
        private string _website;
        private string _contactInfo;

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

        public string Website
        {
            get => _website;
            set
            {
                _website = value;
                OnPropertyChanged();
            }
        }

        public string ContactInfo
        {
            get => _contactInfo;
            set
            {
                _contactInfo = value;
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
