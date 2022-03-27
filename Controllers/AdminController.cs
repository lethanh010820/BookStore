using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.IO;
namespace BookStore.Controllers
{
    public class AdminController : Controller
    {
        dbQLBansachDataContext db = new dbQLBansachDataContext();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Sach(int ?page)
        {
            //return View(db.SACHes.ToList());
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            return View(db.SACHes.ToList().OrderBy(n => n.Masach).ToPagedList(pageNumber, pageSize));

        }
        public ActionResult QLChude(int? page)
        {
            //return View(db.SACHes.ToList());
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            return View(db.CHUDEs.ToList().OrderBy(n => n.MaCD).ToPagedList(pageNumber, pageSize));

        }
        public ActionResult QLNXB(int? page)
        {
            //return View(db.SACHes.ToList());
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            return View(db.NHAXUATBANs.ToList().OrderBy(n => n.MaNXB).ToPagedList(pageNumber, pageSize));

        }
        public ActionResult QLDonDatHang(int? page)
        {
            //return View(db.SACHes.ToList());
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            return View(db.DONDATHANGs.ToList().OrderBy(n => n.MaDonHang).ToPagedList(pageNumber, pageSize));

        }
        public ActionResult QLChitietDH(int? page)
        {
            //return View(db.SACHes.ToList());
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            return View(db.CHITIETDONTHANGs.ToList().OrderBy(n => n.MaDonHang).ToPagedList(pageNumber, pageSize));

        }
        public ActionResult QLKhachHang(int? page)
        {
            //return View(db.SACHes.ToList());
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            return View(db.KHACHHANGs.ToList().OrderBy(n => n.MaKH).ToPagedList(pageNumber, pageSize));

        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            
            var tendn = collection["username"];
            var matkhau = collection["password"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên tài khoản";

            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";

            }
            else
            {
                // Gán giá trị cho đối tượng đc tạo mới
                Admin ad = db.Admins.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);
                if (ad != null)
                {
                    //ViewBag.Thongbao = "Đăng nhập thành công";
                    Session["Tkadmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }
        [HttpGet]
        public ActionResult Themmoisach()
        {
            //Dua du lieu vào dropdownlist
            //lay ds tu table chu de va sap sep theo ten chu de, chon lay gia tri ma cd, hien thi tenchude
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n=>n.TenChuDe),"MaCD","TenChude");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB),"MaNXB","TenNXB");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Themmoisach(SACH sach,HttpPostedFileBase fileUpload)
        {
            //đưa dữ liệu vào dropdownload
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChude");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            //Kiem tra duong dan file
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "vui long chon anh bia";
                return View();
            }
            // them vao db
            else
            {
                if (ModelState.IsValid)
                {
                    // luu ten file
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/img"), fileName);
                    //kiemtra hinh anh da ton tai chua
                    if (System.IO.File.Exists(path))

                        ViewBag.Thongbao = "hình ảnh đã tồn tại";

                    else
                    {
                        //lưu hình ảnh vào đường dẫn
                        fileUpload.SaveAs(path);
                    }
                    sach.Anhbia = fileName;
                    //luu vao db
                    db.SACHes.InsertOnSubmit(sach);
                    db.SubmitChanges();
                }
                return RedirectToAction("Sach");
            }
        }
        //hien thi san pham
        public ActionResult Chitietsach(int id)
        {
            // lay sach theo ma
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
        [HttpGet]
        public ActionResult Xoasach(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
        [HttpPost, ActionName("Xoasach")]
        public ActionResult Xacnhanxoa(int id)
        {
            // lay sach theo ma id
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.SACHes.DeleteOnSubmit(sach);
            db.SubmitChanges();
            return RedirectToAction("Sach");
        }
        [HttpGet]
        public ActionResult Suasach(int id)
        {
            //lay sach theo ma id
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChude", sach.MaCD);
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", sach.MaNXB);
            return View(sach);
        }
        [HttpPost, ActionName("Suasach")]
        public ActionResult XacnhanSuasach(int id)
        {
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            UpdateModel(sach);
            db.SubmitChanges();
            return RedirectToAction("Sach");

        }
    }
}