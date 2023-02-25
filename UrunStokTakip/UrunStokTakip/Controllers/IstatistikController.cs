using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UrunStokTakip.Models;

namespace UrunStokTakip.Controllers
{
    [Authorize(Roles = "A")]
    public class IstatistikController : Controller
    {
        TakipSistemiEntities db = new TakipSistemiEntities();
        // GET: Istatistik
        public ActionResult Index()
        {
            var kategori = db.Kategori.Count();
            ViewBag.kategori = kategori;

            var urun = db.Urun.Count();
            ViewBag.urun = urun;

            var satis = db.Satis.Count();
            ViewBag.satis = satis;

            var sepet = db.Sepet.Count();
            ViewBag.sepet = sepet;

            var kullanici = db.Kullanici.Count();
            ViewBag.kullanici = kullanici;

            return View();
        }
    }
}