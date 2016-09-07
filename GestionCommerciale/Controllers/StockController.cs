using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestionCommerciale.Models;
using System.Web.Script.Serialization;
using System.Globalization;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace GestionCommerciale.Controllers
{
    public class StockController : Controller
    {
        //
        // GET: /Stock/
        GestionCommercialeEntity BD = new GestionCommercialeEntity();
        public ActionResult Index()
        {
            List<PRODUITS> ListeProduits = BD.PRODUITS.ToList();
            List<Stock> ListeStock = new List<Stock>();
            foreach (PRODUITS produit in ListeProduits)
            {

                Stock Stock = new Stock();
                Stock.ID = produit.ID.ToString();
                Stock.CODE = produit.CODE.ToString();
                Stock.LIBELLE = produit.LIBELLE.ToString();
                Stock.FAMILLE = produit.FAMILLE.ToString();
                Stock.PRIX_ACHAT_HT = produit.PRIX_ACHAT_HT.ToString();
                Stock.PRIX_VENTE_HT = produit.PRIX_VENTE_HT.ToString();
                Stock.QUANTITE = produit.QUANTITE.ToString();
                Stock.REMARQUE = string.Empty;
                if (produit.QUANTITE <= produit.QUANTITE_REPTURE_STOCK)
                {
                    Stock.REMARQUE = "REPTURE";
                }
                Stock.BLOQUE = produit.BLOQUE ? "OK" : "NO";
                ListeStock.Add(Stock);
            }
            return View(ListeStock);
        }
        public ActionResult Form(string Mode, string Code)
        {
            dynamic Result = null;
            int ID = int.Parse(Code);
            string CodeProduit = string.Empty;
            if (Mode == "Create")
            {
                Result = new PRODUITS();

                int Max = 0;
                if (BD.PRODUITS.ToList().Count != 0)
                {
                    Max = BD.PRODUITS.Select(pr => pr.ID).Max();
                }
                Max++;
                CodeProduit = "PR" + Max.ToString("0000");
                ViewBag.Mode = "Nouveau produit";
            }
            if (Mode == "Edit")
            {
                Result = BD.PRODUITS.Where(clt => clt.ID == ID).FirstOrDefault();
                CodeProduit = BD.PRODUITS.Where(clt => clt.ID == ID).FirstOrDefault().CODE;
                ViewBag.Mode = "Modifier un produit";
            }
            List<FAMILLES_PRODUITS> ListeFamille = BD.FAMILLES_PRODUITS.ToList();
            ViewBag.Code = Code;
            ViewBag.CodeProduit = CodeProduit;
            return View(Result);
        }
        public JsonResult RecupererToutFamille()
        {
            BD.Configuration.ProxyCreationEnabled = false;
            List<FAMILLES_PRODUITS> ListeFamille = BD.FAMILLES_PRODUITS.ToList();
            return Json(ListeFamille, JsonRequestBehavior.AllowGet);
        }
        public string AddFamille(string ParamPassed, string SelectedMode, string SelectedFamille)
        {
            FAMILLES_PRODUITS Famille = new FAMILLES_PRODUITS();
            if (SelectedMode == "EDIT")
            {
                int ID = int.Parse(SelectedFamille);
                Famille = BD.FAMILLES_PRODUITS.Where(fa => fa.ID == ID).FirstOrDefault();
                Famille.LIBELLE = ParamPassed;
            }
            else
            {
                Famille.LIBELLE = ParamPassed;
                BD.FAMILLES_PRODUITS.Add(Famille);
            }
            BD.SaveChanges();
            return string.Empty;
        }
        public string DeleteFamille(string ParamPassed)
        {
            int ID = int.Parse(ParamPassed);
            FAMILLES_PRODUITS Famille = BD.FAMILLES_PRODUITS.Where(fa => fa.ID == ID).FirstOrDefault();
            BD.FAMILLES_PRODUITS.Remove(Famille);
            BD.SaveChanges();
            return string.Empty;
        }
        [HttpPost]
        public ActionResult Send(string Mode, string Code)
        {
            string CODE = Request["CodeProduit"] != null ? Request["CodeProduit"].ToString() : string.Empty;
            string LIBELLE = Request["Libelle"] != null ? Request["Libelle"].ToString() : string.Empty;
            string DESIGNATION = Request["Designation"] != null ? Request["Designation"].ToString() : string.Empty;
            string FAMILLE = Request["Famille"] != null ? Request["Famille"].ToString() : string.Empty;
            string BLOQUE = Request["Bloque"] != null ? Request["Bloque"].ToString() : "false";
            string CODE_A_BARE = Request["CodeABarre"] != null ? Request["CodeABarre"].ToString() : string.Empty;
            string PRIX_ACHAT_HT = (Request["PrixAchatHT"] != null && !string.IsNullOrEmpty(Request["PrixAchatHT"].ToString())) ? Request["PrixAchatHT"].ToString() : "0";
            string PRIX_VENTE_HT = (Request["PrixVenteHT"] != null && !string.IsNullOrEmpty(Request["PrixVenteHT"].ToString())) ? Request["PrixVenteHT"].ToString() : "0";
            string AVEC_TVA = Request["WTVA"] != null ? Request["WTVA"].ToString() : string.Empty;
            string TVA = (Request["TVA"] != null && !string.IsNullOrEmpty(Request["TVA"].ToString())) ? Request["TVA"].ToString() : "0";
            string PRIX_VENTE_TTC = (Request["PrixVenteTTC"] != null && !string.IsNullOrEmpty(Request["PrixVenteTTC"].ToString())) ? Request["PrixVenteTTC"].ToString() : "0";
            string QUANTITE = (Request["Quantite"] != null && !string.IsNullOrEmpty(Request["Quantite"].ToString())) ? Request["Quantite"].ToString() : "0";
            string QUANTITE_REPTURE_STOCK = (Request["QuantiteRepture"] != null && !string.IsNullOrEmpty(Request["QuantiteRepture"].ToString())) ? Request["QuantiteRepture"].ToString() : "0";
            string OBSERVATIONS = Request["Observation"] != null ? Request["Observation"].ToString() : string.Empty;
            PRODUITS produit = new PRODUITS();
            int ID = int.Parse(Code);
            if (Mode == "Modifier un produit")
            {
                produit = BD.PRODUITS.Where(pr => pr.ID == ID).FirstOrDefault();
            }
            produit.CODE = CODE;
            produit.LIBELLE = LIBELLE;
            produit.DESIGNATION = DESIGNATION;
            produit.FAMILLE = !string.IsNullOrEmpty(FAMILLE) ? int.Parse(FAMILLE) : 0;
            produit.BLOQUE = Boolean.Parse(BLOQUE);
            produit.CODE_A_BARE = CODE_A_BARE;
            produit.PRIX_ACHAT_HT = decimal.Parse(PRIX_ACHAT_HT, CultureInfo.InvariantCulture);
            produit.PRIX_VENTE_HT = decimal.Parse(PRIX_VENTE_HT, CultureInfo.InvariantCulture);
            produit.AVEC_TVA = Boolean.Parse(AVEC_TVA);
            produit.TVA = int.Parse(TVA);
            produit.PRIX_VENTE_TTC = decimal.Parse(PRIX_VENTE_TTC, CultureInfo.InvariantCulture);
            produit.QUANTITE = int.Parse(QUANTITE);
            produit.QUANTITE_REPTURE_STOCK = int.Parse(QUANTITE_REPTURE_STOCK);
            produit.OBSERVATIONS = OBSERVATIONS;
            if (Mode == "Nouveau produit")
            {
                BD.PRODUITS.Add(produit);
            }
            BD.SaveChanges();
            ViewBag.MODE = Mode;
            return RedirectToAction("Index");
        }
        public ActionResult Mouvement(string Code)
        {
            int ID = int.Parse(Code);
            PRODUITS produit = BD.PRODUITS.Where(pr => pr.ID == ID).FirstOrDefault();
            List<MOUVEMENETS_PRODUITS> Liste = BD.MOUVEMENETS_PRODUITS.Where(mouv => mouv.PRODUIT == ID).ToList();
            ViewBag.CodeProduit = produit.CODE;
            ViewBag.LibProduit = produit.LIBELLE;
            ViewBag.CODE = Code;
            return View(Liste);
        }
        public ActionResult PrintAllMouvementProduit(string CODE)
        {
            int ID = int.Parse(CODE);
            PRODUITS produit = BD.PRODUITS.Where(pr => pr.ID == ID).FirstOrDefault();
            List<MOUVEMENETS_PRODUITS> Liste = BD.MOUVEMENETS_PRODUITS.Where(mouv => mouv.PRODUIT == ID).ToList();
            dynamic dt = from mouv in Liste
                         select new { 
                             DATE_MOUVEMENT=mouv.DATE_MOUVEMENT.ToShortDateString(),
                             TYPE_MOUVEMENT = mouv.TYPE_MOUVEMENT.Replace('_',' '),
                             CODE_MOUVEMENT = mouv.CODE_MOUVEMENT,
                             QUANTITE_MOUVEMENT = mouv.QUANTITE_MOUVEMENT,
                             QUANTITE_AVANT_MOUVEMENT = mouv.QUANTITE_AVANT_MOUVEMENT,
                             QUANTITE_APRES_MOUVEMENT = mouv.QUANTITE_APRES_MOUVEMENT,
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/ListeMouvement.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Mouvements Produit " + produit.CODE;
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
    }
    public class Stock
    {
        public string ID;
        public string CODE;
        public string LIBELLE;
        public string FAMILLE;
        public string QUANTITE;
        public string PRIX_ACHAT_HT;
        public string PRIX_VENTE_HT;
        public string REMARQUE;
        public string BLOQUE;
    }
}
