using System;
using System.IO;
using System.Text.Json;
using System.Windows.Input;

namespace LineHelper.Models
{
    public class UserSettings
    {
        public int MarkerRange { get; set; } = 50;
        public string LineInHotkey { get; set; } = "F10";
        public string LineOutHotkey { get; set; } = "F11";
        public ModifierKeys LineInModifiers { get; set; } = ModifierKeys.None;
        public ModifierKeys LineOutModifiers { get; set; } = ModifierKeys.None;
        public bool AutoResetAtLimit { get; set; } = false;
        public bool UseGlobalHotkeys { get; set; } = false;
        public string Theme { get; set; } = "Light";
        public double WindowLeft { get; set; } = 100;
        public double WindowTop { get; set; } = 100;

        private static string SettingsPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "LineHelper",
            "settings.json"
        );

        public static UserSettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    var json = File.ReadAllText(SettingsPath);
                    return JsonSerializer.Deserialize<UserSettings>(json) ?? new UserSettings();
                }
            }
            catch (Exception)
            {
                // If there's any error loading settings, return defaults
            }
            return new UserSettings();
        }

        public void Save()
        {
            try
            {
                var directory = Path.GetDirectoryName(SettingsPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                var json = JsonSerializer.Serialize(this, options);
                File.WriteAllText(SettingsPath, json);
            }
            catch (Exception)
            {
                // Silently fail if we can't save settings
            }
        }

        public string GetLineInHotkeyDisplay()
        {
            var modifiers = GetModifierString(LineInModifiers);
            return string.IsNullOrEmpty(modifiers) ? LineInHotkey : $"{modifiers}+{LineInHotkey}";
        }

        public string GetLineOutHotkeyDisplay()
        {
            var modifiers = GetModifierString(LineOutModifiers);
            return string.IsNullOrEmpty(modifiers) ? LineOutHotkey : $"{modifiers}+{LineOutHotkey}";
        }

        private string GetModifierString(ModifierKeys modifiers)
        {
            if (modifiers == ModifierKeys.None) return "";

            var parts = new System.Collections.Generic.List<string>();
            if ((modifiers & ModifierKeys.Control) != 0) parts.Add("Ctrl");
            if ((modifiers & ModifierKeys.Alt) != 0) parts.Add("Alt");
            if ((modifiers & ModifierKeys.Shift) != 0) parts.Add("Shift");
            if ((modifiers & ModifierKeys.Windows) != 0) parts.Add("Win");

            return string.Join("+", parts);
        }
    }
}