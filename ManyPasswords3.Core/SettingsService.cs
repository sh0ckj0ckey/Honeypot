using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.Storage;

namespace ManyPasswords3.Core
{
    public class SettingsService : ObservableObject
    {
        private const string SETTING_NAME_APPEARANCEINDEX = "AppearanceIndex";
        private const string SETTING_NAME_BACKDROPINDEX = "BackdropIndex";
        private const string SETTING_NAME_ENABLELOCK = "EnableLock";

        private const string SETTING_NAME_ENABLEGLOSSARY = "EnableGlossary";
        private const string SETTING_NAME_AUTOCLEARLASTINPUT = "AutoClearLastInput";
        private const string SETTING_NAME_CLOSEBUTTONMODE = "CloseButtonMode";
        private const string SETTING_NAME_SEARCHBOXSTYLE = "SearchBoxStyle";

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

        // 是否启用生词本
        private bool? _enableGlossary = null;
        public bool EnableGlossary
        {
            get
            {
                try
                {
                    if (_enableGlossary is null)
                    {
                        if (_localSettings.Values[SETTING_NAME_ENABLEGLOSSARY] == null)
                        {
                            _enableGlossary = true;
                        }
                        else if (_localSettings.Values[SETTING_NAME_ENABLEGLOSSARY]?.ToString() == "False")
                        {
                            _enableGlossary = false;
                        }
                        else
                        {
                            _enableGlossary = true;
                        }
                    }
                }
                catch { }
                if (_enableGlossary is null) _enableGlossary = true;
                return _enableGlossary != false;
            }
            set
            {
                SetProperty(ref _enableGlossary, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_ENABLEGLOSSARY] = _enableGlossary;
            }
        }

        // 是否自动清除上次输入
        private bool? _autoClearLastInput = null;
        public bool AutoClearLastInput
        {
            get
            {
                try
                {
                    if (_autoClearLastInput is null)
                    {
                        if (_localSettings.Values[SETTING_NAME_AUTOCLEARLASTINPUT] == null)
                        {
                            _autoClearLastInput = false;
                        }
                        else if (_localSettings.Values[SETTING_NAME_AUTOCLEARLASTINPUT]?.ToString() == "True")
                        {
                            _autoClearLastInput = true;
                        }
                        else
                        {
                            _autoClearLastInput = false;
                        }
                    }
                }
                catch { }
                if (_autoClearLastInput is null) _autoClearLastInput = false;
                return _autoClearLastInput == true;
            }
            set
            {
                SetProperty(ref _autoClearLastInput, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_AUTOCLEARLASTINPUT] = _autoClearLastInput;
            }
        }

        // 设置关闭按钮的作用 0-隐藏 1-退出
        private int _closeButtonMode = -1;
        public int CloseButtonMode
        {
            get
            {
                try
                {
                    if (_closeButtonMode < 0)
                    {
                        if (_localSettings.Values[SETTING_NAME_CLOSEBUTTONMODE] == null)
                        {
                            _closeButtonMode = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_CLOSEBUTTONMODE]?.ToString() == "0")
                        {
                            _closeButtonMode = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_CLOSEBUTTONMODE]?.ToString() == "1")
                        {
                            _closeButtonMode = 1;
                        }
                        else
                        {
                            _closeButtonMode = 0;
                        }
                    }
                }
                catch { }
                if (_closeButtonMode < 0) _closeButtonMode = 0;
                return _closeButtonMode < 0 ? 0 : _closeButtonMode;
            }
            set
            {
                SetProperty(ref _closeButtonMode, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_CLOSEBUTTONMODE] = _closeButtonMode;
            }
        }

        // 搜索框样式
        private int _searchBoxStyle = -1;
        public int SearchBoxStyle
        {
            get
            {
                try
                {
                    if (_searchBoxStyle < 0)
                    {
                        if (_localSettings.Values[SETTING_NAME_SEARCHBOXSTYLE] == null)
                        {
                            _searchBoxStyle = 2;
                        }
                        else if (_localSettings.Values[SETTING_NAME_SEARCHBOXSTYLE]?.ToString() == "0")
                        {
                            _searchBoxStyle = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_SEARCHBOXSTYLE]?.ToString() == "1")
                        {
                            _searchBoxStyle = 1;
                        }
                        else if (_localSettings.Values[SETTING_NAME_SEARCHBOXSTYLE]?.ToString() == "2")
                        {
                            _searchBoxStyle = 2;
                        }
                        else
                        {
                            _searchBoxStyle = 2;
                        }
                    }
                }
                catch { }
                if (_searchBoxStyle < 0) _searchBoxStyle = 2;
                return _searchBoxStyle < 0 ? 2 : _searchBoxStyle;
            }
            set
            {
                SetProperty(ref _searchBoxStyle, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_SEARCHBOXSTYLE] = _searchBoxStyle;
            }
        }

    }
}
