using System;
using System.Threading;
using System.Windows;
using ProductPriceCalculator.Models;

namespace ProductPriceCalculator.Services
{
    /// <summary>
    /// Interface for status notification service
    /// </summary>
    public interface IStatusNotificationService
    {
        event EventHandler<StatusNotification?> NotificationChanged;
        void ShowSuccess(string message, TimeSpan? duration = null);
        void ShowInfo(string message, TimeSpan? duration = null);
        void ShowWarning(string message, TimeSpan? duration = null);
        void ShowError(string message, TimeSpan? duration = null);
        void Dismiss();
    }

    /// <summary>
    /// Service for managing status bar notifications
    /// Uses event-based approach to avoid external dependencies
    /// </summary>
    public class StatusNotificationService : IStatusNotificationService
    {
        private Timer? _autoDismissTimer;
        private readonly object _lock = new object();

        public event EventHandler<StatusNotification?>? NotificationChanged;

        public void ShowSuccess(string message, TimeSpan? duration = null)
        {
            ShowNotification(message, StatusNotificationType.Success, duration);
        }

        public void ShowInfo(string message, TimeSpan? duration = null)
        {
            ShowNotification(message, StatusNotificationType.Info, duration);
        }

        public void ShowWarning(string message, TimeSpan? duration = null)
        {
            ShowNotification(message, StatusNotificationType.Warning, duration);
        }

        public void ShowError(string message, TimeSpan? duration = null)
        {
            ShowNotification(message, StatusNotificationType.Error, duration ?? TimeSpan.FromSeconds(5));
        }

        public void Dismiss()
        {
            lock (_lock)
            {
                _autoDismissTimer?.Dispose();
                _autoDismissTimer = null;
            }

            // Dispatch to UI thread
            Application.Current?.Dispatcher.Invoke(() =>
            {
                NotificationChanged?.Invoke(this, null);
            });
        }

        private void ShowNotification(string message, StatusNotificationType type, TimeSpan? duration)
        {
            lock (_lock)
            {
                // Cancel existing auto-dismiss timer
                _autoDismissTimer?.Dispose();

                var notification = new StatusNotification(message, type, duration);

                // Dispatch to UI thread
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    NotificationChanged?.Invoke(this, notification);
                });

                // Set up auto-dismiss timer
                _autoDismissTimer = new Timer(
                    _ => Dismiss(),
                    null,
                    notification.Duration,
                    Timeout.InfiniteTimeSpan
                );
            }
        }
    }
}
