using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UrunStokTakip.Models;

namespace UrunStokTakip.Controllers
{
    [Authorize(Roles = "A")]
    public class KategoriController : Controller
    {
        // GET: Kategori
        TakipSistemiEntities db = new TakipSistemiEntities();
        public ActionResult Index()
        {
            return View(db.Kategori.Where(x=>x.Gecerli==true).ToList());
        }

        public ActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Ekle(Kategori data)
        {
            db.Kategori.Add(data);
            data.Gecerli = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Sil(int ID)
        {
            var kategori = db.Kategori.Where(x => x.KategoriID == ID).FirstOrDefault();
            kategori.Gecerli = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Guncelle(int ID)
        {
            var guncelle = db.Kategori.Where(x => x.KategoriID == ID).FirstOrDefault();
            return View(guncelle);
        }

        [HttpPost]
        public ActionResult Guncelle(Kategori data, int ID)
        {
            var guncelle = db.Kategori.Find(ID);
            guncelle.Ad = data.Ad;
            guncelle.Aciklama = data.Aciklama;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}