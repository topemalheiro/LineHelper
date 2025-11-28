# LineHelper - Marker Tracking Tool

Here is a small video explaining it: https://www.youtube.com/watch?v=2GdzmFteyRk 

A professional Windows desktop application designed for audio engineers to efficiently track and manage markers during editing sessions. Built with C# and WPF for optimal touch screen support and clipboard integration.

## Features

### Core Functionality
- **One-Click Operation**: Press button → copy marker to clipboard → auto-increment
- **Dual Marker Types**: Track both LINE IN and LINE OUT markers independently
- **Smart Formatting**: Automatic 4-digit formatting (line0001, lout0001)
- **Touch Optimized**: Large buttons designed for touch screen use
- **Clipboard Integration**: Instant copy to clipboard for quick pasting

### Advanced Features
- **Customizable Marker Range**: Set your session size (default: 50 markers)
- **Keyboard Shortcuts**: Fully customizable hotkeys for hands-free operation
- **Manual Adjustment**: Decrement buttons for correcting mistakes
- **Progress Tracking**: Visual progress bar showing current position
- **Auto-Reset Option**: Automatically reset when reaching marker limit
- **Settings Persistence**: All preferences saved between sessions

## Requirements

- Windows 10 or later
- .NET 6.0 Runtime (installer will prompt if not installed)
- Touch screen recommended but not required

## Installation

### Method 1: Build from Source

1. **Prerequisites**:
   - Install Visual Studio 2022 (Community Edition or higher)
   - Install .NET 8.0 SDK
   - Ensure "Desktop development with .NET" workload is installed

2. **Build Steps**:
   ```bash
   # Clone or download the project
   cd to where you want

   # Build using dotnet CLI
   dotnet build --configuration Release

   # Or open LineHelper.sln in Visual Studio and build
   ```

3. **Run the Application**:
   ```bash
   dotnet run --project LineHelper\LineHelper.csproj
   ```

### Method 2: Publish as Single Executable

```bash
# Create a self-contained single file executable
dotnet publish LineHelper\LineHelper.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish

# The executable will be in the publish folder
```

## Usage

### Basic Operation

1. **Launch the Application**: Double-click LineHelper.exe
2. **Click LINE IN Button**: Copies "line0001" to clipboard and increments to line0002
3. **Click LINE OUT Button**: Copies "lout0001" to clipboard and increments to lout0002
4. **Paste in Your DAW**: Use Ctrl+V to paste the marker text

### Keyboard Shortcuts

Default shortcuts (customizable in settings):
- **Space**: Copy and increment LINE IN marker
- **Enter**: Copy and increment LINE OUT marker

### Settings Configuration

Click the gear icon to access settings:
- **Marker Range**: Set maximum number of markers (1-9999)
- **Keyboard Shortcuts**: Click "Change..." to set custom hotkeys
- **Auto-Reset**: Enable to automatically reset at marker limit
- **Theme**: Choose between Light and Dark themes

### Tips for Efficient Use

1. **Touch Screen Setup**: Position the window where easily reachable on your touch monitor
2. **Keyboard Workflow**: Use keyboard shortcuts to keep hands on your control surface
3. **Manual Corrections**: Use the small arrow buttons to decrement if you skip a marker
4. **Session Reset**: Click Reset button to start a new session (confirms before clearing)

## File Locations

- **Settings**: `%APPDATA%\LineHelper\settings.json`
- **Application**: Wherever you place the executable

## Troubleshooting

### Application Won't Start
- Ensure .NET .0 Runtime is installed
- Check Windows Event Viewer for error details
- Run as Administrator if clipboard issues occur

### Keyboard Shortcuts Not Working
- Ensure the application window has focus
- Check that shortcuts aren't conflicting with system hotkeys
- Try different key combinations in settings

### Clipboard Not Working
- Some DAWs may need a slight delay - try clicking then waiting a moment
- Ensure no clipboard managers are interfering
- Restart the application if clipboard stops responding

## Development

### Project Structure
```
LineHelper/
├── Models/          # Data models (MarkerSession, UserSettings)
├── ViewModels/      # MVVM ViewModels
├── Views/           # Windows and UI components
├── Resources/       # Styles and resources
└── MainWindow.xaml  # Main application window
```

### Technologies Used
- **Framework**: .NET 6.0
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Pattern**: MVVM (Model-View-ViewModel)
- **Language**: C# 10.0

## License

This tool was created for the Mochi Project audio production workflow.

## Support

For issues or feature requests, please contact the development team.
