using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using LineHelper.Models;

namespace LineHelper.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly MarkerSession _session;
        private readonly UserSettings _settings;
        private string _statusMessage = "Ready";
        private readonly DispatcherTimer _statusTimer;

        public MainViewModel()
        {
            _settings = UserSettings.Load();
            _session = new MarkerSession { MarkerRange = _settings.MarkerRange };
            _statusTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            _statusTimer.Tick += (s, e) =>
            {
                StatusMessage = "Ready";
                _statusTimer.Stop();
            };
        }

        public int MarkerRange => _session.MarkerRange;

        public string CurrentLineDisplay => $"{_session.CurrentLineNumber}/{_session.MarkerRange}";
        public string CurrentLoutDisplay => $"{_session.CurrentLoutNumber}/{_session.MarkerRange}";

        public string LineMarkerPreview => _session.GetLineMarker();
        public string LoutMarkerPreview => _session.GetLoutMarker();

        public string LineInHotkeyDisplay => _settings.GetLineInHotkeyDisplay();
        public string LineOutHotkeyDisplay => _settings.GetLineOutHotkeyDisplay();

        public string RangeDisplay => $"Range: 1-{_session.MarkerRange}";

        public double ProgressValue => Math.Max(_session.CurrentLineNumber, _session.CurrentLoutNumber);

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public void CopyLineMarker()
        {
            try
            {
                var marker = _session.IncrementLine();
                Clipboard.SetText(marker);
                StatusMessage = $"Copied: {marker}";
                _statusTimer.Stop();
                _statusTimer.Start();

                if (_session.IsLineAtLimit() && _settings.AutoResetAtLimit)
                {
                    _session.CurrentLineNumber = 1;
                    StatusMessage = $"Copied: {marker} (Auto-reset)";
                }

                UpdateAllProperties();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        public void CopyLoutMarker()
        {
            try
            {
                var marker = _session.IncrementLout();
                Clipboard.SetText(marker);
                StatusMessage = $"Copied: {marker}";
                _statusTimer.Stop();
                _statusTimer.Start();

                if (_session.IsLoutAtLimit() && _settings.AutoResetAtLimit)
                {
                    _session.CurrentLoutNumber = 1;
                    StatusMessage = $"Copied: {marker} (Auto-reset)";
                }

                UpdateAllProperties();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        public void DecrementLine()
        {
            _session.DecrementLine();
            UpdateAllProperties();
        }

        public void DecrementLout()
        {
            _session.DecrementLout();
            UpdateAllProperties();
        }

        public void Reset()
        {
            _session.Reset();
            StatusMessage = "Session reset";
            _statusTimer.Stop();
            _statusTimer.Start();
            UpdateAllProperties();
        }

        public void UpdateSettings(UserSettings newSettings)
        {
            _session.MarkerRange = newSettings.MarkerRange;

            // Reset if current numbers exceed new range
            if (_session.CurrentLineNumber > newSettings.MarkerRange)
                _session.CurrentLineNumber = 1;
            if (_session.CurrentLoutNumber > newSettings.MarkerRange)
                _session.CurrentLoutNumber = 1;

            UpdateAllProperties();
        }

        private void UpdateAllProperties()
        {
            OnPropertyChanged(nameof(CurrentLineDisplay));
            OnPropertyChanged(nameof(CurrentLoutDisplay));
            OnPropertyChanged(nameof(LineMarkerPreview));
            OnPropertyChanged(nameof(LoutMarkerPreview));
            OnPropertyChanged(nameof(ProgressValue));
            OnPropertyChanged(nameof(MarkerRange));
            OnPropertyChanged(nameof(RangeDisplay));
            OnPropertyChanged(nameof(LineInHotkeyDisplay));
            OnPropertyChanged(nameof(LineOutHotkeyDisplay));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}