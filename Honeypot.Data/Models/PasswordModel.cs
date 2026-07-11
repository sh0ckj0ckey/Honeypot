namespace Honeypot.Data.Models;

public class PasswordModel
{
    public long Id { get; set; }
    public long CategoryId { get; set; }
    public string Account { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public long ThirdPartyId { get; set; }
    public char FirstLetter { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CreateDate { get; set; } = string.Empty;
    public string EditDate { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public int Favorite { get; set; }
    public string Logo { get; set; } = string.Empty;
}
