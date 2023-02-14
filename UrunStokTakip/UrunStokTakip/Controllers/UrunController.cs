using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using UrunStokTakip.Models;

namespace UrunStokTakip.Controllers
{
    public class UrunController : Controller
    {
        // GET: Urun
        TakipSistemiEntities db = new TakipSistemiEntities();
        [Authorize]
        public ActionResult Index(string ara)
        {
            var kAdi = User.Identity.Name;
            var list = db.Urun.ToList();
            if (!string.IsNullOrEmpty(ara))
            {
                list = list.Where(x=>x.Ad.Contains(ara) || x.Aciklama.Contains(ara)).ToList();
            }
            return View(list);
        }

        
        public ActionResult Ekle()
        {
            List<SelectListItem> deger = (from x in db.Kategori.ToList()

                                          select new SelectListItem
                                          {
                                              Text = x.Ad,
                                              Value = x.KategoriID.ToString()
                                          }).ToList();
            ViewBag.ktgr = deger;

            return View();
        }

        [Authorize(Roles = "A")]
        [HttpPost]
        public ActionResult Ekle(Urun Data, HttpPostedFileBase File)
        {
            string path = Path.Combine("~/Content/Image" + File.FileName);
            File.SaveAs(Server.MapPath(path));
            Data.Resim = File.FileName.ToString();
            db.Urun.Add(Data);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "A")]
        public ActionResult Sil(int ID)
        {
            var urun = db.Urun.Where(x => x.UrunID == ID).FirstOrDefault();
            db.Urun.Remove(urun);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "A")]
        public ActionResult Guncelle(int ID)
        {
            var guncelle = db.Urun.Where(x => x.UrunID == ID).FirstOrDefault();
            List<SelectListItem> deger = (from x in db.Kategori.ToList()

                                          select new SelectListItem
                                          {
                                              Text = x.Ad,
                                              Value = x.KategoriID.ToString()
                                          }).ToList();
            ViewBag.ktgr = deger;
            return View(guncelle);
        }
        [Authorize(Roles = "A")]
        [HttpPost]
        public ActionResult Guncelle(Urun model, HttpPostedFileBase File, int ID)
        {
            var urun = db.Urun.Find(ID);
            if (File != null)
            {
                urun.Resim = File.FileName.ToString();
                urun.Ad = model.Ad;
                urun.Aciklama = model.Aciklama;
                urun.Fiyat = model.Fiyat;
                urun.Stok = model.Stok;
                urun.Populer = model.Populer;
                urun.KategoriID = model.KategoriID;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                urun.Ad = model.Ad;
                urun.Aciklama = model.Aciklama;
                urun.Fiyat = model.Fiyat;
                urun.Stok = model.Stok;
                urun.Populer = model.Populer;
                urun.KategoriID = model.KategoriID;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        public ActionResult KritikStok()
        {
            var kritik = db.Urun.Where(x=>x.Stok<=50).ToList();
            return View(kritik);
        }

        public PartialViewResult StokCount()
        {
            if (User.Identity.IsAuthenticated)
            {
                var count = db.Urun.Where(x => x.Stok < 50).Count();
                ViewBag.count = count;
            }
            return PartialView();
        }

        public ActionResult StokGrafik()
        {
            ArrayList UrunAd = new ArrayList();
            ArrayList UrunStok = new ArrayList();
            var veriler = db.Urun.ToList();
            veriler.ToList().ForEach(x=>UrunAd.Add(x.Ad));
            veriler.ToList().ForEach(x => UrunStok.Add(x.Stok));
            var grafik = new Chart(width: 500, height:500).AddTitle("Ürün-Stok Grafiği").AddSeries(chartType:"Column", name:"Ad",xValue:UrunAd,yValues:UrunStok);
            return File(grafik.ToWebImage().GetBytes(), "image/jpeg");
        }
    }
}