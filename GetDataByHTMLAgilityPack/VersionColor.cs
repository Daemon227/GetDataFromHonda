using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetDataByHTMLAgilityPack
{
    public class VersionColor
    {
        private string maMau;
        private string tenMau;
        
        List<string> anhVersions = new List<string>();

        public string TenMau { get => tenMau; set => tenMau = value; }
        public List<string> AnhVersions { get => anhVersions; set => anhVersions = value; }
        public string MaMau { get => maMau; set => maMau = value; }
    }
}
