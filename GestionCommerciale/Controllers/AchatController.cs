using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestionCommerciale.Models;
using System.Globalization;
using System.Dynamic;
using System.ComponentModel;
using System.Web.Routing;
using System.IO;
using GestionCommerciale;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;


namespace GestionCommerciale.Controllers
{
    public class AchatController : Controller
    {
        //
        // GET: /Achat/
        GestionCommercialeEntity BD = new GestionCommercialeEntity();
        #region Views
        public ActionResult Commandes(string Mode)
        {
            List<COMMANDES_FOURNISSEURS> Liste = BD.COMMANDES_FOURNISSEURS.ToList();
            dynamic Result = (from com in Liste
                              select new
                              {
                                  ID = com.ID,
                                  CODE = com.CODE,
                                  FOURNISSEUR = BD.FOURNISSEURS.Where(fou => fou.ID == com.FOURNISSEUR).FirstOrDefault().NOM,
                                  DATE = com.DATE.ToShortDateString(),
                                  THT = com.NHT,
                                  TTVA = com.TTVA,
                                  TTC = com.TTC,
                                  TNET = com.TNET
                              }).AsEnumerable().Select(c => c.ToExpando());
            ViewData["MODE"] = Mode;
            ViewBag.MODE = Mode;
            return View(Result);
        }
        public ActionResult BonReception(string Mode)
        {
            List<BONS_RECEPTIONS_FOURNISSEURS> Liste = BD.BONS_RECEPTIONS_FOURNISSEURS.ToList();
            dynamic Result = (from com in Liste
                              select new
                              {
                                  ID = com.ID,
                                  CODE = com.CODE,
                                  FOURNISSEUR = BD.FOURNISSEURS.Where(fou => fou.ID == com.FOURNISSEUR).FirstOrDefault().NOM,
                                  DATE = com.DATE.ToShortDateString(),
                                  THT = com.NHT,
                                  TTVA = com.TTVA,
                                  TTC = com.TTC,
                                  TNET = com.TNET,
                                  VALIDE = com.VALIDER,
                              }).AsEnumerable().Select(c => c.ToExpando());
            ViewData["MODE"] = Mode;
            ViewBag.MODE = Mode;
            return View(Result);
        }
        public ActionResult Facture(string Mode)
        {
            List<FACTURES_FOURNISSEURS> Liste = BD.FACTURES_FOURNISSEURS.ToList();
            dynamic Result = (from Fact in Liste
                              select new
                              {
                                  ID = Fact.ID,
                                  CODE = Fact.CODE,
                                  FOURNISSEUR = BD.FOURNISSEURS.Where(fou => fou.ID == Fact.FOURNISSEUR).FirstOrDefault().NOM,
                                  DATE = Fact.DATE.ToShortDateString(),
                                  THT = Fact.NHT,
                                  TTVA = Fact.TTVA,
                                  TTC = Fact.TTC,
                                  TNET = Fact.TNET,
                                  VALIDE = Fact.VALIDER,
                                  PAYEE = Fact.PAYEE
                              }).AsEnumerable().Select(c => c.ToExpando());
            ViewData["MODE"] = Mode;
            ViewBag.MODE = Mode;
            return View(Result);
        }
        public ActionResult Avoir(string Mode)
        {
            List<AVOIRS_FOURNISSEURS> Liste = BD.AVOIRS_FOURNISSEURS.ToList();
            dynamic Result = (from Fact in Liste
                              select new
                              {
                                  ID = Fact.ID,
                                  CODE = Fact.CODE,
                                  FOURNISSEUR = BD.FOURNISSEURS.Where(fou => fou.ID == Fact.FOURNISSEUR).FirstOrDefault().NOM,
                                  DATE = Fact.DATE.ToShortDateString(),
                                  THT = Fact.NHT,
                                  TTVA = Fact.TTVA,
                                  TTC = Fact.TTC,
                                  TNET = Fact.TNET,
                                  VALIDE = Fact.VALIDER,
                              }).AsEnumerable().Select(c => c.ToExpando());
            ViewData["MODE"] = Mode;
            ViewBag.MODE = Mode;
            return View(Result);
        }
        #endregion
        #region Forms
        public ActionResult FormCommande(string Mode, string Code)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            COMMANDES_FOURNISSEURS CommandeFournisseur = new COMMANDES_FOURNISSEURS();
            ViewBag.Mode = Mode;
            ViewBag.Code = Code;
            string Numero = string.Empty;
            if (Mode == "Create")
            {
                int Max = 0;
                if (BD.COMMANDES_FOURNISSEURS.ToList().Count != 0)
                {
                    Max = BD.COMMANDES_FOURNISSEURS.Where(cmd => cmd.DATE.Year == DateTime.Today.Year).Select(cmd => cmd.ID).Count();
                }
                Max++;
                Numero = "CDF" + Max.ToString("0000") + "/" + DateTime.Today.ToString("yy");
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                CommandeFournisseur = BD.COMMANDES_FOURNISSEURS.Where(cmd => cmd.ID == ID).FirstOrDefault();
                Numero = CommandeFournisseur.CODE;
                List<LIGNES_COMMANDES_FOURNISSEURS> ListeLigne = BD.LIGNES_COMMANDES_FOURNISSEURS.Where(lcmd => lcmd.COMMANDE_FOURNISSEUR == ID).ToList();
                foreach (LIGNES_COMMANDES_FOURNISSEURS ligne in ListeLigne)
                {
                    LigneProduit NewLine = new LigneProduit();
                    NewLine.ID = (int)ligne.PRODUIT;
                    NewLine.LIBELLE = BD.PRODUITS.Where(pr => pr.ID == NewLine.ID).FirstOrDefault().LIBELLE;
                    NewLine.DESIGNATION = ligne.DESIGNATION_PRODUIT;
                    NewLine.QUANTITE = (int)ligne.QUANTITE;
                    NewLine.PRIX_VENTE_HT = (decimal)ligne.PRIX_UNITAIRE_HT;
                    NewLine.REMISE = (int)ligne.REMISE;
                    NewLine.PTHT = (decimal)ligne.TOTALE_HT;
                    NewLine.TVA = (int)ligne.TVA;
                    NewLine.TTC = (decimal)ligne.TOTALE_TTC;
                    ListeDesPoduits.Add(NewLine);
                }
                ViewBag.CODE_FOURNISSEUR = CommandeFournisseur.FOURNISSEURS.CODE;
            }
            Session["ProduitsCommandeFournisseur"] = ListeDesPoduits;
            ViewBag.Numero = Numero;
            return View(CommandeFournisseur);
        }
        public ActionResult FormBonReception(string Mode, string Code)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            BONS_RECEPTIONS_FOURNISSEURS BonReceptionFournisseur = new BONS_RECEPTIONS_FOURNISSEURS();
            ViewBag.Mode = Mode;
            ViewBag.Code = Code;
            string Numero = string.Empty;
            if (Mode == "Create")
            {
                int Max = 0;
                if (BD.BONS_RECEPTIONS_FOURNISSEURS.ToList().Count != 0)
                {
                    Max = BD.BONS_RECEPTIONS_FOURNISSEURS.Where(cmd => cmd.DATE.Year == DateTime.Today.Year).Select(cmd => cmd.ID).Count();
                }
                Max++;
                Numero = "BRF" + Max.ToString("0000") + "/" + DateTime.Today.ToString("yy");
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                BonReceptionFournisseur = BD.BONS_RECEPTIONS_FOURNISSEURS.Where(cmd => cmd.ID == ID).FirstOrDefault();
                Numero = BonReceptionFournisseur.CODE;
                List<LIGNES_BONS_RECEPTIONS_FOURNISSEURS> ListeLigne = BD.LIGNES_BONS_RECEPTIONS_FOURNISSEURS.Where(lcmd => lcmd.BON_RECEPTION_FOURNISSEUR == ID).ToList();
                foreach (LIGNES_BONS_RECEPTIONS_FOURNISSEURS ligne in ListeLigne)
                {
                    LigneProduit NewLine = new LigneProduit();
                    NewLine.ID = (int)ligne.PRODUIT;
                    NewLine.LIBELLE = BD.PRODUITS.Where(pr => pr.ID == NewLine.ID).FirstOrDefault().LIBELLE;
                    NewLine.DESIGNATION = ligne.DESIGNATION_PRODUIT;
                    NewLine.QUANTITE = (int)ligne.QUANTITE;
                    NewLine.PRIX_VENTE_HT = (decimal)ligne.PRIX_UNITAIRE_HT;
                    NewLine.REMISE = (int)ligne.REMISE;
                    NewLine.PTHT = (decimal)ligne.TOTALE_HT;
                    NewLine.TVA = (int)ligne.TVA;
                    NewLine.TTC = (decimal)ligne.TOTALE_TTC;
                    ListeDesPoduits.Add(NewLine);
                }
                ViewBag.CODE_FOURNISSEUR = BonReceptionFournisseur.FOURNISSEURS.CODE;
            }
            Session["ProduitsBonCommandeFournisseur"] = ListeDesPoduits;
            ViewBag.Numero = Numero;
            return View(BonReceptionFournisseur);
        }
        public ActionResult FormFacture(string Mode, string Code)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            FACTURES_FOURNISSEURS FactureFournisseur = new FACTURES_FOURNISSEURS();
            ViewBag.Mode = Mode;
            ViewBag.Code = Code;
            string Numero = string.Empty;
            if (Mode == "Create")
            {
                int Max = 0;
                if (BD.FACTURES_FOURNISSEURS.ToList().Count != 0)
                {
                    Max = BD.FACTURES_FOURNISSEURS.Where(cmd => cmd.DATE.Year == DateTime.Today.Year).Select(cmd => cmd.ID).Count();
                }
                Max++;
                Numero = "FF" + Max.ToString("0000") + "/" + DateTime.Today.ToString("yy");
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                FactureFournisseur = BD.FACTURES_FOURNISSEURS.Where(cmd => cmd.ID == ID).FirstOrDefault();
                Numero = FactureFournisseur.CODE;
                List<LIGNES_FACTURES_FOURNISSEURS> ListeLigne = BD.LIGNES_FACTURES_FOURNISSEURS.Where(lcmd => lcmd.FACTURE_FOURNISSEUR == ID).ToList();
                foreach (LIGNES_FACTURES_FOURNISSEURS ligne in ListeLigne)
                {
                    LigneProduit NewLine = new LigneProduit();
                    NewLine.ID = (int)ligne.PRODUIT;
                    NewLine.LIBELLE = BD.PRODUITS.Where(pr => pr.ID == NewLine.ID).FirstOrDefault().LIBELLE;
                    NewLine.DESIGNATION = ligne.DESIGNATION_PRODUIT;
                    NewLine.QUANTITE = (int)ligne.QUANTITE;
                    NewLine.PRIX_VENTE_HT = (decimal)ligne.PRIX_UNITAIRE_HT;
                    NewLine.REMISE = (int)ligne.REMISE;
                    NewLine.PTHT = (decimal)ligne.TOTALE_HT;
                    NewLine.TVA = (int)ligne.TVA;
                    NewLine.TTC = (decimal)ligne.TOTALE_TTC;
                    ListeDesPoduits.Add(NewLine);
                }
                ViewBag.CODE_FOURNISSEUR = FactureFournisseur.FOURNISSEURS.CODE;
            }
            Session["ProduitsFactureFournisseur"] = ListeDesPoduits;
            ViewBag.Numero = Numero;
            return View(FactureFournisseur);
        }
        public ActionResult FormAvoir(string Mode, string Code)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            AVOIRS_FOURNISSEURS AvoirFournisseur = new AVOIRS_FOURNISSEURS();
            ViewBag.Mode = Mode;
            ViewBag.Code = Code;
            string Numero = string.Empty;
            if (Mode == "Create")
            {
                int Max = 0;
                if (BD.AVOIRS_FOURNISSEURS.ToList().Count != 0)
                {
                    Max = BD.AVOIRS_FOURNISSEURS.Where(cmd => cmd.DATE.Year == DateTime.Today.Year).Select(cmd => cmd.ID).Count();
                }
                Max++;
                Numero = "AVF" + Max.ToString("0000") + "/" + DateTime.Today.ToString("yy");
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                AvoirFournisseur = BD.AVOIRS_FOURNISSEURS.Where(cmd => cmd.ID == ID).FirstOrDefault();
                Numero = AvoirFournisseur.CODE;
                List<LIGNES_AVOIRS_FOURNISSEURS> ListeLigne = BD.LIGNES_AVOIRS_FOURNISSEURS.Where(lcmd => lcmd.AVOIR_FOURNISSEUR == ID).ToList();
                foreach (LIGNES_AVOIRS_FOURNISSEURS ligne in ListeLigne)
                {
                    LigneProduit NewLine = new LigneProduit();
                    NewLine.ID = (int)ligne.PRODUIT;
                    NewLine.LIBELLE = BD.PRODUITS.Where(pr => pr.ID == NewLine.ID).FirstOrDefault().LIBELLE;
                    NewLine.DESIGNATION = ligne.DESIGNATION_PRODUIT;
                    NewLine.QUANTITE = (int)ligne.QUANTITE;
                    NewLine.PRIX_VENTE_HT = (decimal)ligne.PRIX_UNITAIRE_HT;
                    NewLine.REMISE = (int)ligne.REMISE;
                    NewLine.PTHT = (decimal)ligne.TOTALE_HT;
                    NewLine.TVA = (int)ligne.TVA;
                    NewLine.TTC = (decimal)ligne.TOTALE_TTC;
                    ListeDesPoduits.Add(NewLine);
                }
                ViewBag.CODE_FOURNISSEUR = AvoirFournisseur.FOURNISSEURS.CODE;
            }
            Session["ProduitsAvoirFournisseur"] = ListeDesPoduits;
            ViewBag.Numero = Numero;
            return View(AvoirFournisseur);
        }
        #endregion
        #region common functions
        public JsonResult GetAllProduct()
        {
            BD.Configuration.ProxyCreationEnabled = false;
            List<PRODUITS> ListeProduit = BD.PRODUITS.ToList();
            return Json(ListeProduit, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllFournisseur()
        {
            BD.Configuration.ProxyCreationEnabled = false;
            List<FOURNISSEURS> Listefournisseur = BD.FOURNISSEURS.ToList();
            return Json(Listefournisseur, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductByID(string ID)
        {
            BD.Configuration.ProxyCreationEnabled = false;
            int id = int.Parse(ID);
            PRODUITS produit = BD.PRODUITS.Where(pr => pr.ID == id).FirstOrDefault();
            return Json(produit, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFournisseuryID(string ID)
        {
            BD.Configuration.ProxyCreationEnabled = false;
            int id = int.Parse(ID);
            FOURNISSEURS fournisseur = BD.FOURNISSEURS.Where(pr => pr.ID == id).FirstOrDefault();
            return Json(fournisseur, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Print
        public ActionResult PrintCommandeClientByID(string CODE)
        {
            int ID = int.Parse(CODE);
            ConvertisseurChiffresLettres convert = new ConvertisseurChiffresLettres();
            COMMANDES_FOURNISSEURS UneCommande = BD.COMMANDES_FOURNISSEURS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            List<LIGNES_COMMANDES_FOURNISSEURS> Liste = BD.LIGNES_COMMANDES_FOURNISSEURS.Where(lcmd => lcmd.COMMANDE_FOURNISSEUR == ID).ToList();
            dynamic dt = from cmd in Liste
                         select new
                         {
                             Expr1 = UneCommande.CODE,
                             DATE = UneCommande.DATE.ToShortDateString(),
                             CODE = UneCommande.FOURNISSEURS.CODE,
                             NOM = UneCommande.FOURNISSEURS.NOM,
                             CODE_PRODUIT = cmd.CODE_PRODUIT,
                             DESIGNATION_PRODUIT = cmd.DESIGNATION_PRODUIT,
                             QUANTITE = cmd.QUANTITE ?? 0,
                             PRIX_UNITAIRE_HT = cmd.PRIX_UNITAIRE_HT ?? 0,
                             Expr2 = cmd.REMISE ?? 0,
                             TOTALE_HT = cmd.TOTALE_HT ?? 0,
                             TVA = cmd.TVA ?? 0,
                             TOTALE_TTC = cmd.TOTALE_TTC ?? 0,
                             MODE_PAIEMENT = UneCommande.MODE_PAIEMENT,
                             THT = UneCommande.THT,
                             TTVA = UneCommande.TTVA,
                             TTC = UneCommande.TTC,
                             NHT = UneCommande.NHT,
                             TNET = UneCommande.TNET,
                             REMISE = UneCommande.REMISE,
                             CHIFFRE = convert.NumberToCurrencyText(UneCommande.TNET.ToString()),
                             RC = BD.SOCIETES.Where(soc => soc.ID == 1).FirstOrDefault().RC,
                             CTVA = BD.SOCIETES.Where(soc => soc.ID == 1).FirstOrDefault().ID_FISCALE
                         };

            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/CommandeFournisseur.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Commandes Fournisseurs";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintBonReceptionClientByID(string CODE)
        {
            int ID = int.Parse(CODE);
            ConvertisseurChiffresLettres convert = new ConvertisseurChiffresLettres();
            BONS_RECEPTIONS_FOURNISSEURS UneCommande = BD.BONS_RECEPTIONS_FOURNISSEURS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            List<LIGNES_BONS_RECEPTIONS_FOURNISSEURS> Liste = BD.LIGNES_BONS_RECEPTIONS_FOURNISSEURS.Where(lcmd => lcmd.BON_RECEPTION_FOURNISSEUR == ID).ToList();
            dynamic dt = from cmd in Liste
                         select new
                         {
                             Expr1 = UneCommande.CODE,
                             DATE = UneCommande.DATE.ToShortDateString(),
                             CODE = UneCommande.FOURNISSEURS.CODE,
                             NOM = UneCommande.FOURNISSEURS.NOM,
                             CODE_PRODUIT = cmd.CODE_PRODUIT,
                             DESIGNATION_PRODUIT = cmd.DESIGNATION_PRODUIT,
                             QUANTITE = cmd.QUANTITE ?? 0,
                             PRIX_UNITAIRE_HT = cmd.PRIX_UNITAIRE_HT ?? 0,
                             Expr2 = cmd.REMISE ?? 0,
                             TOTALE_HT = cmd.TOTALE_HT ?? 0,
                             TVA = cmd.TVA ?? 0,
                             TOTALE_TTC = cmd.TOTALE_TTC ?? 0,
                             MODE_PAIEMENT = UneCommande.MODE_PAIEMENT,
                             THT = UneCommande.THT,
                             TTVA = UneCommande.TTVA,
                             TTC = UneCommande.TTC,
                             NHT = UneCommande.NHT,
                             TNET = UneCommande.TNET,
                             REMISE = UneCommande.REMISE,
                             CHIFFRE = convert.NumberToCurrencyText(UneCommande.TNET.ToString()),
                             RC = BD.SOCIETES.Where(soc => soc.ID == 1).FirstOrDefault().RC,
                             CTVA = BD.SOCIETES.Where(soc => soc.ID == 1).FirstOrDefault().ID_FISCALE
                         };

            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/BonReceptionFournisseur.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Bons Reception Fournisseurs";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintBonFactureByID(string CODE)
        {
            int ID = int.Parse(CODE);
            ConvertisseurChiffresLettres convert = new ConvertisseurChiffresLettres();
            FACTURES_FOURNISSEURS UneFacture = BD.FACTURES_FOURNISSEURS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            List<LIGNES_FACTURES_FOURNISSEURS> Liste = BD.LIGNES_FACTURES_FOURNISSEURS.Where(lcmd => lcmd.FACTURE_FOURNISSEUR == ID).ToList();
            dynamic dt = from cmd in Liste
                         select new
                         {
                             Expr1 = UneFacture.CODE,
                             DATE = UneFacture.DATE.ToShortDateString(),
                             CODE = UneFacture.FOURNISSEURS.CODE,
                             NOM = UneFacture.FOURNISSEURS.NOM,
                             CODE_PRODUIT = cmd.CODE_PRODUIT,
                             DESIGNATION_PRODUIT = cmd.DESIGNATION_PRODUIT,
                             QUANTITE = cmd.QUANTITE ?? 0,
                             PRIX_UNITAIRE_HT = cmd.PRIX_UNITAIRE_HT ?? 0,
                             Expr2 = cmd.REMISE ?? 0,
                             TOTALE_HT = cmd.TOTALE_HT ?? 0,
                             TVA = cmd.TVA ?? 0,
                             TOTALE_TTC = cmd.TOTALE_TTC ?? 0,
                             MODE_PAIEMENT = UneFacture.MODE_PAIEMENT,
                             THT = UneFacture.THT,
                             TTVA = UneFacture.TTVA,
                             TTC = UneFacture.TTC,
                             NHT = UneFacture.NHT,
                             TNET = UneFacture.TNET,
                             REMISE = UneFacture.REMISE,
                             CHIFFRE = convert.NumberToCurrencyText(UneFacture.TNET.ToString()),
                             RC = BD.SOCIETES.Where(soc => soc.ID == 1).FirstOrDefault().RC,
                             CTVA = BD.SOCIETES.Where(soc => soc.ID == 1).FirstOrDefault().ID_FISCALE,
                             TIMBRE = UneFacture.TIMBRE
                         };

            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/FactureFournisseur.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Facture Fournisseurs";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintAvoirByID(string CODE)
        {
            int ID = int.Parse(CODE);
            ConvertisseurChiffresLettres convert = new ConvertisseurChiffresLettres();
            AVOIRS_FOURNISSEURS UnAvoir = BD.AVOIRS_FOURNISSEURS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            List<LIGNES_AVOIRS_FOURNISSEURS> Liste = BD.LIGNES_AVOIRS_FOURNISSEURS.Where(lcmd => lcmd.AVOIR_FOURNISSEUR == ID).ToList();
            dynamic dt = from cmd in Liste
                         select new
                         {
                             Expr1 = UnAvoir.CODE,
                             DATE = UnAvoir.DATE.ToShortDateString(),
                             CODE = UnAvoir.FOURNISSEURS.CODE,
                             NOM = UnAvoir.FOURNISSEURS.NOM,
                             CODE_PRODUIT = cmd.CODE_PRODUIT,
                             DESIGNATION_PRODUIT = cmd.DESIGNATION_PRODUIT,
                             QUANTITE = cmd.QUANTITE ?? 0,
                             PRIX_UNITAIRE_HT = cmd.PRIX_UNITAIRE_HT ?? 0,
                             Expr2 = cmd.REMISE ?? 0,
                             TOTALE_HT = cmd.TOTALE_HT ?? 0,
                             TVA = cmd.TVA ?? 0,
                             TOTALE_TTC = cmd.TOTALE_TTC ?? 0,
                             MODE_PAIEMENT = UnAvoir.MODE_PAIEMENT,
                             THT = UnAvoir.THT,
                             TTVA = UnAvoir.TTVA,
                             TTC = UnAvoir.TTC,
                             NHT = UnAvoir.NHT,
                             TNET = UnAvoir.TNET,
                             REMISE = UnAvoir.REMISE,
                             CHIFFRE = convert.NumberToCurrencyText(UnAvoir.TNET.ToString()),
                             RC = BD.SOCIETES.Where(soc => soc.ID == 1).FirstOrDefault().RC,
                             CTVA = BD.SOCIETES.Where(soc => soc.ID == 1).FirstOrDefault().ID_FISCALE,
                         };

            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/AvoirFournisseur.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Avoirs Fournisseurs";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintAllCommandeFournisseur()
        {
            List<COMMANDES_FOURNISSEURS> Liste = BD.COMMANDES_FOURNISSEURS.ToList();
            dynamic dt = from cmd in Liste
                         select new
                         {
                             CODE = cmd.CODE,
                             FOURNISSEUR = cmd.FOURNISSEURS.NOM,
                             DATE = cmd.DATE.ToShortDateString(),
                             MODE_PAIEMENT = cmd.MODE_PAIEMENT,
                             TTC = cmd.TTC
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/CrystalReport1.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Commandes Fournisseurs";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintAllBonReceptionFournisseur()
        {
            List<BONS_RECEPTIONS_FOURNISSEURS> Liste = BD.BONS_RECEPTIONS_FOURNISSEURS.ToList();
            dynamic dt = from cmd in Liste
                         select new
                         {
                             CODE = cmd.CODE,
                             FOURNISSEUR = cmd.FOURNISSEURS.NOM,
                             DATE = cmd.DATE.ToShortDateString(),
                             VALIDER = cmd.VALIDER ? "VALIDEE" : "NON VALIDEE",
                             NET_HT = cmd.NHT,
                             T_TVA = cmd.TTVA,
                             TTC = cmd.TTC,
                             NET_A_PAYE = cmd.TNET

                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/ListeBonReceptionFournisseur.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Bons Reception Fournisseurs";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintAllFactureFournisseur()
        {
            List<FACTURES_FOURNISSEURS> Liste = BD.FACTURES_FOURNISSEURS.ToList();
            dynamic dt = from cmd in Liste
                         select new
                         {
                             CODE = cmd.CODE,
                             FOURNISSEUR = cmd.FOURNISSEURS.NOM,
                             DATE = cmd.DATE.ToShortDateString(),
                             VALIDEE = cmd.VALIDER ? "VALIDEE" : "NON VALIDEE",
                             PAYEE = cmd.PAYEE ? "PAYEE" : "NON PAYEE",
                             NET_HT = cmd.NHT,
                             T_TVA = cmd.TTVA,
                             TTC = cmd.TTC,
                             NET_A_PAYE = cmd.TNET

                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/ListeFactureFournisseur.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Factures Fournisseurs";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintAllAvoirFournisseur()
        {
            List<FACTURES_FOURNISSEURS> Liste = BD.FACTURES_FOURNISSEURS.ToList();
            dynamic dt = from cmd in Liste
                         select new
                         {
                             CODE = cmd.CODE,
                             FOURNISSEUR = cmd.FOURNISSEURS.NOM,
                             DATE = cmd.DATE.ToShortDateString(),
                             VALIDEE = cmd.VALIDER ? "VALIDEE" : "NON VALIDEE",
                             PAYEE = cmd.PAYEE ? "PAYEE" : "NON PAYEE",
                             NET_HT = cmd.NHT,
                             T_TVA = cmd.TTVA,
                             TTC = cmd.TTC,
                             NET_A_PAYE = cmd.TNET

                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/ListeAvoirFournisseur.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Avoirs Fournisseurs";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        #endregion
        #region Delete
        public string DeleteCommande(string parampassed)
        {
            int ID = int.Parse(parampassed);
            BD.LIGNES_COMMANDES_FOURNISSEURS.Where(p => p.COMMANDE_FOURNISSEUR == ID).ToList().ForEach(p => BD.LIGNES_COMMANDES_FOURNISSEURS.Remove(p));
            BD.SaveChanges();
            COMMANDES_FOURNISSEURS Commande = BD.COMMANDES_FOURNISSEURS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            BD.COMMANDES_FOURNISSEURS.Remove(Commande);
            BD.SaveChanges();
            return string.Empty;
        }
        public string DeleteBonReception(string parampassed)
        {
            int ID = int.Parse(parampassed);
            BD.LIGNES_BONS_RECEPTIONS_FOURNISSEURS.Where(p => p.BON_RECEPTION_FOURNISSEUR == ID).ToList().ForEach(p => BD.LIGNES_BONS_RECEPTIONS_FOURNISSEURS.Remove(p));
            BD.SaveChanges();
            BONS_RECEPTIONS_FOURNISSEURS BonReception = BD.BONS_RECEPTIONS_FOURNISSEURS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            BD.BONS_RECEPTIONS_FOURNISSEURS.Remove(BonReception);
            BD.SaveChanges();
            return string.Empty;
        }
        public string DeleteFacture(string parampassed)
        {
            int ID = int.Parse(parampassed);
            BD.LIGNES_FACTURES_FOURNISSEURS.Where(p => p.FACTURE_FOURNISSEUR == ID).ToList().ForEach(p => BD.LIGNES_FACTURES_FOURNISSEURS.Remove(p));
            BD.SaveChanges();
            FACTURES_FOURNISSEURS Facture = BD.FACTURES_FOURNISSEURS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            BD.FACTURES_FOURNISSEURS.Remove(Facture);
            BD.SaveChanges();
            return string.Empty;
        }
        public string DeleteAvoir(string parampassed)
        {
            int ID = int.Parse(parampassed);
            BD.LIGNES_AVOIRS_FOURNISSEURS.Where(p => p.AVOIR_FOURNISSEUR == ID).ToList().ForEach(p => BD.LIGNES_AVOIRS_FOURNISSEURS.Remove(p));
            BD.SaveChanges();
            AVOIRS_FOURNISSEURS Facture = BD.AVOIRS_FOURNISSEURS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            BD.AVOIRS_FOURNISSEURS.Remove(Facture);
            BD.SaveChanges();
            return string.Empty;
        }
        #endregion
        #region specifique fonctions
        public string validateBonRecepetion(string parampassed)
        {
            int ID = int.Parse(parampassed);
            BONS_RECEPTIONS_FOURNISSEURS BonReception = BD.BONS_RECEPTIONS_FOURNISSEURS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            BonReception.VALIDER = true;
            BD.SaveChanges();
            return string.Empty;
        }
        public string validateFacture(string parampassed)
        {
            int ID = int.Parse(parampassed);
            FACTURES_FOURNISSEURS Facture = BD.FACTURES_FOURNISSEURS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            Facture.VALIDER = true;
            BD.SaveChanges();
            return string.Empty;
        }
        public string PayeFacture(string parampassed)
        {
            int ID = int.Parse(parampassed);
            FACTURES_FOURNISSEURS Facture = BD.FACTURES_FOURNISSEURS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            if (Facture.VALIDER)
            {
                Facture.PAYEE = true;
            }
            BD.SaveChanges();
            return string.Empty;
        }
        public string validateAvoir(string parampassed)
        {
            int ID = int.Parse(parampassed);
            AVOIRS_FOURNISSEURS Facture = BD.AVOIRS_FOURNISSEURS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            Facture.VALIDER = true;
            BD.SaveChanges();
            return string.Empty;
        }
        public string CommandeVersBonReception(string parampassed)
        {
            int ID = int.Parse(parampassed);
            COMMANDES_FOURNISSEURS Element = BD.COMMANDES_FOURNISSEURS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            List<LIGNES_COMMANDES_FOURNISSEURS> Liste = BD.LIGNES_COMMANDES_FOURNISSEURS.Where(cmd => cmd.COMMANDE_FOURNISSEUR == ID).ToList();
            BONS_RECEPTIONS_FOURNISSEURS NewElement = new BONS_RECEPTIONS_FOURNISSEURS();
            string Numero = string.Empty;

            int Max = 0;
            if (BD.BONS_RECEPTIONS_FOURNISSEURS.ToList().Count != 0)
            {
                Max = BD.BONS_RECEPTIONS_FOURNISSEURS.Where(cmd => cmd.DATE.Year == DateTime.Today.Year).Select(cmd => cmd.ID).Count();
            }
            Max++;
            Numero = "BRF" + Max.ToString("0000") + "/" + DateTime.Today.ToString("yy");
            NewElement.CODE = Numero;
            NewElement.DATE = Element.DATE;
            NewElement.MODE_PAIEMENT = Element.MODE_PAIEMENT;
            NewElement.FOURNISSEUR = Element.FOURNISSEUR;
            NewElement.THT = Element.THT;
            NewElement.TTVA = Element.TTVA;
            NewElement.NHT = Element.NHT;
            NewElement.TTC = Element.TTC;
            NewElement.TNET = Element.TNET;
            NewElement.VALIDER = Element.VALIDER;
            NewElement.REMISE = Element.REMISE;
            NewElement.COMMANDE_FOURNISSEUR = Element.ID;
            NewElement.COMMANDES_FOURNISSEURS = Element;
            NewElement.FOURNISSEURS = Element.FOURNISSEURS;
            BD.BONS_RECEPTIONS_FOURNISSEURS.Add(NewElement);
            BD.SaveChanges();
            foreach (LIGNES_COMMANDES_FOURNISSEURS Ligne in Liste)
            {
                LIGNES_BONS_RECEPTIONS_FOURNISSEURS NewLine = new LIGNES_BONS_RECEPTIONS_FOURNISSEURS();
                NewLine.PRODUIT = Ligne.PRODUIT;
                NewLine.CODE_PRODUIT = Ligne.CODE_PRODUIT;
                NewLine.DESIGNATION_PRODUIT = Ligne.DESIGNATION_PRODUIT;
                NewLine.QUANTITE = Ligne.QUANTITE;
                NewLine.PRIX_UNITAIRE_HT = Ligne.PRIX_UNITAIRE_HT;
                NewLine.REMISE = Ligne.REMISE;
                NewLine.TOTALE_REMISE_HT = Ligne.TOTALE_REMISE_HT;
                NewLine.TOTALE_HT = Ligne.TOTALE_HT;
                NewLine.TVA = Ligne.TVA;
                NewLine.TOTALE_TTC = Ligne.TOTALE_TTC;
                NewLine.BON_RECEPTION_FOURNISSEUR = NewElement.ID;
                NewLine.BONS_RECEPTIONS_FOURNISSEURS = NewElement;
                NewLine.PRODUITS = Ligne.PRODUITS;
                BD.LIGNES_BONS_RECEPTIONS_FOURNISSEURS.Add(NewLine);
                BD.SaveChanges();
                AddMouvementProduit("BON_RECEPTION", NewLine.PRODUITS, NewElement.DATE, NewElement.CODE, (int)NewLine.QUANTITE);
            }
            return NewElement.ID.ToString();
        }
        public string CommandeVersfacture(string parampassed)
        {
            int ID = int.Parse(parampassed);
            COMMANDES_FOURNISSEURS Element = BD.COMMANDES_FOURNISSEURS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            List<LIGNES_COMMANDES_FOURNISSEURS> Liste = BD.LIGNES_COMMANDES_FOURNISSEURS.Where(cmd => cmd.COMMANDE_FOURNISSEUR == ID).ToList();
            FACTURES_FOURNISSEURS NewElement = new FACTURES_FOURNISSEURS();
            string Numero = string.Empty;

            int Max = 0;
            if (BD.FACTURES_FOURNISSEURS.ToList().Count != 0)
            {
                Max = BD.FACTURES_FOURNISSEURS.Where(cmd => cmd.DATE.Year == DateTime.Today.Year).Select(cmd => cmd.ID).Count();
            }
            Max++;
            Numero = "FF" + Max.ToString("0000") + "/" + DateTime.Today.ToString("yy");
            NewElement.CODE = Numero;
            NewElement.DATE = Element.DATE;
            NewElement.MODE_PAIEMENT = Element.MODE_PAIEMENT;
            NewElement.FOURNISSEUR = Element.FOURNISSEUR;
            NewElement.THT = Element.THT;
            NewElement.TTVA = Element.TTVA;
            NewElement.NHT = Element.NHT;
            NewElement.TIMBRE = decimal.Parse("0,5");
            NewElement.TTC = Element.TTC + NewElement.TIMBRE;
            NewElement.TNET = Element.TNET + NewElement.TIMBRE;
            NewElement.VALIDER = false;
            NewElement.REMISE = Element.REMISE;
            NewElement.COMMANDE_FOURNISSEUR = Element.ID;
            NewElement.COMMANDES_FOURNISSEURS = Element;
            NewElement.FOURNISSEURS = Element.FOURNISSEURS;
            BD.FACTURES_FOURNISSEURS.Add(NewElement);
            BD.SaveChanges();
            foreach (LIGNES_COMMANDES_FOURNISSEURS Ligne in Liste)
            {
                LIGNES_FACTURES_FOURNISSEURS NewLine = new LIGNES_FACTURES_FOURNISSEURS();
                NewLine.PRODUIT = Ligne.PRODUIT;
                NewLine.CODE_PRODUIT = Ligne.CODE_PRODUIT;
                NewLine.DESIGNATION_PRODUIT = Ligne.DESIGNATION_PRODUIT;
                NewLine.QUANTITE = Ligne.QUANTITE;
                NewLine.PRIX_UNITAIRE_HT = Ligne.PRIX_UNITAIRE_HT;
                NewLine.REMISE = Ligne.REMISE;
                NewLine.TOTALE_REMISE_HT = Ligne.TOTALE_REMISE_HT;
                NewLine.TOTALE_HT = Ligne.TOTALE_HT;
                NewLine.TVA = Ligne.TVA;
                NewLine.TOTALE_TTC = Ligne.TOTALE_TTC;
                NewLine.FACTURE_FOURNISSEUR = NewElement.ID;
                NewLine.FACTURES_FOURNISSEURS = NewElement;
                NewLine.PRODUITS = Ligne.PRODUITS;
                BD.LIGNES_FACTURES_FOURNISSEURS.Add(NewLine);
                BD.SaveChanges();
                AddMouvementProduit("FACTURE", NewLine.PRODUITS, NewElement.DATE, NewElement.CODE, (int)NewLine.QUANTITE);
            }
            return NewElement.ID.ToString();
        }
        public string BonReceptionVersfacture(string parampassed)
        {
            int ID = int.Parse(parampassed);
            BONS_RECEPTIONS_FOURNISSEURS Element = BD.BONS_RECEPTIONS_FOURNISSEURS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            if (Element.VALIDER)
            {
                List<LIGNES_BONS_RECEPTIONS_FOURNISSEURS> Liste = BD.LIGNES_BONS_RECEPTIONS_FOURNISSEURS.Where(cmd => cmd.BON_RECEPTION_FOURNISSEUR == ID).ToList();
                FACTURES_FOURNISSEURS NewElement = new FACTURES_FOURNISSEURS();
                string Numero = string.Empty;

                int Max = 0;
                if (BD.FACTURES_FOURNISSEURS.ToList().Count != 0)
                {
                    Max = BD.FACTURES_FOURNISSEURS.Where(cmd => cmd.DATE.Year == DateTime.Today.Year).Select(cmd => cmd.ID).Count();
                }
                Max++;
                Numero = "FF" + Max.ToString("0000") + "/" + DateTime.Today.ToString("yy");
                NewElement.CODE = Numero;
                NewElement.DATE = Element.DATE;
                NewElement.MODE_PAIEMENT = Element.MODE_PAIEMENT;
                NewElement.FOURNISSEUR = Element.FOURNISSEUR;
                NewElement.THT = Element.THT;
                NewElement.TTVA = Element.TTVA;
                NewElement.NHT = Element.NHT;
                NewElement.TIMBRE = decimal.Parse("0,5");
                NewElement.TTC = Element.TTC + NewElement.TIMBRE;
                NewElement.TNET = Element.TNET + NewElement.TIMBRE;
                NewElement.VALIDER = false;
                NewElement.REMISE = Element.REMISE;
                NewElement.BON_RECEPTION_FOURNISSEUR = Element.ID;
                NewElement.BONS_RECEPTIONS_FOURNISSEURS = Element;
                NewElement.FOURNISSEURS = Element.FOURNISSEURS;
                BD.FACTURES_FOURNISSEURS.Add(NewElement);
                BD.SaveChanges();
                foreach (LIGNES_BONS_RECEPTIONS_FOURNISSEURS Ligne in Liste)
                {
                    LIGNES_FACTURES_FOURNISSEURS NewLine = new LIGNES_FACTURES_FOURNISSEURS();
                    NewLine.PRODUIT = Ligne.PRODUIT;
                    NewLine.CODE_PRODUIT = Ligne.CODE_PRODUIT;
                    NewLine.DESIGNATION_PRODUIT = Ligne.DESIGNATION_PRODUIT;
                    NewLine.QUANTITE = Ligne.QUANTITE;
                    NewLine.PRIX_UNITAIRE_HT = Ligne.PRIX_UNITAIRE_HT;
                    NewLine.REMISE = Ligne.REMISE;
                    NewLine.TOTALE_REMISE_HT = Ligne.TOTALE_REMISE_HT;
                    NewLine.TOTALE_HT = Ligne.TOTALE_HT;
                    NewLine.TVA = Ligne.TVA;
                    NewLine.TOTALE_TTC = Ligne.TOTALE_TTC;
                    NewLine.FACTURE_FOURNISSEUR = NewElement.ID;
                    NewLine.FACTURES_FOURNISSEURS = NewElement;
                    NewLine.PRODUITS = Ligne.PRODUITS;
                    BD.LIGNES_FACTURES_FOURNISSEURS.Add(NewLine);
                    BD.SaveChanges();
                    AddMouvementProduit("FACTURE", NewLine.PRODUITS, NewElement.DATE, NewElement.CODE, (int)NewLine.QUANTITE);
                }
                return NewElement.ID.ToString();
            }
            return "NO";
        }
        public string FactureVersAvoir(string parampassed)
        {
            int ID = int.Parse(parampassed);
            FACTURES_FOURNISSEURS Element = BD.FACTURES_FOURNISSEURS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            if (Element.VALIDER)
            {
                List<LIGNES_FACTURES_FOURNISSEURS> Liste = BD.LIGNES_FACTURES_FOURNISSEURS.Where(cmd => cmd.FACTURE_FOURNISSEUR == ID).ToList();
                AVOIRS_FOURNISSEURS NewElement = new AVOIRS_FOURNISSEURS();
                string Numero = string.Empty;

                int Max = 0;
                if (BD.FACTURES_FOURNISSEURS.ToList().Count != 0)
                {
                    Max = BD.FACTURES_FOURNISSEURS.Where(cmd => cmd.DATE.Year == DateTime.Today.Year).Select(cmd => cmd.ID).Count();
                }
                Max++;
                Numero = "AVF" + Max.ToString("0000") + "/" + DateTime.Today.ToString("yy");
                NewElement.CODE = Numero;
                NewElement.DATE = Element.DATE;
                NewElement.MODE_PAIEMENT = Element.MODE_PAIEMENT;
                NewElement.FOURNISSEUR = Element.FOURNISSEUR;
                NewElement.THT = Element.THT;
                NewElement.TTVA = Element.TTVA;
                NewElement.NHT = Element.NHT;
                NewElement.TTC = Element.TTC ;
                NewElement.TNET = Element.TNET;
                NewElement.VALIDER = false;
                NewElement.REMISE = Element.REMISE;
                NewElement.FACTURE_FOURNISSEUR = Element.ID;
                NewElement.FACTURES_FOURNISSEURS = Element;
                NewElement.FOURNISSEURS = Element.FOURNISSEURS;
                BD.AVOIRS_FOURNISSEURS.Add(NewElement);
                BD.SaveChanges();
                foreach (LIGNES_FACTURES_FOURNISSEURS Ligne in Liste)
                {
                    LIGNES_AVOIRS_FOURNISSEURS NewLine = new LIGNES_AVOIRS_FOURNISSEURS();
                    NewLine.PRODUIT = Ligne.PRODUIT;
                    NewLine.CODE_PRODUIT = Ligne.CODE_PRODUIT;
                    NewLine.DESIGNATION_PRODUIT = Ligne.DESIGNATION_PRODUIT;
                    NewLine.QUANTITE = Ligne.QUANTITE;
                    NewLine.PRIX_UNITAIRE_HT = Ligne.PRIX_UNITAIRE_HT;
                    NewLine.REMISE = Ligne.REMISE;
                    NewLine.TOTALE_REMISE_HT = Ligne.TOTALE_REMISE_HT;
                    NewLine.TOTALE_HT = Ligne.TOTALE_HT;
                    NewLine.TVA = Ligne.TVA;
                    NewLine.TOTALE_TTC = Ligne.TOTALE_TTC;
                    NewLine.AVOIR_FOURNISSEUR = NewElement.ID;
                    NewLine.AVOIRS_FOURNISSEURS = NewElement;
                    NewLine.PRODUITS = Ligne.PRODUITS;
                    BD.LIGNES_AVOIRS_FOURNISSEURS.Add(NewLine);
                    BD.SaveChanges();
                    AddMouvementProduit("AVOIR", NewLine.PRODUITS, NewElement.DATE, NewElement.CODE, (int)NewLine.QUANTITE);
                }
                return NewElement.ID.ToString();
            }
            return "NO";
        }
        #endregion
        public string AddLine(string ID_Produit, string LIB_Produi, string Description_Produit, string Quantite_Produit, string PUHT_Produit, string Remise_Produit, string PTHT_Produit, string TVA_Produit, string TTC_Produit)
        {
            LigneProduit ligne = new LigneProduit();
            ligne.ID = int.Parse(ID_Produit);
            ligne.LIBELLE = LIB_Produi;
            ligne.DESIGNATION = Description_Produit;
            ligne.QUANTITE = int.Parse(Quantite_Produit);
            ligne.PRIX_VENTE_HT = decimal.Parse(PUHT_Produit, CultureInfo.InvariantCulture);
            ligne.REMISE = int.Parse(Remise_Produit);
            ligne.PTHT = decimal.Parse(PTHT_Produit, CultureInfo.InvariantCulture);
            ligne.TVA = int.Parse(TVA_Produit);
            ligne.TTC = decimal.Parse(TTC_Produit, CultureInfo.InvariantCulture);
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsCommandeFournisseur"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsCommandeFournisseur"];
            }
            if (!ListeDesPoduits.Select(prd => prd.ID).Contains(ligne.ID))
            {
                ListeDesPoduits.Add(ligne);
            }
            Session["ProduitsCommandeFournisseur"] = ListeDesPoduits;
            return string.Empty;
        }
        public string AddLineBonReception(string ID_Produit, string LIB_Produi, string Description_Produit, string Quantite_Produit, string PUHT_Produit, string Remise_Produit, string PTHT_Produit, string TVA_Produit, string TTC_Produit)
        {
            LigneProduit ligne = new LigneProduit();
            ligne.ID = int.Parse(ID_Produit);
            ligne.LIBELLE = LIB_Produi;
            ligne.DESIGNATION = Description_Produit;
            ligne.QUANTITE = int.Parse(Quantite_Produit);
            ligne.PRIX_VENTE_HT = decimal.Parse(PUHT_Produit, CultureInfo.InvariantCulture);
            ligne.REMISE = int.Parse(Remise_Produit);
            ligne.PTHT = decimal.Parse(PTHT_Produit, CultureInfo.InvariantCulture);
            ligne.TVA = int.Parse(TVA_Produit);
            ligne.TTC = decimal.Parse(TTC_Produit, CultureInfo.InvariantCulture);
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsBonCommandeFournisseur"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsBonCommandeFournisseur"];
            }
            if (!ListeDesPoduits.Select(prd => prd.ID).Contains(ligne.ID))
            {
                ListeDesPoduits.Add(ligne);
            }
            Session["ProduitsBonCommandeFournisseur"] = ListeDesPoduits;
            return string.Empty;
        }
        public string AddLineFacture(string ID_Produit, string LIB_Produi, string Description_Produit, string Quantite_Produit, string PUHT_Produit, string Remise_Produit, string PTHT_Produit, string TVA_Produit, string TTC_Produit)
        {
            LigneProduit ligne = new LigneProduit();
            ligne.ID = int.Parse(ID_Produit);
            ligne.LIBELLE = LIB_Produi;
            ligne.DESIGNATION = Description_Produit;
            ligne.QUANTITE = int.Parse(Quantite_Produit);
            ligne.PRIX_VENTE_HT = decimal.Parse(PUHT_Produit, CultureInfo.InvariantCulture);
            ligne.REMISE = int.Parse(Remise_Produit);
            ligne.PTHT = decimal.Parse(PTHT_Produit, CultureInfo.InvariantCulture);
            ligne.TVA = int.Parse(TVA_Produit);
            ligne.TTC = decimal.Parse(TTC_Produit, CultureInfo.InvariantCulture);
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsFactureFournisseur"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsFactureFournisseur"];
            }
            if (!ListeDesPoduits.Select(prd => prd.ID).Contains(ligne.ID))
            {
                ListeDesPoduits.Add(ligne);
            }
            Session["ProduitsFactureFournisseur"] = ListeDesPoduits;
            return string.Empty;
        }
        public string AddLineAvoir(string ID_Produit, string LIB_Produi, string Description_Produit, string Quantite_Produit, string PUHT_Produit, string Remise_Produit, string PTHT_Produit, string TVA_Produit, string TTC_Produit)
        {
            LigneProduit ligne = new LigneProduit();
            ligne.ID = int.Parse(ID_Produit);
            ligne.LIBELLE = LIB_Produi;
            ligne.DESIGNATION = Description_Produit;
            ligne.QUANTITE = int.Parse(Quantite_Produit);
            ligne.PRIX_VENTE_HT = decimal.Parse(PUHT_Produit, CultureInfo.InvariantCulture);
            ligne.REMISE = int.Parse(Remise_Produit);
            ligne.PTHT = decimal.Parse(PTHT_Produit, CultureInfo.InvariantCulture);
            ligne.TVA = int.Parse(TVA_Produit);
            ligne.TTC = decimal.Parse(TTC_Produit, CultureInfo.InvariantCulture);
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsAvoirFournisseur"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsAvoirFournisseur"];
            }
            if (!ListeDesPoduits.Select(prd => prd.ID).Contains(ligne.ID))
            {
                ListeDesPoduits.Add(ligne);
            }
            Session["ProduitsAvoirFournisseur"] = ListeDesPoduits;
            return string.Empty;
        }
        public string EditLigne(string ID_Produit, string Quantite_Produit, string PUHT_Produit, string Remise_Produit, string PTHT_Produit, string TVA_Produit, string TTC_Produit)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsCommandeFournisseur"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsCommandeFournisseur"];
            }
            int ID = int.Parse(ID_Produit);
            LigneProduit ligne = ListeDesPoduits.Where(pr => pr.ID == ID).FirstOrDefault();
            ligne.QUANTITE = int.Parse(Quantite_Produit);
            ligne.PRIX_VENTE_HT = decimal.Parse(PUHT_Produit, CultureInfo.InvariantCulture);
            ligne.REMISE = int.Parse(Remise_Produit);
            ligne.PTHT = decimal.Parse(PTHT_Produit, CultureInfo.InvariantCulture);
            ligne.TVA = int.Parse(TVA_Produit);
            ligne.TTC = decimal.Parse(TTC_Produit, CultureInfo.InvariantCulture);
            Session["ProduitsCommandeFournisseur"] = ListeDesPoduits;
            return string.Empty;
        }
        public string EditLigneBonReception(string ID_Produit, string Quantite_Produit, string PUHT_Produit, string Remise_Produit, string PTHT_Produit, string TVA_Produit, string TTC_Produit)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsBonCommandeFournisseur"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsBonCommandeFournisseur"];
            }
            int ID = int.Parse(ID_Produit);
            LigneProduit ligne = ListeDesPoduits.Where(pr => pr.ID == ID).FirstOrDefault();
            ligne.QUANTITE = int.Parse(Quantite_Produit);
            ligne.PRIX_VENTE_HT = decimal.Parse(PUHT_Produit, CultureInfo.InvariantCulture);
            ligne.REMISE = int.Parse(Remise_Produit);
            ligne.PTHT = decimal.Parse(PTHT_Produit, CultureInfo.InvariantCulture);
            ligne.TVA = int.Parse(TVA_Produit);
            ligne.TTC = decimal.Parse(TTC_Produit, CultureInfo.InvariantCulture);
            Session["ProduitsBonCommandeFournisseur"] = ListeDesPoduits;
            return string.Empty;
        }
        public string EditLigneFacture(string ID_Produit, string Quantite_Produit, string PUHT_Produit, string Remise_Produit, string PTHT_Produit, string TVA_Produit, string TTC_Produit)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsFactureFournisseur"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsFactureFournisseur"];
            }
            int ID = int.Parse(ID_Produit);
            LigneProduit ligne = ListeDesPoduits.Where(pr => pr.ID == ID).FirstOrDefault();
            ligne.QUANTITE = int.Parse(Quantite_Produit);
            ligne.PRIX_VENTE_HT = decimal.Parse(PUHT_Produit, CultureInfo.InvariantCulture);
            ligne.REMISE = int.Parse(Remise_Produit);
            ligne.PTHT = decimal.Parse(PTHT_Produit, CultureInfo.InvariantCulture);
            ligne.TVA = int.Parse(TVA_Produit);
            ligne.TTC = decimal.Parse(TTC_Produit, CultureInfo.InvariantCulture);
            Session["ProduitsFactureFournisseur"] = ListeDesPoduits;
            return string.Empty;
        }
        public string EditLigneAvoir(string ID_Produit, string Quantite_Produit, string PUHT_Produit, string Remise_Produit, string PTHT_Produit, string TVA_Produit, string TTC_Produit)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsAvoirFournisseur"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsAvoirFournisseur"];
            }
            int ID = int.Parse(ID_Produit);
            LigneProduit ligne = ListeDesPoduits.Where(pr => pr.ID == ID).FirstOrDefault();
            ligne.QUANTITE = int.Parse(Quantite_Produit);
            ligne.PRIX_VENTE_HT = decimal.Parse(PUHT_Produit, CultureInfo.InvariantCulture);
            ligne.REMISE = int.Parse(Remise_Produit);
            ligne.PTHT = decimal.Parse(PTHT_Produit, CultureInfo.InvariantCulture);
            ligne.TVA = int.Parse(TVA_Produit);
            ligne.TTC = decimal.Parse(TTC_Produit, CultureInfo.InvariantCulture);
            Session["ProduitsAvoirFournisseur"] = ListeDesPoduits;
            return string.Empty;
        }
        public string DeleteLigne(string parampassed)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsCommandeFournisseur"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsCommandeFournisseur"];
            }
            int ID = int.Parse(parampassed);
            LigneProduit ligne = ListeDesPoduits.Where(pr => pr.ID == ID).FirstOrDefault();
            ListeDesPoduits.Remove(ligne);
            return string.Empty;
        }
        public string DeleteLigneBonReception(string parampassed)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsBonCommandeFournisseur"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsBonCommandeFournisseur"];
            }
            int ID = int.Parse(parampassed);
            LigneProduit ligne = ListeDesPoduits.Where(pr => pr.ID == ID).FirstOrDefault();
            ListeDesPoduits.Remove(ligne);
            return string.Empty;
        }
        public string DeleteLigneFacture(string parampassed)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsFactureFournisseur"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsFactureFournisseur"];
            }
            int ID = int.Parse(parampassed);
            LigneProduit ligne = ListeDesPoduits.Where(pr => pr.ID == ID).FirstOrDefault();
            ListeDesPoduits.Remove(ligne);
            return string.Empty;
        }
        public string DeleteLigneAvoir(string parampassed)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsAvoirFournisseur"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsAvoirFournisseur"];
            }
            int ID = int.Parse(parampassed);
            LigneProduit ligne = ListeDesPoduits.Where(pr => pr.ID == ID).FirstOrDefault();
            ListeDesPoduits.Remove(ligne);
            Session["ProduitsAvoirFournisseur"] = ListeDesPoduits;
            return string.Empty;
        }
        public JsonResult GetAllLigne()
        {
            BD.Configuration.ProxyCreationEnabled = false;
            List<LigneProduit> ListeDesPoduits = (List<LigneProduit>)Session["ProduitsCommandeFournisseur"];
            return Json(ListeDesPoduits, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllLigneBonReception()
        {
            BD.Configuration.ProxyCreationEnabled = false;
            List<LigneProduit> ListeDesPoduits = (List<LigneProduit>)Session["ProduitsBonCommandeFournisseur"];
            return Json(ListeDesPoduits, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllLigneFacture()
        {
            BD.Configuration.ProxyCreationEnabled = false;
            List<LigneProduit> ListeDesPoduits = (List<LigneProduit>)Session["ProduitsFactureFournisseur"];
            return Json(ListeDesPoduits, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllLigneAvoir()
        {
            BD.Configuration.ProxyCreationEnabled = false;
            List<LigneProduit> ListeDesPoduits = (List<LigneProduit>)Session["ProduitsAvoirFournisseur"];
            return Json(ListeDesPoduits, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdatePrice(string remise)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsCommandeFournisseur"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsCommandeFournisseur"];
            }
            decimal totalHT = 0;
            decimal totalTVA = 0;
            foreach (LigneProduit ligne in ListeDesPoduits)
            {
                totalHT += ligne.PTHT;
                totalTVA += (ligne.PTHT * ligne.TVA) / 100;
            }
            int IntRemise = int.Parse(remise);
            dynamic Result = new
            {
                totalHT = totalHT,
                totalTVA = totalTVA
            };
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdatePriceBonReception(string remise)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsBonCommandeFournisseur"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsBonCommandeFournisseur"];
            }
            decimal totalHT = 0;
            decimal totalTVA = 0;
            foreach (LigneProduit ligne in ListeDesPoduits)
            {
                totalHT += ligne.PTHT;
                totalTVA += (ligne.PTHT * ligne.TVA) / 100;
            }
            int IntRemise = int.Parse(remise);
            dynamic Result = new
            {
                totalHT = totalHT,
                totalTVA = totalTVA
            };
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdatePriceFacture(string remise)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsFactureFournisseur"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsFactureFournisseur"];
            }
            decimal totalHT = 0;
            decimal totalTVA = 0;
            foreach (LigneProduit ligne in ListeDesPoduits)
            {
                totalHT += ligne.PTHT;
                totalTVA += (ligne.PTHT * ligne.TVA) / 100;
            }
            int IntRemise = int.Parse(remise);
            dynamic Result = new
            {
                totalHT = totalHT,
                totalTVA = totalTVA
            };
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdatePriceAvoir(string remise)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsAvoirFournisseur"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsAvoirFournisseur"];
            }
            decimal totalHT = 0;
            decimal totalTVA = 0;
            foreach (LigneProduit ligne in ListeDesPoduits)
            {
                totalHT += ligne.PTHT;
                totalTVA += (ligne.PTHT * ligne.TVA) / 100;
            }
            int IntRemise = int.Parse(remise);
            dynamic Result = new
            {
                totalHT = totalHT,
                totalTVA = totalTVA
            };
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SendCommande(string Mode, string Code)
        {
            string Numero = Request["numero"] != null ? Request["numero"].ToString() : string.Empty;
            string date = Request["date"] != null ? Request["date"].ToString() : string.Empty;
            string fournisseur = Request["fournisseur"] != null ? Request["fournisseur"].ToString() : string.Empty;
            string modePaiement = Request["modePaiement"] != null ? Request["modePaiement"].ToString() : string.Empty;
            string remise = Request["remise"] != null ? Request["remise"].ToString() : string.Empty;
            string totalHT = Request["totalHT"] != null ? Request["totalHT"].ToString() : "0";
            string NetHT = Request["NetHT"] != null ? Request["NetHT"].ToString() : "0";
            string totalTVA = Request["totalTVA"] != null ? Request["totalTVA"].ToString() : "0";
            string TotalTTC = Request["TotalTTC"] != null ? Request["TotalTTC"].ToString() : "0";
            string netAPaye = Request["netAPaye"] != null ? Request["netAPaye"].ToString() : "0";
            //
            if (string.IsNullOrEmpty(totalHT)) totalHT = "0";
            if (string.IsNullOrEmpty(NetHT)) NetHT = "0";
            if (string.IsNullOrEmpty(totalTVA)) totalTVA = "0";
            if (string.IsNullOrEmpty(TotalTTC)) TotalTTC = "0";
            if (string.IsNullOrEmpty(netAPaye)) netAPaye = "0";
            //
            string WithPrint = Request["WithPrint"] != null ? Request["WithPrint"].ToString() : "false";
            if (string.IsNullOrEmpty(WithPrint)) WithPrint = "false";
            Boolean Print = Boolean.Parse(WithPrint);
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            string SelectedCommande = string.Empty;
            if (Session["ProduitsCommandeFournisseur"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsCommandeFournisseur"];
            }
            if (Mode == "Create")
            {
                if (!BD.COMMANDES_FOURNISSEURS.Select(cmd => cmd.CODE).Contains(Numero))
                {
                    COMMANDES_FOURNISSEURS CommandeFounisseur = new COMMANDES_FOURNISSEURS();
                    CommandeFounisseur.CODE = Numero;
                    CommandeFounisseur.DATE = DateTime.Parse(date);
                    CommandeFounisseur.FOURNISSEUR = int.Parse(fournisseur);
                    int ID_FOURNISSEUR = int.Parse(fournisseur);
                    CommandeFounisseur.FOURNISSEURS = BD.FOURNISSEURS.Where(fou => fou.ID == ID_FOURNISSEUR).FirstOrDefault();
                    CommandeFounisseur.MODE_PAIEMENT = modePaiement;
                    CommandeFounisseur.REMISE = decimal.Parse(remise, CultureInfo.InvariantCulture);
                    CommandeFounisseur.THT = decimal.Parse(totalHT, CultureInfo.InvariantCulture);
                    CommandeFounisseur.NHT = decimal.Parse(NetHT, CultureInfo.InvariantCulture);
                    CommandeFounisseur.TTVA = decimal.Parse(totalTVA, CultureInfo.InvariantCulture);
                    CommandeFounisseur.TTC = decimal.Parse(TotalTTC, CultureInfo.InvariantCulture);
                    CommandeFounisseur.TNET = decimal.Parse(netAPaye, CultureInfo.InvariantCulture);
                    BD.COMMANDES_FOURNISSEURS.Add(CommandeFounisseur);
                    BD.SaveChanges();
                    foreach (LigneProduit Ligne in ListeDesPoduits)
                    {
                        LIGNES_COMMANDES_FOURNISSEURS UneLigne = new LIGNES_COMMANDES_FOURNISSEURS();
                        UneLigne.PRODUIT = Ligne.ID;
                        PRODUITS Produit = BD.PRODUITS.Where(pr => pr.ID == Ligne.ID).FirstOrDefault();
                        UneLigne.CODE_PRODUIT = Produit.CODE;
                        UneLigne.DESIGNATION_PRODUIT = Ligne.DESIGNATION;
                        UneLigne.QUANTITE = Ligne.QUANTITE;
                        UneLigne.PRIX_UNITAIRE_HT = Ligne.PRIX_VENTE_HT;
                        UneLigne.REMISE = Ligne.REMISE;
                        UneLigne.TOTALE_HT = Ligne.PTHT;
                        UneLigne.TVA = Ligne.TVA;
                        UneLigne.TOTALE_TTC = Ligne.TTC;
                        UneLigne.COMMANDE_FOURNISSEUR = CommandeFounisseur.ID;
                        UneLigne.COMMANDES_FOURNISSEURS = CommandeFounisseur;
                        UneLigne.PRODUITS = Produit;
                        BD.LIGNES_COMMANDES_FOURNISSEURS.Add(UneLigne);
                        BD.SaveChanges();
                        AddMouvementProduit("COMMANDE", Produit, CommandeFounisseur.DATE, CommandeFounisseur.CODE, Ligne.QUANTITE);
                    }
                    SelectedCommande = CommandeFounisseur.ID.ToString();
                }
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                COMMANDES_FOURNISSEURS CommandeFounisseur = BD.COMMANDES_FOURNISSEURS.Where(cmd => cmd.ID == ID).FirstOrDefault();
                CommandeFounisseur.CODE = Numero;
                CommandeFounisseur.DATE = DateTime.Parse(date);
                CommandeFounisseur.FOURNISSEUR = int.Parse(fournisseur);
                int ID_FOURNISSEUR = int.Parse(fournisseur);
                CommandeFounisseur.FOURNISSEURS = BD.FOURNISSEURS.Where(fou => fou.ID == ID_FOURNISSEUR).FirstOrDefault();
                CommandeFounisseur.MODE_PAIEMENT = modePaiement;
                CommandeFounisseur.REMISE = decimal.Parse(remise, CultureInfo.InvariantCulture);
                CommandeFounisseur.THT = decimal.Parse(totalHT, CultureInfo.InvariantCulture);
                CommandeFounisseur.NHT = decimal.Parse(NetHT, CultureInfo.InvariantCulture);
                CommandeFounisseur.TTVA = decimal.Parse(totalTVA, CultureInfo.InvariantCulture);
                CommandeFounisseur.TTC = decimal.Parse(TotalTTC, CultureInfo.InvariantCulture);
                CommandeFounisseur.TNET = decimal.Parse(netAPaye, CultureInfo.InvariantCulture);
                BD.SaveChanges();
                BD.LIGNES_COMMANDES_FOURNISSEURS.Where(p => p.COMMANDE_FOURNISSEUR == CommandeFounisseur.ID).ToList().ForEach(p => BD.LIGNES_COMMANDES_FOURNISSEURS.Remove(p));
                BD.SaveChanges();
                foreach (LigneProduit Ligne in ListeDesPoduits)
                {
                    LIGNES_COMMANDES_FOURNISSEURS UneLigne = new LIGNES_COMMANDES_FOURNISSEURS();
                    UneLigne.PRODUIT = Ligne.ID;
                    PRODUITS Produit = BD.PRODUITS.Where(pr => pr.ID == Ligne.ID).FirstOrDefault();
                    UneLigne.CODE_PRODUIT = Produit.CODE;
                    UneLigne.DESIGNATION_PRODUIT = Ligne.DESIGNATION;
                    UneLigne.QUANTITE = Ligne.QUANTITE;
                    UneLigne.PRIX_UNITAIRE_HT = Ligne.PRIX_VENTE_HT;
                    UneLigne.REMISE = Ligne.REMISE;
                    UneLigne.TOTALE_HT = Ligne.PTHT;
                    UneLigne.TVA = Ligne.TVA;
                    UneLigne.TOTALE_TTC = Ligne.TTC;
                    UneLigne.COMMANDE_FOURNISSEUR = CommandeFounisseur.ID;
                    UneLigne.COMMANDES_FOURNISSEURS = CommandeFounisseur;
                    UneLigne.PRODUITS = Produit;
                    BD.LIGNES_COMMANDES_FOURNISSEURS.Add(UneLigne);
                    BD.SaveChanges();
                }
                SelectedCommande = CommandeFounisseur.ID.ToString();
            }
            if (Print)
            {
                return RedirectToAction("PrintCommandeClientByID", new { CODE = SelectedCommande });
            }
            Session["ProduitsCommandeFournisseur"] = null;
            ViewData["MODE"] = Mode;
            ViewBag.MODE = Mode;
            return RedirectToAction("Commandes", new { MODE = Mode });
        }
        [HttpPost]
        public ActionResult SendReception(string Mode, string Code)
        {
            string Numero = Request["numero"] != null ? Request["numero"].ToString() : string.Empty;
            string date = Request["date"] != null ? Request["date"].ToString() : string.Empty;
            string fournisseur = Request["fournisseur"] != null ? Request["fournisseur"].ToString() : string.Empty;
            string modePaiement = Request["modePaiement"] != null ? Request["modePaiement"].ToString() : string.Empty;
            string remise = Request["remise"] != null ? Request["remise"].ToString() : string.Empty;
            string totalHT = Request["totalHT"] != null ? Request["totalHT"].ToString() : "0";
            string NetHT = Request["NetHT"] != null ? Request["NetHT"].ToString() : "0";
            string totalTVA = Request["totalTVA"] != null ? Request["totalTVA"].ToString() : "0";
            string TotalTTC = Request["TotalTTC"] != null ? Request["TotalTTC"].ToString() : "0";
            string netAPaye = Request["netAPaye"] != null ? Request["netAPaye"].ToString() : "0";
            //
            if (string.IsNullOrEmpty(totalHT)) totalHT = "0";
            if (string.IsNullOrEmpty(NetHT)) NetHT = "0";
            if (string.IsNullOrEmpty(totalTVA)) totalTVA = "0";
            if (string.IsNullOrEmpty(TotalTTC)) TotalTTC = "0";
            if (string.IsNullOrEmpty(netAPaye)) netAPaye = "0";
            //
            string WithPrint = Request["WithPrint"] != null ? Request["WithPrint"].ToString() : "false";
            if (string.IsNullOrEmpty(WithPrint)) WithPrint = "false";
            Boolean Print = Boolean.Parse(WithPrint);
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            string SelectedBonReception = string.Empty;
            if (Session["ProduitsBonCommandeFournisseur"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsBonCommandeFournisseur"];
            }
            if (Mode == "Create")
            {
                if (!BD.BONS_RECEPTIONS_FOURNISSEURS.Select(cmd => cmd.CODE).Contains(Numero))
                {
                    BONS_RECEPTIONS_FOURNISSEURS BonReceptionFounisseur = new BONS_RECEPTIONS_FOURNISSEURS();
                    BonReceptionFounisseur.CODE = Numero;
                    BonReceptionFounisseur.DATE = DateTime.Parse(date);
                    BonReceptionFounisseur.FOURNISSEUR = int.Parse(fournisseur);
                    int ID_FOURNISSEUR = int.Parse(fournisseur);
                    BonReceptionFounisseur.FOURNISSEURS = BD.FOURNISSEURS.Where(fou => fou.ID == ID_FOURNISSEUR).FirstOrDefault();
                    BonReceptionFounisseur.MODE_PAIEMENT = modePaiement;
                    BonReceptionFounisseur.REMISE = decimal.Parse(remise, CultureInfo.InvariantCulture);
                    BonReceptionFounisseur.THT = decimal.Parse(totalHT, CultureInfo.InvariantCulture);
                    BonReceptionFounisseur.NHT = decimal.Parse(NetHT, CultureInfo.InvariantCulture);
                    BonReceptionFounisseur.TTVA = decimal.Parse(totalTVA, CultureInfo.InvariantCulture);
                    BonReceptionFounisseur.TTC = decimal.Parse(TotalTTC, CultureInfo.InvariantCulture);
                    BonReceptionFounisseur.TNET = decimal.Parse(netAPaye, CultureInfo.InvariantCulture);
                    BonReceptionFounisseur.VALIDER = false;
                    BD.BONS_RECEPTIONS_FOURNISSEURS.Add(BonReceptionFounisseur);
                    BD.SaveChanges();
                    foreach (LigneProduit Ligne in ListeDesPoduits)
                    {
                        LIGNES_BONS_RECEPTIONS_FOURNISSEURS UneLigne = new LIGNES_BONS_RECEPTIONS_FOURNISSEURS();
                        UneLigne.PRODUIT = Ligne.ID;
                        PRODUITS Produit = BD.PRODUITS.Where(pr => pr.ID == Ligne.ID).FirstOrDefault();
                        UneLigne.CODE_PRODUIT = Produit.CODE;
                        UneLigne.DESIGNATION_PRODUIT = Ligne.DESIGNATION;
                        UneLigne.QUANTITE = Ligne.QUANTITE;
                        UneLigne.PRIX_UNITAIRE_HT = Ligne.PRIX_VENTE_HT;
                        UneLigne.REMISE = Ligne.REMISE;
                        UneLigne.TOTALE_HT = Ligne.PTHT;
                        UneLigne.TVA = Ligne.TVA;
                        UneLigne.TOTALE_TTC = Ligne.TTC;
                        UneLigne.BON_RECEPTION_FOURNISSEUR = BonReceptionFounisseur.ID;
                        UneLigne.BONS_RECEPTIONS_FOURNISSEURS = BonReceptionFounisseur;
                        UneLigne.PRODUITS = Produit;
                        BD.LIGNES_BONS_RECEPTIONS_FOURNISSEURS.Add(UneLigne);
                        BD.SaveChanges();
                        AddMouvementProduit("BON_RECEPTION", Produit, BonReceptionFounisseur.DATE, BonReceptionFounisseur.CODE, Ligne.QUANTITE);
                    }
                    SelectedBonReception = BonReceptionFounisseur.ID.ToString();
                }
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                BONS_RECEPTIONS_FOURNISSEURS BonReceptionFounisseur = BD.BONS_RECEPTIONS_FOURNISSEURS.Where(cmd => cmd.ID == ID).FirstOrDefault();
                BonReceptionFounisseur.CODE = Numero;
                BonReceptionFounisseur.DATE = DateTime.Parse(date);
                BonReceptionFounisseur.FOURNISSEUR = int.Parse(fournisseur);
                int ID_FOURNISSEUR = int.Parse(fournisseur);
                BonReceptionFounisseur.FOURNISSEURS = BD.FOURNISSEURS.Where(fou => fou.ID == ID_FOURNISSEUR).FirstOrDefault();
                BonReceptionFounisseur.MODE_PAIEMENT = modePaiement;
                BonReceptionFounisseur.REMISE = decimal.Parse(remise, CultureInfo.InvariantCulture);
                BonReceptionFounisseur.THT = decimal.Parse(totalHT, CultureInfo.InvariantCulture);
                BonReceptionFounisseur.NHT = decimal.Parse(NetHT, CultureInfo.InvariantCulture);
                BonReceptionFounisseur.TTVA = decimal.Parse(totalTVA, CultureInfo.InvariantCulture);
                BonReceptionFounisseur.TTC = decimal.Parse(TotalTTC, CultureInfo.InvariantCulture);
                BonReceptionFounisseur.TNET = decimal.Parse(netAPaye, CultureInfo.InvariantCulture);
                BD.SaveChanges();
                BD.LIGNES_BONS_RECEPTIONS_FOURNISSEURS.Where(p => p.BON_RECEPTION_FOURNISSEUR == BonReceptionFounisseur.ID).ToList().ForEach(p => BD.LIGNES_BONS_RECEPTIONS_FOURNISSEURS.Remove(p));
                BD.SaveChanges();
                foreach (LigneProduit Ligne in ListeDesPoduits)
                {
                    LIGNES_BONS_RECEPTIONS_FOURNISSEURS UneLigne = new LIGNES_BONS_RECEPTIONS_FOURNISSEURS();
                    UneLigne.PRODUIT = Ligne.ID;
                    PRODUITS Produit = BD.PRODUITS.Where(pr => pr.ID == Ligne.ID).FirstOrDefault();
                    UneLigne.CODE_PRODUIT = Produit.CODE;
                    UneLigne.DESIGNATION_PRODUIT = Ligne.DESIGNATION;
                    UneLigne.QUANTITE = Ligne.QUANTITE;
                    UneLigne.PRIX_UNITAIRE_HT = Ligne.PRIX_VENTE_HT;
                    UneLigne.REMISE = Ligne.REMISE;
                    UneLigne.TOTALE_HT = Ligne.PTHT;
                    UneLigne.TVA = Ligne.TVA;
                    UneLigne.TOTALE_TTC = Ligne.TTC;
                    UneLigne.BON_RECEPTION_FOURNISSEUR = BonReceptionFounisseur.ID;
                    UneLigne.BONS_RECEPTIONS_FOURNISSEURS = BonReceptionFounisseur;
                    UneLigne.PRODUITS = Produit;
                    BD.LIGNES_BONS_RECEPTIONS_FOURNISSEURS.Add(UneLigne);
                    BD.SaveChanges();
                }
                SelectedBonReception = BonReceptionFounisseur.ID.ToString();
            }
            if (Print)
            {
                return RedirectToAction("PrintBonReceptionClientByID", new { CODE = SelectedBonReception });
            }
            Session["ProduitsBonCommandeFournisseur"] = null;
            ViewData["MODE"] = Mode;
            ViewBag.MODE = Mode;
            return RedirectToAction("BonReception", new { MODE = Mode });
        }
        [HttpPost]
        public ActionResult SendFacture(string Mode, string Code)
        {
            string Numero = Request["numero"] != null ? Request["numero"].ToString() : string.Empty;
            string date = Request["date"] != null ? Request["date"].ToString() : string.Empty;
            string fournisseur = Request["fournisseur"] != null ? Request["fournisseur"].ToString() : string.Empty;
            string modePaiement = Request["modePaiement"] != null ? Request["modePaiement"].ToString() : string.Empty;
            string remise = Request["remise"] != null ? Request["remise"].ToString() : string.Empty;
            string totalHT = Request["totalHT"] != null ? Request["totalHT"].ToString() : "0";
            string NetHT = Request["NetHT"] != null ? Request["NetHT"].ToString() : "0";
            string totalTVA = Request["totalTVA"] != null ? Request["totalTVA"].ToString() : "0";
            string TotalTTC = Request["TotalTTC"] != null ? Request["TotalTTC"].ToString() : "0";
            string netAPaye = Request["netAPaye"] != null ? Request["netAPaye"].ToString() : "0";
            string timbre = Request["timbre"] != null ? Request["timbre"].ToString() : "0";

            //
            if (string.IsNullOrEmpty(totalHT)) totalHT = "0";
            if (string.IsNullOrEmpty(NetHT)) NetHT = "0";
            if (string.IsNullOrEmpty(totalTVA)) totalTVA = "0";
            if (string.IsNullOrEmpty(TotalTTC)) TotalTTC = "0";
            if (string.IsNullOrEmpty(netAPaye)) netAPaye = "0";
            if (string.IsNullOrEmpty(timbre)) timbre = "0";
            //
            string WithPrint = Request["WithPrint"] != null ? Request["WithPrint"].ToString() : "false";
            if (string.IsNullOrEmpty(WithPrint)) WithPrint = "false";
            Boolean Print = Boolean.Parse(WithPrint);
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            string SelectedFacture = string.Empty;
            if (Session["ProduitsFactureFournisseur"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsFactureFournisseur"];
            }
            if (Mode == "Create")
            {
                if (!BD.FACTURES_FOURNISSEURS.Select(cmd => cmd.CODE).Contains(Numero))
                {
                    FACTURES_FOURNISSEURS FactureFounisseur = new FACTURES_FOURNISSEURS();
                    FactureFounisseur.CODE = Numero;
                    FactureFounisseur.DATE = DateTime.Parse(date);
                    FactureFounisseur.FOURNISSEUR = int.Parse(fournisseur);
                    int ID_FOURNISSEUR = int.Parse(fournisseur);
                    FactureFounisseur.FOURNISSEURS = BD.FOURNISSEURS.Where(fou => fou.ID == ID_FOURNISSEUR).FirstOrDefault();
                    FactureFounisseur.MODE_PAIEMENT = modePaiement;
                    FactureFounisseur.REMISE = decimal.Parse(remise, CultureInfo.InvariantCulture);
                    FactureFounisseur.THT = decimal.Parse(totalHT, CultureInfo.InvariantCulture);
                    FactureFounisseur.NHT = decimal.Parse(NetHT, CultureInfo.InvariantCulture);
                    FactureFounisseur.TTVA = decimal.Parse(totalTVA, CultureInfo.InvariantCulture);
                    FactureFounisseur.TTC = decimal.Parse(TotalTTC, CultureInfo.InvariantCulture);
                    FactureFounisseur.TNET = decimal.Parse(netAPaye, CultureInfo.InvariantCulture);
                    FactureFounisseur.TIMBRE = decimal.Parse(timbre, CultureInfo.InvariantCulture);
                    FactureFounisseur.VALIDER = false;
                    FactureFounisseur.PAYEE = false;
                    BD.FACTURES_FOURNISSEURS.Add(FactureFounisseur);
                    BD.SaveChanges();
                    foreach (LigneProduit Ligne in ListeDesPoduits)
                    {
                        LIGNES_FACTURES_FOURNISSEURS UneLigne = new LIGNES_FACTURES_FOURNISSEURS();
                        UneLigne.PRODUIT = Ligne.ID;
                        PRODUITS Produit = BD.PRODUITS.Where(pr => pr.ID == Ligne.ID).FirstOrDefault();
                        UneLigne.CODE_PRODUIT = Produit.CODE;
                        UneLigne.DESIGNATION_PRODUIT = Ligne.DESIGNATION;
                        UneLigne.QUANTITE = Ligne.QUANTITE;
                        UneLigne.PRIX_UNITAIRE_HT = Ligne.PRIX_VENTE_HT;
                        UneLigne.REMISE = Ligne.REMISE;
                        UneLigne.TOTALE_HT = Ligne.PTHT;
                        UneLigne.TVA = Ligne.TVA;
                        UneLigne.TOTALE_TTC = Ligne.TTC;
                        UneLigne.FACTURE_FOURNISSEUR = FactureFounisseur.ID;
                        UneLigne.FACTURES_FOURNISSEURS = FactureFounisseur;
                        UneLigne.PRODUITS = Produit;
                        BD.LIGNES_FACTURES_FOURNISSEURS.Add(UneLigne);
                        BD.SaveChanges();
                        AddMouvementProduit("FACTURE", Produit, FactureFounisseur.DATE, FactureFounisseur.CODE, Ligne.QUANTITE);
                    }
                    SelectedFacture = FactureFounisseur.ID.ToString();
                }
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                FACTURES_FOURNISSEURS FactureFounisseur = BD.FACTURES_FOURNISSEURS.Where(cmd => cmd.ID == ID).FirstOrDefault();
                FactureFounisseur.CODE = Numero;
                FactureFounisseur.DATE = DateTime.Parse(date);
                FactureFounisseur.FOURNISSEUR = int.Parse(fournisseur);
                int ID_FOURNISSEUR = int.Parse(fournisseur);
                FactureFounisseur.FOURNISSEURS = BD.FOURNISSEURS.Where(fou => fou.ID == ID_FOURNISSEUR).FirstOrDefault();
                FactureFounisseur.MODE_PAIEMENT = modePaiement;
                FactureFounisseur.REMISE = decimal.Parse(remise, CultureInfo.InvariantCulture);
                FactureFounisseur.THT = decimal.Parse(totalHT, CultureInfo.InvariantCulture);
                FactureFounisseur.NHT = decimal.Parse(NetHT, CultureInfo.InvariantCulture);
                FactureFounisseur.TTVA = decimal.Parse(totalTVA, CultureInfo.InvariantCulture);
                FactureFounisseur.TTC = decimal.Parse(TotalTTC, CultureInfo.InvariantCulture);
                FactureFounisseur.TNET = decimal.Parse(netAPaye, CultureInfo.InvariantCulture);
                FactureFounisseur.TIMBRE = decimal.Parse(timbre, CultureInfo.InvariantCulture);
                BD.SaveChanges();
                BD.LIGNES_FACTURES_FOURNISSEURS.Where(p => p.FACTURE_FOURNISSEUR == FactureFounisseur.ID).ToList().ForEach(p => BD.LIGNES_FACTURES_FOURNISSEURS.Remove(p));
                BD.SaveChanges();
                foreach (LigneProduit Ligne in ListeDesPoduits)
                {
                    LIGNES_FACTURES_FOURNISSEURS UneLigne = new LIGNES_FACTURES_FOURNISSEURS();
                    UneLigne.PRODUIT = Ligne.ID;
                    PRODUITS Produit = BD.PRODUITS.Where(pr => pr.ID == Ligne.ID).FirstOrDefault();
                    UneLigne.CODE_PRODUIT = Produit.CODE;
                    UneLigne.DESIGNATION_PRODUIT = Ligne.DESIGNATION;
                    UneLigne.QUANTITE = Ligne.QUANTITE;
                    UneLigne.PRIX_UNITAIRE_HT = Ligne.PRIX_VENTE_HT;
                    UneLigne.REMISE = Ligne.REMISE;
                    UneLigne.TOTALE_HT = Ligne.PTHT;
                    UneLigne.TVA = Ligne.TVA;
                    UneLigne.TOTALE_TTC = Ligne.TTC;
                    UneLigne.FACTURE_FOURNISSEUR = FactureFounisseur.ID;
                    UneLigne.FACTURES_FOURNISSEURS = FactureFounisseur;
                    UneLigne.PRODUITS = Produit;
                    BD.LIGNES_FACTURES_FOURNISSEURS.Add(UneLigne);
                    BD.SaveChanges();
                }
                SelectedFacture = FactureFounisseur.ID.ToString();
            }
            if (Print)
            {
                return RedirectToAction("PrintBonFactureByID", new { CODE = SelectedFacture });
            }
            Session["ProduitsFactureFournisseur"] = null;
            ViewData["MODE"] = Mode;
            ViewBag.MODE = Mode;
            return RedirectToAction("Facture", new { MODE = Mode });
        }
        [HttpPost]
        public ActionResult SendAvoir(string Mode, string Code)
        {
            string Numero = Request["numero"] != null ? Request["numero"].ToString() : string.Empty;
            string date = Request["date"] != null ? Request["date"].ToString() : string.Empty;
            string fournisseur = Request["fournisseur"] != null ? Request["fournisseur"].ToString() : string.Empty;
            string modePaiement = Request["modePaiement"] != null ? Request["modePaiement"].ToString() : string.Empty;
            string remise = Request["remise"] != null ? Request["remise"].ToString() : string.Empty;
            string totalHT = Request["totalHT"] != null ? Request["totalHT"].ToString() : "0";
            string NetHT = Request["NetHT"] != null ? Request["NetHT"].ToString() : "0";
            string totalTVA = Request["totalTVA"] != null ? Request["totalTVA"].ToString() : "0";
            string TotalTTC = Request["TotalTTC"] != null ? Request["TotalTTC"].ToString() : "0";
            string netAPaye = Request["netAPaye"] != null ? Request["netAPaye"].ToString() : "0";
            string timbre = Request["timbre"] != null ? Request["timbre"].ToString() : "0";

            //
            if (string.IsNullOrEmpty(totalHT)) totalHT = "0";
            if (string.IsNullOrEmpty(NetHT)) NetHT = "0";
            if (string.IsNullOrEmpty(totalTVA)) totalTVA = "0";
            if (string.IsNullOrEmpty(TotalTTC)) TotalTTC = "0";
            if (string.IsNullOrEmpty(netAPaye)) netAPaye = "0";
            if (string.IsNullOrEmpty(timbre)) timbre = "0";
            //
            string WithPrint = Request["WithPrint"] != null ? Request["WithPrint"].ToString() : "false";
            if (string.IsNullOrEmpty(WithPrint)) WithPrint = "false";
            Boolean Print = Boolean.Parse(WithPrint);
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            string SelectedAvoir = string.Empty;
            if (Session["ProduitsAvoirFournisseur"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsAvoirFournisseur"];
            }
            if (Mode == "Create")
            {
                if (!BD.AVOIRS_FOURNISSEURS.Select(cmd => cmd.CODE).Contains(Numero))
                {
                    AVOIRS_FOURNISSEURS AvoirFounisseur = new AVOIRS_FOURNISSEURS();
                    AvoirFounisseur.CODE = Numero;
                    AvoirFounisseur.DATE = DateTime.Parse(date);
                    AvoirFounisseur.FOURNISSEUR = int.Parse(fournisseur);
                    int ID_FOURNISSEUR = int.Parse(fournisseur);
                    AvoirFounisseur.FOURNISSEURS = BD.FOURNISSEURS.Where(fou => fou.ID == ID_FOURNISSEUR).FirstOrDefault();
                    AvoirFounisseur.MODE_PAIEMENT = modePaiement;
                    AvoirFounisseur.REMISE = decimal.Parse(remise, CultureInfo.InvariantCulture);
                    AvoirFounisseur.THT = decimal.Parse(totalHT, CultureInfo.InvariantCulture);
                    AvoirFounisseur.NHT = decimal.Parse(NetHT, CultureInfo.InvariantCulture);
                    AvoirFounisseur.TTVA = decimal.Parse(totalTVA, CultureInfo.InvariantCulture);
                    AvoirFounisseur.TTC = decimal.Parse(TotalTTC, CultureInfo.InvariantCulture);
                    AvoirFounisseur.TNET = decimal.Parse(netAPaye, CultureInfo.InvariantCulture);
                    AvoirFounisseur.VALIDER = false;
                    BD.AVOIRS_FOURNISSEURS.Add(AvoirFounisseur);
                    BD.SaveChanges();
                    foreach (LigneProduit Ligne in ListeDesPoduits)
                    {
                        LIGNES_AVOIRS_FOURNISSEURS UneLigne = new LIGNES_AVOIRS_FOURNISSEURS();
                        UneLigne.PRODUIT = Ligne.ID;
                        PRODUITS Produit = BD.PRODUITS.Where(pr => pr.ID == Ligne.ID).FirstOrDefault();
                        UneLigne.CODE_PRODUIT = Produit.CODE;
                        UneLigne.DESIGNATION_PRODUIT = Ligne.DESIGNATION;
                        UneLigne.QUANTITE = Ligne.QUANTITE;
                        UneLigne.PRIX_UNITAIRE_HT = Ligne.PRIX_VENTE_HT;
                        UneLigne.REMISE = Ligne.REMISE;
                        UneLigne.TOTALE_HT = Ligne.PTHT;
                        UneLigne.TVA = Ligne.TVA;
                        UneLigne.TOTALE_TTC = Ligne.TTC;
                        UneLigne.AVOIR_FOURNISSEUR = AvoirFounisseur.ID;
                        UneLigne.AVOIRS_FOURNISSEURS = AvoirFounisseur;
                        UneLigne.PRODUITS = Produit;
                        BD.LIGNES_AVOIRS_FOURNISSEURS.Add(UneLigne);
                        BD.SaveChanges();
                        AddMouvementProduit("AVOIR", Produit, AvoirFounisseur.DATE, AvoirFounisseur.CODE, Ligne.QUANTITE);
                    }
                    SelectedAvoir = AvoirFounisseur.ID.ToString();
                }
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                AVOIRS_FOURNISSEURS AvoirFounisseur = BD.AVOIRS_FOURNISSEURS.Where(cmd => cmd.ID == ID).FirstOrDefault();
                AvoirFounisseur.CODE = Numero;
                AvoirFounisseur.DATE = DateTime.Parse(date);
                AvoirFounisseur.FOURNISSEUR = int.Parse(fournisseur);
                int ID_FOURNISSEUR = int.Parse(fournisseur);
                AvoirFounisseur.FOURNISSEURS = BD.FOURNISSEURS.Where(fou => fou.ID == ID_FOURNISSEUR).FirstOrDefault();
                AvoirFounisseur.MODE_PAIEMENT = modePaiement;
                AvoirFounisseur.REMISE = decimal.Parse(remise, CultureInfo.InvariantCulture);
                AvoirFounisseur.THT = decimal.Parse(totalHT, CultureInfo.InvariantCulture);
                AvoirFounisseur.NHT = decimal.Parse(NetHT, CultureInfo.InvariantCulture);
                AvoirFounisseur.TTVA = decimal.Parse(totalTVA, CultureInfo.InvariantCulture);
                AvoirFounisseur.TTC = decimal.Parse(TotalTTC, CultureInfo.InvariantCulture);
                AvoirFounisseur.TNET = decimal.Parse(netAPaye, CultureInfo.InvariantCulture);
                BD.SaveChanges();
                BD.LIGNES_AVOIRS_FOURNISSEURS.Where(p => p.AVOIR_FOURNISSEUR == AvoirFounisseur.ID).ToList().ForEach(p => BD.LIGNES_AVOIRS_FOURNISSEURS.Remove(p));
                BD.SaveChanges();
                foreach (LigneProduit Ligne in ListeDesPoduits)
                {
                    LIGNES_AVOIRS_FOURNISSEURS UneLigne = new LIGNES_AVOIRS_FOURNISSEURS();
                    UneLigne.PRODUIT = Ligne.ID;
                    PRODUITS Produit = BD.PRODUITS.Where(pr => pr.ID == Ligne.ID).FirstOrDefault();
                    UneLigne.CODE_PRODUIT = Produit.CODE;
                    UneLigne.DESIGNATION_PRODUIT = Ligne.DESIGNATION;
                    UneLigne.QUANTITE = Ligne.QUANTITE;
                    UneLigne.PRIX_UNITAIRE_HT = Ligne.PRIX_VENTE_HT;
                    UneLigne.REMISE = Ligne.REMISE;
                    UneLigne.TOTALE_HT = Ligne.PTHT;
                    UneLigne.TVA = Ligne.TVA;
                    UneLigne.TOTALE_TTC = Ligne.TTC;
                    UneLigne.AVOIR_FOURNISSEUR = AvoirFounisseur.ID;
                    UneLigne.AVOIRS_FOURNISSEURS = AvoirFounisseur;
                    UneLigne.PRODUITS = Produit;
                    BD.LIGNES_AVOIRS_FOURNISSEURS.Add(UneLigne);
                    BD.SaveChanges();
                }
                SelectedAvoir = AvoirFounisseur.ID.ToString();
            }
            if (Print)
            {
                return RedirectToAction("PrintAvoirByID", new { CODE = SelectedAvoir });
            }
            Session["ProduitsAvoirFournisseur"] = null;
            ViewData["MODE"] = Mode;
            ViewBag.MODE = Mode;
            return RedirectToAction("Avoir", new { MODE = Mode });
        }
        public void AddMouvementProduit(string mode,PRODUITS produit,DateTime date,string code,int quantite)
        {
            MOUVEMENETS_PRODUITS UnMouvement = new MOUVEMENETS_PRODUITS();
            UnMouvement.PRODUIT = produit.ID;
            UnMouvement.PRODUITS = produit;
            UnMouvement.DATE_MOUVEMENT = date;
            UnMouvement.TYPE_MOUVEMENT = mode;
            UnMouvement.CODE_MOUVEMENT = code;
            UnMouvement.QUANTITE_MOUVEMENT = quantite;
            UnMouvement.QUANTITE_AVANT_MOUVEMENT = (int)produit.QUANTITE;
            UnMouvement.QUANTITE_APRES_MOUVEMENT = (int)produit.QUANTITE;
            if (mode == "BON_RECEPTION")
            {
                UnMouvement.QUANTITE_APRES_MOUVEMENT = (int)produit.QUANTITE + quantite;
                PRODUITS Produit= BD.PRODUITS.Where(pr => pr.ID == produit.ID).FirstOrDefault();
                Produit.QUANTITE = Produit.QUANTITE + quantite;
                BD.SaveChanges();
            }
            BD.MOUVEMENETS_PRODUITS.Add(UnMouvement);
            BD.SaveChanges();

        }
    }
}
