using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProductPriceCalculator
{
    public class AddComponentDialog : Window
    {
        private ComboBox componentComboBox;
        private TextBox quantityBox;
        private DatabaseManager dbManager;
        private long currentProductId;
        
        public Product SelectedComponent { get; private set; }
        public double Quantity { get; private set; }

        public AddComponentDialog(DatabaseManager dbManager, long currentProductId)
        {
            this.dbManager = dbManager;
            this.currentProductId = currentProductId;
            
            InitializeDialog();
        }

        private void InitializeDialog()
        {
            // Window settings
            Title = Localization.Get("TitleAddComponent");
            Width = 500;
            Height = 280;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;

            // Main container
            var mainPanel = new StackPanel { Margin = new Thickness(20) };

            // Info text
            var infoText = new TextBlock
            {
                Text = Localization.Get("LabelSelectProduct"),
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            };
            mainPanel.Children.Add(infoText);

            // Component ComboBox
            var comboPanel = new StackPanel { Margin = new Thickness(0, 5, 0, 15) };
            
            var comboLabel = new TextBlock
            {
                Text = Localization.Get("LabelSelectProduct"),
                Margin = new Thickness(0, 0, 0, 5)
            };
            comboPanel.Children.Add(comboLabel);

            componentComboBox = new ComboBox
            {
                Height = 30,
                DisplayMemberPath = "Name"
            };

            // Load components (not products) and exclude current product
            var allProducts = dbManager.GetAllProducts();
            var components = allProducts
                .Where(p => p.IsComponent && p.Id != currentProductId)
                .ToList();
            componentComboBox.ItemsSource = components;

            comboPanel.Children.Add(componentComboBox);
            mainPanel.Children.Add(comboPanel);

            // Quantity input
            var quantityPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 20) };
            
            var quantityLabel = new TextBlock
            {
                Text = Localization.Get("LabelUnitsNeeded"),
                Width = 150,
                VerticalAlignment = VerticalAlignment.Center
            };
            quantityPanel.Children.Add(quantityLabel);

            quantityBox = new TextBox
            {
                Width = 100,
                Height = 25,
                Text = "1"
            };
            quantityPanel.Children.Add(quantityBox);

            mainPanel.Children.Add(quantityPanel);

            // Buttons panel
            var buttonsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 10, 0, 0)
            };

            var addButton = new Button
            {
                Content = Localization.Get("ButtonAdd"),
                Width = 100,
                Height = 30,
                Background = new SolidColorBrush(Color.FromRgb(0, 120, 215)),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                BorderThickness = new Thickness(0),
                Margin = new Thickness(0, 0, 10, 0)
            };
            addButton.Click += AddButton_Click;

            var cancelButton = new Button
            {
                Content = Localization.Get("ButtonCancel"),
                Width = 100,
                Height = 30,
                Background = new SolidColorBrush(Color.FromRgb(150, 150, 150)),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                BorderThickness = new Thickness(0)
            };
            cancelButton.Click += (s, e) => { DialogResult = false; Close(); };

            buttonsPanel.Children.Add(addButton);
            buttonsPanel.Children.Add(cancelButton);

            mainPanel.Children.Add(buttonsPanel);

            Content = mainPanel;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (componentComboBox.SelectedItem is Product selectedComponent &&
                double.TryParse(quantityBox.Text, out double qty) && qty > 0)
            {
                SelectedComponent = selectedComponent;
                Quantity = qty;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show(
                    Localization.Get("MsgSelectProduct"),
                    Localization.Get("MsgInvalidTitle"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
    }
}
