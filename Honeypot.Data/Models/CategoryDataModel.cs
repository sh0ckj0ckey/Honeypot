namespace Honeypot.Data.Models
{
    public class CategoryDataModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public long Order { get; set; }
    }
}
