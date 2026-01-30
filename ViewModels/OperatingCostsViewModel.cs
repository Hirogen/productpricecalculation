using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ProductPriceCalculator.Infrastructure;
using ProductPriceCalculator.Services;

namespace ProductPriceCalculator.ViewModels
{
    /// <summary>
    /// ViewModel for managing operating costs
    /// </summary>
    public class OperatingCostsViewModel : ViewModelBase
    {
        private readonly DatabaseManager _databaseManager;
        private readonly IDialogService _dialogService;
        private readonly IStatusNotificationService _statusNotificationService;

        private OperatingCost _selectedCost;
        private string _newCostName;
        private double _newCostAmount;
        private bool _newCostIsMonthly = true;

        public OperatingCostsViewModel(DatabaseManager databaseManager, IDialogService dialogService, IStatusNotificationService statusNotificationService)
        {
            _databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _statusNotificationService = statusNotificationService ?? throw new ArgumentNullException(nameof(statusNotificationService));

            OperatingCosts = new ObservableCollection<OperatingCost>();

            InitializeCommands();
            LoadOperatingCosts();
            
            // Subscribe to language changes
            Localization.OnLanguageChanged += OnLanguageChanged;
        }

        #region Properties

        public ObservableCollection<OperatingCost> OperatingCosts { get; }

        public OperatingCost SelectedCost
        {
            get => _selectedCost;
            set => SetProperty(ref _selectedCost, value);
        }

        public string NewCostName
        {
            get => _newCostName;
            set => SetProperty(ref _newCostName, value);
        }

        public double NewCostAmount
        {
            get => _newCostAmount;
            set => SetProperty(ref _newCostAmount, value);
        }

        public bool NewCostIsMonthly
        {
            get => _newCostIsMonthly;
            set => SetProperty(ref _newCostIsMonthly, value);
        }

        public double TotalMonthlyCosts => OperatingCosts.Where(c => c.IsMonthly).Sum(c => c.Amount);

        // Localized strings
        public string HeaderOperatingCosts => Localization.Get("NavOperatingCosts");
        public string InfoOperatingCostsBanner => Localization.Get("InfoOperatingCostsBanner");
        public string HeaderAddNewCost => Localization.Get("HeaderAddNewOperatingCost");
        public string LabelCostName => Localization.Get("LabelCostName");
        public string LabelAmount => Localization.Get("LabelAmount");
        public string LabelIsMonthly => Localization.Get("LabelIsMonthly");
        public string ButtonAddCost => Localization.Get("ButtonAddCost");
        public string HeaderCurrentCosts => Localization.Get("HeaderCurrentCosts");
        public string ColName => Localization.Get("ColProductName");
        public string ColAmount => Localization.Get("ColAmount");
        public string ColType => Localization.Get("ColType");
        public string LabelTotalMonthlyCosts => Localization.Get("InfoOperatingCosts");
        public string ButtonDeleteCost => Localization.Get("ButtonDeleteCost");

        #endregion

        #region Commands

        public ICommand AddCostCommand { get; private set; }
        public ICommand DeleteCostCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        private void InitializeCommands()
        {
            AddCostCommand = new RelayCommand(AddCost, CanAddCost);
            DeleteCostCommand = new RelayCommand(DeleteCost, () => SelectedCost != null);
            RefreshCommand = new RelayCommand(LoadOperatingCosts);
        }

        #endregion

        #region Methods

        public void LoadOperatingCosts()
        {
            OperatingCosts.Clear();
            var costs = _databaseManager.GetOperatingCosts();
            foreach (var cost in costs)
            {
                OperatingCosts.Add(cost);
            }
            OnPropertyChanged(nameof(TotalMonthlyCosts));
        }

        private bool CanAddCost()
        {
            return !string.IsNullOrWhiteSpace(NewCostName) && NewCostAmount > 0;
        }

        private void AddCost()
        {
            if (!CanAddCost())
            {
                _dialogService.ShowMessage("Please enter a valid cost name and amount.");
                return;
            }

            try
            {
                var cost = new OperatingCost
                {
                    Name = NewCostName,
                    Amount = NewCostAmount,
                    IsMonthly = NewCostIsMonthly
                };

                cost.Id = _databaseManager.SaveOperatingCost(cost);
                OperatingCosts.Add(cost);

                // Reset form
                NewCostName = string.Empty;
                NewCostAmount = 0;
                NewCostIsMonthly = true;

                OnPropertyChanged(nameof(TotalMonthlyCosts));
                
                // Show success notification in status bar
                _statusNotificationService.ShowSuccess(Localization.Get("MsgOperatingCostAdded"));
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Error adding cost: {ex.Message}");
            }
        }

        private void DeleteCost()
        {
            if (SelectedCost == null) return;

            if (_dialogService.ShowConfirmation($"Delete '{SelectedCost.Name}'?"))
            {
                try
                {
                    _databaseManager.DeleteOperatingCost(SelectedCost.Id);
                    OperatingCosts.Remove(SelectedCost);
                    OnPropertyChanged(nameof(TotalMonthlyCosts));
                    
                    // Show success notification in status bar
                    _statusNotificationService.ShowSuccess(Localization.Get("MsgOperatingCostDeleted"));
                }
                catch (Exception ex)
                {
                    _dialogService.ShowError($"Error deleting cost: {ex.Message}");
                }
            }
        }
        
        private void OnLanguageChanged()
        {
            // Refresh all localized properties including table headers
            OnPropertyChanged(nameof(HeaderOperatingCosts));
            OnPropertyChanged(nameof(InfoOperatingCostsBanner));
            OnPropertyChanged(nameof(HeaderAddNewCost));
            OnPropertyChanged(nameof(LabelCostName));
            OnPropertyChanged(nameof(LabelAmount));
            OnPropertyChanged(nameof(LabelIsMonthly));
            OnPropertyChanged(nameof(ButtonAddCost));
            OnPropertyChanged(nameof(HeaderCurrentCosts));
            OnPropertyChanged(nameof(ColName));
            OnPropertyChanged(nameof(ColAmount));
            OnPropertyChanged(nameof(ColType));
            OnPropertyChanged(nameof(LabelTotalMonthlyCosts));
            OnPropertyChanged(nameof(ButtonDeleteCost));
        }

        #endregion
    }
}
