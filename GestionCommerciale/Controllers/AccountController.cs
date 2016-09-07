using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestionCommerciale.Models;
namespace GestionCommerciale.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/
        //mokhtar
        GestionCommercialeEntity BD = new GestionCommercialeEntity();
        public ActionResult Index()
        {
            HttpCookie CurrentUserInfo = Request.Cookies["UtilisateurActuel"];
            if (CurrentUserInfo != null)
            {
                return RedirectToAction("Home", "Home");
            }
            else
            {
                ViewBag.ErrorText = TempData["ErrorText"] != null ? TempData["ErrorText"].ToString() : string.Empty;
                return View();
            }
        }
        [HttpPost]
        public ActionResult SendLogin()
        {
            string Login = Request.Params["Login"] != null ? Request.Params["Login"].ToString() : string.Empty;
            string Password = Request.Params["Password"] != null ? Request.Params["Password"].ToString() : string.Empty;
            PARAMETRES Parametrage = BD.PARAMETRES.FirstOrDefault();
            if (Parametrage.LOGIN.ToUpper() == Login.ToUpper() && Parametrage.PASSWORD == Password)
            {
                HttpCookie CurrentUserInfo = new HttpCookie("UtilisateurActuel");
                CurrentUserInfo["Login"] = Login;
                CurrentUserInfo.Expires = DateTime.Now.AddHours(8);
                Response.Cookies.Add(CurrentUserInfo);
                return RedirectToAction("Home","Home");
            }
            else
            {
                TempData["ErrorText"] = "Erreur de connexion";
                return RedirectToAction("Index");
            }
        }
    }
}
