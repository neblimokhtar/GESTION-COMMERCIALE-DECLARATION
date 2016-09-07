using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestionCommerciale.Models;
using System.Data.OleDb;
using System.IO;
using System.Data;
using System.Globalization;
using CrystalDecisions.CrystalReports.Engine;
using System.Data.Objects.DataClasses;

namespace GestionCommerciale.Controllers
{
    public class DeclarationBCDController : Controller
    {
        //
        // GET: /DeclarationBCD/
        GestionCommercialeEntity BD = new GestionCommercialeEntity();
        public ActionResult Index()
        {
            List<DECLARATIONS_FACTURES> Liste = BD.DECLARATIONS_FACTURES.Where(Element => Element.VALIDE == true).ToList();
            return View(Liste);
        }
        public ActionResult SecondEdit(int Code, int ParamPassed)
        {
            DECLARATIONS_FACTURES DeclarationBCD = BD.DECLARATIONS_FACTURES.Find(Code);
            LIGNES_DECLARATIONS_FACTURES LigneDeclarationDCB = new LIGNES_DECLARATIONS_FACTURES();
            LigneDeclarationDCB.DATE_BC = DateTime.Today;
            LigneDeclarationDCB.DATE_FACTURE = DateTime.Today;

            if (ParamPassed > 0)
                LigneDeclarationDCB = BD.LIGNES_DECLARATIONS_FACTURES.Find(ParamPassed);

            ViewBag.TRIMESTRE = DeclarationBCD.TRIMESTRE;
            ViewBag.ANNEE = DeclarationBCD.ANNEE;
            ViewBag.SOCIETE = DeclarationBCD.DECLARATIONS.SOCIETE;
            ViewBag.Code = Code;
            ViewBag.ParamPassed = ParamPassed;
            return View(LigneDeclarationDCB);
        }
        public ActionResult MyPartialViewEdit(int Code)
        {
            List<LIGNES_DECLARATIONS_FACTURES> Liste = BD.LIGNES_DECLARATIONS_FACTURES.Where(Element => Element.DECLARATIONS_FACTURES.ID == Code).ToList();
            ViewBag.Code = Code;
            return PartialView(Liste);
        }
        [HttpPost]
        public ActionResult SendSecondEditForm(int Code, int ParamPassed)
        {
            string NUMERO_BC = Request.Params["NUMERO_BC"] != null ? Request.Params["NUMERO_BC"].ToString() : string.Empty;
            string DATE_BC = Request.Params["DATE_BC"] != null ? Request.Params["DATE_BC"].ToString() : string.Empty;
            string MATRICULE = Request.Params["MATRICULE"] != null ? Request.Params["MATRICULE"].ToString() : string.Empty;
            string fournisseur = Request.Params["fournisseur"] != null ? Request.Params["fournisseur"].ToString() : string.Empty;
            string NUMERO_FACTURE = Request.Params["NUMERO_FACTURE"] != null ? Request.Params["NUMERO_FACTURE"].ToString() : string.Empty;
            string DATE_FACTURE = Request.Params["DATE_FACTURE"] != null ? Request.Params["DATE_FACTURE"].ToString() : string.Empty;
            string PRIX_HT = Request.Params["PRIX_HT"] != null ? Request.Params["PRIX_HT"].ToString() : string.Empty;
            string TVA = Request.Params["TVA"] != null ? Request.Params["TVA"].ToString() : string.Empty;
            string OBJET = Request.Params["OBJET"] != null ? Request.Params["OBJET"].ToString() : string.Empty;
            string NUMERO_AUTORISATION = Request.Params["NUMERO_AUTORISATION"] != null ? Request.Params["NUMERO_AUTORISATION"].ToString() : string.Empty;
            string ACTION = Request.Params["ACTION"] != null ? Request.Params["ACTION"].ToString() : string.Empty;
            #region DELETE
            if (ACTION == "DELETE")
            {
                LIGNES_DECLARATIONS_FACTURES Ligne = new LIGNES_DECLARATIONS_FACTURES();
                Ligne = BD.LIGNES_DECLARATIONS_FACTURES.Where(Element => Element.ID == ParamPassed).FirstOrDefault();
                BD.LIGNES_DECLARATIONS_FACTURES.Remove(Ligne);
                BD.SaveChanges();

                for (int i = 0; i < BD.LIGNES_DECLARATIONS_FACTURES.ToList().Count; i++)
                {
                    BD.LIGNES_DECLARATIONS_FACTURES.ToList().ElementAt(i).NUMERO_ORDRE = i + 1;
                    BD.SaveChanges();
                }

            }
            #endregion
            #region ADD
            if (ACTION == "ADD")
            {
                LIGNES_DECLARATIONS_FACTURES Ligne = new LIGNES_DECLARATIONS_FACTURES();
                if (ParamPassed <= 0)
                {
                    Ligne.NUMERO_ORDRE = BD.LIGNES_DECLARATIONS_FACTURES.Where(Element => Element.DECLARATIONS_FACTURES.ID == Code).ToList().Count + 1;
                    Ligne.NUMERO_BC = NUMERO_BC;
                    Ligne.DATE_BC = DateTime.Parse(DATE_BC);
                    int ID_FOURNISSEUR = int.Parse(fournisseur);
                    FOURNISSEURS SelectedFournisseur = BD.FOURNISSEURS.Find(ID_FOURNISSEUR);
                    Ligne.FOURNISSEUR = ID_FOURNISSEUR;
                    Ligne.FOURNISSEURS = SelectedFournisseur;
                    Ligne.DATE_FACTURE = DateTime.Parse(DATE_FACTURE);
                    Ligne.NUMERO_FACTURE = NUMERO_FACTURE;
                    if (PRIX_HT.Contains(','))
                        Ligne.PRIX_HT = decimal.Parse(PRIX_HT);
                    else
                        Ligne.PRIX_HT = decimal.Parse(PRIX_HT, CultureInfo.InvariantCulture);
                    Ligne.TVA = int.Parse(TVA);
                    Ligne.MONTANT_TVA = (Ligne.PRIX_HT * int.Parse(TVA)) / 100;
                    Ligne.NUMERO_AUTORISATION = NUMERO_AUTORISATION;
                    Ligne.OBJET = OBJET;
                    Ligne.DECLARATIONS_FACTURES = BD.DECLARATIONS_FACTURES.Find(Code);
                    Ligne.DECLARATION_FACTURE = Code;
                    BD.LIGNES_DECLARATIONS_FACTURES.Add(Ligne);
                    BD.SaveChanges();
                }
                if (ParamPassed >= 0)
                {
                    Ligne = BD.LIGNES_DECLARATIONS_FACTURES.Where(Element => Element.ID == ParamPassed).FirstOrDefault();
                    Ligne.NUMERO_BC = NUMERO_BC;
                    Ligne.DATE_BC = DateTime.Parse(DATE_BC);
                    int ID_FOURNISSEUR = int.Parse(fournisseur);
                    FOURNISSEURS SelectedFournisseur = BD.FOURNISSEURS.Find(ID_FOURNISSEUR);
                    Ligne.FOURNISSEUR = ID_FOURNISSEUR;
                    Ligne.FOURNISSEURS = SelectedFournisseur;
                    Ligne.DATE_FACTURE = DateTime.Parse(DATE_FACTURE);
                    Ligne.NUMERO_FACTURE = NUMERO_FACTURE;
                    if (PRIX_HT.Contains(','))
                        Ligne.PRIX_HT = decimal.Parse(PRIX_HT);
                    else
                        Ligne.PRIX_HT = decimal.Parse(PRIX_HT, CultureInfo.InvariantCulture);
                    Ligne.TVA = int.Parse(TVA);
                    Ligne.MONTANT_TVA = (Ligne.PRIX_HT * int.Parse(TVA)) / 100;
                    Ligne.NUMERO_AUTORISATION = NUMERO_AUTORISATION;
                    Ligne.OBJET = OBJET;
                    BD.SaveChanges();
                }
            }
            #endregion
            if (ACTION == "VALIDATE")
            {
                #region VALIDATE
                DECLARATIONS_FACTURES NouvelleDeclaration = BD.DECLARATIONS_FACTURES.Find(Code);

                FileInfo info = new FileInfo("Fichier vide.TXT");
                string nom = "BCD_T" + NouvelleDeclaration.TRIMESTRE + "_" + NouvelleDeclaration.ANNEE.ToString().Substring(2);
                var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fileStream.Close();
                info = new FileInfo(Server.MapPath(@"~/Images/" + nom));
                using (StreamWriter writer = info.CreateText())
                {
                    string EF01 = NouvelleDeclaration.DECLARATIONS.MATRICULE;
                    string EF05 = NouvelleDeclaration.ANNEE.ToString();
                    string EF06 = "T" + NouvelleDeclaration.TRIMESTRE.ToString();
                    string EF07 = NouvelleDeclaration.DECLARATIONS.SOCIETE != null ? NouvelleDeclaration.DECLARATIONS.SOCIETE : string.Empty; ;
                    string EF08 = NouvelleDeclaration.DECLARATIONS.ACTIVITE != null ? NouvelleDeclaration.DECLARATIONS.ACTIVITE : string.Empty;
                    string EF09 = NouvelleDeclaration.DECLARATIONS.VILLE != null ? NouvelleDeclaration.DECLARATIONS.VILLE : string.Empty;
                    string EF10 = NouvelleDeclaration.DECLARATIONS.RUE != null ? NouvelleDeclaration.DECLARATIONS.RUE : string.Empty;
                    string EF11 = NouvelleDeclaration.DECLARATIONS.NUMERO != null ? NouvelleDeclaration.DECLARATIONS.NUMERO : string.Empty;
                    string EF12 = NouvelleDeclaration.DECLARATIONS.CODE_POSTAL != null ? NouvelleDeclaration.DECLARATIONS.CODE_POSTAL : string.Empty;
                    while (EF07.Length < 40)
                    {
                        EF07 = EF07 + " ";
                    }
                    while (EF08.Length < 40)
                    {
                        EF08 = EF08 + " ";
                    }
                    while (EF09.Length < 40)
                    {
                        EF09 = EF09 + " ";
                    }
                    while (EF10.Length < 72)
                    {
                        EF10 = EF10 + " ";
                    }
                    while (EF11.Length < 4)
                    {
                        EF11 = EF11 + " ";
                    }
                    while (EF12.Length < 4)
                    {
                        EF12 = EF12 + " ";
                    }
                    string FirstLine = "EF" + EF01 + EF05 + EF06 + EF07 + EF08 + EF09 + EF10 + EF11 + EF12;
                    writer.WriteLine(FirstLine);
                    List<LIGNES_DECLARATIONS_FACTURES> NouvelleListe = BD.LIGNES_DECLARATIONS_FACTURES.Where(Element => Element.DECLARATIONS_FACTURES.ID == NouvelleDeclaration.ID).ToList();

                    decimal SumTotal = 0;
                    decimal SumTva = 0;
                    int NumeroOrdre = 1;
                    foreach (LIGNES_DECLARATIONS_FACTURES Ligne in NouvelleListe)
                    {
                        FOURNISSEURS SelectedFournisseur = BD.FOURNISSEURS.Find(Ligne.FOURNISSEUR);
                        string DF07 = NumeroOrdre.ToString("000000");//6
                        string DF08 = Ligne.NUMERO_AUTORISATION;//30
                        string DF09 = Ligne.NUMERO_BC;//13
                        string DF10 = Ligne.DATE_BC.ToShortDateString().Replace("/", "");//8
                        string DF11 = SelectedFournisseur.ID_FISCAL != null ? SelectedFournisseur.ID_FISCAL : string.Empty;//13
                        string DF12 = SelectedFournisseur.NOM != null ? SelectedFournisseur.NOM : string.Empty;//40
                        string DF13 = Ligne.NUMERO_FACTURE;//30
                        string DF14 = Ligne.DATE_FACTURE.ToShortDateString().Replace("/", "");
                        SumTotal += Ligne.PRIX_HT;
                        string DF15 = Ligne.PRIX_HT.ToString("F3").Replace(",", "");
                        SumTva += Ligne.MONTANT_TVA;
                        string DF16 = Ligne.MONTANT_TVA.ToString("F3").Replace(",", "");
                        string DF18 = Ligne.OBJET;
                        while (DF08.Length < 30)
                        {
                            DF08 = DF08 + " ";
                        }
                        while (DF09.Length < 13)
                        {
                            DF09 = "0" + DF09;
                        }
                        while (DF11.Length < 13)
                        {
                            DF11 = DF11 + " ";
                        }
                        while (DF12.Length < 40)
                        {
                            DF12 = DF12 + " ";
                        }
                        while (DF13.Length < 30)
                        {
                            DF13 = DF13 + " ";
                        }
                        while (DF15.Length < 15)
                        {
                            DF15 = "0" + DF15;
                        }
                        while (DF16.Length < 15)
                        {
                            DF16 = "0" + DF16;
                        }
                        while (DF18.Length < 320)
                        {
                            DF18 = DF18 + " ";
                        }
                        string SubLine = "DF" + EF01 + EF05 + EF06 + DF07 + DF08 + DF09 + DF10 + DF11 + DF12 + DF13 + DF14 + DF15 + DF16 + "<" + DF18 + "/>";
                        writer.WriteLine(SubLine);
                        NumeroOrdre++;
                    }
                    string TF007 = NouvelleListe.Count.ToString("000000");
                    string TF08 = string.Empty;
                    while (TF08.Length < 142)
                    {
                        TF08 = TF08 + " ";
                    }
                    string TF09 = SumTotal.ToString("F3").Replace(",", "");
                    string TF10 = SumTva.ToString("F3").Replace(",", "");
                    while (TF09.Length < 15)
                    {
                        TF09 = "0" + TF09;
                    }
                    while (TF10.Length < 15)
                    {
                        TF10 = "0" + TF10;
                    }
                    string LastLine = "TF" + EF01 + EF05 + EF06 + TF007 + TF08 + TF09 + TF10;
                    writer.WriteLine(LastLine);
                }
                byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Images/" + nom));
                NouvelleDeclaration.DATA = fileBytes;
                BD.SaveChanges();
                #endregion
                return RedirectToAction("Index");
            }
            return RedirectToAction("SecondEdit", "DeclarationBCD", new { ParamPassed = -1, @Code = Code });
        }
        public ActionResult Form(string Mode, int Code)
        {
            List<DECLARATIONS> Liste = BD.DECLARATIONS.ToList();
            ViewBag.Mode = Mode;
            ViewBag.Code = Code;
            if (Mode == "Detail")
            {
                DECLARATIONS_FACTURES SelectedDeclaration = BD.DECLARATIONS_FACTURES.Find(Code);
                string TRIMESTRE = SelectedDeclaration.TRIMESTRE != null ? SelectedDeclaration.TRIMESTRE.ToString() : string.Empty;
                string ANNEE = SelectedDeclaration.ANNEE != null ? SelectedDeclaration.ANNEE.ToString() : string.Empty;
                string SOCIETE = SelectedDeclaration.SOCIETE != null ? SelectedDeclaration.SOCIETE.ToString() : string.Empty;
                Session["TRIMESTRE"] = TRIMESTRE;
                Session["ANNEE"] = ANNEE;
                Session["SOCIETE"] = SOCIETE;
                return RedirectToAction("Second", "DeclarationBCD", new { @Mode = Mode, @Code = Code, @ANNEE = ANNEE, @SOCIETE = SOCIETE, @TRIMESTRE = TRIMESTRE });
            }
            ViewBag.ErrorText = TempData["ErrorText"] != null ? TempData["ErrorText"].ToString() : string.Empty;
            TempData["ErrorText"] = null;
            return View(Liste);
        }
        [HttpPost]
        public ActionResult SendForm(string Mode, string Code)
        {
            string TRIMESTRE = Request.Params["TRIMESTRE"] != null ? Request.Params["TRIMESTRE"].ToString() : string.Empty;
            string ANNEE = Request.Params["ANNEE"] != null ? Request.Params["ANNEE"].ToString() : string.Empty;
            string SOCIETE = Request.Params["SOCIETE"] != null ? Request.Params["SOCIETE"].ToString() : string.Empty;
            Session["TRIMESTRE"] = TRIMESTRE;
            Session["ANNEE"] = ANNEE;
            Session["SOCIETE"] = SOCIETE;
            int SelectedSociete = int.Parse(SOCIETE);
            DECLARATIONS Soc = BD.DECLARATIONS.Find(SelectedSociete);
            if (string.IsNullOrEmpty(Soc.MATRICULE))
            {
                ViewBag.ErrorText = "Matricule fiscale inexistante";
                TempData["ErrorText"] = "Matricule fiscale inexistante";
                return RedirectToAction("Form", "DeclarationBCD", new { @Mode = Mode, @Code = Code });
            }
            else
            {
                return RedirectToAction("Second", "DeclarationBCD", new { @Mode = Mode, @Code = Code, @ANNEE = ANNEE, @SOCIETE = SOCIETE, @TRIMESTRE = TRIMESTRE });
            }
        }
        public ActionResult Second(string Mode, string Code, string ANNEE, string SOCIETE, string TRIMESTRE)
        {
            int ID = int.Parse(SOCIETE);
            string SelectedSociete = BD.DECLARATIONS.Find(ID).SOCIETE;
            FOURNISSEURS Fournisseur = new FOURNISSEURS();
            ViewBag.TRIMESTRE = TRIMESTRE;
            ViewBag.ANNEE = ANNEE;
            ViewBag.SOCIETE = SelectedSociete;
            ViewBag.Mode = Mode;
            ViewBag.Code = Code;
            LIGNES_DECLARATIONS_FACTURES Ligne = new LIGNES_DECLARATIONS_FACTURES();
            Ligne.DATE_BC = DateTime.Today;
            Ligne.DATE_FACTURE = DateTime.Today;

            if (Mode == "Edit")
            {
                List<LIGNES_DECLARATIONS_FACTURES> Liste = new List<LIGNES_DECLARATIONS_FACTURES>();
                if (Session["LignesFacture"] != null)
                {
                    Liste = (List<LIGNES_DECLARATIONS_FACTURES>)Session["LignesFacture"];
                    if (Liste.Count > 0)
                    {
                        int SelectedRow = int.Parse(Code);
                        Ligne = Liste.Where(Element => Element.NUMERO_ORDRE == SelectedRow).FirstOrDefault();
                    }
                }
            }
            return View(Ligne);
        }
        public PartialViewResult MyPartialView(string Mode,int Code)
        {
            List<LIGNES_DECLARATIONS_FACTURES> Liste = new List<LIGNES_DECLARATIONS_FACTURES>();
            if (Session["LignesFacture"] != null)
            {
                Liste = (List<LIGNES_DECLARATIONS_FACTURES>)Session["LignesFacture"];
                Liste = Liste.OrderBy(Element => Element.NUMERO_ORDRE).ToList();
            }
            if (Mode == "Detail")
            {
                Liste = BD.LIGNES_DECLARATIONS_FACTURES.Where(Element => Element.DECLARATIONS_FACTURES.ID == Code).ToList();
            }
            string ANNEE = Session["ANNEE"] != null ? Session["ANNEE"].ToString() : string.Empty;
            string SOCIETE = Session["SOCIETE"] != null ? Session["SOCIETE"].ToString() : string.Empty;
            string TRIMESTRE = Session["TRIMESTRE"] != null ? Session["TRIMESTRE"].ToString() : string.Empty;
            ViewBag.SOCIETE = SOCIETE;
            ViewBag.TRIMESTRE = TRIMESTRE;
            ViewBag.ANNEE = ANNEE;
            ViewBag.Mode = Mode;
            return PartialView(Liste);
        }
        [HttpPost]
        public ActionResult SendSecondForm(string Mode, string Code)
        {
            string ANNEE = Session["ANNEE"] != null ? Session["ANNEE"].ToString() : string.Empty;
            string SOCIETE = Session["SOCIETE"] != null ? Session["SOCIETE"].ToString() : string.Empty;
            string TRIMESTRE = Session["TRIMESTRE"] != null ? Session["TRIMESTRE"].ToString() : string.Empty;
            List<LIGNES_DECLARATIONS_FACTURES> Liste = new List<LIGNES_DECLARATIONS_FACTURES>();
            if (Session["LignesFacture"] != null)
            {
                Liste = (List<LIGNES_DECLARATIONS_FACTURES>)Session["LignesFacture"];
            }
            string NUMERO_BC = Request.Params["NUMERO_BC"] != null ? Request.Params["NUMERO_BC"].ToString() : string.Empty;
            string DATE_BC = Request.Params["DATE_BC"] != null ? Request.Params["DATE_BC"].ToString() : string.Empty;
            string MATRICULE = Request.Params["MATRICULE"] != null ? Request.Params["MATRICULE"].ToString() : string.Empty;
            string fournisseur = Request.Params["fournisseur"] != null ? Request.Params["fournisseur"].ToString() : string.Empty;
            string NUMERO_FACTURE = Request.Params["NUMERO_FACTURE"] != null ? Request.Params["NUMERO_FACTURE"].ToString() : string.Empty;
            string DATE_FACTURE = Request.Params["DATE_FACTURE"] != null ? Request.Params["DATE_FACTURE"].ToString() : string.Empty;
            string PRIX_HT = Request.Params["PRIX_HT"] != null ? Request.Params["PRIX_HT"].ToString() : string.Empty;
            string TVA = Request.Params["TVA"] != null ? Request.Params["TVA"].ToString() : string.Empty;
            string OBJET = Request.Params["OBJET"] != null ? Request.Params["OBJET"].ToString() : string.Empty;
            string NUMERO_AUTORISATION = Request.Params["NUMERO_AUTORISATION"] != null ? Request.Params["NUMERO_AUTORISATION"].ToString() : string.Empty;
            string ACTION = Request.Params["ACTION"] != null ? Request.Params["ACTION"].ToString() : string.Empty;
            if (ACTION == "BACK")
            {
                return RedirectToAction("Form", "DeclarationBCD", new { @Mode = "Edit", @Code = -1 });
            }
            if (ACTION == "PRINT")
            {

            }
            if (ACTION == "VALIDATE")
            {
                DECLARATIONS_FACTURES NouvelleDeclaration = new DECLARATIONS_FACTURES();
                GestionCommercialeEntity BD_A = new GestionCommercialeEntity();
                GestionCommercialeEntity BD_B = new GestionCommercialeEntity();
                int IdSociete = int.Parse(SOCIETE);
                DECLARATIONS SelectedSociete = BD.DECLARATIONS.Find(IdSociete);
                NouvelleDeclaration.ANNEE = int.Parse(ANNEE);
                NouvelleDeclaration.CODE = "BCD_T" + TRIMESTRE + "_" + ANNEE.Substring(2);
                NouvelleDeclaration.DATE = DateTime.Today;
                NouvelleDeclaration.TRIMESTRE = int.Parse(TRIMESTRE);
                NouvelleDeclaration.DECLARATIONS = SelectedSociete;
                NouvelleDeclaration.SOCIETE = IdSociete;
                NouvelleDeclaration.VALIDE = true;
                BD.DECLARATIONS_FACTURES.Add(NouvelleDeclaration);
                BD.SaveChanges();
                foreach (LIGNES_DECLARATIONS_FACTURES Ligne in Liste)
                {
                    LIGNES_DECLARATIONS_FACTURES NouvelleLigne = new LIGNES_DECLARATIONS_FACTURES();
                    NouvelleLigne.DATE_BC = Ligne.DATE_BC;
                    NouvelleLigne.DATE_FACTURE = Ligne.DATE_FACTURE;
                    NouvelleLigne.DECLARATION_FACTURE = NouvelleDeclaration.ID;
                    NouvelleLigne.DECLARATIONS_FACTURES = NouvelleDeclaration;

                    NouvelleLigne.MONTANT_TVA = Ligne.MONTANT_TVA;
                    NouvelleLigne.NUMERO_AUTORISATION = Ligne.NUMERO_AUTORISATION;
                    NouvelleLigne.NUMERO_BC = Ligne.NUMERO_BC;
                    NouvelleLigne.NUMERO_FACTURE = Ligne.NUMERO_FACTURE;
                    NouvelleLigne.NUMERO_ORDRE = Ligne.NUMERO_ORDRE;
                    NouvelleLigne.OBJET = Ligne.OBJET;
                    NouvelleLigne.PRIX_HT = Ligne.PRIX_HT;
                    NouvelleLigne.TVA = Ligne.TVA;
                    FOURNISSEURS Four = Ligne.FOURNISSEURS;
                    NouvelleLigne.FOURNISSEUR = Four.ID;
                    //NouvelleLigne.FOURNISSEURS = Four;
                    BD.LIGNES_DECLARATIONS_FACTURES.Add(NouvelleLigne);
                    BD.SaveChanges();
                }
                #region SaveFile
                FileInfo info = new FileInfo("Fichier vide.TXT");
                string nom = "BCD_T" + TRIMESTRE + "_" + ANNEE.Substring(2);
                var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fileStream.Close();
                info = new FileInfo(Server.MapPath(@"~/Images/" + nom));
                using (StreamWriter writer = info.CreateText())
                {
                    string EF01 = SelectedSociete.MATRICULE;
                    string EF05 = ANNEE;
                    string EF06 = "T" + TRIMESTRE;
                    string EF07 = SelectedSociete.SOCIETE != null ? SelectedSociete.SOCIETE : string.Empty; ;
                    string EF08 = SelectedSociete.ACTIVITE != null ? SelectedSociete.ACTIVITE : string.Empty;
                    string EF09 = SelectedSociete.VILLE != null ? SelectedSociete.VILLE : string.Empty;
                    string EF10 = SelectedSociete.RUE != null ? SelectedSociete.RUE : string.Empty;
                    string EF11 = SelectedSociete.NUMERO != null ? SelectedSociete.NUMERO : string.Empty;
                    string EF12 = SelectedSociete.CODE_POSTAL != null ? SelectedSociete.CODE_POSTAL : string.Empty;
                    while (EF07.Length < 40)
                    {
                        EF07 = EF07 + " ";
                    }
                    while (EF08.Length < 40)
                    {
                        EF08 = EF08 + " ";
                    }
                    while (EF09.Length < 40)
                    {
                        EF09 = EF09 + " ";
                    }
                    while (EF10.Length < 72)
                    {
                        EF10 = EF10 + " ";
                    }
                    while (EF11.Length < 4)
                    {
                        EF11 = EF11 + " ";
                    }
                    while (EF12.Length < 4)
                    {
                        EF12 = EF12 + " ";
                    }
                    string FirstLine = "EF" + EF01 + EF05 + EF06 + EF07 + EF08 + EF09 + EF10 + EF11 + EF12;
                    writer.WriteLine(FirstLine);
                    List<LIGNES_DECLARATIONS_FACTURES> NouvelleListe = BD.LIGNES_DECLARATIONS_FACTURES.Where(Element => Element.DECLARATIONS_FACTURES.ID == NouvelleDeclaration.ID).ToList();

                    decimal SumTotal = 0;
                    decimal SumTva = 0;
                    int NumeroOrdre = 1;
                    foreach (LIGNES_DECLARATIONS_FACTURES Ligne in NouvelleListe)
                    {
                        FOURNISSEURS SelectedFournisseur = BD.FOURNISSEURS.Find(Ligne.FOURNISSEUR);
                        string DF07 = NumeroOrdre.ToString("000000");//6
                        string DF08 = Ligne.NUMERO_AUTORISATION;//30
                        string DF09 = Ligne.NUMERO_BC;//13
                        string DF10 = Ligne.DATE_BC.ToShortDateString().Replace("/", "");//8
                        string DF11 = SelectedFournisseur.ID_FISCAL != null ? SelectedFournisseur.ID_FISCAL : string.Empty;//13
                        string DF12 = SelectedFournisseur.NOM != null ? SelectedFournisseur.NOM : string.Empty;//40
                        string DF13 = Ligne.NUMERO_FACTURE;//30
                        string DF14 = Ligne.DATE_FACTURE.ToShortDateString().Replace("/", "");
                        SumTotal += Ligne.PRIX_HT;
                        string DF15 = Ligne.PRIX_HT.ToString("F3").Replace(",", "");
                        SumTva += Ligne.MONTANT_TVA;
                        string DF16 = Ligne.MONTANT_TVA.ToString("F3").Replace(",", "");
                        string DF18 = Ligne.OBJET;
                        while (DF08.Length < 30)
                        {
                            DF08 = DF08 + " ";
                        }
                        while (DF09.Length < 13)
                        {
                            DF09 = "0" + DF09;
                        }
                        while (DF11.Length < 13)
                        {
                            DF11 = DF11 + " ";
                        }
                        while (DF12.Length < 40)
                        {
                            DF12 = DF12 + " ";
                        }
                        while (DF13.Length < 30)
                        {
                            DF13 = DF13 + " ";
                        }
                        while (DF15.Length < 15)
                        {
                            DF15 = "0" + DF15;
                        }
                        while (DF16.Length < 15)
                        {
                            DF16 = "0" + DF16;
                        }
                        while (DF18.Length < 320)
                        {
                            DF18 = DF18 + " ";
                        }
                        string SubLine = "DF" + EF01 + EF05 + EF06 + DF07 + DF08 + DF09 + DF10 + DF11 + DF12 + DF13 + DF14 + DF15 + DF16 + "<" + DF18 + "/>";
                        writer.WriteLine(SubLine);
                        NumeroOrdre++;
                    }
                    string TF007 = NouvelleListe.Count.ToString("000000");
                    string TF08 = string.Empty;
                    while (TF08.Length < 142)
                    {
                        TF08 = TF08 + " ";
                    }
                    string TF09 = SumTotal.ToString("F3").Replace(",", "");
                    string TF10 = SumTva.ToString("F3").Replace(",", "");
                    while (TF09.Length < 15)
                    {
                        TF09 = "0" + TF09;
                    }
                    while (TF10.Length < 15)
                    {
                        TF10 = "0" + TF10;
                    }
                    string LastLine = "TF" + EF01 + EF05 + EF06 + TF007 + TF08 + TF09 + TF10;
                    writer.WriteLine(LastLine);
                }
                byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Images/" + nom));
                NouvelleDeclaration.DATA = fileBytes;
                BD.SaveChanges();
                #endregion
                Session.Clear();
                return RedirectToAction("Index");
            }
            if (ACTION == "ADD")
            {
                LIGNES_DECLARATIONS_FACTURES Ligne = new LIGNES_DECLARATIONS_FACTURES();
                if (Mode == "Create")
                {
                    Ligne.NUMERO_ORDRE = Liste.Count + 1;
                    Ligne.NUMERO_BC = NUMERO_BC;
                    Ligne.DATE_BC = DateTime.Parse(DATE_BC);
                    int ID_FOURNISSEUR = int.Parse(fournisseur);
                    FOURNISSEURS SelectedFournisseur = BD.FOURNISSEURS.Find(ID_FOURNISSEUR);
                    Ligne.FOURNISSEUR = ID_FOURNISSEUR;
                    Ligne.FOURNISSEURS = SelectedFournisseur;
                    Ligne.DATE_FACTURE = DateTime.Parse(DATE_FACTURE);
                    Ligne.NUMERO_FACTURE = NUMERO_FACTURE;
                    if (PRIX_HT.Contains(','))
                        Ligne.PRIX_HT = decimal.Parse(PRIX_HT);
                    else
                        Ligne.PRIX_HT = decimal.Parse(PRIX_HT, CultureInfo.InvariantCulture);
                    Ligne.TVA = int.Parse(TVA);
                    Ligne.MONTANT_TVA = (Ligne.PRIX_HT * int.Parse(TVA)) / 100;
                    Ligne.NUMERO_AUTORISATION = NUMERO_AUTORISATION;
                    Ligne.OBJET = OBJET;
                    Liste.Add(Ligne);
                }
                if (Mode == "Edit")
                {
                    int SelectedRow = int.Parse(Code);
                    Ligne = Liste.Where(Element => Element.NUMERO_ORDRE == SelectedRow).FirstOrDefault();
                    //Ligne.NUMERO_ORDRE = Liste.Count + 1;
                    Ligne.NUMERO_BC = NUMERO_BC;
                    Ligne.DATE_BC = DateTime.Parse(DATE_BC);
                    int ID_FOURNISSEUR = int.Parse(fournisseur);
                    FOURNISSEURS SelectedFournisseur = BD.FOURNISSEURS.Find(ID_FOURNISSEUR);
                    Ligne.FOURNISSEUR = ID_FOURNISSEUR;
                    Ligne.FOURNISSEURS = SelectedFournisseur;
                    Ligne.DATE_FACTURE = DateTime.Parse(DATE_FACTURE);
                    Ligne.NUMERO_FACTURE = NUMERO_FACTURE;
                    if (PRIX_HT.Contains(','))
                        Ligne.PRIX_HT = decimal.Parse(PRIX_HT);
                    else
                        Ligne.PRIX_HT = decimal.Parse(PRIX_HT, CultureInfo.InvariantCulture);
                    Ligne.TVA = int.Parse(TVA);
                    Ligne.MONTANT_TVA = (Ligne.PRIX_HT * int.Parse(TVA)) / 100;
                    Ligne.NUMERO_AUTORISATION = NUMERO_AUTORISATION;
                    Ligne.OBJET = OBJET;
                }
            }
            if (ACTION == "DELETE")
            {
                LIGNES_DECLARATIONS_FACTURES Ligne = new LIGNES_DECLARATIONS_FACTURES();
                int SelectedRow = int.Parse(Code);
                Ligne = Liste.Where(Element => Element.NUMERO_ORDRE == SelectedRow).FirstOrDefault();
                Liste.Remove(Ligne);
                for (int i = 0; i < Liste.Count; i++)
                {
                    Liste.ElementAt(i).NUMERO_ORDRE = i + 1;
                }
            }
            Session["LignesFacture"] = Liste;
            return RedirectToAction("Second", "DeclarationBCD", new { @Mode = "Create", @Code = "-1", @ANNEE = ANNEE, @SOCIETE = SOCIETE, @TRIMESTRE = TRIMESTRE });
        }
        public ActionResult GetFileByID(int id)
        {
            var fileToRetrieve = BD.DECLARATIONS_FACTURES.Find(id);
            return File(fileToRetrieve.DATA, System.Net.Mime.MediaTypeNames.Application.Octet, fileToRetrieve.CODE + ".TXT");
        }
        public ActionResult Delete(int Code)
        {
            DECLARATIONS_FACTURES Selected = BD.DECLARATIONS_FACTURES.Find(Code);
            BD.LIGNES_DECLARATIONS_FACTURES.Where(p => p.DECLARATIONS_FACTURES.ID == Code).ToList().ForEach(p => BD.LIGNES_DECLARATIONS_FACTURES.Remove(p));
            BD.SaveChanges();
            BD.DECLARATIONS_FACTURES.Remove(Selected);
            BD.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult PrintAllDeclaration()
        {
            List<DECLARATIONS_FACTURES> Liste = BD.DECLARATIONS_FACTURES.ToList();
            dynamic dt = from Element in Liste
                         select new
                         {
                             CODE = Element.CODE,
                             SOCIETE = Element.DECLARATIONS.SOCIETE,
                             ANNEE = Element.ANNEE,
                             TRIMESTRE = Element.TRIMESTRE,
                             DATE = Element.DATE.ToShortDateString()
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/PRINT_DECLARATION_BCD.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Liste des déclarations BCD";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintDeclaration(int Code)
        {
            DECLARATIONS_FACTURES SelectedDeclaration=BD.DECLARATIONS_FACTURES.Find(Code);
            List<LIGNES_DECLARATIONS_FACTURES> Liste = BD.LIGNES_DECLARATIONS_FACTURES.Where(Element=>Element.DECLARATIONS_FACTURES.ID==Code).ToList();
            dynamic dt = from Element in Liste
                         select new
                         {
                             SOCIETE = SelectedDeclaration.DECLARATIONS.SOCIETE,
                             MATRICULE = SelectedDeclaration.DECLARATIONS.MATRICULE,
                             ACTIVITE = SelectedDeclaration.DECLARATIONS.ACTIVITE,
                             ADRESSE = SelectedDeclaration.DECLARATIONS.ADRESSE,
                             TRIMESTRE = SelectedDeclaration.TRIMESTRE,
                             ANNEE=SelectedDeclaration.ANNEE,
                             NUMERO_ORDRE=Element.NUMERO_ORDRE,
                             NUMERO_AUTORISATION=Element.NUMERO_AUTORISATION,
                             NUMERO_BC=Element.NUMERO_BC,
                             DATE_BC=Element.DATE_BC.ToShortDateString(),
                             ID_FISCAL=Element.FOURNISSEURS.ID_FISCAL,
                             NOM = Element.FOURNISSEURS.NOM,
                             NUMERO_FACTURE=Element.NUMERO_FACTURE,
                             DATE_FACTURE = Element.DATE_FACTURE.ToShortDateString(),
                             PRIX_HT=Element.PRIX_HT,
                             MONTANT_TVA = Element.MONTANT_TVA,
                             OBJET = Element.OBJET,
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/PRINT_DETAIL_DECLARATION_BCD.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Détail du déclaration BCD "+SelectedDeclaration.CODE;
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
    }
}
