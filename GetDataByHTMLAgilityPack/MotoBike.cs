using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GetDataByHTMLAgilityPack
{
    public class MotoBike
    {
        // lay o page 1
        //Thong tin chung
        private string maXe;
        private string tenXe;
        private string loaiXe;
        private string hangSanXuat;
        private string anhMoTaUrl;
        private string giaBanMoTa;
        private string moTa;

        // lay o page 2
        // Thong so ky thuat  
        private string trongLuong;
        private string kichThuoc;
        private string khoangCachTrucBanhXe;
        private string doCaoYen;
        private string doCaoGamXe;
        private string dungTichBinhXang;
        private string kichCoLop;
        private string phuocTruoc;
        private string phuocSau;
        private string loaiDongCo;
        private string congSuatToiDa;
        private string mucTieuThuNhienLieu;
        private string heThongKhoiDong;
        private string momentCucDai;
        private string dungTichXyLanh;
        private string duongKinhHanhTrinhPittong;
        private string tySoNen;

        //Thong tin version
        private List<BikeVersion> bikeVersionList = new List<BikeVersion>();

        private MotoLibrary motoLibrary = new MotoLibrary();
        #region Set and Get
        public string MaXe { get => maXe; set => maXe = value; }
        public string TenXe { get => tenXe; set => tenXe = value; }
        public string LoaiXe { get => loaiXe; set => loaiXe = value; }
        public string HangSanXuat { get => hangSanXuat; set => hangSanXuat = value; }
        public string AnhMoTaUrl { get => anhMoTaUrl; set => anhMoTaUrl = value; }
        public string MoTa { get => moTa; set => moTa = value; }
        public List<BikeVersion> BikeVersionList { get => bikeVersionList; set => bikeVersionList = value; }
        public string TrongLuong { get => trongLuong; set => trongLuong = value; }
        public string KichThuoc { get => kichThuoc; set => kichThuoc = value; }
        public string KhoangCachTrucBanhXe { get => khoangCachTrucBanhXe; set => khoangCachTrucBanhXe = value; }
        public string DoCaoYen { get => doCaoYen; set => doCaoYen = value; }
        public string DoCaoGamXe { get => doCaoGamXe; set => doCaoGamXe = value; }
        public string DungTichBinhXang { get => dungTichBinhXang; set => dungTichBinhXang = value; }
        public string KichCoLop { get => kichCoLop; set => kichCoLop = value; }
        public string PhuocTruoc { get => phuocTruoc; set => phuocTruoc = value; }
        public string PhuocSau { get => phuocSau; set => phuocSau = value; }
        public string LoaiDongCo { get => loaiDongCo; set => loaiDongCo = value; }
        public string CongSuatToiDa { get => congSuatToiDa; set => congSuatToiDa = value; }
        public string MucTieuThuNhienLieu { get => mucTieuThuNhienLieu; set => mucTieuThuNhienLieu = value; }
        public string MomentCucDai { get => momentCucDai; set => momentCucDai = value; }
        public string DungTichXyLanh { get => dungTichXyLanh; set => dungTichXyLanh = value; }
        public string DuongKinhHanhTrinhPittong { get => duongKinhHanhTrinhPittong; set => duongKinhHanhTrinhPittong = value; }
        public string TySoNen { get => tySoNen; set => tySoNen = value; }
        public string GiaBanMoTa { get => giaBanMoTa; set => giaBanMoTa = value; }
        public string HeThongKhoiDong { get => heThongKhoiDong; set => heThongKhoiDong = value; }
        internal MotoLibrary MotoLibrary { get => motoLibrary; set => motoLibrary = value; }
        #endregion

        public void SetAnhMauGia(string maPhienBan, string tenPhienBan,string giaBan, string anhUrl, string mau)
        {
            BikeVersion thing = new BikeVersion();
            thing.MaVersion = maPhienBan;
            thing.TenVersion = tenPhienBan;
            thing.GiaBanVersion = giaBan;
            thing.AnhVersionUrl = anhUrl;
            thing.MauVersion = mau;
            this.bikeVersionList.Add(thing);
        }
        
        public void setMotoLibrary(string maLibrary, List<string> imgList)
        { 
            this.motoLibrary.MaLibrary = maLibrary;
            this.motoLibrary.MotoImageList = imgList;
        }
        public void ShowInformation()
        {
            // infor page1
            Console.WriteLine(this.MaXe);
            Console.WriteLine(this.tenXe);
            Console.WriteLine(this.HangSanXuat);
            Console.WriteLine(this.LoaiXe);
            Console.WriteLine(this.anhMoTaUrl);
            Console.WriteLine(this.giaBanMoTa);
            Console.WriteLine(this.moTa);

            //infor thong so ky thuat
            /* Console.WriteLine(this.trongLuong);
             Console.WriteLine(this.kichThuoc);
             Console.WriteLine(this.khoangCachTrucBanhXe);
             Console.WriteLine(this.doCaoYen);
             Console.WriteLine(this.doCaoGamXe);
             Console.WriteLine(this.dungTichBinhXang);
             Console.WriteLine(this.kichCoLop);
             Console.WriteLine(this.phuocTruoc);
             Console.WriteLine(this.phuocSau);
             Console.WriteLine(this.loaiDongCo);
             Console.WriteLine(this.congSuatToiDa);
             Console.WriteLine(this.mucTieuThuNhienLieu);
             Console.WriteLine(this.heThongKhoiDong);
             Console.WriteLine(this.momentCucDai);
             Console.WriteLine(this.dungTichXyLanh);
             Console.WriteLine(this.duongKinhHanhTrinhPittong);
             Console.WriteLine(this.tySoNen);  
             Console.WriteLine();*/

            // thong so version
            ShowBikeVersion();

            // danh sach cac anh trong thu vien anh
            ShowMotoLibrary();
        }

        public void ShowBikeVersion()
        {
            foreach (var amg in bikeVersionList)
            {
                Console.WriteLine(amg.MaVersion);
                Console.WriteLine(amg.TenVersion);
                Console.WriteLine(amg.GiaBanVersion);
                Console.WriteLine(amg.AnhVersionUrl);
                Console.WriteLine(amg.MauVersion);
                Console.WriteLine();
            }
        }

        public void ShowMotoLibrary()
        {
            if (motoLibrary.MotoImageList.Count > 0)
            {
                Console.WriteLine(motoLibrary.MaLibrary);
                foreach(var img in motoLibrary.MotoImageList)
                {
                    Console.WriteLine(img);
                }
                Console.WriteLine();
            }
        }
    }
}
