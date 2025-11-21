using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LineHelper.Models;

namespace LineHelper.Views
{
    public partial class SettingsWindow : Window
    {
        private UserSettings _settings;
        private bool _isRecordingLineIn = false;
        private bool _isRecordingLineOut = false;

        public UserSettings UpdatedSettings { get; private set; }

        public SettingsWindow(UserSettings currentSettings)
        {
            InitializeComponent();
            _settings = new UserSettings
            {
                MarkerRange = currentSettings.MarkerRange,
                LineInHotkey = currentSettings.LineInHotkey,
                LineOutHotkey = currentSettings.LineOutHotkey,
                LineInModifiers = currentSettings.LineInModifiers,
                LineOutModifiers = currentSettings.LineOutModifiers,
                AutoResetAtLimit = currentSettings.AutoResetAtLimit,
                UseGlobalHotkeys = currentSettings.UseGlobalHotkeys,
                Theme = currentSettings.Theme,
                WindowLeft = currentSettings.WindowLeft,
                WindowTop = currentSettings.WindowTop
            };

            DataContext = _settings;
            UpdatedSettings = _settings;

            // Set ComboBox selection
            ThemeComboBox.SelectedIndex = _settings.Theme == "Dark" ? 1 : 0;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ChangeLineInButton_Click(object sender, RoutedEventArgs e)
        {
            _isRecordingLineIn = true;
            _isRecordingLineOut = false;
            StartKeyRecording(LineInHotkeyTextBox, "Press a key combination for Line In...");
        }

        private void ChangeLineOutButton_Click(object sender, RoutedEventArgs e)
        {
            _isRecordingLineOut = true;
            _isRecordingLineIn = false;
            StartKeyRecording(LineOutHotkeyTextBox, "Press a key combination for Line Out...");
        }

        private void StartKeyRecording(TextBox targetTextBox, string prompt)
        {
            targetTextBox.Text = prompt;
            targetTextBox.Background = new SolidColorBrush(Colors.LightYellow);
            targetTextBox.Focus();

            PreviewKeyDown += RecordKey;
        }

        private void RecordKey(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            if (e.Key == Key.Escape)
            {
                CancelKeyRecording();
                return;
            }

            // Ignore modifier keys alone
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl ||
                e.Key == Key.LeftAlt || e.Key == Key.RightAlt ||
                e.Key == Key.LeftShift || e.Key == Key.RightShift ||
                e.Key == Key.LWin || e.Key == Key.RWin)
            {
                return;
            }

            var modifiers = Keyboard.Modifiers;

            if (_isRecordingLineIn)
            {
                _settings.LineInHotkey = e.Key.ToString();
                _settings.LineInModifiers = modifiers;
                LineInHotkeyTextBox.Text = _settings.GetLineInHotkeyDisplay();
                LineInHotkeyTextBox.Background = new SolidColorBrush(Colors.White);
            }
            else if (_isRecordingLineOut)
            {
                _settings.LineOutHotkey = e.Key.ToString();
                _settings.LineOutModifiers = modifiers;
                LineOutHotkeyTextBox.Text = _settings.GetLineOutHotkeyDisplay();
                LineOutHotkeyTextBox.Background = new SolidColorBrush(Colors.White);
            }

            _isRecordingLineIn = false;
            _isRecordingLineOut = false;
            PreviewKeyDown -= RecordKey;
        }

        private void CancelKeyRecording()
        {
            if (_isRecordingLineIn)
            {
                LineInHotkeyTextBox.Text = _settings.GetLineInHotkeyDisplay();
                LineInHotkeyTextBox.Background = new SolidColorBrush(Colors.White);
            }
            else if (_isRecordingLineOut)
            {
                LineOutHotkeyTextBox.Text = _settings.GetLineOutHotkeyDisplay();
                LineOutHotkeyTextBox.Background = new SolidColorBrush(Colors.White);
            }

            _isRecordingLineIn = false;
            _isRecordingLineOut = false;
            PreviewKeyDown -= RecordKey;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate marker range
            if (!int.TryParse(MarkerRangeTextBox.Text, out int markerRange) ||
                markerRange < 1 || markerRange > 9999)
            {
                MessageBox.Show("Please enter a valid marker range between 1 and 9999.",
                    "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _settings.MarkerRange = markerRange;
            _settings.AutoResetAtLimit = AutoResetCheckBox.IsChecked ?? false;
            _settings.Theme = (ThemeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Light";

            // Save settings to file
            _settings.Save();

            UpdatedSettings = _settings;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}