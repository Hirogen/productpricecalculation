using System.Collections.ObjectModel;
using System.Windows.Input;
using ProductPriceCalculator.Infrastructure;
using ProductPriceCalculator.Models;

namespace ProductPriceCalculator.ViewModels
{
    public class CompaniesViewModel : ViewModelBase
    {
        private readonly DatabaseManager _databaseManager;
        private CompanyDb _selectedCompany;
        private string _newCompanyName;
        private string _newCompanyWebsite;
        private string _newCompanyContactInfo;

        public CompaniesViewModel(DatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
            Companies = new ObservableCollection<CompanyDb>(_databaseManager.GetCompanies());
            AddCompanyCommand = new RelayCommand(AddCompany, CanAddCompany);
            DeleteCompanyCommand = new RelayCommand(DeleteCompany, () => SelectedCompany != null);
            
            Localization.OnLanguageChanged += OnLanguageChanged;
        }

        public ObservableCollection<CompanyDb> Companies { get; }

        public CompanyDb SelectedCompany
        {
            get => _selectedCompany;
            set { SetProperty(ref _selectedCompany, value); }
        }

        public string NewCompanyName
        {
            get => _newCompanyName;
            set { SetProperty(ref _newCompanyName, value); }
        }

        public string NewCompanyWebsite
        {
            get => _newCompanyWebsite;
            set { SetProperty(ref _newCompanyWebsite, value); }
        }

        public string NewCompanyContactInfo
        {
            get => _newCompanyContactInfo;
            set { SetProperty(ref _newCompanyContactInfo, value); }
        }

        public ICommand AddCompanyCommand { get; }
        public ICommand DeleteCompanyCommand { get; }

        // Localized properties
        public string HeaderCompanies => Localization.Get("HeaderCompanies");
        public string InfoCompanies => Localization.Get("InfoCompanies");
        public string HeaderAddNewCompany => Localization.Get("HeaderAddNewCompany");
        public string LabelCompanyName => Localization.Get("LabelCompanyName");
        public string LabelCompanyWebsite => Localization.Get("LabelCompanyWebsite");
        public string LabelCompanyContact => Localization.Get("LabelCompanyContact");
        public string ButtonAddCompany => Localization.Get("ButtonAddCompany");
        public string ButtonDeleteCompany => Localization.Get("ButtonDeleteCompany");
        public string HeaderCurrentCompanies => Localization.Get("HeaderCurrentCompanies");
        public string ColCompanyName => Localization.Get("ColCompanyName");
        public string ColCompanyWebsite => Localization.Get("ColCompanyWebsite");
        public string ColCompanyContact => Localization.Get("ColCompanyContact");

        private bool CanAddCompany() => !string.IsNullOrWhiteSpace(NewCompanyName);

        private void AddCompany()
        {
            var company = new CompanyDb { Name = NewCompanyName, Website = NewCompanyWebsite, ContactInfo = NewCompanyContactInfo };
            var id = _databaseManager.SaveCompany(company);
            company.Id = id;
            Companies.Add(company);
            NewCompanyName = string.Empty;
            NewCompanyWebsite = string.Empty;
            NewCompanyContactInfo = string.Empty;
        }

        private void DeleteCompany()
        {
            if (SelectedCompany != null)
            {
                _databaseManager.DeleteCompany(SelectedCompany.Id);
                Companies.Remove(SelectedCompany);
                SelectedCompany = null;
            }
        }

        private void OnLanguageChanged()
        {
            OnPropertyChanged(nameof(HeaderCompanies));
            OnPropertyChanged(nameof(InfoCompanies));
            OnPropertyChanged(nameof(HeaderAddNewCompany));
            OnPropertyChanged(nameof(LabelCompanyName));
            OnPropertyChanged(nameof(LabelCompanyWebsite));
            OnPropertyChanged(nameof(LabelCompanyContact));
            OnPropertyChanged(nameof(ButtonAddCompany));
            OnPropertyChanged(nameof(ButtonDeleteCompany));
            OnPropertyChanged(nameof(HeaderCurrentCompanies));
            OnPropertyChanged(nameof(ColCompanyName));
            OnPropertyChanged(nameof(ColCompanyWebsite));
            OnPropertyChanged(nameof(ColCompanyContact));
        }
    }
}
