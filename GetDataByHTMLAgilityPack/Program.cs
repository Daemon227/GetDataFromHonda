using HtmlAgilityPack;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Net.WebRequestMethods;
using Microsoft.Data.SqlClient;

namespace GetDataByHTMLAgilityPack
{
    internal class Program
    {
        // danh sách các xe
        public static List<MotoBike> motoBikes = new List<MotoBike>();
        // đường dẫn đến folder lưu ảnh, mọi người linh hoạt đổi nhé(đây là link lưu trong máy tôi thôi)
        public static string anhMoTaFolderPath = "D:\\HOC TAP\\HOC KY 5\\PTPM DV\\BTL\\Data\\Img\\MoTa";
        public static string anhVersionFolderPath = "D:\\HOC TAP\\HOC KY 5\\PTPM DV\\BTL\\Data\\Img\\AnhVersion";
        public static string libraryImgFolderPath = "D:\\HOC TAP\\HOC KY 5\\PTPM DV\\BTL\\Data\\Img\\LibraryImgs";
        public static string connectionString = @"Data Source=DatDepTrai;Initial Catalog=MotoDataWebsite;Integrated Security=True;Trust Server Certificate=True";
        public static SqlConnection connection = null;

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            await setInfomation();
            //ShowList();
            AddDataIntoMotoLibrary();
            AddDataIntoMotoBike();
            AddDataIntoMotoImage();
            AddDataIntoBikeVersion();
            Console.WriteLine("Finished");
        }

        public static async Task setInfomation()
        {
            var html = new HtmlWeb();
            string webUrl = "https://www.honda.com.vn/xe-may/san-pham";
            string webUrl2 = "https://www.honda.com.vn";
            //link page1
            var page1 = html.Load(webUrl);
            //lay list sp page1
            var productNodes = page1.DocumentNode.SelectNodes("//div[@class='category-item col-lg-3 col-md-6 show']");

            if (productNodes != null)
            {
                foreach (var productNode in productNodes)
                {
                    MotoBike motoBike = new MotoBike();
                    // set ma va hang san xuat
                    int index = motoBikes.Count;
                    motoBike.MaXe = "0" + index;
                    motoBike.HangSanXuat = "Honda";
                    motoBike.MoTa = "Mo ta";

                    //lay du lieu o trang 1
                    motoBike.TenXe = productNode.SelectSingleNode(".//div[@class='nameAndColor'] /h3").InnerText.Trim();
                    motoBike.LoaiXe = productNode.GetAttributeValue("data-id", "");
                    var imgsrc = productNode.SelectSingleNode(".//div[@class='thumb'] /img");
                    if (imgsrc != null)
                    {
                        string url = imgsrc.GetAttributeValue("src", "");
                        motoBike.AnhMoTaUrl = await DownloadImage(motoBike,url, anhMoTaFolderPath, (motoBike.MaXe + "_MoTa.png"));
                    }
                    motoBike.GiaBanMoTa = productNode.SelectSingleNode(".//div[@class='nameAndColor'] //span").InnerText.Trim();



                    //lay du lieu tu trang 2
                    var productDetailNode = productNode.SelectSingleNode(".//a[@class='category-item-content']");
                    if (productDetailNode != null)
                    {
                        string productDetailUrl = productDetailNode.GetAttributeValue("href", "");
                        var page2 = html.Load(webUrl2 + productDetailUrl);

                        //lay thong tin ve anh, mau, gia
                       await setBikeVersion(motoBike, page2);

                        // lay tu danh sach thong tin ve thong so ky thuat
                        setThongSoKyThuat(motoBike, page2);

                        // lay danh sach anh trong thu vien anh
                        await setMotoLibrary(motoBike, page2);
                    }
                    // them vao
                    motoBikes.Add(motoBike);
                    //break;
                }
            }
        }

        public static async Task setBikeVersion(MotoBike motoBike, HtmlDocument page2)
        {// pt nay set version cua tung xe
            var versionNodes = page2.DocumentNode.SelectSingleNode(".//div[@class ='motor-section-price-color-container price_color_for_pc']");
            if( versionNodes != null)
            {
                foreach (var version in versionNodes.SelectNodes("./div[contains(@class,'motor-360')]"))
                {
                    //dat bien de gan gia tri 
                    string maVersion;
                    string tenPhienBan;
                    string anhUrl ="";
                    string giaBan;
                    string mau = "";

                    // set gia tri cho ma -> gia ban 
                    string id = version.GetAttributeValue("data-index","");
                    maVersion = motoBike.MaXe + "_Version_" + id;
                    tenPhienBan = version.GetAttributeValue("data-label_model","");

                    //set urlAnh cach 1
                    var anhNode1 = version.SelectSingleNode(".//img[@alt ='example']");
                    if (anhNode1 != null) 
                    {
                        string url = anhNode1.GetAttributeValue("src", "");
                        string filename = maVersion;
                        anhUrl = await DownloadImage(motoBike, url, anhVersionFolderPath, (filename +".png"));
                        //anhUrl = anhNode.GetAttributeValue("src", "");
                    }
                    // day la vai cai url thieu
                    var anhNode2 = version.SelectSingleNode(".//div[@class ='motor-360-img']/img");
                    if (anhNode2 != null)
                    {
                        string url = anhNode2.GetAttributeValue("src", "");
                        string filename = maVersion;
                        anhUrl = await DownloadImage(motoBike, url, anhVersionFolderPath, (filename + ".png"));
                        //anhUrl = anhNode.GetAttributeValue("src", "");
                    }

                    giaBan = version.SelectSingleNode(".//h2[@class='proposal-price']").InnerText.Trim();

                    // set gia tri cho mau
                    var mauNodes = versionNodes.SelectNodes(".//div[contains(@class,'version-color version-color')]");
                    if (mauNodes != null)
                    {
                        foreach(var mauNode in mauNodes)
                        {
                            string dataindex = mauNode.GetAttributeValue("data-index","");
                            if (dataindex.Equals(id))
                            {
                                mau = mauNode.SelectSingleNode("./div[@class = 'color-text']").InnerText.Trim();
                                break;
                            }
                        }
                    }
                    motoBike.SetAnhMauGia(maVersion,tenPhienBan,giaBan, anhUrl,mau);
                }
            }
        }
        public static void setThongSoKyThuat(MotoBike motoBike, HtmlDocument page2)
        {// phuong thuc nay set thong so ky thuat
            var productDetailNodes = page2.DocumentNode.SelectNodes(".//div[@class='col-6 col-lg-7 spec-item-value']");
            if (productDetailNodes != null)
            {
                int i = 0;
                foreach (var productDetail in productDetailNodes)
                {
                    string value = productDetail.SelectSingleNode(".//p").InnerText.Trim();
                    switch (i)
                    {
                        case 0: motoBike.TrongLuong = value; i++; break;
                        case 1: motoBike.KichThuoc = value; i++; break;
                        case 2: motoBike.KhoangCachTrucBanhXe = value; i++; break;
                        case 3: motoBike.DoCaoYen = value; i++; break;
                        case 4: motoBike.DoCaoGamXe = value; i++; break;
                        case 5: motoBike.DungTichBinhXang = value; i++; break;
                        case 6: motoBike.KichCoLop = ("Trước/Sau: " + value); i++; break;
                        case 7: motoBike.PhuocTruoc = value; i++; break;
                        case 8: motoBike.PhuocSau = value; i++; break;
                        case 9: motoBike.LoaiDongCo = value; i++; break;
                        case 10: motoBike.CongSuatToiDa = value; i++; break;
                        case 11: i++; break;
                        case 12: motoBike.MucTieuThuNhienLieu = value; i++; break;
                        case 13: i++; break;
                        case 14: motoBike.HeThongKhoiDong = value; i++; break;
                        case 15: motoBike.MomentCucDai = value; i++; break;
                        case 16: motoBike.DungTichXyLanh = value; i++; break;
                        case 17: motoBike.DuongKinhHanhTrinhPittong = value; i++; break;
                        case 18: motoBike.TySoNen = value; i++; break;
                        default: break;
                    }
                }
            }
            else
            {
                Console.WriteLine("null");
            }
        }

        public static async Task setMotoLibrary(MotoBike motoBike, HtmlDocument page2)
        {
            var libraryNodes = page2.DocumentNode.SelectNodes(".//div[@class='col-md-4 col-6 library_image']");
            if (libraryNodes != null) 
            {
                string libraryId = motoBike.MaXe + "_library";
                List<string> imgList = new List<string>();
                int index = 0;
                foreach (var node in libraryNodes)
                {
                    var imgNode = node.SelectSingleNode(".//img[@class='list-img']");
                    string url = imgNode.GetAttributeValue("src", "");
                    string imgUrl = await DownloadImage(motoBike, url, libraryImgFolderPath, (motoBike.MaXe + "_ThuVien_" + index + ".png"));
                    imgList.Add(imgUrl);
                    index++;
                }
                motoBike.setMotoLibrary(libraryId, imgList);
            }
            else
            {
                string libraryId = motoBike.MaXe + "_library";
                string imgurl = "O Day Khong Co Anh";
                List<string> imgList = new List<string>();
                imgList.Add(imgurl);
                motoBike.setMotoLibrary(libraryId, imgList);
            }
        }
        public static void ShowList()
        {
            if (motoBikes.Count > 0)
            {
                foreach (var moto in motoBikes)
                {
                    moto.ShowInformation();
                }
            }
        }
        
        public static async Task<string> DownloadImage (MotoBike motoBike,string imgUrl, string saveFolderPath, string fileName)
        {
            // phuong thuc down anh xuong va tra ve dia chi sau khi da luu vao o dia
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(5);
                try
                {
                    string filePath = Path.Combine(saveFolderPath, fileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        return filePath;
                    }
                    else
                    {
                        byte[] imageBytes = await client.GetByteArrayAsync(imgUrl);
                        await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);
                        return filePath;
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine (ex.Message);
                    return "";
                }
            }
        }

        public static void AddDataIntoMotoBike()
        {
            string insertMotoBikeQuery = @"INSERT INTO MotoBike 
            (MaXe, TenXe, LoaiXe, HangSanXuat, AnhMoTaUrl, GiaBanMoTa, MoTa, TrongLuong, KichThuoc, KhoangCachTrucBanhXe, 
            DoCaoYen, DoCaoGamXe, DungTichBinhXang, KichCoLop, PhuocTruoc, PhuocSau, LoaiDongCo, CongSuatToiDa, 
            MucTieuThuNhienLieu, HeThongKhoiDong, MomentCucDai, DungTichXyLanh, DuongKinhHanhTrinhPittong, TySoNen, MaLibrary) 
            VALUES (@MaXe, @TenXe, @LoaiXe, @HangSanXuat, @AnhMoTaUrl, @GiaBanMoTa, @MoTa, @TrongLuong, @KichThuoc, 
            @KhoangCachTrucBanhXe, @DoCaoYen, @DoCaoGamXe, @DungTichBinhXang, @KichCoLop, @PhuocTruoc, @PhuocSau, 
            @LoaiDongCo, @CongSuatToiDa, @MucTieuThuNhienLieu, @HeThongKhoiDong, @MomentCucDai, @DungTichXyLanh, 
            @DuongKinhHanhTrinhPittong, @TySoNen, @MaLibrary)";


            foreach (var moto in motoBikes)
            {
                
                using (connection = new SqlConnection(connectionString))
                {
                    // truyen vao bang MotoBike
                    SqlCommand cmd = new SqlCommand(insertMotoBikeQuery, connection);
                    cmd.Parameters.AddWithValue("@MaXe", moto.MaXe);
                    cmd.Parameters.AddWithValue("@TenXe", moto.TenXe);
                    cmd.Parameters.AddWithValue("@LoaiXe", moto.LoaiXe);
                    cmd.Parameters.AddWithValue("@HangSanXuat", moto.HangSanXuat);
                    cmd.Parameters.AddWithValue("@AnhMoTaUrl", moto.AnhMoTaUrl);
                    cmd.Parameters.AddWithValue("@GiaBanMoTa", moto.GiaBanMoTa);
                    cmd.Parameters.AddWithValue("@MoTa", moto.MoTa);
                    cmd.Parameters.AddWithValue("@TrongLuong", moto.TrongLuong);
                    cmd.Parameters.AddWithValue("@KichThuoc", moto.KichThuoc);
                    cmd.Parameters.AddWithValue("@KhoangCachTrucBanhXe", moto.KhoangCachTrucBanhXe);
                    cmd.Parameters.AddWithValue("@DoCaoYen", moto.DoCaoYen);
                    cmd.Parameters.AddWithValue("@DoCaoGamXe", moto.DoCaoGamXe);
                    cmd.Parameters.AddWithValue("@DungTichBinhXang", moto.DungTichBinhXang);
                    cmd.Parameters.AddWithValue("@KichCoLop", moto.KichCoLop);
                    cmd.Parameters.AddWithValue("@PhuocTruoc", moto.PhuocTruoc);
                    cmd.Parameters.AddWithValue("@PhuocSau", moto.PhuocSau);
                    cmd.Parameters.AddWithValue("@LoaiDongCo", moto.LoaiDongCo);
                    cmd.Parameters.AddWithValue("@CongSuatToiDa", moto.CongSuatToiDa);
                    cmd.Parameters.AddWithValue("@MucTieuThuNhienLieu", moto.MucTieuThuNhienLieu);
                    cmd.Parameters.AddWithValue("@HeThongKhoiDong", moto.HeThongKhoiDong);
                    cmd.Parameters.AddWithValue("@MomentCucDai", moto.MomentCucDai);
                    cmd.Parameters.AddWithValue("@DungTichXyLanh", moto.DungTichXyLanh);
                    cmd.Parameters.AddWithValue("@DuongKinhHanhTrinhPittong", moto.DuongKinhHanhTrinhPittong);
                    cmd.Parameters.AddWithValue("@TySoNen", moto.TySoNen);
                    cmd.Parameters.AddWithValue("@MaLibrary", moto.MotoLibrary.MaLibrary);

                    ///////////
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }  
                
            }

        }


        public static void AddDataIntoMotoLibrary()
        {
            string insertMotoLibraryQuery = @"INSERT INTO MotoLibrary (MaLibrary) VALUES (@MaLibrary)";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var moto in motoBikes)
                {
                    if (!string.IsNullOrEmpty(moto.MotoLibrary.MaLibrary)) // Kiểm tra xem MaLibrary có hợp lệ không
                    {
                        using (SqlCommand cmd = new SqlCommand(insertMotoLibraryQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@MaLibrary", moto.MotoLibrary.MaLibrary);

                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (SqlException ex)
                            {
                                // Xử lý lỗi nếu cần
                                Console.WriteLine($"Lỗi: {ex.Message}");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Giá trị MaLibrary không hợp lệ.");
                    }
                }
            }
        }


        public static void AddDataIntoMotoImage()
        {
            string insertMotoImageQuery = "INSERT INTO MotoImage (MaLibrary, ImageUrl) VALUES (@MaLibrary, @ImageUrl)";
            foreach (var moto in motoBikes)
            {
                foreach (var image in moto.MotoLibrary.MotoImageList)
                {
                    using (connection = new SqlConnection(connectionString))
                    {
                        SqlCommand cmd = new SqlCommand(insertMotoImageQuery, connection);
                        cmd.Parameters.AddWithValue("@MaLibrary", moto.MotoLibrary.MaLibrary);
                        cmd.Parameters.AddWithValue("@ImageUrl", image);
                        connection.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        public static void AddDataIntoBikeVersion()
        {
            string insertMotoVersionQuery = @"INSERT INTO MotoVersion 
                (MaVersion, TenVersion, GiaBanVersion, AnhVersionUrl, MauVersion, MaXe) 
            VALUES 
                (@MaVersion, @TenVersion, @GiaBanVersion, @AnhVersionUrl, @MauVersion, @MaXe)";

            foreach (var moto in motoBikes)
            {
                foreach (var version in moto.BikeVersionList)
                {
                    using (connection = new SqlConnection(connectionString))
                    {
                        SqlCommand cmd = new SqlCommand(insertMotoVersionQuery, connection);

                        cmd.Parameters.AddWithValue("@MaVersion", version.MaVersion);
                        cmd.Parameters.AddWithValue("@TenVersion", version.TenVersion);
                        cmd.Parameters.AddWithValue("@GiaBanVersion", version.GiaBanVersion);
                        cmd.Parameters.AddWithValue("@AnhVersionUrl", version.AnhVersionUrl);
                        cmd.Parameters.AddWithValue("@MauVersion", version.MauVersion);
                        cmd.Parameters.AddWithValue("@MaXe", moto.MaXe);
                        connection.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
