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
    public class VenteController : Controller
    {
        //
        // GET: /Vente/
        GestionCommercialeEntity BD = new GestionCommercialeEntity();
        #region Views
        public ActionResult Devis(string Mode)
        {
            List<DEVIS_CLIENTS> Liste = BD.DEVIS_CLIENTS.ToList();
            dynamic Result = (from com in Liste
                              select new
                              {
                                  ID = com.ID,
                                  CODE = com.CODE,
                                  FOURNISSEUR = BD.CLIENTS.Where(fou => fou.ID == com.CLIENT).FirstOrDefault().NOM,
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
        public ActionResult Commandes(string Mode)
        {
            List<COMMANDES_CLIENTS> Liste = BD.COMMANDES_CLIENTS.ToList();
            dynamic Result = (from com in Liste
                              select new
                              {
                                  ID = com.ID,
                                  CODE = com.CODE,
                                  FOURNISSEUR = BD.CLIENTS.Where(fou => fou.ID == com.CLIENT).FirstOrDefault().NOM,
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
        public ActionResult BonLivraison(string Mode)
        {
            List<BONS_LIVRAISONS_CLIENTS> Liste = BD.BONS_LIVRAISONS_CLIENTS.ToList();
            dynamic Result = (from com in Liste
                              select new
                              {
                                  ID = com.ID,
                                  CODE = com.CODE,
                                  FOURNISSEUR = BD.CLIENTS.Where(fou => fou.ID == com.CLIENT).FirstOrDefault().NOM,
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
            List<FACTURES_CLIENTS> Liste = BD.FACTURES_CLIENTS.ToList();
            dynamic Result = (from Fact in Liste
                              select new
                              {
                                  ID = Fact.ID,
                                  CODE = Fact.CODE,
                                  FOURNISSEUR = BD.CLIENTS.Where(fou => fou.ID == Fact.CLIENT).FirstOrDefault().NOM,
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
            List<AVOIRS_CLIENTS> Liste = BD.AVOIRS_CLIENTS.ToList();
            dynamic Result = (from Fact in Liste
                              select new
                              {
                                  ID = Fact.ID,
                                  CODE = Fact.CODE,
                                  FOURNISSEUR = BD.CLIENTS.Where(fou => fou.ID == Fact.CLIENT).FirstOrDefault().NOM,
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
        public ActionResult FormDevis(string Mode, string Code)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            DEVIS_CLIENTS DevisClient = new DEVIS_CLIENTS();
            ViewBag.Mode = Mode;
            ViewBag.Code = Code;
            string Numero = string.Empty;
            if (Mode == "Create")
            {
                int Max = 0;
                if (BD.DEVIS_CLIENTS.ToList().Count != 0)
                {
                    Max = BD.DEVIS_CLIENTS.Where(cmd => cmd.DATE.Year == DateTime.Today.Year).Select(cmd => cmd.ID).Count();
                }
                Max++;
                Numero = "DVC" + Max.ToString("0000") + "/" + DateTime.Today.ToString("yy");
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                DevisClient = BD.DEVIS_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
                Numero = DevisClient.CODE;
                List<LIGNES_DEVIS_CLIENTS> ListeLigne = BD.LIGNES_DEVIS_CLIENTS.Where(lcmd => lcmd.DEVIS_CLIENT == ID).ToList();
                foreach (LIGNES_DEVIS_CLIENTS ligne in ListeLigne)
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
                ViewBag.CODE_CLIENT = DevisClient.CLIENTS.CODE;
            }
            Session["ProduitsDevisClient"] = ListeDesPoduits;
            ViewBag.Numero = Numero;
            return View(DevisClient);
        }
        public ActionResult FormCommande(string Mode, string Code)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            COMMANDES_CLIENTS CommandeClient = new COMMANDES_CLIENTS();
            ViewBag.Mode = Mode;
            ViewBag.Code = Code;
            string Numero = string.Empty;
            if (Mode == "Create")
            {
                int Max = 0;
                if (BD.COMMANDES_CLIENTS.ToList().Count != 0)
                {
                    Max = BD.COMMANDES_CLIENTS.Where(cmd => cmd.DATE.Year == DateTime.Today.Year).Select(cmd => cmd.ID).Count();
                }
                Max++;
                Numero = "CDC" + Max.ToString("0000") + "/" + DateTime.Today.ToString("yy");
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                CommandeClient = BD.COMMANDES_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
                Numero = CommandeClient.CODE;
                List<LIGNES_COMMANDES_CLIENTS> ListeLigne = BD.LIGNES_COMMANDES_CLIENTS.Where(lcmd => lcmd.COMMANDE_CLIENT == ID).ToList();
                foreach (LIGNES_COMMANDES_CLIENTS ligne in ListeLigne)
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
                ViewBag.CODE_CLIENT = CommandeClient.CLIENTS.CODE;
            }
            Session["ProduitsCommandeClient"] = ListeDesPoduits;
            ViewBag.Numero = Numero;
            return View(CommandeClient);
        }
        public ActionResult FormBonLivraison(string Mode, string Code)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            BONS_LIVRAISONS_CLIENTS BonLivraisonClient = new BONS_LIVRAISONS_CLIENTS();
            ViewBag.Mode = Mode;
            ViewBag.Code = Code;
            string Numero = string.Empty;
            if (Mode == "Create")
            {
                int Max = 0;
                if (BD.BONS_LIVRAISONS_CLIENTS.ToList().Count != 0)
                {
                    Max = BD.BONS_LIVRAISONS_CLIENTS.Where(cmd => cmd.DATE.Year == DateTime.Today.Year).Select(cmd => cmd.ID).Count();
                }
                Max++;
                Numero = "BLC" + Max.ToString("0000") + "/" + DateTime.Today.ToString("yy");
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                BonLivraisonClient = BD.BONS_LIVRAISONS_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
                Numero = BonLivraisonClient.CODE;
                List<LIGNES_BONS_LIVRAISONS_CLIENTS> ListeLigne = BD.LIGNES_BONS_LIVRAISONS_CLIENTS.Where(lcmd => lcmd.BON_LIVRAISON_CLIENT== ID).ToList();
                foreach (LIGNES_BONS_LIVRAISONS_CLIENTS ligne in ListeLigne)
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
                ViewBag.CODE_CLIENT = BonLivraisonClient.CLIENTS.CODE;
            }
            Session["ProduitsBonLivraisonClient"] = ListeDesPoduits;
            ViewBag.Numero = Numero;
            return View(BonLivraisonClient);
        }
        public ActionResult FormFacture(string Mode, string Code)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            FACTURES_CLIENTS FactureClient = new FACTURES_CLIENTS();
            ViewBag.Mode = Mode;
            ViewBag.Code = Code;
            string Numero = string.Empty;
            if (Mode == "Create")
            {
                int Max = 0;
                if (BD.FACTURES_CLIENTS.ToList().Count != 0)
                {
                    Max = BD.FACTURES_CLIENTS.Where(cmd => cmd.DATE.Year == DateTime.Today.Year).Select(cmd => cmd.ID).Count();
                }
                Max++;
                Numero = "FC" + Max.ToString("0000") + "/" + DateTime.Today.ToString("yy");
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                FactureClient = BD.FACTURES_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
                Numero = FactureClient.CODE;
                List<LIGNES_FACTURES_CLIENTS> ListeLigne = BD.LIGNES_FACTURES_CLIENTS.Where(lcmd => lcmd.FACTURE_CLIENT == ID).ToList();
                foreach (LIGNES_FACTURES_CLIENTS ligne in ListeLigne)
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
                ViewBag.CODE_CLIENT = FactureClient.CLIENTS.CODE;
            }
            Session["ProduitsFactureClient"] = ListeDesPoduits;
            ViewBag.Numero = Numero;
            return View(FactureClient);
        }
        public ActionResult FormAvoir(string Mode, string Code)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            AVOIRS_CLIENTS AvoirClient = new AVOIRS_CLIENTS();
            ViewBag.Mode = Mode;
            ViewBag.Code = Code;
            string Numero = string.Empty;
            if (Mode == "Create")
            {
                int Max = 0;
                if (BD.FACTURES_CLIENTS.ToList().Count != 0)
                {
                    Max = BD.FACTURES_CLIENTS.Where(cmd => cmd.DATE.Year == DateTime.Today.Year).Select(cmd => cmd.ID).Count();
                }
                Max++;
                Numero = "AVC" + Max.ToString("0000") + "/" + DateTime.Today.ToString("yy");
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                AvoirClient = BD.AVOIRS_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
                Numero = AvoirClient.CODE;
                List<LIGNES_AVOIRS_CLIENTS> ListeLigne = BD.LIGNES_AVOIRS_CLIENTS.Where(lcmd => lcmd.AVOIR_CLIENT == ID).ToList();
                foreach (LIGNES_AVOIRS_CLIENTS ligne in ListeLigne)
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
                ViewBag.CODE_CLIENT = AvoirClient.CLIENTS.CODE;
            }
            Session["ProduitsAvoirClient"] = ListeDesPoduits;
            ViewBag.Numero = Numero;
            return View(AvoirClient);
        }
        #endregion
        #region common functions
        public JsonResult GetAllProduct()
        {
            BD.Configuration.ProxyCreationEnabled = false;
            List<PRODUITS> ListeProduit = BD.PRODUITS.ToList();
            return Json(ListeProduit, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductByID(string ID)
        {
            BD.Configuration.ProxyCreationEnabled = false;
            int id = int.Parse(ID);
            PRODUITS produit = BD.PRODUITS.Where(pr => pr.ID == id).FirstOrDefault();
            return Json(produit, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllClient()
        {
            BD.Configuration.ProxyCreationEnabled = false;
            List<CLIENTS> ListeClient = BD.CLIENTS.ToList();
            return Json(ListeClient, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetClientByID(string ID)
        {
            BD.Configuration.ProxyCreationEnabled = false;
            int id = int.Parse(ID);
            CLIENTS client = BD.CLIENTS.Where(pr => pr.ID == id).FirstOrDefault();
            return Json(client, JsonRequestBehavior.AllowGet);
        }
        public void AddMouvementProduit(string mode, PRODUITS produit, DateTime date, string code, int quantite)
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
                PRODUITS Produit = BD.PRODUITS.Where(pr => pr.ID == produit.ID).FirstOrDefault();
                Produit.QUANTITE = Produit.QUANTITE + quantite;
                BD.SaveChanges();
            }
            if (mode == "BON_LIVRAISON")
            {
                UnMouvement.QUANTITE_APRES_MOUVEMENT = (int)produit.QUANTITE - quantite;
                PRODUITS Produit = BD.PRODUITS.Where(pr => pr.ID == produit.ID).FirstOrDefault();
                Produit.QUANTITE = Produit.QUANTITE - quantite;
                BD.SaveChanges();
            }
            BD.MOUVEMENETS_PRODUITS.Add(UnMouvement);
            BD.SaveChanges();

        }
        #endregion
        #region Delete
        public string DeleteDevis(string parampassed)
        {
            int ID = int.Parse(parampassed);
            BD.LIGNES_DEVIS_CLIENTS.Where(p => p.DEVIS_CLIENT == ID).ToList().ForEach(p => BD.LIGNES_DEVIS_CLIENTS.Remove(p));
            BD.SaveChanges();
            DEVIS_CLIENTS Devis = BD.DEVIS_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            BD.DEVIS_CLIENTS.Remove(Devis);
            BD.SaveChanges();
            return string.Empty;
        }
        public string DeleteCommande(string parampassed)
        {
            int ID = int.Parse(parampassed);
            BD.LIGNES_COMMANDES_CLIENTS.Where(p => p.COMMANDE_CLIENT == ID).ToList().ForEach(p => BD.LIGNES_COMMANDES_CLIENTS.Remove(p));
            BD.SaveChanges();
            COMMANDES_CLIENTS Devis = BD.COMMANDES_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            BD.COMMANDES_CLIENTS.Remove(Devis);
            BD.SaveChanges();
            return string.Empty;
        }
        public string DeleteBonLaivraison(string parampassed)
        {
            int ID = int.Parse(parampassed);
            BD.LIGNES_BONS_LIVRAISONS_CLIENTS.Where(p => p.BON_LIVRAISON_CLIENT == ID).ToList().ForEach(p => BD.LIGNES_BONS_LIVRAISONS_CLIENTS.Remove(p));
            BD.SaveChanges();
            BONS_LIVRAISONS_CLIENTS Devis = BD.BONS_LIVRAISONS_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            BD.BONS_LIVRAISONS_CLIENTS.Remove(Devis);
            BD.SaveChanges();
            return string.Empty;
        }
        public string DeleteFacture(string parampassed)
        {
            int ID = int.Parse(parampassed);
            BD.LIGNES_FACTURES_CLIENTS.Where(p => p.FACTURE_CLIENT == ID).ToList().ForEach(p => BD.LIGNES_FACTURES_CLIENTS.Remove(p));
            BD.SaveChanges();
            FACTURES_CLIENTS Devis = BD.FACTURES_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            BD.FACTURES_CLIENTS.Remove(Devis);
            BD.SaveChanges();
            return string.Empty;
        }
        public string DeleteAvoir(string parampassed)
        {
            int ID = int.Parse(parampassed);
            BD.LIGNES_AVOIRS_CLIENTS.Where(p => p.AVOIR_CLIENT == ID).ToList().ForEach(p => BD.LIGNES_AVOIRS_CLIENTS.Remove(p));
            BD.SaveChanges();
            AVOIRS_CLIENTS Devis = BD.AVOIRS_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            BD.AVOIRS_CLIENTS.Remove(Devis);
            BD.SaveChanges();
            return string.Empty;
        }
        #endregion
        #region Print
        public ActionResult PrintDevisClientByID(string CODE)
        {
            int ID = int.Parse(CODE);
            ConvertisseurChiffresLettres convert = new ConvertisseurChiffresLettres();
            DEVIS_CLIENTS UnDevis = BD.DEVIS_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            List<LIGNES_DEVIS_CLIENTS> Liste = BD.LIGNES_DEVIS_CLIENTS.Where(lcmd => lcmd.DEVIS_CLIENT == ID).ToList();
            dynamic dt = from cmd in Liste
                         select new
                         {
                             Expr1 = UnDevis.CODE,
                             DATE = UnDevis.DATE.ToShortDateString(),
                             CODE = UnDevis.CLIENTS.CODE,
                             NOM = UnDevis.CLIENTS.NOM,
                             CODE_PRODUIT = cmd.CODE_PRODUIT,
                             DESIGNATION_PRODUIT = cmd.DESIGNATION_PRODUIT,
                             QUANTITE = cmd.QUANTITE ?? 0,
                             PRIX_UNITAIRE_HT = cmd.PRIX_UNITAIRE_HT ?? 0,
                             Expr2 = cmd.REMISE ?? 0,
                             TOTALE_HT = cmd.TOTALE_HT ?? 0,
                             TVA = cmd.TVA ?? 0,
                             TOTALE_TTC = cmd.TOTALE_TTC ?? 0,
                             MODE_PAIEMENT = UnDevis.MODE_PAIEMENT,
                             THT = UnDevis.THT,
                             TTVA = UnDevis.TTVA,
                             TTC = UnDevis.TTC,
                             NHT = UnDevis.NHT,
                             TNET = UnDevis.TNET,
                             REMISE = UnDevis.REMISE,
                             CHIFFRE = convert.NumberToCurrencyText(UnDevis.TNET.ToString()),
                             RC = BD.SOCIETES.Where(soc => soc.ID == 1).FirstOrDefault().RC,
                             CTVA = BD.SOCIETES.Where(soc => soc.ID == 1).FirstOrDefault().ID_FISCALE
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/DevisClient.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Devis Client";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintCommandeClientByID(string CODE)
        {
            int ID = int.Parse(CODE);
            ConvertisseurChiffresLettres convert = new ConvertisseurChiffresLettres();
            COMMANDES_CLIENTS UnDevis = BD.COMMANDES_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            List<LIGNES_COMMANDES_CLIENTS> Liste = BD.LIGNES_COMMANDES_CLIENTS.Where(lcmd => lcmd.COMMANDE_CLIENT == ID).ToList();
            dynamic dt = from cmd in Liste
                         select new
                         {
                             Expr1 = UnDevis.CODE,
                             DATE = UnDevis.DATE.ToShortDateString(),
                             CODE = UnDevis.CLIENTS.CODE,
                             NOM = UnDevis.CLIENTS.NOM,
                             CODE_PRODUIT = cmd.CODE_PRODUIT,
                             DESIGNATION_PRODUIT = cmd.DESIGNATION_PRODUIT,
                             QUANTITE = cmd.QUANTITE ?? 0,
                             PRIX_UNITAIRE_HT = cmd.PRIX_UNITAIRE_HT ?? 0,
                             Expr2 = cmd.REMISE ?? 0,
                             TOTALE_HT = cmd.TOTALE_HT ?? 0,
                             TVA = cmd.TVA ?? 0,
                             TOTALE_TTC = cmd.TOTALE_TTC ?? 0,
                             MODE_PAIEMENT = UnDevis.MODE_PAIEMENT,
                             THT = UnDevis.THT,
                             TTVA = UnDevis.TTVA,
                             TTC = UnDevis.TTC,
                             NHT = UnDevis.NHT,
                             TNET = UnDevis.TNET,
                             REMISE = UnDevis.REMISE,
                             CHIFFRE = convert.NumberToCurrencyText(UnDevis.TNET.ToString()),
                             RC = BD.SOCIETES.Where(soc => soc.ID == 1).FirstOrDefault().RC,
                             CTVA = BD.SOCIETES.Where(soc => soc.ID == 1).FirstOrDefault().ID_FISCALE
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/CommandeClient.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Commande Client";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintBonLivraisonClientByID(string CODE)
        {
            int ID = int.Parse(CODE);
            ConvertisseurChiffresLettres convert = new ConvertisseurChiffresLettres();
            BONS_LIVRAISONS_CLIENTS UnDevis = BD.BONS_LIVRAISONS_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            List<LIGNES_BONS_LIVRAISONS_CLIENTS> Liste = BD.LIGNES_BONS_LIVRAISONS_CLIENTS.Where(lcmd => lcmd.BON_LIVRAISON_CLIENT == ID).ToList();
            dynamic dt = from cmd in Liste
                         select new
                         {
                             Expr1 = UnDevis.CODE,
                             DATE = UnDevis.DATE.ToShortDateString(),
                             CODE = UnDevis.CLIENTS.CODE,
                             NOM = UnDevis.CLIENTS.NOM,
                             CODE_PRODUIT = cmd.CODE_PRODUIT,
                             DESIGNATION_PRODUIT = cmd.DESIGNATION_PRODUIT,
                             QUANTITE = cmd.QUANTITE ?? 0,
                             PRIX_UNITAIRE_HT = cmd.PRIX_UNITAIRE_HT ?? 0,
                             Expr2 = cmd.REMISE ?? 0,
                             TOTALE_HT = cmd.TOTALE_HT ?? 0,
                             TVA = cmd.TVA ?? 0,
                             TOTALE_TTC = cmd.TOTALE_TTC ?? 0,
                             MODE_PAIEMENT = UnDevis.MODE_PAIEMENT,
                             THT = UnDevis.THT,
                             TTVA = UnDevis.TTVA,
                             TTC = UnDevis.TTC,
                             NHT = UnDevis.NHT,
                             TNET = UnDevis.TNET,
                             REMISE = UnDevis.REMISE,
                             CHIFFRE = convert.NumberToCurrencyText(UnDevis.TNET.ToString()),
                             RC = BD.SOCIETES.Where(soc => soc.ID == 1).FirstOrDefault().RC,
                             CTVA = BD.SOCIETES.Where(soc => soc.ID == 1).FirstOrDefault().ID_FISCALE
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/BonLivraison.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Bon de livraison Client";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintFactureClientByID(string CODE)
        {
            int ID = int.Parse(CODE);
            ConvertisseurChiffresLettres convert = new ConvertisseurChiffresLettres();
            FACTURES_CLIENTS UnDevis = BD.FACTURES_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            List<LIGNES_FACTURES_CLIENTS> Liste = BD.LIGNES_FACTURES_CLIENTS.Where(lcmd => lcmd.FACTURE_CLIENT == ID).ToList();
            dynamic dt = from cmd in Liste
                         select new
                         {
                             Expr1 = UnDevis.CODE,
                             DATE = UnDevis.DATE.ToShortDateString(),
                             CODE = UnDevis.CLIENTS.CODE,
                             NOM = UnDevis.CLIENTS.NOM,
                             CODE_PRODUIT = cmd.CODE_PRODUIT,
                             DESIGNATION_PRODUIT = cmd.DESIGNATION_PRODUIT,
                             QUANTITE = cmd.QUANTITE ?? 0,
                             PRIX_UNITAIRE_HT = cmd.PRIX_UNITAIRE_HT ?? 0,
                             Expr2 = cmd.REMISE ?? 0,
                             TOTALE_HT = cmd.TOTALE_HT ?? 0,
                             TVA = cmd.TVA ?? 0,
                             TOTALE_TTC = cmd.TOTALE_TTC ?? 0,
                             MODE_PAIEMENT = UnDevis.MODE_PAIEMENT,
                             THT = UnDevis.THT,
                             TTVA = UnDevis.TTVA,
                             TTC = UnDevis.TTC,
                             NHT = UnDevis.NHT,
                             TNET = UnDevis.TNET,
                             REMISE = UnDevis.REMISE,
                             CHIFFRE = convert.NumberToCurrencyText(UnDevis.TNET.ToString()),
                             RC = BD.SOCIETES.Where(soc => soc.ID == 1).FirstOrDefault().RC,
                             CTVA = BD.SOCIETES.Where(soc => soc.ID == 1).FirstOrDefault().ID_FISCALE
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/FactureClient.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Facture Client";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintAvoirClientByID(string CODE)
        {
            int ID = int.Parse(CODE);
            ConvertisseurChiffresLettres convert = new ConvertisseurChiffresLettres();
            AVOIRS_CLIENTS UnDevis = BD.AVOIRS_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            List<LIGNES_AVOIRS_CLIENTS> Liste = BD.LIGNES_AVOIRS_CLIENTS.Where(lcmd => lcmd.AVOIR_CLIENT == ID).ToList();
            dynamic dt = from cmd in Liste
                         select new
                         {
                             Expr1 = UnDevis.CODE,
                             DATE = UnDevis.DATE.ToShortDateString(),
                             CODE = UnDevis.CLIENTS.CODE,
                             NOM = UnDevis.CLIENTS.NOM,
                             CODE_PRODUIT = cmd.CODE_PRODUIT,
                             DESIGNATION_PRODUIT = cmd.DESIGNATION_PRODUIT,
                             QUANTITE = cmd.QUANTITE ?? 0,
                             PRIX_UNITAIRE_HT = cmd.PRIX_UNITAIRE_HT ?? 0,
                             Expr2 = cmd.REMISE ?? 0,
                             TOTALE_HT = cmd.TOTALE_HT ?? 0,
                             TVA = cmd.TVA ?? 0,
                             TOTALE_TTC = cmd.TOTALE_TTC ?? 0,
                             MODE_PAIEMENT = UnDevis.MODE_PAIEMENT,
                             THT = UnDevis.THT,
                             TTVA = UnDevis.TTVA,
                             TTC = UnDevis.TTC,
                             NHT = UnDevis.NHT,
                             TNET = UnDevis.TNET,
                             REMISE = UnDevis.REMISE,
                             CHIFFRE = convert.NumberToCurrencyText(UnDevis.TNET.ToString()),
                             RC = BD.SOCIETES.Where(soc => soc.ID == 1).FirstOrDefault().RC,
                             CTVA = BD.SOCIETES.Where(soc => soc.ID == 1).FirstOrDefault().ID_FISCALE
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/AvoirClient.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Avoir Client";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        public ActionResult PrintAllDevisClient()
        {
            List<DEVIS_CLIENTS> Liste = BD.DEVIS_CLIENTS.ToList();
            dynamic dt = from cmd in Liste
                         select new
                         {
                             CODE = cmd.CODE,
                             FOURNISSEUR = cmd.CLIENTS.NOM,
                             DATE = cmd.DATE.ToShortDateString(),
                             VALIDEE = cmd.VALIDER ? "VALIDEE" : "NON VALIDEE",
                             PAYEE = string.Empty,
                             NET_HT = cmd.NHT,
                             T_TVA = cmd.TTVA,
                             TTC = cmd.TTC,
                             NET_A_PAYE = cmd.TNET

                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/ListeDevisClient.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Devis Client";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintAllCommandeClient()
        {
            List<COMMANDES_CLIENTS> Liste = BD.COMMANDES_CLIENTS.ToList();
            dynamic dt = from cmd in Liste
                         select new
                         {
                             CODE = cmd.CODE,
                             FOURNISSEUR = cmd.CLIENTS.NOM,
                             DATE = cmd.DATE.ToShortDateString(),
                             VALIDEE = cmd.VALIDER ? "VALIDEE" : "NON VALIDEE",
                             PAYEE = string.Empty,
                             NET_HT = cmd.NHT,
                             T_TVA = cmd.TTVA,
                             TTC = cmd.TTC,
                             NET_A_PAYE = cmd.TNET

                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/ListeBonLivraisonFournisseur.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Commandes Clients";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintAllBonLivraisonClient()
        {
            List<BONS_LIVRAISONS_CLIENTS> Liste = BD.BONS_LIVRAISONS_CLIENTS.ToList();
            dynamic dt = from cmd in Liste
                         select new
                         {
                             CODE = cmd.CODE,
                             FOURNISSEUR = cmd.CLIENTS.NOM,
                             DATE = cmd.DATE.ToShortDateString(),
                             VALIDEE = cmd.VALIDER ? "VALIDEE" : "NON VALIDEE",
                             PAYEE = string.Empty,
                             NET_HT = cmd.NHT,
                             T_TVA = cmd.TTVA,
                             TTC = cmd.TTC,
                             NET_A_PAYE = cmd.TNET

                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/ListeBonLivraisonFournisseur.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Bon Livraisons";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintAllFactureClient()
        {
            List<FACTURES_CLIENTS> Liste = BD.FACTURES_CLIENTS.ToList();
            dynamic dt = from cmd in Liste
                         select new
                         {
                             CODE = cmd.CODE,
                             FOURNISSEUR = cmd.CLIENTS.NOM,
                             DATE = cmd.DATE.ToShortDateString(),
                             VALIDEE = cmd.VALIDER ? "VALIDEE" : "NON VALIDEE",
                             PAYEE = cmd.PAYEE ? "PAYEE" : "NON PAYEE",
                             NET_HT = cmd.NHT,
                             T_TVA = cmd.TTVA,
                             TTC = cmd.TTC,
                             NET_A_PAYE = cmd.TNET

                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/ListeFactureClient.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Factures Clients";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintAllAvoirClient()
        {
            List<FACTURES_CLIENTS> Liste = BD.FACTURES_CLIENTS.ToList();
            dynamic dt = from cmd in Liste
                         select new
                         {
                             CODE = cmd.CODE,
                             FOURNISSEUR = cmd.CLIENTS.NOM,
                             DATE = cmd.DATE.ToShortDateString(),
                             VALIDEE = cmd.VALIDER ? "VALIDEE" : "NON VALIDEE",
                             PAYEE = cmd.PAYEE ? "PAYEE" : "NON PAYEE",
                             NET_HT = cmd.NHT,
                             T_TVA = cmd.TTVA,
                             TTC = cmd.TTC,
                             NET_A_PAYE = cmd.TNET

                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/ListeAvoirClient.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Avoirs Clients";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        #endregion 
        #region specifique fonctions  
        public string validateBonLivraison(string parampassed)
        {
            int ID = int.Parse(parampassed);
            BONS_LIVRAISONS_CLIENTS Bonlivraison = BD.BONS_LIVRAISONS_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            Bonlivraison.VALIDER = true;
            BD.SaveChanges();
            return string.Empty;
        }
        public string validateFacture(string parampassed)
        {
            int ID = int.Parse(parampassed);
            FACTURES_CLIENTS facture = BD.FACTURES_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            facture.VALIDER = true;
            BD.SaveChanges();
            return string.Empty;
        }
        public string validateAvoir(string parampassed)
        {
            int ID = int.Parse(parampassed);
            AVOIRS_CLIENTS Avoir = BD.AVOIRS_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            Avoir.VALIDER = true;
            BD.SaveChanges();
            return string.Empty;
        }
        public string PayeFacture(string parampassed)
        {
            int ID = int.Parse(parampassed);
            FACTURES_CLIENTS Facture = BD.FACTURES_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            if (Facture.VALIDER)
            {
                Facture.PAYEE = true;
            }
            BD.SaveChanges();
            return string.Empty;
        }
        public string DevisVersCommande(string parampassed)
        {
            int ID = int.Parse(parampassed);
            DEVIS_CLIENTS Element = BD.DEVIS_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            List<LIGNES_DEVIS_CLIENTS> Liste = BD.LIGNES_DEVIS_CLIENTS.Where(cmd => cmd.DEVIS_CLIENT == ID).ToList();
            COMMANDES_CLIENTS NewElement = new COMMANDES_CLIENTS();
            string Numero = string.Empty;
            int Max = 0;
            if (BD.COMMANDES_CLIENTS.ToList().Count != 0)
            {
                Max = BD.COMMANDES_CLIENTS.Where(cmd => cmd.DATE.Year == DateTime.Today.Year).Select(cmd => cmd.ID).Count();
            }
            Max++;
            Numero = "CDC" + Max.ToString("0000") + "/" + DateTime.Today.ToString("yy");
            NewElement.CODE = Numero;
            NewElement.DATE = Element.DATE;
            NewElement.MODE_PAIEMENT = Element.MODE_PAIEMENT;
            NewElement.CLIENT = Element.CLIENT;
            NewElement.THT = Element.THT;
            NewElement.TTVA = Element.TTVA;
            NewElement.NHT = Element.NHT;
            NewElement.TTC = Element.TTC;
            NewElement.TNET = Element.TNET;
            NewElement.VALIDER = false;
            NewElement.REMISE = Element.REMISE;
            NewElement.DEVIS_CLIENT = Element.ID;
            NewElement.DEVIS_CLIENTS = Element;
            NewElement.CLIENTS = Element.CLIENTS;
            BD.COMMANDES_CLIENTS.Add(NewElement);
            BD.SaveChanges();
            foreach (LIGNES_DEVIS_CLIENTS Ligne in Liste)
            {
                LIGNES_COMMANDES_CLIENTS NewLine = new LIGNES_COMMANDES_CLIENTS();
                NewLine.PRODUIT = (int)Ligne.PRODUIT;
                NewLine.CODE_PRODUIT = Ligne.CODE_PRODUIT;
                NewLine.DESIGNATION_PRODUIT = Ligne.DESIGNATION_PRODUIT;
                NewLine.QUANTITE = Ligne.QUANTITE;
                NewLine.PRIX_UNITAIRE_HT = Ligne.PRIX_UNITAIRE_HT;
                NewLine.REMISE = Ligne.REMISE;
                NewLine.TOTALE_REMISE_HT = Ligne.TOTALE_REMISE_HT;
                NewLine.TOTALE_HT = Ligne.TOTALE_HT;
                NewLine.TVA = Ligne.TVA;
                NewLine.TOTALE_TTC = Ligne.TOTALE_TTC;
                NewLine.COMMANDE_CLIENT = NewElement.ID;
                NewLine.COMMANDES_CLIENTS = NewElement;
                NewLine.PRODUITS = Ligne.PRODUITS;
                BD.LIGNES_COMMANDES_CLIENTS.Add(NewLine);
                BD.SaveChanges();
                AddMouvementProduit("COMMANDE", NewLine.PRODUITS, NewElement.DATE, NewElement.CODE, (int)NewLine.QUANTITE);
            }
            return NewElement.ID.ToString();
        }
        public string DevisVersfacture(string parampassed)
        {
            int ID = int.Parse(parampassed);
            DEVIS_CLIENTS Element = BD.DEVIS_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            List<LIGNES_DEVIS_CLIENTS> Liste = BD.LIGNES_DEVIS_CLIENTS.Where(cmd => cmd.DEVIS_CLIENT == ID).ToList();
            FACTURES_CLIENTS NewElement = new FACTURES_CLIENTS();
            string Numero = string.Empty;
            int Max = 0;
            if (BD.FACTURES_CLIENTS.ToList().Count != 0)
            {
                Max = BD.FACTURES_CLIENTS.Where(cmd => cmd.DATE.Year == DateTime.Today.Year).Select(cmd => cmd.ID).Count();
            }
            Max++;
            Numero = "FC" + Max.ToString("0000") + "/" + DateTime.Today.ToString("yy");
            NewElement.CODE = Numero;
            NewElement.DATE = Element.DATE;
            NewElement.MODE_PAIEMENT = Element.MODE_PAIEMENT;
            NewElement.CLIENT = Element.CLIENT;
            NewElement.THT = Element.THT;
            NewElement.TTVA = Element.TTVA;
            NewElement.NHT = Element.NHT;
            NewElement.TIMBRE = decimal.Parse("0,5");
            NewElement.TTC = Element.TTC + NewElement.TIMBRE;
            NewElement.TNET = Element.TNET + NewElement.TIMBRE;
            NewElement.VALIDER = false;
            NewElement.REMISE = Element.REMISE;
            NewElement.DEVIS_CLIENT = Element.ID;
            NewElement.DEVIS_CLIENTS = Element;
            NewElement.CLIENTS = Element.CLIENTS;
            BD.FACTURES_CLIENTS.Add(NewElement);
            BD.SaveChanges();
            foreach (LIGNES_DEVIS_CLIENTS Ligne in Liste)
            {
                LIGNES_FACTURES_CLIENTS NewLine = new LIGNES_FACTURES_CLIENTS();
                NewLine.PRODUIT = (int)Ligne.PRODUIT;
                NewLine.CODE_PRODUIT = Ligne.CODE_PRODUIT;
                NewLine.DESIGNATION_PRODUIT = Ligne.DESIGNATION_PRODUIT;
                NewLine.QUANTITE = Ligne.QUANTITE;
                NewLine.PRIX_UNITAIRE_HT = Ligne.PRIX_UNITAIRE_HT;
                NewLine.REMISE = Ligne.REMISE;
                NewLine.TOTALE_REMISE_HT = Ligne.TOTALE_REMISE_HT;
                NewLine.TOTALE_HT = Ligne.TOTALE_HT;
                NewLine.TVA = Ligne.TVA;
                NewLine.TOTALE_TTC = Ligne.TOTALE_TTC;
                NewLine.FACTURE_CLIENT = NewElement.ID;
                NewLine.FACTURES_CLIENTS = NewElement;
                NewLine.PRODUITS = Ligne.PRODUITS;
                BD.LIGNES_FACTURES_CLIENTS.Add(NewLine);
                BD.SaveChanges();
                AddMouvementProduit("FACTURE", NewLine.PRODUITS, NewElement.DATE, NewElement.CODE, (int)NewLine.QUANTITE);
            }
            return NewElement.ID.ToString();
        }
        public string CommandeVersBonLivraison(string parampassed)
        {
            int ID = int.Parse(parampassed);
            COMMANDES_CLIENTS Element = BD.COMMANDES_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            List<LIGNES_COMMANDES_CLIENTS> Liste = BD.LIGNES_COMMANDES_CLIENTS.Where(cmd => cmd.COMMANDE_CLIENT == ID).ToList();
            BONS_LIVRAISONS_CLIENTS NewElement = new BONS_LIVRAISONS_CLIENTS();
            string Numero = string.Empty;
            int Max = 0;
            if (BD.BONS_LIVRAISONS_CLIENTS.ToList().Count != 0)
            {
                Max = BD.BONS_LIVRAISONS_CLIENTS.Where(cmd => cmd.DATE.Year == DateTime.Today.Year).Select(cmd => cmd.ID).Count();
            }
            Max++;
            Numero = "BLC" + Max.ToString("0000") + "/" + DateTime.Today.ToString("yy");
            NewElement.CODE = Numero;
            NewElement.DATE = Element.DATE;
            NewElement.MODE_PAIEMENT = Element.MODE_PAIEMENT;
            NewElement.CLIENT = Element.CLIENT;
            NewElement.THT = Element.THT;
            NewElement.TTVA = Element.TTVA;
            NewElement.NHT = Element.NHT;
            NewElement.TTC = Element.TTC;
            NewElement.TNET = Element.TNET;
            NewElement.VALIDER = false;
            NewElement.REMISE = Element.REMISE;
            NewElement.COMMANDE_CLIENT = Element.ID;
            NewElement.COMMANDES_CLIENTS = Element;
            NewElement.CLIENTS = Element.CLIENTS;
            BD.BONS_LIVRAISONS_CLIENTS.Add(NewElement);
            BD.SaveChanges();
            foreach (LIGNES_COMMANDES_CLIENTS Ligne in Liste)
            {
                LIGNES_BONS_LIVRAISONS_CLIENTS NewLine = new LIGNES_BONS_LIVRAISONS_CLIENTS();
                NewLine.PRODUIT = (int)Ligne.PRODUIT;
                NewLine.CODE_PRODUIT = Ligne.CODE_PRODUIT;
                NewLine.DESIGNATION_PRODUIT = Ligne.DESIGNATION_PRODUIT;
                NewLine.QUANTITE = Ligne.QUANTITE;
                NewLine.PRIX_UNITAIRE_HT = Ligne.PRIX_UNITAIRE_HT;
                NewLine.REMISE = Ligne.REMISE;
                NewLine.TOTALE_REMISE_HT = Ligne.TOTALE_REMISE_HT;
                NewLine.TOTALE_HT = Ligne.TOTALE_HT;
                NewLine.TVA = Ligne.TVA;
                NewLine.TOTALE_TTC = Ligne.TOTALE_TTC;
                NewLine.BON_LIVRAISON_CLIENT = NewElement.ID;
                NewLine.BONS_LIVRAISONS_CLIENTS = NewElement;
                NewLine.PRODUITS = Ligne.PRODUITS;
                BD.LIGNES_BONS_LIVRAISONS_CLIENTS.Add(NewLine);
                BD.SaveChanges();
                AddMouvementProduit("BON_LIVRAISON", NewLine.PRODUITS, NewElement.DATE, NewElement.CODE, (int)NewLine.QUANTITE);
            }
            return NewElement.ID.ToString();
        }
        public string CommandeVersfacture(string parampassed)
        {
            int ID = int.Parse(parampassed);
            COMMANDES_CLIENTS Element = BD.COMMANDES_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            List<LIGNES_COMMANDES_CLIENTS> Liste = BD.LIGNES_COMMANDES_CLIENTS.Where(cmd => cmd.COMMANDE_CLIENT == ID).ToList();
            FACTURES_CLIENTS NewElement = new FACTURES_CLIENTS();
            string Numero = string.Empty;
            int Max = 0;
            if (BD.FACTURES_CLIENTS.ToList().Count != 0)
            {
                Max = BD.FACTURES_CLIENTS.Where(cmd => cmd.DATE.Year == DateTime.Today.Year).Select(cmd => cmd.ID).Count();
            }
            Max++;
            Numero = "FC" + Max.ToString("0000") + "/" + DateTime.Today.ToString("yy");
            NewElement.CODE = Numero;
            NewElement.DATE = Element.DATE;
            NewElement.MODE_PAIEMENT = Element.MODE_PAIEMENT;
            NewElement.CLIENT = Element.CLIENT;
            NewElement.THT = Element.THT;
            NewElement.TTVA = Element.TTVA;
            NewElement.NHT = Element.NHT;
            NewElement.TIMBRE = decimal.Parse("0,5");
            NewElement.TTC = Element.TTC + NewElement.TIMBRE;
            NewElement.TNET = Element.TNET + NewElement.TIMBRE;
            NewElement.VALIDER = false;
            NewElement.REMISE = Element.REMISE;
            NewElement.COMMANDE_CLIENT = Element.ID;
            NewElement.COMMANDES_CLIENTS = Element;
            NewElement.CLIENTS = Element.CLIENTS;
            BD.FACTURES_CLIENTS.Add(NewElement);
            BD.SaveChanges();
            foreach (LIGNES_COMMANDES_CLIENTS Ligne in Liste)
            {
                LIGNES_FACTURES_CLIENTS NewLine = new LIGNES_FACTURES_CLIENTS();
                NewLine.PRODUIT = (int)Ligne.PRODUIT;
                NewLine.CODE_PRODUIT = Ligne.CODE_PRODUIT;
                NewLine.DESIGNATION_PRODUIT = Ligne.DESIGNATION_PRODUIT;
                NewLine.QUANTITE = Ligne.QUANTITE;
                NewLine.PRIX_UNITAIRE_HT = Ligne.PRIX_UNITAIRE_HT;
                NewLine.REMISE = Ligne.REMISE;
                NewLine.TOTALE_REMISE_HT = Ligne.TOTALE_REMISE_HT;
                NewLine.TOTALE_HT = Ligne.TOTALE_HT;
                NewLine.TVA = Ligne.TVA;
                NewLine.TOTALE_TTC = Ligne.TOTALE_TTC;
                NewLine.FACTURE_CLIENT = NewElement.ID;
                NewLine.FACTURES_CLIENTS = NewElement;
                NewLine.PRODUITS = Ligne.PRODUITS;
                BD.LIGNES_FACTURES_CLIENTS.Add(NewLine);
                BD.SaveChanges();
                AddMouvementProduit("FACTURE", NewLine.PRODUITS, NewElement.DATE, NewElement.CODE, (int)NewLine.QUANTITE);
            }
            return NewElement.ID.ToString();
        }
        public string BonLivraisonVersfacture(string parampassed)
        {
            int ID = int.Parse(parampassed);
            BONS_LIVRAISONS_CLIENTS Element = BD.BONS_LIVRAISONS_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            if (Element.VALIDER)
            {
                List<LIGNES_BONS_LIVRAISONS_CLIENTS> Liste = BD.LIGNES_BONS_LIVRAISONS_CLIENTS.Where(cmd => cmd.BON_LIVRAISON_CLIENT == ID).ToList();
                FACTURES_CLIENTS NewElement = new FACTURES_CLIENTS();
                string Numero = string.Empty;
                int Max = 0;
                if (BD.FACTURES_CLIENTS.ToList().Count != 0)
                {
                    Max = BD.FACTURES_CLIENTS.Where(cmd => cmd.DATE.Year == DateTime.Today.Year).Select(cmd => cmd.ID).Count();
                }
                Max++;
                Numero = "FC" + Max.ToString("0000") + "/" + DateTime.Today.ToString("yy");
                NewElement.CODE = Numero;
                NewElement.DATE = Element.DATE;
                NewElement.MODE_PAIEMENT = Element.MODE_PAIEMENT;
                NewElement.CLIENT = Element.CLIENT;
                NewElement.THT = Element.THT;
                NewElement.TTVA = Element.TTVA;
                NewElement.NHT = Element.NHT;
                NewElement.TIMBRE = decimal.Parse("0,5");
                NewElement.TTC = Element.TTC + NewElement.TIMBRE;
                NewElement.TNET = Element.TNET + NewElement.TIMBRE;
                NewElement.VALIDER = false;
                NewElement.REMISE = Element.REMISE;
                NewElement.BON_LIVRAISON_CLIENT = Element.ID;
                NewElement.BONS_LIVRAISONS_CLIENTS = Element;
                NewElement.CLIENTS = Element.CLIENTS;
                BD.FACTURES_CLIENTS.Add(NewElement);
                BD.SaveChanges();
                foreach (LIGNES_BONS_LIVRAISONS_CLIENTS Ligne in Liste)
                {
                    LIGNES_FACTURES_CLIENTS NewLine = new LIGNES_FACTURES_CLIENTS();
                    NewLine.PRODUIT = (int)Ligne.PRODUIT;
                    NewLine.CODE_PRODUIT = Ligne.CODE_PRODUIT;
                    NewLine.DESIGNATION_PRODUIT = Ligne.DESIGNATION_PRODUIT;
                    NewLine.QUANTITE = Ligne.QUANTITE;
                    NewLine.PRIX_UNITAIRE_HT = Ligne.PRIX_UNITAIRE_HT;
                    NewLine.REMISE = Ligne.REMISE;
                    NewLine.TOTALE_REMISE_HT = Ligne.TOTALE_REMISE_HT;
                    NewLine.TOTALE_HT = Ligne.TOTALE_HT;
                    NewLine.TVA = Ligne.TVA;
                    NewLine.TOTALE_TTC = Ligne.TOTALE_TTC;
                    NewLine.FACTURE_CLIENT = NewElement.ID;
                    NewLine.FACTURES_CLIENTS = NewElement;
                    NewLine.PRODUITS = Ligne.PRODUITS;
                    BD.LIGNES_FACTURES_CLIENTS.Add(NewLine);
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
            FACTURES_CLIENTS Element = BD.FACTURES_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
            if (Element.VALIDER)
            {
                List<LIGNES_FACTURES_CLIENTS> Liste = BD.LIGNES_FACTURES_CLIENTS.Where(cmd => cmd.FACTURE_CLIENT == ID).ToList();
                AVOIRS_CLIENTS NewElement = new AVOIRS_CLIENTS();
                string Numero = string.Empty;
                int Max = 0;
                if (BD.AVOIRS_CLIENTS.ToList().Count != 0)
                {
                    Max = BD.AVOIRS_CLIENTS.Where(cmd => cmd.DATE.Year == DateTime.Today.Year).Select(cmd => cmd.ID).Count();
                }
                Max++;
                Numero = "AVC" + Max.ToString("0000") + "/" + DateTime.Today.ToString("yy");
                NewElement.CODE = Numero;
                NewElement.DATE = Element.DATE;
                NewElement.MODE_PAIEMENT = Element.MODE_PAIEMENT;
                NewElement.CLIENT = Element.CLIENT;
                NewElement.THT = Element.THT;
                NewElement.TTVA = Element.TTVA;
                NewElement.NHT = Element.NHT;
                NewElement.TTC = Element.TTC + Element.TIMBRE;
                NewElement.TNET = Element.TNET + Element.TIMBRE;
                NewElement.VALIDER = false;
                NewElement.REMISE = Element.REMISE;
                NewElement.FACTURE_CLIENT = Element.ID;
                NewElement.FACTURES_CLIENTS = Element;
                NewElement.CLIENTS = Element.CLIENTS;
                BD.AVOIRS_CLIENTS.Add(NewElement);
                BD.SaveChanges();
                foreach (LIGNES_FACTURES_CLIENTS Ligne in Liste)
                {
                    LIGNES_AVOIRS_CLIENTS NewLine = new LIGNES_AVOIRS_CLIENTS();
                    NewLine.PRODUIT = (int)Ligne.PRODUIT;
                    NewLine.CODE_PRODUIT = Ligne.CODE_PRODUIT;
                    NewLine.DESIGNATION_PRODUIT = Ligne.DESIGNATION_PRODUIT;
                    NewLine.QUANTITE = Ligne.QUANTITE;
                    NewLine.PRIX_UNITAIRE_HT = Ligne.PRIX_UNITAIRE_HT;
                    NewLine.REMISE = Ligne.REMISE;
                    NewLine.TOTALE_REMISE_HT = Ligne.TOTALE_REMISE_HT;
                    NewLine.TOTALE_HT = Ligne.TOTALE_HT;
                    NewLine.TVA = Ligne.TVA;
                    NewLine.TOTALE_TTC = Ligne.TOTALE_TTC;
                    NewLine.AVOIR_CLIENT = NewElement.ID;
                    NewLine.AVOIRS_CLIENTS = NewElement;
                    NewLine.PRODUITS = Ligne.PRODUITS;
                    BD.LIGNES_AVOIRS_CLIENTS.Add(NewLine);
                    BD.SaveChanges();
                    AddMouvementProduit("AVOIR", NewLine.PRODUITS, NewElement.DATE, NewElement.CODE, (int)NewLine.QUANTITE);
                }
                return NewElement.ID.ToString();
            }
            return "NO";
        }
        #endregion
        public string AddLineDevis(string ID_Produit, string LIB_Produi, string Description_Produit, string Quantite_Produit, string PUHT_Produit, string Remise_Produit, string PTHT_Produit, string TVA_Produit, string TTC_Produit)
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
            if (Session["ProduitsDevisClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsDevisClient"];
            }
            if (!ListeDesPoduits.Select(prd => prd.ID).Contains(ligne.ID))
            {
                ListeDesPoduits.Add(ligne);
            }
            Session["ProduitsDevisClient"] = ListeDesPoduits;
            return string.Empty;
        }
        public string AddLineCommande(string ID_Produit, string LIB_Produi, string Description_Produit, string Quantite_Produit, string PUHT_Produit, string Remise_Produit, string PTHT_Produit, string TVA_Produit, string TTC_Produit)
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
            if (Session["ProduitsCommandeClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsCommandeClient"];
            }
            if (!ListeDesPoduits.Select(prd => prd.ID).Contains(ligne.ID))
            {
                ListeDesPoduits.Add(ligne);
            }
            Session["ProduitsCommandeClient"] = ListeDesPoduits;
            return string.Empty;
        }
        public string AddLineBonLivraison(string ID_Produit, string LIB_Produi, string Description_Produit, string Quantite_Produit, string PUHT_Produit, string Remise_Produit, string PTHT_Produit, string TVA_Produit, string TTC_Produit)
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
            if (Session["ProduitsBonLivraisonClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsBonLivraisonClient"];
            }
            if (!ListeDesPoduits.Select(prd => prd.ID).Contains(ligne.ID))
            {
                ListeDesPoduits.Add(ligne);
            }
            Session["ProduitsBonLivraisonClient"] = ListeDesPoduits;
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
            if (Session["ProduitsFactureClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsFactureClient"];
            }
            if (!ListeDesPoduits.Select(prd => prd.ID).Contains(ligne.ID))
            {
                ListeDesPoduits.Add(ligne);
            }
            Session["ProduitsFactureClient"] = ListeDesPoduits;
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
            if (Session["ProduitsAvoirClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsAvoirClient"];
            }
            if (!ListeDesPoduits.Select(prd => prd.ID).Contains(ligne.ID))
            {
                ListeDesPoduits.Add(ligne);
            }
            Session["ProduitsAvoirClient"] = ListeDesPoduits;
            return string.Empty;
        }
        
        public string EditLineDevis(string ID_Produit, string Quantite_Produit, string PUHT_Produit, string Remise_Produit, string PTHT_Produit, string TVA_Produit, string TTC_Produit)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsDevisClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsDevisClient"];
            }
            int ID = int.Parse(ID_Produit);
            LigneProduit ligne = ListeDesPoduits.Where(pr => pr.ID == ID).FirstOrDefault();
            ligne.QUANTITE = int.Parse(Quantite_Produit);
            ligne.PRIX_VENTE_HT = decimal.Parse(PUHT_Produit, CultureInfo.InvariantCulture);
            ligne.REMISE = int.Parse(Remise_Produit);
            ligne.PTHT = decimal.Parse(PTHT_Produit, CultureInfo.InvariantCulture);
            ligne.TVA = int.Parse(TVA_Produit);
            ligne.TTC = decimal.Parse(TTC_Produit, CultureInfo.InvariantCulture);
            Session["ProduitsDevisClient"] = ListeDesPoduits;
            return string.Empty;
        }
        public string EditLineCommande(string ID_Produit, string Quantite_Produit, string PUHT_Produit, string Remise_Produit, string PTHT_Produit, string TVA_Produit, string TTC_Produit)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsCommandeClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsCommandeClient"];
            }
            int ID = int.Parse(ID_Produit);
            LigneProduit ligne = ListeDesPoduits.Where(pr => pr.ID == ID).FirstOrDefault();
            ligne.QUANTITE = int.Parse(Quantite_Produit);
            ligne.PRIX_VENTE_HT = decimal.Parse(PUHT_Produit, CultureInfo.InvariantCulture);
            ligne.REMISE = int.Parse(Remise_Produit);
            ligne.PTHT = decimal.Parse(PTHT_Produit, CultureInfo.InvariantCulture);
            ligne.TVA = int.Parse(TVA_Produit);
            ligne.TTC = decimal.Parse(TTC_Produit, CultureInfo.InvariantCulture);
            Session["ProduitsCommandeClient"] = ListeDesPoduits;
            return string.Empty;
        }
        public string EditLineBonLivraison(string ID_Produit, string Quantite_Produit, string PUHT_Produit, string Remise_Produit, string PTHT_Produit, string TVA_Produit, string TTC_Produit)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsBonLivraisonClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsBonLivraisonClient"];
            }
            int ID = int.Parse(ID_Produit);
            LigneProduit ligne = ListeDesPoduits.Where(pr => pr.ID == ID).FirstOrDefault();
            ligne.QUANTITE = int.Parse(Quantite_Produit);
            ligne.PRIX_VENTE_HT = decimal.Parse(PUHT_Produit, CultureInfo.InvariantCulture);
            ligne.REMISE = int.Parse(Remise_Produit);
            ligne.PTHT = decimal.Parse(PTHT_Produit, CultureInfo.InvariantCulture);
            ligne.TVA = int.Parse(TVA_Produit);
            ligne.TTC = decimal.Parse(TTC_Produit, CultureInfo.InvariantCulture);
            Session["ProduitsBonLivraisonClient"] = ListeDesPoduits;
            return string.Empty;
        }
        public string EditLineFacture(string ID_Produit, string Quantite_Produit, string PUHT_Produit, string Remise_Produit, string PTHT_Produit, string TVA_Produit, string TTC_Produit)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsFactureClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsFactureClient"];
            }
            int ID = int.Parse(ID_Produit);
            LigneProduit ligne = ListeDesPoduits.Where(pr => pr.ID == ID).FirstOrDefault();
            ligne.QUANTITE = int.Parse(Quantite_Produit);
            ligne.PRIX_VENTE_HT = decimal.Parse(PUHT_Produit, CultureInfo.InvariantCulture);
            ligne.REMISE = int.Parse(Remise_Produit);
            ligne.PTHT = decimal.Parse(PTHT_Produit, CultureInfo.InvariantCulture);
            ligne.TVA = int.Parse(TVA_Produit);
            ligne.TTC = decimal.Parse(TTC_Produit, CultureInfo.InvariantCulture);
            Session["ProduitsFactureClient"] = ListeDesPoduits;
            return string.Empty;
        }
        public string EditLineAvoir(string ID_Produit, string Quantite_Produit, string PUHT_Produit, string Remise_Produit, string PTHT_Produit, string TVA_Produit, string TTC_Produit)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsAvoirClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsAvoirClient"];
            }
            int ID = int.Parse(ID_Produit);
            LigneProduit ligne = ListeDesPoduits.Where(pr => pr.ID == ID).FirstOrDefault();
            ligne.QUANTITE = int.Parse(Quantite_Produit);
            ligne.PRIX_VENTE_HT = decimal.Parse(PUHT_Produit, CultureInfo.InvariantCulture);
            ligne.REMISE = int.Parse(Remise_Produit);
            ligne.PTHT = decimal.Parse(PTHT_Produit, CultureInfo.InvariantCulture);
            ligne.TVA = int.Parse(TVA_Produit);
            ligne.TTC = decimal.Parse(TTC_Produit, CultureInfo.InvariantCulture);
            Session["ProduitsAvoirClient"] = ListeDesPoduits;
            return string.Empty;
        }
        
        public string DeleteLineDevis(string parampassed)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsDevisClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsDevisClient"];
            }
            int ID = int.Parse(parampassed);
            LigneProduit ligne = ListeDesPoduits.Where(pr => pr.ID == ID).FirstOrDefault();
            ListeDesPoduits.Remove(ligne);
            Session["ProduitsDevisClient"] = ListeDesPoduits;
            return string.Empty;
        }
        public string DeleteLineCommande(string parampassed)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsCommandeClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsCommandeClient"];
            }
            int ID = int.Parse(parampassed);
            LigneProduit ligne = ListeDesPoduits.Where(pr => pr.ID == ID).FirstOrDefault();
            ListeDesPoduits.Remove(ligne);
            Session["ProduitsCommandeClient"] = ListeDesPoduits;
            return string.Empty;
        }
        public string DeleteLineBonLivraison(string parampassed)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsBonLivraisonClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsBonLivraisonClient"];
            }
            int ID = int.Parse(parampassed);
            LigneProduit ligne = ListeDesPoduits.Where(pr => pr.ID == ID).FirstOrDefault();
            ListeDesPoduits.Remove(ligne);
            Session["ProduitsBonLivraisonClient"] = ListeDesPoduits;
            return string.Empty;
        }
        public string DeleteLineFacture(string parampassed)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsFactureClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsFactureClient"];
            }
            int ID = int.Parse(parampassed);
            LigneProduit ligne = ListeDesPoduits.Where(pr => pr.ID == ID).FirstOrDefault();
            ListeDesPoduits.Remove(ligne);
            Session["ProduitsFactureClient"] = ListeDesPoduits;
            return string.Empty;
        }
        public string DeleteLineAvoir(string parampassed)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsAvoirClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsAvoirClient"];
            }
            int ID = int.Parse(parampassed);
            LigneProduit ligne = ListeDesPoduits.Where(pr => pr.ID == ID).FirstOrDefault();
            ListeDesPoduits.Remove(ligne);
            Session["ProduitsAvoirClient"] = ListeDesPoduits;
            return string.Empty;
        }
        
        public JsonResult GetAllLineDevis()
        {
            BD.Configuration.ProxyCreationEnabled = false;
            List<LigneProduit> ListeDesPoduits = (List<LigneProduit>)Session["ProduitsDevisClient"];
            return Json(ListeDesPoduits, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllLineCommande()
        {
            BD.Configuration.ProxyCreationEnabled = false;
            List<LigneProduit> ListeDesPoduits = (List<LigneProduit>)Session["ProduitsCommandeClient"];
            return Json(ListeDesPoduits, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllLineBonLivraison()
        {
            BD.Configuration.ProxyCreationEnabled = false;
            List<LigneProduit> ListeDesPoduits = (List<LigneProduit>)Session["ProduitsBonLivraisonClient"];
            return Json(ListeDesPoduits, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllLineFacture()
        {
            BD.Configuration.ProxyCreationEnabled = false;
            List<LigneProduit> ListeDesPoduits = (List<LigneProduit>)Session["ProduitsFactureClient"];
            return Json(ListeDesPoduits, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllLineAvoir()
        {
            BD.Configuration.ProxyCreationEnabled = false;
            List<LigneProduit> ListeDesPoduits = (List<LigneProduit>)Session["ProduitsAvoirClient"];
            return Json(ListeDesPoduits, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult UpdatePriceDevis(string remise)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsDevisClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsDevisClient"];
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
        public JsonResult UpdatePriceCommande(string remise)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsCommandeClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsCommandeClient"];
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
        public JsonResult UpdatePriceBonLivraison(string remise)
        {
            List<LigneProduit> ListeDesPoduits = new List<LigneProduit>();
            if (Session["ProduitsBonLivraisonClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsBonLivraisonClient"];
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
            if (Session["ProduitsFactureClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsFactureClient"];
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
            if (Session["ProduitsAvoirClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsAvoirClient"];
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
        public ActionResult SendDevis(string Mode, string Code)
        {
            string Numero = Request["numero"] != null ? Request["numero"].ToString() : string.Empty;
            string date = Request["date"] != null ? Request["date"].ToString() : string.Empty;
            string client = Request["client"] != null ? Request["client"].ToString() : string.Empty;
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
            string SelectedDevis = string.Empty;
            if (Session["ProduitsDevisClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsDevisClient"];
            }
            if (Mode == "Create")
            {
                if (!BD.DEVIS_CLIENTS.Select(cmd => cmd.CODE).Contains(Numero))
                {
                    DEVIS_CLIENTS DevisClient = new DEVIS_CLIENTS();
                    DevisClient.CODE = Numero;
                    DevisClient.DATE = DateTime.Parse(date);
                    DevisClient.CLIENT = int.Parse(client);
                    int ID_CLIENT = int.Parse(client);
                    DevisClient.CLIENTS = BD.CLIENTS.Where(fou => fou.ID == ID_CLIENT).FirstOrDefault();
                    DevisClient.MODE_PAIEMENT = modePaiement;
                    DevisClient.REMISE = decimal.Parse(remise, CultureInfo.InvariantCulture);
                    DevisClient.THT = decimal.Parse(totalHT, CultureInfo.InvariantCulture);
                    DevisClient.NHT = decimal.Parse(NetHT, CultureInfo.InvariantCulture);
                    DevisClient.TTVA = decimal.Parse(totalTVA, CultureInfo.InvariantCulture);
                    DevisClient.TTC = decimal.Parse(TotalTTC, CultureInfo.InvariantCulture);
                    DevisClient.TNET = decimal.Parse(netAPaye, CultureInfo.InvariantCulture);
                    BD.DEVIS_CLIENTS.Add(DevisClient);
                    BD.SaveChanges();
                    foreach (LigneProduit Ligne in ListeDesPoduits)
                    {
                        LIGNES_DEVIS_CLIENTS UneLigne = new LIGNES_DEVIS_CLIENTS();
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
                        UneLigne.DEVIS_CLIENT = DevisClient.ID;
                        UneLigne.DEVIS_CLIENTS = DevisClient;
                        UneLigne.PRODUITS = Produit;
                        BD.LIGNES_DEVIS_CLIENTS.Add(UneLigne);
                        BD.SaveChanges();
                        AddMouvementProduit("DEVIS", Produit, DevisClient.DATE, DevisClient.CODE, Ligne.QUANTITE);
                    }
                    SelectedDevis = DevisClient.ID.ToString();
                }
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                DEVIS_CLIENTS DevisClient = BD.DEVIS_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
                DevisClient.CODE = Numero;
                DevisClient.DATE = DateTime.Parse(date);
                DevisClient.CLIENT = int.Parse(client);
                int ID_CLIENT = int.Parse(client);
                DevisClient.CLIENTS = BD.CLIENTS.Where(fou => fou.ID == ID_CLIENT).FirstOrDefault();
                DevisClient.MODE_PAIEMENT = modePaiement;
                DevisClient.REMISE = decimal.Parse(remise, CultureInfo.InvariantCulture);
                DevisClient.THT = decimal.Parse(totalHT, CultureInfo.InvariantCulture);
                DevisClient.NHT = decimal.Parse(NetHT, CultureInfo.InvariantCulture);
                DevisClient.TTVA = decimal.Parse(totalTVA, CultureInfo.InvariantCulture);
                DevisClient.TTC = decimal.Parse(TotalTTC, CultureInfo.InvariantCulture);
                DevisClient.TNET = decimal.Parse(netAPaye, CultureInfo.InvariantCulture);
                BD.SaveChanges();
                BD.LIGNES_DEVIS_CLIENTS.Where(p => p.DEVIS_CLIENT == DevisClient.ID).ToList().ForEach(p => BD.LIGNES_DEVIS_CLIENTS.Remove(p));
                BD.SaveChanges();
                foreach (LigneProduit Ligne in ListeDesPoduits)
                {
                    LIGNES_DEVIS_CLIENTS UneLigne = new LIGNES_DEVIS_CLIENTS();
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
                    UneLigne.DEVIS_CLIENT = DevisClient.ID;
                    UneLigne.DEVIS_CLIENTS = DevisClient;
                    UneLigne.PRODUITS = Produit;
                    BD.LIGNES_DEVIS_CLIENTS.Add(UneLigne);
                    BD.SaveChanges();
                }
                SelectedDevis = DevisClient.ID.ToString();
            }
            if (Print)
            {
                return RedirectToAction("PrintDevisClientByID", new { CODE = SelectedDevis });
            }
            Session["ProduitsDevisClient"] = null;
            ViewData["MODE"] = Mode;
            ViewBag.MODE = Mode;
            return RedirectToAction("Devis", new { MODE = Mode });
        }
        [HttpPost]
        public ActionResult SendCommande(string Mode, string Code)
        {
            string Numero = Request["numero"] != null ? Request["numero"].ToString() : string.Empty;
            string date = Request["date"] != null ? Request["date"].ToString() : string.Empty;
            string client = Request["client"] != null ? Request["client"].ToString() : string.Empty;
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
            if (Session["ProduitsCommandeClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsCommandeClient"];
            }
            if (Mode == "Create")
            {
                if (!BD.COMMANDES_CLIENTS.Select(cmd => cmd.CODE).Contains(Numero))
                {
                    COMMANDES_CLIENTS CommandeClient = new COMMANDES_CLIENTS();
                    CommandeClient.CODE = Numero;
                    CommandeClient.DATE = DateTime.Parse(date);
                    CommandeClient.CLIENT = int.Parse(client);
                    int ID_CLIENT = int.Parse(client);
                    CommandeClient.CLIENTS = BD.CLIENTS.Where(fou => fou.ID == ID_CLIENT).FirstOrDefault();
                    CommandeClient.MODE_PAIEMENT = modePaiement;
                    CommandeClient.REMISE = decimal.Parse(remise, CultureInfo.InvariantCulture);
                    CommandeClient.THT = decimal.Parse(totalHT, CultureInfo.InvariantCulture);
                    CommandeClient.NHT = decimal.Parse(NetHT, CultureInfo.InvariantCulture);
                    CommandeClient.TTVA = decimal.Parse(totalTVA, CultureInfo.InvariantCulture);
                    CommandeClient.TTC = decimal.Parse(TotalTTC, CultureInfo.InvariantCulture);
                    CommandeClient.TNET = decimal.Parse(netAPaye, CultureInfo.InvariantCulture);
                    BD.COMMANDES_CLIENTS.Add(CommandeClient);
                    BD.SaveChanges();
                    foreach (LigneProduit Ligne in ListeDesPoduits)
                    {
                        LIGNES_COMMANDES_CLIENTS UneLigne = new LIGNES_COMMANDES_CLIENTS();
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
                        UneLigne.COMMANDE_CLIENT = CommandeClient.ID;
                        UneLigne.COMMANDES_CLIENTS = CommandeClient;
                        UneLigne.PRODUITS = Produit;
                        BD.LIGNES_COMMANDES_CLIENTS.Add(UneLigne);
                        BD.SaveChanges();
                        AddMouvementProduit("COMMANDE", Produit, CommandeClient.DATE, CommandeClient.CODE, Ligne.QUANTITE);
                    }
                    SelectedCommande = CommandeClient.ID.ToString();
                }
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                COMMANDES_CLIENTS CommandeClient = BD.COMMANDES_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
                CommandeClient.CODE = Numero;
                CommandeClient.DATE = DateTime.Parse(date);
                CommandeClient.CLIENT = int.Parse(client);
                int ID_CLIENT = int.Parse(client);
                CommandeClient.CLIENTS = BD.CLIENTS.Where(fou => fou.ID == ID_CLIENT).FirstOrDefault();
                CommandeClient.MODE_PAIEMENT = modePaiement;
                CommandeClient.REMISE = decimal.Parse(remise, CultureInfo.InvariantCulture);
                CommandeClient.THT = decimal.Parse(totalHT, CultureInfo.InvariantCulture);
                CommandeClient.NHT = decimal.Parse(NetHT, CultureInfo.InvariantCulture);
                CommandeClient.TTVA = decimal.Parse(totalTVA, CultureInfo.InvariantCulture);
                CommandeClient.TTC = decimal.Parse(TotalTTC, CultureInfo.InvariantCulture);
                CommandeClient.TNET = decimal.Parse(netAPaye, CultureInfo.InvariantCulture);
                BD.SaveChanges();
                BD.LIGNES_COMMANDES_CLIENTS.Where(p => p.COMMANDE_CLIENT == CommandeClient.ID).ToList().ForEach(p => BD.LIGNES_COMMANDES_CLIENTS.Remove(p));
                BD.SaveChanges();
                foreach (LigneProduit Ligne in ListeDesPoduits)
                {
                    LIGNES_COMMANDES_CLIENTS UneLigne = new LIGNES_COMMANDES_CLIENTS();
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
                    UneLigne.COMMANDE_CLIENT = CommandeClient.ID;
                    UneLigne.COMMANDES_CLIENTS = CommandeClient;
                    UneLigne.PRODUITS = Produit;
                    BD.LIGNES_COMMANDES_CLIENTS.Add(UneLigne);
                    BD.SaveChanges();
                }
                SelectedCommande = CommandeClient.ID.ToString();
            }
            if (Print)
            {
                return RedirectToAction("PrintCommandeClientByID", new { CODE = SelectedCommande });
            }
            Session["ProduitsCommandeClient"] = null;
            ViewData["MODE"] = Mode;
            ViewBag.MODE = Mode;
            return RedirectToAction("Commandes", new { MODE = Mode });
        }
        [HttpPost]
        public ActionResult SendBonLivraison(string Mode, string Code)
        {
            string Numero = Request["numero"] != null ? Request["numero"].ToString() : string.Empty;
            string date = Request["date"] != null ? Request["date"].ToString() : string.Empty;
            string client = Request["client"] != null ? Request["client"].ToString() : string.Empty;
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
            string SelectedBonLivraison = string.Empty;
            if (Session["ProduitsBonLivraisonClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsBonLivraisonClient"];
            }
            if (Mode == "Create")
            {
                if (!BD.BONS_LIVRAISONS_CLIENTS.Select(cmd => cmd.CODE).Contains(Numero))
                {
                    BONS_LIVRAISONS_CLIENTS BonLivraisonClient = new BONS_LIVRAISONS_CLIENTS();
                    BonLivraisonClient.CODE = Numero;
                    BonLivraisonClient.DATE = DateTime.Parse(date);
                    BonLivraisonClient.CLIENT = int.Parse(client);
                    int ID_CLIENT = int.Parse(client);
                    BonLivraisonClient.CLIENTS = BD.CLIENTS.Where(fou => fou.ID == ID_CLIENT).FirstOrDefault();
                    BonLivraisonClient.MODE_PAIEMENT = modePaiement;
                    BonLivraisonClient.REMISE = decimal.Parse(remise, CultureInfo.InvariantCulture);
                    BonLivraisonClient.THT = decimal.Parse(totalHT, CultureInfo.InvariantCulture);
                    BonLivraisonClient.NHT = decimal.Parse(NetHT, CultureInfo.InvariantCulture);
                    BonLivraisonClient.TTVA = decimal.Parse(totalTVA, CultureInfo.InvariantCulture);
                    BonLivraisonClient.TTC = decimal.Parse(TotalTTC, CultureInfo.InvariantCulture);
                    BonLivraisonClient.TNET = decimal.Parse(netAPaye, CultureInfo.InvariantCulture);
                    BD.BONS_LIVRAISONS_CLIENTS.Add(BonLivraisonClient);
                    BD.SaveChanges();
                    foreach (LigneProduit Ligne in ListeDesPoduits)
                    {
                        LIGNES_BONS_LIVRAISONS_CLIENTS UneLigne = new LIGNES_BONS_LIVRAISONS_CLIENTS();
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
                        UneLigne.BON_LIVRAISON_CLIENT = BonLivraisonClient.ID;
                        UneLigne.BONS_LIVRAISONS_CLIENTS = BonLivraisonClient;
                        UneLigne.PRODUITS = Produit;
                        BD.LIGNES_BONS_LIVRAISONS_CLIENTS.Add(UneLigne);
                        BD.SaveChanges();
                        AddMouvementProduit("BON_LIVRAISON", Produit, BonLivraisonClient.DATE, BonLivraisonClient.CODE, Ligne.QUANTITE);
                    }
                    SelectedBonLivraison = BonLivraisonClient.ID.ToString();
                }
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                BONS_LIVRAISONS_CLIENTS BonLivraisonClient = BD.BONS_LIVRAISONS_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
                BonLivraisonClient.CODE = Numero;
                BonLivraisonClient.DATE = DateTime.Parse(date);
                BonLivraisonClient.CLIENT = int.Parse(client);
                int ID_CLIENT = int.Parse(client);
                BonLivraisonClient.CLIENTS = BD.CLIENTS.Where(fou => fou.ID == ID_CLIENT).FirstOrDefault();
                BonLivraisonClient.MODE_PAIEMENT = modePaiement;
                BonLivraisonClient.REMISE = decimal.Parse(remise, CultureInfo.InvariantCulture);
                BonLivraisonClient.THT = decimal.Parse(totalHT, CultureInfo.InvariantCulture);
                BonLivraisonClient.NHT = decimal.Parse(NetHT, CultureInfo.InvariantCulture);
                BonLivraisonClient.TTVA = decimal.Parse(totalTVA, CultureInfo.InvariantCulture);
                BonLivraisonClient.TTC = decimal.Parse(TotalTTC, CultureInfo.InvariantCulture);
                BonLivraisonClient.TNET = decimal.Parse(netAPaye, CultureInfo.InvariantCulture);
                BD.SaveChanges();
                BD.LIGNES_BONS_LIVRAISONS_CLIENTS.Where(p => p.BON_LIVRAISON_CLIENT == BonLivraisonClient.ID).ToList().ForEach(p => BD.LIGNES_BONS_LIVRAISONS_CLIENTS.Remove(p));
                BD.SaveChanges();
                foreach (LigneProduit Ligne in ListeDesPoduits)
                {
                    LIGNES_BONS_LIVRAISONS_CLIENTS UneLigne = new LIGNES_BONS_LIVRAISONS_CLIENTS();
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
                    UneLigne.BON_LIVRAISON_CLIENT = BonLivraisonClient.ID;
                    UneLigne.BONS_LIVRAISONS_CLIENTS = BonLivraisonClient;
                    UneLigne.PRODUITS = Produit;
                    BD.LIGNES_BONS_LIVRAISONS_CLIENTS.Add(UneLigne);
                    BD.SaveChanges();
                }
                SelectedBonLivraison = BonLivraisonClient.ID.ToString();
            }
            if (Print)
            {
                return RedirectToAction("PrintBonLivraisonClientByID", new { CODE = SelectedBonLivraison });
            }
            Session["ProduitsBonLivraisonClient"] = null;
            ViewData["MODE"] = Mode;
            ViewBag.MODE = Mode;
            return RedirectToAction("BonLivraison", new { MODE = Mode });
        }
        [HttpPost]
        public ActionResult SendFacture(string Mode, string Code)
        {
            string Numero = Request["numero"] != null ? Request["numero"].ToString() : string.Empty;
            string date = Request["date"] != null ? Request["date"].ToString() : string.Empty;
            string client = Request["client"] != null ? Request["client"].ToString() : string.Empty;
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
            if (Session["ProduitsFactureClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsFactureClient"];
            }
            if (Mode == "Create")
            {
                if (!BD.FACTURES_CLIENTS.Select(cmd => cmd.CODE).Contains(Numero))
                {
                    FACTURES_CLIENTS FactureClient = new FACTURES_CLIENTS();
                    FactureClient.CODE = Numero;
                    FactureClient.DATE = DateTime.Parse(date);
                    FactureClient.CLIENT = int.Parse(client);
                    int ID_CLIENT = int.Parse(client);
                    FactureClient.CLIENTS = BD.CLIENTS.Where(fou => fou.ID == ID_CLIENT).FirstOrDefault();
                    FactureClient.MODE_PAIEMENT = modePaiement;
                    FactureClient.REMISE = decimal.Parse(remise, CultureInfo.InvariantCulture);
                    FactureClient.THT = decimal.Parse(totalHT, CultureInfo.InvariantCulture);
                    FactureClient.NHT = decimal.Parse(NetHT, CultureInfo.InvariantCulture);
                    FactureClient.TTVA = decimal.Parse(totalTVA, CultureInfo.InvariantCulture);
                    FactureClient.TIMBRE = decimal.Parse(timbre, CultureInfo.InvariantCulture);
                    FactureClient.TTC = decimal.Parse(TotalTTC, CultureInfo.InvariantCulture);
                    FactureClient.TNET = decimal.Parse(netAPaye, CultureInfo.InvariantCulture);

                    BD.FACTURES_CLIENTS.Add(FactureClient);
                    BD.SaveChanges();
                    foreach (LigneProduit Ligne in ListeDesPoduits)
                    {
                        LIGNES_FACTURES_CLIENTS UneLigne = new LIGNES_FACTURES_CLIENTS();
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
                        UneLigne.FACTURE_CLIENT = FactureClient.ID;
                        UneLigne.FACTURES_CLIENTS = FactureClient;
                        UneLigne.PRODUITS = Produit;
                        BD.LIGNES_FACTURES_CLIENTS.Add(UneLigne);
                        BD.SaveChanges();
                        AddMouvementProduit("FACTURE", Produit, FactureClient.DATE, FactureClient.CODE, Ligne.QUANTITE);
                    }
                    SelectedFacture = FactureClient.ID.ToString();
                }
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                FACTURES_CLIENTS FactureClient = BD.FACTURES_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
                FactureClient.CODE = Numero;
                FactureClient.DATE = DateTime.Parse(date);
                FactureClient.CLIENT = int.Parse(client);
                int ID_CLIENT = int.Parse(client);
                FactureClient.CLIENTS = BD.CLIENTS.Where(fou => fou.ID == ID_CLIENT).FirstOrDefault();
                FactureClient.MODE_PAIEMENT = modePaiement;
                FactureClient.REMISE = decimal.Parse(remise, CultureInfo.InvariantCulture);
                FactureClient.THT = decimal.Parse(totalHT, CultureInfo.InvariantCulture);
                FactureClient.NHT = decimal.Parse(NetHT, CultureInfo.InvariantCulture);
                FactureClient.TTVA = decimal.Parse(totalTVA, CultureInfo.InvariantCulture);
                FactureClient.TIMBRE = decimal.Parse(timbre, CultureInfo.InvariantCulture);
                FactureClient.TTC = decimal.Parse(TotalTTC, CultureInfo.InvariantCulture);
                FactureClient.TNET = decimal.Parse(netAPaye, CultureInfo.InvariantCulture);
                BD.SaveChanges();
                BD.LIGNES_FACTURES_CLIENTS.Where(p => p.FACTURE_CLIENT == FactureClient.ID).ToList().ForEach(p => BD.LIGNES_FACTURES_CLIENTS.Remove(p));
                BD.SaveChanges();
                foreach (LigneProduit Ligne in ListeDesPoduits)
                {
                    LIGNES_FACTURES_CLIENTS UneLigne = new LIGNES_FACTURES_CLIENTS();
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
                    UneLigne.FACTURE_CLIENT = FactureClient.ID;
                    UneLigne.FACTURES_CLIENTS = FactureClient;
                    UneLigne.PRODUITS = Produit;
                    BD.LIGNES_FACTURES_CLIENTS.Add(UneLigne);
                    BD.SaveChanges();
                }
                SelectedFacture = FactureClient.ID.ToString();
            }
            if (Print)
            {
                return RedirectToAction("PrintFactureClientByID", new { CODE = SelectedFacture });
            }
            Session["ProduitsFactureClient"] = null;
            ViewData["MODE"] = Mode;
            ViewBag.MODE = Mode;
            return RedirectToAction("Facture", new { MODE = Mode });
        }
        [HttpPost]
        public ActionResult SendAvoir(string Mode, string Code)
        {
            string Numero = Request["numero"] != null ? Request["numero"].ToString() : string.Empty;
            string date = Request["date"] != null ? Request["date"].ToString() : string.Empty;
            string client = Request["client"] != null ? Request["client"].ToString() : string.Empty;
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
            string SelectedAvoir = string.Empty;
            if (Session["ProduitsAvoirClient"] != null)
            {
                ListeDesPoduits = (List<LigneProduit>)Session["ProduitsAvoirClient"];
            }
            if (Mode == "Create")
            {
                if (!BD.AVOIRS_CLIENTS.Select(cmd => cmd.CODE).Contains(Numero))
                {
                    AVOIRS_CLIENTS AvoirClient = new AVOIRS_CLIENTS();
                    AvoirClient.CODE = Numero;
                    AvoirClient.DATE = DateTime.Parse(date);
                    AvoirClient.CLIENT = int.Parse(client);
                    int ID_CLIENT = int.Parse(client);
                    AvoirClient.CLIENTS = BD.CLIENTS.Where(fou => fou.ID == ID_CLIENT).FirstOrDefault();
                    AvoirClient.MODE_PAIEMENT = modePaiement;
                    AvoirClient.REMISE = decimal.Parse(remise, CultureInfo.InvariantCulture);
                    AvoirClient.THT = decimal.Parse(totalHT, CultureInfo.InvariantCulture);
                    AvoirClient.NHT = decimal.Parse(NetHT, CultureInfo.InvariantCulture);
                    AvoirClient.TTVA = decimal.Parse(totalTVA, CultureInfo.InvariantCulture);
                    AvoirClient.TTC = decimal.Parse(TotalTTC, CultureInfo.InvariantCulture);
                    AvoirClient.TNET = decimal.Parse(netAPaye, CultureInfo.InvariantCulture);

                    BD.AVOIRS_CLIENTS.Add(AvoirClient);
                    BD.SaveChanges();
                    foreach (LigneProduit Ligne in ListeDesPoduits)
                    {
                        LIGNES_AVOIRS_CLIENTS UneLigne = new LIGNES_AVOIRS_CLIENTS();
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
                        UneLigne.AVOIR_CLIENT = AvoirClient.ID;
                        UneLigne.AVOIRS_CLIENTS = AvoirClient;
                        UneLigne.PRODUITS = Produit;
                        BD.LIGNES_AVOIRS_CLIENTS.Add(UneLigne);
                        BD.SaveChanges();
                        AddMouvementProduit("AVOIR", Produit, AvoirClient.DATE, AvoirClient.CODE, Ligne.QUANTITE);
                    }
                    SelectedAvoir = AvoirClient.ID.ToString();
                }
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                AVOIRS_CLIENTS AvoirClient = BD.AVOIRS_CLIENTS.Where(cmd => cmd.ID == ID).FirstOrDefault();
                AvoirClient.CODE = Numero;
                AvoirClient.DATE = DateTime.Parse(date);
                AvoirClient.CLIENT = int.Parse(client);
                int ID_CLIENT = int.Parse(client);
                AvoirClient.CLIENTS = BD.CLIENTS.Where(fou => fou.ID == ID_CLIENT).FirstOrDefault();
                AvoirClient.MODE_PAIEMENT = modePaiement;
                AvoirClient.REMISE = decimal.Parse(remise, CultureInfo.InvariantCulture);
                AvoirClient.THT = decimal.Parse(totalHT, CultureInfo.InvariantCulture);
                AvoirClient.NHT = decimal.Parse(NetHT, CultureInfo.InvariantCulture);
                AvoirClient.TTVA = decimal.Parse(totalTVA, CultureInfo.InvariantCulture);
                AvoirClient.TTC = decimal.Parse(TotalTTC, CultureInfo.InvariantCulture);
                AvoirClient.TNET = decimal.Parse(netAPaye, CultureInfo.InvariantCulture);
                BD.SaveChanges();
                BD.LIGNES_AVOIRS_CLIENTS.Where(p => p.AVOIR_CLIENT == AvoirClient.ID).ToList().ForEach(p => BD.LIGNES_AVOIRS_CLIENTS.Remove(p));
                BD.SaveChanges();
                foreach (LigneProduit Ligne in ListeDesPoduits)
                {
                    LIGNES_AVOIRS_CLIENTS UneLigne = new LIGNES_AVOIRS_CLIENTS();
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
                    UneLigne.AVOIR_CLIENT = AvoirClient.ID;
                    UneLigne.AVOIRS_CLIENTS = AvoirClient;
                    UneLigne.PRODUITS = Produit;
                    BD.LIGNES_AVOIRS_CLIENTS.Add(UneLigne);
                    BD.SaveChanges();
                }
                SelectedAvoir = AvoirClient.ID.ToString();
            }
            if (Print)
            {
                return RedirectToAction("PrintAvoirClientByID", new { CODE = SelectedAvoir });
            }
            Session["ProduitsAvoirClient"] = null;
            ViewData["MODE"] = Mode;
            ViewBag.MODE = Mode;
            return RedirectToAction("Avoir", new { MODE = Mode });
        }
        
    }
}
