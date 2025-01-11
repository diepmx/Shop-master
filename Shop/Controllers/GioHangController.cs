using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PayPal.Api;
using Shop.Areas.Administrator.Data.message;
using Shop.Mail;
using Shop.Models;
using Shop.MoMo;
using Shop.Zalo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VNPAY_CS_ASPX;
using Notification = Shop.Areas.Administrator.Data.message.Notification;

namespace Shop.Controllers
{
    public class GioHangController : Controller
    {
        MyDataDataContext data = new MyDataDataContext();

        public JsonResult GetbyID(int ID)
        {
            HomeModel home = new HomeModel();
            var Dienthoai = home.GetListChiTietDonHangTheoDonDatHang(ID).Find(x => x.madon.Equals(ID));
            return Json(Dienthoai, JsonRequestBehavior.AllowGet);
        }

        public List<GioHang> Laygiohang()// lấy ra danh sách sản phẩm trong giỏ hàng
        {
            List<GioHang> lstGiohang = Session["Giohang"] as List<GioHang>;
            if (lstGiohang == null) // nếu giỏ hàng bằng null thì khởi tạo giỏ hàng rồi gắn giỏ hàng cho Session["Giohang"]
            {
                lstGiohang = new List<GioHang>();
                Session["Giohang"] = lstGiohang;
            }
            return lstGiohang;
        }
        public ActionResult ThemGioHang(int id, string strURL)// thêm 1 sản phẩm vào giỏ hàng
        {
            List<GioHang> lstGioHang = Laygiohang();
            GioHang sanpham = lstGioHang.Find(n => n.madienthoai == id); // tìm sản phẩm đã chọn theo id
            if (sanpham == null)
            {
                sanpham = new GioHang(id);
                lstGioHang.Add(sanpham);
                Notification.set_flash("Thêm giỏ hàng thành công!", "success");
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoluong++;
                return Redirect(strURL);
            }
        }
        private int TongSoLuong()// tính tổng số lượng đã mua trong giỏ hàng
        {
            int tsl = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                tsl = lstGiohang.Sum(n => n.iSoluong);
            }
            return tsl;

        }
        private int TongSoLuongSanPham()// tính tổng số loại đã chọn mua
        {
            int tsl = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                tsl = lstGiohang.Count;
            }
            return tsl;
        }
        private double TongTien()// tính tổng tiền giỏ hàng
        {
            double tt = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                tt = lstGiohang.Sum(n => n.dThanhTien);
            }
            return tt;

        }
        public ActionResult GioHang()// request trả về View giỏ hàng
        {
            List<GioHang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return View(lstGiohang);
        }
        public ActionResult GioHangPartial()// request trả về partialview giỏ hàng
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return PartialView();
        }
        public ActionResult XoaGiohang(int id)// xóa sản phẩm theo id
        {
            List<GioHang> lstGiohang = Laygiohang();
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.madienthoai == id);
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.madienthoai == id);
                Notification.set_flash("Xóa mặt hàng thành công!", "success");
                return RedirectToAction("GioHang");
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult CapnhatGiohang(int id, FormCollection collection)// cập nhật giỏ hàng theo id và form có số lượng
        {
            List<GioHang> lstGiohang = Laygiohang();
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.madienthoai == id);
            try
            {
                if (sanpham != null)
                {               
                    if (int.Parse(collection["txtSolg"].ToString()) > 50)
                    {
                        Notification.set_flash("Mua hàng số lượng lớn > 50 liên hệ Admin!", "warning");
                        return RedirectToAction("GioHang");
                    }
                    else
                    {
                        sanpham.iSoluong = int.Parse(collection["txtSolg"].ToString());
                    }
                }
            }
            catch (Exception)
            {
                Notification.set_flash("Nhập sai định dạng số lượng!", "warning");
                return RedirectToAction("GioHang");
            }                                          
            
            return RedirectToAction("GioHang");
        }
        public ActionResult XoaTatCaGioHang()// xóa tất cả các mặt hàng trong giỏ hàng
        {
            /*List<GioHang> lstGioHang = Laygiohang();*/
            List<GioHang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            Notification.set_flash("Xóa giỏ hàng thành công!", "success");
            return RedirectToAction("GioHang");
        }

        [HttpGet]
        public ActionResult DatHang()// đặt hàng
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                ViewBag.thongbao = "Bạn phải đăng nhập tài khoản để mua hàng!";
                return RedirectToAction("Login", "Account");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<GioHang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return View(lstGiohang);
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection collection)
        {
            DonHang dh = new DonHang();
            AspNetUser kh = (AspNetUser)Session["TaiKhoan"];
            List<GioHang> gh = Laygiohang();

            // Thiết lập thông tin đơn hàng
            dh.makh = kh.Id;
            dh.ngaydat = DateTime.Now;
            dh.giaohang = null;
            dh.thanhtoan = false;
            dh.tinhtrang = '0';

            data.DonHangs.InsertOnSubmit(dh);
            data.SubmitChanges();

            // Lưu chi tiết đơn hàng
            foreach (var item in gh)
            {
                ChiTietDonHang ctdh = new ChiTietDonHang();
                ctdh.madon = dh.madon;
                ctdh.madienthoai = item.madienthoai;
                ctdh.soluong = item.iSoluong;
                ctdh.dongia = (decimal)item.giaban;
                data.ChiTietDonHangs.InsertOnSubmit(ctdh);
            }
            data.SubmitChanges();

            // Xóa giỏ hàng sau khi đặt hàng
            Session["GioHang"] = null;

            // Thông báo thành công và chuyển hướng
            Notification.set_flash("Bạn đã đặt hàng thành công!", "success");
            return RedirectToAction("XacnhanDonhang", "GioHang");
        }

        public ActionResult XacnhanDonhang()//xác nhận đơn mạng
        {
            return View();
        }

        public ActionResult XacnhanThanhToan_MoMo()//xác nhận đơn mạng
        {
            return View();
        }

        public ActionResult ThanhToanThatBai()// Trả về View thông báo Thanh toán Thất bại
        {
            return View();
        }

        public ActionResult BadRequestMoMo()//trả về view thông báo Bad Request
        {
            return View();
        }
        //Thực hiện thanh toán Momo

        public ActionResult PaymentMoMo()// Thanh toán MoMo HTTP GET lấy link
        {
            DonHang dh = new DonHang();
            AspNetUser kh = (AspNetUser)Session["TaiKhoan"];// ép session về kh để lấy thông tin
            Dienthoai s = new Dienthoai();
            List<GioHang> gh = Laygiohang();// lấy giỏ hàng
              //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMO";
            string accessKey = "F8BBA842ECF85";
            string secretKey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";
            string orderInfo = "Thanh toán mua điện thoại";

            //HTTPGET chỉ hiện thông báo người dùng
            string returnUrl = "https://localhost:44381/GioHang/ReturnUrl";

            //HTTPPOST cập nhật database 
            string notifyurl = "https://localhost:44381/GioHang/NotifyUrl"; 
            string amount = gh.Sum(p => p.dThanhTien).ToString();
            string orderid = DateTime.Now.Ticks.ToString();
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            //Kiểm tra số tiền
            if(gh.Sum(p => p.dThanhTien) >= 50000000)
            {
                Notification.set_flash("Số tiền vượt quá 50 triệu!", "danger");
                return RedirectToAction("BadRequestMoMo", "GioHang");
            }

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, secretKey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }
            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }

        //Khi thanh toán xong ở cổng thanh toán Momo, Momo sẽ trả về một số thông tin, trong đó có errorCode để check thông tin thanh toán
        //errorCode = 0 : thanh toán thành công (Request.QueryString["errorCode"])
        //Tham khảo bảng mã lỗi tại: https://developers.momo.vn/#/docs/aio/?id=b%e1%ba%a3ng-m%c3%a3-l%e1%bb%97i

        public ActionResult ReturnUrl() //trả về URL GET thông báo thanh toán thành công hoặc thất bại hoặc lỗi
        {
            string param = Request.QueryString.ToString().Substring(0, Request.QueryString.ToString().IndexOf("signature") - 1);
            param = Server.UrlDecode(param);
            MoMoSecurity crypto = new MoMoSecurity();
            //string serectkey = ConfigurationManager.AppSettings["serectkey"].ToString();
            string secretkey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";
            string signature = crypto.signSHA256(param, secretkey);
            if (signature != Request["signature"].ToString())
            {
                ViewBag.message = "Thông tin request không hợp lệ";
                return RedirectToAction("BadRequestMoMo", "GioHang");
            }
            if (Request.QueryString["errorCode"].Equals("0"))
            {
                ViewBag.message = "Thanh toán thành công!";
                /*List<GioHang> lstGiohang = Laygiohang();
                Session["GioHang"] = lstGiohang;*/
                SavePayment();
                return RedirectToAction("ConfirmPaymentClient","GioHang");
            }
            else
            {
                string errorCode = Request.QueryString["errorCode"];
                ViewBag.message = "Thanh toán thất bại! Mã lỗi: " + errorCode;
                return RedirectToAction("ThanhToanThatBai", "GioHang");
            }
        }

        //POST trả về JSON trạng thái thanh toán MoMo
        public JsonResult NotifyUrl()
        {
            string param = "";
            param = "partner_code=" + Request["partner_code"] +
                "&access_key=" + Request["access_key"] +
                "&amount=" + Request["amount"] +
                "&order_id=" + Request["order_id"] +
                "&order_info=" + Request["order_info"] +
                "&order_type=" + Request["order_type"] +
                "&transaction_id=" + Request["transaction_id"] +
                "&message=" + Request["message"] +
                "&response_time=" + Request["response_time"] +
                "&status_code=" + Request["status_code"];

            param = Server.UrlDecode(param);
            MoMoSecurity crypto = new MoMoSecurity();
            string secretkey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";
            string signature = crypto.signSHA256(param, secretkey);
            //Không được phép cập nhật trạng thái đơn hàng vào Database khi đang chờ thanh toán
            //Trạng thái đơn kích nút thanh toán - Đang chờ thanh toán
            //Trang thái giao dịch thành công
            //Trạng thái giao dịch thất bại
            if (signature != Request["signature"].ToString())
            {
               
            }
            string status_code = Request["status_code"].ToString();
            if (status_code == "0")
            {
                SavePayment();
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }



        public ActionResult ReturnMoMo()
        {
            string param = Request.QueryString.ToString().Substring(0, Request.QueryString.ToString().IndexOf("signature") - 1);
            param = Server.UrlDecode(param);
            MoMoSecurity crypto = new MoMoSecurity();
            //string serectkey = ConfigurationManager.AppSettings["serectkey"].ToString();
            string secretkey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";
            string signature = crypto.signSHA256(param, secretkey);
            if (signature != Request["signature"].ToString())
            {
                return RedirectToAction("BadRequestMoMo","GioHang");
            }
            if (Request.QueryString["errorCode"].Equals("0"))
            {
                SavePayment();
                return RedirectToAction("ConfirmPaymentClient","GioHang");
            }
            else
            {
                return RedirectToAction("ThanhToanThatBai","GioHang");
            }
        }

        public ActionResult ConfirmPaymentClient()
        {

            //hiển thị thông báo cho người dùng
            return View();
        }

        public void SavePayment() // Lưu đơn hàng vào database
        {
            DonHang dh = new DonHang();
            AspNetUser kh = (AspNetUser)Session["TaiKhoan"]; // ép session về kh để lấy thông tin
            List<GioHang> gh = Laygiohang();

            dh.makh = kh.Id;
            dh.ngaydat = DateTime.Now;
            dh.giaohang = null;
            dh.thanhtoan = true;
            dh.tinhtrang = '0';

            data.DonHangs.InsertOnSubmit(dh);
            data.SubmitChanges();

            foreach (var item in gh)
            {
                ChiTietDonHang ctdh = new ChiTietDonHang();
                ctdh.madon = dh.madon;
                ctdh.madienthoai = item.madienthoai;
                ctdh.soluong = item.iSoluong;
                ctdh.dongia = (decimal)item.giaban;

                Dienthoai s = data.Dienthoais.Single(n => n.madienthoai == item.madienthoai); // Lấy sản phẩm để cập nhật số lượng tồn
                if (s.soluongton >= ctdh.soluong)
                {
                    s.soluongton -= ctdh.soluong; // Cập nhật số lượng tồn
                }
                else
                {
                    // Xử lý trường hợp không đủ hàng, ví dụ: thông báo cho người dùng, không tiếp tục giao dịch
                    Notification.set_flash("Không đủ số lượng sản phẩm trong kho!", "warning");
                    return; // Thoát khỏi phương thức nếu không đủ số lượng
                }

                data.ChiTietDonHangs.InsertOnSubmit(ctdh);
            }

            data.SubmitChanges();
            Notification.set_flash("Bạn đã thanh toán thành công!", "success");
            Session["GioHang"] = null;
        }


        /* ZaloPay Test*/
        public async Task<ActionResult> Zalopay()
        {
            DonHang dh = new DonHang();
            AspNetUser kh = (AspNetUser)Session["TaiKhoan"]; // ép kiểu session để lấy thông tin
            Dienthoai s = new Dienthoai();
            List<GioHang> gh = Laygiohang(); // lấy giỏ hàng

            // Cấu hình cho ZaloPay
            string appid = "554";
            string key1 = "8NdU5pG5R2spGHGhyO99HN1OhD8IQJBn";
            string createOrderUrl = "https://sandbox.zalopay.com.vn/v001/tpe/createorder";
            string returnUrl = "https://localhost:44381/GioHang/ZalopayCallback"; // URL này cần được cấu hình để tiếp nhận kết quả trả về

            var param = new Dictionary<string, string>
            {
                ["appid"] = appid,
                ["appuser"] = "demo",
                ["apptime"] = DateTime.Now.ToString("yyyyMMddHHmmss"),
                ["amount"] = gh.Sum(p => p.dThanhTien).ToString(), // Đảm bảo là string
                ["apptransid"] = DateTime.Now.Ticks.ToString(), // Đây đã là string
                ["embeddata"] = JsonConvert.SerializeObject(new { redirecturl = returnUrl }), // Serialize object thành string JSON
                ["item"] = "",
                ["description"] = "Thanh toan mua dien thoai",
                ["bankcode"] = "zalopayapp"
            };

            string dataString = $"{appid}|{param["apptransid"]}|{param["appuser"]}|{param["amount"]}|{param["apptime"]}|{param["embeddata"]}|{param["item"]}";
            param["mac"] = HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, dataString);

            try
            {
                // Thực hiện gọi đồng bộ tới ZaloPay
                var response = await HttpHelper.PostFormAsync(createOrderUrl, param);

                if (response != null && response.ContainsKey("orderurl") && response["orderurl"] != null)
                {
                    var orderUrl = response["orderurl"].ToString(); // Chuyển đổi an toàn sang string
                    return Redirect(orderUrl);
                }
                else
                {
                    ViewBag.Error = "Không thể tạo đơn hàng với ZaloPay hoặc URL đặt hàng không được trả về.";
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
                ViewBag.Error = ex.Message;
                return View("Error");
            }

        }

        [HttpPost]
        public ActionResult ZalopayCallback()
        {
            string key2 = "uUfsWgfLkRLzq6W2uNXTCxrfxs51auny"; // Khóa bí mật để xác minh MAC
            string receivedData = Request.Form["data"]; // Dữ liệu nhận được từ ZaloPay
            string receivedMac = Request.Form["mac"]; // MAC nhận được từ ZaloPay

            string computedMac = HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key2, receivedData); // Tính toán MAC từ dữ liệu nhận được

            if (computedMac.Equals(receivedMac, StringComparison.OrdinalIgnoreCase))
            {
                // Xử lý logic nếu MAC hợp lệ, ví dụ: cập nhật trạng thái đơn hàng, lưu thông tin giao dịch
                SavePayment(); // Giả sử SavePayment là phương thức lưu trạng thái thanh toán thành công
                return RedirectToAction("ConfirmPaymentClient", "GioHang");
            }
            else
            {
                // Xử lý nếu MAC không hợp lệ
                return RedirectToAction("ThanhToanThatBai", "GioHang");
            }
        }






        /*Thanh toán VNPAY && ZaloPay*/

        //Thanh toán VNPAY
        public ActionResult PayMent()
        {
            DonHang dh = new DonHang();
            AspNetUser kh = (AspNetUser)Session["TaiKhoan"];// ép session về kh để lấy thông tin
            Dienthoai s = new Dienthoai();
            List<GioHang> gh = Laygiohang();// lấy giỏ hàng

            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", gh.Sum(p => p.dThanhTien).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            
            vnpay.AddRequestData("vnp_BankCode", "");
            

            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", VNPAY_CS_ASPX.Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang" );
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            return Redirect(paymentUrl);
        }
        
       
        //Xác thực thanh toán VNPAY

        public ActionResult PaymentConfirm()
        {
           
    
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                //vnp_TxnRef: Ma don hang merchant gui VNPAY tai command=pay    
                //vnp_TransactionNo: Ma GD tai he thong VNPAY
                //vnp_ResponseCode:Response code from VNPAY: 00: Thanh cong, Khac 00: Xem tai lieu
                //vnp_SecureHash: HmacSHA512 cua du lieu tra ve

                long orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        //Thanh toan thanh cong
                        SavePayment();
                        return RedirectToAction("ConfirmPaymentClient", "GioHang");
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        return RedirectToAction("ThanhToanThatBai", "GioHang");
                    }
                    
                }
                else
                {
                    return RedirectToAction("ThanhToanThatBai", "GioHang");
                }
            
        }


        /* PayPal Test*/

        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            // Khởi tạo các đối tượng cần thiết cho quá trình thanh toán
            DonHang dh = new DonHang();
            AspNetUser kh = (AspNetUser)Session["TaiKhoan"]; // Lấy thông tin tài khoản từ session
            Dienthoai s = new Dienthoai();
            List<GioHang> gh = Laygiohang(); // Lấy thông tin giỏ hàng

            // Lấy đối tượng APIContext từ cấu hình PayPal
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                // Đối tượng Payer đại diện cho người thanh toán sử dụng PayPal
                // Mã PayerID sẽ được trả về khi quá trình thanh toán diễn ra hoặc khi nhấn vào nút thanh toán
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    // Phần này được thực thi trước tiên vì PayerID không tồn tại
                    // Được trả về bởi hàm tạo của lớp Payment
                    // Tạo một thanh toán mới
                    // baseURI là URL mà PayPal sẽ gửi dữ liệu trả về
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/shoppingcart/PaymentWithPayPal?";

                    // Tạo GUID để lưu trữ paymentID nhận được trong session
                    // Sẽ được sử dụng để thực hiện thanh toán
                    var guid = Convert.ToString((new Random()).Next(100000));

                    // Hàm CreatePayment cung cấp URL phê duyệt thanh toán
                    // Nơi người dùng được chuyển hướng để thanh toán qua tài khoản PayPal
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);

                    // Lấy các link trả về từ PayPal trong phản hồi của hàm Create
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            // Lưu URL chuyển hướng PayPal để người dùng thực hiện thanh toán
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    // Lưu paymentID trong session với khóa là guid
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // Hàm này thực thi sau khi nhận tất cả các tham số cần thiết cho thanh toán
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                    // Nếu thanh toán thất bại, hiển thị thông báo lỗi cho người dùng
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return RedirectToAction("ThanhToanThatBai", "GioHang");
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về trang lỗi
                return RedirectToAction("ThanhToanThatBai", "GioHang");
            }
            //Thanh toan thanh cong
            SavePayment();
            return RedirectToAction("ConfirmPaymentClient", "GioHang");
        }

        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            List<GioHang> gh = Laygiohang();

            // Giả sử tỷ giá hối đoái là 23,000 VND = 1 USD
            double exchangeRate = 23000;

            // Tính tổng tiền hàng đã quy đổi sang USD
            double total = gh.Sum(p => (p.dThanhTien / exchangeRate));

            var amount = new Amount()
            {
                currency = "USD",
                total = total.ToString("F2"), // Định dạng số thập phân
                details = new Details()
                {
                    subtotal = total.ToString("F2"),
                    shipping = "0.00", // Không tính phí vận chuyển
                    tax = "0.00" // Không tính thuế
                }
            };

            var transactionList = new List<Transaction>()
    {
        new Transaction()
        {
            description = "Purchase from your site.",
            invoice_number = DateTime.Now.Ticks.ToString(), // Sử dụng timestamp để đảm bảo mã hóa đơn duy nhất
            amount = amount,
            item_list = new ItemList() // Không cần liệt kê các sản phẩm chi tiết
        }
    };

            var payer = new Payer() { payment_method = "paypal" };
            var redirectUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };

            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirectUrls
            };

            // Tạo thanh toán sử dụng APIContext
            return this.payment.Create(apiContext);
        }



    }

}