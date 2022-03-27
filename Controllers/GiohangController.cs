using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Controllers
{
    public class GiohangController : Controller
    {
        // GET: Giohang
        public ActionResult Index()
        {
            return View();
        }
        dbQLBansachDataContext db = new dbQLBansachDataContext();

        //Lay gio hang
        public List<Giohang> Laygiohang()
        {
            List<Giohang> listGiohang = Session["Giohang"] as List<Giohang>;
            if (listGiohang == null)
            {
                //Neu gio hang chua ton tai thif tao listGiohang
                listGiohang = new List<Giohang>();
                Session["Giohang"] = listGiohang;

            }
            return listGiohang;
        }
        //Them gio hang
        public ActionResult Themgiohang(int iMasach, string strURL)
        {
            //:ay ra Session gio hang
            List<Giohang> listGiohang = Laygiohang();
            //Ktra sach nay ton tai trong session chua
            Giohang sp = listGiohang.Find(n => n.iMasach == iMasach);
            if (sp == null)
            {
                sp = new Giohang(iMasach);
                listGiohang.Add(sp);
                return Redirect(strURL);
            }
            else
            {
                sp.iSoluong++;
                return Redirect(strURL);
            }
        }
        public ActionResult Themgiohang1(int iMasach, string strURL)
        {
            //:ay ra Session gio hang
            List<Giohang> listGiohang = Laygiohang();
            //Ktra sach nay ton tai trong session chua
            Giohang sp = listGiohang.Find(n => n.iMasach == iMasach);
            if (sp == null)
            {
                sp = new Giohang(iMasach);
                listGiohang.Add(sp);
            }
            else
            {
                sp.iSoluong++;
            }
            return RedirectToAction("GIohang");
        }
        //Tinh tong so luong
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<Giohang> listGiohang = Session["Giohang"] as List<Giohang>;
            if (listGiohang != null)
            {
                iTongSoLuong = listGiohang.Sum(n => n.iSoluong);

            }
            return iTongSoLuong;
        }
        //Tinh tong tien
        private double TongTien()
        {
            double dTongTien = 0;
            List<Giohang> listGiohang = Session["Giohang"] as List<Giohang>;
            if (listGiohang != null)
            {
                dTongTien = listGiohang.Sum(n => n.dThanhtien);
            }
            return dTongTien;
        }
        //Xay dung trang Giohang
        public ActionResult Giohang()
        {
            
            if (Session["Giohang"] == null)
            {
                return RedirectToAction("index", "BookStore");
            }
            List<Giohang> listGiohang = Laygiohang();
           
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(listGiohang);
        }
        public ActionResult Voucher(string voucher)
        {
            List<Giohang> listGiohang = Laygiohang();
            if (voucher == "ABCDEFG")
            {
                int voucherFree = 20000;
                ViewBag.Tongtien = TongTien() - voucherFree;
                ViewBag.Voucher = " Mã Voucher giảm: 20.000 VNĐ";
                
            }
            else
            {
                ViewBag.Tongtien = TongTien();
                ViewBag.Voucher = "Mã Voucher không đúng";
            }
            Session["tongtien"] = ViewBag.Tongtien;
            ViewBag.Tongsoluong = TongSoLuong();
            return View(listGiohang);
        }
        public ActionResult Giohangpartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }
        public ActionResult XoaGiohang(int iMasp)
        {
            //Lay gio hang tu session
            List<Giohang> listGiohang = Laygiohang();
            //ktra sach da co trong session gio hang
            Giohang sp = listGiohang.SingleOrDefault(n => n.iMasach == iMasp);
            //Neu san pham ton tai thi co sua lai soluong
            if(sp != null)
            {
                listGiohang.RemoveAll(n => n.iMasach == iMasp);
                return RedirectToAction("Giohang");
            }
            if(listGiohang.Count == 0)
            {
                return RedirectToAction("index","BookStore");
            }
            return RedirectToAction("Gionghang");
        }
        public ActionResult Capnhatgiohang (int iMasp, FormCollection f)
        {

            //lay gio hang tu session
            List<Giohang> listGiohang = Laygiohang();
            //ktra sach co trong session gio hang
            Giohang sp = listGiohang.SingleOrDefault(n => n.iMasach == iMasp);
            //Neu ton tai thi cho sua so luong
            if(sp != null)
            {
                sp.iSoluong = int.Parse(f["inputSL"].ToString());
                if(sp.iSoluong == 0)
                {
                    listGiohang.RemoveAll(n => n.iMasach == iMasp);
                }
            }
            
            return RedirectToAction("Giohang");
        }
        [HttpGet]
        public ActionResult DatHang()
        {
            //Kiemtra dang nhap
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }
            if(Session["Giohang"] == null)
            {
                return RedirectToAction("index", "BookStore");
            }
            //Lay gio hang tu session
            List<Giohang> listGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = Session["tongtien"];

            return View(listGiohang);
        }
        public ActionResult DatHang(FormCollection collection)
        {
            //Them Don hang
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG) Session["Taikhoan"];
            List<Giohang> gh = Laygiohang();
            ddh.MaKH = kh.MaKH;
            ddh.Ngaydat = DateTime.Now;
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["Ngaygiao"]);
            ddh.Ngaygiao =DateTime.Parse(ngaygiao);
            ddh.Tinhtranggiaohang = false;
            ddh.Dathanhtoan = false;
            db.DONDATHANGs.InsertOnSubmit(ddh);
            db.SubmitChanges();
            //them chi tiet don hang
            foreach (var item in gh)
            {
                CHITIETDONTHANG ctdh = new CHITIETDONTHANG();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.Masach = item.iMasach;
                ctdh.Soluong = item.iSoluong;
                ctdh.Dongia = (decimal)item.dDongia;
                db.CHITIETDONTHANGs.InsertOnSubmit(ctdh);
            }
            db.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Xacnhandonhang", "Giohang");
        }
        public ActionResult Xacnhandonhang()
        {
            return View();
        }
    }
}