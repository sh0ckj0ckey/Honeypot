using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Honeypot.Data.Models;
using Honeypot.Services;
using Microsoft.UI.Xaml.Media.Imaging;

namespace Honeypot.ViewModels;

/// <summary>
/// Represents a password item displayed in the UI.
/// </summary>
public partial class PasswordItemViewModel : ObservableObject
{
    private int _id = -1;

    private int _categoryId = -1;

    private string _account;

    private string _password;

    private int _thirdPartyId = -1;

    private char _firstLetter = '#';

    private string _name;

    private string _createDate;

    private string _editDate;

    private string _website;

    private string _note;

    private bool _favorite = false;

    private string _logoImageFileName;

    private BitmapImage? _logoImage = null;

    private BitmapImage? _largeLogoImage = null;

    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public int CategoryId
    {
        get => _categoryId;
        set => SetProperty(ref _categoryId, value);
    }

    public string Account
    {
        get => _account;
        set => SetProperty(ref _account, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public int ThirdPartyId
    {
        get => _thirdPartyId;
        set => SetProperty(ref _thirdPartyId, value);
    }

    public char FirstLetter
    {
        get => _firstLetter;
        set => SetProperty(ref _firstLetter, value);
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string CreateDate
    {
        get => _createDate;
        set => SetProperty(ref _createDate, value);
    }

    public string EditDate
    {
        get => _editDate;
        set => SetProperty(ref _editDate, value);
    }

    public string Website
    {
        get => _website;
        set => SetProperty(ref _website, value);
    }

    public string Note
    {
        get => _note;
        set => SetProperty(ref _note, value);
    }

    public bool Favorite
    {
        get => _favorite;
        set => SetProperty(ref _favorite, value);
    }

    public string LogoImageFileName
    {
        get => _logoImageFileName;
        set => SetProperty(ref _logoImageFileName, value);
    }

    public BitmapImage? LogoImage
    {
        get => _logoImage;
        set => SetProperty(ref _logoImage, value);
    }

    public BitmapImage? LargeLogoImage
    {
        get => _largeLogoImage;
        set => SetProperty(ref _largeLogoImage, value);
    }

    public PasswordItemViewModel(PasswordModel passwordModel)
    {
        _id = passwordModel.Id;
        _categoryId = passwordModel.CategoryId;
        _account = passwordModel.Account;
        _password = passwordModel.Password;
        _thirdPartyId = passwordModel.ThirdPartyId;
        _firstLetter = passwordModel.FirstLetter;
        _name = passwordModel.Name;
        _createDate = passwordModel.CreateDate;
        _editDate = passwordModel.EditDate;
        _website = passwordModel.Website;
        _note = passwordModel.Note;
        _favorite = passwordModel.Favorite != 0;
        _logoImageFileName = passwordModel.Logo;
    }
}
