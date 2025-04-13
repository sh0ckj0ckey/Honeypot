using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.Storage;

namespace Honeypot.Core
{
    public class SettingsService : ObservableObject
    {
        private const string SETTING_NAME_APPEARANCE_INDEX = "AppearanceIndex";
        private const string SETTING_NAME_BACKDROP_INDEX = "BackdropIndex";
        private const string SETTING_NAME_ENABLE_LOCK = "EnableLock";

        private const string SETTING_NAME_ORDER_MODE = "PasswordsOrderMode";

        private const string SETTING_NAME_HIDE_PASSWORD = "HidePasswordAtEnter";

        private const string SETTING_NAME_NO_RANDOM_TIP_AT_ADDING = "NoRandomTipAtAdding";
        private const string SETTING_NAME_NO_RANDOM_TIP_AT_RANDOM = "NoRandomTipAtRandom";


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
                        if (_localSettings.Values[SETTING_NAME_APPEARANCE_INDEX] == null)
                        {
                            _appearanceIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_APPEARANCE_INDEX]?.ToString() == "0")
                        {
                            _appearanceIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_APPEARANCE_INDEX]?.ToString() == "1")
                        {
                            _appearanceIndex = 1;
                        }
                        else if (_localSettings.Values[SETTING_NAME_APPEARANCE_INDEX]?.ToString() == "2")
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
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_APPEARANCE_INDEX] = _appearanceIndex;
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
                        if (_localSettings.Values[SETTING_NAME_BACKDROP_INDEX] == null)
                        {
                            _backdropIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_BACKDROP_INDEX]?.ToString() == "0")
                        {
                            _backdropIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_BACKDROP_INDEX]?.ToString() == "1")
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
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_BACKDROP_INDEX] = _backdropIndex;
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
                        if (_localSettings.Values[SETTING_NAME_ENABLE_LOCK] == null)
                        {
                            _enableLock = false;
                        }
                        else if (_localSettings.Values[SETTING_NAME_ENABLE_LOCK]?.ToString() == "True")
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
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_ENABLE_LOCK] = _enableLock;
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
                        if (_localSettings.Values[SETTING_NAME_ORDER_MODE] == null)
                        {
                            _passwordsOrderMode = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_ORDER_MODE]?.ToString() == "0")
                        {
                            _passwordsOrderMode = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_ORDER_MODE]?.ToString() == "1")
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
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_ORDER_MODE] = _passwordsOrderMode;
            }
        }

        // 是否在详情页默认隐藏密码
        private bool? _hidePassword = null;
        public bool HidePassword
        {
            get
            {
                try
                {
                    if (_hidePassword is null)
                    {
                        if (_localSettings.Values[SETTING_NAME_HIDE_PASSWORD] == null)
                        {
                            _hidePassword = false;
                        }
                        else if (_localSettings.Values[SETTING_NAME_HIDE_PASSWORD]?.ToString() == "True")
                        {
                            _hidePassword = true;
                        }
                        else
                        {
                            _hidePassword = false;
                        }
                    }
                }
                catch { }
                _hidePassword ??= false;
                return _hidePassword == true;
            }
            set
            {
                SetProperty(ref _hidePassword, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_HIDE_PASSWORD] = _hidePassword;
            }
        }

        // 是否隐藏在创建密码页面点击随机密码时，已复制的提示
        private bool? _noTipAtAdding = null;
        public bool NoTipAtAdding
        {
            get
            {
                try
                {
                    if (_noTipAtAdding is null)
                    {
                        if (_localSettings.Values[SETTING_NAME_NO_RANDOM_TIP_AT_ADDING] == null)
                        {
                            _noTipAtAdding = false;
                        }
                        else if (_localSettings.Values[SETTING_NAME_NO_RANDOM_TIP_AT_ADDING]?.ToString() == "True")
                        {
                            _noTipAtAdding = true;
                        }
                        else
                        {
                            _noTipAtAdding = false;
                        }
                    }
                }
                catch { }
                _noTipAtAdding ??= false;
                return _noTipAtAdding == true;
            }
            set
            {
                SetProperty(ref _noTipAtAdding, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_NO_RANDOM_TIP_AT_ADDING] = _noTipAtAdding;
            }
        }

        // 是否隐藏在随机生成页面点击随机密码时，已复制的提示
        private bool? _noTipAtRandom = null;
        public bool NoTipAtRandom
        {
            get
            {
                try
                {
                    if (_noTipAtRandom is null)
                    {
                        if (_localSettings.Values[SETTING_NAME_NO_RANDOM_TIP_AT_RANDOM] == null)
                        {
                            _noTipAtRandom = false;
                        }
                        else if (_localSettings.Values[SETTING_NAME_NO_RANDOM_TIP_AT_RANDOM]?.ToString() == "True")
                        {
                            _noTipAtRandom = true;
                        }
                        else
                        {
                            _noTipAtRandom = false;
                        }
                    }
                }
                catch { }
                _noTipAtRandom ??= false;
                return _noTipAtRandom == true;
            }
            set
            {
                SetProperty(ref _noTipAtRandom, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_NO_RANDOM_TIP_AT_RANDOM] = _noTipAtRandom;
            }
        }
    }
}
