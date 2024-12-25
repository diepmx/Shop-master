using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Shop.Areas.Administrator.Data.message;
using Shop.EF;

namespace Shop.Areas.Administrator.Controllers
{
    public class MetaDienthoaiController : Controller
    {
        private DataModel db = new DataModel();

        // GET: Administrator/MetaDienthoai
        public ActionResult Index()
        {
            if (Session["taikhoanadmin"] == null)
            {
                return RedirectToAction("Error401", "MainPage");
            }
            else
            {
                var metaDienthoais = db.MetaDienthoais.Include(m => m.Dienthoai);
                return View(metaDienthoais.ToList());
            }
        }

        // GET: Administrator/MetaDienthoai/Details/5
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
                    Notification.set_flash("Không tìm thấy META-DIENTHOAI !", "warning");
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                MetaDienthoai metaDienthoai = db.MetaDienthoais.Find(id);
                if (metaDienthoai == null)
                {
                    Notification.set_flash("Không tìm thấy META-DIENTHOAI !", "warning");
                    return HttpNotFound();
                }
                return View(metaDienthoai);
            }
        }

        // GET: Administrator/MetaDienthoai/Create
        public ActionResult Create()
        {
            if (Session["taikhoanadmin"] == null)
            {
                return RedirectToAction("Error401", "MainPage");
            }
            else
            {
                ViewBag.madienthoai = new SelectList(db.Dienthoais, "madienthoai", "tendienthoai");
                return View();
            }
        }

        // POST: Administrator/MetaDienthoai/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "mameta,keymeta,valuemeta,madienthoai")] MetaDienthoai metaDienthoai)
        {
            if (Session["taikhoanadmin"] == null)
            {
                return RedirectToAction("Error401", "MainPage");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    db.MetaDienthoais.Add(metaDienthoai);
                    db.SaveChanges();
                    Notification.set_flash("Thêm mới META-DIENTHOAI thành công !", "success");
                    return RedirectToAction("Index");
                }

                ViewBag.madienthoai = new SelectList(db.Dienthoais, "madienthoai", "tendienthoai", metaDienthoai.madienthoai);
                return View(metaDienthoai);
            }
        }

        // GET: Administrator/MetaDienthoai/Edit/5
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
                    Notification.set_flash("Không tìm thấy META-DIENTHOAI !", "warning");
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                MetaDienthoai metaDienthoai = db.MetaDienthoais.Find(id);
                if (metaDienthoai == null)
                {
                    Notification.set_flash("Không tìm thấy META-DIENTHOAI !", "warning");
                    return HttpNotFound();
                }
                ViewBag.madienthoai = new SelectList(db.Dienthoais, "madienthoai", "tendienthoai", metaDienthoai.madienthoai);
                return View(metaDienthoai);
            }
        }

        // POST: Administrator/MetaDienthoai/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "mameta,keymeta,valuemeta,madienthoai")] MetaDienthoai metaDienthoai)
        {
            if (Session["taikhoanadmin"] == null)
            {
                return RedirectToAction("Error401", "MainPage");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    db.Entry(metaDienthoai).State = EntityState.Modified;
                    db.SaveChanges();
                    Notification.set_flash("Cập nhật META-DIENTHOAI thành công !", "success");
                    return RedirectToAction("Index");
                }
                ViewBag.madienthoai = new SelectList(db.Dienthoais, "madienthoai", "tendienthoai", metaDienthoai.madienthoai);
                return View(metaDienthoai);
            }
        }

        // GET: Administrator/MetaDienthoai/Delete/5
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
                    Notification.set_flash("Không tìm thấy META-DIENTHOAI !", "warning");
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                MetaDienthoai metaDienthoai = db.MetaDienthoais.Find(id);
                if (metaDienthoai == null)
                {
                    Notification.set_flash("Không tìm thấy META-DIENTHOAI !", "warning");
                    return HttpNotFound();
                }
                return View(metaDienthoai);
            }
        }

        // POST: Administrator/MetaDienthoai/Delete/5
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
                MetaDienthoai metaDienthoai = db.MetaDienthoais.Find(id);
                db.MetaDienthoais.Remove(metaDienthoai);
                db.SaveChanges();
                Notification.set_flash("Xóa META-DIENTHOAI thành công !", "success");
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
    }
}
