using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestionCommerciale.Models;
using System.Web.Script.Serialization;
using System.Globalization;

namespace GestionCommerciale.Controllers
{
    public class HomeController : Controller
    {
        GestionCommercialeEntity BD = new GestionCommercialeEntity();
        public ActionResult Home()
        {
            HttpCookie CurrentUserInfo = Request.Cookies["UtilisateurActuel"];
            if (CurrentUserInfo == null)
            {
                return RedirectToAction("Index", "Account");
            }
            else
            {
                List<NOTIFICATION> Liste = new List<NOTIFICATION>();
                List<DECLARATIONS> ListeSociete = BD.DECLARATIONS.ToList();
                foreach (DECLARATIONS Societe in ListeSociete)
                {
                    #region MOIS 1
                    if (DateTime.Today.Month == 1)
                    {
                        List<GENERATIONS> ListeCNSS = BD.GENERATIONS.Where(Element => Element.DATE.Month == DateTime.Today.Month && Element.DATE.Year == DateTime.Today.Year).ToList();
                        List<DECLARATIONS_FACTURES> ListeBCD = BD.DECLARATIONS_FACTURES.Where(Element => Element.DATE.Month == DateTime.Today.Month && Element.DATE.Year == DateTime.Today.Year).ToList();
                        List<DECLARATIONS_FACS> ListeFAC = BD.DECLARATIONS_FACS.Where(Element => Element.DATE.Month == DateTime.Today.Month && Element.DATE.Year == DateTime.Today.Year).ToList();
                        if (!ListeCNSS.Select(Element => Element.DECLARATIONS).Contains(Societe) && DateTime.Today.Day <= 28)
                        {
                            NOTIFICATION NouvelleNotification = new NOTIFICATION();
                            NouvelleNotification.SOCIETE = Societe.SOCIETE;
                            NouvelleNotification.TYPE = "CNSS";
                            NouvelleNotification.DESCRIPTION = "T4_" + (DateTime.Today.Year - 1);
                            DateTime startDate = DateTime.Today;
                            DateTime endDate = new DateTime(DateTime.Today.Year, 1, 28);
                            if ((endDate - startDate).TotalDays >= 7)
                            {
                                NouvelleNotification.ETIQUETTE = "A";
                            }
                            if ((endDate - startDate).TotalDays < 7 && (endDate - startDate).TotalDays > 2)
                            {
                                NouvelleNotification.ETIQUETTE = "B";
                            }
                            if ((endDate - startDate).TotalDays <= 2)
                            {
                                NouvelleNotification.ETIQUETTE = "C";
                            }
                            Liste.Add(NouvelleNotification);
                        }
                        if (!ListeBCD.Select(Element => Element.DECLARATIONS).Contains(Societe) && DateTime.Today.Day <= 28)
                        {
                            NOTIFICATION NouvelleNotification = new NOTIFICATION();
                            NouvelleNotification.SOCIETE = Societe.SOCIETE;
                            NouvelleNotification.TYPE = "BCD";
                            NouvelleNotification.DESCRIPTION = "T4_" + (DateTime.Today.Year - 1);
                            DateTime startDate = DateTime.Today;
                            DateTime endDate = new DateTime(DateTime.Today.Year, 1, 28);
                            if ((endDate - startDate).TotalDays >= 7)
                            {
                                NouvelleNotification.ETIQUETTE = "A";
                            }
                            if ((endDate - startDate).TotalDays < 7 && (endDate - startDate).TotalDays > 2)
                            {
                                NouvelleNotification.ETIQUETTE = "B";
                            }
                            if ((endDate - startDate).TotalDays <= 2)
                            {
                                NouvelleNotification.ETIQUETTE = "C";
                            }
                            Liste.Add(NouvelleNotification);
                        }
                        if (!ListeFAC.Select(Element => Element.DECLARATIONS).Contains(Societe) && DateTime.Today.Day <= 28)
                        {
                            NOTIFICATION NouvelleNotification = new NOTIFICATION();
                            NouvelleNotification.SOCIETE = Societe.SOCIETE;
                            NouvelleNotification.TYPE = "FAC";
                            NouvelleNotification.DESCRIPTION = "T4_" + (DateTime.Today.Year - 1);
                            DateTime startDate = DateTime.Today;
                            DateTime endDate = new DateTime(DateTime.Today.Year, 1, 28);
                            if ((endDate - startDate).TotalDays >= 7)
                            {
                                NouvelleNotification.ETIQUETTE = "A";
                            }
                            if ((endDate - startDate).TotalDays < 7 && (endDate - startDate).TotalDays > 2)
                            {
                                NouvelleNotification.ETIQUETTE = "B";
                            }
                            if ((endDate - startDate).TotalDays <= 2)
                            {
                                NouvelleNotification.ETIQUETTE = "C";
                            }
                            Liste.Add(NouvelleNotification);
                        }
                    }
                    #endregion
                    #region MOIS 4
                    if (DateTime.Today.Month == 4)
                    {
                        List<GENERATIONS> ListeCNSS = BD.GENERATIONS.Where(Element => Element.DATE.Month == DateTime.Today.Month && Element.DATE.Year == DateTime.Today.Year).ToList();
                        List<DECLARATIONS_FACTURES> ListeBCD = BD.DECLARATIONS_FACTURES.Where(Element => Element.DATE.Month == DateTime.Today.Month && Element.DATE.Year == DateTime.Today.Year).ToList();
                        List<DECLARATIONS_FACS> ListeFAC = BD.DECLARATIONS_FACS.Where(Element => Element.DATE.Month == DateTime.Today.Month && Element.DATE.Year == DateTime.Today.Year).ToList();
                        if (!ListeCNSS.Select(Element => Element.DECLARATIONS).Contains(Societe) && DateTime.Today.Day <= 28)
                        {
                            NOTIFICATION NouvelleNotification = new NOTIFICATION();
                            NouvelleNotification.SOCIETE = Societe.SOCIETE;
                            NouvelleNotification.TYPE = "CNSS";
                            NouvelleNotification.DESCRIPTION = "T1_" + (DateTime.Today.Year);
                            DateTime startDate = DateTime.Today;
                            DateTime endDate = new DateTime(DateTime.Today.Year, 4, 28);
                            if ((endDate - startDate).TotalDays >= 7)
                            {
                                NouvelleNotification.ETIQUETTE = "A";
                            }
                            if ((endDate - startDate).TotalDays < 7 && (endDate - startDate).TotalDays > 2)
                            {
                                NouvelleNotification.ETIQUETTE = "B";
                            }
                            if ((endDate - startDate).TotalDays <= 2)
                            {
                                NouvelleNotification.ETIQUETTE = "C";
                            }
                            Liste.Add(NouvelleNotification);
                        }
                        if (!ListeBCD.Select(Element => Element.DECLARATIONS).Contains(Societe) && DateTime.Today.Day <= 28)
                        {
                            NOTIFICATION NouvelleNotification = new NOTIFICATION();
                            NouvelleNotification.SOCIETE = Societe.SOCIETE;
                            NouvelleNotification.TYPE = "BCD";
                            NouvelleNotification.DESCRIPTION = "T1_" + (DateTime.Today.Year);
                            DateTime startDate = DateTime.Today;
                            DateTime endDate = new DateTime(DateTime.Today.Year, 4, 28);
                            if ((endDate - startDate).TotalDays >= 7)
                            {
                                NouvelleNotification.ETIQUETTE = "A";
                            }
                            if ((endDate - startDate).TotalDays < 7 && (endDate - startDate).TotalDays > 2)
                            {
                                NouvelleNotification.ETIQUETTE = "B";
                            }
                            if ((endDate - startDate).TotalDays <= 2)
                            {
                                NouvelleNotification.ETIQUETTE = "C";
                            }
                            Liste.Add(NouvelleNotification);
                        }
                        if (!ListeFAC.Select(Element => Element.DECLARATIONS).Contains(Societe) && DateTime.Today.Day <= 28)
                        {
                            NOTIFICATION NouvelleNotification = new NOTIFICATION();
                            NouvelleNotification.SOCIETE = Societe.SOCIETE;
                            NouvelleNotification.TYPE = "FAC";
                            NouvelleNotification.DESCRIPTION = "T1_" + (DateTime.Today.Year);
                            DateTime startDate = DateTime.Today;
                            DateTime endDate = new DateTime(DateTime.Today.Year, 4, 28);
                            if ((endDate - startDate).TotalDays >= 7)
                            {
                                NouvelleNotification.ETIQUETTE = "A";
                            }
                            if ((endDate - startDate).TotalDays < 7 && (endDate - startDate).TotalDays > 2)
                            {
                                NouvelleNotification.ETIQUETTE = "B";
                            }
                            if ((endDate - startDate).TotalDays <= 2)
                            {
                                NouvelleNotification.ETIQUETTE = "C";
                            }
                            Liste.Add(NouvelleNotification);
                        }
                    }
                    #endregion
                    #region MOIS 7
                    if (DateTime.Today.Month == 7)
                    {
                        List<GENERATIONS> ListeCNSS = BD.GENERATIONS.Where(Element => Element.DATE.Month == DateTime.Today.Month && Element.DATE.Year == DateTime.Today.Year).ToList();
                        List<DECLARATIONS_FACTURES> ListeBCD = BD.DECLARATIONS_FACTURES.Where(Element => Element.DATE.Month == DateTime.Today.Month && Element.DATE.Year == DateTime.Today.Year).ToList();
                        List<DECLARATIONS_FACS> ListeFAC = BD.DECLARATIONS_FACS.Where(Element => Element.DATE.Month == DateTime.Today.Month && Element.DATE.Year == DateTime.Today.Year).ToList();
                        if (!ListeCNSS.Select(Element => Element.DECLARATIONS).Contains(Societe) && DateTime.Today.Day <= 28)
                        {
                            NOTIFICATION NouvelleNotification = new NOTIFICATION();
                            NouvelleNotification.SOCIETE = Societe.SOCIETE;
                            NouvelleNotification.TYPE = "CNSS";
                            NouvelleNotification.DESCRIPTION = "T2_" + (DateTime.Today.Year);
                            DateTime startDate = DateTime.Today;
                            DateTime endDate = new DateTime(DateTime.Today.Year, 7, 28);
                            if ((endDate - startDate).TotalDays >= 7)
                            {
                                NouvelleNotification.ETIQUETTE = "A";
                            }
                            if ((endDate - startDate).TotalDays < 7 && (endDate - startDate).TotalDays > 2)
                            {
                                NouvelleNotification.ETIQUETTE = "B";
                            }
                            if ((endDate - startDate).TotalDays <= 2)
                            {
                                NouvelleNotification.ETIQUETTE = "C";
                            }
                            Liste.Add(NouvelleNotification);
                        }
                        if (!ListeBCD.Select(Element => Element.DECLARATIONS).Contains(Societe) && DateTime.Today.Day <= 28)
                        {
                            NOTIFICATION NouvelleNotification = new NOTIFICATION();
                            NouvelleNotification.SOCIETE = Societe.SOCIETE;
                            NouvelleNotification.TYPE = "BCD";
                            NouvelleNotification.DESCRIPTION = "T2_" + (DateTime.Today.Year);
                            DateTime startDate = DateTime.Today;
                            DateTime endDate = new DateTime(DateTime.Today.Year, 7, 28);
                            if ((endDate - startDate).TotalDays >= 7)
                            {
                                NouvelleNotification.ETIQUETTE = "A";
                            }
                            if ((endDate - startDate).TotalDays < 7 && (endDate - startDate).TotalDays > 2)
                            {
                                NouvelleNotification.ETIQUETTE = "B";
                            }
                            if ((endDate - startDate).TotalDays <= 2)
                            {
                                NouvelleNotification.ETIQUETTE = "C";
                            }
                            Liste.Add(NouvelleNotification);
                        }
                        if (!ListeFAC.Select(Element => Element.DECLARATIONS).Contains(Societe) && DateTime.Today.Day <= 28)
                        {
                            NOTIFICATION NouvelleNotification = new NOTIFICATION();
                            NouvelleNotification.SOCIETE = Societe.SOCIETE;
                            NouvelleNotification.TYPE = "FAC";
                            NouvelleNotification.DESCRIPTION = "T2_" + (DateTime.Today.Year);
                            DateTime startDate = DateTime.Today;
                            DateTime endDate = new DateTime(DateTime.Today.Year, 7, 28);
                            if ((endDate - startDate).TotalDays >= 7)
                            {
                                NouvelleNotification.ETIQUETTE = "A";
                            }
                            if ((endDate - startDate).TotalDays < 7 && (endDate - startDate).TotalDays > 2)
                            {
                                NouvelleNotification.ETIQUETTE = "B";
                            }
                            if ((endDate - startDate).TotalDays <= 2)
                            {
                                NouvelleNotification.ETIQUETTE = "C";
                            }
                            Liste.Add(NouvelleNotification);
                        }
                    }
                    #endregion
                    #region MOIS 10
                    if (DateTime.Today.Month == 1)
                    {
                        List<GENERATIONS> ListeCNSS = BD.GENERATIONS.Where(Element => Element.DATE.Month == DateTime.Today.Month && Element.DATE.Year == DateTime.Today.Year).ToList();
                        List<DECLARATIONS_FACTURES> ListeBCD = BD.DECLARATIONS_FACTURES.Where(Element => Element.DATE.Month == DateTime.Today.Month && Element.DATE.Year == DateTime.Today.Year).ToList();
                        List<DECLARATIONS_FACS> ListeFAC = BD.DECLARATIONS_FACS.Where(Element => Element.DATE.Month == DateTime.Today.Month && Element.DATE.Year == DateTime.Today.Year).ToList();
                        if (!ListeCNSS.Select(Element => Element.DECLARATIONS).Contains(Societe) && DateTime.Today.Day <= 28)
                        {
                            NOTIFICATION NouvelleNotification = new NOTIFICATION();
                            NouvelleNotification.SOCIETE = Societe.SOCIETE;
                            NouvelleNotification.TYPE = "CNSS";
                            NouvelleNotification.DESCRIPTION = "T3_" + (DateTime.Today.Year);
                            DateTime startDate = DateTime.Today;
                            DateTime endDate = new DateTime(DateTime.Today.Year, 10, 28);
                            if ((endDate - startDate).TotalDays >= 7)
                            {
                                NouvelleNotification.ETIQUETTE = "A";
                            }
                            if ((endDate - startDate).TotalDays < 7 && (endDate - startDate).TotalDays > 2)
                            {
                                NouvelleNotification.ETIQUETTE = "B";
                            }
                            if ((endDate - startDate).TotalDays <= 2)
                            {
                                NouvelleNotification.ETIQUETTE = "C";
                            }
                            Liste.Add(NouvelleNotification);
                        }
                        if (!ListeBCD.Select(Element => Element.DECLARATIONS).Contains(Societe) && DateTime.Today.Day <= 28)
                        {
                            NOTIFICATION NouvelleNotification = new NOTIFICATION();
                            NouvelleNotification.SOCIETE = Societe.SOCIETE;
                            NouvelleNotification.TYPE = "BCD";
                            NouvelleNotification.DESCRIPTION = "T3_" + (DateTime.Today.Year - 1);
                            DateTime startDate = DateTime.Today;
                            DateTime endDate = new DateTime(DateTime.Today.Year, 10, 28);
                            if ((endDate - startDate).TotalDays >= 7)
                            {
                                NouvelleNotification.ETIQUETTE = "A";
                            }
                            if ((endDate - startDate).TotalDays < 7 && (endDate - startDate).TotalDays > 2)
                            {
                                NouvelleNotification.ETIQUETTE = "B";
                            }
                            if ((endDate - startDate).TotalDays <= 2)
                            {
                                NouvelleNotification.ETIQUETTE = "C";
                            }
                            Liste.Add(NouvelleNotification);
                        }
                        if (!ListeFAC.Select(Element => Element.DECLARATIONS).Contains(Societe) && DateTime.Today.Day <= 28)
                        {
                            NOTIFICATION NouvelleNotification = new NOTIFICATION();
                            NouvelleNotification.SOCIETE = Societe.SOCIETE;
                            NouvelleNotification.TYPE = "FAC";
                            NouvelleNotification.DESCRIPTION = "T3_" + (DateTime.Today.Year - 1);
                            DateTime startDate = DateTime.Today;
                            DateTime endDate = new DateTime(DateTime.Today.Year, 10, 28);
                            if ((endDate - startDate).TotalDays >= 7)
                            {
                                NouvelleNotification.ETIQUETTE = "A";
                            }
                            if ((endDate - startDate).TotalDays < 7 && (endDate - startDate).TotalDays > 2)
                            {
                                NouvelleNotification.ETIQUETTE = "B";
                            }
                            if ((endDate - startDate).TotalDays <= 2)
                            {
                                NouvelleNotification.ETIQUETTE = "C";
                            }
                            Liste.Add(NouvelleNotification);
                        }
                    }
                    #endregion
                }
                ViewBag.Login = CurrentUserInfo["Login"] != null ? CurrentUserInfo["Login"].ToString() : string.Empty;
                return View(Liste);
            }
        }
        public ActionResult Index()
        {
            List<NOTIFICATION> ListeNotification = new List<NOTIFICATION>();
            List<PRODUITS> ListeProduit = BD.PRODUITS.ToList();
            foreach (PRODUITS produit in ListeProduit)
            {
                if (produit.BLOQUE)
                {
                    NOTIFICATION Notif = new NOTIFICATION();
                    Notif.CODE_PRODUIT = produit.CODE;
                    Notif.LIB_PRODUIT = produit.DESIGNATION;
                    Notif.ETIQUETTE = "BLOQUEE";
                    ListeNotification.Add(Notif);
                }
                if (!produit.BLOQUE && produit.QUANTITE <= produit.QUANTITE_REPTURE_STOCK)
                {
                    NOTIFICATION Notif = new NOTIFICATION();
                    Notif.CODE_PRODUIT = produit.CODE;
                    Notif.LIB_PRODUIT = produit.DESIGNATION;
                    Notif.ETIQUETTE = "REPTURE";
                }
            }
            int CDF = 0;
            int BRF = 0;
            int FF = 0;
            int AVF = 0;
            int DVC = 0;
            int CDC = 0;
            int BLC = 0;
            int FC = 0;
            int AVC = 0;
            CDF = BD.COMMANDES_FOURNISSEURS.Where(item => item.DATE.Month == DateTime.Today.Month).Count();
            BRF = BD.BONS_RECEPTIONS_FOURNISSEURS.Where(item => item.DATE.Month == DateTime.Today.Month).Count();
            FF = BD.FACTURES_FOURNISSEURS.Where(item => item.DATE.Month == DateTime.Today.Month).Count();
            AVF = BD.AVOIRS_FOURNISSEURS.Where(item => item.DATE.Month == DateTime.Today.Month).Count();
            DVC = BD.DEVIS_CLIENTS.Where(item => item.DATE.Month == DateTime.Today.Month).Count();
            CDC = BD.COMMANDES_CLIENTS.Where(item => item.DATE.Month == DateTime.Today.Month).Count();
            BLC = BD.BONS_LIVRAISONS_CLIENTS.Where(item => item.DATE.Month == DateTime.Today.Month).Count();
            FC = BD.FACTURES_CLIENTS.Where(item => item.DATE.Month == DateTime.Today.Month).Count();
            AVC = BD.AVOIRS_CLIENTS.Where(item => item.DATE.Month == DateTime.Today.Month).Count();
            decimal AchatMois = 0;
            decimal AchatAnnee = 0;
            decimal VenteMois = 0;
            decimal VenteAnnee = 0;
            decimal AchatPrecMois = 0;
            decimal AchatPrecAnnee = 0;
            decimal VentePrecMois = 0;
            decimal VentePrecAnnee = 0;
            decimal RapportAchatMois = 0;
            decimal RapportAchatAnnee = 0;
            decimal RapportVenteMois = 0;
            decimal RapportVenteAnnee = 0;

            AchatMois = BD.FACTURES_FOURNISSEURS.Where(item => item.DATE.Month == DateTime.Today.Month).Select(c => c.TTC).DefaultIfEmpty().Sum(c => c);
            AchatAnnee = BD.FACTURES_FOURNISSEURS.Where(item => item.DATE.Year == DateTime.Today.Year).Select(c => c.TTC).DefaultIfEmpty().Sum(c => c);
            VenteMois = BD.FACTURES_CLIENTS.Where(item => item.DATE.Month == DateTime.Today.Month).Select(c => c.TTC).DefaultIfEmpty().Sum(c => c);
            VenteAnnee = BD.FACTURES_CLIENTS.Where(item => item.DATE.Year == DateTime.Today.Year).Select(c => c.TTC).DefaultIfEmpty().Sum(c => c);

            int LastMonth = DateTime.Today.AddMonths(-1).Month;
            int LastYear = DateTime.Today.AddYears(-1).Year;
            AchatPrecMois = BD.FACTURES_FOURNISSEURS.Where(item => item.DATE.Month == LastMonth).Select(c => c.TTC).DefaultIfEmpty().Sum(c => c);
            AchatPrecAnnee = BD.FACTURES_FOURNISSEURS.Where(item => item.DATE.Year == LastYear).Select(c => c.TTC).DefaultIfEmpty().Sum(c => c);
            VentePrecMois = BD.FACTURES_CLIENTS.Where(item => item.DATE.Month == LastMonth).Select(c => c.TTC).DefaultIfEmpty().Sum(c => c);
            VentePrecAnnee = BD.FACTURES_CLIENTS.Where(item => item.DATE.Year == LastYear).Select(c => c.TTC).DefaultIfEmpty().Sum(c => c);

            if (AchatMois > AchatPrecMois)
            {
                if (AchatPrecMois != 0)
                    RapportAchatMois = (AchatMois * 100) / AchatPrecMois;
                else
                    RapportAchatMois = AchatMois;
            }
            else
            {
                if (AchatMois != 0)
                    RapportAchatMois = -(AchatPrecMois * 100) / AchatMois;
                else
                    RapportAchatMois = -AchatPrecMois;
            }

            if (AchatAnnee > AchatPrecAnnee)
            {
                if (AchatPrecAnnee != 0)
                    RapportAchatAnnee = (AchatAnnee * 100) / AchatPrecAnnee;
                else
                    RapportAchatAnnee = AchatAnnee;
            }
            else
            {
                if (AchatAnnee != 0)
                    RapportAchatAnnee = -(AchatPrecAnnee * 100) / AchatAnnee;
                else
                    RapportAchatAnnee = -AchatPrecAnnee;
            }

            if (VenteMois > VentePrecMois)
            {
                if (VentePrecMois != 0)
                    RapportVenteMois = (VenteMois * 100) / VentePrecMois;
                else
                    RapportVenteMois = VenteMois;
            }
            else
            {
                if (VenteMois != 0)
                    RapportVenteMois = -(VentePrecMois * 100) / VenteMois;
                else
                    RapportVenteMois = -VentePrecMois;
            }

            if (VenteAnnee > VentePrecAnnee)
            {
                if (VentePrecAnnee != 0)
                    RapportVenteAnnee = (VenteAnnee * 100) / VentePrecAnnee;
                else
                    RapportVenteAnnee = VenteAnnee;
            }
            else
            {
                if (VenteAnnee != 0)
                    RapportVenteAnnee = -(VentePrecAnnee * 100) / VenteAnnee;
                else
                    RapportVenteAnnee = -VentePrecAnnee;
            }
            //
            int NbrCommandeFournisseur = BD.COMMANDES_FOURNISSEURS.Count();
            int NbrCommandeClient = BD.COMMANDES_CLIENTS.Count();
            //
            int NbrBonReceptionValide = BD.BONS_RECEPTIONS_FOURNISSEURS.Where(item => item.VALIDER == true).Count();
            int NbrBonReceptionNonValide = BD.BONS_RECEPTIONS_FOURNISSEURS.Where(item => item.VALIDER == false).Count();
            //
            int NbrLivraisonValide = BD.BONS_LIVRAISONS_CLIENTS.Where(item => item.VALIDER == true).Count();
            int NbrLivraisonNonValide = BD.BONS_LIVRAISONS_CLIENTS.Where(item => item.VALIDER == false).Count();
            //
            int NbrFactureFournisseurPaye = BD.FACTURES_FOURNISSEURS.Where(item => item.PAYEE == true).Count();
            int NbrFactureFournisseurNonPaye = BD.FACTURES_FOURNISSEURS.Where(item => item.PAYEE == false).Count();
            //
            int NbrFactureClientPaye = BD.FACTURES_CLIENTS.Where(item => item.PAYEE == true).Count();
            int NbrFactureClientNonPaye = BD.FACTURES_CLIENTS.Where(item => item.PAYEE == false).Count();
            //
            int NbrAvoirFournisseur = BD.AVOIRS_FOURNISSEURS.Count();
            int NbrAvoirClient = BD.AVOIRS_CLIENTS.Count();
            //
            JavaScriptSerializer jss = new JavaScriptSerializer();
            List<CountPerMonth> ListCommande = new List<CountPerMonth>();
            for (int i = 1; i <= 12; i++)
            {
                int a = BD.COMMANDES_FOURNISSEURS.Where(item => item.DATE.Month == i && item.DATE.Year == DateTime.Today.Year).Count();
                int b = BD.COMMANDES_CLIENTS.Where(item => item.DATE.Month == i && item.DATE.Year == DateTime.Today.Year).Count();
                CountPerMonth count = new CountPerMonth();
                DateTime Date = new DateTime(2015, i, 1);
                CultureInfo ci = new CultureInfo("en-US");
                count.y = Date.ToString("MMMM", ci);
                count.a = a;
                count.b = b;
                ListCommande.Add(count);
            }
            List<CountPerMonth> ListeBonReceptionFournisseur = new List<CountPerMonth>();
            for (int i = 1; i <= 12; i++)
            {
                int a = BD.BONS_RECEPTIONS_FOURNISSEURS.Where(item => item.DATE.Month == i && item.DATE.Year == DateTime.Today.Year && item.VALIDER == true).Count();
                int b = BD.BONS_RECEPTIONS_FOURNISSEURS.Where(item => item.DATE.Month == i && item.DATE.Year == DateTime.Today.Year && item.VALIDER == false).Count();
                CountPerMonth count = new CountPerMonth();
                DateTime Date = new DateTime(2015, i, 1);
                CultureInfo ci = new CultureInfo("en-US");
                count.y = Date.ToString("MMMM", ci);
                count.a = a;
                count.b = b;
                ListeBonReceptionFournisseur.Add(count);
            }
            List<CountPerMonth> ListeBonLivraisonClient = new List<CountPerMonth>();
            for (int i = 1; i <= 12; i++)
            {
                int a = BD.BONS_LIVRAISONS_CLIENTS.Where(item => item.DATE.Month == i && item.DATE.Year == DateTime.Today.Year && item.VALIDER == true).Count();
                int b = BD.BONS_LIVRAISONS_CLIENTS.Where(item => item.DATE.Month == i && item.DATE.Year == DateTime.Today.Year && item.VALIDER == false).Count();
                CountPerMonth count = new CountPerMonth();
                DateTime Date = new DateTime(2015, i, 1);
                CultureInfo ci = new CultureInfo("en-US");
                count.y = Date.ToString("MMMM", ci);
                count.a = a;
                count.b = b;
                ListeBonLivraisonClient.Add(count);
            }
            List<CountPerMonth> ListeFactureFournisseur = new List<CountPerMonth>();
            for (int i = 1; i <= 12; i++)
            {
                int a = BD.FACTURES_FOURNISSEURS.Where(item => item.DATE.Month == i && item.DATE.Year == DateTime.Today.Year && item.PAYEE == true).Count();
                int b = BD.FACTURES_FOURNISSEURS.Where(item => item.DATE.Month == i && item.DATE.Year == DateTime.Today.Year && item.PAYEE == false).Count();
                CountPerMonth count = new CountPerMonth();
                DateTime Date = new DateTime(2015, i, 1);
                CultureInfo ci = new CultureInfo("en-US");
                count.y = Date.ToString("MMMM", ci);
                count.a = a;
                count.b = b;
                ListeFactureFournisseur.Add(count);
            }
            List<CountPerMonth> ListeFactureClient = new List<CountPerMonth>();
            for (int i = 1; i <= 12; i++)
            {
                int a = BD.FACTURES_CLIENTS.Where(item => item.DATE.Month == i && item.DATE.Year == DateTime.Today.Year && item.PAYEE == true).Count();
                int b = BD.FACTURES_CLIENTS.Where(item => item.DATE.Month == i && item.DATE.Year == DateTime.Today.Year && item.PAYEE == false).Count();
                CountPerMonth count = new CountPerMonth();
                DateTime Date = new DateTime(2015, i, 1);
                CultureInfo ci = new CultureInfo("en-US");
                count.y = Date.ToString("MMMM", ci);
                count.a = a;
                count.b = b;
                ListeFactureClient.Add(count);
            }
            List<CountPerMonth> ListeAvoir = new List<CountPerMonth>();
            for (int i = 1; i <= 12; i++)
            {
                int a = BD.AVOIRS_FOURNISSEURS.Where(item => item.DATE.Month == i && item.DATE.Year == DateTime.Today.Year).Count();
                int b = BD.AVOIRS_CLIENTS.Where(item => item.DATE.Month == i && item.DATE.Year == DateTime.Today.Year).Count();
                CountPerMonth count = new CountPerMonth();
                DateTime Date = new DateTime(2015, i, 1);
                CultureInfo ci = new CultureInfo("en-US");
                count.y = Date.ToString("MMMM", ci);
                count.a = a;
                count.b = b;
                ListeAvoir.Add(count);
            }
            List<CountPerMonth> ListeDevis = new List<CountPerMonth>();
            for (int i = 1; i <= 12; i++)
            {
                int a = BD.DEVIS_CLIENTS.Where(item => item.DATE.Month == i && item.DATE.Year == DateTime.Today.Year).Count();
                CountPerMonth count = new CountPerMonth();
                DateTime Date = new DateTime(2015, i, 1);
                CultureInfo ci = new CultureInfo("en-US");
                count.y = Date.ToString("MMMM", ci);
                count.a = a;
                ListeDevis.Add(count);
            }
            string Commande = jss.Serialize(ListCommande);
            string BonReception = jss.Serialize(ListeBonReceptionFournisseur);
            string BonLivraison = jss.Serialize(ListeBonLivraisonClient);
            string FactureFournisseur = jss.Serialize(ListeFactureFournisseur);
            string FactureClient = jss.Serialize(ListeFactureClient);
            string Avoir = jss.Serialize(ListeAvoir);
            string Devis = jss.Serialize(ListeDevis);
            //
            ViewBag.CDF = CDF;
            ViewBag.BRF = BRF;
            ViewBag.FF = FF;
            ViewBag.AVF = AVF;
            ViewBag.DVC = DVC;
            ViewBag.CDC = CDC;
            ViewBag.BLC = BLC;
            ViewBag.FC = FC;
            ViewBag.AVC = AVC;

            ViewBag.AchatMois = AchatMois;
            ViewBag.AchatAnnee = AchatAnnee;
            ViewBag.VenteMois = VenteMois;
            ViewBag.VenteAnnee = VenteAnnee;

            ViewBag.RapportAchatMois = RapportAchatMois;
            ViewBag.RapportAchatAnnee = RapportAchatAnnee;
            ViewBag.RapportVenteMois = RapportVenteMois;
            ViewBag.RapportVenteAnnee = RapportVenteAnnee;


            ViewBag.NbrCommandeFournisseur = NbrCommandeFournisseur;
            ViewBag.NbrCommandeClient = NbrCommandeClient;
            ViewBag.NbrBonReceptionValide = NbrBonReceptionValide;
            ViewBag.NbrBonReceptionNonValide = NbrBonReceptionNonValide;
            ViewBag.NbrLivraisonValide = NbrLivraisonValide;
            ViewBag.NbrLivraisonNonValide = NbrLivraisonNonValide;
            ViewBag.NbrFactureFournisseurPaye = NbrFactureFournisseurPaye;
            ViewBag.NbrFactureFournisseurNonPaye = NbrFactureFournisseurNonPaye;
            ViewBag.NbrFactureClientPaye = NbrFactureClientPaye;
            ViewBag.NbrFactureClientNonPaye = NbrFactureClientNonPaye;
            ViewBag.NbrAvoirFournisseur = NbrAvoirFournisseur;
            ViewBag.NbrAvoirClient = NbrAvoirClient;

            ViewBag.Commande = Commande;
            ViewBag.BonReception = BonReception;
            ViewBag.BonLivraison = BonLivraison;
            ViewBag.FactureFournisseur = FactureFournisseur;
            ViewBag.FactureClient = FactureClient;
            ViewBag.Avoir = Avoir;
            ViewBag.Devis = Devis;
            HttpCookie CurrentUserInfo = Request.Cookies["UtilisateurActuel"];
            if (CurrentUserInfo == null)
            {
                return RedirectToAction("Index", "Account");
            }
            else
            {
                ViewBag.Login = CurrentUserInfo["Login"] != null ? CurrentUserInfo["Login"].ToString() : string.Empty;
                return View(ListeNotification);
            }

        }
    }
    public class NOTIFICATION
    {
        public string CODE_PRODUIT;
        public string LIB_PRODUIT;
        public string ETIQUETTE;

        public string TYPE;
        public string SOCIETE;
        public string DESCRIPTION;
    }
    public class CountPerMonth
    {
        public string y;
        public int a;
        public int b;
    }
}
