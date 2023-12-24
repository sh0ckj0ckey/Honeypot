using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.Storage;

namespace Honeypot.Core
{
    public class SettingsService : ObservableObject
    {
        private const string SETTING_NAME_APPEARANCEINDEX = "AppearanceIndex";
        private const string SETTING_NAME_BACKDROPINDEX = "BackdropIndex";
        private const string SETTING_NAME_ENABLELOCK = "EnableLock";
        private const string SETTING_NAME_ORDERMODE = "PasswordsOrderMode";


        private ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        public Action<int> OnAppearanceSettingChanged { get; set; } = null;
        public Action<int> OnBackdropSettingChanged { get; set; } = null;

        // 设置的应用程序的主题 0-System 1-Dark 2-Light
        private int _appearanceIndex = -1;
        public int AppearanceIndex
        {
            get
            {
                try
                {
                    if (_appearanceIndex < 0)
                    {
                        if (_localSettings.Values[SETTING_NAME_APPEARANCEINDEX] == null)
                        {
                            _appearanceIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_APPEARANCEINDEX]?.ToString() == "0")
                        {
                            _appearanceIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_APPEARANCEINDEX]?.ToString() == "1")
                        {
                            _appearanceIndex = 1;
                        }
                        else if (_localSettings.Values[SETTING_NAME_APPEARANCEINDEX]?.ToString() == "2")
                        {
                            _appearanceIndex = 2;
                        }
                        else
                        {
                            _appearanceIndex = 0;
                        }
                    }
                }
                catch { }
                if (_appearanceIndex < 0) _appearanceIndex = 0;
                return _appearanceIndex < 0 ? 0 : _appearanceIndex;
            }
            set
            {
                SetProperty(ref _appearanceIndex, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_APPEARANCEINDEX] = _appearanceIndex;
                OnAppearanceSettingChanged?.Invoke(_appearanceIndex);
            }
        }

        // 设置的应用程序的背景材质 0-Mica 1-Acrylic
        private int _backdropIndex = -1;
        public int BackdropIndex
        {
            get
            {
                try
                {
                    if (_backdropIndex < 0)
                    {
                        if (_localSettings.Values[SETTING_NAME_BACKDROPINDEX] == null)
                        {
                            _backdropIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_BACKDROPINDEX]?.ToString() == "0")
                        {
                            _backdropIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_BACKDROPINDEX]?.ToString() == "1")
                        {
                            _backdropIndex = 1;
                        }
                        else
                        {
                            _backdropIndex = 0;
                        }
                    }
                }
                catch { }
                if (_backdropIndex < 0) _backdropIndex = 0;
                return _backdropIndex < 0 ? 0 : _backdropIndex;
            }
            set
            {
                SetProperty(ref _backdropIndex, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_BACKDROPINDEX] = _backdropIndex;
                OnBackdropSettingChanged?.Invoke(_backdropIndex);
            }
        }

        // 是否使用Windows Hello锁定
        private bool? _enableLock = null;
        public bool EnableLock
        {
            get
            {
                try
                {
                    if (_enableLock is null)
                    {
                        if (_localSettings.Values[SETTING_NAME_ENABLELOCK] == null)
                        {
                            _enableLock = false;
                        }
                        else if (_localSettings.Values[SETTING_NAME_ENABLELOCK]?.ToString() == "True")
                        {
                            _enableLock = true;
                        }
                        else
                        {
                            _enableLock = false;
                        }
                    }
                }
                catch { }
                _enableLock ??= false;
                return _enableLock == true;
            }
            set
            {
                SetProperty(ref _enableLock, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_ENABLELOCK] = _enableLock;
            }
        }

        // 密码列表的排序方式 0-首字母 1-时间
        private int _passwordsOrderMode = -1;
        public int PasswordsOrderMode
        {
            get
            {
                try
                {
                    if (_passwordsOrderMode < 0)
                    {
                        if (_localSettings.Values[SETTING_NAME_ORDERMODE] == null)
                        {
                            _passwordsOrderMode = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_ORDERMODE]?.ToString() == "0")
                        {
                            _passwordsOrderMode = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_ORDERMODE]?.ToString() == "1")
                        {
                            _passwordsOrderMode = 1;
                        }
                        else
                        {
                            _passwordsOrderMode = 0;
                        }
                    }
                }
                catch { }
                if (_passwordsOrderMode < 0) _passwordsOrderMode = 0;
                return _passwordsOrderMode < 0 ? 0 : _passwordsOrderMode;
            }
            set
            {
                SetProperty(ref _passwordsOrderMode, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_ORDERMODE] = _passwordsOrderMode;
            }
        }
    }
}
