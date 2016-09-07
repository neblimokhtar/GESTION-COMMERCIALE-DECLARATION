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
using System.Text;


namespace GestionCommerciale.Controllers
{
    public class DeclarationFACController : Controller
    {
        //
        // GET: /DeclarationFAC/
        GestionCommercialeEntity BD = new GestionCommercialeEntity();

        public ActionResult Index()
        {
            List<DECLARATIONS_FACS> Liste = BD.DECLARATIONS_FACS.ToList();
            return View(Liste);
        }
        public ActionResult SecondEdit(int Code, int ParamPassed)
        {
            DECLARATIONS_FACS DeclarationFAC = BD.DECLARATIONS_FACS.Find(Code);
            LIGNES_DECLARATIONS_FACS LigneDeclarationFAC = new LIGNES_DECLARATIONS_FACS();
            LigneDeclarationFAC.DATE_AUTORISATION = DateTime.Today;
            LigneDeclarationFAC.DATE_FACTURE = DateTime.Today;
            if (ParamPassed > 0)
                LigneDeclarationFAC = BD.LIGNES_DECLARATIONS_FACS.Find(ParamPassed);


            ViewBag.TRIMESTRE = DeclarationFAC.TRIMESTRE;
            ViewBag.ANNEE = DeclarationFAC.ANNEE;
            ViewBag.SOCIETE = DeclarationFAC.DECLARATIONS.SOCIETE;
            ViewBag.Code = Code;
            ViewBag.ParamPassed = ParamPassed;
            return View(LigneDeclarationFAC);
        }
        public ActionResult MyPartialViewEdit(int Code)
        {
            List<LIGNES_DECLARATIONS_FACS> Liste = BD.LIGNES_DECLARATIONS_FACS.Where(Element => Element.DECLARATIONS_FACS.ID == Code).ToList();
            ViewBag.Code = Code;
            return PartialView(Liste);
        }
        [HttpPost]
        public ActionResult SendSecondEditForm(int Code, int ParamPassed)
        {
            string NUMERO_FACTURE = Request.Params["NUMERO_FACTURE"] != null ? Request.Params["NUMERO_FACTURE"].ToString() : string.Empty;
            string DATE_FACTURE = Request.Params["DATE_FACTURE"] != null ? Request.Params["DATE_FACTURE"].ToString() : string.Empty;
            string MATRICULE = Request.Params["MATRICULE"] != null ? Request.Params["MATRICULE"].ToString() : string.Empty;
            string client = Request.Params["client"] != null ? Request.Params["client"].ToString() : string.Empty;
            string TYPE_CLIENT = Request.Params["TYPE_CLIENT"] != null ? Request.Params["TYPE_CLIENT"].ToString() : string.Empty;
            string PRIX_HT = Request.Params["PRIX_HT"] != null ? Request.Params["PRIX_HT"].ToString() : string.Empty;
            string TVA = Request.Params["TVA"] != null ? Request.Params["TVA"].ToString() : string.Empty;
            string FODEC = Request.Params["FODEC"] != null ? Request.Params["FODEC"].ToString() : string.Empty;
            string DROIT_CONSOMMATION = Request.Params["DROIT_CONSOMMATION"] != null ? Request.Params["DROIT_CONSOMMATION"].ToString() : string.Empty;
            string NUMERO_AUTORISATION = Request.Params["NUMERO_AUTORISATION"] != null ? Request.Params["NUMERO_AUTORISATION"].ToString() : string.Empty;
            string DATE_AUTORISATION = Request.Params["DATE_AUTORISATION"] != null ? Request.Params["DATE_AUTORISATION"].ToString() : string.Empty;
            string ACTION = Request.Params["ACTION"] != null ? Request.Params["ACTION"].ToString() : string.Empty;
            #region DELETE
            if (ACTION == "DELETE")
            {
                LIGNES_DECLARATIONS_FACS Ligne = new LIGNES_DECLARATIONS_FACS();
                Ligne = BD.LIGNES_DECLARATIONS_FACS.Where(Element => Element.ID == ParamPassed).FirstOrDefault();
                BD.LIGNES_DECLARATIONS_FACS.Remove(Ligne);
                BD.SaveChanges();

                for (int i = 0; i < BD.LIGNES_DECLARATIONS_FACS.ToList().Count; i++)
                {
                    BD.LIGNES_DECLARATIONS_FACS.ToList().ElementAt(i).NUMERO_ORDRE = i + 1;
                    BD.SaveChanges();
                }

            }
            #endregion
            #region ADD
            if (ACTION == "ADD")
            {
                LIGNES_DECLARATIONS_FACS Ligne = new LIGNES_DECLARATIONS_FACS();
                if (ParamPassed <= 0)
                {
                    Ligne.NUMERO_ORDRE = BD.LIGNES_DECLARATIONS_FACS.Where(Element => Element.DECLARATIONS_FACS.ID == Code).ToList().Count + 1;
                    Ligne.NUMERO_FACTURE = NUMERO_FACTURE;
                    Ligne.DATE_FACTURE = DateTime.Parse(DATE_FACTURE);
                    int ID_CLIENT = int.Parse(client);
                    CLIENTS SelectedClient = BD.CLIENTS.Find(ID_CLIENT);
                    Ligne.CLIENT = ID_CLIENT;
                    Ligne.CLIENTS = SelectedClient;
                    Ligne.TYPE_CLIENT = int.Parse(TYPE_CLIENT);

                    if (PRIX_HT.Contains(','))
                        Ligne.PRIX_HT = decimal.Parse(PRIX_HT);
                    else
                        Ligne.PRIX_HT = decimal.Parse(PRIX_HT, CultureInfo.InvariantCulture);
                    int fodec = int.Parse(FODEC);
                    int droit_consommation = int.Parse(DROIT_CONSOMMATION);

                    decimal MONTANT_FODEC = (decimal)((Ligne.PRIX_HT * fodec) / 100);
                    decimal MONTANT_DROIT_CONSOMMATION = (decimal)((Ligne.PRIX_HT * droit_consommation) / 100);

                    Ligne.FODEC = fodec;
                    Ligne.MONTANT_FODEC = MONTANT_FODEC;
                    Ligne.DROIT_CONSOMMATION = droit_consommation;
                    Ligne.MONTANT_DROIT_CONSOMMATION = MONTANT_DROIT_CONSOMMATION;



                    Ligne.TVA = int.Parse(TVA);
                    //Ligne.PRIX_HT = totale_ht;
                    Ligne.MONTANT_TVA = ((Ligne.PRIX_HT + MONTANT_FODEC + MONTANT_DROIT_CONSOMMATION) * int.Parse(TVA)) / 100;
                    Ligne.NUMERO_AUTORISATION = NUMERO_AUTORISATION;
                    Ligne.DATE_AUTORISATION = DateTime.Parse(DATE_AUTORISATION);
                    Ligne.DECLARATIONS_FACS = BD.DECLARATIONS_FACS.Find(Code);
                    Ligne.DECLARATION_FAC = Code;
                    BD.LIGNES_DECLARATIONS_FACS.Add(Ligne);
                    BD.SaveChanges();
                }
                if (ParamPassed > 0)
                {

                    Ligne = BD.LIGNES_DECLARATIONS_FACS.Where(Element => Element.DECLARATIONS_FACS.ID == Code).FirstOrDefault();
                    //Ligne.NUMERO_ORDRE = Liste.Count + 1;
                    Ligne.NUMERO_FACTURE = NUMERO_FACTURE;
                    Ligne.DATE_FACTURE = DateTime.Parse(DATE_FACTURE);
                    int ID_CLIENT = int.Parse(client);
                    CLIENTS SelectedClient = BD.CLIENTS.Find(ID_CLIENT);
                    Ligne.CLIENT = ID_CLIENT;
                    Ligne.CLIENTS = SelectedClient;
                    Ligne.TYPE_CLIENT = int.Parse(TYPE_CLIENT);

                    if (PRIX_HT.Contains(','))
                        Ligne.PRIX_HT = decimal.Parse(PRIX_HT);
                    else
                        Ligne.PRIX_HT = decimal.Parse(PRIX_HT, CultureInfo.InvariantCulture);
                    int fodec = int.Parse(FODEC);
                    int droit_consommation = int.Parse(DROIT_CONSOMMATION);

                    decimal MONTANT_FODEC = (decimal)((Ligne.PRIX_HT * fodec) / 100);
                    decimal MONTANT_DROIT_CONSOMMATION = (decimal)((Ligne.PRIX_HT * droit_consommation) / 100);

                    Ligne.FODEC = fodec;
                    Ligne.MONTANT_FODEC = MONTANT_FODEC;
                    Ligne.DROIT_CONSOMMATION = droit_consommation;
                    Ligne.MONTANT_DROIT_CONSOMMATION = MONTANT_DROIT_CONSOMMATION;



                    Ligne.TVA = int.Parse(TVA);
                    //Ligne.PRIX_HT = totale_ht;
                    Ligne.MONTANT_TVA = ((Ligne.PRIX_HT + MONTANT_FODEC + MONTANT_DROIT_CONSOMMATION) * int.Parse(TVA)) / 100;
                    Ligne.NUMERO_AUTORISATION = NUMERO_AUTORISATION;
                    Ligne.DATE_AUTORISATION = DateTime.Parse(DATE_AUTORISATION);
                    BD.SaveChanges();
                }
            }
            #endregion
            if (ACTION == "VALIDATE")
            {
                #region SaveFile
                DECLARATIONS_FACS NouvelleDeclaration = BD.DECLARATIONS_FACS.Find(Code);
                FileInfo info = new FileInfo("Fichier vide.TXT");
                string nom = "FAC_T" + NouvelleDeclaration.TRIMESTRE + "_" + NouvelleDeclaration.ANNEE.ToString().Substring(2);
                var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fileStream.Close();
                info = new FileInfo(Server.MapPath(@"~/Images/" + nom));
                //"c:\\Variables.txt", true, Encoding.ASCII
                //using (StreamWriter writer = info.CreateText())
                using (StreamWriter writer = new StreamWriter(Server.MapPath(@"~/Images/" + nom), false, Encoding.ASCII))
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
                    string EF13 = string.Empty;
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
                    while (EF13.Length < 111)
                    {
                        EF13 = EF13 + " ";
                    }
                    string FirstLine = "EF" + EF01 + EF05 + EF06 + EF07 + EF08 + EF09 + EF10 + EF11 + EF12 + EF13;
                    writer.WriteLine(FirstLine);
                    List<LIGNES_DECLARATIONS_FACS> NouvelleListe = BD.LIGNES_DECLARATIONS_FACS.Where(Element => Element.DECLARATIONS_FACS.ID == NouvelleDeclaration.ID).ToList();

                    decimal SumTotal = 0;
                    decimal SumTva = 0;
                    decimal SumFodec = 0;
                    decimal SumDroitConsommation = 0;
                    int NumeroOrdre = 1;
                    foreach (LIGNES_DECLARATIONS_FACS Ligne in NouvelleListe)
                    {
                        CLIENTS SelectedClient = BD.CLIENTS.Find(Ligne.CLIENT);
                        string DF07 = NumeroOrdre.ToString("000000");//6
                        string DF08 = Ligne.NUMERO_FACTURE;//20
                        string DF09 = Ligne.DATE_FACTURE.ToShortDateString().Replace("/", "");//8
                        string DF10 = Ligne.TYPE_CLIENT.ToString();//1
                        string DF11 = SelectedClient.ID_FISCAL != null ? SelectedClient.ID_FISCAL : string.Empty;//13
                        string DF12 = SelectedClient.NOM != null ? SelectedClient.NOM : string.Empty;//40
                        string DF13 = SelectedClient.ADRESSE != null ? SelectedClient.ADRESSE : string.Empty;//120
                        string DF14 = Ligne.NUMERO_AUTORISATION;//20
                        string DF15 = Ligne.DATE_AUTORISATION.ToShortDateString().Replace("/", "");//8
                        SumTotal += Ligne.PRIX_HT;
                        string DF16 = Ligne.PRIX_HT.ToString("F3").Replace(",", "");//15
                        SumFodec += Ligne.MONTANT_FODEC;
                        string DF17 = Ligne.FODEC.ToString("00000");//5
                        string DF18 = Ligne.MONTANT_FODEC.ToString("F3").Replace(",", "");//15
                        SumDroitConsommation += Ligne.MONTANT_DROIT_CONSOMMATION;
                        string DF19 = Ligne.DROIT_CONSOMMATION.ToString("00000");//5
                        string DF20 = Ligne.MONTANT_DROIT_CONSOMMATION.ToString("F3").Replace(",", "");//15
                        SumTva += Ligne.MONTANT_TVA;
                        string DF21 = Ligne.TVA.ToString("00000");//5
                        string DF22 = Ligne.MONTANT_TVA.ToString("F3").Replace(",", "");//15

                        while (DF08.Length < 20)
                        {
                            DF08 = DF08 + " ";
                        }
                        while (DF11.Length < 13)
                        {
                            DF11 = DF11 + " ";
                        }
                        while (DF12.Length < 40)
                        {
                            DF12 = DF12 + " ";
                        }
                        while (DF13.Length < 120)
                        {
                            DF13 = DF13 + " ";
                        }
                        if (DF13.Length > 120) { DF13 = DF13.Substring(0, 120); }
                        while (DF14.Length < 20)
                        {
                            DF14 = DF14 + " ";
                        }
                        while (DF16.Length < 15)
                        {
                            DF16 = "0" + DF16;
                        }
                        while (DF18.Length < 15)
                        {
                            DF18 = "0" + DF18;
                        }
                        while (DF20.Length < 15)
                        {
                            DF20 = "0" + DF20;
                        }
                        while (DF22.Length < 15)
                        {
                            DF22 = "0" + DF22;
                        }
                        string SubLine = "DF" + EF01 + EF05 + EF06 + DF07 + DF08 + DF09 + DF10 + DF11 + DF12 + DF13 + DF14 + DF15 + DF16 + DF17 + DF18 + DF19 + DF20 + DF21 + DF22;
                        writer.WriteLine(SubLine);
                        NumeroOrdre++;
                    }
                    string TF007 = NouvelleListe.Count.ToString("000000");//6
                    string TF08 = string.Empty;//230
                    while (TF08.Length < 230)
                    {
                        TF08 = TF08 + " ";
                    }
                    string TF09 = SumTotal.ToString("F3").Replace(",", "");//15
                    string TF10 = "00000";//50
                    string TF11 = SumFodec.ToString("F3").Replace(",", "");//15
                    string TF12 = "00000";//5
                    string TF13 = SumDroitConsommation.ToString("F3").Replace(",", "");//15
                    string TF14 = "00000";
                    string TF15 = SumTva.ToString("F3").Replace(",", "");//15
                    while (TF09.Length < 15)
                    {
                        TF09 = "0" + TF09;
                    }
                    while (TF11.Length < 15)
                    {
                        TF11 = "0" + TF11;
                    }
                    while (TF13.Length < 15)
                    {
                        TF13 = "0" + TF13;
                    }
                    while (TF15.Length < 15)
                    {
                        TF15 = "0" + TF15;
                    }
                    string LastLine = "TF" + EF01 + EF05 + EF06 + TF007 + TF08 + TF09 + TF10 + TF11 + TF12 + TF13 + TF14 + TF15;
                    writer.WriteLine(LastLine);
                }
                byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Images/" + nom));
                NouvelleDeclaration.DATA = fileBytes;
                BD.SaveChanges();
                #endregion
                Session.Clear();
                return RedirectToAction("Index");
            }
            return RedirectToAction("SecondEdit", "DeclarationFAC", new { ParamPassed = -1, @Code = Code });
        }
        public ActionResult Form(string Mode, int Code)
        {
            List<DECLARATIONS> Liste = BD.DECLARATIONS.ToList();
            ViewBag.Mode = Mode;
            ViewBag.Code = Code;
            if (Mode == "Detail")
            {
                DECLARATIONS_FACS SelectedDeclaration = BD.DECLARATIONS_FACS.Find(Code);
                string TRIMESTRE = SelectedDeclaration.TRIMESTRE != null ? SelectedDeclaration.TRIMESTRE.ToString() : string.Empty;
                string ANNEE = SelectedDeclaration.ANNEE != null ? SelectedDeclaration.ANNEE.ToString() : string.Empty;
                string SOCIETE = SelectedDeclaration.SOCIETE != null ? SelectedDeclaration.SOCIETE.ToString() : string.Empty;
                Session["TRIMESTRE_FAC"] = TRIMESTRE;
                Session["ANNEE_FAC"] = ANNEE;
                Session["SOCIETE_FAC"] = SOCIETE;
                return RedirectToAction("Second", "DeclarationFAC", new { @Mode = Mode, @Code = Code, @ANNEE = ANNEE, @SOCIETE = SOCIETE, @TRIMESTRE = TRIMESTRE });
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
            Session["TRIMESTRE_FAC"] = TRIMESTRE;
            Session["ANNEE_FAC"] = ANNEE;
            Session["SOCIETE_FAC"] = SOCIETE;
            int SelectedSociete = int.Parse(SOCIETE);
            DECLARATIONS Soc = BD.DECLARATIONS.Find(SelectedSociete);
            if (string.IsNullOrEmpty(Soc.MATRICULE))
            {
                ViewBag.ErrorText = "Matricule fiscale inexistante";
                TempData["ErrorText"] = "Matricule fiscale inexistante";
                return RedirectToAction("Form", "DeclarationFAC", new { @Mode = Mode, @Code = Code });
            }
            else
            {
                return RedirectToAction("Second", "DeclarationFAC", new { @Mode = Mode, @Code = Code, @ANNEE = ANNEE, @SOCIETE = SOCIETE, @TRIMESTRE = TRIMESTRE });
            }
        }
        public ActionResult Second(string Mode, string Code, string ANNEE, string SOCIETE, string TRIMESTRE)
        {
            int ID = int.Parse(SOCIETE);
            string SelectedSociete = BD.DECLARATIONS.Find(ID).SOCIETE;
            CLIENTS Client = new CLIENTS();
            ViewBag.TRIMESTRE = TRIMESTRE;
            ViewBag.ANNEE = ANNEE;
            ViewBag.SOCIETE = SelectedSociete;
            ViewBag.Mode = Mode;
            ViewBag.Code = Code;
            LIGNES_DECLARATIONS_FACS Ligne = new LIGNES_DECLARATIONS_FACS();
            Ligne.DATE_AUTORISATION = DateTime.Today;
            Ligne.DATE_FACTURE = DateTime.Today;
            if (Mode == "Edit")
            {
                List<LIGNES_DECLARATIONS_FACS> Liste = new List<LIGNES_DECLARATIONS_FACS>();
                if (Session["LignesFacture_FAC"] != null)
                {
                    Liste = (List<LIGNES_DECLARATIONS_FACS>)Session["LignesFacture_FAC"];
                    if (Liste.Count > 0)
                    {
                        int SelectedRow = int.Parse(Code);
                        Ligne = Liste.Where(Element => Element.NUMERO_ORDRE == SelectedRow).FirstOrDefault();
                    }
                }
            }
            return View(Ligne);
        }
        public PartialViewResult MyPartialView(string Mode, int Code)
        {
            List<LIGNES_DECLARATIONS_FACS> Liste = new List<LIGNES_DECLARATIONS_FACS>();
            if (Session["LignesFacture_FAC"] != null)
            {
                Liste = (List<LIGNES_DECLARATIONS_FACS>)Session["LignesFacture_FAC"];
                Liste = Liste.OrderBy(Element => Element.NUMERO_ORDRE).ToList();
            }
            if (Mode == "Detail")
            {
                Liste = BD.LIGNES_DECLARATIONS_FACS.Where(Element => Element.DECLARATIONS_FACS.ID == Code).ToList();
            }
            string ANNEE = Session["ANNEE_FAC"] != null ? Session["ANNEE_FAC"].ToString() : string.Empty;
            string SOCIETE = Session["SOCIETE_FAC"] != null ? Session["SOCIETE_FAC"].ToString() : string.Empty;
            string TRIMESTRE = Session["TRIMESTRE_FAC"] != null ? Session["TRIMESTRE_FAC"].ToString() : string.Empty;
            ViewBag.SOCIETE = SOCIETE;
            ViewBag.TRIMESTRE = TRIMESTRE;
            ViewBag.ANNEE = ANNEE;
            ViewBag.Mode = Mode;
            return PartialView(Liste);
        }
        [HttpPost]
        public ActionResult SendSecondForm(string Mode, string Code)
        {
            string ANNEE = Session["ANNEE_FAC"] != null ? Session["ANNEE_FAC"].ToString() : string.Empty;
            string SOCIETE = Session["SOCIETE_FAC"] != null ? Session["SOCIETE_FAC"].ToString() : string.Empty;
            string TRIMESTRE = Session["TRIMESTRE_FAC"] != null ? Session["TRIMESTRE_FAC"].ToString() : string.Empty;
            List<LIGNES_DECLARATIONS_FACS> Liste = new List<LIGNES_DECLARATIONS_FACS>();
            if (Session["LignesFacture_FAC"] != null)
            {
                Liste = (List<LIGNES_DECLARATIONS_FACS>)Session["LignesFacture_FAC"];
            }
            string NUMERO_FACTURE = Request.Params["NUMERO_FACTURE"] != null ? Request.Params["NUMERO_FACTURE"].ToString() : string.Empty;
            string DATE_FACTURE = Request.Params["DATE_FACTURE"] != null ? Request.Params["DATE_FACTURE"].ToString() : string.Empty;
            string MATRICULE = Request.Params["MATRICULE"] != null ? Request.Params["MATRICULE"].ToString() : string.Empty;
            string client = Request.Params["client"] != null ? Request.Params["client"].ToString() : string.Empty;
            string TYPE_CLIENT = Request.Params["TYPE_CLIENT"] != null ? Request.Params["TYPE_CLIENT"].ToString() : string.Empty;
            string PRIX_HT = Request.Params["PRIX_HT"] != null ? Request.Params["PRIX_HT"].ToString() : string.Empty;
            string TVA = Request.Params["TVA"] != null ? Request.Params["TVA"].ToString() : string.Empty;
            string FODEC = Request.Params["FODEC"] != null ? Request.Params["FODEC"].ToString() : string.Empty;
            string DROIT_CONSOMMATION = Request.Params["DROIT_CONSOMMATION"] != null ? Request.Params["DROIT_CONSOMMATION"].ToString() : string.Empty;
            string NUMERO_AUTORISATION = Request.Params["NUMERO_AUTORISATION"] != null ? Request.Params["NUMERO_AUTORISATION"].ToString() : string.Empty;
            string DATE_AUTORISATION = Request.Params["DATE_AUTORISATION"] != null ? Request.Params["DATE_AUTORISATION"].ToString() : string.Empty;
            string ACTION = Request.Params["ACTION"] != null ? Request.Params["ACTION"].ToString() : string.Empty;
            if (ACTION == "BACK")
            {
                return RedirectToAction("Form", "DeclarationFAC", new { @Mode = "Edit", @Code = -1 });
            }
            if (ACTION == "PRINT")
            {

            }
            if (ACTION == "VALIDATE")
            {
                DECLARATIONS_FACS NouvelleDeclaration = new DECLARATIONS_FACS();
                GestionCommercialeEntity BD_A = new GestionCommercialeEntity();
                GestionCommercialeEntity BD_B = new GestionCommercialeEntity();
                int IdSociete = int.Parse(SOCIETE);
                DECLARATIONS SelectedSociete = BD.DECLARATIONS.Find(IdSociete);
                NouvelleDeclaration.ANNEE = int.Parse(ANNEE);
                NouvelleDeclaration.CODE = "FAC_T" + TRIMESTRE + "_" + ANNEE.Substring(2);
                NouvelleDeclaration.DATE = DateTime.Today;
                NouvelleDeclaration.TRIMESTRE = int.Parse(TRIMESTRE);
                NouvelleDeclaration.DECLARATIONS = SelectedSociete;
                NouvelleDeclaration.SOCIETE = IdSociete;
                NouvelleDeclaration.VALIDE = true;
                BD.DECLARATIONS_FACS.Add(NouvelleDeclaration);
                BD.SaveChanges();
                foreach (LIGNES_DECLARATIONS_FACS Ligne in Liste)
                {
                    LIGNES_DECLARATIONS_FACS NouvelleLigne = new LIGNES_DECLARATIONS_FACS();

                    NouvelleLigne.NUMERO_FACTURE = Ligne.NUMERO_FACTURE;
                    NouvelleLigne.DATE_FACTURE = Ligne.DATE_FACTURE;

                    NouvelleLigne.DECLARATION_FAC = NouvelleDeclaration.ID;
                    NouvelleLigne.DECLARATIONS_FACS = NouvelleDeclaration;

                    NouvelleLigne.MONTANT_TVA = Ligne.MONTANT_TVA;
                    NouvelleLigne.FODEC = Ligne.FODEC;
                    NouvelleLigne.DROIT_CONSOMMATION = Ligne.DROIT_CONSOMMATION;
                    NouvelleLigne.PRIX_HT = Ligne.PRIX_HT;
                    NouvelleLigne.TVA = Ligne.TVA;

                    NouvelleLigne.DATE_AUTORISATION = Ligne.DATE_AUTORISATION;
                    NouvelleLigne.NUMERO_AUTORISATION = Ligne.NUMERO_AUTORISATION;

                    NouvelleLigne.NUMERO_ORDRE = Ligne.NUMERO_ORDRE;



                    CLIENTS Cli = Ligne.CLIENTS;
                    NouvelleLigne.CLIENT = Cli.ID;
                    NouvelleLigne.TYPE_CLIENT = Ligne.TYPE_CLIENT;

                    BD.LIGNES_DECLARATIONS_FACS.Add(NouvelleLigne);
                    BD.SaveChanges();
                }
                #region SaveFile
                FileInfo info = new FileInfo("Fichier vide.TXT");
                string nom = "FAC_T" + TRIMESTRE + "_" + ANNEE.Substring(2);
                var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fileStream.Close();
                info = new FileInfo(Server.MapPath(@"~/Images/" + nom));
                //"c:\\Variables.txt", true, Encoding.ASCII
                using (StreamWriter writer = new StreamWriter(Server.MapPath(@"~/Images/" + nom), true, Encoding.ASCII))
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
                    string EF13 = string.Empty;
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
                    while (EF13.Length < 111)
                    {
                        EF13 = EF13 + " ";
                    }
                    string FirstLine = "EF" + EF01 + EF05 + EF06 + EF07 + EF08 + EF09 + EF10 + EF11 + EF12 + EF13;
                    writer.WriteLine(FirstLine);
                    List<LIGNES_DECLARATIONS_FACS> NouvelleListe = BD.LIGNES_DECLARATIONS_FACS.Where(Element => Element.DECLARATIONS_FACS.ID == NouvelleDeclaration.ID).ToList();

                    decimal SumTotal = 0;
                    decimal SumTva = 0;
                    decimal SumFodec = 0;
                    decimal SumDroitConsommation = 0;
                    int NumeroOrdre = 1;
                    foreach (LIGNES_DECLARATIONS_FACS Ligne in NouvelleListe)
                    {
                        CLIENTS SelectedClient = BD.CLIENTS.Find(Ligne.CLIENT);
                        string DF07 = NumeroOrdre.ToString("000000");//6
                        string DF08 = Ligne.NUMERO_FACTURE;//20
                        string DF09 = Ligne.DATE_FACTURE.ToShortDateString().Replace("/", "");//8
                        string DF10 = Ligne.TYPE_CLIENT.ToString();//1
                        string DF11 = SelectedClient.ID_FISCAL != null ? SelectedClient.ID_FISCAL : string.Empty;//13
                        string DF12 = SelectedClient.NOM != null ? SelectedClient.NOM : string.Empty;//40
                        string DF13 = SelectedClient.ADRESSE != null ? SelectedClient.ADRESSE : string.Empty;//120
                        string DF14 = Ligne.NUMERO_AUTORISATION;//20
                        string DF15 = Ligne.DATE_AUTORISATION.ToShortDateString().Replace("/", "");//8
                        SumTotal += Ligne.PRIX_HT;
                        string DF16 = Ligne.PRIX_HT.ToString("F3").Replace(",", "");//15
                        SumFodec += Ligne.MONTANT_FODEC;
                        string DF17 = Ligne.FODEC.ToString("00000");//5
                        string DF18 = Ligne.MONTANT_FODEC.ToString("F3").Replace(",", "");//15
                        SumDroitConsommation += Ligne.MONTANT_DROIT_CONSOMMATION;
                        string DF19 = Ligne.DROIT_CONSOMMATION.ToString("00000");//5
                        string DF20 = Ligne.MONTANT_DROIT_CONSOMMATION.ToString("F3").Replace(",", "");//15
                        SumTva += Ligne.MONTANT_TVA;
                        string DF21 = Ligne.TVA.ToString("00000");//5
                        string DF22 = Ligne.MONTANT_TVA.ToString("F3").Replace(",", "");//15

                        while (DF08.Length < 20)
                        {
                            DF08 = DF08 + " ";
                        }
                        while (DF11.Length < 13)
                        {
                            DF11 = DF11 + " ";
                        }
                        while (DF12.Length < 40)
                        {
                            DF12 = DF12 + " ";
                        }
                        while (DF13.Length < 120)
                        {
                            DF13 = DF13 + " ";
                        }
                        if (DF13.Length > 120) { DF13 = DF13.Substring(0, 120); }
                        while (DF14.Length < 20)
                        {
                            DF14 = DF14 + " ";
                        }
                        while (DF16.Length < 15)
                        {
                            DF16 = "0" + DF16;
                        }
                        while (DF18.Length < 15)
                        {
                            DF18 = "0" + DF18;
                        }
                        while (DF20.Length < 15)
                        {
                            DF20 = "0" + DF20;
                        }
                        while (DF22.Length < 15)
                        {
                            DF22 = "0" + DF22;
                        }
                        string SubLine = "DF" + EF01 + EF05 + EF06 + DF07 + DF08 + DF09 + DF10 + DF11 + DF12 + DF13 + DF14 + DF15 + DF16 + DF17 + DF18 + DF19 + DF20 + DF21 + DF22;
                        writer.WriteLine(SubLine);
                        NumeroOrdre++;
                    }
                    string TF007 = NouvelleListe.Count.ToString("000000");//6
                    string TF08 = string.Empty;//230
                    while (TF08.Length < 230)
                    {
                        TF08 = TF08 + " ";
                    }
                    string TF09 = SumTotal.ToString("F3").Replace(",", "");//15
                    string TF10 = "00000";//50
                    string TF11 = SumFodec.ToString("F3").Replace(",", "");//15
                    string TF12 = "00000";//5
                    string TF13 = SumDroitConsommation.ToString("F3").Replace(",", "");//15
                    string TF14 = "00000";
                    string TF15 = SumTva.ToString("F3").Replace(",", "");//15
                    while (TF09.Length < 15)
                    {
                        TF09 = "0" + TF09;
                    }
                    while (TF11.Length < 15)
                    {
                        TF11 = "0" + TF11;
                    }
                    while (TF13.Length < 15)
                    {
                        TF13 = "0" + TF13;
                    }
                    while (TF15.Length < 15)
                    {
                        TF15 = "0" + TF15;
                    }
                    string LastLine = "TF" + EF01 + EF05 + EF06 + TF007 + TF08 + TF09 + TF10 + TF11 + TF12 + TF13 + TF14 + TF15;
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
                LIGNES_DECLARATIONS_FACS Ligne = new LIGNES_DECLARATIONS_FACS();
                if (Mode == "Create")
                {
                    Ligne.NUMERO_ORDRE = Liste.Count + 1;
                    Ligne.NUMERO_FACTURE = NUMERO_FACTURE;
                    Ligne.DATE_FACTURE = DateTime.Parse(DATE_FACTURE);
                    int ID_CLIENT = int.Parse(client);
                    CLIENTS SelectedClient = BD.CLIENTS.Find(ID_CLIENT);
                    Ligne.CLIENT = ID_CLIENT;
                    Ligne.CLIENTS = SelectedClient;
                    Ligne.TYPE_CLIENT = int.Parse(TYPE_CLIENT);

                    if (PRIX_HT.Contains(','))
                        Ligne.PRIX_HT = decimal.Parse(PRIX_HT);
                    else
                        Ligne.PRIX_HT = decimal.Parse(PRIX_HT, CultureInfo.InvariantCulture);
                    int fodec = int.Parse(FODEC);
                    int droit_consommation = int.Parse(DROIT_CONSOMMATION);

                    decimal MONTANT_FODEC = (decimal)((Ligne.PRIX_HT * fodec) / 100);
                    decimal MONTANT_DROIT_CONSOMMATION = (decimal)((Ligne.PRIX_HT * droit_consommation) / 100);

                    Ligne.FODEC = fodec;
                    Ligne.MONTANT_FODEC = MONTANT_FODEC;
                    Ligne.DROIT_CONSOMMATION = droit_consommation;
                    Ligne.MONTANT_DROIT_CONSOMMATION = MONTANT_DROIT_CONSOMMATION;



                    Ligne.TVA = int.Parse(TVA);
                    //Ligne.PRIX_HT = totale_ht;
                    Ligne.MONTANT_TVA = ((Ligne.PRIX_HT + MONTANT_FODEC + MONTANT_DROIT_CONSOMMATION) * int.Parse(TVA)) / 100;
                    Ligne.NUMERO_AUTORISATION = NUMERO_AUTORISATION;
                    Ligne.DATE_AUTORISATION = DateTime.Parse(DATE_AUTORISATION);
                    Liste.Add(Ligne);
                }
                if (Mode == "Edit")
                {
                    int SelectedRow = int.Parse(Code);
                    Ligne = Liste.Where(Element => Element.NUMERO_ORDRE == SelectedRow).FirstOrDefault();
                    //Ligne.NUMERO_ORDRE = Liste.Count + 1;
                    Ligne.NUMERO_FACTURE = NUMERO_FACTURE;
                    Ligne.DATE_FACTURE = DateTime.Parse(DATE_FACTURE);
                    int ID_CLIENT = int.Parse(client);
                    CLIENTS SelectedClient = BD.CLIENTS.Find(ID_CLIENT);
                    Ligne.CLIENT = ID_CLIENT;
                    Ligne.CLIENTS = SelectedClient;
                    Ligne.TYPE_CLIENT = int.Parse(TYPE_CLIENT);

                    if (PRIX_HT.Contains(','))
                        Ligne.PRIX_HT = decimal.Parse(PRIX_HT);
                    else
                        Ligne.PRIX_HT = decimal.Parse(PRIX_HT, CultureInfo.InvariantCulture);
                    int fodec = int.Parse(FODEC);
                    int droit_consommation = int.Parse(DROIT_CONSOMMATION);

                    decimal MONTANT_FODEC = (decimal)((Ligne.PRIX_HT * fodec) / 100);
                    decimal MONTANT_DROIT_CONSOMMATION = (decimal)((Ligne.PRIX_HT * droit_consommation) / 100);

                    Ligne.FODEC = fodec;
                    Ligne.MONTANT_FODEC = MONTANT_FODEC;
                    Ligne.DROIT_CONSOMMATION = droit_consommation;
                    Ligne.MONTANT_DROIT_CONSOMMATION = MONTANT_DROIT_CONSOMMATION;



                    Ligne.TVA = int.Parse(TVA);
                    //Ligne.PRIX_HT = totale_ht;
                    Ligne.MONTANT_TVA = ((Ligne.PRIX_HT + MONTANT_FODEC + MONTANT_DROIT_CONSOMMATION) * int.Parse(TVA)) / 100;
                    Ligne.NUMERO_AUTORISATION = NUMERO_AUTORISATION;
                    Ligne.DATE_AUTORISATION = DateTime.Parse(DATE_AUTORISATION);
                }
            }
            if (ACTION == "DELETE")
            {
                LIGNES_DECLARATIONS_FACS Ligne = new LIGNES_DECLARATIONS_FACS();
                int SelectedRow = int.Parse(Code);
                Ligne = Liste.Where(Element => Element.NUMERO_ORDRE == SelectedRow).FirstOrDefault();
                Liste.Remove(Ligne);
                for (int i = 0; i < Liste.Count; i++)
                {
                    Liste.ElementAt(i).NUMERO_ORDRE = i + 1;
                }
            }
            Session["LignesFacture_FAC"] = Liste;
            return RedirectToAction("Second", "DeclarationFAC", new { @Mode = "Create", @Code = "-1", @ANNEE = ANNEE, @SOCIETE = SOCIETE, @TRIMESTRE = TRIMESTRE });
        }
        public ActionResult GetFileByID(int id)
        {
            var fileToRetrieve = BD.DECLARATIONS_FACS.Find(id);
            return File(fileToRetrieve.DATA, System.Net.Mime.MediaTypeNames.Application.Octet, fileToRetrieve.CODE + ".TXT");
        }
        public ActionResult Delete(int Code)
        {
            DECLARATIONS_FACS Selected = BD.DECLARATIONS_FACS.Find(Code);
            BD.LIGNES_DECLARATIONS_FACS.Where(p => p.DECLARATIONS_FACS.ID == Code).ToList().ForEach(p => BD.LIGNES_DECLARATIONS_FACS.Remove(p));
            BD.SaveChanges();
            BD.DECLARATIONS_FACS.Remove(Selected);
            BD.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult PrintAllDeclaration()
        {
            List<DECLARATIONS_FACS> Liste = BD.DECLARATIONS_FACS.ToList();
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
            rptH.SummaryInfo.ReportTitle = "Liste des déclarations FAC";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintDeclaration(int Code)
        {
            DECLARATIONS_FACS SelectedDeclaration = BD.DECLARATIONS_FACS.Find(Code);
            List<LIGNES_DECLARATIONS_FACS> Liste = BD.LIGNES_DECLARATIONS_FACS.Where(Element => Element.DECLARATIONS_FACS.ID == Code).ToList();
            dynamic dt = from Element in Liste
                         select new
                         {
                             SOCIETE = SelectedDeclaration.DECLARATIONS.SOCIETE,
                             MATRICULE = SelectedDeclaration.DECLARATIONS.MATRICULE,
                             ACTIVITE = SelectedDeclaration.DECLARATIONS.ACTIVITE,
                             ADRESSE = SelectedDeclaration.DECLARATIONS.ADRESSE,
                             TRIMESTRE = SelectedDeclaration.TRIMESTRE,
                             ANNEE = SelectedDeclaration.ANNEE,
                             NUMERO_ORDRE = Element.NUMERO_ORDRE,
                             NUMERO_AUTORISATION = Element.NUMERO_AUTORISATION,
                             DATE_AUTORISATION = Element.DATE_AUTORISATION.ToShortDateString(),
                             ID_FISCAL = Element.CLIENTS.ID_FISCAL,
                             NOM = Element.CLIENTS.NOM,
                             NUMERO_FACTURE = Element.NUMERO_FACTURE,
                             DATE_FACTURE = Element.DATE_FACTURE.ToShortDateString(),
                             PRIX_HT = Element.PRIX_HT,
                             MONTANT_TVA = Element.MONTANT_TVA,
                             MONTANT_FODEC = Element.MONTANT_FODEC,
                             MONTANT_DROIT_CONSOMMATION = Element.MONTANT_DROIT_CONSOMMATION,
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/PRINT_DETAIL_DECLARATION_FAC.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Détail du déclaration FAC " + SelectedDeclaration.CODE;
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
    }
}
