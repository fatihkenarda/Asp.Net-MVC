using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UrunStokTakip.Models;

namespace UrunStokTakip.Controllers
{
    public class SepetController : Controller
    {
        TakipSistemiEntities db = new TakipSistemiEntities();
        // GET: Sepet
        public ActionResult Index(decimal? Tutar)
        {
            if (User.Identity.IsAuthenticated)
            {
                var kullaniciAdi = User.Identity.Name;
                var kullanici = db.Kullanici.FirstOrDefault(x => x.Email == kullaniciAdi);
                var model = db.Sepet.Where(x => x.KullaniciID == kullanici.KullaniciID);
                var kid = db.Sepet.FirstOrDefault(x => x.KullaniciID == kullanici.KullaniciID);
                if (model != null)
                {
                    if (kid == null)
                    {
                        ViewBag.Tutar = "Sepetinizde Ürün Bulunmamaktadır.";
                    }
                    else if(kid!=null)
                    {
                        Tutar = db.Sepet.Where(x => x.KullaniciID == kid.KullaniciID).Sum(x => x.Urun.Fiyat * x.Adet);
                        ViewBag.Tutar = "Toplam Tutar = " + Tutar + " TL";
                    }
                    return View(model);
                }
            }
            return HttpNotFound();
        }

        public ActionResult Ekle(int ID)
        {
            if (User.Identity.IsAuthenticated)
            {
                var kullaniciAdi = User.Identity.Name;
                var model = db.Kullanici.FirstOrDefault(x => x.Email == kullaniciAdi);
                var u = db.Urun.Find(ID);
                var sepet = db.Sepet.FirstOrDefault(x => x.KullaniciID == model.KullaniciID && x.UrunID == ID);
                if (model!=null)
                {
                    if (sepet!=null)
                    {
                        sepet.Adet++;
                        sepet.Fiyat = u.Fiyat * sepet.Adet;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    var s = new Sepet
                    {
                        KullaniciID = model.KullaniciID,
                        UrunID = u.UrunID,
                        Adet = 1,
                        Fiyat = u.Fiyat,
                        Tarih = DateTime.Now
                    };
                    db.Sepet.Add(s);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View();
            }
            return HttpNotFound();
        }

        public ActionResult Sil(int ID)
        {
            var sepet = db.Sepet.Where(x => x.SepetID == ID).FirstOrDefault();
            db.Sepet.Remove(sepet);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult HepsiniSil()
        {
            if (User.Identity.IsAuthenticated)
            {
                var kullaniciAdi = User.Identity.Name;
                var kullanici = db.Kullanici.FirstOrDefault(x => x.Email == kullaniciAdi);
                var model = db.Sepet.Where(x => x.KullaniciID == kullanici.KullaniciID).ToList();
                db.Sepet.RemoveRange(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return HttpNotFound();
        }

        public ActionResult SepetCount(int? count)
        {
            if (User.Identity.IsAuthenticated)
            {
                var model = db.Kullanici.FirstOrDefault(x => x.Email == User.Identity.Name);
                count = db.Sepet.Where(x => x.KullaniciID == model.KullaniciID).Count();
                ViewBag.count = count;
                if (count==0)
                {
                    ViewBag.count = "";
                }
                return PartialView();
            }
            return HttpNotFound();
        }

        public ActionResult arttir(int ID)
        {
            var model = db.Sepet.Find(ID);
            var modelUrun = db.Urun.FirstOrDefault(x=>x.UrunID==model.UrunID);
            model.Adet++;
            model.Fiyat = modelUrun.Fiyat * model.Adet;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult azalt(int ID)
        {
            var model = db.Sepet.Find(ID);
            var modelUrun = db.Urun.FirstOrDefault(x => x.UrunID == model.UrunID);
            if (model.Adet==1)
            {
                db.Sepet.Remove(model);
                db.SaveChanges();
            }
            model.Adet--;
            model.Fiyat = modelUrun.Fiyat * model.Adet;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public void AdetYaz(int id, int miktari)
        {
            var model = db.Sepet.Find(id);
            model.Adet = miktari;
            model.Fiyat = model.Fiyat * model.Adet;
            db.SaveChanges();
        }
    }
}