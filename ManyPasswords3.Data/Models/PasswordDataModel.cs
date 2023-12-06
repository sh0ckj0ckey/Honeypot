using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyPasswords3.Data.Models
{
    public class PasswordDataModel
    {
        public int Id { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string FirstLetter { get; set; }
        public string Name { get; set; }
        public string CreateDate { get; set; }
        public string EditDate { get; set; }
        public string Website { get; set; }
        public string Note { get; set; }
        public int Favorite { get; set; }
        public byte[] Image { get; set; }
    }
}
