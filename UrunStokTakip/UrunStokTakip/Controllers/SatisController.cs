using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UrunStokTakip.Models;
using PagedList;
using PagedList.Mvc;

namespace UrunStokTakip.Controllers
{
    public class SatisController : Controller
    {
        TakipSistemiEntities db = new TakipSistemiEntities();
        // GET: Satis
        public ActionResult Index(int sayfa=1)
        {
            if (User.Identity.IsAuthenticated)
            {
                var kullaniciAdi = User.Identity.Name;
                var kullanici = db.Kullanici.FirstOrDefault(x => x.Email == kullaniciAdi);
                var model = db.Satis.Where(x => x.KullaniciID == kullanici.KullaniciID).ToList().ToPagedList(sayfa, 5);
                return View(model);
            }
            return HttpNotFound();
        }

        public ActionResult SatinAl(int ID)
        {
            var model = db.Sepet.FirstOrDefault(x=>x.SepetID==ID);
            return View(model);
        }

        [HttpPost]
        public ActionResult SatinAl2(Sepet ID)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var model = db.Sepet.FirstOrDefault(x => x.SepetID==ID.SepetID);

                    var satis = new Satis
                    {
                        KullaniciID = model.KullaniciID,
                        UrunID = model.UrunID,
                        Adet = model.Adet,
                        Resim = model.Resim,
                        Fiyat = model.Fiyat,
                        Tarih = model.Tarih
                    };
                    db.Sepet.Remove(model);
                    db.Satis.Add(satis);
                    db.SaveChanges();
                    ViewBag.islem = "Satın Alma İşlemi Gerçekleşmiştir.";
                }
            }
            catch(Exception)
            {
                ViewBag.islem = "Satın Alma İşlemi Başarısız.";
            }
            return View("islem");
        }

        public ActionResult HepsiniSatinAl(decimal? Tutar)
        {
            if (User.Identity.IsAuthenticated)
            {
                var kullaniciAdi = User.Identity.Name;
                var kullanici = db.Kullanici.FirstOrDefault(x => x.Email == kullaniciAdi);
                var model = db.Sepet.Where(x => x.KullaniciID == kullanici.KullaniciID).ToList();
                var kid = db.Sepet.FirstOrDefault(x => x.KullaniciID == kullanici.KullaniciID);
                if (model != null)
                {
                    if (kid == null)
                    {
                        ViewBag.Tutar = "Sepetinizde Ürün Bulunmamaktadır.";
                    }
                    else if (kid != null)
                    {
                        Tutar = db.Sepet.Where(x => x.KullaniciID == kid.KullaniciID).Sum(x => x.Urun.Fiyat * x.Adet);
                        ViewBag.Tutar = "Toplam Tutar = " + Tutar + " TL";
                    }
                    return View(model);
                }
                return View();
            }
            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult HepsiniSatinAl2()
        {
            var kullaniciAdi = User.Identity.Name;
            var kullanici = db.Kullanici.FirstOrDefault(x => x.Email == kullaniciAdi);
            var model = db.Sepet.Where(x => x.KullaniciID == kullanici.KullaniciID).ToList();
            int satir = 0;
            foreach (var item in model)
            {
                var satis = new Satis
                {
                    KullaniciID = model[satir].KullaniciID,
                    UrunID = model[satir].UrunID,
                    Adet = model[satir].Adet,
                    Resim = model[satir].Resim,
                    Fiyat = model[satir].Fiyat,
                    Tarih = DateTime.Now
                };
                db.Satis.Add(satis);
                db.SaveChanges();
                satir++;
            }
            db.Sepet.RemoveRange(model);
            db.SaveChanges();
            return RedirectToAction("Index","Sepet");
        }
    }
}