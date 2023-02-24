using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using UrunStokTakip.Models;

namespace UrunStokTakip.Controllers
{
    public class KullaniciController : Controller
    {
        // GET: Kullanici
        TakipSistemiEntities db = new TakipSistemiEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SifreReset()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SifreReset(string eposta)
        {
            var mail = db.Kullanici.Where(x => x.Email == eposta).SingleOrDefault();
            if (mail != null)
            {
                Random rnd = new Random();
                var yeniSifre = string.Empty;
                for (int i = 0; i < 6; i++)
                {
                    yeniSifre += rnd.Next(1,10);
                }
                mail.Sifre = yeniSifre;
                mail.SifreTekrar = mail.Sifre;
                //mail.Sifre = Crypto.Hash(Convert.ToString(yeniSifre), "MD5");
                //mail.SifreTekrar = mail.Sifre;
                db.SaveChanges();

                var sc = new SmtpClient("smtp.outlook.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("Email@hotmail.com", "password"), //Example
                    EnableSsl = true,
                };

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("Email@hotmail.com"); //Example
                mailMessage.To.Add(eposta);
                mailMessage.IsBodyHtml = true;
                mailMessage.Subject = "Giriş Şifreniz";
                mailMessage.Body = "Şifreniz: " + yeniSifre;
                sc.Send(mailMessage);
                
                ViewBag.uyari = "Şifreniz Başarıyla Gönderilmiştir.";
            }
            else
            {
                ViewBag.uyari = "Hata Oluştu Tekrar Deneyiniz.";
            }
            return View();
        }
    }
}