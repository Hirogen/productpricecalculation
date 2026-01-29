using System;
using System.Windows.Input;
using ProductPriceCalculator.Infrastructure;
using ProductPriceCalculator.Models;
using ProductPriceCalculator.Services;

namespace ProductPriceCalculator.ViewModels
{
    /// <summary>
    /// ViewModel for the status bar notification display
    /// </summary>
    public class StatusBarViewModel : ViewModelBase
    {
        private readonly IStatusNotificationService _notificationService;
        private StatusNotification? _currentNotification;
        private bool _isVisible;

        public StatusBarViewModel(IStatusNotificationService notificationService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            
            DismissCommand = new RelayCommand(Dismiss);
            
            // Subscribe to notification changes
            _notificationService.NotificationChanged += OnNotificationReceived;
        }

        public StatusNotification? CurrentNotification
        {
            get => _currentNotification;
            private set
            {
                if (SetProperty(ref _currentNotification, value))
                {
                    // Notify dependent properties
                    OnPropertyChanged(nameof(NotificationMessage));
                    OnPropertyChanged(nameof(NotificationIcon));
                    OnPropertyChanged(nameof(NotificationBackground));
                    OnPropertyChanged(nameof(NotificationForeground));
                }
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            private set => SetProperty(ref _isVisible, value);
        }

        public string NotificationMessage => CurrentNotification?.Message ?? string.Empty;
        
        public string NotificationIcon => GetIconForType(CurrentNotification?.Type ?? StatusNotificationType.Info);
        
        public string NotificationBackground => GetBackgroundForType(CurrentNotification?.Type ?? StatusNotificationType.Info);
        
        public string NotificationForeground => "White";

        public ICommand DismissCommand { get; }

        private void OnNotificationReceived(object? sender, StatusNotification? notification)
        {
            if (notification == null)
            {
                // Auto-dismiss or manual dismiss
                IsVisible = false;
                CurrentNotification = null;
            }
            else
            {
                CurrentNotification = notification;
                IsVisible = true;
            }
        }

        private void Dismiss()
        {
            _notificationService.Dismiss();
        }

        private string GetIconForType(StatusNotificationType type)
        {
            return type switch
            {
                StatusNotificationType.Success => "?",  // Check mark emoji
                StatusNotificationType.Info => "??",    // Light bulb emoji
                StatusNotificationType.Warning => "??",  // Warning emoji
                StatusNotificationType.Error => "?",   // Cross mark emoji
                _ => "??"
            };
        }

        private string GetBackgroundForType(StatusNotificationType type)
        {
            return type switch
            {
                StatusNotificationType.Success => "#4CAF50", // Green
                StatusNotificationType.Info => "#2196F3",    // Blue
                StatusNotificationType.Warning => "#FF9800", // Orange
                StatusNotificationType.Error => "#F44336",   // Red
                _ => "#2196F3"
            };
        }
    }
}
