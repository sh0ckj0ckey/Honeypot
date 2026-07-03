using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.Storage;

namespace Honeypot.Services;

public partial class SettingsService : ObservableObject
{
    private const string SettingsAppearance = "AppearanceIndex";
    private const string SettingsBackdrop = "BackdropIndex";
    private const string SettingsLockWithWindowsHello = "EnableLock";
    private const string SettingsPasswordsSortMode = "PasswordsOrderMode";
    private const string SettingsHidePasswordOnDetailPage = "HidePasswordAtEnter";
    private const string SettingsDoNotShowRandomPasswordTipAgainOnAddingPage = "NoRandomTipAtAdding";
    private const string SettingsDoNotShowRandomPasswordTipAgainOnGeneratorPage = "NoRandomTipAtRandom";

    private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

    private int _appearanceIndex = -1;

    private int _backdropIndex = -1;

    private bool? _lockWithWindowsHello = null;

    private int _passwordsSortMode = -1;

    private bool? _hidePasswordOnDetailPage = null;

    private bool? _doNotShowRandomPasswordTipAgainOnAddingPage = null;

    private bool? _doNotShowRandomPasswordTipAgainOnGeneratorPage = null;

    /// <summary>
    /// Occurs when the appearance setting changes.
    /// </summary>
    public event EventHandler<int>? AppearanceSettingChanged;

    /// <summary>
    /// Occurs when the backdrop setting changes.
    /// </summary>
    public event EventHandler<int>? BackdropSettingChanged;

    /// <summary>
    /// Gets or sets the app theme. 0 = System, 1 = Dark, 2 = Light.
    /// </summary>
    public int AppearanceIndex
    {
        get
        {
            if (!IsAllowedValue(_appearanceIndex, 0, 1, 2))
            {
                _appearanceIndex = ReadIntSetting(SettingsAppearance, 0, 0, 1, 2);
            }

            return _appearanceIndex;
        }
        set
        {
            SetProperty(ref _appearanceIndex, value);
            _localSettings.Values[SettingsAppearance] = _appearanceIndex;
            AppearanceSettingChanged?.Invoke(this, this.AppearanceIndex);
        }
    }

    /// <summary>
    /// Gets or sets the app backdrop material. 0 = Mica, 1 = MicaAlt, 2 = Acrylic.
    /// </summary>
    public int BackdropIndex
    {
        get
        {
            if (!IsAllowedValue(_backdropIndex, 0, 1, 2))
            {
                _backdropIndex = ReadIntSetting(SettingsBackdrop, 0, 0, 1, 2);
            }

            return _backdropIndex;
        }
        set
        {
            SetProperty(ref _backdropIndex, value);
            _localSettings.Values[SettingsBackdrop] = _backdropIndex;
            BackdropSettingChanged?.Invoke(this, this.BackdropIndex);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the app is locked with Windows Hello on startup.
    /// </summary>
    public bool LockWithWindowsHello
    {
        get
        {
            _lockWithWindowsHello ??= ReadBoolSetting(SettingsLockWithWindowsHello, false);
            return _lockWithWindowsHello.Value;
        }
        set
        {
            SetProperty(ref _lockWithWindowsHello, value);
            _localSettings.Values[SettingsLockWithWindowsHello] = value;
        }
    }

    /// <summary>
    /// Gets or sets the password list sort mode. 0 = First letter, 1 = Time.
    /// </summary>
    public int PasswordsSortMode
    {
        get
        {
            if (!IsAllowedValue(_passwordsSortMode, 0, 1))
            {
                _passwordsSortMode = ReadIntSetting(SettingsPasswordsSortMode, 0, 0, 1);
            }

            return _passwordsSortMode;
        }
        set
        {
            SetProperty(ref _passwordsSortMode, value);
            _localSettings.Values[SettingsPasswordsSortMode] = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the password is hidden by default on the detail page.
    /// </summary>
    public bool HidePasswordOnDetailPage
    {
        get
        {
            _hidePasswordOnDetailPage ??= ReadBoolSetting(SettingsHidePasswordOnDetailPage, false);
            return _hidePasswordOnDetailPage.Value;
        }
        set
        {
            SetProperty(ref _hidePasswordOnDetailPage, value);
            _localSettings.Values[SettingsHidePasswordOnDetailPage] = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the random password tip is hidden on the add password page.
    /// </summary>
    public bool DoNotShowRandomPasswordTipAgainOnAddingPage
    {
        get
        {
            _doNotShowRandomPasswordTipAgainOnAddingPage ??= ReadBoolSetting(SettingsDoNotShowRandomPasswordTipAgainOnAddingPage, false);
            return _doNotShowRandomPasswordTipAgainOnAddingPage.Value;
        }
        set
        {
            SetProperty(ref _doNotShowRandomPasswordTipAgainOnAddingPage, value);
            _localSettings.Values[SettingsDoNotShowRandomPasswordTipAgainOnAddingPage] = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the random password tip is hidden on the password generator page.
    /// </summary>
    public bool DoNotShowRandomPasswordTipAgainOnGeneratorPage
    {
        get
        {
            _doNotShowRandomPasswordTipAgainOnGeneratorPage ??= ReadBoolSetting(SettingsDoNotShowRandomPasswordTipAgainOnGeneratorPage, false);
            return _doNotShowRandomPasswordTipAgainOnGeneratorPage.Value;
        }
        set
        {
            SetProperty(ref _doNotShowRandomPasswordTipAgainOnGeneratorPage, value);
            _localSettings.Values[SettingsDoNotShowRandomPasswordTipAgainOnGeneratorPage] = value;
        }
    }

    /// <summary>
    /// Reads an integer setting and returns the default value if the stored value is missing or invalid.
    /// </summary>
    /// <param name="key">The local settings key.</param>
    /// <param name="defaultValue">The default value to use when the stored value is missing or invalid.</param>
    /// <param name="allowedValues">The allowed values for this setting.</param>
    /// <returns>The stored integer value if it is valid; otherwise, the default value.</returns>
    private int ReadIntSetting(string key, int defaultValue, params int[] allowedValues)
    {
        try
        {
            object? value = _localSettings.Values[key];

            if (value is null)
            {
                return defaultValue;
            }

            if (!int.TryParse(value.ToString(), out int parsedValue))
            {
                return defaultValue;
            }

            return IsAllowedValue(parsedValue, allowedValues) ? parsedValue : defaultValue;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine(ex);
            return defaultValue;
        }
    }

    /// <summary>
    /// Reads a boolean setting and returns the default value if the stored value is missing or invalid.
    /// </summary>
    /// <param name="key">The local settings key.</param>
    /// <param name="defaultValue">The default value to use when the stored value is missing or invalid.</param>
    /// <returns>The stored boolean value if it is valid; otherwise, the default value.</returns>
    private bool ReadBoolSetting(string key, bool defaultValue)
    {
        try
        {
            object? value = _localSettings.Values[key];

            if (value is null)
            {
                return defaultValue;
            }

            if (!bool.TryParse(value.ToString(), out bool parsedValue))
            {
                return defaultValue;
            }

            return parsedValue;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine(ex);
            return defaultValue;
        }
    }

    /// <summary>
    /// Determines whether an integer value is contained in the allowed values list.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="allowedValues">The allowed values.</param>
    /// <returns><see langword="true"/> if the value is allowed; otherwise, <see langword="false"/>.</returns>
    private static bool IsAllowedValue(int value, params int[] allowedValues)
    {
        foreach (int allowedValue in allowedValues)
        {
            if (value == allowedValue)
            {
                return true;
            }
        }

        return false;
    }
}
