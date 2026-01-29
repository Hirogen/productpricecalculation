using System.Windows;

namespace ProductPriceCalculator.Services
{
    /// <summary>
    /// Localized MessageBox helper
    /// </summary>
    public static class LocalizedMessageBox
    {
        public static MessageBoxResult ShowConfirmation(string message, string title = null)
        {
            return MessageBox.Show(
                message,
                title ?? Localization.Get("MsgConfirmation"),
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
        }

        public static void ShowInformation(string message, string title = null)
        {
            MessageBox.Show(
                message,
                title ?? Localization.Get("MsgConfirmation"),
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        public static void ShowWarning(string message, string title = null)
        {
            MessageBox.Show(
                message,
                title ?? Localization.Get("MsgInvalidTitle"),
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        public static void ShowError(string message, string title = null)
        {
            MessageBox.Show(
                message,
                title ?? Localization.Get("MsgCalcErrorTitle"),
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
