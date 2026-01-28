using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProductPriceCalculator
{
    /// <summary>
    /// Product Price Calculator - WPF Desktop Application
    /// Calculates product prices based on rent, taxes, subproducts, and other factors
    /// </summary>
    public partial class MainWindow : Window
    {
        private PriceCalculatorViewModel viewModel;
        private DatabaseManager dbManager;

        public MainWindow()
        {
            // Initialize database manager
            dbManager = new DatabaseManager();

            // Initialize view model FIRST before InitializeComponent
            viewModel = new PriceCalculatorViewModel();

            // Load global operating costs on startup
            var operatingCosts = dbManager.GetOperatingCosts();
            foreach (var cost in operatingCosts)
            {
                viewModel.OperatingCosts.Add(cost);
            }

            // Initialize localization based on system culture (German/English)
            Localization.InitializeFromSystemCulture();

            InitializeComponent();
            this.DataContext = viewModel;

            // Subscribe to language changes
            Localization.OnLanguageChanged += RefreshUI;
        }

        private void InitializeComponent()
        {
            // Window Configuration
            this.Title = Localization.Get("AppTitle");
            this.Width = 1000;
            this.Height = 700;
            this.MinWidth = 800;
            this.MinHeight = 600;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Menu Bar
            var menu = CreateMenuBar();

            // Main Grid
            var mainGrid = new Grid();
            mainGrid.Margin = new Thickness(10);
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            Grid.SetRow(menu, 0);
            mainGrid.Children.Add(menu);

            // Content Grid
            var contentGrid = new Grid();
            Grid.SetRow(contentGrid, 1);

            // Define columns
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Navigation Panel (Left)
            var navPanel = CreateNavigationPanel();
            Grid.SetColumn(navPanel, 0);
            contentGrid.Children.Add(navPanel);

            // Content Area (Right)
            var contentBorder = new Border
            {
                Background = Brushes.White,
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(10, 0, 0, 0)
            };

            var contentScrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Padding = new Thickness(15)
            };

            viewModel.ContentPanel = new StackPanel();
            contentScrollViewer.Content = viewModel.ContentPanel;
            contentBorder.Child = contentScrollViewer;

            Grid.SetColumn(contentBorder, 1);
            contentGrid.Children.Add(contentBorder);

            mainGrid.Children.Add(contentGrid);

            this.Content = mainGrid;

            // Show default view
            ShowProductsView();
        }

        private Menu CreateMenuBar()
        {
            var menu = new Menu();

            var languageMenu = new MenuItem { Header = Localization.Get("MenuLanguage") };

            var englishItem = new MenuItem { Header = Localization.Get("MenuEnglish") };
            englishItem.Click += (s, e) => { Localization.CurrentLanguage = "en"; };

            var germanItem = new MenuItem { Header = Localization.Get("MenuGerman") };
            germanItem.Click += (s, e) => { Localization.CurrentLanguage = "de"; };

            languageMenu.Items.Add(englishItem);
            languageMenu.Items.Add(germanItem);

            menu.Items.Add(languageMenu);

            return menu;
        }

        private void RefreshUI()
        {
            // Update window title
            this.Title = Localization.Get("AppTitle");

            // Refresh current view
            ShowProductsView();
        }

        private StackPanel CreateNavigationPanel()
        {
            var panel = new StackPanel
            {
                Background = new SolidColorBrush(Color.FromRgb(45, 45, 48))
            };

            // Header
            var header = new TextBlock
            {
                Text = Localization.Get("NavWorkflows"),
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Padding = new Thickness(15, 20, 15, 15)
            };
            panel.Children.Add(header);

            // Navigation Buttons
            panel.Children.Add(CreateNavButton(Localization.Get("NavProducts"), ShowProductsView));
            panel.Children.Add(CreateNavButton(Localization.Get("NavCalculation"), (s, e) => ShowCalculation(s, e, viewModel.IsComponentValue)));
            panel.Children.Add(CreateNavButton(Localization.Get("NavProductPortfolio"), ShowProductPortfolioView));
            panel.Children.Add(CreateNavButton(Localization.Get("NavOperatingCosts"), ShowOperatingCostsView));
            panel.Children.Add(CreateNavButton(Localization.Get("NavAdvancedPricing"), ShowAdvancedPricingView));
            panel.Children.Add(CreateNavButton(Localization.Get("NavSummary"), ShowSummaryReport));

            return panel;
        }

        private Button CreateNavButton(string text, RoutedEventHandler clickHandler)
        {
            var btn = new Button
            {
                Content = text,
                Height = 45,
                Background = Brushes.Transparent,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Padding = new Thickness(20, 0, 0, 0),
                FontSize = 14,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            btn.Click += clickHandler;

            // Hover effect
            btn.MouseEnter += (s, e) => btn.Background = new SolidColorBrush(Color.FromRgb(62, 62, 66));
            btn.MouseLeave += (s, e) => btn.Background = Brushes.Transparent;

            return btn;
        }

        private void ShowProductsView(object sender = null, RoutedEventArgs e = null)
        {
            // Reset current product when entering Products view
            viewModel.CurrentProductId = 0;
            viewModel.ProductNameValue = "";
            viewModel.IsComponentValue = false;
            viewModel.BaseCostValue = "0";
            viewModel.MarkupPercentValue = "30";
            viewModel.TaxRateValue = "8.5";
            viewModel.QuantityValue = "1";
            viewModel.UnitsPerPackageValue = "1";
            viewModel.ExpectedUnitsValue = "100";
            viewModel.Subproducts.Clear();

            viewModel.ContentPanel.Children.Clear();

            // ========== PRODUCTS SECTION ==========
            AddSectionHeader(viewModel.ContentPanel, Localization.Get("HeaderProducts"));

            // Column Headers for Products
            var productsHeaderPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5), Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)) };
            productsHeaderPanel.Children.Add(new TextBlock { Text = "Product Name", Width = 250, FontWeight = FontWeights.Bold, Padding = new Thickness(5) });
            productsHeaderPanel.Children.Add(new TextBlock { Text = "Base Cost", Width = 100, FontWeight = FontWeights.Bold, Padding = new Thickness(5) });
            productsHeaderPanel.Children.Add(new TextBlock { Text = "Markup %", Width = 80, FontWeight = FontWeights.Bold, Padding = new Thickness(5) });
            viewModel.ContentPanel.Children.Add(productsHeaderPanel);

            // Products List
            var productsListBox = new ListBox { Height = 200, Margin = new Thickness(0, 0, 0, 10) };
            RefreshProductsList(productsListBox, isComponent: false);
            viewModel.ContentPanel.Children.Add(productsListBox);

            // Products Buttons
            var productsButtonsPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 20) };

            var newProductButton = new Button
            {
                Content = Localization.Get("ButtonNewProduct"),
                Width = 150,
                Height = 35,
                Background = new SolidColorBrush(Color.FromRgb(0, 120, 215)),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                BorderThickness = new Thickness(0),
                Margin = new Thickness(0, 0, 10, 0)
            };

            newProductButton.Click += (s, args) =>
            {
                viewModel.CurrentProductId = 0;
                viewModel.BaseCostValue = "0";
                viewModel.MarkupPercentValue = "30";
                viewModel.TaxRateValue = "8.5";
                viewModel.ProductNameValue = "";
                viewModel.Subproducts.Clear();
                ShowCalculation();
            };

            var loadProductButton = new Button
            {
                Content = Localization.Get("ButtonLoadProduct"),
                Width = 150,
                Height = 35,
                Background = new SolidColorBrush(Color.FromRgb(16, 124, 16)),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                BorderThickness = new Thickness(0),
                Margin = new Thickness(0, 0, 10, 0)
            };

            loadProductButton.Click += (s, args) =>
            {
                if (productsListBox.SelectedItem is Product selectedProduct)
                {
                    LoadProduct(selectedProduct.Id);
                    ShowCalculation(isComponent: viewModel.IsComponentValue);
                }
            };

            var deleteProductButton = new Button
            {
                Content = Localization.Get("ButtonDeleteProduct"),
                Width = 150,
                Height = 35,
                Background = new SolidColorBrush(Color.FromRgb(180, 0, 0)),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                BorderThickness = new Thickness(0)
            };

            deleteProductButton.Click += (s, args) =>
            {
                if (productsListBox.SelectedItem is Product selectedProduct)
                {
                    var result = MessageBox.Show(
                        Localization.Get("MsgDeleteConfirm"),
                        Localization.Get("MsgConfirmation"),
                        MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        dbManager.DeleteProduct(selectedProduct.Id);
                        RefreshProductsList(productsListBox, isComponent: false);
                        MessageBox.Show(Localization.Get("MsgProductDeleted"), Localization.Get("AppTitle"));
                    }
                }
            };

            productsButtonsPanel.Children.Add(newProductButton);
            productsButtonsPanel.Children.Add(loadProductButton);
            productsButtonsPanel.Children.Add(deleteProductButton);
            
            // Add Convert to Component button
            var convertToComponentButton = new Button
            {
                Content = "🔄 " + Localization.Get("ButtonConvertToComponent"),
                Width = 180,
                Height = 35,
                Background = new SolidColorBrush(Color.FromRgb(100, 100, 180)),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                BorderThickness = new Thickness(0),
                Margin = new Thickness(10, 0, 0, 0)
            };

            convertToComponentButton.Click += (s, args) =>
            {
                if (productsListBox.SelectedItem is Product selectedProduct)
                {
                    var result = MessageBox.Show(
                        Localization.Get("MsgConvertToComponentConfirm"),
                        Localization.Get("MsgConfirmation"),
                        MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        selectedProduct.IsComponent = true;
                        selectedProduct.Markup = 0;
                        selectedProduct.TaxRate = 0;
                        dbManager.SaveProduct(selectedProduct);
                        RefreshProductsList(productsListBox, isComponent: false);
                        MessageBox.Show(Localization.Get("MsgConvertedToComponent"), Localization.Get("AppTitle"));
                    }
                }
            };
            
            productsButtonsPanel.Children.Add(convertToComponentButton);
            viewModel.ContentPanel.Children.Add(productsButtonsPanel);

            // ========== COMPONENTS SECTION ==========
            AddSectionHeader(viewModel.ContentPanel, Localization.Get("HeaderComponents"));

            // Column Headers for Components
            var componentsHeaderPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5), Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)) };
            componentsHeaderPanel.Children.Add(new TextBlock { Text = "Component Name", Width = 200, FontWeight = FontWeights.Bold, Padding = new Thickness(5) });
            componentsHeaderPanel.Children.Add(new TextBlock { Text = "Base Cost", Width = 100, FontWeight = FontWeights.Bold, Padding = new Thickness(5) });
            componentsHeaderPanel.Children.Add(new TextBlock { Text = "Units/Package", Width = 100, FontWeight = FontWeights.Bold, Padding = new Thickness(5) });
            viewModel.ContentPanel.Children.Add(componentsHeaderPanel);

            // Components List
            var componentsListBox = new ListBox { Height = 200, Margin = new Thickness(0, 0, 0, 10) };
            RefreshProductsList(componentsListBox, isComponent: true);
            viewModel.ContentPanel.Children.Add(componentsListBox);

            // Components Buttons
            var componentsButtonsPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 20) };

            var newComponentButton = new Button
            {
                Content = Localization.Get("ButtonNewComponent"),
                Width = 180,
                Height = 35,
                Background = new SolidColorBrush(Color.FromRgb(120, 80, 0)),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                BorderThickness = new Thickness(0),
                Margin = new Thickness(0, 0, 10, 0)
            };

            newComponentButton.Click += (s, args) =>
            {
                viewModel.CurrentProductId = 0;
                viewModel.BaseCostValue = "0";
                viewModel.MarkupPercentValue = "0";
                viewModel.TaxRateValue = "0";
                viewModel.ProductNameValue = "";
                viewModel.UnitsPerPackageValue = "1";
                viewModel.Subproducts.Clear();
                ShowCalculation(isComponent: true);
            };

            var loadComponentButton = new Button
            {
                Content = Localization.Get("ButtonLoadProduct"),
                Width = 150,
                Height = 35,
                Background = new SolidColorBrush(Color.FromRgb(16, 124, 16)),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                BorderThickness = new Thickness(0),
                Margin = new Thickness(0, 0, 10, 0)
            };

            loadComponentButton.Click += (s, args) =>
            {
                if (componentsListBox.SelectedItem is Product selectedComponent)
                {
                    LoadProduct(selectedComponent.Id);
                    ShowCalculation(isComponent: viewModel.IsComponentValue);
                }
            };

            var deleteComponentButton = new Button
            {
                Content = Localization.Get("ButtonDeleteCost"),
                Width = 120,
                Height = 30,
                Background = new SolidColorBrush(Color.FromRgb(180, 0, 0)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Margin = new Thickness(10, 0, 0, 0)
            };

            deleteComponentButton.Click += (s, args) =>
            {
                if (componentsListBox.SelectedItem is Product selectedComponent)
                {
                    var result = MessageBox.Show(
                        Localization.Get("MsgDeleteConfirm"),
                        Localization.Get("MsgConfirmation"),
                        MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        dbManager.DeleteProduct(selectedComponent.Id);
                        RefreshProductsList(componentsListBox, isComponent: true);
                        MessageBox.Show(Localization.Get("MsgProductDeleted"), Localization.Get("AppTitle"));
                    }
                }
            };

            componentsButtonsPanel.Children.Add(newComponentButton);
            componentsButtonsPanel.Children.Add(loadComponentButton);
            componentsButtonsPanel.Children.Add(deleteComponentButton);
            
            // Add Convert to Product button
            var convertToProductButton = new Button
            {
                Content = "🔄 " + Localization.Get("ButtonConvertToProduct"),
                Width = 180,
                Height = 35,
                Background = new SolidColorBrush(Color.FromRgb(100, 100, 180)),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                BorderThickness = new Thickness(0),
                Margin = new Thickness(10, 0, 0, 0)
            };

            convertToProductButton.Click += (s, args) =>
            {
                if (componentsListBox.SelectedItem is Product selectedComponent)
                {
                    var result = MessageBox.Show(
                        Localization.Get("MsgConvertToProductConfirm"),
                        Localization.Get("MsgConfirmation"),
                        MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        selectedComponent.IsComponent = false;
                        if (selectedComponent.Markup == 0) selectedComponent.Markup = 30;
                        if (selectedComponent.TaxRate == 0) selectedComponent.TaxRate = 8.5;
                        dbManager.SaveProduct(selectedComponent);
                        RefreshProductsList(componentsListBox, isComponent: true);
                        MessageBox.Show(Localization.Get("MsgConvertedToProduct"), Localization.Get("AppTitle"));
                    }
                }
            };
            
            componentsButtonsPanel.Children.Add(convertToProductButton);
            viewModel.ContentPanel.Children.Add(componentsButtonsPanel);
        }

        private void RefreshProductsList(ListBox listBox, bool isComponent)
        {
            var allProducts = dbManager.GetAllProducts();
            var filteredProducts = allProducts.Where(p => p.IsComponent == isComponent).ToList();
            listBox.ItemsSource = filteredProducts;

            // Configure display template with columns
            listBox.ItemTemplate = new DataTemplate();
            var stackPanelFactory = new FrameworkElementFactory(typeof(StackPanel));
            stackPanelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            var nameText = new FrameworkElementFactory(typeof(TextBlock));
            nameText.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("Name"));
            nameText.SetValue(TextBlock.WidthProperty, isComponent ? 200.0 : 250.0);
            nameText.SetValue(TextBlock.PaddingProperty, new Thickness(5));
            stackPanelFactory.AppendChild(nameText);

            var costText = new FrameworkElementFactory(typeof(TextBlock));
            costText.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("BaseCost")
            {
                StringFormat = Localization.Get("CurrencySymbol") + "{0:F2}"
            });
            costText.SetValue(TextBlock.WidthProperty, 100.0);
            costText.SetValue(TextBlock.PaddingProperty, new Thickness(5));
            stackPanelFactory.AppendChild(costText);

            if (isComponent)
            {
                var unitsText = new FrameworkElementFactory(typeof(TextBlock));
                unitsText.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("UnitsPerPackage")
                {
                    StringFormat = "{0:F0}"
                });
                unitsText.SetValue(TextBlock.WidthProperty, 100.0);
                unitsText.SetValue(TextBlock.PaddingProperty, new Thickness(5));
                stackPanelFactory.AppendChild(unitsText);
            }
            else
            {
                var markupText = new FrameworkElementFactory(typeof(TextBlock));
                markupText.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("Markup")
                {
                    StringFormat = "{0:F0}%"
                });
                markupText.SetValue(TextBlock.WidthProperty, 80.0);
                markupText.SetValue(TextBlock.PaddingProperty, new Thickness(5));
                stackPanelFactory.AppendChild(markupText);
            }

            listBox.ItemTemplate.VisualTree = stackPanelFactory;
        }

        private void LoadProduct(long productId)
        {
            var product = dbManager.GetProduct(productId);
            if (product != null)
            {
                viewModel.CurrentProductId = product.Id;
                viewModel.ProductNameValue = product.Name;
                viewModel.IsComponentValue = product.IsComponent;
                viewModel.BaseCostValue = product.BaseCost.ToString();
                viewModel.MarkupPercentValue = product.Markup.ToString();
                viewModel.TaxRateValue = product.TaxRate.ToString();
                viewModel.ExpectedUnitsValue = product.ExpectedMonthlyUnits.ToString();
                viewModel.UnitsPerPackageValue = product.UnitsPerPackage.ToString();

                // Load subproducts
                viewModel.Subproducts.Clear();
                var subproducts = dbManager.GetSubproducts(productId);
                foreach (var sub in subproducts)
                {
                    viewModel.Subproducts.Add(new Subproduct
                    {
                        Id = sub.Id,
                        Name = sub.Name,
                        Description = sub.Description,
                        Cost = sub.Cost,
                        TaxRate = sub.TaxRate
                    });
                }

                // Operating costs are global - already loaded on startup
            }
        }

        private void ShowCalculation(object sender = null, RoutedEventArgs e = null, bool isComponent = false)
        {
            viewModel.IsComponentValue = isComponent;
            viewModel.ContentPanel.Children.Clear();

            // Different header based on whether it's a component or product
            AddSectionHeader(viewModel.ContentPanel, 
                isComponent ? Localization.Get("HeaderComponentCalculation") : Localization.Get("HeaderCalculation"));

            // Show info banner to indicate what we're creating
            if (isComponent)
            {
                var componentInfoPanel = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(255, 248, 220)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(200, 150, 0)),
                    BorderThickness = new Thickness(2),
                    CornerRadius = new CornerRadius(5),
                    Padding = new Thickness(10),
                    Margin = new Thickness(0, 0, 0, 15)
                };

                var componentInfoText = new TextBlock
                {
                    Text = "🔧 " + Localization.Get("InfoCreatingComponent"),
                    TextWrapping = TextWrapping.Wrap,
                    FontWeight = FontWeights.Bold
                };

                componentInfoPanel.Child = componentInfoText;
                viewModel.ContentPanel.Children.Add(componentInfoPanel);
            }

            // Product Name Input
            var namePanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 15) };
            namePanel.Children.Add(new TextBlock
            {
                Text = isComponent ? Localization.Get("LabelComponentName") : Localization.Get("LabelProductName"),
                Width = 200,
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = FontWeights.SemiBold
            });

            var productNameBox = new TextBox
            {
                Width = 300,
                Height = 25,
                Text = viewModel.ProductNameValue
            };
            productNameBox.TextChanged += (s, args) => { viewModel.ProductNameValue = productNameBox.Text; };
            namePanel.Children.Add(productNameBox);
            viewModel.ContentPanel.Children.Add(namePanel);

            // Create fresh TextBox instances for this view
            var baseCostBox = new TextBox { Text = viewModel.BaseCostValue.ToString() };
            var markupBox = new TextBox { Text = viewModel.MarkupPercentValue.ToString() };
            var taxRateBox = new TextBox { Text = viewModel.TaxRateValue.ToString() };
            var quantityBox = new TextBox { Text = viewModel.QuantityValue.ToString() };

            // Base Cost
            AddLabeledInput(viewModel.ContentPanel, Localization.Get("LabelBaseCost"), baseCostBox,
                (s, args) => { viewModel.BaseCostValue = baseCostBox.Text; });

            // For components, markup and tax are typically 0 or minimal
            if (isComponent)
            {
                var componentNotePanel = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(240, 248, 255)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(5),
                    Padding = new Thickness(8),
                    Margin = new Thickness(0, 5, 0, 10)
                };

                var componentNoteText = new TextBlock
                {
                    Text = "ℹ️ " + Localization.Get("InfoComponentPricing"),
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 11,
                    Foreground = Brushes.DarkBlue
                };

                componentNotePanel.Child = componentNoteText;
                viewModel.ContentPanel.Children.Add(componentNotePanel);
            }

            // Markup Percentage
            AddLabeledInput(viewModel.ContentPanel, Localization.Get("LabelMarkup"), markupBox,
                (s, args) => { viewModel.MarkupPercentValue = markupBox.Text; });

            // Tax Rate
            AddLabeledInput(viewModel.ContentPanel, Localization.Get("LabelTaxRate"), taxRateBox,
                (s, args) => { viewModel.TaxRateValue = taxRateBox.Text; });

            // Units Per Package (for bulk items)
            var unitsPerPackageBox = new TextBox { Text = viewModel.UnitsPerPackageValue.ToString() };
            AddLabeledInput(viewModel.ContentPanel, Localization.Get("LabelUnitsPerPackage"), unitsPerPackageBox,
                (s, args) => { viewModel.UnitsPerPackageValue = unitsPerPackageBox.Text; });

            // Info about bulk pricing
            var bulkInfoPanel = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(240, 248, 255)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(8),
                Margin = new Thickness(0, 5, 0, 10)
            };

            var bulkInfoText = new TextBlock
            {
                Text = "ℹ️ " + Localization.Get("InfoBulkPricing"),
                TextWrapping = TextWrapping.Wrap,
                FontSize = 11,
                Foreground = Brushes.DarkBlue
            };

            bulkInfoPanel.Child = bulkInfoText;
            viewModel.ContentPanel.Children.Add(bulkInfoPanel);

            // Quantity
            AddLabeledInput(viewModel.ContentPanel, Localization.Get("LabelQuantity"), quantityBox,
                (s, args) => { viewModel.QuantityValue = quantityBox.Text; });

            // Expected Monthly Units (for operating cost calculation) - only for products
            if (!isComponent)
            {
                var expectedUnitsBox = new TextBox { Text = viewModel.ExpectedUnitsValue.ToString() };
                AddLabeledInput(viewModel.ContentPanel, Localization.Get("LabelExpectedUnits"), expectedUnitsBox,
                    (s, args) => { viewModel.ExpectedUnitsValue = expectedUnitsBox.Text; });

                // Operating Costs Summary Display
                var operatingCostInfoPanel = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(255, 250, 230)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(255, 200, 100)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(5),
                    Margin = new Thickness(0, 10, 0, 15),
                    Padding = new Thickness(10)
                };

                var operatingCostText = new TextBlock
                {
                    FontSize = 12,
                    TextWrapping = TextWrapping.Wrap
                };

                double totalOperatingCosts = viewModel.OperatingCosts.Where(c => c.IsMonthly).Sum(c => c.Amount);
                double expectedUnits = ParseDouble(expectedUnitsBox.Text);
                double costPerUnit = expectedUnits > 0 ? totalOperatingCosts / expectedUnits : 0;

                operatingCostText.Text = string.Format(
                    "{0} {1}/month\n{2} {3}/month\n{4} {5}\n\n{6}",
                    Localization.Get("InfoOperatingCosts"),
                    Localization.FormatCurrency(totalOperatingCosts),
                    Localization.Get("InfoExpectedUnits"),
                    expectedUnits,
                    Localization.Get("InfoCostPerUnit"),
                    Localization.FormatCurrency(costPerUnit),
                    Localization.Get("InfoCostAddedBeforeMarkup")
                );

                operatingCostInfoPanel.Child = operatingCostText;
                viewModel.ContentPanel.Children.Add(operatingCostInfoPanel);

                // Button to manage operating costs
                var manageOperatingCostsButton = new Button
                {
                    Content = "⚙️ " + Localization.Get("ButtonManageOperatingCosts"),
                    Width = 250,
                    Height = 30,
                    Background = new SolidColorBrush(Color.FromRgb(100, 100, 100)),
                    Foreground = Brushes.White,
                    BorderThickness = new Thickness(0),
                    Margin = new Thickness(0, 0, 0, 15)
                };
                manageOperatingCostsButton.Click += (s, args) => ShowOperatingCostsView(null, null);
                viewModel.ContentPanel.Children.Add(manageOperatingCostsButton);
            }

            // === SUBPRODUCTS SECTION === (Only for products, not for components)
            if (!isComponent)
            {
                AddSectionHeader(viewModel.ContentPanel, Localization.Get("HeaderAddSubproducts"), 16);

                // Current Components List
                var componentsListBox = new ListBox
                {
                    Height = 150,
                    Margin = new Thickness(0, 10, 0, 10),
                    ItemsSource = viewModel.Subproducts
                };

                componentsListBox.ItemTemplate = new DataTemplate();
                var stackPanelFactory = new FrameworkElementFactory(typeof(StackPanel));
                stackPanelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
                stackPanelFactory.SetValue(StackPanel.MarginProperty, new Thickness(0, 3, 0, 3));

                var nameText = new FrameworkElementFactory(typeof(TextBlock));
                nameText.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("Name"));
                nameText.SetValue(TextBlock.WidthProperty, 200.0);
                nameText.SetValue(TextBlock.FontWeightProperty, FontWeights.SemiBold);
                stackPanelFactory.AppendChild(nameText);

                var costText = new FrameworkElementFactory(typeof(TextBlock));
                costText.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("Cost")
                {
                    StringFormat = Localization.Get("CurrencySymbol") + "{0:F2}"
                });
                costText.SetValue(TextBlock.WidthProperty, 80.0);
                stackPanelFactory.AppendChild(costText);

                var descText = new FrameworkElementFactory(typeof(TextBlock));
                descText.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("Description"));
                descText.SetValue(TextBlock.ForegroundProperty, Brushes.Gray);
                descText.SetValue(TextBlock.FontSizeProperty, 11.0);
                stackPanelFactory.AppendChild(descText);

                componentsListBox.ItemTemplate.VisualTree = stackPanelFactory;
                viewModel.ContentPanel.Children.Add(componentsListBox);

                // Buttons for managing components
                var componentButtonsPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 10, 0, 0) };

                var manageComponentsButton = new Button
                {
                    Content = Localization.Get("ButtonManageComponents"),
                    Width = 200,
                    Height = 30,
                    Background = new SolidColorBrush(Color.FromRgb(0, 120, 215)),
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.Bold,
                    BorderThickness = new Thickness(0),
                    Margin = new Thickness(0, 0, 10, 0)
                };

                manageComponentsButton.Click += (s, args) =>
                {
                    var dialog = new AddComponentDialog(dbManager, viewModel.CurrentProductId);
                    dialog.Owner = this;
                    
                    if (dialog.ShowDialog() == true)
                    {
                        var selectedComponent = dialog.SelectedComponent;
                        double unitsNeeded = dialog.Quantity;

                        // Load the component's full calculation
                        var fullProduct = dbManager.GetProduct(selectedComponent.Id);
                        var productSubproducts = dbManager.GetSubproducts(selectedComponent.Id);

                        // Calculate total cost of selected component including its subproducts
                        double productBaseCost = fullProduct.BaseCost;
                        double productSubproductsTotal = productSubproducts.Sum(sp => sp.Cost);
                        double productTotalCost = productBaseCost + productSubproductsTotal;

                        // Apply component's markup and tax to get package price
                        double productPriceAfterMarkup = productTotalCost * (1 + fullProduct.Markup / 100);
                        double packagePrice = productPriceAfterMarkup * (1 + fullProduct.TaxRate / 100);

                        // Calculate cost per unit based on UnitsPerPackage
                        double unitsPerPackage = fullProduct.UnitsPerPackage > 0 ? fullProduct.UnitsPerPackage : 1;
                        double costPerUnit = packagePrice / unitsPerPackage;

                        // Calculate total cost for units needed
                        double componentCost = costPerUnit * unitsNeeded;

                        // Add as subproduct with detailed breakdown
                        string description = unitsPerPackage > 1
                            ? $"Base: {Localization.FormatCurrency(fullProduct.BaseCost)}, +{fullProduct.Markup}% = {Localization.FormatCurrency(productPriceAfterMarkup)}, +{fullProduct.TaxRate}% = {Localization.FormatCurrency(packagePrice)} package ÷ {unitsPerPackage} units = {Localization.FormatCurrency(costPerUnit)}/unit × {unitsNeeded} = {Localization.FormatCurrency(componentCost)}"
                            : $"Base: {Localization.FormatCurrency(fullProduct.BaseCost)}, Markup: {fullProduct.Markup}%, Tax: {fullProduct.TaxRate}%, Final: {Localization.FormatCurrency(packagePrice)} × {unitsNeeded}";

                        viewModel.Subproducts.Add(new Subproduct
                        {
                            Name = unitsPerPackage > 1
                                ? $"{selectedComponent.Name} ({unitsNeeded} units)"
                                : $"{selectedComponent.Name} (x{unitsNeeded})",
                            Description = description,
                            Cost = componentCost,
                            TaxRate = 0
                        });

                        MessageBox.Show($"✓ Component added!\n✓ Komponente hinzugefügt!", Localization.Get("AppTitle"));
                    }
                };

                var deleteComponentButton = new Button
                {
                    Content = Localization.Get("ButtonDeleteCost"),
                    Width = 120,
                    Height = 30,
                    Background = new SolidColorBrush(Color.FromRgb(180, 0, 0)),
                    Foreground = Brushes.White,
                    BorderThickness = new Thickness(0),
                    Margin = new Thickness(10, 0, 0, 0)
                };

                deleteComponentButton.Click += (s, args) =>
                {
                    if (componentsListBox.SelectedItem is Subproduct selectedComp)
                    {
                        viewModel.Subproducts.Remove(selectedComp);
                        if (selectedComp.Id > 0)
                        {
                            dbManager.DeleteSubproduct(selectedComp.Id);
                        }
                    }
                };

                componentButtonsPanel.Children.Add(manageComponentsButton);
                componentButtonsPanel.Children.Add(deleteComponentButton);

                viewModel.ContentPanel.Children.Add(componentButtonsPanel);
            }
        }

        private void ShowSummaryReport(object sender, RoutedEventArgs e)
        {
            viewModel.ContentPanel.Children.Clear();

            AddSectionHeader(viewModel.ContentPanel, Localization.Get("HeaderSummary"));

            // Calculate comprehensive summary
            double baseCost = ParseDouble(viewModel.BaseCostValue);
            double markup = ParseDouble(viewModel.MarkupPercentValue);
            double tax = ParseDouble(viewModel.TaxRateValue);
            double subproductsTotal = viewModel.Subproducts.Sum(sp => sp.Cost);
            double totalOperatingCosts = viewModel.OperatingCosts.Where(c => c.IsMonthly).Sum(c => c.Amount);
            double expectedUnits = ParseDouble(viewModel.ExpectedUnitsValue);
            double operatingCostPerUnit = expectedUnits > 0 ? totalOperatingCosts / expectedUnits : 0;

            var summaryText = new TextBlock
            {
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 10, 0, 0)
            };

            // Total cost including operating costs per unit
            double totalCost = baseCost + subproductsTotal + operatingCostPerUnit;
            double priceBeforeTax = totalCost * (1 + markup / 100);
            double finalPrice = priceBeforeTax * (1 + tax / 100);
            double profitPerUnit = finalPrice - totalCost;

            summaryText.Text = $"{Localization.Get("SummaryCostBreakdown")}\n" +
                               $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                               $"{Localization.Get("SummaryBaseCost")} {Localization.FormatCurrency(baseCost)}\n" +
                               $"{Localization.Get("SummarySubproductsTotal")} {Localization.FormatCurrency(subproductsTotal)}\n" +
                               $"{Localization.Get("ResultOperatingCostPerUnit")} {Localization.FormatCurrency(operatingCostPerUnit)}\n" +
                               $"  ({Localization.FormatCurrency(totalOperatingCosts)}/month ÷ {expectedUnits} units)\n" +
                               $"{Localization.Get("SummaryTotalDirectCost")} {Localization.FormatCurrency(totalCost)}\n\n" +
                               $"{Localization.Get("SummaryPricing")}\n" +
                               $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                               $"{Localization.Get("SummaryMarkup")} ({markup}%): {Localization.FormatCurrency(priceBeforeTax - totalCost)}\n" +
                               $"{Localization.Get("SummaryPriceBeforeTax")} {Localization.FormatCurrency(priceBeforeTax)}\n" +
                               $"{Localization.Get("SummaryTax")} ({tax}%): {Localization.FormatCurrency(finalPrice - priceBeforeTax)}\n" +
                               $"{Localization.Get("SummaryFinalPrice")} {Localization.FormatCurrency(finalPrice)}\n\n" +
                               $"{Localization.Get("SummaryOperatingCosts")}\n" +
                               $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                               $"{Localization.Get("SummaryMonthlyOperating")} {Localization.FormatCurrency(totalOperatingCosts)}\n" +
                               $"{Localization.Get("InfoExpectedMonthlyUnits")} {expectedUnits}\n" +
                               $"{Localization.Get("SummaryOperatingPerUnit")} {Localization.FormatCurrency(operatingCostPerUnit)}\n\n" +
                               $"{Localization.Get("SummaryProfitability")}\n" +
                               $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                               $"{Localization.Get("SummaryGrossMargin")} {(markup / (1 + markup / 100) * 100):F2}%\n" +
                               $"{Localization.Get("SummaryProfitPerUnit")} {Localization.FormatCurrency(profitPerUnit)}\n" +
                               $"{string.Format(Localization.Get("InfoMonthlyProfit"), expectedUnits)} {Localization.FormatCurrency(profitPerUnit * expectedUnits)}";

            viewModel.ContentPanel.Children.Add(summaryText);

            // Export Button
            var exportButton = new Button
            {
                Content = Localization.Get("ButtonExportReport"),
                Width = 150,
                Height = 35,
                Margin = new Thickness(0, 20, 0, 0),
                Background = new SolidColorBrush(Color.FromRgb(16, 124, 16)),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                BorderThickness = new Thickness(0)
            };

            exportButton.Click += (s, args) =>
            {
                MessageBox.Show(Localization.Get("MsgExportReport"), Localization.Get("MsgExport"));
            };

            viewModel.ContentPanel.Children.Add(exportButton);
        }

        private void ShowProductPortfolioView(object sender, RoutedEventArgs e)
        {
            viewModel.ContentPanel.Children.Clear();

            AddSectionHeader(viewModel.ContentPanel, Localization.Get("HeaderProductPortfolio"));

            // Info banner
            var infoPanel = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(240, 248, 255)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 120, 215)),
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 15)
            };

            var infoText = new TextBlock
            {
                Text = "💡 " + Localization.Get("InfoPortfolio"),
                TextWrapping = TextWrapping.Wrap,
                FontWeight = FontWeights.SemiBold
            };

            infoPanel.Child = infoText;
            viewModel.ContentPanel.Children.Add(infoPanel);

            // Get all products (not components)
            var allProducts = dbManager.GetAllProducts().Where(p => !p.IsComponent).ToList();

            if (allProducts.Count == 0)
            {
                var noProductsText = new TextBlock
                {
                    Text = "⚠️ No products available. Please create products first.",
                    FontSize = 14,
                    Foreground = Brushes.DarkOrange,
                    Margin = new Thickness(0, 20, 0, 0)
                };
                viewModel.ContentPanel.Children.Add(noProductsText);
                return;
            }

            // Product selection list with checkboxes
            var productsListPanel = new StackPanel { Margin = new Thickness(0, 10, 0, 15) };
            var productCheckboxes = new System.Collections.Generic.Dictionary<long, CheckBox>();

            foreach (var product in allProducts)
            {
                var checkBox = new CheckBox
                {
                    Content = $"{product.Name} ({product.ExpectedMonthlyUnits} units/month)",
                    Margin = new Thickness(0, 5, 0, 5),
                    FontSize = 13
                };
                checkBox.Tag = product.Id;
                productCheckboxes[product.Id] = checkBox;
                productsListPanel.Children.Add(checkBox);
            }

            viewModel.ContentPanel.Children.Add(productsListPanel);

            // Calculate button
            var calculateButton = new Button
            {
                Content = Localization.Get("ButtonCalculatePortfolio"),
                Width = 250,
                Height = 35,
                Background = new SolidColorBrush(Color.FromRgb(0, 120, 215)),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                BorderThickness = new Thickness(0),
                Margin = new Thickness(0, 0, 0, 20)
            };

            // Results panel (initially hidden)
            var resultsPanel = new StackPanel { Visibility = Visibility.Collapsed, Margin = new Thickness(0, 20, 0, 0) };
            viewModel.ContentPanel.Children.Add(calculateButton);
            viewModel.ContentPanel.Children.Add(resultsPanel);

            calculateButton.Click += (s, args) =>
            {
                // Get selected products
                var selectedProductIds = productCheckboxes
                    .Where(kvp => kvp.Value.IsChecked == true)
                    .Select(kvp => kvp.Key)
                    .ToList();

                if (selectedProductIds.Count == 0)
                {
                    MessageBox.Show(
                        Localization.Get("MsgSelectAtLeastOneProduct"),
                        Localization.Get("MsgInvalidTitle"));
                    return;
                }

                // Clear previous results
                resultsPanel.Children.Clear();
                resultsPanel.Visibility = Visibility.Visible;

                // Get selected products with full data
                var selectedProducts = allProducts.Where(p => selectedProductIds.Contains(p.Id)).ToList();

                // Calculate total units across all selected products
                double totalUnits = selectedProducts.Sum(p => p.ExpectedMonthlyUnits);

                // Get total operating costs
                double totalOperatingCosts = viewModel.OperatingCosts.Where(c => c.IsMonthly).Sum(c => c.Amount);

                // Results header
                AddSectionHeader(resultsPanel, Localization.Get("HeaderSelectedProducts"), 16);

                // Summary info
                var summaryText = new TextBlock
                {
                    FontSize = 13,
                    FontWeight = FontWeights.SemiBold,
                    Margin = new Thickness(0, 0, 0, 15)
                };
                summaryText.Text = $"{Localization.Get("LabelTotalPortfolioUnits")} {totalUnits}\n" +
                                   $"{Localization.Get("LabelDistributedOperatingCost")} {Localization.FormatCurrency(totalOperatingCosts)}/month";
                resultsPanel.Children.Add(summaryText);

                // Results table header
                var headerPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Background = new SolidColorBrush(Color.FromRgb(230, 230, 230)),
                    Margin = new Thickness(0, 10, 0, 5)
                };

                headerPanel.Children.Add(new TextBlock { Text = Localization.Get("ColProductName"), Width = 150, Padding = new Thickness(5), FontWeight = FontWeights.Bold });
                headerPanel.Children.Add(new TextBlock { Text = Localization.Get("ColExpectedUnits"), Width = 100, Padding = new Thickness(5), FontWeight = FontWeights.Bold });
                headerPanel.Children.Add(new TextBlock { Text = Localization.Get("ColPercentage"), Width = 80, Padding = new Thickness(5), FontWeight = FontWeights.Bold });
                headerPanel.Children.Add(new TextBlock { Text = Localization.Get("ColOperatingCostPerUnit"), Width = 100, Padding = new Thickness(5), FontWeight = FontWeights.Bold });
                headerPanel.Children.Add(new TextBlock { Text = Localization.Get("ColFinalPrice"), Width = 100, Padding = new Thickness(5), FontWeight = FontWeights.Bold });

                resultsPanel.Children.Add(headerPanel);

                // Calculate and display each product
                foreach (var product in selectedProducts)
                {
                    // Calculate percentage of total units
                    double percentage = (product.ExpectedMonthlyUnits / totalUnits) * 100;

                    // Calculate operating cost share for this product
                    double productOperatingCostShare = totalOperatingCosts * (product.ExpectedMonthlyUnits / totalUnits);

                    // Operating cost per unit for this product
                    double operatingCostPerUnit = productOperatingCostShare / product.ExpectedMonthlyUnits;

                    // Get subproducts for this product
                    var subproducts = dbManager.GetSubproducts(product.Id);
                    double subproductsTotal = subproducts.Sum(sp => sp.Cost);

                    // Calculate final price
                    double totalDirectCost = product.BaseCost + subproductsTotal + operatingCostPerUnit;
                    double priceAfterMarkup = totalDirectCost * (1 + product.Markup / 100);
                    double finalPrice = priceAfterMarkup * (1 + product.TaxRate / 100);

                    // Display row
                    var rowPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(0, 2, 0, 2),
                        Background = new SolidColorBrush(Color.FromRgb(250, 250, 250))
                    };

                    rowPanel.Children.Add(new TextBlock { Text = product.Name, Width = 150, Padding = new Thickness(5) });
                    rowPanel.Children.Add(new TextBlock { Text = product.ExpectedMonthlyUnits.ToString("F0"), Width = 100, Padding = new Thickness(5) });
                    rowPanel.Children.Add(new TextBlock { Text = percentage.ToString("F1") + "%", Width = 80, Padding = new Thickness(5) });
                    rowPanel.Children.Add(new TextBlock { Text = Localization.FormatCurrency(operatingCostPerUnit), Width = 100, Padding = new Thickness(5) });
                    rowPanel.Children.Add(new TextBlock { Text = Localization.FormatCurrency(finalPrice), Width = 100, Padding = new Thickness(5), FontWeight = FontWeights.Bold });

                    resultsPanel.Children.Add(rowPanel);
                }

                // Summary explanation
                var explanationPanel = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(255, 255, 220)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 150)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(5),
                    Padding = new Thickness(10),
                    Margin = new Thickness(0, 15, 0, 0)
                };

                var explanationText = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 12
                };
                explanationText.Text = "📊 Operating costs are distributed proportionally based on each product's expected monthly sales volume. " +
                                       "Products with higher expected sales carry a larger share of the operating costs.\n\n" +
                                       "📊 Betriebskosten werden proportional basierend auf dem erwarteten monatlichen Verkaufsvolumen jedes Produkts verteilt. " +
                                       "Produkte mit höheren erwarteten Verkäufen tragen einen größeren Anteil der Betriebskosten.";

                explanationPanel.Child = explanationText;
                resultsPanel.Children.Add(explanationPanel);
            };
        }

        private void AddSectionHeader(Panel panel, string text, int fontSize = 18)
        {
            var header = new TextBlock
            {
                Text = text,
                FontSize = fontSize,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var line = new Border
            {
                Height = 2,
                Background = new SolidColorBrush(Color.FromRgb(0, 120, 215)),
                Margin = new Thickness(0, 0, 0, 10)
            };

            panel.Children.Add(header);
            panel.Children.Add(line);
        }

        private void AddLabeledInput(Panel panel, string label, TextBox textBox, TextChangedEventHandler textChanged)
        {
            var stackPanel = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(0, 0, 0, 15) };

            var labelBlock = new TextBlock
            {
                Text = label,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 5)
            };
            stackPanel.Children.Add(labelBlock);

            textBox.TextChanged += textChanged;
            stackPanel.Children.Add(textBox);

            panel.Children.Add(stackPanel);
        }

        private double ParseDouble(string text)
        {
            double result;
            double.TryParse(text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result);
            return result;
        }

        private void ShowOperatingCostsView(object sender, RoutedEventArgs e)
        {
            // Placeholder - to be implemented
            MessageBox.Show("Operating Costs view - Under construction");
        }

        private void ShowAdvancedPricingView(object sender, RoutedEventArgs e)
        {
            // Placeholder - to be implemented
            MessageBox.Show("Advanced Pricing view - Under construction");
        }

        private void CalculatePrice(TextBox baseCostBox, TextBox markupBox, TextBox taxRateBox, TextBox quantityBox)
        {
            try
            {
                double baseCost = ParseDouble(baseCostBox.Text);
                double markup = ParseDouble(markupBox.Text);
                double tax = ParseDouble(taxRateBox.Text);
                int quantity = (int)ParseDouble(quantityBox.Text);

                if (baseCost < 0 || markup < 0 || tax < 0 || quantity <= 0)
                {
                    MessageBox.Show(Localization.Get("MsgInvalidInput"), Localization.Get("MsgInvalidTitle"));
                    return;
                }

                // Calculate subproducts total
                double subproductsTotal = viewModel.Subproducts.Sum(sp => sp.Cost);

                // Calculate operating cost per unit
                double totalOperatingCosts = viewModel.OperatingCosts.Where(c => c.IsMonthly).Sum(c => c.Amount);
                double expectedUnits = ParseDouble(viewModel.ExpectedUnitsValue);
                double operatingCostPerUnit = expectedUnits > 0 ? totalOperatingCosts / expectedUnits : 0;

                // Total cost including subproducts AND operating costs per unit
                double totalDirectCost = baseCost + subproductsTotal + operatingCostPerUnit;

                // Apply markup to the total (including operating costs)
                double priceAfterMarkup = totalDirectCost * (1 + markup / 100);

                // Apply tax
                double priceAfterTax = priceAfterMarkup * (1 + tax / 100);

                // Calculate total for quantity
                double totalPrice = priceAfterTax * quantity;

                // Calculate profit per unit
                double profitPerUnit = priceAfterTax - totalDirectCost;

                // Build detailed result text
                var resultBuilder = new System.Text.StringBuilder();
                resultBuilder.AppendLine($"{Localization.Get("ResultUnitCalc")}\n");
                resultBuilder.AppendLine($"📋 {Localization.Get("ResultCostBreakdown")}");
                resultBuilder.AppendLine($"━━━━━━━━━━━━━━━━━━━━━━");
                resultBuilder.AppendLine($"{Localization.Get("ResultBaseCost")} {Localization.FormatCurrency(baseCost)}");

                if (viewModel.Subproducts.Count > 0)
                {
                    resultBuilder.AppendLine($"+ {Localization.Get("SummarySubproductsTotal")} {Localization.FormatCurrency(subproductsTotal)}");
                }

                if (operatingCostPerUnit > 0)
                {
                    resultBuilder.AppendLine($"+ {Localization.Get("ResultOperatingCostPerUnit")} {Localization.FormatCurrency(operatingCostPerUnit)}");
                    resultBuilder.AppendLine($"  ({Localization.Get("ResultTotal")} {Localization.FormatCurrency(totalOperatingCosts)} ÷ {expectedUnits} units)");
                }

                resultBuilder.AppendLine($"= {Localization.Get("SummaryTotalDirectCost")} {Localization.FormatCurrency(totalDirectCost)}\n");

                resultBuilder.AppendLine($"💰 {Localization.Get("ResultPricing")}");
                resultBuilder.AppendLine($"━━━━━━━━━━━━━━━━━━━━━━");
                resultBuilder.AppendLine($"{Localization.Get("ResultAfterMarkup")} (+{markup}%): {Localization.FormatCurrency(priceAfterMarkup)}");
                resultBuilder.AppendLine($"{Localization.Get("ResultAfterTax")} (+{tax}%): {Localization.FormatCurrency(priceAfterTax)}\n");

                resultBuilder.AppendLine($"📊 {Localization.Get("ResultSummary")}");
                resultBuilder.AppendLine($"━━━━━━━━━━━━━━━━━━━━━━");
                resultBuilder.AppendLine($"{Localization.Get("ResultFinalUnitPrice")} {Localization.FormatCurrency(priceAfterTax)}");
                resultBuilder.AppendLine($"{Localization.Get("ResultProfitPerUnit")} {Localization.FormatCurrency(profitPerUnit)}");
                resultBuilder.AppendLine($"{Localization.Get("ResultQuantity")} {quantity}");
                resultBuilder.AppendLine($"{Localization.Get("ResultTotalPrice")} {Localization.FormatCurrency(totalPrice)}");
                resultBuilder.AppendLine($"{Localization.Get("ResultTotalProfit")} {Localization.FormatCurrency(profitPerUnit * quantity)}");

                MessageBox.Show(resultBuilder.ToString(), Localization.Get("AppTitle"));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Localization.Get("MsgCalcError")} {ex.Message}", Localization.Get("MsgCalcErrorTitle"));
            }
        }
    }

    // ViewModel
    public class PriceCalculatorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public StackPanel ContentPanel { get; set; }
        public Border ResultPanel { get; set; }
        public TextBlock ResultText { get; set; }

        // Product ID tracking
        public long CurrentProductId { get; set; } = 0;
        public string ProductNameValue { get; set; } = "";
        public bool IsComponentValue { get; set; } = false;

        // Basic Calculation Fields - Store values as strings
        public string BaseCostValue { get; set; } = "0";
        public string MarkupPercentValue { get; set; } = "30";
        public string TaxRateValue { get; set; } = "8.5";
        public string QuantityValue { get; set; } = "1";
        public string UnitsPerPackageValue { get; set; } = "1";

        // Subproducts (with description and tax)
        public ObservableCollection<Subproduct> Subproducts { get; set; } = new ObservableCollection<Subproduct>();

        // Operating Costs (custom)
        public ObservableCollection<OperatingCost> OperatingCosts { get; set; } = new ObservableCollection<OperatingCost>();

        // Legacy values for backward compatibility
        public string MonthlyRentValue { get; set; } = "0";
        public string UtilitiesValue { get; set; } = "0";
        public string LaborCostsValue { get; set; } = "0";
        public string InsuranceValue { get; set; } = "0";
        public string OtherCostsValue { get; set; } = "0";
        public string ExpectedUnitsValue { get; set; } = "100";

        // Advanced Pricing - Store values as strings
        public string DiscountMinQtyValue { get; set; } = "10";
        public string DiscountPercentValue { get; set; } = "5";
    }

    // Subproduct Model
    public class Subproduct : INotifyPropertyChanged
    {
        private long id;
        private string name;
        private string description;
        private double cost;
        private double taxRate;

        public long Id
        {
            get => id;
            set { id = value; OnPropertyChanged(nameof(Id)); }
        }

        public string Name
        {
            get => name;
            set { name = value; OnPropertyChanged(nameof(Name)); }
        }

        public string Description
        {
            get => description;
            set { description = value; OnPropertyChanged(nameof(Description)); }
        }

        public double Cost
        {
            get => cost;
            set { cost = value; OnPropertyChanged(nameof(Cost)); }
        }

        public double TaxRate
        {
            get => taxRate;
            set { taxRate = value; OnPropertyChanged(nameof(TaxRate)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Application Entry Point
    public class App : Application
    {
        [STAThread]
        public static void Main()
        {
            var app = new App();
            app.Run(new MainWindow());
        }
    }
}