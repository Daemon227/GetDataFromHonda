using HtmlAgilityPack;
using System.Data.SqlTypes;
using System.Text;
using static System.Net.WebRequestMethods;

namespace GetDataByHTMLAgilityPack

{
    public class MotoBike 
    {
        // phan nay co the lay o page1
        private string id;
        private string name;
        private string loaixe;
        private string price;

        // phan nay phai lay o page2
        private string imagePath;
        private string color;
        private string khoiLuong;
        private string kichThuoc;

        public string Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Price { get => price; set => price = value; }
        public string Loaixe { get => loaixe; set => loaixe = value; }
        public string ImagePath { get => imagePath; set => imagePath = value; }
        public string KhoiLuong { get => khoiLuong; set => khoiLuong = value; }
        public string KichThuoc { get => kichThuoc; set => kichThuoc = value; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            List<MotoBike> motoBikes = new List<MotoBike>();
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

                    //lay du lieu o trang 1
                    motoBike.Name = productNode.SelectSingleNode(".//div[@class='nameAndColor'] /h3").InnerText.Trim();
                    motoBike.Loaixe = productNode.GetAttributeValue("data-id", "");
                    var imgsrc = productNode.SelectSingleNode(".//div[@class='thumb'] /img");
                    if (imgsrc != null)
                    {
                        motoBike.ImagePath = imgsrc.GetAttributeValue("src", "");
                    }
                    motoBike.Price = productNode.SelectSingleNode(".//div[@class='nameAndColor'] //span").InnerText.Trim();
                    int index = motoBikes.Count;
                    motoBike.Id = "0" + index;

                    //lay du lieu tu trang 2
                    var productDetailNode = productNode.SelectSingleNode(".//a[@class='category-item-content']");
                    if (productDetailNode != null)
                    {
                        string productDetailUrl = productDetailNode.GetAttributeValue("href", "hihi");
                        var page2 = html.Load(webUrl2+ productDetailUrl);
    
                        // lay tu danh sach thong tin
                        var productDetailNodes = page2.DocumentNode.SelectNodes(".//div[@class='row spec-item']");
                        if (productDetailNodes != null)
                        {
                            int i = 0;
                            foreach (var productDetail in productDetailNodes)
                            {
                                string value = productDetail.SelectSingleNode(".//div[@class='col-6 col-lg-7 spec-item-value']//p").InnerText.Trim();
                                switch (i)
                                {
                                    case 0: motoBike.KhoiLuong = value; i++; break;
                                    case 1: motoBike.KichThuoc = value;i++; break;
                                    default: break;
                                }
                            }
                            
                        }
                        else
                        {
                            Console.WriteLine("null");
                        }
                    }

                    // them vao
                    motoBikes.Add(motoBike);
    
                }
            }
            // show thong tin
            foreach (var moto in motoBikes)
            {
                Console.WriteLine(moto.Id);
                Console.WriteLine(moto.Name);
                Console.WriteLine(moto.Price);
                Console.WriteLine(moto.Loaixe);
                Console.WriteLine(moto.ImagePath);
                Console.WriteLine(moto.KhoiLuong);
                Console.WriteLine(moto.KichThuoc);
                Console.WriteLine();
            }
        }
        
    }
}
