using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GetDataByHTMLAgilityPack
{
    public class BikeVersion
    {
        private string maVersion;
        private string tenVersion;
        private string giaBanVersion;
        private List<VersionColor> versionColor = new List<VersionColor>();

        public string MaVersion { get => maVersion; set => maVersion = value; }
        public string GiaBanVersion { get => giaBanVersion; set => giaBanVersion = value; }
        public string TenVersion { get => tenVersion; set => tenVersion = value; }
        public List<VersionColor> VersionColor { get => versionColor; set => versionColor = value; }

        public void setVersionColor(string mamau,string name, List<string> urls)
        {
            VersionColor v = new VersionColor();
            v.MaMau = mamau;
            v.TenMau = name;
            v.AnhVersions = urls;
            this.versionColor.Add(v);
        }
    }
}
