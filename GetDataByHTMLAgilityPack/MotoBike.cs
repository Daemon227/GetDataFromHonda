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
        private string anhMoTaUrl;
        private string giaBanMoTa;
        private string maHangSanXuat;
        private string maLoai;
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
        private string tinhNangNoiBat;
        private string thietKe;
        private string tienIch;

        //Thong tin version
        private List<BikeVersion> bikeVersionList = new List<BikeVersion>();

        private MotoLibrary motoLibrary = new MotoLibrary();
        #region Set and Get
        public string MaXe { get => maXe; set => maXe = value; }
        public string TenXe { get => tenXe; set => tenXe = value; }

        public string AnhMoTaUrl { get => anhMoTaUrl; set => anhMoTaUrl = value; }
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
        public string MaHangSanXuat { get => maHangSanXuat; set => maHangSanXuat = value; }
        public string MaLoai { get => maLoai; set => maLoai = value; }
        public string TinhNangNoiBat { get => tinhNangNoiBat; set => tinhNangNoiBat = value; }
        public string ThietKe { get => thietKe; set => thietKe = value; }
        public string TienIch { get => tienIch; set => tienIch = value; }
        #endregion

        public void setMotoVersion(string maPhienBan, string tenPhienBan, string giaBan, string mamau,string tenmau, List<string> anhVersions)
        {
            BikeVersion thing = new BikeVersion();
            thing.MaVersion = maPhienBan;
            thing.TenVersion = tenPhienBan;
            thing.GiaBanVersion = giaBan;
            thing.setVersionColor(mamau,tenmau, anhVersions);
            this.bikeVersionList.Add(thing);
        }

        public void setMotoLibrary(string maLibrary, List<string> imgList)
        {
            this.motoLibrary.MaLibrary = maLibrary;
            this.motoLibrary.MotoImageList = imgList;
        }

        public override string ToString()
        {
            return $"MaXe: {maXe}\n" +
                   $"TenXe: {tenXe}\n" +
                   $"AnhMoTaUrl: {anhMoTaUrl}\n" +
                   $"GiaBanMoTa: {giaBanMoTa}\n" +
                   $"MaHangSanXuat: {maHangSanXuat}\n" +
                   $"MaLoai: {maLoai}\n" +
                   $"TrongLuong: {trongLuong}\n" +
                   $"KichThuoc: {kichThuoc}\n" +
                   $"KhoangCachTrucBanhXe: {khoangCachTrucBanhXe}\n" +
                   $"DoCaoYen: {doCaoYen}\n" +
                   $"DoCaoGamXe: {doCaoGamXe}\n" +
                   $"DungTichBinhXang: {dungTichBinhXang}\n" +
                   $"KichCoLop: {kichCoLop}\n" +
                   $"PhuocTruoc: {phuocTruoc}\n" +
                   $"PhuocSau: {phuocSau}\n" +
                   $"LoaiDongCo: {loaiDongCo}\n" +
                   $"CongSuatToiDa: {congSuatToiDa}\n" +
                   $"MucTieuThuNhienLieu: {mucTieuThuNhienLieu}\n" +
                   $"HeThongKhoiDong: {heThongKhoiDong}\n" +
                   $"MomentCucDai: {momentCucDai}\n" +
                   $"DungTichXyLanh: {dungTichXyLanh}\n" +
                   $"DuongKinhHanhTrinhPittong: {duongKinhHanhTrinhPittong}\n" +
                   $"TySoNen: {tySoNen}\n" +
                   $"TinhNangNoiBat: {tinhNangNoiBat}\n" +
                   $"ThietKe: {thietKe}\n" +
                   $"TienIch: {tienIch}";
        }
    }
}
