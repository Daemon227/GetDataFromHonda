using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetDataByHTMLAgilityPack
{
    public class Brand
    {
        private string maHangSanXuat;
        private string tenHangSanXuat;
        private List<MotoBike> bikeList = new List<MotoBike>();
        public string MaHangSanXuat { get => maHangSanXuat; set => maHangSanXuat = value; }
        public string TenHangSanXuat { get => tenHangSanXuat; set => tenHangSanXuat = value; }
        public List<MotoBike> BikeList { get => bikeList; set => bikeList = value; }

        public override bool Equals(object? obj)
        {
            return obj is Brand brand &&
                   maHangSanXuat == brand.maHangSanXuat;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(maHangSanXuat);
        }
    }
}
