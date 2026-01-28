using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ProductPriceCalculator.Infrastructure;
using ProductPriceCalculator.Services;

namespace ProductPriceCalculator.ViewModels
{
    /// <summary>
    /// ViewModel for managing products and components
    /// </summary>
    public class ProductsViewModel : ViewModelBase
    {
        private readonly DatabaseManager _databaseManager;
        private readonly IDialogService _dialogService;
        private readonly Action<long, bool> _navigateToCalculation;

        private ObservableCollection<Product> _products;
        private ObservableCollection<Product> _components;
        private Product _selectedProduct;
        private Product _selectedComponent;

        public ProductsViewModel(
            DatabaseManager databaseManager, 
            IDialogService dialogService,
            Action<long, bool> navigateToCalculation)
        {
            _databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _navigateToCalculation = navigateToCalculation ?? throw new ArgumentNullException(nameof(navigateToCalculation));

            Products = new ObservableCollection<Product>();
            Components = new ObservableCollection<Product>();

            InitializeCommands();
            LoadProducts();
        }

        #region Properties

        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        public ObservableCollection<Product> Components
        {
            get => _components;
            set => SetProperty(ref _components, value);
        }

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public Product SelectedComponent
        {
            get => _selectedComponent;
            set => SetProperty(ref _selectedComponent, value);
        }

        // Localized strings
        public string HeaderProducts => Localization.Get("HeaderProducts");
        public string HeaderComponents => Localization.Get("HeaderComponents");
        public string ButtonNewProduct => Localization.Get("ButtonNewProduct");
        public string ButtonLoadProduct => Localization.Get("ButtonLoadProduct");
        public string ButtonDeleteProduct => Localization.Get("ButtonDeleteProduct");
        public string ButtonConvertToComponent => Localization.Get("ButtonConvertToComponent");
        public string ButtonNewComponent => Localization.Get("ButtonNewComponent");
        public string ButtonLoadComponent => Localization.Get("ButtonLoadComponent");
        public string ButtonDeleteComponent => Localization.Get("ButtonDeleteComponent");
        public string ButtonConvertToProduct => Localization.Get("ButtonConvertToProduct");
        public string ColProductName => Localization.Get("ColProductName");

        #endregion

        #region Commands

        public ICommand NewProductCommand { get; private set; }
        public ICommand LoadProductCommand { get; private set; }
        public ICommand DeleteProductCommand { get; private set; }
        public ICommand ConvertToComponentCommand { get; private set; }

        public ICommand NewComponentCommand { get; private set; }
        public ICommand LoadComponentCommand { get; private set; }
        public ICommand DeleteComponentCommand { get; private set; }
        public ICommand ConvertToProductCommand { get; private set; }

        private void InitializeCommands()
        {
            NewProductCommand = new RelayCommand(() => _navigateToCalculation(0, false));
            LoadProductCommand = new RelayCommand(LoadProduct, () => SelectedProduct != null);
            DeleteProductCommand = new RelayCommand(DeleteProduct, () => SelectedProduct != null);
            ConvertToComponentCommand = new RelayCommand(ConvertToComponent, () => SelectedProduct != null);

            NewComponentCommand = new RelayCommand(() => _navigateToCalculation(0, true));
            LoadComponentCommand = new RelayCommand(LoadComponent, () => SelectedComponent != null);
            DeleteComponentCommand = new RelayCommand(DeleteComponent, () => SelectedComponent != null);
            ConvertToProductCommand = new RelayCommand(ConvertToProduct, () => SelectedComponent != null);
        }

        #endregion

        #region Methods

        public void LoadProducts()
        {
            var allProducts = _databaseManager.GetAllProducts();
            
            Products.Clear();
            foreach (var product in allProducts.Where(p => !p.IsComponent))
            {
                Products.Add(product);
            }

            Components.Clear();
            foreach (var component in allProducts.Where(p => p.IsComponent))
            {
                Components.Add(component);
            }
        }

        private void LoadProduct()
        {
            if (SelectedProduct != null)
            {
                _navigateToCalculation(SelectedProduct.Id, false);
            }
        }

        private void DeleteProduct()
        {
            if (SelectedProduct == null) return;

            if (_dialogService.ShowConfirmation(Localization.Get("MsgDeleteConfirm")))
            {
                _databaseManager.DeleteProduct(SelectedProduct.Id);
                LoadProducts();
                _dialogService.ShowMessage(Localization.Get("MsgProductDeleted"));
            }
        }

        private void ConvertToComponent()
        {
            if (SelectedProduct == null) return;

            if (_dialogService.ShowConfirmation(Localization.Get("MsgConvertToComponentConfirm")))
            {
                SelectedProduct.IsComponent = true;
                SelectedProduct.Markup = 0;
                SelectedProduct.TaxRate = 0;
                _databaseManager.SaveProduct(SelectedProduct);
                LoadProducts();
                _dialogService.ShowMessage(Localization.Get("MsgConvertedToComponent"));
            }
        }

        private void LoadComponent()
        {
            if (SelectedComponent != null)
            {
                _navigateToCalculation(SelectedComponent.Id, true);
            }
        }

        private void DeleteComponent()
        {
            if (SelectedComponent == null) return;

            if (_dialogService.ShowConfirmation(Localization.Get("MsgDeleteConfirm")))
            {
                _databaseManager.DeleteProduct(SelectedComponent.Id);
                LoadProducts();
                _dialogService.ShowMessage(Localization.Get("MsgProductDeleted"));
            }
        }

        private void ConvertToProduct()
        {
            if (SelectedComponent == null) return;

            if (_dialogService.ShowConfirmation(Localization.Get("MsgConvertToProductConfirm")))
            {
                SelectedComponent.IsComponent = false;
                if (SelectedComponent.Markup == 0) SelectedComponent.Markup = 30;
                if (SelectedComponent.TaxRate == 0) SelectedComponent.TaxRate = 8.5;
                _databaseManager.SaveProduct(SelectedComponent);
                LoadProducts();
                _dialogService.ShowMessage(Localization.Get("MsgConvertedToProduct"));
            }
        }

        #endregion
    }
}
