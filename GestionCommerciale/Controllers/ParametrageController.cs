using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestionCommerciale.Models;
using System.Web.Script.Serialization;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace GestionCommerciale.Controllers
{
    public class ParametrageController : Controller
    {
        //
        // GET: /Parametrage/
        GestionCommercialeEntity BD = new GestionCommercialeEntity();
        public ActionResult Societe()
        {
            SOCIETES UneSociete = BD.SOCIETES.Where(Soc => Soc.ID == 1).FirstOrDefault();
            ViewBag.Action = "Affichage";
            return View(UneSociete);

        }
        [HttpPost]
        public ActionResult Societe(string ReturnUrl)
        {
            string Nom = Request["Nom"] != null ? Request["Nom"].ToString() : string.Empty;
            string Telephone = Request["Telephone"] != null ? Request["Telephone"].ToString() : string.Empty;
            string Adresse = Request["Adresse"] != null ? Request["Adresse"].ToString() : string.Empty;
            string Fax = Request["Fax"] != null ? Request["Fax"].ToString() : string.Empty;
            string Email = Request["Email"] != null ? Request["Email"].ToString() : string.Empty;
            string SiteWeb = Request["SiteWeb"] != null ? Request["SiteWeb"].ToString() : string.Empty;
            string IdFiscal = Request["IdFiscal"] != null ? Request["IdFiscal"].ToString() : string.Empty;
            string AI = Request["AI"] != null ? Request["AI"].ToString() : string.Empty;
            string NIS = Request["NIS"] != null ? Request["NIS"].ToString() : string.Empty;
            string RC = Request["RC"] != null ? Request["RC"].ToString() : string.Empty;
            string RIB = Request["RIB"] != null ? Request["RIB"].ToString() : string.Empty;
            SOCIETES UneSociete = BD.SOCIETES.Where(Soc => Soc.ID == 1).FirstOrDefault();
            UneSociete.NOM = Nom;
            UneSociete.TELEPHONE = Telephone;
            UneSociete.ADRESSE = Adresse;
            UneSociete.FAX = Fax;
            UneSociete.EMAIL = Email;
            UneSociete.SITE_WEB = SiteWeb;
            UneSociete.ID_FISCALE = IdFiscal;
            UneSociete.AI = AI;
            UneSociete.NIS = NIS;
            UneSociete.RC = RC;
            UneSociete.RIB = RIB;
            BD.SaveChanges();
            ViewBag.Action = "Edition";
            return View(UneSociete);
        }
        public ActionResult Clients()
        {
            List<CLIENTS> Liste = BD.CLIENTS.ToList();
            return View(Liste);
        }
        public ActionResult Fournisseurs()
        {
            List<FOURNISSEURS> Liste = BD.FOURNISSEURS.ToList();
            return View(Liste);
        }
        public ActionResult Form(string Mode, string Type, string Code)
        {
            dynamic Result = null;
            int ID = int.Parse(Code);
            if (Type == "Client")
            {
                if (Mode == "Create")
                {
                    Result = new CLIENTS();
                    ViewBag.Mode = "Nouveau";
                }
                if (Mode == "Edit")
                {
                    Result = BD.CLIENTS.Where(clt => clt.ID == ID).FirstOrDefault();
                    ViewBag.Mode = "Modifier";
                }
                ViewBag.Type = "client";
                ViewBag.Title = "CLIENTS";
            }
            if (Type == "Fournisseur")
            {
                if (Mode == "Create")
                {
                    Result = new FOURNISSEURS();
                    ViewBag.Mode = "Nouveau";
                }
                if (Mode == "Edit")
                {
                    Result = BD.FOURNISSEURS.Where(clt => clt.ID == ID).FirstOrDefault();
                    ViewBag.Mode = "Modifier";
                }
                ViewBag.Type = "fournisseur";
                ViewBag.Title = "FOURNUISSEURS";
            }
            ViewBag.Code = Code;
            return View(Result);
        }
        [HttpPost]
        public ActionResult Send(string Mode, string Type, string Code)
        {
            string Nom = Request["Nom"] != null ? Request["Nom"].ToString() : string.Empty;
            string Contact = Request["Contact"] != null ? Request["Contact"].ToString() : string.Empty;
            string Telephone = Request["Telephone"] != null ? Request["Telephone"].ToString() : string.Empty;
            string Adresse = Request["Adresse"] != null ? Request["Adresse"].ToString() : string.Empty;
            string Fax = Request["Fax"] != null ? Request["Fax"].ToString() : string.Empty;
            string Email = Request["Email"] != null ? Request["Email"].ToString() : string.Empty;
            string SiteWeb = Request["SiteWeb"] != null ? Request["SiteWeb"].ToString() : string.Empty;
            string IdFiscal = Request["IdFiscal"] != null ? Request["IdFiscal"].ToString() : string.Empty;
            string AI = Request["AI"] != null ? Request["AI"].ToString() : string.Empty;
            string NIS = Request["NIS"] != null ? Request["NIS"].ToString() : string.Empty;
            string RC = Request["RC"] != null ? Request["RC"].ToString() : string.Empty;
            string RIB = Request["RIB"] != null ? Request["RIB"].ToString() : string.Empty;
            if (Type == "client")
            {
                if (Mode == "Nouveau")
                {
                    CLIENTS Client = new CLIENTS();
                    Client.NOM = Nom;
                    Client.CONTACT = Contact;
                    Client.TELEPHONE = Telephone;
                    Client.ADRESSE = Adresse;
                    Client.FAX = Fax;
                    Client.EMAIL = Email;
                    Client.SITE_WEB = SiteWeb;
                    Client.ID_FISCAL = IdFiscal;
                    Client.AI = AI;
                    Client.NIS = NIS;
                    Client.RC = RC;
                    Client.RIB = RIB;
                    int max = BD.CLIENTS.Count() != 0 ? BD.CLIENTS.Select(clt => clt.ID).Max() : 1;
                    Client.CODE = "CL" + max.ToString("0000");
                    BD.CLIENTS.Add(Client);
                    BD.SaveChanges();
                }
                if (Mode == "Modifier")
                {
                    int ID = int.Parse(Code);
                    CLIENTS Client = BD.CLIENTS.Where(clt => clt.ID == ID).FirstOrDefault();
                    Client.NOM = Nom;
                    Client.CONTACT = Contact;
                    Client.TELEPHONE = Telephone;
                    Client.ADRESSE = Adresse;
                    Client.FAX = Fax;
                    Client.EMAIL = Email;
                    Client.SITE_WEB = SiteWeb;
                    Client.ID_FISCAL = IdFiscal;
                    Client.AI = AI;
                    Client.NIS = NIS;
                    Client.RC = RC;
                    Client.RIB = RIB;
                    BD.SaveChanges();
                }
                return RedirectToAction("Clients");
            }
            if (Type == "fournisseur")
            {
                if (Mode == "Nouveau")
                {
                    FOURNISSEURS Fournisseur = new FOURNISSEURS();
                    Fournisseur.NOM = Nom;
                    Fournisseur.CONTACT = Contact;
                    Fournisseur.TELEPHONE = Telephone;
                    Fournisseur.ADRESSE = Adresse;
                    Fournisseur.FAX = Fax;
                    Fournisseur.EMAIL = Email;
                    Fournisseur.SITE_WEB = SiteWeb;
                    Fournisseur.ID_FISCAL = IdFiscal;
                    Fournisseur.AI = AI;
                    Fournisseur.NIS = NIS;
                    Fournisseur.RC = RC;
                    Fournisseur.RIB = RIB;
                    int max = BD.FOURNISSEURS.Count() != 0 ? BD.CLIENTS.Select(clt => clt.ID).Max() : 1;
                    Fournisseur.CODE = "FOU" + max.ToString("0000");
                    BD.FOURNISSEURS.Add(Fournisseur);
                    BD.SaveChanges();
                }
                if (Mode == "Modifier")
                {
                    int ID = int.Parse(Code);
                    FOURNISSEURS Fournisseur = BD.FOURNISSEURS.Where(clt => clt.ID == ID).FirstOrDefault();
                    Fournisseur.NOM = Nom;
                    Fournisseur.CONTACT = Contact;
                    Fournisseur.TELEPHONE = Telephone;
                    Fournisseur.ADRESSE = Adresse;
                    Fournisseur.FAX = Fax;
                    Fournisseur.EMAIL = Email;
                    Fournisseur.SITE_WEB = SiteWeb;
                    Fournisseur.ID_FISCAL = IdFiscal;
                    Fournisseur.AI = AI;
                    Fournisseur.NIS = NIS;
                    Fournisseur.RC = RC;
                    Fournisseur.RIB = RIB;
                    BD.SaveChanges();
                }
                return RedirectToAction("Fournisseurs");
            }
            return RedirectToAction("Form");
        }
        public ActionResult HistoriqueFournisseur(string Code)
        {
            List<Historique> listeHistorique = new List<Historique>();
            int ID = int.Parse(Code);
            FOURNISSEURS fournisseur = BD.FOURNISSEURS.Where(fou => fou.ID == ID).FirstOrDefault();
            foreach (COMMANDES_FOURNISSEURS Commande in fournisseur.COMMANDES_FOURNISSEURS)
            {
                Historique Hist = new Historique();
                Hist.date = Commande.DATE.ToShortDateString();
                Hist.type = "COMMANDE";
                Hist.code = Commande.CODE;
                Hist.remarque = string.Empty;
                Hist.ttc = Commande.TTC.ToString();
                listeHistorique.Add(Hist);
            }
            foreach (BONS_RECEPTIONS_FOURNISSEURS BonReception in fournisseur.BONS_RECEPTIONS_FOURNISSEURS)
            {
                Historique Hist = new Historique();
                Hist.date = BonReception.DATE.ToShortDateString();
                Hist.type = "BON RECEPTION";
                Hist.code = BonReception.CODE;
                Hist.remarque = "NON VALIDEE";
                if (BonReception.VALIDER)
                    Hist.remarque = "VALIDEE";
                Hist.ttc = BonReception.TTC.ToString();
                listeHistorique.Add(Hist);
            }
            foreach (FACTURES_FOURNISSEURS Facture in fournisseur.FACTURES_FOURNISSEURS)
            {
                Historique Hist = new Historique();
                Hist.date = Facture.DATE.ToShortDateString();
                Hist.type = "FACTURE";
                Hist.code = Facture.CODE;
                Hist.remarque = "NON PAYEE";
                if (Facture.PAYEE)
                    Hist.remarque = "PAYEE";
                Hist.ttc = Facture.TTC.ToString();
                listeHistorique.Add(Hist);
            }
            foreach (AVOIRS_FOURNISSEURS Avoir in fournisseur.AVOIRS_FOURNISSEURS)
            {
                Historique Hist = new Historique();
                Hist.date = Avoir.DATE.ToShortDateString();
                Hist.type = "AVOIR";
                Hist.code = Avoir.CODE;
                Hist.remarque = "NON VALIDEE";
                if(Avoir.VALIDER)
                    Hist.remarque = "VALIDEE";
                Hist.ttc = Avoir.TTC.ToString();
                listeHistorique.Add(Hist);
            }
            ViewBag.CODE = fournisseur.ID;
            ViewBag.LibFournisseur = fournisseur.NOM;
            ViewBag.CodeFournisseur = fournisseur.CODE;
            return View(listeHistorique);
        }
        public ActionResult HistoriqueClient(string Code)
        {
            List<Historique> listeHistorique = new List<Historique>();
            int ID = int.Parse(Code);
            CLIENTS client = BD.CLIENTS.Where(fou => fou.ID == ID).FirstOrDefault();
            foreach (DEVIS_CLIENTS Devis in client.DEVIS_CLIENTS)
            {
                Historique Hist = new Historique();
                Hist.date = Devis.DATE.ToShortDateString();
                Hist.type = "DEVIS";
                Hist.code = Devis.CODE;
                Hist.remarque = string.Empty;
                Hist.ttc = Devis.TTC.ToString();
                listeHistorique.Add(Hist);
            }
            foreach (COMMANDES_CLIENTS Commande in client.COMMANDES_CLIENTS)
            {
                Historique Hist = new Historique();
                Hist.date = Commande.DATE.ToShortDateString();
                Hist.type = "COMMANDE";
                Hist.code = Commande.CODE;
                Hist.remarque = string.Empty;
                Hist.ttc = Commande.TTC.ToString();
                listeHistorique.Add(Hist);
            }
            foreach (BONS_LIVRAISONS_CLIENTS BonLivraison in client.BONS_LIVRAISONS_CLIENTS)
            {
                Historique Hist = new Historique();
                Hist.date = BonLivraison.DATE.ToShortDateString();
                Hist.type = "BON LIVRAISON";
                Hist.code = BonLivraison.CODE;
                Hist.remarque = "NON VALIDEE";
                if (BonLivraison.VALIDER)
                    Hist.remarque = "VALIDEE";
                Hist.ttc = BonLivraison.TTC.ToString();
                listeHistorique.Add(Hist);
            }
            foreach (FACTURES_CLIENTS Facture in client.FACTURES_CLIENTS)
            {
                Historique Hist = new Historique();
                Hist.date = Facture.DATE.ToShortDateString();
                Hist.type = "FACTURE";
                Hist.code = Facture.CODE;
                Hist.remarque = "NON PAYEE";
                if (Facture.PAYEE)
                    Hist.remarque = "PAYEE";
                Hist.ttc = Facture.TTC.ToString();
                listeHistorique.Add(Hist);
            }
            foreach (AVOIRS_CLIENTS Avoir in client.AVOIRS_CLIENTS)
            {
                Historique Hist = new Historique();
                Hist.date = Avoir.DATE.ToShortDateString();
                Hist.type = "AVOIR";
                Hist.code = Avoir.CODE;
                Hist.remarque = "NON VALIDEE";
                if (Avoir.VALIDER)
                    Hist.remarque = "VALIDEE";
                Hist.ttc = Avoir.TTC.ToString();
                listeHistorique.Add(Hist);
            }
            ViewBag.CODE = client.ID;
            ViewBag.LibClient = client.NOM;
            ViewBag.CodeClient = client.CODE;
            return View(listeHistorique);
        }
        public ActionResult PrintHistoriqueFournisseur(string CODE)
        {
            int ID = int.Parse(CODE);
            FOURNISSEURS fournisseur = BD.FOURNISSEURS.Where(fou => fou.ID == ID).FirstOrDefault();
            List<Historique> listeHistorique = new List<Historique>();
            foreach (COMMANDES_FOURNISSEURS Commande in fournisseur.COMMANDES_FOURNISSEURS)
            {
                Historique Hist = new Historique();
                Hist.date = Commande.DATE.ToShortDateString();
                Hist.type = "COMMANDE";
                Hist.code = Commande.CODE;
                Hist.remarque = string.Empty;
                Hist.ttc = Commande.TTC.ToString();
                listeHistorique.Add(Hist);
            }
            foreach (BONS_RECEPTIONS_FOURNISSEURS BonReception in fournisseur.BONS_RECEPTIONS_FOURNISSEURS)
            {
                Historique Hist = new Historique();
                Hist.date = BonReception.DATE.ToShortDateString();
                Hist.type = "BON RECEPTION";
                Hist.code = BonReception.CODE;
                Hist.remarque = "NON VALIDEE";
                if (BonReception.VALIDER)
                    Hist.remarque = "VALIDEE";
                Hist.ttc = BonReception.TTC.ToString();
                listeHistorique.Add(Hist);
            }
            foreach (FACTURES_FOURNISSEURS Facture in fournisseur.FACTURES_FOURNISSEURS)
            {
                Historique Hist = new Historique();
                Hist.date = Facture.DATE.ToShortDateString();
                Hist.type = "FACTURE";
                Hist.code = Facture.CODE;
                Hist.remarque = "NON PAYEE";
                if (Facture.PAYEE)
                    Hist.remarque = "PAYEE";
                Hist.ttc = Facture.TTC.ToString();
                listeHistorique.Add(Hist);
            }
            foreach (AVOIRS_FOURNISSEURS Avoir in fournisseur.AVOIRS_FOURNISSEURS)
            {
                Historique Hist = new Historique();
                Hist.date = Avoir.DATE.ToShortDateString();
                Hist.type = "AVOIR";
                Hist.code = Avoir.CODE;
                Hist.remarque = "NON VALIDEE";
                if (Avoir.VALIDER)
                    Hist.remarque = "VALIDEE";
                Hist.ttc = Avoir.TTC.ToString();
                listeHistorique.Add(Hist);
            }
            dynamic dt = from mouv in listeHistorique
                         select new
                         {
                             DATE = mouv.date,
                             TYPE = mouv.type,
                             CODE = mouv.code,
                             REMARQUE = mouv.remarque,
                             TTC = mouv.ttc
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/Historique.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Historique Fournisseur " + fournisseur.CODE;
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintHistoriqueClient(string CODE)
        {
            List<Historique> listeHistorique = new List<Historique>();
            int ID = int.Parse(CODE);
            CLIENTS client = BD.CLIENTS.Where(fou => fou.ID == ID).FirstOrDefault();
            foreach (DEVIS_CLIENTS Devis in client.DEVIS_CLIENTS)
            {
                Historique Hist = new Historique();
                Hist.date = Devis.DATE.ToShortDateString();
                Hist.type = "DEVIS";
                Hist.code = Devis.CODE;
                Hist.remarque = string.Empty;
                Hist.ttc = Devis.TTC.ToString();
                listeHistorique.Add(Hist);
            }
            foreach (COMMANDES_CLIENTS Commande in client.COMMANDES_CLIENTS)
            {
                Historique Hist = new Historique();
                Hist.date = Commande.DATE.ToShortDateString();
                Hist.type = "COMMANDE";
                Hist.code = Commande.CODE;
                Hist.remarque = string.Empty;
                Hist.ttc = Commande.TTC.ToString();
                listeHistorique.Add(Hist);
            }
            foreach (BONS_LIVRAISONS_CLIENTS BonLivraison in client.BONS_LIVRAISONS_CLIENTS)
            {
                Historique Hist = new Historique();
                Hist.date = BonLivraison.DATE.ToShortDateString();
                Hist.type = "BON LIVRAISON";
                Hist.code = BonLivraison.CODE;
                Hist.remarque = "NON VALIDEE";
                if (BonLivraison.VALIDER)
                    Hist.remarque = "VALIDEE";
                Hist.ttc = BonLivraison.TTC.ToString();
                listeHistorique.Add(Hist);
            }
            foreach (FACTURES_CLIENTS Facture in client.FACTURES_CLIENTS)
            {
                Historique Hist = new Historique();
                Hist.date = Facture.DATE.ToShortDateString();
                Hist.type = "FACTURE";
                Hist.code = Facture.CODE;
                Hist.remarque = "NON PAYEE";
                if (Facture.PAYEE)
                    Hist.remarque = "PAYEE";
                Hist.ttc = Facture.TTC.ToString();
                listeHistorique.Add(Hist);
            }
            foreach (AVOIRS_CLIENTS Avoir in client.AVOIRS_CLIENTS)
            {
                Historique Hist = new Historique();
                Hist.date = Avoir.DATE.ToShortDateString();
                Hist.type = "AVOIR";
                Hist.code = Avoir.CODE;
                Hist.remarque = "NON VALIDEE";
                if (Avoir.VALIDER)
                    Hist.remarque = "VALIDEE";
                Hist.ttc = Avoir.TTC.ToString();
                listeHistorique.Add(Hist);
            }
            dynamic dt = from mouv in listeHistorique
                         select new
                         {
                             DATE = mouv.date,
                             TYPE = mouv.type,
                             CODE = mouv.code,
                             REMARQUE = mouv.remarque,
                             TTC = mouv.ttc
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/Historique.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Historique Client " + client.CODE;
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
    }
    public class Historique
    {
        public string date;
        public string type;
        public string code;
        public string remarque;
        public string ttc;
    }
}
