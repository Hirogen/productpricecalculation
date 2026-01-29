using System;

namespace ProductPriceCalculator.Models
{
    /// <summary>
    /// Enum defining the types of status notifications
    /// </summary>
    public enum StatusNotificationType
    {
        Success,
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// Model representing a status bar notification
    /// </summary>
    public class StatusNotification
    {
        public string Message { get; set; }
        public StatusNotificationType Type { get; set; }
        public DateTime Timestamp { get; set; }
        public TimeSpan Duration { get; set; }

        public StatusNotification(string message, StatusNotificationType type, TimeSpan? duration = null)
        {
            Message = message;
            Type = type;
            Timestamp = DateTime.Now;
            Duration = duration ?? TimeSpan.FromSeconds(3); // Default 3 seconds
        }
    }
}
