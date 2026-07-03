using Windows.Storage;

namespace Honeypot.Services;

/// <summary>
/// Represents the placement state of a window.
/// </summary>
/// <param name="X">The left position of the window.</param>
/// <param name="Y">The top position of the window.</param>
/// <param name="Width">The width of the window.</param>
/// <param name="Height">The height of the window.</param>
/// <param name="IsMaximized">Whether the window is maximized.</param>
public readonly record struct WindowPlacement(double X, double Y, double Width, double Height, bool IsMaximized);

/// <summary>
/// Provides persistence operations for the main window placement.
/// </summary>
public class WindowPlacementService
{
    private const string KeyX = "MainWindow_X";
    private const string KeyY = "MainWindow_Y";
    private const string KeyWidth = "MainWindow_Width";
    private const string KeyHeight = "MainWindow_Height";
    private const string KeyIsMaximized = "MainWindow_IsMaximized";

    private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

    /// <summary>
    /// Tries to load the saved main window placement.
    /// </summary>
    /// <param name="placement">When this method returns, contains the saved window placement if loading succeeds.</param>
    /// <returns>
    /// <see langword="true"/> if a valid saved placement was loaded; otherwise, <see langword="false"/>.
    /// </returns>
    public bool TryLoad(out WindowPlacement placement)
    {
        if (_localSettings.Values[KeyX] is double x &&
            _localSettings.Values[KeyY] is double y &&
            _localSettings.Values[KeyWidth] is double width &&
            _localSettings.Values[KeyHeight] is double height &&
            _localSettings.Values[KeyIsMaximized] is bool isMaximized &&
            double.IsFinite(x) &&
            double.IsFinite(y) &&
            double.IsFinite(width) &&
            double.IsFinite(height) &&
            width > 0 &&
            height > 0)
        {
            placement = new WindowPlacement(x, y, width, height, isMaximized);
            return true;
        }

        placement = default;
        return false;
    }
    
    /// <summary>
    /// Saves the main window placement.
    /// </summary>
    /// <param name="placement">The window placement to save.</param>
    public void Save(WindowPlacement placement)
    {
        _localSettings.Values[KeyX] = placement.X;
        _localSettings.Values[KeyY] = placement.Y;
        _localSettings.Values[KeyWidth] = placement.Width;
        _localSettings.Values[KeyHeight] = placement.Height;
        _localSettings.Values[KeyIsMaximized] = placement.IsMaximized;
    }
}
