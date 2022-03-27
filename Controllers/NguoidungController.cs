using BookStore.Models;
using BotDetect.Web.Mvc;
using Facebook;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Controllers
{
    public class NguoidungController : Controller
    {
        // GET: Nguoidung

        //tao 1 doi tuong chua tat ca bo CSDL tu dbQLBanNhacCu
        dbQLBansachDataContext db = new dbQLBansachDataContext();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Dangky()
        {
            return View();
        }
        [HttpPost]
        [CaptchaValidationActionFilter("CaptchaCode", "dangkyCaptcha", "Mã xác nhận không đụng!")]
        public ActionResult Dangky(FormCollection collection,KHACHHANG kh)
        {
            var hoten = collection["HotenKH"];
            var tendn = collection["TenDN"];
            var matkhau = collection["Matkhau"];
            var matkhau2 = collection["Matkhau2"];
            var diachi = collection["Diachi"];
            var email = collection["Email"];
            var dienthoai = collection["Dienthoai"];
            var ngaysinh = String.Format("{0:MM/dd/yyyy}", collection["Ngaysinh"]);

            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = " Họ và Tên không được để trống";

            }
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi2"] = " Nhập tên đăng nhập";

            }
            if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi3"] = " Phải nhập mật khẩu";

            }
            if (String.IsNullOrEmpty(matkhau2))
            {
                ViewData["Loi4"] = " Phải nhập lại mật khẩu";
            }
            if (String.IsNullOrEmpty(diachi))
            {
                ViewData["Loi5"] = " Phải nhập địa chỉ";
            }
            if (String.IsNullOrEmpty(email))
            {
                ViewData["Loi6"] = " Email không được bỏ trống";

            }
            if (String.IsNullOrEmpty(dienthoai))
            {
                ViewData["Loi7"] = " Phải nhập số điện thoại";

            }
            else
            {
                //Gán giá trị cho đối tượng được tạo mới
                kh.HoTen = hoten;
                kh.Taikhoan = tendn;
                kh.Matkhau = matkhau;
                kh.DiachiKH = diachi;
                kh.Email = email;
                kh.DienthoaiKH = dienthoai;
                kh.Ngaysinh = DateTime.Parse(ngaysinh);
                db.KHACHHANGs.InsertOnSubmit(kh);
                db.SubmitChanges();
                return RedirectToAction("Dangnhap");
            }
            return this.Dangky();
        }
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }
        public ActionResult LoginFacebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                respone_type = "code",
                scope = "email",
            });
            return Redirect(loginUrl.AbsoluteUri);
        }
        public ActionResult FacebookCallback(string code)
        {

            var fb = new FacebookClient();
            dynamic result = fb.Get("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });


            var accessToken = result.access_token;
            if (!string.IsNullOrEmpty(accessToken))
            {
                fb.AccessToken = accessToken;
                dynamic me = fb.Get("me?fileds=first_name, middle_name,last_name,id,email");
                string userName = me.name;
                string id = me.id;

                var kh = new KHACHHANG();
                kh.HoTen = userName;
                kh.Taikhoan = id;
                kh.Matkhau = "12345";

                if (kh != null)
                {
                    var userDB = db.KHACHHANGs.Where(n => n.Taikhoan.Equals(kh.Taikhoan)).FirstOrDefault();
                    if(userDB != null)
                    {
                        Session["Taikhoan"] = kh;
                        ViewBag.Thongbao = "Đăng nhập thành công";
                    }
                    else
                    {
                        ViewBag.Thongbao = "Đăng nhập thành công";
                        db.KHACHHANGs.InsertOnSubmit(kh);
                        db.SubmitChanges();
                        Session["Taikhoan"] = kh;
                    }
                    
                }

            }
            return Redirect("/");

        }
        [HttpGet]

        public ActionResult Dangnhap()
        {
            return View();
        }

        public ActionResult Dangnhap(FormCollection collection)
        {
            var tendn = collection["TenDN"];
            var matkhau = collection["Matkhau"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
                {
                ViewData["Loi2"] = "Phải nhập mật khẩu:";

                }
                else
                {
                    //Gán giá trị đối tượng đc tạo mới
                    KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(n => n.Taikhoan == tendn && n.Matkhau == matkhau);
                    if (kh != null)
                    {
                        Session["Taikhoan"] = kh;
                    return Redirect("/");
                    }
                    else
                        ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            return View();
        }
        public ActionResult Dangxuat()
        {
            Session.Remove("Taikhoan");
            return Redirect("/");
        }
    }
}