namespace Honeypot.Data.Models
{
    public class PasswordDataModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public int ThirdPartyId { get; set; }
        public char FirstLetter { get; set; }
        public string Name { get; set; }
        public string CreateDate { get; set; }
        public string EditDate { get; set; }
        public string Website { get; set; }
        public string Note { get; set; }
        public int Favorite { get; set; }
        public string Logo { get; set; }
    }
}
