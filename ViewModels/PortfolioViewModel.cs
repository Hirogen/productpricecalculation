using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ProductPriceCalculator.Infrastructure;
using ProductPriceCalculator.Services;

namespace ProductPriceCalculator.ViewModels
{
    /// <summary>
    /// ViewModel for product portfolio analysis
    /// </summary>
    public class PortfolioViewModel : ViewModelBase
    {
        private readonly DatabaseManager _databaseManager;
        private readonly IDialogService _dialogService;
        private readonly PriceCalculationService _calculationService;

        private PortfolioCalculationResult _portfolioResult;

        public PortfolioViewModel(
            DatabaseManager databaseManager,
            IDialogService dialogService,
            PriceCalculationService calculationService)
        {
            _databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _calculationService = calculationService ?? throw new ArgumentNullException(nameof(calculationService));

            AvailableProducts = new ObservableCollection<ProductSelectionItem>();
            
            InitializeCommands();
            LoadProducts();
        }

        #region Properties

        public ObservableCollection<ProductSelectionItem> AvailableProducts { get; }

        public PortfolioCalculationResult PortfolioResult
        {
            get => _portfolioResult;
            set => SetProperty(ref _portfolioResult, value);
        }

        public bool HasResults => PortfolioResult != null;

        // Localized strings
        public string HeaderProductPortfolio => Localization.Get("HeaderProductPortfolio");
        public string InfoPortfolio => Localization.Get("InfoPortfolio");
        public string HeaderProductSelection => Localization.Get("HeaderProductSelection");
        public string InfoSelectProducts => Localization.Get("InfoSelectProducts");
        public string ColSelected => Localization.Get("ColSelected");
        public string ColProduct => Localization.Get("ColProduct");
        public string ColBaseCost => Localization.Get("ColBaseCost");
        public string ColMarkup => Localization.Get("ColMarkup");
        public string ColExpectedUnits => Localization.Get("ColExpectedUnits");
        public string ButtonCalculatePortfolio => Localization.Get("ButtonCalculatePortfolio");
        public string HeaderPortfolioAnalysis => Localization.Get("HeaderPortfolioAnalysis");
        public string LabelTotalPortfolioUnits => Localization.Get("LabelTotalPortfolioUnits");
        public string LabelDistributedOperatingCost => Localization.Get("LabelDistributedOperatingCost");
        public string ColProductName => Localization.Get("ColProductName");
        public string ColPercentage => Localization.Get("ColPercentage");
        public string ColOperatingCostPerUnit => Localization.Get("ColOperatingCostPerUnit");
        public string ColFinalPrice => Localization.Get("ColFinalPrice");
        public string ColProfit => Localization.Get("ColProfit");
        public string LabelPerMonth => Localization.Get("LabelPerMonth");

        #endregion

        #region Commands

        public ICommand CalculatePortfolioCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        private void InitializeCommands()
        {
            CalculatePortfolioCommand = new RelayCommand(CalculatePortfolio, CanCalculatePortfolio);
            RefreshCommand = new RelayCommand(LoadProducts);
        }

        #endregion

        #region Methods

        public void LoadProducts()
        {
            AvailableProducts.Clear();
            var products = _databaseManager.GetAllProducts().Where(p => !p.IsComponent);

            foreach (var product in products)
            {
                AvailableProducts.Add(new ProductSelectionItem
                {
                    Product = product,
                    IsSelected = false
                });
            }
        }

        private bool CanCalculatePortfolio()
        {
            return AvailableProducts.Any(p => p.IsSelected);
        }

        private void CalculatePortfolio()
        {
            var selectedProducts = AvailableProducts.Where(p => p.IsSelected).Select(p => p.Product).ToList();

            if (selectedProducts.Count == 0)
            {
                _dialogService.ShowMessage(Localization.Get("MsgSelectAtLeastOneProduct"));
                return;
            }

            try
            {
                var operatingCosts = _databaseManager.GetOperatingCosts();
                var totalOperatingCosts = operatingCosts.Where(c => c.IsMonthly).Sum(c => c.Amount);

                var input = new PortfolioCalculationInput
                {
                    TotalOperatingCosts = totalOperatingCosts,
                    Products = selectedProducts.Select(p =>
                    {
                        var subproducts = _databaseManager.GetSubproducts(p.Id);
                        return new PortfolioProductInput
                        {
                            Name = p.Name,
                            ExpectedMonthlyUnits = p.ExpectedMonthlyUnits,
                            BaseCost = p.BaseCost,
                            SubproductsCost = subproducts.Sum(s => s.Cost),
                            Markup = p.Markup,
                            TaxRate = p.TaxRate
                        };
                    }).ToList()
                };

                PortfolioResult = _calculationService.CalculatePortfolio(input);
                OnPropertyChanged(nameof(HasResults));
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Error calculating portfolio: {ex.Message}");
            }
        }

        #endregion
    }

    public class ProductSelectionItem : ViewModelBase
    {
        private bool _isSelected;

        public Product Product { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
