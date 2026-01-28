using System.Windows;

namespace ProductPriceCalculator.Services
{
    /// <summary>
    /// Service for handling UI dialogs and message boxes
    /// </summary>
    public interface IDialogService
    {
        void ShowMessage(string message, string title = null);
        void ShowError(string message, string title = null);
        bool ShowConfirmation(string message, string title = null);
        bool ShowDialog(Window dialog);
    }

    public class DialogService : IDialogService
    {
        public void ShowMessage(string message, string title = null)
        {
            MessageBox.Show(
                message,
                title ?? Localization.Get("AppTitle"),
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        public void ShowError(string message, string title = null)
        {
            MessageBox.Show(
                message,
                title ?? Localization.Get("MsgCalcErrorTitle"),
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        public bool ShowConfirmation(string message, string title = null)
        {
            var result = MessageBox.Show(
                message,
                title ?? Localization.Get("MsgConfirmation"),
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            return result == MessageBoxResult.Yes;
        }

        public bool ShowDialog(Window dialog)
        {
            return dialog.ShowDialog() == true;
        }
    }
}
