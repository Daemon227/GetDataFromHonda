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
        private string anhVersionUrl;
        private string mauVersion;

        public string MaVersion { get => maVersion; set => maVersion = value; }
        public string GiaBanVersion { get => giaBanVersion; set => giaBanVersion = value; }
        public string AnhVersionUrl { get => anhVersionUrl; set => anhVersionUrl = value; }
        public string MauVersion { get => mauVersion; set => mauVersion = value; }
        public string TenVersion { get => tenVersion; set => tenVersion = value; }
    }
}
