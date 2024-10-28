using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetDataByHTMLAgilityPack
{
    public class BikeType
    {
        private string maLoai;
        private string tenLoai;
        private List<MotoBike> bikeList = new List<MotoBike>();
        public string MaLoai { get => maLoai; set => maLoai = value; }
        public string TenLoai { get => tenLoai; set => tenLoai = value; }
        public List<MotoBike> BikeList { get => bikeList; set => bikeList = value; }

        public override bool Equals(object? obj)
        {
            return obj is BikeType type &&
                   maLoai == type.maLoai;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(maLoai);
        }

        public void SetTenLoai()
        {
            if (maLoai.Equals("xe-tay-ga")) tenLoai = "Xe Tay Ga";
            else if (maLoai.Equals("xe-so")) tenLoai = "Xe Số";
            else if (maLoai.Equals("xe-con-tay")) tenLoai = "Xe Côn Tay";
            else tenLoai = "Xe Phân Khối Lớn";
        }
    }
}
