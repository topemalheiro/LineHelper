using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using LineHelper.Models;
using LineHelper.ViewModels;
using LineHelper.Views;
using LineHelper.Services;

namespace LineHelper
{
    public partial class MainWindow : Window
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        private MainViewModel _viewModel;
        private UserSettings _settings;
        private Dictionary<Key, DateTime> _lastKeyPress = new();
        private readonly GlobalHotkeyService _hotkeyService = new();

        public MainWindow()
        {
            InitializeComponent();

            _settings = UserSettings.Load();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;

            // Apply theme
            ApplyTheme(_settings.Theme);

            // Restore window position
            if (_settings.WindowLeft > 0 && _settings.WindowTop > 0)
            {
                Left = _settings.WindowLeft;
                Top = _settings.WindowTop;
            }
            else
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            // Set up keyboard shortcuts after window is loaded (so we have a handle)
            Loaded += (s, e) => SetupKeyboardShortcuts();
        }

        private void SetupKeyboardShortcuts()
        {
            // Unregister old hotkeys
            _hotkeyService.UnregisterHotkeys();

            // Register global hotkeys
            _hotkeyService.RegisterHotkeys(
                this,
                _settings.LineInHotkey,
                _settings.LineInModifiers,
                _settings.LineOutHotkey,
                _settings.LineOutModifiers,
                () => Dispatcher.Invoke(() => LineInButton_Click(this, new RoutedEventArgs())),
                () => Dispatcher.Invoke(() => LineOutButton_Click(this, new RoutedEventArgs()))
            );

            // Also keep local keyboard shortcuts for when window has focus
            PreviewKeyDown += MainWindow_PreviewKeyDown;
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Prevent rapid key repeats
            if (_lastKeyPress.ContainsKey(e.Key) &&
                (DateTime.Now - _lastKeyPress[e.Key]).TotalMilliseconds < 100)
            {
                return;
            }
            _lastKeyPress[e.Key] = DateTime.Now;

            // Check for Line In hotkey
            if (IsHotkeyMatch(e, _settings.LineInHotkey, _settings.LineInModifiers))
            {
                LineInButton_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
            // Check for Line Out hotkey
            else if (IsHotkeyMatch(e, _settings.LineOutHotkey, _settings.LineOutModifiers))
            {
                LineOutButton_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
        }

        private bool IsHotkeyMatch(KeyEventArgs e, string hotkeyString, ModifierKeys requiredModifiers)
        {
            // Check modifiers
            var currentModifiers = Keyboard.Modifiers;
            if (currentModifiers != requiredModifiers)
                return false;

            // Parse the key string
            if (Enum.TryParse<Key>(hotkeyString, out Key hotkey))
            {
                return e.Key == hotkey;
            }

            return false;
        }

        private void LineInButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CopyLineMarker();
            AnimateButton(LineInButton);
        }

        private void LineOutButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CopyLoutMarker();
            AnimateButton(LineOutButton);
        }

        private void LineInDecrementButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.DecrementLine();
        }

        private void LineOutDecrementButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.DecrementLout();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to reset the session?",
                "Reset Session",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _viewModel.Reset();
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(_settings);
            settingsWindow.Owner = this;

            if (settingsWindow.ShowDialog() == true)
            {
                _settings = settingsWindow.UpdatedSettings;
                _viewModel.UpdateSettings(_settings);

                // Apply theme
                ApplyTheme(_settings.Theme);

                // Update keyboard shortcuts
                SetupKeyboardShortcuts();
            }
        }

        private void ApplyTheme(string theme)
        {
            var resources = Application.Current.Resources;

            if (theme == "Dark")
            {
                // Apply dark theme
                resources["WindowBackgroundBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)resources["DarkBackgroundColor"]);
                resources["SurfaceBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)resources["DarkSurfaceColor"]);
                resources["TextBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)resources["DarkTextColor"]);
                resources["HeaderTextBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)resources["DarkHeaderTextColor"]);
                resources["SecondaryTextBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)resources["DarkSecondaryTextColor"]);
                resources["StatusTextBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)resources["DarkStatusTextColor"]);
                resources["IconBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)resources["DarkIconColor"]);

                // Set dark title bar
                SetTitleBarTheme(true);
            }
            else
            {
                // Apply light theme
                resources["WindowBackgroundBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)resources["LightBackgroundColor"]);
                resources["SurfaceBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)resources["LightSurfaceColor"]);
                resources["TextBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)resources["LightTextColor"]);
                resources["HeaderTextBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)resources["LightHeaderTextColor"]);
                resources["SecondaryTextBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)resources["LightSecondaryTextColor"]);
                resources["StatusTextBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)resources["LightStatusTextColor"]);
                resources["IconBrush"] = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)resources["LightIconColor"]);

                // Set light title bar
                SetTitleBarTheme(false);
            }
        }

        private void SetTitleBarTheme(bool isDark)
        {
            try
            {
                var windowHelper = new WindowInteropHelper(this);
                IntPtr hwnd = windowHelper.Handle;

                if (hwnd != IntPtr.Zero)
                {
                    int useDarkMode = isDark ? 1 : 0;
                    DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, sizeof(int));
                }
            }
            catch
            {
                // Silently fail on older Windows versions that don't support this
            }
        }

        private void AnimateButton(FrameworkElement button)
        {
            // Create a simple scale animation for visual feedback
            var scaleTransform = new System.Windows.Media.ScaleTransform(1, 1);
            button.RenderTransform = scaleTransform;
            button.RenderTransformOrigin = new Point(0.5, 0.5);

            var scaleXAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0.95,
                Duration = TimeSpan.FromMilliseconds(100),
                AutoReverse = true
            };

            var scaleYAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0.95,
                Duration = TimeSpan.FromMilliseconds(100),
                AutoReverse = true
            };

            scaleTransform.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleXProperty, scaleXAnimation);
            scaleTransform.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleYProperty, scaleYAnimation);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            // Unregister global hotkeys
            _hotkeyService.UnregisterHotkeys();

            // Save window position
            _settings.WindowLeft = Left;
            _settings.WindowTop = Top;
            _settings.Save();
        }
    }
}