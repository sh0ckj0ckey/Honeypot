﻿using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;

namespace Honeypot.Models
{
    public class PasswordModel : ObservableObject
    {
        public int Id { get; set; } = -1;

        private string _account = "";
        public string Account
        {
            get => _account;
            set => SetProperty(ref _account, value);
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private char _firstLetter = '#';
        public char FirstLetter
        {
            get => _firstLetter;
            set => SetProperty(ref _firstLetter, value);
        }

        private string _name = "";
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _createDate = "";
        public string CreateDate
        {
            get => _createDate;
            set => SetProperty(ref _createDate, value);
        }

        private string _editDate = "";
        public string EditDate
        {
            get => _editDate;
            set => SetProperty(ref _editDate, value);
        }

        private string _website = "";
        public string Website
        {
            get => _website;
            set => SetProperty(ref _website, value);
        }

        private string _note = "";
        public string Note
        {
            get => _note;
            set => SetProperty(ref _note, value);
        }

        private bool _favorite = false;
        public bool Favorite
        {
            get => _favorite;
            set => SetProperty(ref _favorite, value);
        }

        private int _categoryId = -1;
        public int CategoryId
        {
            get => _categoryId;
            set => SetProperty(ref _categoryId, value);
        }

        private int _thirdPartyId = -1;
        public int ThirdPartyId
        {
            get => _thirdPartyId;
            set => SetProperty(ref _thirdPartyId, value);
        }

        public string LogoFileName { get; set; } = string.Empty;

        private BitmapImage _normalLogoImage = null;
        public BitmapImage NormalLogoImage
        {
            get => _normalLogoImage;
            set => SetProperty(ref _normalLogoImage, value);
        }

        private BitmapImage _largeLogoImage = null;
        public BitmapImage LargeLogoImage
        {
            get => _largeLogoImage;
            set => SetProperty(ref _largeLogoImage, value);
        }
    }
}
