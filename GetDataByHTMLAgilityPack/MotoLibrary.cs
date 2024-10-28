using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetDataByHTMLAgilityPack
{
    internal class MotoLibrary
    {
        private string maLibrary;
        private List<string> motoImageList = new List<string>();
        public string MaLibrary { get => maLibrary; set => maLibrary = value; }
        public List<string> MotoImageList { get => motoImageList; set => motoImageList = value; }

    }
}
