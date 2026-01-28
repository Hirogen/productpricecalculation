using System;
using System.Windows.Input;
using ProductPriceCalculator.Infrastructure;
using ProductPriceCalculator.Services;

namespace ProductPriceCalculator.ViewModels
{
    /// <summary>
    /// Main ViewModel that coordinates navigation between different views
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly DatabaseManager _databaseManager;
        private readonly IDialogService _dialogService;
        private readonly PriceCalculationService _calculationService;

        private ViewModelBase _currentViewModel;
        private string _currentLanguage;

        public MainViewModel()
        {
            _databaseManager = new DatabaseManager();
            _dialogService = new DialogService();
            _calculationService = new PriceCalculationService();

            // Initialize localization
            Localization.InitializeFromSystemCulture();
            _currentLanguage = Localization.CurrentLanguage;
            Localization.OnLanguageChanged += OnLanguageChanged;

            InitializeCommands();
            
            // Show products view by default
            NavigateToProducts();
        }

        #region Properties

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public string WindowTitle => Localization.Get("AppTitle");

        public string CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (SetProperty(ref _currentLanguage, value))
                {
                    Localization.CurrentLanguage = value;
                }
            }
        }

        // Localized strings for menus
        public string MenuLanguage => Localization.Get("MenuLanguage");
        public string MenuEnglish => Localization.Get("MenuEnglish");
        public string MenuGerman => Localization.Get("MenuGerman");

        // Localized strings for navigation
        public string NavWorkflows => Localization.Get("NavWorkflows");
        public string NavProducts => Localization.Get("NavProducts");
        public string NavCalculation => Localization.Get("NavCalculation");
        public string NavProductPortfolio => Localization.Get("NavProductPortfolio");
        public string NavOperatingCosts => Localization.Get("NavOperatingCosts");
        public string NavCategories => Localization.Get("NavCategories");
        public string NavCompanies => Localization.Get("NavCompanies");

        #endregion

        #region Commands

        public ICommand NavigateToProductsCommand { get; private set; }
        public ICommand NavigateToCalculationCommand { get; private set; }
        public ICommand NavigateToOperatingCostsCommand { get; private set; }
        public ICommand NavigateToPortfolioCommand { get; private set; }
        public ICommand NavigateToCategoriesCommand { get; private set; }
        public ICommand NavigateToCompaniesCommand { get; private set; }
        public ICommand SwitchToEnglishCommand { get; private set; }
        public ICommand SwitchToGermanCommand { get; private set; }

        private void InitializeCommands()
        {
            NavigateToProductsCommand = new RelayCommand(NavigateToProducts);
            NavigateToCalculationCommand = new RelayCommand(() => NavigateToCalculation(0, false));
            NavigateToOperatingCostsCommand = new RelayCommand(NavigateToOperatingCosts);
            NavigateToPortfolioCommand = new RelayCommand(NavigateToPortfolio);
            NavigateToCategoriesCommand = new RelayCommand(NavigateToCategories);
            NavigateToCompaniesCommand = new RelayCommand(NavigateToCompanies);
            SwitchToEnglishCommand = new RelayCommand(() => CurrentLanguage = "en");
            SwitchToGermanCommand = new RelayCommand(() => CurrentLanguage = "de");
        }

        #endregion

        #region Navigation Methods

        public void NavigateToProducts()
        {
            var viewModel = new ProductsViewModel(_databaseManager, _dialogService, NavigateToCalculation);
            CurrentViewModel = viewModel;
        }

        public void NavigateToCalculation(long productId, bool isComponent)
        {
            var viewModel = new CalculationViewModel(_databaseManager, _dialogService, _calculationService);
            
            if (productId > 0)
            {
                viewModel.LoadProduct(productId);
            }
            else
            {
                viewModel.ResetForNewProduct(isComponent);
            }

            CurrentViewModel = viewModel;
        }

        public void NavigateToOperatingCosts()
        {
            var viewModel = new OperatingCostsViewModel(_databaseManager, _dialogService);
            CurrentViewModel = viewModel;
        }

        public void NavigateToPortfolio()
        {
            var viewModel = new PortfolioViewModel(_databaseManager, _dialogService, _calculationService);
            CurrentViewModel = viewModel;
        }

        public void NavigateToCategories()
        {
            var viewModel = new CategoriesViewModel(_databaseManager);
            CurrentViewModel = viewModel;
        }

        public void NavigateToCompanies()
        {
            var viewModel = new CompaniesViewModel(_databaseManager);
            CurrentViewModel = viewModel;
        }

        #endregion

        #region Private Methods

        private void OnLanguageChanged()
        {
            OnPropertyChanged(nameof(WindowTitle));
            OnPropertyChanged(nameof(MenuLanguage));
            OnPropertyChanged(nameof(MenuEnglish));
            OnPropertyChanged(nameof(MenuGerman));
            OnPropertyChanged(nameof(NavWorkflows));
            OnPropertyChanged(nameof(NavProducts));
            OnPropertyChanged(nameof(NavCalculation));
            OnPropertyChanged(nameof(NavProductPortfolio));
            OnPropertyChanged(nameof(NavOperatingCosts));
            OnPropertyChanged(nameof(NavCategories));
            OnPropertyChanged(nameof(NavCompanies));
            
            // Refresh current view to update localized text
            if (CurrentViewModel is ProductsViewModel)
                NavigateToProducts();
            else if (CurrentViewModel is CalculationViewModel calc)
                NavigateToCalculation(calc.CurrentProductId, calc.IsComponent);
            else if (CurrentViewModel is OperatingCostsViewModel)
                NavigateToOperatingCosts();
            else if (CurrentViewModel is PortfolioViewModel)
                NavigateToPortfolio();
            else if (CurrentViewModel is CategoriesViewModel)
                NavigateToCategories();
            else if (CurrentViewModel is CompaniesViewModel)
                NavigateToCompanies();
        }

        #endregion
    }
}
