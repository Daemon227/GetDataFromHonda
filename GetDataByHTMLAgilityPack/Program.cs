using HtmlAgilityPack;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Net.WebRequestMethods;
using Microsoft.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;
using System;
using System.Xml.Linq;
using System.Security.AccessControl;

namespace GetDataByHTMLAgilityPack
{
    internal class Program
    {
        // danh sách các xe
        public static List<MotoBike> motoBikes = new List<MotoBike>();
        public static List<BikeType> bikeTypes = new List<BikeType>();
        public static List<Brand> brands = new List<Brand>();
        // đường dẫn đến folder lưu ảnh, mọi người linh hoạt đổi nhé(đây là link lưu trong máy tôi thôi)
        public static string anhMoTaFolderPath = "D:\\HOC TAP\\HOC KY 5\\PTPM DV\\BTL\\Data\\Img\\MoTa";
        public static string anhVersionFolderPath = "D:\\HOC TAP\\HOC KY 5\\PTPM DV\\BTL\\Data\\Img\\AnhVersion";
        public static string libraryImgFolderPath = "D:\\HOC TAP\\HOC KY 5\\PTPM DV\\BTL\\Data\\Img\\LibraryImgs";

        public static string connectionString = @"Data Source=DatDepTrai;Initial Catalog=MotoWebsite;Integrated Security=True;Trust Server Certificate=True";
        public static SqlConnection connection = null;

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            await setInfomation();
            //ShowList();

            AddDataIntoMotoLibrary();
            AddDataIntoMotoType();
            AdddataIntoBrand();
            AddDataIntoMotoBike();
            AddDataIntoLibraryImage();
            AddDataIntoMotoVersion();
            AddDataIntoVersionColor();
            /*foreach (MotoBike bike in motoBikes)
            {
                Console.WriteLine(bike.ToString());
                Console.WriteLine();

            }*/

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
            //var productNodes = page1.DocumentNode.SelectNodes("//div");
            //var productNodes = page1.DocumentNode.SelectNodes("//div[contains(@class, 'category-item')]");
            var product = page1.DocumentNode.SelectSingleNode("//div[@class = 'row-content row content_cate']");
            
            var productNodes = product.SelectNodes("./div");

            if (productNodes != null)
            {
                foreach (var productNode in productNodes)
                {
                    if (!productNode.GetAttributeValue("data-id", "").Equals("xe-dien"))
                    {
                        MotoBike motoBike = new MotoBike();
                        // set ma va hang san xuat
                        int index = motoBikes.Count;
                        motoBike.MaXe = "0" + index;
                        motoBike.MaHangSanXuat = "honda";

                        //lay du lieu o trang 1
                        motoBike.TenXe = productNode.SelectSingleNode(".//div[@class='nameAndColor'] /h3").InnerText.Trim();
                        motoBike.MaLoai = productNode.GetAttributeValue("data-id", "");

                        var imgsrc = productNode.SelectSingleNode(".//div[@class='thumb'] /img");
                        if (imgsrc != null)
                        {
                            string url = imgsrc.GetAttributeValue("src", "");
                            motoBike.AnhMoTaUrl = await DownloadImage(motoBike, url, anhMoTaFolderPath, (motoBike.MaXe + "_MoTa.png"));
                        }
                        motoBike.GiaBanMoTa = productNode.SelectSingleNode(".//div[@class='nameAndColor'] //span").InnerText.Trim();

                        //lay du lieu tu trang 2
                        var productDetailNode = productNode.SelectSingleNode(".//a[@class='category-item-content']");
                        if (productDetailNode != null)
                        {
                            string productDetailUrl = productDetailNode.GetAttributeValue("href", "");
                            string page2Url = webUrl2 + productDetailUrl;
                            var page2 = html.Load(page2Url);

                            //lay thong tin ve anh, mau, gia
                            await setMotoVersion(motoBike, page2, page2Url, html);

                            // lay tu danh sach thong tin ve thong so ky thuat
                            setThongSoKyThuat(motoBike, page2);

                            // lay danh sach anh trong thu vien anh
                            await setMotoLibrary(motoBike, page2);

                        }
                        // them vao
                        motoBikes.Add(motoBike);

                        #region Them brand &type
                        Brand brand = new Brand();
                        brand.MaHangSanXuat = motoBike.MaHangSanXuat;
                        brand.TenHangSanXuat = "Honda";
                        if (!brands.Contains(brand))
                        {
                            brand.BikeList.Add(motoBike);
                            brands.Add(brand);
                        }
                        else
                        {
                            foreach (var b in brands)
                            {
                                if (b.MaHangSanXuat.Equals(brand.MaHangSanXuat))
                                {
                                    b.BikeList.Add(motoBike);
                                    break;
                                }
                            }
                        }
                        BikeType type = new BikeType();
                        type.MaLoai = motoBike.MaLoai;
                        if (!bikeTypes.Contains(type))
                        {
                            type.SetTenLoai();
                            type.BikeList.Add(motoBike);
                            bikeTypes.Add(type);
                        }
                        else
                        {
                            foreach (var t in bikeTypes)
                            {
                                if (t.MaLoai.Equals(type.MaLoai))
                                {
                                    t.BikeList.Add(motoBike);
                                    break;
                                }
                            }
                        }
                        #endregion
                        //break;
                    }
                
                }
            }
            else
            {
                Console.WriteLine("Product Nodes is null");
            }
        }

        public static async Task setMotoVersion(MotoBike motoBike, HtmlDocument page2, string url, HtmlWeb html)
        {
            var versionList = page2.DocumentNode.SelectSingleNode(".//select[@name ='version']");
            var list = versionList.SelectNodes("./option");
            Console.WriteLine("xe " + motoBike.TenXe + " co bang nay version nay " + list.Count);
            
            foreach (var node in list)
            {
                string newUrl = "";
                int queryIndex = url.IndexOf('?');
                if (queryIndex > -1)
                {
                    newUrl = url.Substring(0, queryIndex);
                }
                int idVersion = node.GetAttributeValue("value", 0);
                Console.WriteLine("link moi ne: " + newUrl + "?version=" + idVersion);
                var page3 = html.Load(newUrl + "?version=" + idVersion);
                //tren nay chay ngon roi nhe dcu no

                string maVersion = motoBike.MaXe + "_Version_" + idVersion;
                string tenVersion = node.InnerText.Trim();
                string giaBan = page3.DocumentNode.SelectSingleNode(".//h2[@class='proposal-price']").InnerText.Trim();
                string color = "";
                string colorID = "";
                List<string> anhVersionsURL = new List<string>();
                //set ten version roi
                //Console.WriteLine(maVersion + " " + tenVersion + " " + giaBan);

                var mauNode = page3.DocumentNode.SelectSingleNode(".//div[@class='card-body']");
                var mauNodes = mauNode.SelectNodes("./div");
                if (mauNodes != null)
                {
                    Console.WriteLine("version nay co bang nay mau day" + mauNodes.Count);
                    foreach (var colorNode in mauNodes)
                    {
                        string tenmau = colorNode.SelectSingleNode("./div[@class = 'color-text']//span").InnerText.Trim();
                        color = tenmau;
                        colorID = motoBike.MaXe + "_Version_" + idVersion + "_" + tenmau; 
                        int VersionColorID = int.Parse(colorNode.GetAttributeValue("data-index", ""));


                        var container = page3.DocumentNode.SelectSingleNode(".//div[@id='collapseModelDK0']");
                        var containerNodes = container.SelectNodes("./div");

                        foreach (var containerNode in containerNodes)
                        {
                            int containerId = int.Parse(containerNode.GetAttributeValue("data-index", ""));
                            if (VersionColorID == containerId)
                            {
                                var imgNode = containerNode.SelectSingleNode(".//div[@class ='canvas-image-360']/img[@alt='example']");
                                if (imgNode != null)
                                {
                                    Console.WriteLine("imgNodes khong bi null nhe ca nha" + idVersion);
                                    for (int j = 0; j < 8; j++)
                                    {
                                        string anhUrl = imgNode.GetAttributeValue("src", "");
                                        string newLink = anhUrl.Substring(0, anhUrl.Length - 5) + j + ".png";
                                        string fileName = maVersion + "_" + j + ".png";
                                        string anhDownloadUrl = await DownloadImage(motoBike, newLink, anhVersionFolderPath, fileName);
                                        anhVersionsURL.Add(fileName);
                                    }
                                }
                                else
                                {
                                    //set th2 o day
                                    Console.WriteLine("imgNodes null nhe ca nha" + idVersion);
                                    var imgNode2 = containerNode.SelectSingleNode(".//img");
                                    if (imgNode2 != null)
                                    {
                                        string anhUrl = imgNode2.GetAttributeValue("src", "");
                                        string fileName = maVersion + "_" + 0 + ".png";
                                        string anhDownloadUrl = await DownloadImage(motoBike, url, anhVersionFolderPath, fileName);
                                        anhVersionsURL.Add(fileName);
                                        //Console.WriteLine(fileName);
                                    }
                                }
                            }

                        }
                    }
                }
                else
                {
                    Console.WriteLine("Null anh");
                }
                motoBike.setMotoVersion(maVersion, tenVersion, giaBan, colorID,color, anhVersionsURL);
                Console.WriteLine();
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

            // set cac thu khac
            var descriptionNodes = page2.DocumentNode.SelectNodes(".//div[@class='description_mb slide-animation']");
            if (descriptionNodes != null)
            {
                for (int i = 0; i < descriptionNodes.Count; i++)
                {
                    string description = descriptionNodes[i].SelectSingleNode(".//p[@class='description']").InnerText.Trim();
                    if (i == 0)
                    {
                        motoBike.TinhNangNoiBat = description;

                    }
                    else if (i == 1)
                    {
                        motoBike.ThietKe = description;

                    }
                    else if (i == 3)
                    {
                        motoBike.TienIch = description;

                    }
                }

            }

        }

        public static async Task setMotoLibrary(MotoBike motoBike, HtmlDocument page2)
        {
            var libraryNodes = page2.DocumentNode.SelectNodes(".//div[@class='library_image']");
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

        public static async Task<string> DownloadImage(MotoBike motoBike, string imgUrl, string saveFolderPath, string fileName)
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
                        return fileName;
                    }
                    else
                    {
                        byte[] imageBytes = await client.GetByteArrayAsync(imgUrl);
                        await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);
                        return fileName;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return "";
                }
            }
        }

        public static void AddDataIntoMotoBike()
        {
            string insertMotoBikeQuery = @"INSERT INTO MotoBike 
                (MaXe, TenXe, MaLoai, MaHangSanXuat, AnhMoTaUrl, GiaBanMoTa, TrongLuong, KichThuoc, KhoangCachTrucBanhXe, 
                DoCaoYen, DoCaoGamXe, DungTichBinhXang, KichCoLop, PhuocTruoc, PhuocSau, LoaiDongCo, CongSuatToiDa, 
                MucTieuThuNhienLieu, HeThongKhoiDong, MomentCucDai, DungTichXyLanh, DuongKinhHanhTrinhPittong, TySoNen, 
                TinhNangNoiBat, ThietKe, TienIch, MaLibrary) 
                VALUES (@MaXe, @TenXe, @MaLoai, @MaHangSanXuat, @AnhMoTaUrl, @GiaBanMoTa, @TrongLuong, @KichThuoc, 
                @KhoangCachTrucBanhXe, @DoCaoYen, @DoCaoGamXe, @DungTichBinhXang, @KichCoLop, @PhuocTruoc, @PhuocSau, 
                @LoaiDongCo, @CongSuatToiDa, @MucTieuThuNhienLieu, @HeThongKhoiDong, @MomentCucDai, @DungTichXyLanh, 
                @DuongKinhHanhTrinhPittong, @TySoNen, @TinhNangNoiBat, @ThietKe, @TienIch, @MaLibrary)";


            foreach (var moto in motoBikes)
            {

                using (connection = new SqlConnection(connectionString))
                {
                    // truyen vao bang MotoBike
                    SqlCommand cmd = new SqlCommand(insertMotoBikeQuery, connection);
                    cmd.Parameters.AddWithValue("@MaXe", moto.MaXe);
                    cmd.Parameters.AddWithValue("@TenXe", moto.TenXe);
                    cmd.Parameters.AddWithValue("@MaLoai", moto.MaLoai);
                    cmd.Parameters.AddWithValue("@MaHangSanXuat", moto.MaHangSanXuat);
                    cmd.Parameters.AddWithValue("@AnhMoTaUrl", moto.AnhMoTaUrl);
                    cmd.Parameters.AddWithValue("@GiaBanMoTa", moto.GiaBanMoTa); 
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
                    if (moto.TySoNen == null)
                    {
                        cmd.Parameters.AddWithValue("@TySoNen", "11,5:1");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@TySoNen", moto.TySoNen);
                    }
                    cmd.Parameters.AddWithValue("@TinhNangNoiBat", moto.TinhNangNoiBat);
                    cmd.Parameters.AddWithValue("@ThietKe", moto.ThietKe);
                    if (moto.TienIch != null)
                    {
                        cmd.Parameters.AddWithValue("@TienIch", moto.TienIch);
                    }
                    else 
                    {
                        cmd.Parameters.AddWithValue("@TienIch", "@Hệ thống phanh chống bó cứng (ABS) giúp tăng tính an toàn khi phanh gấp, tránh bị trượt bánh, đặc biệt là trên đường trơn trượt.");
                    }
                    cmd.Parameters.AddWithValue("@MaLibrary", moto.MotoLibrary.MaLibrary);
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

        public static void AddDataIntoLibraryImage()
        {
            string insertMotoImageQuery = "INSERT INTO LibraryImage (MaLibrary, ImageUrl) VALUES (@MaLibrary, @ImageUrl)";
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

        public static void AddDataIntoMotoVersion()
        {
            string insertMotoVersionQuery = @"INSERT INTO MotoVersion 
                            (MaVersion, TenVersion, GiaBanVersion, MaXe) 
                     VALUES (@MaVersion, @TenVersion, @GiaBanVersion, @MaXe)";

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
                        cmd.Parameters.AddWithValue("@MaXe", moto.MaXe);
                        connection.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        public static void AddDataIntoVersionColor()
        {
            string insertVersionColorQuery = @"INSERT INTO VersionColor (MaVersionColor, TenMau, MaVersion) 
                                   VALUES (@MaVersionColor, @TenMau, @MaVersion)";
            string insertVersionImageQuery = @"INSERT INTO VersionImage (MaVersionColor, ImageUrl) 
                                   VALUES (@MaVersionColor, @ImageUrl)";
            foreach (var moto in motoBikes)
            {
                foreach (var version in moto.BikeVersionList)
                {
                    foreach (var versionColor in version.VersionColor)
                    {
                        using (connection = new SqlConnection(connectionString))
                        {
                            SqlCommand cmd = new SqlCommand(insertVersionColorQuery, connection);
                            cmd.Parameters.AddWithValue("@MaVersionColor", versionColor.MaMau);
                            cmd.Parameters.AddWithValue("@TenMau", versionColor.TenMau);
                            cmd.Parameters.AddWithValue("@MaVersion", version.MaVersion);
                            connection.Open();
                            cmd.ExecuteNonQuery();
                        }
                        foreach (var anh in versionColor.AnhVersions)
                        {
                            using (connection = new SqlConnection(connectionString))
                            {
                                SqlCommand cmd = new SqlCommand(insertVersionImageQuery, connection);
                                cmd.Parameters.AddWithValue("@MaVersionColor", versionColor.MaMau);
                                cmd.Parameters.AddWithValue("@ImageUrl", anh);
                                connection.Open();
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }

        /*public static void AddDataIntoVersionColor()
        {
            string insertVersionColorQuery = @"INSERT INTO VersionColor (MaVersionColor, TenMau, MaVersion) 
                                   VALUES (@MaVersionColor, @TenMau, @MaVersion)";
            string insertVersionImageQuery = @"INSERT INTO VersionImage (MaVersionColor, ImageUrl) 
                                   VALUES (@MaVersionColor, @ImageUrl)";
            foreach (var moto in motoBikes)
            {
                foreach (var version in moto.BikeVersionList)
                {
                    foreach (var versionColor in version.VersionColor)
                    {
                        using (connection = new SqlConnection(connectionString))
                        {
                            SqlCommand cmd = new SqlCommand(insertVersionColorQuery, connection);
                            cmd.Parameters.AddWithValue("@MaVersionColor", versionColor.MaMau);
                            cmd.Parameters.AddWithValue("@TenMau", versionColor.TenMau);
                            cmd.Parameters.AddWithValue("@MaVersion", version.MaVersion);
                            connection.Open();
                            cmd.ExecuteNonQuery();
                        }
                        foreach (var anh in versionColor.AnhVersions)
                        {
                            using (connection = new SqlConnection(connectionString))
                            {
                                SqlCommand cmd = new SqlCommand(insertVersionImageQuery, connection);
                                cmd.Parameters.AddWithValue("@MaVersionColor", versionColor.MaMau);
                                cmd.Parameters.AddWithValue("@ImageUrl", anh);
                                connection.Open();
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }
*/
        public static void AdddataIntoBrand()
        {
            string insertBrandQuery = @"INSERT INTO Brand 
                    (MaHangSanXuat, TenHangSanXuat) 
                    VALUES (@MaHangSanXuat, @TenHangSanXuat)";

            foreach (var brand in brands)
            {
                using (connection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(insertBrandQuery, connection);
                    cmd.Parameters.AddWithValue("@MaHangSanXuat", brand.MaHangSanXuat);
                    cmd.Parameters.AddWithValue("@TenHangSanXuat", brand.TenHangSanXuat);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void AddDataIntoMotoType()
        {
            string insertBikeTypeQuery = @"INSERT INTO MotoType 
                    (MaLoai, TenLoai) 
                    VALUES (@MaLoai, @TenLoai)";
            foreach (var type in bikeTypes)
            {
                using (connection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(insertBikeTypeQuery, connection);
                    cmd.Parameters.AddWithValue("@MaLoai", type.MaLoai);
                    cmd.Parameters.AddWithValue("@TenLoai", type.TenLoai);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
