using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UrunStokTakip.Models;

namespace UrunStokTakip.Controllers
{
    public class TumSatislarController : Controller
    {
        // GET: TumSatislar
        TakipSistemiEntities db = new TakipSistemiEntities();
        [Authorize(Roles ="A")]
        public ActionResult Index(int sayfa=1)
        {
            return View(db.Satis.ToList().ToPagedList(sayfa,5));
        }
    }
}