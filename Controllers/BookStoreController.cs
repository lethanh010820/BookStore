using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStore.Models;
using PagedList;

namespace BookStore.Controllers
{
    public class BookStoreController : Controller
    {

        //tao 1 doi tuong chua tat ca bo CSDL tu dbQLBanNhacCu
        dbQLBansachDataContext data = new dbQLBansachDataContext();

        public ActionResult Index(int? page)
        {

            // 1. Tham số int? dùng để thể hiện null và kiểu int
            // page có thể có giá trị là null và kiểu int.

            // 2. Nếu page = null thì đặt lại là 1.
            if (page == null) page = 1;

            // 3. Tạo truy vấn, lưu ý phải sắp xếp theo trường nào đó, ví dụ OrderBy
            // theo LinkID mới có thể phân trang.
            var links = (from l in data.SACHes
                         select l).OrderBy(x => x.Masach);

            // 4. Tạo kích thước trang (pageSize) hay là số Link hiển thị trên 1 trang
            int pageSize = 6;

            // 4.1 Toán tử ?? trong C# mô tả nếu page khác null thì lấy giá trị page, còn
            // nếu page = null thì lấy giá trị 1 cho biến pageNumber.
            int pageNumber = (page ?? 1);

            // 5. Trả về các Link được phân trang theo kích thước và số trang.
            return View(links.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Chude()
        {
            var chude = from cd in data.CHUDEs select cd;
            return PartialView(chude);
        }
        public ActionResult NhaXuatBan()
        {
            var NXB = from nxb in data.NHAXUATBANs select nxb;
            return PartialView(NXB);
        }
        public ActionResult SPTheochude (int id)
        {
            var sach = from s in data.SACHes where s.MaCD ==id select s ;
            return PartialView(sach);
        }
        public ActionResult SPTheoNXB(int id)
        {
            var sach = from s in data.SACHes where s.MaNXB == id select s;
            return PartialView(sach);
        }
        public ActionResult SachTop5sp()
        {
            var sach = from s in data.SACHes select s;
            return PartialView(sach);
        }
        public ActionResult Details(int id)
        {
            var sach = from s in data.SACHes
                       where s.Masach == id
                       select s;
            return View(sach.Single());
        }
        public ActionResult Timkiem(string searchString)
        {
            var sach = from s in data.SACHes select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                sach = sach.Where(s => s.Tensach.Contains(searchString));
            }
            return View(sach);
        }
        
    }
}