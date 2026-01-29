using System.Collections.ObjectModel;
using System.Windows.Input;
using ProductPriceCalculator.Infrastructure;
using ProductPriceCalculator.Models;

namespace ProductPriceCalculator.ViewModels
{
    public class CategoriesViewModel : ViewModelBase
    {
        private readonly DatabaseManager _databaseManager;
        private ProductCategoryDb _selectedCategory;
        private string _newCategoryName;
        private string _newCategoryDescription;

        public CategoriesViewModel(DatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
            Categories = new ObservableCollection<ProductCategoryDb>(_databaseManager.GetProductCategories());
            AddCategoryCommand = new RelayCommand(AddCategory, CanAddCategory);
            DeleteCategoryCommand = new RelayCommand(DeleteCategory, () => SelectedCategory != null);
            
            Localization.OnLanguageChanged += OnLanguageChanged;
        }

        public ObservableCollection<ProductCategoryDb> Categories { get; }

        public ProductCategoryDb SelectedCategory
        {
            get => _selectedCategory;
            set { SetProperty(ref _selectedCategory, value); }
        }

        public string NewCategoryName
        {
            get => _newCategoryName;
            set { SetProperty(ref _newCategoryName, value); }
        }

        public string NewCategoryDescription
        {
            get => _newCategoryDescription;
            set { SetProperty(ref _newCategoryDescription, value); }
        }

        public ICommand AddCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }

        // Localized properties
        public string HeaderCategories => Localization.Get("HeaderCategories");
        public string InfoCategories => Localization.Get("InfoCategories");
        public string HeaderAddNewCategory => Localization.Get("HeaderAddNewCategory");
        public string LabelCategoryName => Localization.Get("LabelCategoryName");
        public string LabelCategoryDescription => Localization.Get("LabelCategoryDescription");
        public string ButtonAddCategory => Localization.Get("ButtonAddCategory");
        public string ButtonDeleteCategory => Localization.Get("ButtonDeleteCategory");
        public string HeaderCurrentCategories => Localization.Get("HeaderCurrentCategories");
        public string ColCategoryName => Localization.Get("ColCategoryName");
        public string ColCategoryDescription => Localization.Get("ColCategoryDescription");

        private bool CanAddCategory() => !string.IsNullOrWhiteSpace(NewCategoryName);

        private void AddCategory()
        {
            var category = new ProductCategoryDb { Name = NewCategoryName, Description = NewCategoryDescription };
            var id = _databaseManager.SaveProductCategory(category);
            category.Id = id;
            Categories.Add(category);
            NewCategoryName = string.Empty;
            NewCategoryDescription = string.Empty;
        }

        private void DeleteCategory()
        {
            if (SelectedCategory == null)
                return;

            var result = Services.LocalizedMessageBox.ShowConfirmation(
                Localization.Get("MsgDeleteCategoryConfirm"),
                Localization.Get("MsgConfirmation"));

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                _databaseManager.DeleteProductCategory(SelectedCategory.Id);
                Categories.Remove(SelectedCategory);
                Services.LocalizedMessageBox.ShowInformation(Localization.Get("MsgCategoryDeleted"));
            }
        }

        private void OnLanguageChanged()
        {
            OnPropertyChanged(nameof(HeaderCategories));
            OnPropertyChanged(nameof(InfoCategories));
            OnPropertyChanged(nameof(HeaderAddNewCategory));
            OnPropertyChanged(nameof(LabelCategoryName));
            OnPropertyChanged(nameof(LabelCategoryDescription));
            OnPropertyChanged(nameof(ButtonAddCategory));
            OnPropertyChanged(nameof(ButtonDeleteCategory));
            OnPropertyChanged(nameof(HeaderCurrentCategories));
            OnPropertyChanged(nameof(ColCategoryName));
            OnPropertyChanged(nameof(ColCategoryDescription));
        }
    }
}
