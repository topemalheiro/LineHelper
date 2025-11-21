using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace LineHelper.Services
{
    public class GlobalHotkeyService
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int HOTKEY_ID_LINE_IN = 9000;
        private const int HOTKEY_ID_LINE_OUT = 9001;
        private const int WM_HOTKEY = 0x0312;

        private IntPtr _windowHandle;
        private HwndSource? _source;
        private Action? _lineInAction;
        private Action? _lineOutAction;

        public void RegisterHotkeys(Window window, string lineInKey, ModifierKeys lineInMods, string lineOutKey, ModifierKeys lineOutMods, Action lineInAction, Action lineOutAction)
        {
            var helper = new WindowInteropHelper(window);
            _windowHandle = helper.Handle;
            _lineInAction = lineInAction;
            _lineOutAction = lineOutAction;

            _source = HwndSource.FromHwnd(_windowHandle);
            _source?.AddHook(HwndHook);

            // Convert WPF ModifierKeys to Win32 modifiers
            uint lineInModifiers = ConvertModifiers(lineInMods);
            uint lineOutModifiers = ConvertModifiers(lineOutMods);

            // Convert key strings to virtual key codes
            uint lineInVk = GetVirtualKeyCode(lineInKey);
            uint lineOutVk = GetVirtualKeyCode(lineOutKey);

            // Register the hotkeys
            if (lineInVk != 0)
            {
                RegisterHotKey(_windowHandle, HOTKEY_ID_LINE_IN, lineInModifiers, lineInVk);
            }

            if (lineOutVk != 0)
            {
                RegisterHotKey(_windowHandle, HOTKEY_ID_LINE_OUT, lineOutModifiers, lineOutVk);
            }
        }

        public void UnregisterHotkeys()
        {
            if (_windowHandle != IntPtr.Zero)
            {
                UnregisterHotKey(_windowHandle, HOTKEY_ID_LINE_IN);
                UnregisterHotKey(_windowHandle, HOTKEY_ID_LINE_OUT);
            }

            if (_source != null)
            {
                _source.RemoveHook(HwndHook);
                _source = null;
            }
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();

                if (id == HOTKEY_ID_LINE_IN)
                {
                    _lineInAction?.Invoke();
                    handled = true;
                }
                else if (id == HOTKEY_ID_LINE_OUT)
                {
                    _lineOutAction?.Invoke();
                    handled = true;
                }
            }

            return IntPtr.Zero;
        }

        private uint ConvertModifiers(ModifierKeys modifiers)
        {
            uint mod = 0;
            if ((modifiers & ModifierKeys.Alt) != 0) mod |= 0x0001; // MOD_ALT
            if ((modifiers & ModifierKeys.Control) != 0) mod |= 0x0002; // MOD_CONTROL
            if ((modifiers & ModifierKeys.Shift) != 0) mod |= 0x0004; // MOD_SHIFT
            if ((modifiers & ModifierKeys.Windows) != 0) mod |= 0x0008; // MOD_WIN
            return mod;
        }

        private uint GetVirtualKeyCode(string keyString)
        {
            if (Enum.TryParse<Key>(keyString, out Key key))
            {
                return (uint)KeyInterop.VirtualKeyFromKey(key);
            }
            return 0;
        }
    }
}