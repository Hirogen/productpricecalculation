using System.Windows;

namespace ProductPriceCalculator
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Initialize localization on startup
            Localization.InitializeFromSystemCulture();
        }
    }
}
