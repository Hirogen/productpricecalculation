using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ProductPriceCalculator.Infrastructure;
using ProductPriceCalculator.Services;
using ProductPriceCalculator.Models;

namespace ProductPriceCalculator.ViewModels
{
    /// <summary>
    /// ViewModel for product price calculation
    /// </summary>
    public class CalculationViewModel : ViewModelBase
    {
        private readonly DatabaseManager _databaseManager;
        private readonly IDialogService _dialogService;
        private readonly IStatusNotificationService _statusNotificationService;
        private readonly PriceCalculationService _calculationService;

        private long _currentProductId;
        private string _productName;
        private bool _isComponent;
        private double _baseCost;
        private double _markupPercent = 30;
        private double _taxRate = 8.5;
        private int _quantity = 1;
        private double _unitsPerPackage = 1;
        private double _expectedMonthlyUnits = 100;
        private string _productCategory;
        private string _company;
        private string _purchaseLink;
        private PriceCalculationResult _calculationResult;
        private bool _showCalculationDetails;

        public CalculationViewModel(
            DatabaseManager databaseManager,
            IDialogService dialogService,
            IStatusNotificationService statusNotificationService,
            PriceCalculationService calculationService)
        {
            _databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _statusNotificationService = statusNotificationService ?? throw new ArgumentNullException(nameof(statusNotificationService));
            _calculationService = calculationService ?? throw new ArgumentNullException(nameof(calculationService));

            Subproducts = new ObservableCollection<Subproduct>();
            OperatingCosts = new ObservableCollection<OperatingCost>();
            AvailableCategories = new ObservableCollection<string>();
            AvailableCompanies = new ObservableCollection<string>();

            InitializeCommands();
            LoadOperatingCosts();
            LoadCategoriesAndCompanies();
            
            // Subscribe to language changes
            Localization.OnLanguageChanged += OnLanguageChanged;
        }

        #region Properties

        public long CurrentProductId
        {
            get => _currentProductId;
            set => SetProperty(ref _currentProductId, value);
        }

        public string ProductName
        {
            get => _productName;
            set
            {
                if (SetProperty(ref _productName, value))
                    RecalculatePrice();
            }
        }

        public bool IsComponent
        {
            get => _isComponent;
            set
            {
                if (SetProperty(ref _isComponent, value))
                {
                    if (value)
                    {
                        MarkupPercent = 0;
                        TaxRate = 0;
                    }
                    RecalculatePrice();
                }
            }
        }

        public double BaseCost
        {
            get => _baseCost;
            set
            {
                if (SetProperty(ref _baseCost, value))
                    RecalculatePrice();
            }
        }

        public double MarkupPercent
        {
            get => _markupPercent;
            set
            {
                if (SetProperty(ref _markupPercent, value))
                    RecalculatePrice();
            }
        }

        public double TaxRate
        {
            get => _taxRate;
            set
            {
                if (SetProperty(ref _taxRate, value))
                    RecalculatePrice();
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (SetProperty(ref _quantity, value))
                    RecalculatePrice();
            }
        }

        public double UnitsPerPackage
        {
            get => _unitsPerPackage;
            set
            {
                if (SetProperty(ref _unitsPerPackage, value))
                    RecalculatePrice();
            }
        }

        public double ExpectedMonthlyUnits
        {
            get => _expectedMonthlyUnits;
            set
            {
                if (SetProperty(ref _expectedMonthlyUnits, value))
                    RecalculatePrice();
            }
        }

        public string ProductCategory
        {
            get => _productCategory;
            set => SetProperty(ref _productCategory, value);
        }

        public string Company
        {
            get => _company;
            set => SetProperty(ref _company, value);
        }

        public string PurchaseLink
        {
            get => _purchaseLink;
            set => SetProperty(ref _purchaseLink, value);
        }

        public ObservableCollection<Subproduct> Subproducts { get; }
        public ObservableCollection<OperatingCost> OperatingCosts { get; }
        public ObservableCollection<string> AvailableCategories { get; }
        public ObservableCollection<string> AvailableCompanies { get; }

        public PriceCalculationResult CalculationResult
        {
            get => _calculationResult;
            set => SetProperty(ref _calculationResult, value);
        }

        public bool ShowCalculationDetails
        {
            get => _showCalculationDetails;
            set => SetProperty(ref _showCalculationDetails, value);
        }

        public double TotalOperatingCosts => OperatingCosts.Where(c => c.IsMonthly).Sum(c => c.Amount);
        public double SubproductsTotalCost => Subproducts.Sum(s => s.Cost);

        // Localized strings
        public string LabelProductName => Localization.Get("LabelProductName");
        public string LabelProductCategory => Localization.Get("LabelProductCategory");
        public string LabelCompany => Localization.Get("LabelCompany");
        public string LabelPurchaseLink => Localization.Get("LabelPurchaseLink");
        public string LabelBaseCost => Localization.Get("LabelBaseCost");
        public string LabelMarkup => Localization.Get("LabelMarkup");
        public string LabelTaxRate => Localization.Get("LabelTaxRate");
        public string LabelUnitsPerPackage => Localization.Get("LabelUnitsPerPackage");
        public string LabelQuantity => Localization.Get("LabelQuantity");
        public string LabelExpectedUnits => Localization.Get("LabelExpectedUnits");
        public string InfoBulkPricing => Localization.Get("InfoBulkPricing");
        public string ButtonManageOperatingCosts => Localization.Get("ButtonManageOperatingCosts");
        public string InfoComponentPricing => Localization.Get("InfoComponentPricing");
        public string ButtonManageOperatingCostsIcon => Localization.Get("ButtonManageOperatingCostsIcon");
        public string HeaderComponentsUsed => Localization.Get("HeaderComponentsUsed");
        public string ButtonAddComponent => Localization.Get("ButtonAddComponent");
        public string ButtonDeleteSelected => Localization.Get("ButtonDeleteSelected");
        public string ButtonSave => Localization.Get("ButtonSave");
        public string ButtonShowDetails => Localization.Get("ButtonShowDetails");
        public string HeaderPriceCalculationResult => Localization.Get("HeaderPriceCalculationResult");
        public string LabelFinalPricePerUnit => Localization.Get("LabelFinalPricePerUnit");
        public string LabelTotalPrice => Localization.Get("LabelTotalPrice");
        public string LabelProfitPerUnit => Localization.Get("LabelProfitPerUnit");
        public string LabelGrossMargin => Localization.Get("LabelGrossMargin");
        public string LabelTotalMonthlyOperatingCosts => Localization.Get("LabelTotalMonthlyOperatingCosts");
        public string LabelExpectedMonthlyUnits => Localization.Get("LabelExpectedMonthlyUnits");
        public string LabelCostPerUnit => Localization.Get("LabelCostPerUnit");
        public string InfoOperatingCostsAddedBeforeMarkup => Localization.Get("InfoOperatingCostsAddedBeforeMarkup");
        public string LabelCalculation => Localization.Get("LabelCalculation");
        public string InfoCreatingComponentBanner => Localization.Get("InfoCreatingComponentBanner");
        public string HeaderDetailedCalculationBreakdown => Localization.Get("HeaderDetailedCalculationBreakdown");
        public string LabelSubproductsTotal => Localization.Get("LabelSubproductsTotal");
        public string LabelTotalDirectCost => Localization.Get("LabelTotalDirectCost");
        public string ColName => Localization.Get("ColName");
        
        // Table column headers (Issue #9)
        public string ColComponentName => Localization.Get("ColComponentName");
        public string ColCost => Localization.Get("ColCost");
        public string ColDescription => Localization.Get("ColDescription");
        
        // Detailed breakdown properties (Issue #7)
        public string ResultCostBreakdown => Localization.Get("ResultCostBreakdown");
        public string ResultBaseCost => Localization.Get("ResultBaseCost");
        public string ResultOperatingCostPerUnit => Localization.Get("ResultOperatingCostPerUnit");
        public string ResultPricing => Localization.Get("ResultPricing");
        public string ResultAfterMarkup => Localization.Get("ResultAfterMarkup");
        public string ResultAfterTax => Localization.Get("ResultAfterTax");
        public string ResultSummary => Localization.Get("ResultSummary");
        public string ResultFinalUnitPrice => Localization.Get("ResultFinalUnitPrice");
        public string ResultQuantity => Localization.Get("ResultQuantity");
        public string ResultTotalPrice => Localization.Get("ResultTotalPrice");

        #endregion

        #region Commands

        public ICommand SaveProductCommand { get; private set; }
        public ICommand CalculatePriceCommand { get; private set; }
        public ICommand AddComponentCommand { get; private set; }
        public ICommand DeleteSubproductCommand { get; private set; }

        private void InitializeCommands()
        {
            SaveProductCommand = new RelayCommand(SaveProduct, CanSaveProduct);
            CalculatePriceCommand = new RelayCommand(ToggleCalculationDetails);
            AddComponentCommand = new RelayCommand(AddComponent);
            DeleteSubproductCommand = new RelayCommand<Subproduct>(DeleteSubproduct);
        }

        #endregion

        #region Methods

        private void LoadCategoriesAndCompanies()
        {
            AvailableCategories.Clear();
            var categories = _databaseManager.GetProductCategories();
            foreach (var category in categories)
            {
                AvailableCategories.Add(category.Name);
            }

            AvailableCompanies.Clear();
            var companies = _databaseManager.GetCompanies();
            foreach (var company in companies)
            {
                AvailableCompanies.Add(company.Name);
            }
        }

        public void LoadProduct(long productId)
        {
            var product = _databaseManager.GetProduct(productId);
            if (product != null)
            {
                CurrentProductId = product.Id;
                ProductName = product.Name;
                IsComponent = product.IsComponent;
                BaseCost = product.BaseCost;
                MarkupPercent = product.Markup;
                TaxRate = product.TaxRate;
                ExpectedMonthlyUnits = product.ExpectedMonthlyUnits;
                UnitsPerPackage = product.UnitsPerPackage;
                ProductCategory = product.ProductCategory;
                Company = product.Company;
                PurchaseLink = product.PurchaseLink;

                Subproducts.Clear();
                var subproducts = _databaseManager.GetSubproducts(productId);
                foreach (var sub in subproducts)
                {
                    Subproducts.Add(new Subproduct
                    {
                        Id = sub.Id,
                        Name = sub.Name,
                        Description = sub.Description,
                        Cost = sub.Cost,
                        TaxRate = sub.TaxRate
                    });
                }

                RecalculatePrice();
            }
        }

        public void ResetForNewProduct(bool isComponent)
        {
            CurrentProductId = 0;
            ProductName = string.Empty;
            IsComponent = isComponent;
            BaseCost = 0;
            MarkupPercent = isComponent ? 0 : 30;
            TaxRate = isComponent ? 0 : 8.5;
            Quantity = 1;
            UnitsPerPackage = 1;
            ExpectedMonthlyUnits = 100;
            ProductCategory = string.Empty;
            Company = string.Empty;
            PurchaseLink = string.Empty;
            Subproducts.Clear();
            LoadCategoriesAndCompanies();
            RecalculatePrice();
        }

        private void LoadOperatingCosts()
        {
            OperatingCosts.Clear();
            var costs = _databaseManager.GetOperatingCosts();
            foreach (var cost in costs)
            {
                OperatingCosts.Add(cost);
            }
        }

        private void RecalculatePrice()
        {
            try
            {
                var input = new PriceCalculationInput
                {
                    BaseCost = BaseCost,
                    MarkupPercent = MarkupPercent,
                    TaxRatePercent = TaxRate,
                    SubproductsCost = SubproductsTotalCost,
                    TotalOperatingCosts = TotalOperatingCosts,
                    ExpectedMonthlyUnits = ExpectedMonthlyUnits,
                    Quantity = Quantity,
                    UnitsPerPackage = UnitsPerPackage
                };

                CalculationResult = _calculationService.Calculate(input);
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"{Localization.Get("MsgCalcError")} {ex.Message}");
            }
        }

        private bool CanSaveProduct()
        {
            return !string.IsNullOrWhiteSpace(ProductName) && BaseCost >= 0;
        }

        private void SaveProduct()
        {
            if (!CanSaveProduct())
            {
                _dialogService.ShowMessage(
                    IsComponent ? Localization.Get("MsgEnterComponentName") : Localization.Get("MsgEnterProductName"));
                return;
            }

            try
            {
                var product = new Product
                {
                    Id = CurrentProductId,
                    Name = ProductName,
                    BaseCost = BaseCost,
                    Markup = MarkupPercent,
                    TaxRate = TaxRate,
                    ExpectedMonthlyUnits = ExpectedMonthlyUnits,
                    UnitsPerPackage = UnitsPerPackage,
                    IsComponent = IsComponent,
                    ProductCategory = ProductCategory,
                    Company = Company,
                    PurchaseLink = PurchaseLink,
                    CreatedDate = CurrentProductId == 0 ? DateTime.Now : _databaseManager.GetProduct(CurrentProductId)?.CreatedDate ?? DateTime.Now,
                    LastModified = DateTime.Now
                };

                var savedId = _databaseManager.SaveProduct(product);
                CurrentProductId = savedId;

                // Save subproducts
                foreach (var subproduct in Subproducts.Where(s => s.Id == 0))
                {
                    var subproductDb = new SubproductDb
                    {
                        ProductId = savedId,
                        Name = subproduct.Name,
                        Description = subproduct.Description,
                        Cost = subproduct.Cost,
                        TaxRate = subproduct.TaxRate
                    };
                    subproduct.Id = _databaseManager.SaveSubproduct(subproductDb);
                }

                // Show success notification in status bar
                _statusNotificationService.ShowSuccess(
                    IsComponent ? Localization.Get("MsgComponentSaved") : Localization.Get("MsgProductSaved"));
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"{Localization.Get("MsgCalcError")} {ex.Message}");
            }
        }

        private void ToggleCalculationDetails()
        {
            if (CalculationResult == null)
            {
                RecalculatePrice();
            }

            ShowCalculationDetails = !ShowCalculationDetails;
        }

        private string BuildCalculationDetails()
        {
            var result = CalculationResult;
            var text = $"{Localization.Get("ResultUnitCalc")}\n\n";
            text += $"?? {Localization.Get("ResultCostBreakdown")}\n";
            text += "???????????????\n";
            text += $"{Localization.Get("ResultBaseCost")} {Localization.FormatCurrency(result.BaseCost)}\n";

            if (result.SubproductsCost > 0)
            {
                text += $"+ {Localization.Get("SummarySubproductsTotal")} {Localization.FormatCurrency(result.SubproductsCost)}\n";
            }

            if (result.OperatingCostPerUnit > 0)
            {
                text += $"+ {Localization.Get("ResultOperatingCostPerUnit")} {Localization.FormatCurrency(result.OperatingCostPerUnit)}\n";
            }

            text += $"= {Localization.Get("SummaryTotalDirectCost")} {Localization.FormatCurrency(result.TotalDirectCost)}\n\n";
            text += $"?? {Localization.Get("ResultPricing")}\n";
            text += "???????????????\n";
            text += $"{Localization.Get("ResultAfterMarkup")} (+{MarkupPercent}%): {Localization.FormatCurrency(result.PriceAfterMarkup)}\n";
            text += $"{Localization.Get("ResultAfterTax")} (+{TaxRate}%): {Localization.FormatCurrency(result.FinalPricePerUnit)}\n\n";
            text += $"?? {Localization.Get("ResultSummary")}\n";
            text += "???????????????\n";
            text += $"{Localization.Get("ResultFinalUnitPrice")} {Localization.FormatCurrency(result.FinalPricePerUnit)}\n";
            text += $"{Localization.Get("ResultQuantity")} {Quantity}\n";
            text += $"{Localization.Get("ResultTotalPrice")} {Localization.FormatCurrency(result.TotalPrice)}";

            return text;
        }

        private void AddComponent()
        {
            var dialog = new AddComponentDialog(_databaseManager, CurrentProductId);
            
            if (_dialogService.ShowDialog(dialog))
            {
                var selectedComponent = dialog.SelectedComponent;
                double unitsNeeded = dialog.Quantity;

                var fullProduct = _databaseManager.GetProduct(selectedComponent.Id);
                var productSubproducts = _databaseManager.GetSubproducts(selectedComponent.Id);

                double productBaseCost = fullProduct.BaseCost;
                double productSubproductsTotal = productSubproducts.Sum(sp => sp.Cost);
                double productTotalCost = productBaseCost + productSubproductsTotal;

                double productPriceAfterMarkup = productTotalCost * (1 + fullProduct.Markup / 100);
                double packagePrice = productPriceAfterMarkup * (1 + fullProduct.TaxRate / 100);

                double unitsPerPackage = fullProduct.UnitsPerPackage > 0 ? fullProduct.UnitsPerPackage : 1;
                double costPerUnit = packagePrice / unitsPerPackage;
                double componentCost = costPerUnit * unitsNeeded;

                string description = unitsPerPackage > 1
                    ? $"{Localization.Get("DescBase")}: {Localization.FormatCurrency(fullProduct.BaseCost)}, +{fullProduct.Markup}% = {Localization.FormatCurrency(productPriceAfterMarkup)}, +{fullProduct.TaxRate}% = {Localization.FormatCurrency(packagePrice)} {Localization.Get("DescPackage")} ÷ {unitsPerPackage} {Localization.Get("DescUnits")} = {Localization.FormatCurrency(costPerUnit)}{Localization.Get("DescPerUnit")} × {unitsNeeded} = {Localization.FormatCurrency(componentCost)}"
                    : $"{Localization.Get("DescBase")}: {Localization.FormatCurrency(fullProduct.BaseCost)}, {Localization.Get("DescMarkup")}: {fullProduct.Markup}%, {Localization.Get("DescTax")}: {fullProduct.TaxRate}%, {Localization.Get("DescFinal")}: {Localization.FormatCurrency(packagePrice)} × {unitsNeeded}";

                Subproducts.Add(new Subproduct
                {
                    Name = unitsPerPackage > 1
                        ? $"{selectedComponent.Name} ({unitsNeeded} {Localization.Get("DescUnits")})"
                        : $"{selectedComponent.Name} (×{unitsNeeded})",
                    Description = description,
                    Cost = componentCost,
                    TaxRate = 0
                });

                RecalculatePrice();
            }
        }

        private void DeleteSubproduct(Subproduct subproduct)
        {
            if (subproduct != null)
            {
                Subproducts.Remove(subproduct);
                if (subproduct.Id > 0)
                {
                    _databaseManager.DeleteSubproduct(subproduct.Id);
                }
                RecalculatePrice();
            }
        }
        
        private void OnLanguageChanged()
        {
            // Refresh all localized properties including table headers
            OnPropertyChanged(nameof(LabelProductName));
            OnPropertyChanged(nameof(LabelProductCategory));
            OnPropertyChanged(nameof(LabelCompany));
            OnPropertyChanged(nameof(LabelPurchaseLink));
            OnPropertyChanged(nameof(LabelBaseCost));
            OnPropertyChanged(nameof(LabelMarkup));
            OnPropertyChanged(nameof(LabelTaxRate));
            OnPropertyChanged(nameof(LabelUnitsPerPackage));
            OnPropertyChanged(nameof(LabelQuantity));
            OnPropertyChanged(nameof(LabelExpectedUnits));
            OnPropertyChanged(nameof(InfoBulkPricing));
            OnPropertyChanged(nameof(ButtonManageOperatingCosts));
            OnPropertyChanged(nameof(InfoComponentPricing));
            OnPropertyChanged(nameof(ButtonManageOperatingCostsIcon));
            OnPropertyChanged(nameof(HeaderComponentsUsed));
            OnPropertyChanged(nameof(ButtonAddComponent));
            OnPropertyChanged(nameof(ButtonDeleteSelected));
            OnPropertyChanged(nameof(ButtonSave));
            OnPropertyChanged(nameof(ButtonShowDetails));
            OnPropertyChanged(nameof(HeaderPriceCalculationResult));
            OnPropertyChanged(nameof(LabelFinalPricePerUnit));
            OnPropertyChanged(nameof(LabelTotalPrice));
            OnPropertyChanged(nameof(LabelProfitPerUnit));
            OnPropertyChanged(nameof(LabelGrossMargin));
            OnPropertyChanged(nameof(LabelTotalMonthlyOperatingCosts));
            OnPropertyChanged(nameof(LabelExpectedMonthlyUnits));
            OnPropertyChanged(nameof(LabelCostPerUnit));
            OnPropertyChanged(nameof(InfoOperatingCostsAddedBeforeMarkup));
            OnPropertyChanged(nameof(LabelCalculation));
            OnPropertyChanged(nameof(InfoCreatingComponentBanner));
            OnPropertyChanged(nameof(HeaderDetailedCalculationBreakdown));
            OnPropertyChanged(nameof(LabelSubproductsTotal));
            OnPropertyChanged(nameof(LabelTotalDirectCost));
            OnPropertyChanged(nameof(ColName));
            OnPropertyChanged(nameof(ColComponentName));
            OnPropertyChanged(nameof(ColCost));
            OnPropertyChanged(nameof(ColDescription));
            OnPropertyChanged(nameof(ResultCostBreakdown));
            OnPropertyChanged(nameof(ResultBaseCost));
            OnPropertyChanged(nameof(ResultOperatingCostPerUnit));
            OnPropertyChanged(nameof(ResultPricing));
            OnPropertyChanged(nameof(ResultAfterMarkup));
            OnPropertyChanged(nameof(ResultAfterTax));
            OnPropertyChanged(nameof(ResultSummary));
            OnPropertyChanged(nameof(ResultFinalUnitPrice));
            OnPropertyChanged(nameof(ResultQuantity));
            OnPropertyChanged(nameof(ResultTotalPrice));
        }

        #endregion
    }
}
