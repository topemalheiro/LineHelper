using System.Windows;

namespace LineHelper
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set up any global exception handling or initialization here
            Current.DispatcherUnhandledException += (sender, args) =>
            {
                MessageBox.Show($"An unexpected error occurred: {args.Exception.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };
        }
    }
}