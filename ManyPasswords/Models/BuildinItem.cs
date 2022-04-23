using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyPasswords.Models
{
    public class BuildinItem
    {
        public string sName { get; set; }
        public string sNote { get; set; }
        public string sWebsite { get; set; }
        public char sFirstLetter { get; set; }
        public string sCoverImage { get; set; }

        public BuildinItem(string image, string name, string note, string website, char firstLetter)
        {
            try
            {
                this.sName = name;
                this.sNote = note;
                this.sWebsite = website;
                this.sFirstLetter = firstLetter;
                this.sCoverImage = "ms-appx:///Assets/BuildInIcon/" + image + ".jpg"; ;
            }
            catch { }
        }
    }
}
