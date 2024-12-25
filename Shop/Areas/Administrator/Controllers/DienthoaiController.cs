using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using Shop.Areas.Administrator.Data.excel;
using Shop.Areas.Administrator.Data.message;
using Shop.EF;

namespace Shop.Areas.Administrator.Controllers
{
    public class DienthoaiController : Controller
    {
        private DataModel db = new DataModel();

        // GET: Administrator/Dienthoai
        public ActionResult Index()
        {
            if (Session["taikhoanadmin"] == null)
            {
                return RedirectToAction("Error401", "MainPage");
            }
            else
            {
                var dienthoais = db.Dienthoais.Include(l => l.Hang).Include(l => l.NhuCau);
                return View(dienthoais.ToList());
            }
        }

        // GET: Administrator/Dienthoai/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["taikhoanadmin"] == null)
            {
                return RedirectToAction("Error401", "MainPage");
            }
            else
            {
                if (id == null)
                {
                    Notification.set_flash("Không tìm thấy Điện thoại !", "warning");
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Dienthoai dienthoai = db.Dienthoais.Find(id);
                if (dienthoai == null)
                {
                    Notification.set_flash("Không tìm thấy Điện thoại !", "warning");
                    return HttpNotFound();
                }
                return View(dienthoai);
            }
        }

        // GET: Administrator/Dienthoai/Create
        public ActionResult Create()
        {
            if (Session["taikhoanadmin"] == null)
            {
                return RedirectToAction("Error401", "MainPage");
            }
            else
            {
                ViewBag.mahang = new SelectList(db.Hangs, "mahang", "tenhang");
                ViewBag.manhucau = new SelectList(db.NhuCaus, "manhucau", "tennhucau");
                return View();
            }
        }

        // POST: Administrator/Dienthoai/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "madienthoai,tendienthoai,giaban,mota,hinh,mahang,manhucau,camera,rom,ram,hedieuhanh,manhinh,ngaycapnhat,soluongton,pin,trangthai")] Dienthoai dienthoai)
        {
            if (Session["taikhoanadmin"] == null)
            {
                return RedirectToAction("Error401", "MainPage");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (dienthoai.tendienthoai == null || dienthoai.tendienthoai.Equals(""))
                    {
                        Notification.set_flash("Vui lòng nhập tên điện thoại!", "danger");
                        return RedirectToAction("Index");
                    }
                    if (dienthoai.giaban == null)
                    {
                        Notification.set_flash("Vui lòng nhập giá bán!", "danger");
                        return RedirectToAction("Index");
                    }
                    if (dienthoai.giaban <= 0)
                    {
                        Notification.set_flash("Giá bán điện thoại phải > 0đ!", "danger");
                        return RedirectToAction("Index");
                    }
                    if (dienthoai.soluongton < 0)
                    {
                        Notification.set_flash("Số lượng tồn phải >= 0!", "danger");
                        return RedirectToAction("Index");
                    }
                    db.Dienthoais.Add(dienthoai);
                    db.SaveChanges();
                    Notification.set_flash("Thêm mới điện thoại thành công !", "success");
                    return RedirectToAction("Index");
                }

                ViewBag.mahang = new SelectList(db.Hangs, "mahang", "tenhang", dienthoai.mahang);
                ViewBag.manhucau = new SelectList(db.NhuCaus, "manhucau", "tennhucau", dienthoai.manhucau);
                return View(dienthoai);
            }
        }

        // GET: Administrator/Dienthoai/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["taikhoanadmin"] == null)
            {
                return RedirectToAction("Error401", "MainPage");
            }
            else
            {
                if (id == null)
                {
                    Notification.set_flash("Không tìm thấy điện thoại !", "warning");
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Dienthoai dienthoai = db.Dienthoais.Find(id);
                if (dienthoai == null)
                {
                    Notification.set_flash("Không tìm thấy điện thoại !", "warning");
                    return HttpNotFound();
                }
                ViewBag.mahang = new SelectList(db.Hangs, "mahang", "tenhang", dienthoai.mahang);
                ViewBag.manhucau = new SelectList(db.NhuCaus, "manhucau", "tennhucau", dienthoai.manhucau);
                return View(dienthoai);
            }
        }

        // POST: Administrator/Dienthoai/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "madienthoai,tendienthoai,giaban,mota,hinh,mahang,manhucau,camera,rom,ram,hedieuhanh,manhinh,ngaycapnhat,soluongton,pin,trangthai")] Dienthoai dienthoai)
        {
            if (Session["taikhoanadmin"] == null)
            {
                return RedirectToAction("Error401", "MainPage");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (dienthoai.tendienthoai == null || dienthoai.tendienthoai.Equals(""))
                    {
                        Notification.set_flash("Vui lòng nhập tên điện thoại!", "danger");
                        return RedirectToAction("Index");
                    }
                    if (dienthoai.giaban == null)
                    {
                        Notification.set_flash("Vui lòng nhập giá bán!", "danger");
                        return RedirectToAction("Index");
                    }
                    if (dienthoai.giaban <= 0)
                    {
                        Notification.set_flash("Giá bán điện thoại phải > 0đ!", "danger");
                        return RedirectToAction("Index");
                    }
                    if (dienthoai.soluongton < 0)
                    {
                        Notification.set_flash("Số lượng tồn phải >= 0!", "danger");
                        return RedirectToAction("Index");
                    }
                    db.Entry(dienthoai).State = EntityState.Modified;
                    db.SaveChanges();
                    Notification.set_flash("Cập nhật điện thoại thành công !", "success");
                    return RedirectToAction("Index");
                }
                ViewBag.mahang = new SelectList(db.Hangs, "mahang", "tenhang", dienthoai.mahang);
                ViewBag.manhucau = new SelectList(db.NhuCaus, "manhucau", "tennhucau", dienthoai.manhucau);
                return View(dienthoai);
            }
        }

        // GET: Administrator/Dienthoai/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["taikhoanadmin"] == null)
            {
                return RedirectToAction("Error401", "MainPage");
            }
            else
            {
                if (id == null)
                {
                    Notification.set_flash("Không tìm thấy điện thoại !", "warning");
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Dienthoai dienthoai = db.Dienthoais.Find(id);
                if (dienthoai == null)
                {
                    Notification.set_flash("Không tìm thấy điện thoại !", "warning");
                    return HttpNotFound();
                }
                return View(dienthoai);
            }
        }

        // POST: Administrator/Dienthoai/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["taikhoanadmin"] == null)
            {
                return RedirectToAction("Error401", "MainPage");
            }
            else
            {
                Dienthoai dienthoai = db.Dienthoais.Find(id);
                db.Dienthoais.Remove(dienthoai);
                db.SaveChanges();
                Notification.set_flash("Xóa điện thoại thành công !", "success");
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public string ProcessUpload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return "";
            }
            file.SaveAs(Server.MapPath("~/Content/images/" + file.FileName));
            return file.FileName;
        }

        //EPPlus Excel
        public ActionResult GetDienthoaisFromExcel(HttpPostedFileBase fileExcel)
        {
            if (Session["taikhoanadmin"] == null)
            {
                return RedirectToAction("Error401", "MainPage");
            }
            else
            {
                List<Dienthoai> list = new List<Dienthoai>();
                if (fileExcel != null)
                {
                    try
                    {
                        using (ExcelPackage package = new ExcelPackage(fileExcel.InputStream))
                        {
                            ExcelWorkbook workbook = package.Workbook;
                            if (workbook != null)
                            {
                                ExcelWorksheet worksheet = workbook.Worksheets.FirstOrDefault();
                                if (worksheet != null)
                                {
                                    list = worksheet.ReadExcelToList<Dienthoai>();
                                    foreach (var item in list)
                                    {
                                        ViewBag.listDienthoai = item.tendienthoai;
                                    }
                                    //ViewBag.listDienthoai = list;
                                    return RedirectToAction("ExcelDonHangImport","Dienthoai");
                                    //Your code
                                }
                            }
                        }

                    }
                    catch (Exception)
                    {
                        //Save error log
                    }
                }
                return RedirectToAction("Index");
            }
        }


        public ActionResult ExcelDonHangImport()
        {
            if (Session["taikhoanadmin"] == null)
            {
                return RedirectToAction("Error401", "MainPage");
            }
            else
            {
                return View();
            }
        }

        //Làm nhập file Excel
        /// <summary>
        /// giải thích
        /// Sử dụng: SqlClient + OleDb
        /// </summary>
        /// <param name="postedFile"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ImportDonHang(HttpPostedFileBase postedFile)
        {
            if (Session["taikhoanadmin"] == null)
            {
                return RedirectToAction("Error401", "MainPage");
            }
            else
            {
                try
                {
                    string filePath = string.Empty;
                    if (postedFile != null)
                    {
                        string path = Server.MapPath("~/Upload/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                                                
                        filePath = path + Path.GetFileName(postedFile.FileName);
                        string extension = Path.GetExtension(postedFile.FileName);
                        postedFile.SaveAs(filePath);

                        string conString = string.Empty;
                        switch (extension)
                        {
                            case ".xls":
                                conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                                break;
                            case ".xlsx":
                                conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                                break;
                        }

                        DataTable dtExcel = new DataTable();
                        conString = string.Format(conString, filePath);
                        using (OleDbConnection connExcel = new OleDbConnection(conString))
                        {
                            using (OleDbCommand cmdExcel = new OleDbCommand())
                            {
                                using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                                {
                                    cmdExcel.Connection = connExcel;
                                    connExcel.Open();
                                    DataTable dtExcelSchema;
                                    dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                    string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                    connExcel.Close();

                                    connExcel.Open();
                                    cmdExcel.CommandText = "SELECT *from [" + sheetName + "]";
                                    odaExcel.SelectCommand = cmdExcel;
                                    odaExcel.Fill(dtExcel);
                                    connExcel.Close();
                                }
                            }
                        }
                        conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                        using (SqlConnection con = new SqlConnection(conString))
                        {
                            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                            {
                                sqlBulkCopy.DestinationTableName = "[dbo].[Dienthoai]";
                                sqlBulkCopy.ColumnMappings.Add("madienthoai", "madienthoai");
                                sqlBulkCopy.ColumnMappings.Add("tendienthoai", "tendienthoai");
                                sqlBulkCopy.ColumnMappings.Add("giaban", "giaban");
                                sqlBulkCopy.ColumnMappings.Add("mota", "mota");
                                sqlBulkCopy.ColumnMappings.Add("hinh", "hinh");
                                sqlBulkCopy.ColumnMappings.Add("mahang", "mahang");
                                sqlBulkCopy.ColumnMappings.Add("manhucau", "manhucau");
                                sqlBulkCopy.ColumnMappings.Add("camera", "camera");
                                sqlBulkCopy.ColumnMappings.Add("rom", "rom");
                                sqlBulkCopy.ColumnMappings.Add("ram", "ram");
                                sqlBulkCopy.ColumnMappings.Add("hedieuhanh", "hedieuhanh");
                                sqlBulkCopy.ColumnMappings.Add("manhinh", "manhinh");
                                sqlBulkCopy.ColumnMappings.Add("ngaycapnhat", "ngaycapnhat");
                                sqlBulkCopy.ColumnMappings.Add("soluongton", "soluongton");
                                sqlBulkCopy.ColumnMappings.Add("pin", "pin");
                                sqlBulkCopy.ColumnMappings.Add("trangthai", "trangthai");
                                con.Open();
                                sqlBulkCopy.WriteToServer(dtExcel);
                                con.Close();
                            }
                        }

                    }
                    Notification.set_flash("Nhập Excel điện thoại thành công!", "success");
                    return RedirectToAction("Index","Dienthoai");
                }
                catch (Exception)
                {
                    Notification.set_flash("Lỗi nhập File Excel!", "danger");
                    return RedirectToAction("Index", "Dienthoai");
                }
            }
        }


    }
}
