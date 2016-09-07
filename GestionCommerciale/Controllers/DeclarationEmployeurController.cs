using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestionCommerciale.Models;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Text;
using CrystalDecisions.CrystalReports.Engine;

namespace GestionCommerciale.Controllers
{
    public class DeclarationEmployeurController : Controller
    {
        //
        // GET: /DeclarationEmployeur/
        GestionCommercialeEntity BD = new GestionCommercialeEntity();
        public ActionResult Index()
        {
            List<DECLARATIONS_EMPLOYEURS> Liste = BD.DECLARATIONS_EMPLOYEURS.ToList();
            return View(Liste);
        }
        public ActionResult Form()
        {
            List<DECLARATIONS> Liste = BD.DECLARATIONS.ToList();
            return View(Liste);
        }
        public ActionResult Detail(int Code)
        {
            DECLARATIONS_EMPLOYEURS SelectedDeclaration = BD.DECLARATIONS_EMPLOYEURS.Find(Code);
            ViewBag.CODE = SelectedDeclaration.CODE != null ? SelectedDeclaration.CODE : string.Empty;
            ViewBag.SOCIETE = SelectedDeclaration.DECLARATIONS != null ? SelectedDeclaration.DECLARATIONS.SOCIETE : string.Empty;
            ViewBag.ANNEE = SelectedDeclaration.ANNEE != null ? SelectedDeclaration.ANNEE.ToString() : string.Empty;
            ViewBag.DATE = SelectedDeclaration.DATE != null ? SelectedDeclaration.DATE.ToShortDateString() : string.Empty;
            ViewBag.ACTE = string.Empty;
            if (SelectedDeclaration.CODE_ACTE == 0)
                ViewBag.ACTE = "Spontané";
            if (SelectedDeclaration.CODE_ACTE == 1)
                ViewBag.ACTE = "Régularisation";
            if (SelectedDeclaration.CODE_ACTE == 2)
                ViewBag.ACTE = "Redressement";
            ViewBag.ID = SelectedDeclaration.ID;
            return View();
        }
        public ActionResult PrintAnnexe1(int Code)
        {
            DECLARATIONS_EMPLOYEURS SelectedDeclaration = BD.DECLARATIONS_EMPLOYEURS.Find(Code);
            List<ANNEXE_1> Liste = BD.ANNEXE_1.Where(Element => Element.DECLARATIONS_EMPLOYEURS.ID == Code).ToList();
            dynamic dt = from Element in Liste
                         select new
                         {
                             SOCIETE = SelectedDeclaration.DECLARATIONS.SOCIETE,
                             ANNEE = SelectedDeclaration.ANNEE,
                             DATE = SelectedDeclaration.DATE.ToShortDateString(),
                             CODE_ACTE = SelectedDeclaration.CODE_ACTE,
                             TYPE = Element.TYPE,
                             CIN = Element.EMPLOYEES.CIN,
                             FULLNAME = Element.EMPLOYEES.FULLNAME,
                             ACTIVITE = Element.EMPLOYEES.ACTIVITE,
                             NOMBRE_ENFANT = Element.EMPLOYEES.NOMBRE_ENFANT,
                             SITUATION_FAMILIALE = Element.EMPLOYEES.SITUATION_FAMILIALE,
                             ADRESSE = Element.EMPLOYEES.ADRESSE,
                             DATE_DEBUT = Element.DATE_DEBUT.ToShortDateString(),
                             DATE_FIN = Element.DATE_FIN.ToShortDateString(),
                             DUREE = Element.DUREE,
                             REVENU_IMPOSABLE = Element.REVENU_IMPOSABLE,
                             AVANTAGE_EN_NATURE = Element.AVANTAGE_EN_NATURE,
                             BRUT = Element.BRUT,
                             MONTANT_REINVESTI = Element.MONTANT_REINVESTI,
                             RETENUE_IRPP = Element.RETENUE_IRPP,
                             RETENUE_20 = Element.RETENUE_20,
                             REDEVANCE = Element.REDEVANCE,
                             NET_SERVI = Element.NET_SERVI
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/PRINT_ANNEXE_1.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "ANNEXE 1 ";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintAnnexe2(int Code)
        {
            DECLARATIONS_EMPLOYEURS SelectedDeclaration = BD.DECLARATIONS_EMPLOYEURS.Find(Code);
            List<ANNEXE_2> Liste = BD.ANNEXE_2.Where(Element => Element.DECLARATIONS_EMPLOYEURS.ID == Code).ToList();
            dynamic dt = from Element in Liste
                         select new
                         {
                             SOCIETE = SelectedDeclaration.DECLARATIONS.SOCIETE,
                             ANNEE = SelectedDeclaration.ANNEE,
                             DATE = SelectedDeclaration.DATE.ToShortDateString(),
                             CODE_ACTE = SelectedDeclaration.CODE_ACTE,
                             TYPE = Element.TYPE,
                             IDENTIFIANT = Element.IDENTIFIANT,
                             NOM_PRENOM = Element.NOM_PRENOM,
                             ACTIVITE = Element.ACTIVITE,
                             ADRESSE = Element.ADRESSE,
                             TYPE_MONTANT = Element.TYPE_MONTANT,
                             HONORAIRE_NON_COMMERCIALE = Element.HONORAIRE_NON_COMMERCIALE,
                             HONORAIRE_SOCIETE = Element.HONORAIRE_SOCIETE,
                             JETON = Element.JETON,
                             REMUNERATION = Element.REMUNERATION,
                             PLUS_VALUE_IMMOBILIERE = Element.PLUS_VALUE_IMMOBILIERE,
                             HOTEL = Element.HOTEL,
                             ARTISTES = Element.ARTISTES,
                             BUREAU_ETUDE_EXPORTATEUR = Element.BUREAU_ETUDE_EXPORTATEUR,
                             AUTRES = Element.AUTRES,
                             TYPE_MONTANT_OPERATION_EXPORTATION = Element.TYPE_MONTANT_OPERATION_EXPORTATION,
                             RETENUE = Element.RETENUE,
                             REDEVANCE_CGC = Element.REDEVANCE_CGC,
                             NET_SERVI = Element.NET_SERVI
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/PRINT_ANNEXE_2.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "ANNEXE 2 ";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult Download()
        {
            PARAMETRES Parametre = BD.PARAMETRES.FirstOrDefault();
            if (Parametre.ANNEXE_1 == null)
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(@"~/MODELES EXCEL/MODELE_ANNEXE_1.xls"));
                Parametre.ANNEXE_1 = fileBytes;
                BD.SaveChanges();
            }
            if (Parametre.ANNEXE_2 == null)
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(@"~/MODELES EXCEL/MODELE_ANNEXE_2.xls"));
                Parametre.ANNEXE_2 = fileBytes;
                BD.SaveChanges();
            }
            if (Parametre.ANNEXE_3 == null)
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(@"~/MODELES EXCEL/MODELE_ANNEXE_3.xls"));
                Parametre.ANNEXE_3 = fileBytes;
                BD.SaveChanges();
            }
            if (Parametre.ANNEXE_4 == null)
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(@"~/MODELES EXCEL/MODELE_ANNEXE_4.xls"));
                Parametre.ANNEXE_4 = fileBytes;
                BD.SaveChanges();
            }
            if (Parametre.ANNEXE_5 == null)
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(@"~/MODELES EXCEL/MODELE_ANNEXE_5.xls"));
                Parametre.ANNEXE_5 = fileBytes;
                BD.SaveChanges();
            }
            if (Parametre.ANNEXE_6 == null)
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(@"~/MODELES EXCEL/MODELE_ANNEXE_6.xls"));
                Parametre.ANNEXE_6 = fileBytes;
                BD.SaveChanges();
            }
            if (Parametre.ANNEXE_7 == null)
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(@"~/MODELES EXCEL/MODELE_ANNEXE_7.xls"));
                Parametre.ANNEXE_7 = fileBytes;
                BD.SaveChanges();
            }
            if (Parametre.RECAP == null)
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(@"~/MODELES EXCEL/RECAP.xls"));
                Parametre.RECAP = fileBytes;
                BD.SaveChanges();
            }
            return View();
        }
        public FileResult GetFile(int CODE)
        {
            var fileToRetrieve = BD.PARAMETRES.FirstOrDefault();
            if (CODE == 1)
                return File(fileToRetrieve.ANNEXE_1, System.Net.Mime.MediaTypeNames.Application.Octet, "ANNEXE_1.xls");
            if (CODE == 2)
                return File(fileToRetrieve.ANNEXE_2, System.Net.Mime.MediaTypeNames.Application.Octet, "ANNEXE_2.xls");
            if (CODE == 3)
                return File(fileToRetrieve.ANNEXE_3, System.Net.Mime.MediaTypeNames.Application.Octet, "ANNEXE_3.xls");
            if (CODE == 4)
                return File(fileToRetrieve.ANNEXE_4, System.Net.Mime.MediaTypeNames.Application.Octet, "ANNEXE_4.xls");
            if (CODE == 5)
                return File(fileToRetrieve.ANNEXE_5, System.Net.Mime.MediaTypeNames.Application.Octet, "ANNEXE_5.xls");
            if (CODE == 6)
                return File(fileToRetrieve.ANNEXE_6, System.Net.Mime.MediaTypeNames.Application.Octet, "ANNEXE_6.xls");
            if (CODE == 7)
                return File(fileToRetrieve.ANNEXE_7, System.Net.Mime.MediaTypeNames.Application.Octet, "ANNEXE_7.xls");
            if (CODE == 999)
                return File(fileToRetrieve.RECAP, System.Net.Mime.MediaTypeNames.Application.Octet, "RECAP.xls");
            return null;

        }
        public ActionResult GetFileByID(int id, string filter)
        {
            var fileToRetrieve = BD.DECLARATIONS_EMPLOYEURS.Find(id);
            if (filter == "RECAP")
                return File(fileToRetrieve.DATA, System.Net.Mime.MediaTypeNames.Application.Octet, fileToRetrieve.CODE);
            if (filter == "ANNEXE_1")
                return File(fileToRetrieve.ANNEXE_1_DATA, System.Net.Mime.MediaTypeNames.Application.Octet, "ANXEMP_1_" + fileToRetrieve.ANNEE.ToString().Substring(2) + "_1");
            if (filter == "ANNEXE_2")
                return File(fileToRetrieve.ANNEXE_2_DATA, System.Net.Mime.MediaTypeNames.Application.Octet, "ANXEMP_2_" + fileToRetrieve.ANNEE.ToString().Substring(2) + "_1");
            if (filter == "ANNEXE_3")
                return File(fileToRetrieve.ANNEXE_3_DATA, System.Net.Mime.MediaTypeNames.Application.Octet, "ANXEMP_3_" + fileToRetrieve.ANNEE.ToString().Substring(2) + "_1");
            if (filter == "ANNEXE_4")
                return File(fileToRetrieve.ANNEXE_4_DATA, System.Net.Mime.MediaTypeNames.Application.Octet, "ANXEMP_4_" + fileToRetrieve.ANNEE.ToString().Substring(2) + "_1");
            if (filter == "ANNEXE_5")
                return File(fileToRetrieve.ANNEXE_5_DATA, System.Net.Mime.MediaTypeNames.Application.Octet, "ANXEMP_5_" + fileToRetrieve.ANNEE.ToString().Substring(2) + "_1");
            if (filter == "ANNEXE_6")
                return File(fileToRetrieve.ANNEXE_6_DATA, System.Net.Mime.MediaTypeNames.Application.Octet, "ANXEMP_6_" + fileToRetrieve.ANNEE.ToString().Substring(2) + "_1");
            if (filter == "ANNEXE_7")
                return File(fileToRetrieve.ANNEXE_7_DATA, System.Net.Mime.MediaTypeNames.Application.Octet, "ANXEMP_7_" + fileToRetrieve.ANNEE.ToString().Substring(2) + "_1");
            return null;
        }
        public ActionResult Delete(int Code)
        {
            DECLARATIONS_EMPLOYEURS Selected = BD.DECLARATIONS_EMPLOYEURS.Find(Code);
            BD.ANNEXE_1.Where(p => p.DECLARATIONS_EMPLOYEURS.ID == Code).ToList().ForEach(p => BD.ANNEXE_1.Remove(p));
            BD.SaveChanges();
            BD.ANNEXE_2.Where(p => p.DECLARATIONS_EMPLOYEURS.ID == Code).ToList().ForEach(p => BD.ANNEXE_2.Remove(p));
            BD.SaveChanges();
            BD.ANNEXE_3.Where(p => p.DECLARATIONS_EMPLOYEURS.ID == Code).ToList().ForEach(p => BD.ANNEXE_3.Remove(p));
            BD.SaveChanges();
            BD.ANNEXE_4.Where(p => p.DECLARATIONS_EMPLOYEURS.ID == Code).ToList().ForEach(p => BD.ANNEXE_4.Remove(p));
            BD.SaveChanges();
            BD.ANNEXE_5.Where(p => p.DECLARATIONS_EMPLOYEURS.ID == Code).ToList().ForEach(p => BD.ANNEXE_5.Remove(p));
            BD.SaveChanges();
            BD.ANNEXE_6.Where(p => p.DECLARATIONS_EMPLOYEURS.ID == Code).ToList().ForEach(p => BD.ANNEXE_6.Remove(p));
            BD.SaveChanges();
            BD.ANNEXE_7.Where(p => p.DECLARATIONS_EMPLOYEURS.ID == Code).ToList().ForEach(p => BD.ANNEXE_7.Remove(p));
            BD.SaveChanges();
            BD.DECLARATIONS_EMPLOYEURS.Remove(Selected);
            BD.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult SendFormToIndex()
        {
            HttpPostedFileBase ANNEXE_1 = Request.Files["ANNEXE_1"];
            HttpPostedFileBase ANNEXE_2 = Request.Files["ANNEXE_2"];
            HttpPostedFileBase ANNEXE_3 = Request.Files["ANNEXE_3"];
            HttpPostedFileBase ANNEXE_4 = Request.Files["ANNEXE_4"];
            HttpPostedFileBase ANNEXE_5 = Request.Files["ANNEXE_5"];
            HttpPostedFileBase ANNEXE_6 = Request.Files["ANNEXE_6"];
            HttpPostedFileBase ANNEXE_7 = Request.Files["ANNEXE_7"];
            HttpPostedFileBase RECAP = Request.Files["RECAP"];
            string ANNEE = Request.Params["ANNEE"] != null ? Request.Params["ANNEE"].ToString() : string.Empty;
            string SOCIETE = Request.Params["SOCIETE"] != null ? Request.Params["SOCIETE"].ToString() : string.Empty;
            string CODE_ACTE = Request.Params["CODE_ACTE"] != null ? Request.Params["CODE_ACTE"].ToString() : string.Empty;
            string ACTION = Request.Params["ACTION"] != null ? Request.Params["ACTION"].ToString() : string.Empty;
            int SOCIETE_ID = int.Parse(SOCIETE);
            DECLARATIONS SelectedSociete = BD.DECLARATIONS.Find(SOCIETE_ID);
            string D006 = "1";
            string D007 = "1";
            string D008 = "1";
            string D009 = "1";
            string D010 = "1";
            string D011 = "1";
            string D012 = "1";
            int LastAdded = AddNew(ANNEE, SelectedSociete, CODE_ACTE);
            DECLARATIONS_EMPLOYEURS SelectedDeclaration = BD.DECLARATIONS_EMPLOYEURS.Find(LastAdded);
            if (ANNEXE_1 != null && ANNEXE_1.ContentLength > 0)
            {
                string path = Path.Combine(Server.MapPath("~/Images/"), ANNEXE_1.FileName);
                ANNEXE_1.SaveAs(path);
                Uploadfile(ANNEXE_1.InputStream, path, "ANNEXE_1");
                D006 = "0";
                byte[] file = SaveAnnexe1(ANNEE, SelectedSociete, CODE_ACTE, SelectedDeclaration);
                SelectedDeclaration.ANNEXE_1_DATA = file;
                BD.SaveChanges();
            }
            if (ANNEXE_2 != null && ANNEXE_2.ContentLength > 0)
            {
                string path = Path.Combine(Server.MapPath("~/Images/"), ANNEXE_2.FileName);
                ANNEXE_2.SaveAs(path);
                Uploadfile(ANNEXE_2.InputStream, path, "ANNEXE_2");
                D007 = "0";
                byte[] file = SaveAnnexe2(ANNEE, SelectedSociete, CODE_ACTE, SelectedDeclaration);
                SelectedDeclaration.ANNEXE_2_DATA = file;
                BD.SaveChanges();
            }
            if (ANNEXE_3 != null && ANNEXE_3.ContentLength > 0)
            {
                string path = Path.Combine(Server.MapPath("~/Images/"), ANNEXE_3.FileName);
                ANNEXE_3.SaveAs(path);
                Uploadfile(ANNEXE_3.InputStream, path, "ANNEXE_3");
                D008 = "0";
                byte[] file = SaveAnnexe3(ANNEE, SelectedSociete, CODE_ACTE, SelectedDeclaration);
                SelectedDeclaration.ANNEXE_3_DATA = file;
                BD.SaveChanges();
            }
            if (ANNEXE_4 != null && ANNEXE_4.ContentLength > 0)
            {
                string path = Path.Combine(Server.MapPath("~/Images/"), ANNEXE_4.FileName);
                ANNEXE_4.SaveAs(path);
                Uploadfile(ANNEXE_4.InputStream, path, "ANNEXE_4");
                D009 = "0";
                byte[] file = SaveAnnexe4(ANNEE, SelectedSociete, CODE_ACTE, SelectedDeclaration);
                SelectedDeclaration.ANNEXE_4_DATA = file;
                BD.SaveChanges();
            }
            if (ANNEXE_5 != null && ANNEXE_5.ContentLength > 0)
            {
                string path = Path.Combine(Server.MapPath("~/Images/"), ANNEXE_5.FileName);
                ANNEXE_5.SaveAs(path);
                Uploadfile(ANNEXE_5.InputStream, path, "ANNEXE_5");
                D010 = "0";
                byte[] file = SaveAnnexe5(ANNEE, SelectedSociete, CODE_ACTE, SelectedDeclaration);
                SelectedDeclaration.ANNEXE_5_DATA = file;
                BD.SaveChanges();
            }
            if (ANNEXE_6 != null && ANNEXE_6.ContentLength > 0)
            {
                string path = Path.Combine(Server.MapPath("~/Images/"), ANNEXE_6.FileName);
                ANNEXE_6.SaveAs(path);
                Uploadfile(ANNEXE_6.InputStream, path, "ANNEXE_6");
                D011 = "0";
                byte[] file = SaveAnnexe6(ANNEE, SelectedSociete, CODE_ACTE, SelectedDeclaration);
                SelectedDeclaration.ANNEXE_6_DATA = file;
                BD.SaveChanges();
            }
            if (ANNEXE_7 != null && ANNEXE_7.ContentLength > 0)
            {
                string path = Path.Combine(Server.MapPath("~/Images/"), ANNEXE_7.FileName);
                ANNEXE_7.SaveAs(path);
                Uploadfile(ANNEXE_7.InputStream, path, "ANNEXE_7");
                D012 = "0";
                byte[] file = SaveAnnexe7(ANNEE, SelectedSociete, CODE_ACTE, SelectedDeclaration);
                SelectedDeclaration.ANNEXE_7_DATA = file;
                BD.SaveChanges();
            }
            if (RECAP != null && RECAP.ContentLength > 0)
            {
                string path = Path.Combine(Server.MapPath("~/Images/"), RECAP.FileName);
                RECAP.SaveAs(path);
                Uploadfile(RECAP.InputStream, path, "RECAP");
                byte[] file = SaveRecap(ANNEE, SelectedSociete, CODE_ACTE, SelectedDeclaration, D006, D007, D008, D009, D010, D011, D012);
                SelectedDeclaration.DATA = file;
                BD.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public FileResult SendForm()
        {
            HttpPostedFileBase ANNEXE_1 = Request.Files["ANNEXE_1"];
            HttpPostedFileBase ANNEXE_2 = Request.Files["ANNEXE_2"];
            HttpPostedFileBase ANNEXE_3 = Request.Files["ANNEXE_3"];
            HttpPostedFileBase ANNEXE_4 = Request.Files["ANNEXE_4"];
            HttpPostedFileBase ANNEXE_5 = Request.Files["ANNEXE_5"];
            HttpPostedFileBase ANNEXE_6 = Request.Files["ANNEXE_6"];
            HttpPostedFileBase ANNEXE_7 = Request.Files["ANNEXE_7"];
            HttpPostedFileBase RECAP = Request.Files["RECAP"];
            string ANNEE = Request.Params["ANNEE"] != null ? Request.Params["ANNEE"].ToString() : string.Empty;
            string SOCIETE = Request.Params["SOCIETE"] != null ? Request.Params["SOCIETE"].ToString() : string.Empty;
            string CODE_ACTE = Request.Params["CODE_ACTE"] != null ? Request.Params["CODE_ACTE"].ToString() : string.Empty;
            string ACTION = Request.Params["ACTION"] != null ? Request.Params["ACTION"].ToString() : string.Empty;
            int SOCIETE_ID = int.Parse(SOCIETE);
            DECLARATIONS SelectedSociete = BD.DECLARATIONS.Find(SOCIETE_ID);
            string D006 = "1";
            string D007 = "1";
            string D008 = "1";
            string D009 = "1";
            string D010 = "1";
            string D011 = "1";
            string D012 = "1";
            byte[] fileBytes_Annexe1 = null;
            byte[] fileBytes_Annexe2 = null;
            byte[] fileBytes_Annexe3 = null;
            byte[] fileBytes_Annexe4 = null;
            byte[] fileBytes_Annexe5 = null;
            byte[] fileBytes_Annexe6 = null;
            byte[] fileBytes_Annexe7 = null;
            byte[] fileBytes_RECAP = null;
            if (ANNEXE_1 != null && ANNEXE_1.ContentLength > 0)
            {
                string path = Path.Combine(Server.MapPath("~/Images/"), ANNEXE_1.FileName);
                ANNEXE_1.SaveAs(path);
                Uploadfile(ANNEXE_1.InputStream, path, "ANNEXE_1");
                D006 = "0";
            }
            if (ANNEXE_2 != null && ANNEXE_2.ContentLength > 0)
            {
                string path = Path.Combine(Server.MapPath("~/Images/"), ANNEXE_2.FileName);
                ANNEXE_2.SaveAs(path);
                Uploadfile(ANNEXE_2.InputStream, path, "ANNEXE_2");
                D007 = "0";
            }
            if (ANNEXE_3 != null && ANNEXE_3.ContentLength > 0)
            {
                string path = Path.Combine(Server.MapPath("~/Images/"), ANNEXE_3.FileName);
                ANNEXE_3.SaveAs(path);
                Uploadfile(ANNEXE_3.InputStream, path, "ANNEXE_3");
                D008 = "0";
            }
            if (ANNEXE_4 != null && ANNEXE_4.ContentLength > 0)
            {
                string path = Path.Combine(Server.MapPath("~/Images/"), ANNEXE_4.FileName);
                ANNEXE_4.SaveAs(path);
                Uploadfile(ANNEXE_4.InputStream, path, "ANNEXE_4");
                D009 = "0";
            }
            if (ANNEXE_5 != null && ANNEXE_5.ContentLength > 0)
            {
                string path = Path.Combine(Server.MapPath("~/Images/"), ANNEXE_5.FileName);
                ANNEXE_5.SaveAs(path);
                Uploadfile(ANNEXE_5.InputStream, path, "ANNEXE_5");
                D010 = "0";
            }
            if (ANNEXE_6 != null && ANNEXE_6.ContentLength > 0)
            {
                string path = Path.Combine(Server.MapPath("~/Images/"), ANNEXE_6.FileName);
                ANNEXE_6.SaveAs(path);
                Uploadfile(ANNEXE_6.InputStream, path, "ANNEXE_6");
                D011 = "0";
            }
            if (ANNEXE_7 != null && ANNEXE_7.ContentLength > 0)
            {
                string path = Path.Combine(Server.MapPath("~/Images/"), ANNEXE_7.FileName);
                ANNEXE_7.SaveAs(path);
                Uploadfile(ANNEXE_7.InputStream, path, "ANNEXE_7");
                D012 = "0";
            }
            if (RECAP != null && RECAP.ContentLength > 0)
            {
                string path = Path.Combine(Server.MapPath("~/Images/"), RECAP.FileName);
                RECAP.SaveAs(path);
                Uploadfile(RECAP.InputStream, path, "RECAP");
            }
            #region ACTION ANNEXE 1
            if (ACTION == "ANNEXE_1")
            {
                List<ANNEXE_1> ListeAnnexe1 = new List<ANNEXE_1>();
                if (Session["LISTE_ANNEXE_1_" + Session.SessionID] != null)
                {
                    ListeAnnexe1 = (List<ANNEXE_1>)Session["LISTE_ANNEXE_1_" + Session.SessionID];

                }
                FileInfo info = new FileInfo("Fichier vide.TXT");
                string nom = "ANXEMP_1_" + ANNEE.ToString().Substring(2) + "_1";
                var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fileStream.Close();
                using (StreamWriter writer = new StreamWriter(Server.MapPath(@"~/Images/" + nom), false, Encoding.ASCII))
                {
                    string E000 = "E1";//2
                    string E001 = SelectedSociete.MATRICULE;//12
                    string E005 = ANNEE;//4
                    string E006 = "An1";//3
                    string E007 = CODE_ACTE;//1
                    string E008 = ListeAnnexe1.Count.ToString("000000");//6
                    string E009 = SelectedSociete.SOCIETE;//40
                    string E010 = SelectedSociete.ACTIVITE;//40
                    string E011 = SelectedSociete.VILLE;//40
                    string E012 = SelectedSociete.RUE;//72
                    string E013 = SelectedSociete.NUMERO;//4
                    string E014 = SelectedSociete.CODE_POSTAL;//4
                    string E015 = new string(' ', 192);//192
                    while (E009.Length < 40)
                    {
                        E009 = E009 + " ";
                    }
                    while (E010.Length < 40)
                    {
                        E010 = E010 + " ";
                    }
                    while (E011.Length < 40)
                    {
                        E011 = E011 + " ";
                    }
                    while (E012.Length < 72)
                    {
                        E012 = E012 + " ";
                    }
                    while (E013.Length < 4)
                    {
                        E013 = E013 + " ";
                    }
                    while (E014.Length < 4)
                    {
                        E014 = E014 + " ";
                    }
                    string FirstLine = E000 + E001 + E005 + E006 + E007 + E008 + E009 + E010 + E011 + E012 + E013 + E014 + E015;
                    writer.WriteLine(FirstLine);
                    decimal Total_REVENU_IMPOSABLE = 0;
                    decimal Total_AVANTAGE_EN_NATURE = 0;
                    decimal Total_BRUT = 0;
                    decimal Total_MONTANT_REINVESTI = 0;
                    decimal Total_RETENUE_IRPP = 0;
                    decimal Total_RETENUE_20 = 0;
                    decimal Total_REDEVANCE = 0;
                    decimal Total_NET_SERVI = 0;
                    for (int i = 0; i < ListeAnnexe1.Count; i++)
                    {
                        ANNEXE_1 Element = ListeAnnexe1.ElementAt(i);
                        string A100 = "L1";//2
                        string A101 = SelectedSociete.MATRICULE;//12
                        string A105 = ANNEE;//4
                        string A106 = (i + 1).ToString("000000");//6
                        string A107 = Element.TYPE.ToString();//1
                        string A108 = Element.EMPLOYEES.CIN;//13
                        string A109 = Element.EMPLOYEES.FULLNAME;//40
                        string A110 = Element.EMPLOYEES.ACTIVITE;//40
                        string A111 = Element.EMPLOYEES.ADRESSE;//120
                        string A112 = Element.EMPLOYEES.SITUATION_FAMILIALE.ToString();//1
                        string A113 = Element.EMPLOYEES.NOMBRE_ENFANT.ToString("00");//2
                        string A114 = Element.DATE_DEBUT.ToShortDateString().Replace("/", "");//8
                        string A115 = Element.DATE_FIN.ToShortDateString().Replace("/", "");//8
                        string A116 = Element.DUREE.ToString("000");//3
                        string A117 = Element.REVENU_IMPOSABLE.ToString("F3").Replace(",", "");//15
                        string A118 = Element.AVANTAGE_EN_NATURE.ToString("F3").Replace(",", "");//15
                        string A119 = Element.BRUT.ToString("F3").Replace(",", "");//15
                        string A120 = Element.MONTANT_REINVESTI.ToString("F3").Replace(",", "");//15
                        string A121 = Element.RETENUE_IRPP.ToString("F3").Replace(",", "");//15
                        string A122 = Element.RETENUE_20.ToString("F3").Replace(",", "");//15
                        string A123 = Element.REDEVANCE.ToString("F3").Replace(",", "");//15
                        string A124 = Element.NET_SERVI.ToString("F3").Replace(",", "");//15
                        string A125 = new string(' ', 40);//40
                        while (A108.Length < 13)
                        {
                            A108 = A108 + " ";
                        }
                        while (A109.Length < 40)
                        {
                            A109 = A109 + " ";
                        }
                        while (A110.Length < 40)
                        {
                            A110 = A110 + " ";
                        }
                        while (A111.Length < 120)
                        {
                            A111 = A111 + " ";
                        }
                        while (A117.Length < 15)
                        {
                            A117 = "0" + A117;
                        }
                        while (A118.Length < 15)
                        {
                            A118 = "0" + A118;
                        }
                        while (A119.Length < 15)
                        {
                            A119 = "0" + A119;
                        }
                        while (A120.Length < 15)
                        {
                            A120 = "0" + A120;
                        }
                        while (A121.Length < 15)
                        {
                            A121 = "0" + A121;
                        }
                        while (A122.Length < 15)
                        {
                            A122 = "0" + A122;
                        }
                        while (A123.Length < 15)
                        {
                            A123 = "0" + A123;
                        }
                        while (A124.Length < 15)
                        {
                            A124 = "0" + A124;
                        }
                        string SubLine = A100 + A101 + A105 + A106 + A107 + A108 + A109 + A110 + A111 + A112 + A113 + A114 + A115 + A116 + A117 + A118 + A119 + A120 + A121 + A122 + A123 + A124 + A125;
                        writer.WriteLine(SubLine);
                        Total_REVENU_IMPOSABLE += Element.REVENU_IMPOSABLE;
                        Total_AVANTAGE_EN_NATURE += Element.AVANTAGE_EN_NATURE;
                        Total_BRUT += Element.BRUT;
                        Total_MONTANT_REINVESTI += Element.MONTANT_REINVESTI;
                        Total_RETENUE_IRPP += Element.RETENUE_IRPP;
                        Total_RETENUE_20 += Element.RETENUE_20;
                        Total_REDEVANCE += Element.REDEVANCE;
                        Total_NET_SERVI += Element.NET_SERVI;
                    }
                    string T100 = "T1";//2
                    string T101 = SelectedSociete.MATRICULE;//12
                    string T105 = ANNEE;//4
                    string T106 = new string(' ', 242);//242
                    string T107 = Total_REVENU_IMPOSABLE.ToString("F3").Replace(",", "");//15
                    string T108 = Total_AVANTAGE_EN_NATURE.ToString("F3").Replace(",", "");//15
                    string T109 = Total_BRUT.ToString("F3").Replace(",", "");//15
                    string T110 = Total_MONTANT_REINVESTI.ToString("F3").Replace(",", "");//15
                    string T111 = Total_RETENUE_IRPP.ToString("F3").Replace(",", "");//15
                    string T112 = Total_RETENUE_20.ToString("F3").Replace(",", "");//15
                    string T113 = Total_REDEVANCE.ToString("F3").Replace(",", "");//15
                    string T114 = Total_NET_SERVI.ToString("F3").Replace(",", "");//15
                    string T115 = new string(' ', 40);//40
                    while (T107.Length < 15)
                    {
                        T107 = "0" + T107;
                    }
                    while (T108.Length < 15)
                    {
                        T108 = "0" + T108;
                    }
                    while (T109.Length < 15)
                    {
                        T109 = "0" + T109;
                    }
                    while (T110.Length < 15)
                    {
                        T110 = "0" + T110;
                    }
                    while (T111.Length < 15)
                    {
                        T111 = "0" + T111;
                    }
                    while (T112.Length < 15)
                    {
                        T112 = "0" + T112;
                    }
                    while (T113.Length < 15)
                    {
                        T113 = "0" + T113;
                    }
                    while (T114.Length < 15)
                    {
                        T114 = "0" + T114;
                    }
                    string LastLine = T100 + T101 + T105 + T106 + T107 + T108 + T109 + T110 + T111 + T112 + T113 + T114 + T115;
                    writer.WriteLine(LastLine);
                }
                fileBytes_Annexe1 = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Images/" + nom));
                return File(fileBytes_Annexe1, System.Net.Mime.MediaTypeNames.Application.Octet, nom);
            }
            #endregion
            #region ACTION ANNEXE 2
            if (ACTION == "ANNEXE_2")
            {
                List<ANNEXE_2> ListeAnnexe2 = new List<ANNEXE_2>();
                if (Session["LISTE_ANNEXE_2_" + Session.SessionID] != null)
                {
                    ListeAnnexe2 = (List<ANNEXE_2>)Session["LISTE_ANNEXE_2_" + Session.SessionID];
                }
                FileInfo info = new FileInfo("Fichier vide.TXT");
                string nom = "ANXEMP_2_" + ANNEE.ToString().Substring(2) + "_1";
                var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fileStream.Close();
                using (StreamWriter writer = new StreamWriter(Server.MapPath(@"~/Images/" + nom), false, Encoding.ASCII))
                {
                    string E000 = "E2";//2
                    string E001 = SelectedSociete.MATRICULE;//12
                    string E005 = ANNEE;//4
                    string E006 = "An2";//3
                    string E007 = CODE_ACTE;//1
                    string E008 = ListeAnnexe2.Count.ToString("000000");//6
                    string E009 = SelectedSociete.SOCIETE;//40
                    string E010 = SelectedSociete.ACTIVITE;//40
                    string E011 = SelectedSociete.VILLE;//40
                    string E012 = SelectedSociete.RUE;//72
                    string E013 = SelectedSociete.NUMERO;//4
                    string E014 = SelectedSociete.CODE_POSTAL;//4
                    string E015 = new string(' ', 192);//192
                    while (E009.Length < 40)
                    {
                        E009 = E009 + " ";
                    }
                    while (E010.Length < 40)
                    {
                        E010 = E010 + " ";
                    }
                    while (E011.Length < 40)
                    {
                        E011 = E011 + " ";
                    }
                    while (E012.Length < 72)
                    {
                        E012 = E012 + " ";
                    }
                    while (E013.Length < 4)
                    {
                        E013 = E013 + " ";
                    }
                    while (E014.Length < 4)
                    {
                        E014 = E014 + " ";
                    }
                    string FirstLine = E000 + E001 + E005 + E006 + E007 + E008 + E009 + E010 + E011 + E012 + E013 + E014 + E015;
                    writer.WriteLine(FirstLine);
                    decimal Total_HONORAIRE_NON_COMMERCIALE = 0;
                    decimal Total_HONORAIRE_SOCIETE = 0;
                    decimal Total_JETON = 0;
                    decimal Total_REMUNERATION = 0;
                    decimal Total_PLUS_VALUE_IMMOBILIERE = 0;
                    decimal Total_HOTEL = 0;
                    decimal Total_ARTISTES = 0;
                    decimal Total_BUREAU_ETUDE_EXPORTATEUR = 0;
                    decimal Total_AUTRES = 0;
                    decimal Total_RETENUE = 0;
                    decimal Total_REDEVANCE_CGC = 0;
                    decimal Total_NET_SERVI = 0;
                    for (int i = 0; i < ListeAnnexe2.Count; i++)
                    {
                        ANNEXE_2 Element = ListeAnnexe2.ElementAt(i);
                        string A200 = "L2";//2
                        string A201 = SelectedSociete.MATRICULE;//12
                        string A205 = ANNEE;//4
                        string A206 = (i + 1).ToString("000000");//6
                        string A207 = Element.TYPE.ToString();//1
                        string A208 = Element.IDENTIFIANT;//13
                        string A209 = Element.NOM_PRENOM;//40
                        string A210 = Element.ACTIVITE;//40
                        string A211 = Element.ADRESSE;//120
                        string A212 = Element.TYPE_MONTANT.ToString();//1
                        string A213 = Element.HONORAIRE_NON_COMMERCIALE.ToString("F3").Replace(",", "");//15
                        string A214 = Element.HONORAIRE_SOCIETE.ToString("F3").Replace(",", "");//15
                        string A215 = Element.JETON.ToString("F3").Replace(",", "");//15
                        string A216 = Element.REMUNERATION.ToString("F3").Replace(",", "");//15
                        string A217 = Element.PLUS_VALUE_IMMOBILIERE.ToString("F3").Replace(",", "");//15
                        string A218 = Element.HOTEL.ToString("F3").Replace(",", "");//15
                        string A219 = Element.ARTISTES.ToString("F3").Replace(",", "");//15
                        string A220 = Element.BUREAU_ETUDE_EXPORTATEUR.ToString("F3").Replace(",", "");//15
                        string A221 = Element.TYPE_MONTANT_OPERATION_EXPORTATION.ToString();//1
                        string A222 = Element.AUTRES.ToString("F3").Replace(",", "");//15
                        string A223 = Element.RETENUE.ToString("F3").Replace(",", "");//15
                        string A224 = Element.REDEVANCE_CGC.ToString("F3").Replace(",", "");//15
                        string A225 = Element.NET_SERVI.ToString("F3").Replace(",", "");//15
                        while (A208.Length < 13)
                        {
                            A208 = A208 + " ";
                        }
                        while (A209.Length < 40)
                        {
                            A209 = A209 + " ";
                        }
                        while (A210.Length < 40)
                        {
                            A210 = A210 + " ";
                        }
                        while (A211.Length < 120)
                        {
                            A211 = A211 + " ";
                        }
                        while (A213.Length < 15)
                        {
                            A213 = "0" + A213;
                        }
                        while (A214.Length < 15)
                        {
                            A214 = "0" + A214;
                        }
                        while (A215.Length < 15)
                        {
                            A215 = "0" + A215;
                        }
                        while (A216.Length < 15)
                        {
                            A216 = "0" + A216;
                        }
                        while (A217.Length < 15)
                        {
                            A217 = "0" + A217;
                        }
                        while (A218.Length < 15)
                        {
                            A218 = "0" + A218;
                        }
                        while (A219.Length < 15)
                        {
                            A219 = "0" + A219;
                        }
                        while (A220.Length < 15)
                        {
                            A220 = "0" + A220;
                        }
                        while (A222.Length < 15)
                        {
                            A222 = "0" + A222;
                        }
                        while (A223.Length < 15)
                        {
                            A223 = "0" + A223;
                        }
                        while (A224.Length < 15)
                        {
                            A224 = "0" + A224;
                        }
                        while (A225.Length < 15)
                        {
                            A225 = "0" + A225;
                        }
                        string SubLine = A200 + A201 + A205 + A206 + A207 + A208 + A209 + A210 + A211 + A212 + A213 + A214 + A215 + A216 + A217 + A218 + A219 + A220 + A221 + A222 + A223 + A224 + A225;
                        writer.WriteLine(SubLine);
                        Total_HONORAIRE_NON_COMMERCIALE += Element.HONORAIRE_NON_COMMERCIALE;
                        Total_HONORAIRE_SOCIETE += Element.HONORAIRE_SOCIETE;
                        Total_JETON += Element.JETON;
                        Total_REMUNERATION += Element.REMUNERATION;
                        Total_PLUS_VALUE_IMMOBILIERE += Element.PLUS_VALUE_IMMOBILIERE;
                        Total_HOTEL += Element.HOTEL;
                        Total_ARTISTES += Element.ARTISTES;
                        Total_BUREAU_ETUDE_EXPORTATEUR += Element.BUREAU_ETUDE_EXPORTATEUR;
                        Total_AUTRES += Element.AUTRES;
                        Total_RETENUE += Element.RETENUE;
                        Total_REDEVANCE_CGC += Element.REDEVANCE_CGC;
                        Total_NET_SERVI += Element.NET_SERVI;
                    }
                    string T200 = "T2";//2
                    string T201 = SelectedSociete.MATRICULE;//12
                    string T205 = ANNEE;//4
                    string T206 = new string(' ', 221);//221
                    string T207 = Total_HONORAIRE_NON_COMMERCIALE.ToString("F3").Replace(",", "");//15
                    string T208 = Total_HONORAIRE_SOCIETE.ToString("F3").Replace(",", "");//15
                    string T209 = Total_JETON.ToString("F3").Replace(",", "");//15
                    string T210 = Total_REMUNERATION.ToString("F3").Replace(",", "");//15
                    string T211 = Total_PLUS_VALUE_IMMOBILIERE.ToString("F3").Replace(",", "");//15
                    string T212 = Total_HOTEL.ToString("F3").Replace(",", "");//15
                    string T213 = Total_ARTISTES.ToString("F3").Replace(",", "");//15
                    string T214 = Total_BUREAU_ETUDE_EXPORTATEUR.ToString("F3").Replace(",", "");//15
                    string T215 = " ";//1
                    string T216 = Total_AUTRES.ToString("F3").Replace(",", "");//15
                    string T217 = Total_RETENUE.ToString("F3").Replace(",", "");//15
                    string T218 = Total_REDEVANCE_CGC.ToString("F3").Replace(",", "");//15
                    string T219 = Total_NET_SERVI.ToString("F3").Replace(",", "");//15

                    while (T207.Length < 15)
                    {
                        T207 = "0" + T207;
                    }
                    while (T208.Length < 15)
                    {
                        T208 = "0" + T208;
                    }
                    while (T209.Length < 15)
                    {
                        T209 = "0" + T209;
                    }
                    while (T210.Length < 15)
                    {
                        T210 = "0" + T210;
                    }
                    while (T211.Length < 15)
                    {
                        T211 = "0" + T211;
                    }
                    while (T212.Length < 15)
                    {
                        T212 = "0" + T212;
                    }
                    while (T213.Length < 15)
                    {
                        T213 = "0" + T213;
                    }
                    while (T214.Length < 15)
                    {
                        T214 = "0" + T214;
                    }
                    while (T216.Length < 15)
                    {
                        T216 = "0" + T216;
                    }
                    while (T217.Length < 15)
                    {
                        T217 = "0" + T217;
                    }
                    while (T218.Length < 15)
                    {
                        T218 = "0" + T218;
                    }
                    while (T219.Length < 15)
                    {
                        T219 = "0" + T219;
                    }
                    string LastLine = T200 + T201 + T205 + T206 + T207 + T208 + T209 + T210 + T211 + T212 + T213 + T214 + T215 + T216 + T217 + T218 + T219;
                    writer.WriteLine(LastLine);
                }
                fileBytes_Annexe2 = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Images/" + nom));
                return File(fileBytes_Annexe2, System.Net.Mime.MediaTypeNames.Application.Octet, nom);
            }
            #endregion
            #region ACTION ANNEXE 3
            if (ACTION == "ANNEXE_3")
            {
                List<ANNEXE_3> ListeAnnexe3 = new List<ANNEXE_3>();
                if (Session["LISTE_ANNEXE_3_" + Session.SessionID] != null)
                {
                    ListeAnnexe3 = (List<ANNEXE_3>)Session["LISTE_ANNEXE_3_" + Session.SessionID];
                }
                FileInfo info = new FileInfo("Fichier vide.TXT");
                string nom = "ANXEMP_3_" + ANNEE.ToString().Substring(2) + "_1";
                var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fileStream.Close();
                using (StreamWriter writer = new StreamWriter(Server.MapPath(@"~/Images/" + nom), false, Encoding.ASCII))
                {
                    string E000 = "E3";//2
                    string E001 = SelectedSociete.MATRICULE;//12
                    string E005 = ANNEE;//4
                    string E006 = "An3";//3
                    string E007 = CODE_ACTE;//1
                    string E008 = ListeAnnexe3.Count.ToString("000000");//6
                    string E009 = SelectedSociete.SOCIETE;//40
                    string E010 = SelectedSociete.ACTIVITE;//40
                    string E011 = SelectedSociete.VILLE;//40
                    string E012 = SelectedSociete.RUE;//72
                    string E013 = SelectedSociete.NUMERO;//4
                    string E014 = SelectedSociete.CODE_POSTAL;//4
                    string E015 = new string(' ', 192);//192
                    while (E009.Length < 40)
                    {
                        E009 = E009 + " ";
                    }
                    while (E010.Length < 40)
                    {
                        E010 = E010 + " ";
                    }
                    while (E011.Length < 40)
                    {
                        E011 = E011 + " ";
                    }
                    while (E012.Length < 72)
                    {
                        E012 = E012 + " ";
                    }
                    while (E013.Length < 4)
                    {
                        E013 = E013 + " ";
                    }
                    while (E014.Length < 4)
                    {
                        E014 = E014 + " ";
                    }
                    string FirstLine = E000 + E001 + E005 + E006 + E007 + E008 + E009 + E010 + E011 + E012 + E013 + E014 + E015;
                    writer.WriteLine(FirstLine);
                    decimal Total_Interet_CENT = 0;
                    decimal Total_Interet_Capitaux = 0;
                    decimal Total_Interet_Pret = 0;
                    decimal Total_Retenue = 0;
                    decimal Total_Redevance = 0;
                    decimal Total_Net_Servi = 0;
                    for (int i = 0; i < ListeAnnexe3.Count; i++)
                    {
                        ANNEXE_3 Element = ListeAnnexe3.ElementAt(i);
                        string A300 = "L3";//2
                        string A301 = SelectedSociete.MATRICULE;//12
                        string A305 = ANNEE;//4
                        string A306 = (i + 1).ToString("000000");//6
                        string A307 = Element.TYPE.ToString();//1
                        string A308 = Element.IDENTIFIANT;//13
                        string A309 = Element.NOM_PRENOM;//40
                        string A310 = Element.ACTIVITE;//40
                        string A311 = Element.ADRESSE;//120
                        string A312 = Element.INTERET_COMPTES_CENT.ToString("F3").Replace(",", "");//15
                        string A313 = Element.INTERET_CAPITAUX_MOBILIERES.ToString("F3").Replace(",", "");//15
                        string A314 = Element.INTERET_PRETS.ToString("F3").Replace(",", "");//15
                        string A315 = Element.MONTANT_RETENUE.ToString("F3").Replace(",", "");//15
                        string A316 = Element.REDEVANCE_CGC.ToString("F3").Replace(",", "");//15
                        string A317 = Element.NET_SERVI.ToString("F3").Replace(",", "");//15
                        string A318 = new string(' ', 92);//92
                        while (A308.Length < 13)
                        {
                            A308 = A308 + " ";
                        }
                        while (A309.Length < 40)
                        {
                            A309 = A309 + " ";
                        }
                        while (A310.Length < 40)
                        {
                            A310 = A310 + " ";
                        }
                        while (A311.Length < 120)
                        {
                            A311 = A311 + " ";
                        }
                        while (A312.Length < 15)
                        {
                            A312 = "0" + A312;
                        }
                        while (A313.Length < 15)
                        {
                            A313 = "0" + A313;
                        }
                        while (A314.Length < 15)
                        {
                            A314 = "0" + A314;
                        }
                        while (A315.Length < 15)
                        {
                            A315 = "0" + A315;
                        }
                        while (A316.Length < 15)
                        {
                            A316 = "0" + A316;
                        }
                        while (A317.Length < 15)
                        {
                            A317 = "0" + A317;
                        }
                        string SubLine = A300 + A301 + A305 + A306 + A307 + A308 + A309 + A310 + A311 + A312 + A313 + A314 + A315 + A316 + A317 + A318;
                        writer.WriteLine(SubLine);
                        Total_Interet_CENT += Element.INTERET_COMPTES_CENT;
                        Total_Interet_Capitaux += Element.INTERET_CAPITAUX_MOBILIERES;
                        Total_Interet_Pret += Element.INTERET_PRETS;
                        Total_Retenue += Element.MONTANT_RETENUE;
                        Total_Redevance += Element.MONTANT_RETENUE;
                        Total_Net_Servi += Element.NET_SERVI;
                    }
                    string T300 = "T3";//2
                    string T301 = SelectedSociete.MATRICULE;//12
                    string T305 = ANNEE;//4
                    string T306 = new string(' ', 220);//220
                    string T307 = Total_Interet_CENT.ToString("F3").Replace(",", "");//15
                    string T308 = Total_Interet_Capitaux.ToString("F3").Replace(",", "");//15
                    string T309 = Total_Interet_Pret.ToString("F3").Replace(",", "");//15
                    string T310 = Total_Retenue.ToString("F3").Replace(",", "");//15
                    string T311 = Total_Redevance.ToString("F3").Replace(",", "");//15
                    string T312 = Total_Net_Servi.ToString("F3").Replace(",", "");//15
                    string T313 = new string(' ', 92);//92
                    while (T307.Length < 15)
                    {
                        T307 = "0" + T307;
                    }
                    while (T308.Length < 15)
                    {
                        T308 = "0" + T308;
                    }
                    while (T309.Length < 15)
                    {
                        T309 = "0" + T309;
                    }
                    while (T310.Length < 15)
                    {
                        T310 = "0" + T310;
                    }
                    while (T311.Length < 15)
                    {
                        T311 = "0" + T311;
                    }
                    while (T312.Length < 15)
                    {
                        T312 = "0" + T312;
                    }
                    while (T313.Length < 15)
                    {
                        T313 = "0" + T313;
                    }
                    string LastLine = T300 + T301 + T305 + T306 + T307 + T308 + T309 + T310 + T311 + T312 + T313;
                    writer.WriteLine(LastLine);
                }
                fileBytes_Annexe3 = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Images/" + nom));
                return File(fileBytes_Annexe3, System.Net.Mime.MediaTypeNames.Application.Octet, nom);
            }
            #endregion
            #region ACTION ANNEXE 4
            if (ACTION == "ANNEXE_4")
            {
                List<ANNEXE_4> ListeAnnexe4 = new List<ANNEXE_4>();
                if (Session["LISTE_ANNEXE_4_" + Session.SessionID] != null)
                {
                    ListeAnnexe4 = (List<ANNEXE_4>)Session["LISTE_ANNEXE_4_" + Session.SessionID];
                }
                FileInfo info = new FileInfo("Fichier vide.TXT");
                string nom = "ANXEMP_4_" + ANNEE.ToString().Substring(2) + "_1";
                var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fileStream.Close();
                using (StreamWriter writer = new StreamWriter(Server.MapPath(@"~/Images/" + nom), false, Encoding.ASCII))
                {
                    string E000 = "E4";//2
                    string E001 = SelectedSociete.MATRICULE;//12
                    string E005 = ANNEE;//4
                    string E006 = "An4";//3
                    string E007 = CODE_ACTE;//1
                    string E008 = ListeAnnexe4.Count.ToString("000000");//6
                    string E009 = SelectedSociete.SOCIETE;//40
                    string E010 = SelectedSociete.ACTIVITE;//40
                    string E011 = SelectedSociete.VILLE;//40
                    string E012 = SelectedSociete.RUE;//72
                    string E013 = SelectedSociete.NUMERO;//4
                    string E014 = SelectedSociete.CODE_POSTAL;//4
                    string E015 = new string(' ', 192);//192
                    while (E009.Length < 40)
                    {
                        E009 = E009 + " ";
                    }
                    while (E010.Length < 40)
                    {
                        E010 = E010 + " ";
                    }
                    while (E011.Length < 40)
                    {
                        E011 = E011 + " ";
                    }
                    while (E012.Length < 72)
                    {
                        E012 = E012 + " ";
                    }
                    while (E013.Length < 4)
                    {
                        E013 = E013 + " ";
                    }
                    while (E014.Length < 4)
                    {
                        E014 = E014 + " ";
                    }
                    string FirstLine = E000 + E001 + E005 + E006 + E007 + E008 + E009 + E010 + E011 + E012 + E013 + E014 + E015;
                    writer.WriteLine(FirstLine);
                    decimal Total_TAUX_MONTANT_HONORAIRE = 0;
                    decimal Total_MONTANT_BRUT_HONORAIRE = 0;
                    decimal Total_TAUX_HONORAIRE_6_MOIS = 0;
                    decimal Total_MONTANT_HONORAIRE_6_MOIS = 0;
                    decimal Total_TAUX_PLUS_VALUE_IMMOBILIERE = 0;
                    decimal Total_MONTANT_PLUS_VALUE_IMMOBILIERE = 0;
                    decimal Total_TAUX_REVENUES_IMMOBILIERE = 0;
                    decimal Total_MONTANT_REVENUE_IMMOBILIERE = 0;
                    decimal Total_TAUX_CESSION_ACTION = 0;
                    decimal Total_MONTANT_CESSION_ACTION = 0;
                    decimal Total_MONTANT_RETENUE = 0;
                    decimal Total_MONTANT_OP_EXPORTATION = 0;
                    decimal Total_MONTANT_PARADIS_FISCAUX = 0;
                    decimal Total_NET_SERVI = 0;
                    for (int i = 0; i < ListeAnnexe4.Count; i++)
                    {
                        ANNEXE_4 Element = ListeAnnexe4.ElementAt(i);
                        string A400 = "L4";//2
                        string A401 = SelectedSociete.MATRICULE;//12
                        string A405 = ANNEE;//4
                        string A406 = (i + 1).ToString("000000");//6
                        string A407 = Element.TYPE_BENEFICIAIRE.ToString();//1
                        string A408 = Element.IDENTIFIANT;//13
                        string A409 = Element.NOM_PRENOM;//40
                        string A410 = Element.ACTIVITE;//40
                        string A411 = Element.ADRESSE;//120
                        string A412 = Element.TYPE_MONTANT.ToString();//1
                        string A413 = Element.TAUX_MONTANT_HONORAIRE.ToString("F3").Replace(",", "");//5
                        string A414 = Element.MONTANT_BRUT_HONORAIRE.ToString("F3").Replace(",", "");//15
                        string A415 = Element.TAUX_HONORAIRE_6_MOIS.ToString("F3").Replace(",", "");//5
                        string A416 = Element.MONTANT_HONORAIRE_6_MOIS.ToString("F3").Replace(",", "");//15
                        string A417 = Element.TAUX_PLUS_VALUE_IMMOBILIERE.ToString("F3").Replace(",", "");//5
                        string A418 = Element.MONTANT_PLUS_VALUE_IMMOBILIERE.ToString("F3").Replace(",", "");//15
                        string A419 = Element.TAUX_REVENUES_IMMOBILIERE.ToString("F3").Replace(",", "");//5
                        string A420 = Element.MONTANT_REVENUE_IMMOBILIERE.ToString("F3").Replace(",", "");//15
                        string A421 = Element.TAUX_CESSION_ACTION.ToString("F3").Replace(",", "");//5
                        string A422 = Element.MONTANT_CESSION_ACTION.ToString("F3").Replace(",", "");//15
                        string A423 = Element.MONTANT_RETENUE.ToString("F3").Replace(",", "");//15
                        string A424 = Element.TYPE_MONTANT_OP_EXPORTATION.ToString();//1
                        string A425 = Element.MONTANT_OP_EXPORTATION.ToString("F3").Replace(",", "");//15
                        string A426 = Element.MONTANT_PARADIS_FISCAUX.ToString("F3").Replace(",", "");//15
                        string A427 = Element.NET_SERVI.ToString("F3").Replace(",", "");//15
                        string A428 = new string(' ', 20);//20
                        while (A408.Length < 13)
                        {
                            A408 = A408 + " ";
                        }
                        while (A409.Length < 40)
                        {
                            A409 = A409 + " ";
                        }
                        while (A410.Length < 40)
                        {
                            A410 = A410 + " ";
                        }
                        while (A411.Length < 120)
                        {
                            A411 = A411 + " ";
                        }
                        while (A413.Length < 5)
                        {
                            A413 = "0" + A413;
                        }
                        while (A414.Length < 15)
                        {
                            A414 = "0" + A414;
                        }
                        while (A415.Length < 5)
                        {
                            A415 = "0" + A415;
                        }
                        while (A416.Length < 15)
                        {
                            A416 = "0" + A416;
                        }
                        while (A417.Length < 5)
                        {
                            A417 = "0" + A417;
                        }
                        while (A418.Length < 15)
                        {
                            A418 = "0" + A418;
                        }
                        while (A419.Length < 5)
                        {
                            A419 = "0" + A419;
                        }
                        while (A420.Length < 15)
                        {
                            A420 = "0" + A420;
                        }
                        while (A421.Length < 5)
                        {
                            A421 = "0" + A421;
                        }
                        while (A422.Length < 15)
                        {
                            A422 = "0" + A422;
                        }
                        while (A423.Length < 15)
                        {
                            A423 = "0" + A423;
                        }
                        while (A425.Length < 15)
                        {
                            A425 = "0" + A425;
                        }
                        while (A426.Length < 15)
                        {
                            A426 = "0" + A426;
                        }
                        while (A427.Length < 15)
                        {
                            A427 = "0" + A427;
                        }
                        string SubLine = A400 + A401 + A405 + A406 + A407 + A408 + A409 + A410 + A411 + A412 + A413 + A414 + A415 + A416 + A417 + A418 + A419 + A420 + A421 + A422 + A423 + A424 + A425 + A426 + A427 + A428;
                        writer.WriteLine(SubLine);
                        Total_TAUX_MONTANT_HONORAIRE += Element.TAUX_MONTANT_HONORAIRE;
                        Total_MONTANT_BRUT_HONORAIRE += Element.MONTANT_BRUT_HONORAIRE;
                        Total_TAUX_HONORAIRE_6_MOIS += Element.TAUX_HONORAIRE_6_MOIS;
                        Total_MONTANT_HONORAIRE_6_MOIS += Element.MONTANT_HONORAIRE_6_MOIS;
                        Total_TAUX_PLUS_VALUE_IMMOBILIERE += Element.TAUX_PLUS_VALUE_IMMOBILIERE;
                        Total_MONTANT_PLUS_VALUE_IMMOBILIERE += Element.MONTANT_PLUS_VALUE_IMMOBILIERE;
                        Total_TAUX_REVENUES_IMMOBILIERE += Element.TAUX_REVENUES_IMMOBILIERE;
                        Total_MONTANT_REVENUE_IMMOBILIERE += Element.MONTANT_REVENUE_IMMOBILIERE;
                        Total_TAUX_CESSION_ACTION += Element.TAUX_CESSION_ACTION;
                        Total_MONTANT_CESSION_ACTION += Element.MONTANT_CESSION_ACTION;
                        Total_MONTANT_RETENUE += Element.MONTANT_RETENUE;
                        Total_MONTANT_OP_EXPORTATION += Element.MONTANT_OP_EXPORTATION;
                        Total_MONTANT_PARADIS_FISCAUX += Element.MONTANT_PARADIS_FISCAUX;
                        Total_NET_SERVI += Element.NET_SERVI;
                    }
                    string T400 = "T4";//2
                    string T401 = SelectedSociete.MATRICULE;//12
                    string T405 = ANNEE;//4
                    string T406 = new string(' ', 221);//221
                    string T407 = "00000";//5
                    string T408 = Total_MONTANT_BRUT_HONORAIRE.ToString("F3").Replace(",", "");//15
                    string T409 = "00000";//5
                    string T410 = Total_MONTANT_HONORAIRE_6_MOIS.ToString("F3").Replace(",", "");//15
                    string T411 = "00000";//5
                    string T412 = Total_MONTANT_PLUS_VALUE_IMMOBILIERE.ToString("F3").Replace(",", "");//15
                    string T413 = "00000";//5
                    string T414 = Total_MONTANT_REVENUE_IMMOBILIERE.ToString("F3").Replace(",", "");//15
                    string T415 = "00000";//5
                    string T416 = Total_MONTANT_CESSION_ACTION.ToString("F3").Replace(",", "");//15
                    string T417 = Total_MONTANT_RETENUE.ToString("F3").Replace(",", "");//15
                    string T418 = " ";//1
                    string T419 = Total_MONTANT_OP_EXPORTATION.ToString("F3").Replace(",", "");//15
                    string T420 = Total_MONTANT_PARADIS_FISCAUX.ToString("F3").Replace(",", "");//15
                    string T421 = Total_NET_SERVI.ToString("F3").Replace(",", "");//15
                    string T422 = new string(' ', 20);
                    while (T408.Length < 15)
                    {
                        T408 = "0" + T408;
                    }
                    while (T410.Length < 15)
                    {
                        T410 = "0" + T410;
                    }
                    while (T412.Length < 15)
                    {
                        T412 = "0" + T412;
                    }
                    while (T414.Length < 15)
                    {
                        T414 = "0" + T414;
                    }
                    while (T416.Length < 15)
                    {
                        T416 = "0" + T416;
                    }
                    while (T417.Length < 15)
                    {
                        T417 = "0" + T417;
                    }
                    while (T419.Length < 15)
                    {
                        T419 = "0" + T419;
                    }
                    while (T420.Length < 15)
                    {
                        T420 = "0" + T420;
                    }
                    while (T421.Length < 15)
                    {
                        T421 = "0" + T421;
                    }
                    string LastLine = T400 + T401 + T405 + T406 + T407 + T408 + T409 + T410 + T411 + T412 + T413 + T414 + T415 + T416 + T417 + T418 + T419 + T420 + T421 + T422;
                    writer.WriteLine(LastLine);
                }
                fileBytes_Annexe4 = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Images/" + nom));
                return File(fileBytes_Annexe4, System.Net.Mime.MediaTypeNames.Application.Octet, nom);
            }
            #endregion
            #region ACTION ANNEXE 5
            if (ACTION == "ANNEXE_5")
            {
                List<ANNEXE_5> ListeAnnexe5 = new List<ANNEXE_5>();
                if (Session["LISTE_ANNEXE_5_" + Session.SessionID] != null)
                {
                    ListeAnnexe5 = (List<ANNEXE_5>)Session["LISTE_ANNEXE_5_" + Session.SessionID];
                }
                FileInfo info = new FileInfo("Fichier vide.TXT");
                string nom = "ANXEMP_5_" + ANNEE.ToString().Substring(2) + "_1";
                var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fileStream.Close();
                using (StreamWriter writer = new StreamWriter(Server.MapPath(@"~/Images/" + nom), false, Encoding.ASCII))
                {
                    string E000 = "E5";//2
                    string E001 = SelectedSociete.MATRICULE;//12
                    string E005 = ANNEE;//4
                    string E006 = "An5";//3
                    string E007 = CODE_ACTE;//1
                    string E008 = ListeAnnexe5.Count.ToString("000000");//6
                    string E009 = SelectedSociete.SOCIETE;//40
                    string E010 = SelectedSociete.ACTIVITE;//40
                    string E011 = SelectedSociete.VILLE;//40
                    string E012 = SelectedSociete.RUE;//72
                    string E013 = SelectedSociete.NUMERO;//4
                    string E014 = SelectedSociete.CODE_POSTAL;//4
                    string E015 = new string(' ', 192);//192
                    while (E009.Length < 40)
                    {
                        E009 = E009 + " ";
                    }
                    while (E010.Length < 40)
                    {
                        E010 = E010 + " ";
                    }
                    while (E011.Length < 40)
                    {
                        E011 = E011 + " ";
                    }
                    while (E012.Length < 72)
                    {
                        E012 = E012 + " ";
                    }
                    while (E013.Length < 4)
                    {
                        E013 = E013 + " ";
                    }
                    while (E014.Length < 4)
                    {
                        E014 = E014 + " ";
                    }
                    string FirstLine = E000 + E001 + E005 + E006 + E007 + E008 + E009 + E010 + E011 + E012 + E013 + E014 + E015;
                    writer.WriteLine(FirstLine);
                    decimal Total_MONTANT_MARCHES = 0;
                    decimal Total_RETENUE = 0;
                    decimal Total_TVA_ETAT = 0;
                    decimal Total_REVENUE_TVA = 0;
                    decimal Total_MONTANT_ETABLISSEMENTS_PUBLICS = 0;
                    decimal Total_RETENUE_ETABLISSEMENT_PUBLICS = 0;
                    decimal Total_MONTANT_MARCHES_ETRANGES = 0;
                    decimal Total_RETENUE_MARCHES_ETRANGES = 0;
                    decimal Total_REDEVANCE_CGC = 0;
                    decimal Total_NET_SERVI = 0;
                    for (int i = 0; i < ListeAnnexe5.Count; i++)
                    {
                        ANNEXE_5 Element = ListeAnnexe5.ElementAt(i);
                        string A500 = "L5";//2
                        string A501 = SelectedSociete.MATRICULE;//12
                        string A505 = ANNEE;//4
                        string A506 = (i + 1).ToString("000000");//6
                        string A507 = Element.TYPE.ToString();//1
                        string A508 = Element.IDENTIFIANT;//13
                        string A509 = Element.NOM_PRENOM;//40
                        string A510 = Element.ACTIVITE;//40
                        string A511 = Element.ADRESSE;//120
                        string A512 = Element.MONTANT_MARCHES.ToString("F3").Replace(",", "");//15
                        string A513 = Element.RETENUE.ToString("F3").Replace(",", "");//15
                        string A514 = Element.TVA_ETAT.ToString("F3").Replace(",", "");//15
                        string A515 = Element.REVENUE_TVA.ToString("F3").Replace(",", "");//15
                        string A516 = Element.MONTANT_ETABLISSEMENTS_PUBLICS.ToString("F3").Replace(",", "");//15
                        string A517 = Element.RETENUE_ETABLISSEMENT_PUBLICS.ToString("F3").Replace(",", "");//15
                        string A518 = Element.MONTANT_MARCHES_ETRANGES.ToString("F3").Replace(",", "");//15
                        string A519 = Element.RETENUE_MARCHES_ETRANGES.ToString("F3").Replace(",", "");//15
                        string A520 = Element.REDEVANCE_CGC.ToString("F3").Replace(",", "");//15
                        string A521 = Element.NET_SERVI.ToString("F3").Replace(",", "");//15
                        string A522 = new string(' ', 32);//32
                        while (A508.Length < 13)
                        {
                            A508 = A508 + " ";
                        }
                        while (A509.Length < 40)
                        {
                            A509 = A509 + " ";
                        }
                        while (A510.Length < 40)
                        {
                            A510 = A510 + " ";
                        }
                        while (A511.Length < 120)
                        {
                            A511 = A511 + " ";
                        }
                        while (A512.Length < 15)
                        {
                            A512 = "0" + A512;
                        }
                        while (A513.Length < 15)
                        {
                            A513 = "0" + A513;
                        }
                        while (A514.Length < 15)
                        {
                            A514 = "0" + A514;
                        }
                        while (A515.Length < 15)
                        {
                            A515 = "0" + A515;
                        }
                        while (A516.Length < 15)
                        {
                            A516 = "0" + A516;
                        }
                        while (A517.Length < 15)
                        {
                            A517 = "0" + A517;
                        }
                        while (A518.Length < 15)
                        {
                            A518 = "0" + A518;
                        }
                        while (A519.Length < 15)
                        {
                            A519 = "0" + A519;
                        }
                        while (A520.Length < 15)
                        {
                            A520 = "0" + A520;
                        }
                        while (A521.Length < 15)
                        {
                            A521 = "0" + A521;
                        }
                        string SubLine = A500 + A501 + A505 + A506 + A507 + A508 + A509 + A510 + A511 + A512 + A513 + A514 + A515 + A516 + A517 + A518 + A519 + A520 + A521 + A522;
                        writer.WriteLine(SubLine);
                        Total_MONTANT_MARCHES += Element.MONTANT_MARCHES;
                        Total_RETENUE += Element.RETENUE;
                        Total_TVA_ETAT += Element.TVA_ETAT;
                        Total_REVENUE_TVA += Element.REVENUE_TVA;
                        Total_MONTANT_ETABLISSEMENTS_PUBLICS += Element.MONTANT_ETABLISSEMENTS_PUBLICS;
                        Total_RETENUE_ETABLISSEMENT_PUBLICS += Element.RETENUE_ETABLISSEMENT_PUBLICS;
                        Total_MONTANT_MARCHES_ETRANGES += Element.MONTANT_MARCHES_ETRANGES;
                        Total_RETENUE_MARCHES_ETRANGES += Element.RETENUE_MARCHES_ETRANGES;
                        Total_REDEVANCE_CGC += Element.REDEVANCE_CGC;
                        Total_NET_SERVI += Element.NET_SERVI;
                    }
                    string T500 = "T5";//2
                    string T501 = SelectedSociete.MATRICULE;//12
                    string T505 = ANNEE;//4
                    string T506 = new string(' ', 220);//220
                    string T507 = Total_MONTANT_MARCHES.ToString("F3").Replace(",", "");//15
                    string T508 = Total_RETENUE.ToString("F3").Replace(",", "");//15
                    string T509 = Total_TVA_ETAT.ToString("F3").Replace(",", "");//15
                    string T510 = Total_REVENUE_TVA.ToString("F3").Replace(",", "");//15
                    string T511 = Total_MONTANT_ETABLISSEMENTS_PUBLICS.ToString("F3").Replace(",", "");//15
                    string T512 = Total_RETENUE_ETABLISSEMENT_PUBLICS.ToString("F3").Replace(",", "");//15
                    string T513 = Total_MONTANT_MARCHES_ETRANGES.ToString("F3").Replace(",", "");//15
                    string T514 = Total_RETENUE_MARCHES_ETRANGES.ToString("F3").Replace(",", "");//15
                    string T515 = Total_REDEVANCE_CGC.ToString("F3").Replace(",", "");//15
                    string T516 = Total_NET_SERVI.ToString("F3").Replace(",", "");//15
                    string T517 = new string(' ', 32);//32
                    while (T507.Length < 15)
                    {
                        T507 = "0" + T507;
                    }
                    while (T508.Length < 15)
                    {
                        T508 = "0" + T508;
                    }
                    while (T509.Length < 15)
                    {
                        T509 = "0" + T509;
                    }
                    while (T510.Length < 15)
                    {
                        T510 = "0" + T510;
                    }
                    while (T511.Length < 15)
                    {
                        T511 = "0" + T511;
                    }
                    while (T512.Length < 15)
                    {
                        T512 = "0" + T512;
                    }
                    while (T513.Length < 15)
                    {
                        T513 = "0" + T513;
                    }
                    while (T514.Length < 15)
                    {
                        T514 = "0" + T514;
                    }
                    while (T515.Length < 15)
                    {
                        T515 = "0" + T515;
                    }
                    while (T516.Length < 15)
                    {
                        T516 = "0" + T516;
                    }
                    string LastLine = T500 + T501 + T505 + T506 + T507 + T508 + T509 + T510 + T511 + T512 + T513 + T514 + T515 + T516 + T517;
                    writer.WriteLine(LastLine);
                }
                fileBytes_Annexe5 = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Images/" + nom));
                return File(fileBytes_Annexe5, System.Net.Mime.MediaTypeNames.Application.Octet, nom);

            }
            #endregion
            #region ACTION ANNEXE 6
            if (ACTION == "ANNEXE_6")
            {
                List<ANNEXE_6> ListeAnnexe6 = new List<ANNEXE_6>();
                if (Session["LISTE_ANNEXE_6_" + Session.SessionID] != null)
                {
                    ListeAnnexe6 = (List<ANNEXE_6>)Session["LISTE_ANNEXE_6_" + Session.SessionID];
                }
                FileInfo info = new FileInfo("Fichier vide.TXT");
                string nom = "ANXEMP_6_" + ANNEE.ToString().Substring(2) + "_1";
                var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fileStream.Close();
                using (StreamWriter writer = new StreamWriter(Server.MapPath(@"~/Images/" + nom), false, Encoding.ASCII))
                {
                    string E000 = "E6";//2
                    string E001 = SelectedSociete.MATRICULE;//12
                    string E005 = ANNEE;//4
                    string E006 = "An6";//3
                    string E007 = CODE_ACTE;//1
                    string E008 = ListeAnnexe6.Count.ToString("000000");//6
                    string E009 = SelectedSociete.SOCIETE;//40
                    string E010 = SelectedSociete.ACTIVITE;//40
                    string E011 = SelectedSociete.VILLE;//40
                    string E012 = SelectedSociete.RUE;//72
                    string E013 = SelectedSociete.NUMERO;//4
                    string E014 = SelectedSociete.CODE_POSTAL;//4
                    string E015 = new string(' ', 192);//192
                    while (E009.Length < 40)
                    {
                        E009 = E009 + " ";
                    }
                    while (E010.Length < 40)
                    {
                        E010 = E010 + " ";
                    }
                    while (E011.Length < 40)
                    {
                        E011 = E011 + " ";
                    }
                    while (E012.Length < 72)
                    {
                        E012 = E012 + " ";
                    }
                    while (E013.Length < 4)
                    {
                        E013 = E013 + " ";
                    }
                    while (E014.Length < 4)
                    {
                        E014 = E014 + " ";
                    }
                    string FirstLine = E000 + E001 + E005 + E006 + E007 + E008 + E009 + E010 + E011 + E012 + E013 + E014 + E015;
                    writer.WriteLine(FirstLine);
                    decimal Total_Montant_Commerciale = 0;
                    decimal Total_Montant_Vente_PP = 0;
                    decimal Total_Montant_Avance_PP = 0;
                    decimal Total_Montant_Marchandise = 0;
                    for (int i = 0; i < ListeAnnexe6.Count; i++)
                    {
                        ANNEXE_6 Element = ListeAnnexe6.ElementAt(i);
                        string A600 = "L6";//2
                        string A601 = SelectedSociete.MATRICULE;//12
                        string A605 = ANNEE;//4
                        string A606 = (i + 1).ToString("000000");//6
                        string A607 = Element.TYPE_BENEFICIAIRE.ToString();//1
                        string A608 = Element.IDENTIFIANT;//13
                        string A609 = Element.NOM_PRENOM;//40
                        string A610 = Element.ACTIVITE;//40
                        string A611 = Element.ADRESSE;//120
                        string A612 = Element.MONTANT_RISTOURNES.ToString("F3").Replace(",", "");//15
                        string A613 = Element.MONTANT_VENTE_PP.ToString("F3").Replace(",", "");//15
                        string A614 = Element.MONTANT_AVANCE_VENTE_PP.ToString("F3").Replace(",", "");//15
                        string A615 = Element.MONTANT_ESPECES_MARCHANDISES.ToString("F3").Replace(",", "");//15
                        string A616 = new string(' ', 122);//122
                        while (A608.Length < 13)
                        {
                            A608 = A608 + " ";
                        }
                        while (A609.Length < 40)
                        {
                            A609 = A609 + " ";
                        }
                        while (A610.Length < 40)
                        {
                            A610 = A610 + " ";
                        }
                        while (A611.Length < 120)
                        {
                            A611 = A611 + " ";
                        }
                        while (A612.Length < 15)
                        {
                            A612 = "0" + A612;
                        }
                        while (A613.Length < 15)
                        {
                            A613 = "0" + A613;
                        }
                        while (A614.Length < 15)
                        {
                            A614 = "0" + A614;
                        }
                        while (A615.Length < 15)
                        {
                            A615 = "0" + A615;
                        }
                        string SubLine = A600 + A601 + A605 + A606 + A607 + A608 + A609 + A610 + A611 + A612 + A613 + A614 + A615 + A616;
                        writer.WriteLine(SubLine);
                        Total_Montant_Commerciale += Element.MONTANT_RISTOURNES;
                        Total_Montant_Vente_PP += Element.MONTANT_VENTE_PP;
                        Total_Montant_Avance_PP += Element.MONTANT_AVANCE_VENTE_PP;
                        Total_Montant_Marchandise += Element.MONTANT_ESPECES_MARCHANDISES;
                    }
                    string T600 = "T6";//2
                    string T601 = SelectedSociete.MATRICULE;//12
                    string T605 = ANNEE;//4
                    string T606 = new string(' ', 220);//220
                    string T607 = Total_Montant_Commerciale.ToString("F3").Replace(",", "");//15
                    string T608 = Total_Montant_Vente_PP.ToString("F3").Replace(",", "");//15
                    string T609 = Total_Montant_Avance_PP.ToString("F3").Replace(",", "");//15
                    string T610 = Total_Montant_Marchandise.ToString("F3").Replace(",", "");//15
                    string T611 = new string(' ', 122);//122
                    while (T607.Length < 15)
                    {
                        T607 = "0" + T607;
                    }
                    while (T608.Length < 15)
                    {
                        T608 = "0" + T608;
                    }
                    while (T609.Length < 15)
                    {
                        T609 = "0" + T609;
                    }
                    while (T610.Length < 15)
                    {
                        T610 = "0" + T610;
                    }
                    while (T611.Length < 15)
                    {
                        T611 = "0" + T611;
                    }

                    string LastLine = T600 + T601 + T605 + T606 + T607 + T608 + T609 + T610 + T611;
                    writer.WriteLine(LastLine);
                }
                fileBytes_Annexe6 = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Images/" + nom));
                return File(fileBytes_Annexe6, System.Net.Mime.MediaTypeNames.Application.Octet, nom);
            }
            #endregion
            #region ACTION ANNEXE 7
            if (ACTION == "ANNEXE_7")
            {
                List<ANNEXE_7> ListeAnnexe7 = new List<ANNEXE_7>();
                if (Session["LISTE_ANNEXE_7_" + Session.SessionID] != null)
                {
                    ListeAnnexe7 = (List<ANNEXE_7>)Session["LISTE_ANNEXE_7_" + Session.SessionID];
                }
                FileInfo info = new FileInfo("Fichier vide.TXT");
                string nom = "ANXEMP_7_" + ANNEE.ToString().Substring(2) + "_1";
                var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fileStream.Close();
                using (StreamWriter writer = new StreamWriter(Server.MapPath(@"~/Images/" + nom), false, Encoding.ASCII))
                {
                    string E000 = "E7";//2
                    string E001 = SelectedSociete.MATRICULE;//12
                    string E005 = ANNEE;//4
                    string E006 = "An7";//3
                    string E007 = CODE_ACTE;//1
                    string E008 = ListeAnnexe7.Count.ToString("000000");//6
                    string E009 = SelectedSociete.SOCIETE;//40
                    string E010 = SelectedSociete.ACTIVITE;//40
                    string E011 = SelectedSociete.VILLE;//40
                    string E012 = SelectedSociete.RUE;//72
                    string E013 = SelectedSociete.NUMERO;//4
                    string E014 = SelectedSociete.CODE_POSTAL;//4
                    string E015 = new string(' ', 192);//192
                    while (E009.Length < 40)
                    {
                        E009 = E009 + " ";
                    }
                    while (E010.Length < 40)
                    {
                        E010 = E010 + " ";
                    }
                    while (E011.Length < 40)
                    {
                        E011 = E011 + " ";
                    }
                    while (E012.Length < 72)
                    {
                        E012 = E012 + " ";
                    }
                    while (E013.Length < 4)
                    {
                        E013 = E013 + " ";
                    }
                    while (E014.Length < 4)
                    {
                        E014 = E014 + " ";
                    }
                    string FirstLine = E000 + E001 + E005 + E006 + E007 + E008 + E009 + E010 + E011 + E012 + E013 + E014 + E015;
                    writer.WriteLine(FirstLine);
                    decimal Total_Montant = 0;
                    decimal Total_Retenue = 0;
                    decimal Total_Net = 0;
                    for (int i = 0; i < ListeAnnexe7.Count; i++)
                    {
                        ANNEXE_7 Element = ListeAnnexe7.ElementAt(i);
                        string A700 = "L7";//2
                        string A701 = SelectedSociete.MATRICULE;//12
                        string A705 = ANNEE;//4
                        string A706 = (i + 1).ToString("000000");//6
                        string A707 = Element.TYPE_BENEFICIAIRE.ToString();//1
                        string A708 = Element.IDENTIFIANT;//13
                        string A709 = Element.NOM_PRENOM;//40
                        string A710 = Element.ACTIVITE;//40
                        string A711 = Element.ADRESSE;//120
                        string A712 = Element.TYPE_MONTANT.ToString("00");//2
                        string A713 = Element.MONTANT_PAYES.ToString("F3").Replace(",", "");//15
                        string A714 = Element.RETENUE_SOURCE.ToString("F3").Replace(",", "");//15
                        string A715 = Element.NET_SERVI.ToString("F3").Replace(",", "");//15
                        string A716 = new string(' ', 135);//135
                        while (A708.Length < 13)
                        {
                            A708 = A708 + " ";
                        }
                        while (A709.Length < 40)
                        {
                            A709 = A709 + " ";
                        }
                        while (A710.Length < 40)
                        {
                            A710 = A710 + " ";
                        }
                        while (A711.Length < 120)
                        {
                            A711 = A711 + " ";
                        }
                        while (A713.Length < 15)
                        {
                            A713 = "0" + A713;
                        }
                        while (A714.Length < 15)
                        {
                            A714 = "0" + A714;
                        }
                        while (A715.Length < 15)
                        {
                            A715 = "0" + A715;
                        }
                        string SubLine = A700 + A701 + A705 + A706 + A707 + A708 + A709 + A710 + A711 + A712 + A713 + A714 + A715 + A716;
                        writer.WriteLine(SubLine);
                        Total_Montant += Element.MONTANT_PAYES;
                        Total_Retenue += Element.RETENUE_SOURCE;
                        Total_Net += Element.NET_SERVI;
                    }
                    string T700 = "T7";//2
                    string T701 = SelectedSociete.MATRICULE;//12
                    string T705 = ANNEE;//4
                    string T706 = new string(' ', 222);//222
                    string T707 = Total_Montant.ToString("F3").Replace(",", "");//15
                    string T708 = Total_Retenue.ToString("F3").Replace(",", "");//15
                    string T709 = Total_Net.ToString("F3").Replace(",", "");//15
                    string T710 = new string(' ', 135);//135
                    while (T707.Length < 15)
                    {
                        T707 = "0" + T707;
                    }
                    while (T708.Length < 15)
                    {
                        T708 = "0" + T708;
                    }
                    while (T709.Length < 15)
                    {
                        T709 = "0" + T709;
                    }
                    while (T710.Length < 15)
                    {
                        T710 = "0" + T710;
                    }
                    string LastLine = T700 + T701 + T705 + T706 + T707 + T708 + T709 + T710;
                    writer.WriteLine(LastLine);
                }
                fileBytes_Annexe7 = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Images/" + nom));
                return File(fileBytes_Annexe7, System.Net.Mime.MediaTypeNames.Application.Octet, nom);
            }
            #endregion
            #region ACTION RECAP
            if (ACTION == "RECAP")
            {
                RECAPS RECAPS = new RECAPS();
                if (Session["RECAP" + Session.SessionID] != null)
                {
                    RECAPS = (RECAPS)Session["RECAP" + Session.SessionID];
                }
                FileInfo info = new FileInfo("Fichier vide.TXT");
                string nom = "DECEMP_" + ANNEE.ToString().Substring(2);
                var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fileStream.Close();
                using (StreamWriter writer = new StreamWriter(Server.MapPath(@"~/Images/" + nom), false, Encoding.ASCII))
                {
                    #region DECEMP00
                    string D000 = "000";//3
                    string D001 = SelectedSociete.MATRICULE;//12
                    string D005 = ANNEE;//4
                    string D013 = new string(' ', 12);
                    string DECEMP00 = D000 + D001 + D005 + D006 + D007 + D008 + D009 + D010 + D011 + D012 + D013;
                    writer.WriteLine(DECEMP00);
                    #endregion
                    #region DECEMP01
                    string D010_P = "010";//3
                    string D011_P = RECAPS.Montant_Ligne1.ToString("F3").Replace(",", "");//15
                    string D012_P = "00000";//5
                    string D013_P = RECAPS.Retenu_Ligne1.ToString("F3").Replace(",", "");//15
                    while (D011_P.Length < 15)
                    {
                        D011_P = "0" + D011_P;
                    }
                    while (D013_P.Length < 15)
                    {
                        D013_P = "0" + D013_P;
                    }
                    string DECEMP01 = D010_P + D011_P + D012_P + D013_P;
                    writer.WriteLine(DECEMP01);
                    #endregion
                    #region DECEMP02
                    string D020 = "170";//3
                    string D021 = RECAPS.Montant_Ligne2.ToString("F3").Replace(",", "");//15
                    string D022 = "00000";//5
                    string D023 = RECAPS.Retenu_Ligne2.ToString("F3").Replace(",", "");//15
                    while (D021.Length < 15)
                    {
                        D021 = "0" + D021;
                    }
                    while (D023.Length < 15)
                    {
                        D023 = "0" + D023;
                    }
                    string DECEMP02 = D020 + D021 + D022 + D023;
                    writer.WriteLine(DECEMP02);
                    #endregion
                    #region DECEMP03
                    string D030 = "021";//3
                    string D031 = RECAPS.Montant_Ligne3.ToString("F3").Replace(",", "");//15;
                    string D032 = "01500";
                    string D033 = RECAPS.Retenu_Ligne3.ToString("F3").Replace(",", "");//15;
                    while (D031.Length < 15)
                    {
                        D031 = "0" + D031;
                    }
                    while (D033.Length < 15)
                    {
                        D033 = "0" + D033;
                    }
                    string DECEMP03 = D030 + D031 + D032 + D033;
                    writer.WriteLine(DECEMP03);
                    #endregion
                    #region DECEMP04
                    string D040 = "023";//3
                    string D041 = RECAPS.Montant_Ligne4.ToString("F3").Replace(",", "");//15;
                    string D042 = "01500";//5
                    string D043 = RECAPS.Retenu_Ligne4.ToString("F3").Replace(",", ""); ;//15;
                    while (D041.Length < 15)
                    {
                        D041 = "0" + D041;
                    }
                    while (D043.Length < 15)
                    {
                        D043 = "0" + D043;
                    }
                    string DECEMP04 = D040 + D041 + D042 + D043;
                    writer.WriteLine(DECEMP04);
                    #endregion
                    #region DECEMP05
                    string D050 = "025";//3
                    string D051 = RECAPS.Montant_Ligne5.ToString("F3").Replace(",", "");//15;
                    string D052 = "00250";//5
                    string D053 = RECAPS.Retenu_Ligne5.ToString("F3").Replace(",", "");//15;
                    while (D051.Length < 15)
                    {
                        D051 = "0" + D051;
                    }
                    while (D053.Length < 15)
                    {
                        D053 = "0" + D053;
                    }
                    string DECEMP05 = D050 + D051 + D052 + D053;
                    writer.WriteLine(DECEMP05);
                    #endregion
                    #region DECEMP06
                    string D060 = "030";//3
                    string D061 = RECAPS.Montant_Ligne6.ToString("F3").Replace(",", "");//15;
                    string D062 = "00500";//5
                    string D063 = RECAPS.Retenu_Ligne6.ToString("F3").Replace(",", "");//15;
                    while (D061.Length < 15)
                    {
                        D061 = "0" + D061;
                    }
                    while (D063.Length < 15)
                    {
                        D063 = "0" + D063;
                    }
                    string DECEMP06 = D060 + D061 + D062 + D063;
                    writer.WriteLine(DECEMP06);
                    #endregion
                    #region DECEMP07
                    string D070 = "180";//3
                    string D071 = RECAPS.Montant_Ligne7.ToString("F3").Replace(",", "");//15;
                    string D072 = "00250";//5
                    string D073 = RECAPS.Retenu_Ligne7.ToString("F3").Replace(",", "");//15;
                    while (D071.Length < 15)
                    {
                        D071 = "0" + D071;
                    }
                    while (D073.Length < 15)
                    {
                        D073 = "0" + D073;
                    }
                    string DECEMP07 = D070 + D071 + D072 + D073;
                    writer.WriteLine(DECEMP07);
                    #endregion
                    #region DECEMP08
                    string D080 = "040";//3
                    string D081 = RECAPS.Montant_Ligne8.ToString("F3").Replace(",", "");//15;
                    string D082 = "00500";//5
                    string D083 = RECAPS.Retenu_Ligne8.ToString("F3").Replace(",", "");//15;
                    while (D081.Length < 15)
                    {
                        D081 = "0" + D081;
                    }
                    while (D083.Length < 15)
                    {
                        D083 = "0" + D083;
                    }
                    string DECEMP08 = D080 + D081 + D082 + D083;
                    writer.WriteLine(DECEMP08);
                    #endregion
                    #region DECEMP09
                    string D090 = "060";
                    string D091 = RECAPS.Montant_Ligne9.ToString("F3").Replace(",", "");//15;
                    string D092 = "02000";
                    string D093 = RECAPS.Retenu_Ligne9.ToString("F3").Replace(",", "");//15;
                    while (D091.Length < 15)
                    {
                        D091 = "0" + D091;
                    }
                    while (D093.Length < 15)
                    {
                        D093 = "0" + D093;
                    }
                    string DECEMP09 = D090 + D091 + D092 + D093;
                    writer.WriteLine(DECEMP09);
                    #endregion
                    #region DECEMP10
                    string D100 = "071";
                    string D101 = RECAPS.Montant_Ligne10.ToString("F3").Replace(",", "");//15;
                    string D102 = "02000";
                    string D103 = RECAPS.Retenu_Ligne10.ToString("F3").Replace(",", "");//15;
                    while (D101.Length < 15)
                    {
                        D101 = "0" + D101;
                    }
                    while (D103.Length < 15)
                    {
                        D103 = "0" + D103;
                    }
                    string DECEMP10 = D100 + D101 + D102 + D103;
                    writer.WriteLine(DECEMP10);
                    #endregion
                    #region DECEMP11
                    string D110 = "073";
                    string D111 = RECAPS.Montant_Ligne11.ToString("F3").Replace(",", "");//15;
                    string D112 = "02000";
                    string D113 = RECAPS.Retenu_Ligne11.ToString("F3").Replace(",", "");//15;
                    while (D111.Length < 15)
                    {
                        D111 = "0" + D111;
                    }
                    while (D113.Length < 15)
                    {
                        D113 = "0" + D113;
                    }
                    string DECEMP11 = D110 + D111 + D112 + D113;
                    writer.WriteLine(DECEMP11);
                    #endregion
                    #region DECEMP12
                    string D120 = "080";
                    string D121 = RECAPS.Montant_Ligne12.ToString("F3").Replace(",", "");//15;
                    string D122 = "01500";
                    string D123 = RECAPS.Retenu_Ligne12.ToString("F3").Replace(",", "");//15;
                    while (D121.Length < 15)
                    {
                        D121 = "0" + D121;
                    }
                    while (D123.Length < 15)
                    {
                        D123 = "0" + D123;
                    }
                    string DECEMP12 = D120 + D121 + D122 + D123;
                    writer.WriteLine(DECEMP12);
                    #endregion
                    #region DECEMP13
                    string D130 = "241";
                    string D131 = RECAPS.Montant_Ligne13.ToString("F3").Replace(",", "");//15;
                    string D132 = "00500";
                    string D133 = RECAPS.Retenu_Ligne13.ToString("F3").Replace(",", "");//15;
                    while (D131.Length < 15)
                    {
                        D131 = "0" + D131;
                    }
                    while (D133.Length < 15)
                    {
                        D133 = "0" + D133;
                    }
                    string DECEMP13 = D130 + D131 + D132 + D133;
                    writer.WriteLine(DECEMP13);
                    #endregion
                    #region DECEMP14
                    string D140 = "242";
                    string D141 = RECAPS.Montant_Ligne14.ToString("F3").Replace(",", "");//15;
                    string D142 = "00500";
                    string D143 = RECAPS.Retenu_Ligne14.ToString("F3").Replace(",", "");//15;
                    while (D141.Length < 15)
                    {
                        D141 = "0" + D141;
                    }
                    while (D143.Length < 15)
                    {
                        D143 = "0" + D143;
                    }
                    string DECEMP14 = D140 + D141 + D142 + D143;
                    writer.WriteLine(DECEMP14);
                    #endregion
                    #region DECEMP15
                    string D150 = "091";
                    string D151 = RECAPS.Montant_Ligne15.ToString("F3").Replace(",", "");//15;
                    string D152 = "02000";
                    string D153 = RECAPS.Retenu_Ligne15.ToString("F3").Replace(",", "");//15;
                    while (D151.Length < 15)
                    {
                        D151 = "0" + D151;
                    }
                    while (D153.Length < 15)
                    {
                        D153 = "0" + D153;
                    }
                    string DECEMP15 = D150 + D151 + D152 + D153;
                    writer.WriteLine(DECEMP15);
                    #endregion
                    #region DECEMP16
                    string D160 = "093";
                    string D161 = RECAPS.Montant_Ligne16.ToString("F3").Replace(",", "");//15;
                    string D162 = "02000";
                    string D163 = RECAPS.Retenu_Ligne16.ToString("F3").Replace(",", "");//15;
                    while (D161.Length < 15)
                    {
                        D161 = "0" + D161;
                    }
                    while (D163.Length < 15)
                    {
                        D163 = "0" + D163;
                    }
                    string DECEMP16 = D160 + D161 + D162 + D163;
                    writer.WriteLine(DECEMP16);
                    #endregion
                    #region DECEMP17
                    string D170 = "100";
                    string D171 = RECAPS.Montant_Ligne17.ToString("F3").Replace(",", "");//15;
                    string D172 = "01500";
                    string D173 = RECAPS.Retenu_Ligne17.ToString("F3").Replace(",", "");//15;
                    while (D171.Length < 15)
                    {
                        D171 = "0" + D171;
                    }
                    while (D173.Length < 15)
                    {
                        D173 = "0" + D173;
                    }
                    string DECEMP17 = D170 + D171 + D172 + D173;
                    writer.WriteLine(DECEMP17);
                    #endregion
                    #region DECEMP18
                    string D180 = "110";
                    string D181 = RECAPS.Montant_Ligne18.ToString("F3").Replace(",", "");//15;
                    string D182 = "00500";
                    string D183 = RECAPS.Retenu_Ligne18.ToString("F3").Replace(",", "");//15;
                    while (D181.Length < 15)
                    {
                        D181 = "0" + D181;
                    }
                    while (D183.Length < 15)
                    {
                        D183 = "0" + D183;
                    }
                    string DECEMP18 = D180 + D181 + D182 + D183;
                    writer.WriteLine(DECEMP18);
                    #endregion
                    #region DECEMP19
                    string D190 = "121";
                    string D191 = RECAPS.Montant_Ligne19.ToString("F3").Replace(",", "");//15;
                    string D192 = "00250";
                    string D193 = RECAPS.Retenu_Ligne19.ToString("F3").Replace(",", "");//15;
                    while (D191.Length < 15)
                    {
                        D191 = "0" + D191;
                    }
                    while (D193.Length < 15)
                    {
                        D193 = "0" + D193;
                    }
                    string DECEMP19 = D190 + D191 + D192 + D193;
                    writer.WriteLine(DECEMP19);
                    #endregion
                    #region DECEMP20
                    string D200 = "122";
                    string D201 = RECAPS.Montant_Ligne20.ToString("F3").Replace(",", "");//15;
                    string D202 = "00250";
                    string D203 = RECAPS.Retenu_Ligne20.ToString("F3").Replace(",", "");//15;
                    while (D201.Length < 15)
                    {
                        D201 = "0" + D201;
                    }
                    while (D203.Length < 15)
                    {
                        D203 = "0" + D203;
                    }
                    string DECEMP20 = D200 + D201 + D202 + D203;
                    writer.WriteLine(DECEMP20);
                    #endregion
                    #region DECEMP21
                    string D210 = "123";
                    string D211 = RECAPS.Montant_Ligne21.ToString("F3").Replace(",", "");//15;
                    string D212 = "01500";
                    string D213 = RECAPS.Retenu_Ligne21.ToString("F3").Replace(",", "");//15;
                    while (D211.Length < 15)
                    {
                        D211 = "0" + D211;
                    }
                    while (D213.Length < 15)
                    {
                        D213 = "0" + D213;
                    }
                    string DECEMP21 = D210 + D211 + D212 + D213;
                    writer.WriteLine(DECEMP21);
                    #endregion
                    #region DECEMP22
                    string D220 = "131";
                    string D221 = RECAPS.Montant_Ligne22.ToString("F3").Replace(",", "");//15;
                    string D222 = "00050";
                    string D223 = RECAPS.Retenu_Ligne22.ToString("F3").Replace(",", "");//15;
                    while (D221.Length < 15)
                    {
                        D221 = "0" + D221;
                    }
                    while (D223.Length < 15)
                    {
                        D223 = "0" + D223;
                    }
                    string DECEMP22 = D220 + D221 + D222 + D223;
                    writer.WriteLine(DECEMP22);
                    #endregion
                    #region DECEMP23
                    string D230 = "132";
                    string D231 = RECAPS.Montant_Ligne23.ToString("F3").Replace(",", "");//15;
                    string D232 = "00150";
                    string D233 = RECAPS.Retenu_Ligne23.ToString("F3").Replace(",", "");//15;
                    while (D231.Length < 15)
                    {
                        D231 = "0" + D231;
                    }
                    while (D233.Length < 15)
                    {
                        D233 = "0" + D233;
                    }
                    string DECEMP23 = D230 + D231 + D232 + D233;
                    writer.WriteLine(DECEMP23);
                    #endregion
                    #region DECEMP24
                    string D240 = "140";
                    string D241 = RECAPS.Montant_Ligne24.ToString("F3").Replace(",", "");//15;
                    string D242 = "05000";
                    string D243 = RECAPS.Retenu_Ligne24.ToString("F3").Replace(",", "");//15;
                    while (D241.Length < 15)
                    {
                        D241 = "0" + D241;
                    }
                    while (D243.Length < 15)
                    {
                        D243 = "0" + D243;
                    }
                    string DECEMP24 = D240 + D241 + D242 + D243;
                    writer.WriteLine(DECEMP24);
                    #endregion
                    #region DECEMP25
                    string D250 = "150";
                    string D251 = RECAPS.Montant_Ligne25.ToString("F3").Replace(",", "");//15;
                    string D252 = "10000";
                    string D253 = RECAPS.Retenu_Ligne25.ToString("F3").Replace(",", "");//15;
                    while (D251.Length < 15)
                    {
                        D251 = "0" + D251;
                    }
                    while (D253.Length < 15)
                    {
                        D253 = "0" + D253;
                    }
                    string DECEMP25 = D250 + D251 + D252 + D253;
                    writer.WriteLine(DECEMP25);
                    #endregion
                    #region DECEMP26
                    string D260 = "160";
                    string D261 = RECAPS.Montant_Ligne26.ToString("F3").Replace(",", "");//15;
                    string D262 = "00000";
                    string D263 = RECAPS.Retenu_Ligne26.ToString("F3").Replace(",", "");//15;
                    while (D261.Length < 15)
                    {
                        D261 = "0" + D261;
                    }
                    while (D263.Length < 15)
                    {
                        D263 = "0" + D263;
                    }
                    string DECEMP26 = D260 + D261 + D262 + D263;
                    writer.WriteLine(DECEMP26);
                    #endregion
                    #region DECEMP27
                    string D270 = "200";
                    string D271 = RECAPS.Montant_Ligne27.ToString("F3").Replace(",", "");//15;
                    string D272 = "00100";
                    string D273 = RECAPS.Retenu_Ligne27.ToString("F3").Replace(",", "");//15;
                    while (D271.Length < 15)
                    {
                        D271 = "0" + D271;
                    }
                    while (D273.Length < 15)
                    {
                        D273 = "0" + D273;
                    }
                    string DECEMP27 = D270 + D271 + D272 + D273;
                    writer.WriteLine(DECEMP27);
                    #endregion
                    #region DECEMP28
                    string D280 = "191";
                    string D281 = RECAPS.Montant_Ligne28.ToString("F3").Replace(",", "");//15;
                    string D282 = "01000";
                    string D283 = RECAPS.Retenu_Ligne28.ToString("F3").Replace(",", "");//15;
                    while (D281.Length < 15)
                    {
                        D281 = "0" + D281;
                    }
                    while (D283.Length < 15)
                    {
                        D283 = "0" + D283;
                    }
                    string DECEMP28 = D280 + D281 + D282 + D283;
                    writer.WriteLine(DECEMP28);
                    #endregion
                    #region DECEMP29
                    string D290 = "192";
                    string D291 = RECAPS.Montant_Ligne29.ToString("F3").Replace(",", "");//15;
                    string D292 = "02500";
                    string D293 = RECAPS.Retenu_Ligne29.ToString("F3").Replace(",", "");//15;
                    while (D291.Length < 15)
                    {
                        D291 = "0" + D291;
                    }
                    while (D293.Length < 15)
                    {
                        D293 = "0" + D293;
                    }
                    string DECEMP29 = D290 + D291 + D292 + D293;
                    writer.WriteLine(DECEMP29);
                    #endregion
                    #region DECEMP30
                    string D300 = "051";
                    string D301 = RECAPS.Montant_Ligne30.ToString("F3").Replace(",", "");//15;
                    string D302 = "01500";
                    string D303 = RECAPS.Retenu_Ligne30.ToString("F3").Replace(",", "");//15;
                    while (D301.Length < 15)
                    {
                        D301 = "0" + D301;
                    }
                    while (D303.Length < 15)
                    {
                        D303 = "0" + D303;
                    }
                    string DECEMP30 = D300 + D301 + D302 + D303;
                    writer.WriteLine(DECEMP30);
                    #endregion
                    #region DECEMP31
                    string D310 = "210";
                    string D311 = RECAPS.Montant_Ligne31.ToString("F3").Replace(",", "");//15;
                    string D312 = "00100";
                    string D313 = RECAPS.Retenu_Ligne31.ToString("F3").Replace(",", "");//15;
                    while (D311.Length < 15)
                    {
                        D311 = "0" + D311;
                    }
                    while (D313.Length < 15)
                    {
                        D313 = "0" + D313;
                    }
                    string DECEMP31 = D310 + D311 + D312 + D313;
                    writer.WriteLine(DECEMP31);
                    #endregion
                    #region DECEMP32
                    string D320 = "220";
                    string D321 = RECAPS.Montant_Ligne32.ToString("F3").Replace(",", "");//15;
                    string D322 = "02500";
                    string D323 = RECAPS.Retenu_Ligne32.ToString("F3").Replace(",", "");//15;
                    while (D321.Length < 15)
                    {
                        D321 = "0" + D321;
                    }
                    while (D323.Length < 15)
                    {
                        D323 = "0" + D323;
                    }
                    string DECEMP32 = D320 + D321 + D322 + D323;
                    writer.WriteLine(DECEMP32);
                    #endregion
                    #region DECEMP33
                    string D330 = "250";
                    string D331 = RECAPS.Montant_Ligne33.ToString("F3").Replace(",", "");//15;
                    string D332 = "00150";
                    string D333 = RECAPS.Retenu_Ligne33.ToString("F3").Replace(",", "");//15;
                    while (D331.Length < 15)
                    {
                        D331 = "0" + D331;
                    }
                    while (D333.Length < 15)
                    {
                        D333 = "0" + D333;
                    }
                    string DECEMP33 = D330 + D331 + D332 + D333;
                    writer.WriteLine(DECEMP33);
                    #endregion
                    #region DECEMP34
                    string D340 = "999";
                    string D341 = "00000000000000000000";
                    decimal TOTALE = RECAPS.Retenu_Ligne1 + RECAPS.Retenu_Ligne2 + RECAPS.Retenu_Ligne3 + RECAPS.Retenu_Ligne4 + RECAPS.Retenu_Ligne5 + RECAPS.Retenu_Ligne6 + RECAPS.Retenu_Ligne7 + RECAPS.Retenu_Ligne8 + RECAPS.Retenu_Ligne9 + RECAPS.Retenu_Ligne10 +
                        RECAPS.Retenu_Ligne11 + RECAPS.Retenu_Ligne12 + RECAPS.Retenu_Ligne13 + RECAPS.Retenu_Ligne14 + RECAPS.Retenu_Ligne15 + RECAPS.Retenu_Ligne16 + RECAPS.Retenu_Ligne17 + RECAPS.Retenu_Ligne18 + RECAPS.Retenu_Ligne19 + RECAPS.Retenu_Ligne20 +
                         RECAPS.Retenu_Ligne21 + RECAPS.Retenu_Ligne22 + RECAPS.Retenu_Ligne23 + RECAPS.Retenu_Ligne24 + RECAPS.Retenu_Ligne25 + RECAPS.Retenu_Ligne26 + RECAPS.Retenu_Ligne27 + RECAPS.Retenu_Ligne28 + RECAPS.Retenu_Ligne29 + RECAPS.Retenu_Ligne30 +
                        RECAPS.Retenu_Ligne31 + RECAPS.Retenu_Ligne32 + RECAPS.Retenu_Ligne33;
                    string D342 = TOTALE.ToString("F3").Replace(",", "");
                    while (D342.Length < 15)
                    {
                        D342 = "0" + D342;
                    }
                    //15;
                    string DECEMP34 = D340 + D341 + D342;
                    writer.WriteLine(DECEMP34);
                    #endregion
                }
                fileBytes_RECAP = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Images/" + nom));
                return File(fileBytes_RECAP, System.Net.Mime.MediaTypeNames.Application.Octet, nom);

            }
            #endregion
            return null;
        }
        public int AddNew(string ANNEE, DECLARATIONS SelectedSociete, string CODE_ACTE)
        {
            DECLARATIONS_EMPLOYEURS NouvelleDeclaration = new DECLARATIONS_EMPLOYEURS();
            NouvelleDeclaration.ANNEE = int.Parse(ANNEE);
            NouvelleDeclaration.SOCIETE = SelectedSociete.ID;
            NouvelleDeclaration.CODE = "DECEMP_" + ANNEE.ToString().Substring(2);
            NouvelleDeclaration.VALIDE = true;
            NouvelleDeclaration.CODE_ACTE = int.Parse(CODE_ACTE);
            NouvelleDeclaration.DECLARATIONS = SelectedSociete;
            //NouvelleDeclaration.DATA = DATA;
            NouvelleDeclaration.DATE = DateTime.Today;
            BD.DECLARATIONS_EMPLOYEURS.Add(NouvelleDeclaration);
            BD.SaveChanges();
            return NouvelleDeclaration.ID;
        }
        public byte[] SaveRecap(string ANNEE, DECLARATIONS SelectedSociete, string CODE_ACTE, DECLARATIONS_EMPLOYEURS SelectedDeclaration, string D006, string D007, string D008, string D009, string D010, string D011, string D012)
        {
            RECAPS RECAPS = new RECAPS();
            if (Session["RECAP" + Session.SessionID] != null)
            {
                RECAPS = (RECAPS)Session["RECAP" + Session.SessionID];
            }
            FileInfo info = new FileInfo("Fichier vide.TXT");
            string nom = "DECEMP_" + ANNEE.ToString().Substring(2);
            var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fileStream.Close();
            using (StreamWriter writer = new StreamWriter(Server.MapPath(@"~/Images/" + nom), false, Encoding.ASCII))
            {
                #region DECEMP00
                string D000 = "000";//3
                string D001 = SelectedSociete.MATRICULE;//12
                string D005 = ANNEE;//4
                string D013 = new string(' ', 12);
                string DECEMP00 = D000 + D001 + D005 + D006 + D007 + D008 + D009 + D010 + D011 + D012 + D013;
                writer.WriteLine(DECEMP00);
                #endregion
                #region DECEMP01
                string D010_P = "010";//3
                string D011_P = RECAPS.Montant_Ligne1.ToString("F3").Replace(",", "");//15
                string D012_P = "00000";//5
                string D013_P = RECAPS.Retenu_Ligne1.ToString("F3").Replace(",", "");//15
                while (D011_P.Length < 15)
                {
                    D011_P = "0" + D011_P;
                }
                while (D013_P.Length < 15)
                {
                    D013_P = "0" + D013_P;
                }
                string DECEMP01 = D010_P + D011_P + D012_P + D013_P;
                writer.WriteLine(DECEMP01);
                #endregion
                #region DECEMP02
                string D020 = "170";//3
                string D021 = RECAPS.Montant_Ligne2.ToString("F3").Replace(",", "");//15
                string D022 = "00000";//5
                string D023 = RECAPS.Retenu_Ligne2.ToString("F3").Replace(",", "");//15
                while (D021.Length < 15)
                {
                    D021 = "0" + D021;
                }
                while (D023.Length < 15)
                {
                    D023 = "0" + D023;
                }
                string DECEMP02 = D020 + D021 + D022 + D023;
                writer.WriteLine(DECEMP02);
                #endregion
                #region DECEMP03
                string D030 = "021";//3
                string D031 = RECAPS.Montant_Ligne3.ToString("F3").Replace(",", "");//15;
                string D032 = "01500";
                string D033 = RECAPS.Retenu_Ligne3.ToString("F3").Replace(",", "");//15;
                while (D031.Length < 15)
                {
                    D031 = "0" + D031;
                }
                while (D033.Length < 15)
                {
                    D033 = "0" + D033;
                }
                string DECEMP03 = D030 + D031 + D032 + D033;
                writer.WriteLine(DECEMP03);
                #endregion
                #region DECEMP04
                string D040 = "023";//3
                string D041 = RECAPS.Montant_Ligne4.ToString("F3").Replace(",", "");//15;
                string D042 = "01500";//5
                string D043 = RECAPS.Retenu_Ligne4.ToString("F3").Replace(",", ""); ;//15;
                while (D041.Length < 15)
                {
                    D041 = "0" + D041;
                }
                while (D043.Length < 15)
                {
                    D043 = "0" + D043;
                }
                string DECEMP04 = D040 + D041 + D042 + D043;
                writer.WriteLine(DECEMP04);
                #endregion
                #region DECEMP05
                string D050 = "025";//3
                string D051 = RECAPS.Montant_Ligne5.ToString("F3").Replace(",", "");//15;
                string D052 = "00250";//5
                string D053 = RECAPS.Retenu_Ligne5.ToString("F3").Replace(",", "");//15;
                while (D051.Length < 15)
                {
                    D051 = "0" + D051;
                }
                while (D053.Length < 15)
                {
                    D053 = "0" + D053;
                }
                string DECEMP05 = D050 + D051 + D052 + D053;
                writer.WriteLine(DECEMP05);
                #endregion
                #region DECEMP06
                string D060 = "030";//3
                string D061 = RECAPS.Montant_Ligne6.ToString("F3").Replace(",", "");//15;
                string D062 = "00500";//5
                string D063 = RECAPS.Retenu_Ligne6.ToString("F3").Replace(",", "");//15;
                while (D061.Length < 15)
                {
                    D061 = "0" + D061;
                }
                while (D063.Length < 15)
                {
                    D063 = "0" + D063;
                }
                string DECEMP06 = D060 + D061 + D062 + D063;
                writer.WriteLine(DECEMP06);
                #endregion
                #region DECEMP07
                string D070 = "180";//3
                string D071 = RECAPS.Montant_Ligne7.ToString("F3").Replace(",", "");//15;
                string D072 = "00250";//5
                string D073 = RECAPS.Retenu_Ligne7.ToString("F3").Replace(",", "");//15;
                while (D071.Length < 15)
                {
                    D071 = "0" + D071;
                }
                while (D073.Length < 15)
                {
                    D073 = "0" + D073;
                }
                string DECEMP07 = D070 + D071 + D072 + D073;
                writer.WriteLine(DECEMP07);
                #endregion
                #region DECEMP08
                string D080 = "040";//3
                string D081 = RECAPS.Montant_Ligne8.ToString("F3").Replace(",", "");//15;
                string D082 = "00500";//5
                string D083 = RECAPS.Retenu_Ligne8.ToString("F3").Replace(",", "");//15;
                while (D081.Length < 15)
                {
                    D081 = "0" + D081;
                }
                while (D083.Length < 15)
                {
                    D083 = "0" + D083;
                }
                string DECEMP08 = D080 + D081 + D082 + D083;
                writer.WriteLine(DECEMP08);
                #endregion
                #region DECEMP09
                string D090 = "060";
                string D091 = RECAPS.Montant_Ligne9.ToString("F3").Replace(",", "");//15;
                string D092 = "02000";
                string D093 = RECAPS.Retenu_Ligne9.ToString("F3").Replace(",", "");//15;
                while (D091.Length < 15)
                {
                    D091 = "0" + D091;
                }
                while (D093.Length < 15)
                {
                    D093 = "0" + D093;
                }
                string DECEMP09 = D090 + D091 + D092 + D093;
                writer.WriteLine(DECEMP09);
                #endregion
                #region DECEMP10
                string D100 = "071";
                string D101 = RECAPS.Montant_Ligne10.ToString("F3").Replace(",", "");//15;
                string D102 = "02000";
                string D103 = RECAPS.Retenu_Ligne10.ToString("F3").Replace(",", "");//15;
                while (D101.Length < 15)
                {
                    D101 = "0" + D101;
                }
                while (D103.Length < 15)
                {
                    D103 = "0" + D103;
                }
                string DECEMP10 = D100 + D101 + D102 + D103;
                writer.WriteLine(DECEMP10);
                #endregion
                #region DECEMP11
                string D110 = "073";
                string D111 = RECAPS.Montant_Ligne11.ToString("F3").Replace(",", "");//15;
                string D112 = "02000";
                string D113 = RECAPS.Retenu_Ligne11.ToString("F3").Replace(",", "");//15;
                while (D111.Length < 15)
                {
                    D111 = "0" + D111;
                }
                while (D113.Length < 15)
                {
                    D113 = "0" + D113;
                }
                string DECEMP11 = D110 + D111 + D112 + D113;
                writer.WriteLine(DECEMP11);
                #endregion
                #region DECEMP12
                string D120 = "080";
                string D121 = RECAPS.Montant_Ligne12.ToString("F3").Replace(",", "");//15;
                string D122 = "01500";
                string D123 = RECAPS.Retenu_Ligne12.ToString("F3").Replace(",", "");//15;
                while (D121.Length < 15)
                {
                    D121 = "0" + D121;
                }
                while (D123.Length < 15)
                {
                    D123 = "0" + D123;
                }
                string DECEMP12 = D120 + D121 + D122 + D123;
                writer.WriteLine(DECEMP12);
                #endregion
                #region DECEMP13
                string D130 = "241";
                string D131 = RECAPS.Montant_Ligne13.ToString("F3").Replace(",", "");//15;
                string D132 = "00500";
                string D133 = RECAPS.Retenu_Ligne13.ToString("F3").Replace(",", "");//15;
                while (D131.Length < 15)
                {
                    D131 = "0" + D131;
                }
                while (D133.Length < 15)
                {
                    D133 = "0" + D133;
                }
                string DECEMP13 = D130 + D131 + D132 + D133;
                writer.WriteLine(DECEMP13);
                #endregion
                #region DECEMP14
                string D140 = "242";
                string D141 = RECAPS.Montant_Ligne14.ToString("F3").Replace(",", "");//15;
                string D142 = "00500";
                string D143 = RECAPS.Retenu_Ligne14.ToString("F3").Replace(",", "");//15;
                while (D141.Length < 15)
                {
                    D141 = "0" + D141;
                }
                while (D143.Length < 15)
                {
                    D143 = "0" + D143;
                }
                string DECEMP14 = D140 + D141 + D142 + D143;
                writer.WriteLine(DECEMP14);
                #endregion
                #region DECEMP15
                string D150 = "091";
                string D151 = RECAPS.Montant_Ligne15.ToString("F3").Replace(",", "");//15;
                string D152 = "02000";
                string D153 = RECAPS.Retenu_Ligne15.ToString("F3").Replace(",", "");//15;
                while (D151.Length < 15)
                {
                    D151 = "0" + D151;
                }
                while (D153.Length < 15)
                {
                    D153 = "0" + D153;
                }
                string DECEMP15 = D150 + D151 + D152 + D153;
                writer.WriteLine(DECEMP15);
                #endregion
                #region DECEMP16
                string D160 = "093";
                string D161 = RECAPS.Montant_Ligne16.ToString("F3").Replace(",", "");//15;
                string D162 = "02000";
                string D163 = RECAPS.Retenu_Ligne16.ToString("F3").Replace(",", "");//15;
                while (D161.Length < 15)
                {
                    D161 = "0" + D161;
                }
                while (D163.Length < 15)
                {
                    D163 = "0" + D163;
                }
                string DECEMP16 = D160 + D161 + D162 + D163;
                writer.WriteLine(DECEMP16);
                #endregion
                #region DECEMP17
                string D170 = "100";
                string D171 = RECAPS.Montant_Ligne17.ToString("F3").Replace(",", "");//15;
                string D172 = "01500";
                string D173 = RECAPS.Retenu_Ligne17.ToString("F3").Replace(",", "");//15;
                while (D171.Length < 15)
                {
                    D171 = "0" + D171;
                }
                while (D173.Length < 15)
                {
                    D173 = "0" + D173;
                }
                string DECEMP17 = D170 + D171 + D172 + D173;
                writer.WriteLine(DECEMP17);
                #endregion
                #region DECEMP18
                string D180 = "110";
                string D181 = RECAPS.Montant_Ligne18.ToString("F3").Replace(",", "");//15;
                string D182 = "00500";
                string D183 = RECAPS.Retenu_Ligne18.ToString("F3").Replace(",", "");//15;
                while (D181.Length < 15)
                {
                    D181 = "0" + D181;
                }
                while (D183.Length < 15)
                {
                    D183 = "0" + D183;
                }
                string DECEMP18 = D180 + D181 + D182 + D183;
                writer.WriteLine(DECEMP18);
                #endregion
                #region DECEMP19
                string D190 = "121";
                string D191 = RECAPS.Montant_Ligne19.ToString("F3").Replace(",", "");//15;
                string D192 = "00250";
                string D193 = RECAPS.Retenu_Ligne19.ToString("F3").Replace(",", "");//15;
                while (D191.Length < 15)
                {
                    D191 = "0" + D191;
                }
                while (D193.Length < 15)
                {
                    D193 = "0" + D193;
                }
                string DECEMP19 = D190 + D191 + D192 + D193;
                writer.WriteLine(DECEMP19);
                #endregion
                #region DECEMP20
                string D200 = "122";
                string D201 = RECAPS.Montant_Ligne20.ToString("F3").Replace(",", "");//15;
                string D202 = "00250";
                string D203 = RECAPS.Retenu_Ligne20.ToString("F3").Replace(",", "");//15;
                while (D201.Length < 15)
                {
                    D201 = "0" + D201;
                }
                while (D203.Length < 15)
                {
                    D203 = "0" + D203;
                }
                string DECEMP20 = D200 + D201 + D202 + D203;
                writer.WriteLine(DECEMP20);
                #endregion
                #region DECEMP21
                string D210 = "123";
                string D211 = RECAPS.Montant_Ligne21.ToString("F3").Replace(",", "");//15;
                string D212 = "01500";
                string D213 = RECAPS.Retenu_Ligne21.ToString("F3").Replace(",", "");//15;
                while (D211.Length < 15)
                {
                    D211 = "0" + D211;
                }
                while (D213.Length < 15)
                {
                    D213 = "0" + D213;
                }
                string DECEMP21 = D210 + D211 + D212 + D213;
                writer.WriteLine(DECEMP21);
                #endregion
                #region DECEMP22
                string D220 = "131";
                string D221 = RECAPS.Montant_Ligne22.ToString("F3").Replace(",", "");//15;
                string D222 = "00050";
                string D223 = RECAPS.Retenu_Ligne22.ToString("F3").Replace(",", "");//15;
                while (D221.Length < 15)
                {
                    D221 = "0" + D221;
                }
                while (D223.Length < 15)
                {
                    D223 = "0" + D223;
                }
                string DECEMP22 = D220 + D221 + D222 + D223;
                writer.WriteLine(DECEMP22);
                #endregion
                #region DECEMP23
                string D230 = "132";
                string D231 = RECAPS.Montant_Ligne23.ToString("F3").Replace(",", "");//15;
                string D232 = "00150";
                string D233 = RECAPS.Retenu_Ligne23.ToString("F3").Replace(",", "");//15;
                while (D231.Length < 15)
                {
                    D231 = "0" + D231;
                }
                while (D233.Length < 15)
                {
                    D233 = "0" + D233;
                }
                string DECEMP23 = D230 + D231 + D232 + D233;
                writer.WriteLine(DECEMP23);
                #endregion
                #region DECEMP24
                string D240 = "140";
                string D241 = RECAPS.Montant_Ligne24.ToString("F3").Replace(",", "");//15;
                string D242 = "05000";
                string D243 = RECAPS.Retenu_Ligne24.ToString("F3").Replace(",", "");//15;
                while (D241.Length < 15)
                {
                    D241 = "0" + D241;
                }
                while (D243.Length < 15)
                {
                    D243 = "0" + D243;
                }
                string DECEMP24 = D240 + D241 + D242 + D243;
                writer.WriteLine(DECEMP24);
                #endregion
                #region DECEMP25
                string D250 = "150";
                string D251 = RECAPS.Montant_Ligne25.ToString("F3").Replace(",", "");//15;
                string D252 = "10000";
                string D253 = RECAPS.Retenu_Ligne25.ToString("F3").Replace(",", "");//15;
                while (D251.Length < 15)
                {
                    D251 = "0" + D251;
                }
                while (D253.Length < 15)
                {
                    D253 = "0" + D253;
                }
                string DECEMP25 = D250 + D251 + D252 + D253;
                writer.WriteLine(DECEMP25);
                #endregion
                #region DECEMP26
                string D260 = "160";
                string D261 = RECAPS.Montant_Ligne26.ToString("F3").Replace(",", "");//15;
                string D262 = "00000";
                string D263 = RECAPS.Retenu_Ligne26.ToString("F3").Replace(",", "");//15;
                while (D261.Length < 15)
                {
                    D261 = "0" + D261;
                }
                while (D263.Length < 15)
                {
                    D263 = "0" + D263;
                }
                string DECEMP26 = D260 + D261 + D262 + D263;
                writer.WriteLine(DECEMP26);
                #endregion
                #region DECEMP27
                string D270 = "200";
                string D271 = RECAPS.Montant_Ligne27.ToString("F3").Replace(",", "");//15;
                string D272 = "00100";
                string D273 = RECAPS.Retenu_Ligne27.ToString("F3").Replace(",", "");//15;
                while (D271.Length < 15)
                {
                    D271 = "0" + D271;
                }
                while (D273.Length < 15)
                {
                    D273 = "0" + D273;
                }
                string DECEMP27 = D270 + D271 + D272 + D273;
                writer.WriteLine(DECEMP27);
                #endregion
                #region DECEMP28
                string D280 = "191";
                string D281 = RECAPS.Montant_Ligne28.ToString("F3").Replace(",", "");//15;
                string D282 = "01000";
                string D283 = RECAPS.Retenu_Ligne28.ToString("F3").Replace(",", "");//15;
                while (D281.Length < 15)
                {
                    D281 = "0" + D281;
                }
                while (D283.Length < 15)
                {
                    D283 = "0" + D283;
                }
                string DECEMP28 = D280 + D281 + D282 + D283;
                writer.WriteLine(DECEMP28);
                #endregion
                #region DECEMP29
                string D290 = "192";
                string D291 = RECAPS.Montant_Ligne29.ToString("F3").Replace(",", "");//15;
                string D292 = "02500";
                string D293 = RECAPS.Retenu_Ligne29.ToString("F3").Replace(",", "");//15;
                while (D291.Length < 15)
                {
                    D291 = "0" + D291;
                }
                while (D293.Length < 15)
                {
                    D293 = "0" + D293;
                }
                string DECEMP29 = D290 + D291 + D292 + D293;
                writer.WriteLine(DECEMP29);
                #endregion
                #region DECEMP30
                string D300 = "051";
                string D301 = RECAPS.Montant_Ligne30.ToString("F3").Replace(",", "");//15;
                string D302 = "01500";
                string D303 = RECAPS.Retenu_Ligne30.ToString("F3").Replace(",", "");//15;
                while (D301.Length < 15)
                {
                    D301 = "0" + D301;
                }
                while (D303.Length < 15)
                {
                    D303 = "0" + D303;
                }
                string DECEMP30 = D300 + D301 + D302 + D303;
                writer.WriteLine(DECEMP30);
                #endregion
                #region DECEMP31
                string D310 = "210";
                string D311 = RECAPS.Montant_Ligne31.ToString("F3").Replace(",", "");//15;
                string D312 = "00100";
                string D313 = RECAPS.Retenu_Ligne31.ToString("F3").Replace(",", "");//15;
                while (D311.Length < 15)
                {
                    D311 = "0" + D311;
                }
                while (D313.Length < 15)
                {
                    D313 = "0" + D313;
                }
                string DECEMP31 = D310 + D311 + D312 + D313;
                writer.WriteLine(DECEMP31);
                #endregion
                #region DECEMP32
                string D320 = "220";
                string D321 = RECAPS.Montant_Ligne32.ToString("F3").Replace(",", "");//15;
                string D322 = "02500";
                string D323 = RECAPS.Retenu_Ligne32.ToString("F3").Replace(",", "");//15;
                while (D321.Length < 15)
                {
                    D321 = "0" + D321;
                }
                while (D323.Length < 15)
                {
                    D323 = "0" + D323;
                }
                string DECEMP32 = D320 + D321 + D322 + D323;
                writer.WriteLine(DECEMP32);
                #endregion
                #region DECEMP33
                string D330 = "250";
                string D331 = RECAPS.Montant_Ligne33.ToString("F3").Replace(",", "");//15;
                string D332 = "00150";
                string D333 = RECAPS.Retenu_Ligne33.ToString("F3").Replace(",", "");//15;
                while (D331.Length < 15)
                {
                    D331 = "0" + D331;
                }
                while (D333.Length < 15)
                {
                    D333 = "0" + D333;
                }
                string DECEMP33 = D330 + D331 + D332 + D333;
                writer.WriteLine(DECEMP33);
                #endregion
                #region DECEMP34
                string D340 = "999";
                string D341 = "00000000000000000000";
                decimal TOTALE = RECAPS.Retenu_Ligne1 + RECAPS.Retenu_Ligne2 + RECAPS.Retenu_Ligne3 + RECAPS.Retenu_Ligne4 + RECAPS.Retenu_Ligne5 + RECAPS.Retenu_Ligne6 + RECAPS.Retenu_Ligne7 + RECAPS.Retenu_Ligne8 + RECAPS.Retenu_Ligne9 + RECAPS.Retenu_Ligne10 +
                    RECAPS.Retenu_Ligne11 + RECAPS.Retenu_Ligne12 + RECAPS.Retenu_Ligne13 + RECAPS.Retenu_Ligne14 + RECAPS.Retenu_Ligne15 + RECAPS.Retenu_Ligne16 + RECAPS.Retenu_Ligne17 + RECAPS.Retenu_Ligne18 + RECAPS.Retenu_Ligne19 + RECAPS.Retenu_Ligne20 +
                     RECAPS.Retenu_Ligne21 + RECAPS.Retenu_Ligne22 + RECAPS.Retenu_Ligne23 + RECAPS.Retenu_Ligne24 + RECAPS.Retenu_Ligne25 + RECAPS.Retenu_Ligne26 + RECAPS.Retenu_Ligne27 + RECAPS.Retenu_Ligne28 + RECAPS.Retenu_Ligne29 + RECAPS.Retenu_Ligne30 +
                    RECAPS.Retenu_Ligne31 + RECAPS.Retenu_Ligne32 + RECAPS.Retenu_Ligne33;
                string D342 = TOTALE.ToString("F3").Replace(",", "");
                while (D342.Length < 15)
                {
                    D342 = "0" + D342;
                }
                //15;
                string DECEMP34 = D340 + D341 + D342;
                writer.WriteLine(DECEMP34);
                #endregion
            }
            byte[] fileBytes_RECAP = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Images/" + nom));
            return fileBytes_RECAP;
        }
        public byte[] SaveAnnexe1(string ANNEE, DECLARATIONS SelectedSociete, string CODE_ACTE, DECLARATIONS_EMPLOYEURS SelectedDeclaration)
        {
            List<ANNEXE_1> ListeAnnexe1 = new List<ANNEXE_1>();
            if (Session["LISTE_ANNEXE_1_" + Session.SessionID] != null)
            {
                ListeAnnexe1 = (List<ANNEXE_1>)Session["LISTE_ANNEXE_1_" + Session.SessionID];

            }
            FileInfo info = new FileInfo("Fichier vide.TXT");
            string nom = "ANXEMP_1_" + ANNEE.ToString().Substring(2) + "_1";
            var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fileStream.Close();
            using (StreamWriter writer = new StreamWriter(Server.MapPath(@"~/Images/" + nom), false, Encoding.ASCII))
            {
                string E000 = "E1";//2
                string E001 = SelectedSociete.MATRICULE;//12
                string E005 = ANNEE;//4
                string E006 = "An1";//3
                string E007 = CODE_ACTE;//1
                string E008 = ListeAnnexe1.Count.ToString("000000");//6
                string E009 = SelectedSociete.SOCIETE;//40
                string E010 = SelectedSociete.ACTIVITE;//40
                string E011 = SelectedSociete.VILLE;//40
                string E012 = SelectedSociete.RUE;//72
                string E013 = SelectedSociete.NUMERO;//4
                string E014 = SelectedSociete.CODE_POSTAL;//4
                string E015 = new string(' ', 192);//192
                while (E009.Length < 40)
                {
                    E009 = E009 + " ";
                }
                while (E010.Length < 40)
                {
                    E010 = E010 + " ";
                }
                while (E011.Length < 40)
                {
                    E011 = E011 + " ";
                }
                while (E012.Length < 72)
                {
                    E012 = E012 + " ";
                }
                while (E013.Length < 4)
                {
                    E013 = E013 + " ";
                }
                while (E014.Length < 4)
                {
                    E014 = E014 + " ";
                }
                string FirstLine = E000 + E001 + E005 + E006 + E007 + E008 + E009 + E010 + E011 + E012 + E013 + E014 + E015;
                writer.WriteLine(FirstLine);
                decimal Total_REVENU_IMPOSABLE = 0;
                decimal Total_AVANTAGE_EN_NATURE = 0;
                decimal Total_BRUT = 0;
                decimal Total_MONTANT_REINVESTI = 0;
                decimal Total_RETENUE_IRPP = 0;
                decimal Total_RETENUE_20 = 0;
                decimal Total_REDEVANCE = 0;
                decimal Total_NET_SERVI = 0;
                for (int i = 0; i < ListeAnnexe1.Count; i++)
                {
                    ANNEXE_1 Element = ListeAnnexe1.ElementAt(i);
                    string A100 = "L1";//2
                    string A101 = SelectedSociete.MATRICULE;//12
                    string A105 = ANNEE;//4
                    string A106 = (i + 1).ToString("000000");//6
                    string A107 = Element.TYPE.ToString();//1
                    string A108 = Element.EMPLOYEES.CIN;//13
                    string A109 = Element.EMPLOYEES.FULLNAME;//40
                    string A110 = Element.EMPLOYEES.ACTIVITE;//40
                    string A111 = Element.EMPLOYEES.ADRESSE;//120
                    string A112 = Element.EMPLOYEES.SITUATION_FAMILIALE.ToString();//1
                    string A113 = Element.EMPLOYEES.NOMBRE_ENFANT.ToString("00");//2
                    string A114 = Element.DATE_DEBUT.ToShortDateString().Replace("/", "");//8
                    string A115 = Element.DATE_FIN.ToShortDateString().Replace("/", "");//8
                    string A116 = Element.DUREE.ToString("000");//3
                    string A117 = Element.REVENU_IMPOSABLE.ToString("F3").Replace(",", "");//15
                    string A118 = Element.AVANTAGE_EN_NATURE.ToString("F3").Replace(",", "");//15
                    string A119 = Element.BRUT.ToString("F3").Replace(",", "");//15
                    string A120 = Element.MONTANT_REINVESTI.ToString("F3").Replace(",", "");//15
                    string A121 = Element.RETENUE_IRPP.ToString("F3").Replace(",", "");//15
                    string A122 = Element.RETENUE_20.ToString("F3").Replace(",", "");//15
                    string A123 = Element.REDEVANCE.ToString("F3").Replace(",", "");//15
                    string A124 = Element.NET_SERVI.ToString("F3").Replace(",", "");//15
                    string A125 = new string(' ', 40);//40
                    while (A108.Length < 13)
                    {
                        A108 = A108 + " ";
                    }
                    while (A109.Length < 40)
                    {
                        A109 = A109 + " ";
                    }
                    while (A110.Length < 40)
                    {
                        A110 = A110 + " ";
                    }
                    while (A111.Length < 120)
                    {
                        A111 = A111 + " ";
                    }
                    while (A117.Length < 15)
                    {
                        A117 = "0" + A117;
                    }
                    while (A118.Length < 15)
                    {
                        A118 = "0" + A118;
                    }
                    while (A119.Length < 15)
                    {
                        A119 = "0" + A119;
                    }
                    while (A120.Length < 15)
                    {
                        A120 = "0" + A120;
                    }
                    while (A121.Length < 15)
                    {
                        A121 = "0" + A121;
                    }
                    while (A122.Length < 15)
                    {
                        A122 = "0" + A122;
                    }
                    while (A123.Length < 15)
                    {
                        A123 = "0" + A123;
                    }
                    while (A124.Length < 15)
                    {
                        A124 = "0" + A124;
                    }
                    string SubLine = A100 + A101 + A105 + A106 + A107 + A108 + A109 + A110 + A111 + A112 + A113 + A114 + A115 + A116 + A117 + A118 + A119 + A120 + A121 + A122 + A123 + A124 + A125;
                    writer.WriteLine(SubLine);
                    Total_REVENU_IMPOSABLE += Element.REVENU_IMPOSABLE;
                    Total_AVANTAGE_EN_NATURE += Element.AVANTAGE_EN_NATURE;
                    Total_BRUT += Element.BRUT;
                    Total_MONTANT_REINVESTI += Element.MONTANT_REINVESTI;
                    Total_RETENUE_IRPP += Element.RETENUE_IRPP;
                    Total_RETENUE_20 += Element.RETENUE_20;
                    Total_REDEVANCE += Element.REDEVANCE;
                    Total_NET_SERVI += Element.NET_SERVI;
                    Element.DECLARATION = SelectedDeclaration.ID;
                    Element.DECLARATIONS_EMPLOYEURS = SelectedDeclaration;
                    BD.ANNEXE_1.Add(Element);
                    BD.SaveChanges();
                }
                string T100 = "T1";//2
                string T101 = SelectedSociete.MATRICULE;//12
                string T105 = ANNEE;//4
                string T106 = new string(' ', 242);//242
                string T107 = Total_REVENU_IMPOSABLE.ToString("F3").Replace(",", "");//15
                string T108 = Total_AVANTAGE_EN_NATURE.ToString("F3").Replace(",", "");//15
                string T109 = Total_BRUT.ToString("F3").Replace(",", "");//15
                string T110 = Total_MONTANT_REINVESTI.ToString("F3").Replace(",", "");//15
                string T111 = Total_RETENUE_IRPP.ToString("F3").Replace(",", "");//15
                string T112 = Total_RETENUE_20.ToString("F3").Replace(",", "");//15
                string T113 = Total_REDEVANCE.ToString("F3").Replace(",", "");//15
                string T114 = Total_NET_SERVI.ToString("F3").Replace(",", "");//15
                string T115 = new string(' ', 40);//40
                while (T107.Length < 15)
                {
                    T107 = "0" + T107;
                }
                while (T108.Length < 15)
                {
                    T108 = "0" + T108;
                }
                while (T109.Length < 15)
                {
                    T109 = "0" + T109;
                }
                while (T110.Length < 15)
                {
                    T110 = "0" + T110;
                }
                while (T111.Length < 15)
                {
                    T111 = "0" + T111;
                }
                while (T112.Length < 15)
                {
                    T112 = "0" + T112;
                }
                while (T113.Length < 15)
                {
                    T113 = "0" + T113;
                }
                while (T114.Length < 15)
                {
                    T114 = "0" + T114;
                }
                string LastLine = T100 + T101 + T105 + T106 + T107 + T108 + T109 + T110 + T111 + T112 + T113 + T114 + T115;
                writer.WriteLine(LastLine);
            }
            byte[] fileBytes_Annexe1 = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Images/" + nom));
            return fileBytes_Annexe1;
        }
        public byte[] SaveAnnexe2(string ANNEE, DECLARATIONS SelectedSociete, string CODE_ACTE, DECLARATIONS_EMPLOYEURS SelectedDeclaration)
        {
            List<ANNEXE_2> ListeAnnexe2 = new List<ANNEXE_2>();
            if (Session["LISTE_ANNEXE_2_" + Session.SessionID] != null)
            {
                ListeAnnexe2 = (List<ANNEXE_2>)Session["LISTE_ANNEXE_2_" + Session.SessionID];
            }
            FileInfo info = new FileInfo("Fichier vide.TXT");
            string nom = "ANXEMP_2_" + ANNEE.ToString().Substring(2) + "_1";
            var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fileStream.Close();
            using (StreamWriter writer = new StreamWriter(Server.MapPath(@"~/Images/" + nom), false, Encoding.ASCII))
            {
                string E000 = "E2";//2
                string E001 = SelectedSociete.MATRICULE;//12
                string E005 = ANNEE;//4
                string E006 = "An2";//3
                string E007 = CODE_ACTE;//1
                string E008 = ListeAnnexe2.Count.ToString("000000");//6
                string E009 = SelectedSociete.SOCIETE;//40
                string E010 = SelectedSociete.ACTIVITE;//40
                string E011 = SelectedSociete.VILLE;//40
                string E012 = SelectedSociete.RUE;//72
                string E013 = SelectedSociete.NUMERO;//4
                string E014 = SelectedSociete.CODE_POSTAL;//4
                string E015 = new string(' ', 192);//192
                while (E009.Length < 40)
                {
                    E009 = E009 + " ";
                }
                while (E010.Length < 40)
                {
                    E010 = E010 + " ";
                }
                while (E011.Length < 40)
                {
                    E011 = E011 + " ";
                }
                while (E012.Length < 72)
                {
                    E012 = E012 + " ";
                }
                while (E013.Length < 4)
                {
                    E013 = E013 + " ";
                }
                while (E014.Length < 4)
                {
                    E014 = E014 + " ";
                }
                string FirstLine = E000 + E001 + E005 + E006 + E007 + E008 + E009 + E010 + E011 + E012 + E013 + E014 + E015;
                writer.WriteLine(FirstLine);
                decimal Total_HONORAIRE_NON_COMMERCIALE = 0;
                decimal Total_HONORAIRE_SOCIETE = 0;
                decimal Total_JETON = 0;
                decimal Total_REMUNERATION = 0;
                decimal Total_PLUS_VALUE_IMMOBILIERE = 0;
                decimal Total_HOTEL = 0;
                decimal Total_ARTISTES = 0;
                decimal Total_BUREAU_ETUDE_EXPORTATEUR = 0;
                decimal Total_AUTRES = 0;
                decimal Total_RETENUE = 0;
                decimal Total_REDEVANCE_CGC = 0;
                decimal Total_NET_SERVI = 0;
                for (int i = 0; i < ListeAnnexe2.Count; i++)
                {
                    ANNEXE_2 Element = ListeAnnexe2.ElementAt(i);
                    string A200 = "L2";//2
                    string A201 = SelectedSociete.MATRICULE;//12
                    string A205 = ANNEE;//4
                    string A206 = (i + 1).ToString("000000");//6
                    string A207 = Element.TYPE.ToString();//1
                    string A208 = Element.IDENTIFIANT;//13
                    string A209 = Element.NOM_PRENOM;//40
                    string A210 = Element.ACTIVITE;//40
                    string A211 = Element.ADRESSE;//120
                    string A212 = Element.TYPE_MONTANT.ToString();//1
                    string A213 = Element.HONORAIRE_NON_COMMERCIALE.ToString("F3").Replace(",", "");//15
                    string A214 = Element.HONORAIRE_SOCIETE.ToString("F3").Replace(",", "");//15
                    string A215 = Element.JETON.ToString("F3").Replace(",", "");//15
                    string A216 = Element.REMUNERATION.ToString("F3").Replace(",", "");//15
                    string A217 = Element.PLUS_VALUE_IMMOBILIERE.ToString("F3").Replace(",", "");//15
                    string A218 = Element.HOTEL.ToString("F3").Replace(",", "");//15
                    string A219 = Element.ARTISTES.ToString("F3").Replace(",", "");//15
                    string A220 = Element.BUREAU_ETUDE_EXPORTATEUR.ToString("F3").Replace(",", "");//15
                    string A221 = Element.TYPE_MONTANT_OPERATION_EXPORTATION.ToString();//1
                    string A222 = Element.AUTRES.ToString("F3").Replace(",", "");//15
                    string A223 = Element.RETENUE.ToString("F3").Replace(",", "");//15
                    string A224 = Element.REDEVANCE_CGC.ToString("F3").Replace(",", "");//15
                    string A225 = Element.NET_SERVI.ToString("F3").Replace(",", "");//15
                    while (A208.Length < 13)
                    {
                        A208 = A208 + " ";
                    }
                    while (A209.Length < 40)
                    {
                        A209 = A209 + " ";
                    }
                    while (A210.Length < 40)
                    {
                        A210 = A210 + " ";
                    }
                    while (A211.Length < 120)
                    {
                        A211 = A211 + " ";
                    }
                    while (A213.Length < 15)
                    {
                        A213 = "0" + A213;
                    }
                    while (A214.Length < 15)
                    {
                        A214 = "0" + A214;
                    }
                    while (A215.Length < 15)
                    {
                        A215 = "0" + A215;
                    }
                    while (A216.Length < 15)
                    {
                        A216 = "0" + A216;
                    }
                    while (A217.Length < 15)
                    {
                        A217 = "0" + A217;
                    }
                    while (A218.Length < 15)
                    {
                        A218 = "0" + A218;
                    }
                    while (A219.Length < 15)
                    {
                        A219 = "0" + A219;
                    }
                    while (A220.Length < 15)
                    {
                        A220 = "0" + A220;
                    }
                    while (A222.Length < 15)
                    {
                        A222 = "0" + A222;
                    }
                    while (A223.Length < 15)
                    {
                        A223 = "0" + A223;
                    }
                    while (A224.Length < 15)
                    {
                        A224 = "0" + A224;
                    }
                    while (A225.Length < 15)
                    {
                        A225 = "0" + A225;
                    }
                    string SubLine = A200 + A201 + A205 + A206 + A207 + A208 + A209 + A210 + A211 + A212 + A213 + A214 + A215 + A216 + A217 + A218 + A219 + A220 + A221 + A222 + A223 + A224 + A225;
                    writer.WriteLine(SubLine);
                    Total_HONORAIRE_NON_COMMERCIALE += Element.HONORAIRE_NON_COMMERCIALE;
                    Total_HONORAIRE_SOCIETE += Element.HONORAIRE_SOCIETE;
                    Total_JETON += Element.JETON;
                    Total_REMUNERATION += Element.REMUNERATION;
                    Total_PLUS_VALUE_IMMOBILIERE += Element.PLUS_VALUE_IMMOBILIERE;
                    Total_HOTEL += Element.HOTEL;
                    Total_ARTISTES += Element.ARTISTES;
                    Total_BUREAU_ETUDE_EXPORTATEUR += Element.BUREAU_ETUDE_EXPORTATEUR;
                    Total_AUTRES += Element.AUTRES;
                    Total_RETENUE += Element.RETENUE;
                    Total_REDEVANCE_CGC += Element.REDEVANCE_CGC;
                    Total_NET_SERVI += Element.NET_SERVI;
                    Element.DECLARATION = SelectedDeclaration.ID;
                    Element.DECLARATIONS_EMPLOYEURS = SelectedDeclaration;
                    BD.ANNEXE_2.Add(Element);
                    BD.SaveChanges();
                }
                string T200 = "T2";//2
                string T201 = SelectedSociete.MATRICULE;//12
                string T205 = ANNEE;//4
                string T206 = new string(' ', 221);//221
                string T207 = Total_HONORAIRE_NON_COMMERCIALE.ToString("F3").Replace(",", "");//15
                string T208 = Total_HONORAIRE_SOCIETE.ToString("F3").Replace(",", "");//15
                string T209 = Total_JETON.ToString("F3").Replace(",", "");//15
                string T210 = Total_REMUNERATION.ToString("F3").Replace(",", "");//15
                string T211 = Total_PLUS_VALUE_IMMOBILIERE.ToString("F3").Replace(",", "");//15
                string T212 = Total_HOTEL.ToString("F3").Replace(",", "");//15
                string T213 = Total_ARTISTES.ToString("F3").Replace(",", "");//15
                string T214 = Total_BUREAU_ETUDE_EXPORTATEUR.ToString("F3").Replace(",", "");//15
                string T215 = " ";//1
                string T216 = Total_AUTRES.ToString("F3").Replace(",", "");//15
                string T217 = Total_RETENUE.ToString("F3").Replace(",", "");//15
                string T218 = Total_REDEVANCE_CGC.ToString("F3").Replace(",", "");//15
                string T219 = Total_NET_SERVI.ToString("F3").Replace(",", "");//15

                while (T207.Length < 15)
                {
                    T207 = "0" + T207;
                }
                while (T208.Length < 15)
                {
                    T208 = "0" + T208;
                }
                while (T209.Length < 15)
                {
                    T209 = "0" + T209;
                }
                while (T210.Length < 15)
                {
                    T210 = "0" + T210;
                }
                while (T211.Length < 15)
                {
                    T211 = "0" + T211;
                }
                while (T212.Length < 15)
                {
                    T212 = "0" + T212;
                }
                while (T213.Length < 15)
                {
                    T213 = "0" + T213;
                }
                while (T214.Length < 15)
                {
                    T214 = "0" + T214;
                }
                while (T216.Length < 15)
                {
                    T216 = "0" + T216;
                }
                while (T217.Length < 15)
                {
                    T217 = "0" + T217;
                }
                while (T218.Length < 15)
                {
                    T218 = "0" + T218;
                }
                while (T219.Length < 15)
                {
                    T219 = "0" + T219;
                }
                string LastLine = T200 + T201 + T205 + T206 + T207 + T208 + T209 + T210 + T211 + T212 + T213 + T214 + T215 + T216 + T217 + T218 + T219;
                writer.WriteLine(LastLine);
            }
            byte[] fileBytes_Annexe2 = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Images/" + nom));
            return fileBytes_Annexe2;
        }
        public byte[] SaveAnnexe3(string ANNEE, DECLARATIONS SelectedSociete, string CODE_ACTE, DECLARATIONS_EMPLOYEURS SelectedDeclaration)
        {
            List<ANNEXE_3> ListeAnnexe3 = new List<ANNEXE_3>();
            if (Session["LISTE_ANNEXE_3_" + Session.SessionID] != null)
            {
                ListeAnnexe3 = (List<ANNEXE_3>)Session["LISTE_ANNEXE_3_" + Session.SessionID];
            }
            FileInfo info = new FileInfo("Fichier vide.TXT");
            string nom = "ANXEMP_3_" + ANNEE.ToString().Substring(2) + "_1";
            var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fileStream.Close();
            using (StreamWriter writer = new StreamWriter(Server.MapPath(@"~/Images/" + nom), false, Encoding.ASCII))
            {
                string E000 = "E3";//2
                string E001 = SelectedSociete.MATRICULE;//12
                string E005 = ANNEE;//4
                string E006 = "An3";//3
                string E007 = CODE_ACTE;//1
                string E008 = ListeAnnexe3.Count.ToString("000000");//6
                string E009 = SelectedSociete.SOCIETE;//40
                string E010 = SelectedSociete.ACTIVITE;//40
                string E011 = SelectedSociete.VILLE;//40
                string E012 = SelectedSociete.RUE;//72
                string E013 = SelectedSociete.NUMERO;//4
                string E014 = SelectedSociete.CODE_POSTAL;//4
                string E015 = new string(' ', 192);//192
                while (E009.Length < 40)
                {
                    E009 = E009 + " ";
                }
                while (E010.Length < 40)
                {
                    E010 = E010 + " ";
                }
                while (E011.Length < 40)
                {
                    E011 = E011 + " ";
                }
                while (E012.Length < 72)
                {
                    E012 = E012 + " ";
                }
                while (E013.Length < 4)
                {
                    E013 = E013 + " ";
                }
                while (E014.Length < 4)
                {
                    E014 = E014 + " ";
                }
                string FirstLine = E000 + E001 + E005 + E006 + E007 + E008 + E009 + E010 + E011 + E012 + E013 + E014 + E015;
                writer.WriteLine(FirstLine);
                decimal Total_Interet_CENT = 0;
                decimal Total_Interet_Capitaux = 0;
                decimal Total_Interet_Pret = 0;
                decimal Total_Retenue = 0;
                decimal Total_Redevance = 0;
                decimal Total_Net_Servi = 0;
                for (int i = 0; i < ListeAnnexe3.Count; i++)
                {
                    ANNEXE_3 Element = ListeAnnexe3.ElementAt(i);
                    string A300 = "L3";//2
                    string A301 = SelectedSociete.MATRICULE;//12
                    string A305 = ANNEE;//4
                    string A306 = (i + 1).ToString("000000");//6
                    string A307 = Element.TYPE.ToString();//1
                    string A308 = Element.IDENTIFIANT;//13
                    string A309 = Element.NOM_PRENOM;//40
                    string A310 = Element.ACTIVITE;//40
                    string A311 = Element.ADRESSE;//120
                    string A312 = Element.INTERET_COMPTES_CENT.ToString("F3").Replace(",", "");//15
                    string A313 = Element.INTERET_CAPITAUX_MOBILIERES.ToString("F3").Replace(",", "");//15
                    string A314 = Element.INTERET_PRETS.ToString("F3").Replace(",", "");//15
                    string A315 = Element.MONTANT_RETENUE.ToString("F3").Replace(",", "");//15
                    string A316 = Element.REDEVANCE_CGC.ToString("F3").Replace(",", "");//15
                    string A317 = Element.NET_SERVI.ToString("F3").Replace(",", "");//15
                    string A318 = new string(' ', 92);//92
                    while (A308.Length < 13)
                    {
                        A308 = A308 + " ";
                    }
                    while (A309.Length < 40)
                    {
                        A309 = A309 + " ";
                    }
                    while (A310.Length < 40)
                    {
                        A310 = A310 + " ";
                    }
                    while (A311.Length < 120)
                    {
                        A311 = A311 + " ";
                    }
                    while (A312.Length < 15)
                    {
                        A312 = "0" + A312;
                    }
                    while (A313.Length < 15)
                    {
                        A313 = "0" + A313;
                    }
                    while (A314.Length < 15)
                    {
                        A314 = "0" + A314;
                    }
                    while (A315.Length < 15)
                    {
                        A315 = "0" + A315;
                    }
                    while (A316.Length < 15)
                    {
                        A316 = "0" + A316;
                    }
                    while (A317.Length < 15)
                    {
                        A317 = "0" + A317;
                    }
                    string SubLine = A300 + A301 + A305 + A306 + A307 + A308 + A309 + A310 + A311 + A312 + A313 + A314 + A315 + A316 + A317 + A318;
                    writer.WriteLine(SubLine);
                    Total_Interet_CENT += Element.INTERET_COMPTES_CENT;
                    Total_Interet_Capitaux += Element.INTERET_CAPITAUX_MOBILIERES;
                    Total_Interet_Pret += Element.INTERET_PRETS;
                    Total_Retenue += Element.MONTANT_RETENUE;
                    Total_Redevance += Element.MONTANT_RETENUE;
                    Total_Net_Servi += Element.NET_SERVI;
                    Element.DECLARATION = SelectedDeclaration.ID;
                    Element.DECLARATIONS_EMPLOYEURS = SelectedDeclaration;
                    BD.ANNEXE_3.Add(Element);
                    BD.SaveChanges();
                }
                string T300 = "T3";//2
                string T301 = SelectedSociete.MATRICULE;//12
                string T305 = ANNEE;//4
                string T306 = new string(' ', 220);//220
                string T307 = Total_Interet_CENT.ToString("F3").Replace(",", "");//15
                string T308 = Total_Interet_Capitaux.ToString("F3").Replace(",", "");//15
                string T309 = Total_Interet_Pret.ToString("F3").Replace(",", "");//15
                string T310 = Total_Retenue.ToString("F3").Replace(",", "");//15
                string T311 = Total_Redevance.ToString("F3").Replace(",", "");//15
                string T312 = Total_Net_Servi.ToString("F3").Replace(",", "");//15
                string T313 = new string(' ', 92);//92
                while (T307.Length < 15)
                {
                    T307 = "0" + T307;
                }
                while (T308.Length < 15)
                {
                    T308 = "0" + T308;
                }
                while (T309.Length < 15)
                {
                    T309 = "0" + T309;
                }
                while (T310.Length < 15)
                {
                    T310 = "0" + T310;
                }
                while (T311.Length < 15)
                {
                    T311 = "0" + T311;
                }
                while (T312.Length < 15)
                {
                    T312 = "0" + T312;
                }
                while (T313.Length < 15)
                {
                    T313 = "0" + T313;
                }
                string LastLine = T300 + T301 + T305 + T306 + T307 + T308 + T309 + T310 + T311 + T312 + T313;
                writer.WriteLine(LastLine);
            }
            byte[] fileBytes_Annexe3 = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Images/" + nom));
            return fileBytes_Annexe3;
        }
        public byte[] SaveAnnexe4(string ANNEE, DECLARATIONS SelectedSociete, string CODE_ACTE, DECLARATIONS_EMPLOYEURS SelectedDeclaration)
        {
            List<ANNEXE_4> ListeAnnexe4 = new List<ANNEXE_4>();
            if (Session["LISTE_ANNEXE_4_" + Session.SessionID] != null)
            {
                ListeAnnexe4 = (List<ANNEXE_4>)Session["LISTE_ANNEXE_4_" + Session.SessionID];
            }
            FileInfo info = new FileInfo("Fichier vide.TXT");
            string nom = "ANXEMP_4_" + ANNEE.ToString().Substring(2) + "_1";
            var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fileStream.Close();
            using (StreamWriter writer = new StreamWriter(Server.MapPath(@"~/Images/" + nom), false, Encoding.ASCII))
            {
                string E000 = "E4";//2
                string E001 = SelectedSociete.MATRICULE;//12
                string E005 = ANNEE;//4
                string E006 = "An4";//3
                string E007 = CODE_ACTE;//1
                string E008 = ListeAnnexe4.Count.ToString("000000");//6
                string E009 = SelectedSociete.SOCIETE;//40
                string E010 = SelectedSociete.ACTIVITE;//40
                string E011 = SelectedSociete.VILLE;//40
                string E012 = SelectedSociete.RUE;//72
                string E013 = SelectedSociete.NUMERO;//4
                string E014 = SelectedSociete.CODE_POSTAL;//4
                string E015 = new string(' ', 192);//192
                while (E009.Length < 40)
                {
                    E009 = E009 + " ";
                }
                while (E010.Length < 40)
                {
                    E010 = E010 + " ";
                }
                while (E011.Length < 40)
                {
                    E011 = E011 + " ";
                }
                while (E012.Length < 72)
                {
                    E012 = E012 + " ";
                }
                while (E013.Length < 4)
                {
                    E013 = E013 + " ";
                }
                while (E014.Length < 4)
                {
                    E014 = E014 + " ";
                }
                string FirstLine = E000 + E001 + E005 + E006 + E007 + E008 + E009 + E010 + E011 + E012 + E013 + E014 + E015;
                writer.WriteLine(FirstLine);
                decimal Total_TAUX_MONTANT_HONORAIRE = 0;
                decimal Total_MONTANT_BRUT_HONORAIRE = 0;
                decimal Total_TAUX_HONORAIRE_6_MOIS = 0;
                decimal Total_MONTANT_HONORAIRE_6_MOIS = 0;
                decimal Total_TAUX_PLUS_VALUE_IMMOBILIERE = 0;
                decimal Total_MONTANT_PLUS_VALUE_IMMOBILIERE = 0;
                decimal Total_TAUX_REVENUES_IMMOBILIERE = 0;
                decimal Total_MONTANT_REVENUE_IMMOBILIERE = 0;
                decimal Total_TAUX_CESSION_ACTION = 0;
                decimal Total_MONTANT_CESSION_ACTION = 0;
                decimal Total_MONTANT_RETENUE = 0;
                decimal Total_MONTANT_OP_EXPORTATION = 0;
                decimal Total_MONTANT_PARADIS_FISCAUX = 0;
                decimal Total_NET_SERVI = 0;
                for (int i = 0; i < ListeAnnexe4.Count; i++)
                {
                    ANNEXE_4 Element = ListeAnnexe4.ElementAt(i);
                    string A400 = "L4";//2
                    string A401 = SelectedSociete.MATRICULE;//12
                    string A405 = ANNEE;//4
                    string A406 = (i + 1).ToString("000000");//6
                    string A407 = Element.TYPE_BENEFICIAIRE.ToString();//1
                    string A408 = Element.IDENTIFIANT;//13
                    string A409 = Element.NOM_PRENOM;//40
                    string A410 = Element.ACTIVITE;//40
                    string A411 = Element.ADRESSE;//120
                    string A412 = Element.TYPE_MONTANT.ToString();//1
                    string A413 = Element.TAUX_MONTANT_HONORAIRE.ToString("F3").Replace(",", "");//5
                    string A414 = Element.MONTANT_BRUT_HONORAIRE.ToString("F3").Replace(",", "");//15
                    string A415 = Element.TAUX_HONORAIRE_6_MOIS.ToString("F3").Replace(",", "");//5
                    string A416 = Element.MONTANT_HONORAIRE_6_MOIS.ToString("F3").Replace(",", "");//15
                    string A417 = Element.TAUX_PLUS_VALUE_IMMOBILIERE.ToString("F3").Replace(",", "");//5
                    string A418 = Element.MONTANT_PLUS_VALUE_IMMOBILIERE.ToString("F3").Replace(",", "");//15
                    string A419 = Element.TAUX_REVENUES_IMMOBILIERE.ToString("F3").Replace(",", "");//5
                    string A420 = Element.MONTANT_REVENUE_IMMOBILIERE.ToString("F3").Replace(",", "");//15
                    string A421 = Element.TAUX_CESSION_ACTION.ToString("F3").Replace(",", "");//5
                    string A422 = Element.MONTANT_CESSION_ACTION.ToString("F3").Replace(",", "");//15
                    string A423 = Element.MONTANT_RETENUE.ToString("F3").Replace(",", "");//15
                    string A424 = Element.TYPE_MONTANT_OP_EXPORTATION.ToString();//1
                    string A425 = Element.MONTANT_OP_EXPORTATION.ToString("F3").Replace(",", "");//15
                    string A426 = Element.MONTANT_PARADIS_FISCAUX.ToString("F3").Replace(",", "");//15
                    string A427 = Element.NET_SERVI.ToString("F3").Replace(",", "");//15
                    string A428 = new string(' ', 20);//20
                    while (A408.Length < 13)
                    {
                        A408 = A408 + " ";
                    }
                    while (A409.Length < 40)
                    {
                        A409 = A409 + " ";
                    }
                    while (A410.Length < 40)
                    {
                        A410 = A410 + " ";
                    }
                    while (A411.Length < 120)
                    {
                        A411 = A411 + " ";
                    }
                    while (A413.Length < 5)
                    {
                        A413 = "0" + A413;
                    }
                    while (A414.Length < 15)
                    {
                        A414 = "0" + A414;
                    }
                    while (A415.Length < 5)
                    {
                        A415 = "0" + A415;
                    }
                    while (A416.Length < 15)
                    {
                        A416 = "0" + A416;
                    }
                    while (A417.Length < 5)
                    {
                        A417 = "0" + A417;
                    }
                    while (A418.Length < 15)
                    {
                        A418 = "0" + A418;
                    }
                    while (A419.Length < 5)
                    {
                        A419 = "0" + A419;
                    }
                    while (A420.Length < 15)
                    {
                        A420 = "0" + A420;
                    }
                    while (A421.Length < 5)
                    {
                        A421 = "0" + A421;
                    }
                    while (A422.Length < 15)
                    {
                        A422 = "0" + A422;
                    }
                    while (A423.Length < 15)
                    {
                        A423 = "0" + A423;
                    }
                    while (A425.Length < 15)
                    {
                        A425 = "0" + A425;
                    }
                    while (A426.Length < 15)
                    {
                        A426 = "0" + A426;
                    }
                    while (A427.Length < 15)
                    {
                        A427 = "0" + A427;
                    }
                    string SubLine = A400 + A401 + A405 + A406 + A407 + A408 + A409 + A410 + A411 + A412 + A413 + A414 + A415 + A416 + A417 + A418 + A419 + A420 + A421 + A422 + A423 + A424 + A425 + A426 + A427 + A428;
                    writer.WriteLine(SubLine);
                    Total_TAUX_MONTANT_HONORAIRE += Element.TAUX_MONTANT_HONORAIRE;
                    Total_MONTANT_BRUT_HONORAIRE += Element.MONTANT_BRUT_HONORAIRE;
                    Total_TAUX_HONORAIRE_6_MOIS += Element.TAUX_HONORAIRE_6_MOIS;
                    Total_MONTANT_HONORAIRE_6_MOIS += Element.MONTANT_HONORAIRE_6_MOIS;
                    Total_TAUX_PLUS_VALUE_IMMOBILIERE += Element.TAUX_PLUS_VALUE_IMMOBILIERE;
                    Total_MONTANT_PLUS_VALUE_IMMOBILIERE += Element.MONTANT_PLUS_VALUE_IMMOBILIERE;
                    Total_TAUX_REVENUES_IMMOBILIERE += Element.TAUX_REVENUES_IMMOBILIERE;
                    Total_MONTANT_REVENUE_IMMOBILIERE += Element.MONTANT_REVENUE_IMMOBILIERE;
                    Total_TAUX_CESSION_ACTION += Element.TAUX_CESSION_ACTION;
                    Total_MONTANT_CESSION_ACTION += Element.MONTANT_CESSION_ACTION;
                    Total_MONTANT_RETENUE += Element.MONTANT_RETENUE;
                    Total_MONTANT_OP_EXPORTATION += Element.MONTANT_OP_EXPORTATION;
                    Total_MONTANT_PARADIS_FISCAUX += Element.MONTANT_PARADIS_FISCAUX;
                    Total_NET_SERVI += Element.NET_SERVI;
                    Element.DECLARATION = SelectedDeclaration.ID;
                    Element.DECLARATIONS_EMPLOYEURS = SelectedDeclaration;
                    BD.ANNEXE_4.Add(Element);
                    BD.SaveChanges();
                }
                string T400 = "T4";//2
                string T401 = SelectedSociete.MATRICULE;//12
                string T405 = ANNEE;//4
                string T406 = new string(' ', 221);//221
                string T407 = "00000";//5
                string T408 = Total_MONTANT_BRUT_HONORAIRE.ToString("F3").Replace(",", "");//15
                string T409 = "00000";//5
                string T410 = Total_MONTANT_HONORAIRE_6_MOIS.ToString("F3").Replace(",", "");//15
                string T411 = "00000";//5
                string T412 = Total_MONTANT_PLUS_VALUE_IMMOBILIERE.ToString("F3").Replace(",", "");//15
                string T413 = "00000";//5
                string T414 = Total_MONTANT_REVENUE_IMMOBILIERE.ToString("F3").Replace(",", "");//15
                string T415 = "00000";//5
                string T416 = Total_MONTANT_CESSION_ACTION.ToString("F3").Replace(",", "");//15
                string T417 = Total_MONTANT_RETENUE.ToString("F3").Replace(",", "");//15
                string T418 = " ";//1
                string T419 = Total_MONTANT_OP_EXPORTATION.ToString("F3").Replace(",", "");//15
                string T420 = Total_MONTANT_PARADIS_FISCAUX.ToString("F3").Replace(",", "");//15
                string T421 = Total_NET_SERVI.ToString("F3").Replace(",", "");//15
                string T422 = new string(' ', 20);
                while (T408.Length < 15)
                {
                    T408 = "0" + T408;
                }
                while (T410.Length < 15)
                {
                    T410 = "0" + T410;
                }
                while (T412.Length < 15)
                {
                    T412 = "0" + T412;
                }
                while (T414.Length < 15)
                {
                    T414 = "0" + T414;
                }
                while (T416.Length < 15)
                {
                    T416 = "0" + T416;
                }
                while (T417.Length < 15)
                {
                    T417 = "0" + T417;
                }
                while (T419.Length < 15)
                {
                    T419 = "0" + T419;
                }
                while (T420.Length < 15)
                {
                    T420 = "0" + T420;
                }
                while (T421.Length < 15)
                {
                    T421 = "0" + T421;
                }
                string LastLine = T400 + T401 + T405 + T406 + T407 + T408 + T409 + T410 + T411 + T412 + T413 + T414 + T415 + T416 + T417 + T418 + T419 + T420 + T421 + T422;
                writer.WriteLine(LastLine);
            }
            byte[] fileBytes_Annexe4 = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Images/" + nom));
            return fileBytes_Annexe4;
        }
        public byte[] SaveAnnexe5(string ANNEE, DECLARATIONS SelectedSociete, string CODE_ACTE, DECLARATIONS_EMPLOYEURS SelectedDeclaration)
        {
            List<ANNEXE_5> ListeAnnexe5 = new List<ANNEXE_5>();
            if (Session["LISTE_ANNEXE_5_" + Session.SessionID] != null)
            {
                ListeAnnexe5 = (List<ANNEXE_5>)Session["LISTE_ANNEXE_5_" + Session.SessionID];
            }
            FileInfo info = new FileInfo("Fichier vide.TXT");
            string nom = "ANXEMP_5_" + ANNEE.ToString().Substring(2) + "_1";
            var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fileStream.Close();
            using (StreamWriter writer = new StreamWriter(Server.MapPath(@"~/Images/" + nom), false, Encoding.ASCII))
            {
                string E000 = "E5";//2
                string E001 = SelectedSociete.MATRICULE;//12
                string E005 = ANNEE;//4
                string E006 = "An5";//3
                string E007 = CODE_ACTE;//1
                string E008 = ListeAnnexe5.Count.ToString("000000");//6
                string E009 = SelectedSociete.SOCIETE;//40
                string E010 = SelectedSociete.ACTIVITE;//40
                string E011 = SelectedSociete.VILLE;//40
                string E012 = SelectedSociete.RUE;//72
                string E013 = SelectedSociete.NUMERO;//4
                string E014 = SelectedSociete.CODE_POSTAL;//4
                string E015 = new string(' ', 192);//192
                while (E009.Length < 40)
                {
                    E009 = E009 + " ";
                }
                while (E010.Length < 40)
                {
                    E010 = E010 + " ";
                }
                while (E011.Length < 40)
                {
                    E011 = E011 + " ";
                }
                while (E012.Length < 72)
                {
                    E012 = E012 + " ";
                }
                while (E013.Length < 4)
                {
                    E013 = E013 + " ";
                }
                while (E014.Length < 4)
                {
                    E014 = E014 + " ";
                }
                string FirstLine = E000 + E001 + E005 + E006 + E007 + E008 + E009 + E010 + E011 + E012 + E013 + E014 + E015;
                writer.WriteLine(FirstLine);
                decimal Total_MONTANT_MARCHES = 0;
                decimal Total_RETENUE = 0;
                decimal Total_TVA_ETAT = 0;
                decimal Total_REVENUE_TVA = 0;
                decimal Total_MONTANT_ETABLISSEMENTS_PUBLICS = 0;
                decimal Total_RETENUE_ETABLISSEMENT_PUBLICS = 0;
                decimal Total_MONTANT_MARCHES_ETRANGES = 0;
                decimal Total_RETENUE_MARCHES_ETRANGES = 0;
                decimal Total_REDEVANCE_CGC = 0;
                decimal Total_NET_SERVI = 0;
                for (int i = 0; i < ListeAnnexe5.Count; i++)
                {
                    ANNEXE_5 Element = ListeAnnexe5.ElementAt(i);
                    string A500 = "L5";//2
                    string A501 = SelectedSociete.MATRICULE;//12
                    string A505 = ANNEE;//4
                    string A506 = (i + 1).ToString("000000");//6
                    string A507 = Element.TYPE.ToString();//1
                    string A508 = Element.IDENTIFIANT;//13
                    string A509 = Element.NOM_PRENOM;//40
                    string A510 = Element.ACTIVITE;//40
                    string A511 = Element.ADRESSE;//120
                    string A512 = Element.MONTANT_MARCHES.ToString("F3").Replace(",", "");//15
                    string A513 = Element.RETENUE.ToString("F3").Replace(",", "");//15
                    string A514 = Element.TVA_ETAT.ToString("F3").Replace(",", "");//15
                    string A515 = Element.REVENUE_TVA.ToString("F3").Replace(",", "");//15
                    string A516 = Element.MONTANT_ETABLISSEMENTS_PUBLICS.ToString("F3").Replace(",", "");//15
                    string A517 = Element.RETENUE_ETABLISSEMENT_PUBLICS.ToString("F3").Replace(",", "");//15
                    string A518 = Element.MONTANT_MARCHES_ETRANGES.ToString("F3").Replace(",", "");//15
                    string A519 = Element.RETENUE_MARCHES_ETRANGES.ToString("F3").Replace(",", "");//15
                    string A520 = Element.REDEVANCE_CGC.ToString("F3").Replace(",", "");//15
                    string A521 = Element.NET_SERVI.ToString("F3").Replace(",", "");//15
                    string A522 = new string(' ', 32);//32
                    while (A508.Length < 13)
                    {
                        A508 = A508 + " ";
                    }
                    while (A509.Length < 40)
                    {
                        A509 = A509 + " ";
                    }
                    while (A510.Length < 40)
                    {
                        A510 = A510 + " ";
                    }
                    while (A511.Length < 120)
                    {
                        A511 = A511 + " ";
                    }
                    while (A512.Length < 15)
                    {
                        A512 = "0" + A512;
                    }
                    while (A513.Length < 15)
                    {
                        A513 = "0" + A513;
                    }
                    while (A514.Length < 15)
                    {
                        A514 = "0" + A514;
                    }
                    while (A515.Length < 15)
                    {
                        A515 = "0" + A515;
                    }
                    while (A516.Length < 15)
                    {
                        A516 = "0" + A516;
                    }
                    while (A517.Length < 15)
                    {
                        A517 = "0" + A517;
                    }
                    while (A518.Length < 15)
                    {
                        A518 = "0" + A518;
                    }
                    while (A519.Length < 15)
                    {
                        A519 = "0" + A519;
                    }
                    while (A520.Length < 15)
                    {
                        A520 = "0" + A520;
                    }
                    while (A521.Length < 15)
                    {
                        A521 = "0" + A521;
                    }
                    string SubLine = A500 + A501 + A505 + A506 + A507 + A508 + A509 + A510 + A511 + A512 + A513 + A514 + A515 + A516 + A517 + A518 + A519 + A520 + A521 + A522;
                    writer.WriteLine(SubLine);
                    Total_MONTANT_MARCHES += Element.MONTANT_MARCHES;
                    Total_RETENUE += Element.RETENUE;
                    Total_TVA_ETAT += Element.TVA_ETAT;
                    Total_REVENUE_TVA += Element.REVENUE_TVA;
                    Total_MONTANT_ETABLISSEMENTS_PUBLICS += Element.MONTANT_ETABLISSEMENTS_PUBLICS;
                    Total_RETENUE_ETABLISSEMENT_PUBLICS += Element.RETENUE_ETABLISSEMENT_PUBLICS;
                    Total_MONTANT_MARCHES_ETRANGES += Element.MONTANT_MARCHES_ETRANGES;
                    Total_RETENUE_MARCHES_ETRANGES += Element.RETENUE_MARCHES_ETRANGES;
                    Total_REDEVANCE_CGC += Element.REDEVANCE_CGC;
                    Total_NET_SERVI += Element.NET_SERVI;
                    Element.DECLARATION = SelectedDeclaration.ID;
                    Element.DECLARATIONS_EMPLOYEURS = SelectedDeclaration;
                    BD.ANNEXE_5.Add(Element);
                    BD.SaveChanges();
                }
                string T500 = "T5";//2
                string T501 = SelectedSociete.MATRICULE;//12
                string T505 = ANNEE;//4
                string T506 = new string(' ', 220);//220
                string T507 = Total_MONTANT_MARCHES.ToString("F3").Replace(",", "");//15
                string T508 = Total_RETENUE.ToString("F3").Replace(",", "");//15
                string T509 = Total_TVA_ETAT.ToString("F3").Replace(",", "");//15
                string T510 = Total_REVENUE_TVA.ToString("F3").Replace(",", "");//15
                string T511 = Total_MONTANT_ETABLISSEMENTS_PUBLICS.ToString("F3").Replace(",", "");//15
                string T512 = Total_RETENUE_ETABLISSEMENT_PUBLICS.ToString("F3").Replace(",", "");//15
                string T513 = Total_MONTANT_MARCHES_ETRANGES.ToString("F3").Replace(",", "");//15
                string T514 = Total_RETENUE_MARCHES_ETRANGES.ToString("F3").Replace(",", "");//15
                string T515 = Total_REDEVANCE_CGC.ToString("F3").Replace(",", "");//15
                string T516 = Total_NET_SERVI.ToString("F3").Replace(",", "");//15
                string T517 = new string(' ', 32);//32
                while (T507.Length < 15)
                {
                    T507 = "0" + T507;
                }
                while (T508.Length < 15)
                {
                    T508 = "0" + T508;
                }
                while (T509.Length < 15)
                {
                    T509 = "0" + T509;
                }
                while (T510.Length < 15)
                {
                    T510 = "0" + T510;
                }
                while (T511.Length < 15)
                {
                    T511 = "0" + T511;
                }
                while (T512.Length < 15)
                {
                    T512 = "0" + T512;
                }
                while (T513.Length < 15)
                {
                    T513 = "0" + T513;
                }
                while (T514.Length < 15)
                {
                    T514 = "0" + T514;
                }
                while (T515.Length < 15)
                {
                    T515 = "0" + T515;
                }
                while (T516.Length < 15)
                {
                    T516 = "0" + T516;
                }
                string LastLine = T500 + T501 + T505 + T506 + T507 + T508 + T509 + T510 + T511 + T512 + T513 + T514 + T515 + T516 + T517;
                writer.WriteLine(LastLine);
            }
            byte[] fileBytes_Annexe5 = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Images/" + nom));
            return fileBytes_Annexe5;
        }
        public byte[] SaveAnnexe6(string ANNEE, DECLARATIONS SelectedSociete, string CODE_ACTE, DECLARATIONS_EMPLOYEURS SelectedDeclaration)
        {
            List<ANNEXE_6> ListeAnnexe6 = new List<ANNEXE_6>();
            if (Session["LISTE_ANNEXE_6_" + Session.SessionID] != null)
            {
                ListeAnnexe6 = (List<ANNEXE_6>)Session["LISTE_ANNEXE_6_" + Session.SessionID];
            }
            FileInfo info = new FileInfo("Fichier vide.TXT");
            string nom = "ANXEMP_6_" + ANNEE.ToString().Substring(2) + "_1";
            var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fileStream.Close();
            using (StreamWriter writer = new StreamWriter(Server.MapPath(@"~/Images/" + nom), false, Encoding.ASCII))
            {
                string E000 = "E6";//2
                string E001 = SelectedSociete.MATRICULE;//12
                string E005 = ANNEE;//4
                string E006 = "An6";//3
                string E007 = CODE_ACTE;//1
                string E008 = ListeAnnexe6.Count.ToString("000000");//6
                string E009 = SelectedSociete.SOCIETE;//40
                string E010 = SelectedSociete.ACTIVITE;//40
                string E011 = SelectedSociete.VILLE;//40
                string E012 = SelectedSociete.RUE;//72
                string E013 = SelectedSociete.NUMERO;//4
                string E014 = SelectedSociete.CODE_POSTAL;//4
                string E015 = new string(' ', 192);//192
                while (E009.Length < 40)
                {
                    E009 = E009 + " ";
                }
                while (E010.Length < 40)
                {
                    E010 = E010 + " ";
                }
                while (E011.Length < 40)
                {
                    E011 = E011 + " ";
                }
                while (E012.Length < 72)
                {
                    E012 = E012 + " ";
                }
                while (E013.Length < 4)
                {
                    E013 = E013 + " ";
                }
                while (E014.Length < 4)
                {
                    E014 = E014 + " ";
                }
                string FirstLine = E000 + E001 + E005 + E006 + E007 + E008 + E009 + E010 + E011 + E012 + E013 + E014 + E015;
                writer.WriteLine(FirstLine);
                decimal Total_Montant_Commerciale = 0;
                decimal Total_Montant_Vente_PP = 0;
                decimal Total_Montant_Avance_PP = 0;
                decimal Total_Montant_Marchandise = 0;
                for (int i = 0; i < ListeAnnexe6.Count; i++)
                {
                    ANNEXE_6 Element = ListeAnnexe6.ElementAt(i);
                    string A600 = "L6";//2
                    string A601 = SelectedSociete.MATRICULE;//12
                    string A605 = ANNEE;//4
                    string A606 = (i + 1).ToString("000000");//6
                    string A607 = Element.TYPE_BENEFICIAIRE.ToString();//1
                    string A608 = Element.IDENTIFIANT;//13
                    string A609 = Element.NOM_PRENOM;//40
                    string A610 = Element.ACTIVITE;//40
                    string A611 = Element.ADRESSE;//120
                    string A612 = Element.MONTANT_RISTOURNES.ToString("F3").Replace(",", "");//15
                    string A613 = Element.MONTANT_VENTE_PP.ToString("F3").Replace(",", "");//15
                    string A614 = Element.MONTANT_AVANCE_VENTE_PP.ToString("F3").Replace(",", "");//15
                    string A615 = Element.MONTANT_ESPECES_MARCHANDISES.ToString("F3").Replace(",", "");//15
                    string A616 = new string(' ', 122);//122
                    while (A608.Length < 13)
                    {
                        A608 = A608 + " ";
                    }
                    while (A609.Length < 40)
                    {
                        A609 = A609 + " ";
                    }
                    while (A610.Length < 40)
                    {
                        A610 = A610 + " ";
                    }
                    while (A611.Length < 120)
                    {
                        A611 = A611 + " ";
                    }
                    while (A612.Length < 15)
                    {
                        A612 = "0" + A612;
                    }
                    while (A613.Length < 15)
                    {
                        A613 = "0" + A613;
                    }
                    while (A614.Length < 15)
                    {
                        A614 = "0" + A614;
                    }
                    while (A615.Length < 15)
                    {
                        A615 = "0" + A615;
                    }
                    string SubLine = A600 + A601 + A605 + A606 + A607 + A608 + A609 + A610 + A611 + A612 + A613 + A614 + A615 + A616;
                    writer.WriteLine(SubLine);
                    Total_Montant_Commerciale += Element.MONTANT_RISTOURNES;
                    Total_Montant_Vente_PP += Element.MONTANT_VENTE_PP;
                    Total_Montant_Avance_PP += Element.MONTANT_AVANCE_VENTE_PP;
                    Total_Montant_Marchandise += Element.MONTANT_ESPECES_MARCHANDISES;
                    Element.DECLARATION = SelectedDeclaration.ID;
                    Element.DECLARATIONS_EMPLOYEURS = SelectedDeclaration;
                    BD.ANNEXE_6.Add(Element);
                    BD.SaveChanges();
                }
                string T600 = "T6";//2
                string T601 = SelectedSociete.MATRICULE;//12
                string T605 = ANNEE;//4
                string T606 = new string(' ', 220);//220
                string T607 = Total_Montant_Commerciale.ToString("F3").Replace(",", "");//15
                string T608 = Total_Montant_Vente_PP.ToString("F3").Replace(",", "");//15
                string T609 = Total_Montant_Avance_PP.ToString("F3").Replace(",", "");//15
                string T610 = Total_Montant_Marchandise.ToString("F3").Replace(",", "");//15
                string T611 = new string(' ', 122);//122
                while (T607.Length < 15)
                {
                    T607 = "0" + T607;
                }
                while (T608.Length < 15)
                {
                    T608 = "0" + T608;
                }
                while (T609.Length < 15)
                {
                    T609 = "0" + T609;
                }
                while (T610.Length < 15)
                {
                    T610 = "0" + T610;
                }
                while (T611.Length < 15)
                {
                    T611 = "0" + T611;
                }
                string LastLine = T600 + T601 + T605 + T606 + T607 + T608 + T609 + T610 + T611;
                writer.WriteLine(LastLine);
            }
            byte[] fileBytes_Annexe6 = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Images/" + nom));
            return fileBytes_Annexe6;
        }
        public byte[] SaveAnnexe7(string ANNEE, DECLARATIONS SelectedSociete, string CODE_ACTE, DECLARATIONS_EMPLOYEURS SelectedDeclaration)
        {
            List<ANNEXE_7> ListeAnnexe7 = new List<ANNEXE_7>();
            if (Session["LISTE_ANNEXE_7_" + Session.SessionID] != null)
            {
                ListeAnnexe7 = (List<ANNEXE_7>)Session["LISTE_ANNEXE_7_" + Session.SessionID];
            }
            FileInfo info = new FileInfo("Fichier vide.TXT");
            string nom = "ANXEMP_7_" + ANNEE.ToString().Substring(2) + "_1";
            var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fileStream.Close();
            using (StreamWriter writer = new StreamWriter(Server.MapPath(@"~/Images/" + nom), false, Encoding.ASCII))
            {
                string E000 = "E7";//2
                string E001 = SelectedSociete.MATRICULE;//12
                string E005 = ANNEE;//4
                string E006 = "An7";//3
                string E007 = CODE_ACTE;//1
                string E008 = ListeAnnexe7.Count.ToString("000000");//6
                string E009 = SelectedSociete.SOCIETE;//40
                string E010 = SelectedSociete.ACTIVITE;//40
                string E011 = SelectedSociete.VILLE;//40
                string E012 = SelectedSociete.RUE;//72
                string E013 = SelectedSociete.NUMERO;//4
                string E014 = SelectedSociete.CODE_POSTAL;//4
                string E015 = new string(' ', 192);//192
                while (E009.Length < 40)
                {
                    E009 = E009 + " ";
                }
                while (E010.Length < 40)
                {
                    E010 = E010 + " ";
                }
                while (E011.Length < 40)
                {
                    E011 = E011 + " ";
                }
                while (E012.Length < 72)
                {
                    E012 = E012 + " ";
                }
                while (E013.Length < 4)
                {
                    E013 = E013 + " ";
                }
                while (E014.Length < 4)
                {
                    E014 = E014 + " ";
                }
                string FirstLine = E000 + E001 + E005 + E006 + E007 + E008 + E009 + E010 + E011 + E012 + E013 + E014 + E015;
                writer.WriteLine(FirstLine);
                decimal Total_Montant = 0;
                decimal Total_Retenue = 0;
                decimal Total_Net = 0;
                for (int i = 0; i < ListeAnnexe7.Count; i++)
                {
                    ANNEXE_7 Element = ListeAnnexe7.ElementAt(i);
                    string A700 = "L7";//2
                    string A701 = SelectedSociete.MATRICULE;//12
                    string A705 = ANNEE;//4
                    string A706 = (i + 1).ToString("000000");//6
                    string A707 = Element.TYPE_BENEFICIAIRE.ToString();//1
                    string A708 = Element.IDENTIFIANT;//13
                    string A709 = Element.NOM_PRENOM;//40
                    string A710 = Element.ACTIVITE;//40
                    string A711 = Element.ADRESSE;//120
                    string A712 = Element.TYPE_MONTANT.ToString("00");//2
                    string A713 = Element.MONTANT_PAYES.ToString("F3").Replace(",", "");//15
                    string A714 = Element.RETENUE_SOURCE.ToString("F3").Replace(",", "");//15
                    string A715 = Element.NET_SERVI.ToString("F3").Replace(",", "");//15
                    string A716 = new string(' ', 135);//135
                    while (A708.Length < 13)
                    {
                        A708 = A708 + " ";
                    }
                    while (A709.Length < 40)
                    {
                        A709 = A709 + " ";
                    }
                    while (A710.Length < 40)
                    {
                        A710 = A710 + " ";
                    }
                    while (A711.Length < 120)
                    {
                        A711 = A711 + " ";
                    }
                    while (A713.Length < 15)
                    {
                        A713 = "0" + A713;
                    }
                    while (A714.Length < 15)
                    {
                        A714 = "0" + A714;
                    }
                    while (A715.Length < 15)
                    {
                        A715 = "0" + A715;
                    }
                    string SubLine = A700 + A701 + A705 + A706 + A707 + A708 + A709 + A710 + A711 + A712 + A713 + A714 + A715 + A716;
                    writer.WriteLine(SubLine);
                    Total_Montant += Element.MONTANT_PAYES;
                    Total_Retenue += Element.RETENUE_SOURCE;
                    Total_Net += Element.NET_SERVI;
                    Element.DECLARATION = SelectedDeclaration.ID;
                    Element.DECLARATIONS_EMPLOYEURS = SelectedDeclaration;
                    BD.ANNEXE_7.Add(Element);
                    BD.SaveChanges();
                }
                string T700 = "T7";//2
                string T701 = SelectedSociete.MATRICULE;//12
                string T705 = ANNEE;//4
                string T706 = new string(' ', 222);//222
                string T707 = Total_Montant.ToString("F3").Replace(",", "");//15
                string T708 = Total_Retenue.ToString("F3").Replace(",", "");//15
                string T709 = Total_Net.ToString("F3").Replace(",", "");//15
                string T710 = new string(' ', 135);//135
                while (T707.Length < 15)
                {
                    T707 = "0" + T707;
                }
                while (T708.Length < 15)
                {
                    T708 = "0" + T708;
                }
                while (T709.Length < 15)
                {
                    T709 = "0" + T709;
                }
                while (T710.Length < 15)
                {
                    T710 = "0" + T710;
                }
                string LastLine = T700 + T701 + T705 + T706 + T707 + T708 + T709 + T710;
                writer.WriteLine(LastLine);
            }
            byte[] fileBytes_Annexe7 = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Images/" + nom));
            return fileBytes_Annexe7;

        }
        public void Uploadfile(Stream fStream, string fileName, string annexe)
        {
            byte[] contents = new byte[fStream.Length];
            fStream.Read(contents, 0, (int)fStream.Length);
            fStream.Close();
            string connectionString = "";
            string fileExtension = Path.GetExtension(fileName).ToUpper();
            if (fileExtension == ".XLS")
            {
                connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + fileName + "'; Extended Properties='Excel 8.0;HDR=YES;'";
            }
            else if (fileExtension == ".XLSX")
            {
                connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + fileName + "';Extended Properties='Excel 12.0 Xml;HDR=YES;'";
            }
            if (!(string.IsNullOrEmpty(connectionString)))
            {
                string[] sheetNames = GetExcelSheetNames(connectionString);
                if ((sheetNames != null) && (sheetNames.Length > 0))
                {
                    DataTable dt = null;
                    OleDbConnection con = new OleDbConnection(connectionString);
                    OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [" + sheetNames[0] + "]", con);
                    dt = new DataTable();
                    da.Fill(dt);
                    InsertIntoList(fStream, dt, annexe);
                }
            }
        }
        private string[] GetExcelSheetNames(string strConnection)
        {
            var connectionString = strConnection;
            String[] excelSheets;
            using (var connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                var dt = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dt == null)
                {
                    return null;
                }
                excelSheets = new String[dt.Rows.Count];
                int i = 0;
                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[i] = row["TABLE_NAME"].ToString();
                    i++;
                }
            }
            return excelSheets;
        }
        private void InsertIntoList(Stream fStream, DataTable listTable, string annexe)
        {
            #region ANNEXE_1
            if (annexe == "ANNEXE_1")
            {
                List<ANNEXE_1> ListeAnnexe1 = new List<ANNEXE_1>();
                try
                {
                    for (int iRow = 0; iRow < listTable.Rows.Count; iRow++)
                    {
                        string TYPE_BENEFICAIRE = listTable.Rows[iRow][0] != null ? Convert.ToString(listTable.Rows[iRow][0]) : "2";
                        string CIN = listTable.Rows[iRow][1] != null ? Convert.ToString(listTable.Rows[iRow][1]) : string.Empty;
                        string FULLNAME = listTable.Rows[iRow][2] != null ? Convert.ToString(listTable.Rows[iRow][2]) : string.Empty;
                        string ACTIVITE = listTable.Rows[iRow][3] != null ? Convert.ToString(listTable.Rows[iRow][3]) : string.Empty;
                        string ADRESSE = listTable.Rows[iRow][4] != null ? Convert.ToString(listTable.Rows[iRow][4]) : string.Empty;
                        string SITUATION_FAMILIALE = listTable.Rows[iRow][5] != null ? Convert.ToString(listTable.Rows[iRow][5]) : "0";
                        string NOMBRE_ENFANT = listTable.Rows[iRow][6] != null ? Convert.ToString(listTable.Rows[iRow][6]) : "0";
                        string DATE_DEBUT = listTable.Rows[iRow][7] != null ? Convert.ToString(listTable.Rows[iRow][7]) : string.Empty;
                        string DATE_FIN = listTable.Rows[iRow][8] != null ? Convert.ToString(listTable.Rows[iRow][8]) : string.Empty;
                        string DUREE = listTable.Rows[iRow][9] != null ? Convert.ToString(listTable.Rows[iRow][9]) : "1";
                        string REVENU_IMPOSABLE = listTable.Rows[iRow][10] != null ? Convert.ToString(listTable.Rows[iRow][10]) : "0";
                        string AVANTAGE_EN_NATURE = listTable.Rows[iRow][11] != null ? Convert.ToString(listTable.Rows[iRow][11]) : "0";
                        string BRUT = listTable.Rows[iRow][12] != null ? Convert.ToString(listTable.Rows[iRow][12]) : "0";
                        string MONTANT_REINVESTI = listTable.Rows[iRow][13] != null ? Convert.ToString(listTable.Rows[iRow][13]) : "0";
                        string RETENUE_IRPP = listTable.Rows[iRow][14] != null ? Convert.ToString(listTable.Rows[iRow][14]) : "0";
                        string RETENUE_20 = listTable.Rows[iRow][15] != null ? Convert.ToString(listTable.Rows[iRow][15]) : "0";
                        string REDEVANCE = listTable.Rows[iRow][16] != null ? Convert.ToString(listTable.Rows[iRow][16]) : "0";
                        string CONTRIBUTION_CONJONTURELLE = listTable.Rows[iRow][17] != null ? Convert.ToString(listTable.Rows[iRow][17]) : "0";
                        string NET_SERVI = listTable.Rows[iRow][18] != null ? Convert.ToString(listTable.Rows[iRow][18]) : "0";
                        if (CIN.Length > 0)
                        {
                            ANNEXE_1 Nouveau = new ANNEXE_1();
                            Nouveau.TYPE = int.Parse(TYPE_BENEFICAIRE);
                            while (CIN.Length <= 8)
                            {
                                CIN = "0" + CIN;
                            }
                            EMPLOYEES Employee = BD.EMPLOYEES.Where(Element => Element.CIN == CIN).FirstOrDefault();
                            if (Employee == null)
                            {
                                Employee = new EMPLOYEES();
                                Employee.CIN = CIN;
                                Employee.FULLNAME = FULLNAME;
                                Employee.ACTIVITE = ACTIVITE;
                                Employee.ADRESSE = ADRESSE;
                                Employee.SITUATION_FAMILIALE = int.Parse(SITUATION_FAMILIALE);
                                Employee.NOMBRE_ENFANT = int.Parse(NOMBRE_ENFANT);
                                Employee.ACTIF = true;
                                BD.EMPLOYEES.Add(Employee);
                            }
                            else
                            {
                                Employee.CIN = CIN;
                                Employee.FULLNAME = FULLNAME;
                                Employee.ACTIVITE = ACTIVITE;
                                Employee.ADRESSE = ADRESSE;
                                Employee.SITUATION_FAMILIALE = int.Parse(SITUATION_FAMILIALE);
                                Employee.NOMBRE_ENFANT = int.Parse(NOMBRE_ENFANT);
                                BD.SaveChanges();
                            }
                            Nouveau.EMPLOYEES = Employee;
                            Nouveau.EMPLOYEE = Employee.ID;
                            Nouveau.DATE_DEBUT = DateTime.Parse(DATE_DEBUT);
                            Nouveau.DATE_FIN = DateTime.Parse(DATE_FIN);
                            Nouveau.DUREE = int.Parse(DUREE);
                            Nouveau.REVENU_IMPOSABLE = !string.IsNullOrEmpty(REVENU_IMPOSABLE) ? decimal.Parse(REVENU_IMPOSABLE) : 0;
                            Nouveau.AVANTAGE_EN_NATURE = !string.IsNullOrEmpty(AVANTAGE_EN_NATURE) ? decimal.Parse(AVANTAGE_EN_NATURE) : 0;
                            Nouveau.BRUT = !string.IsNullOrEmpty(BRUT) ? decimal.Parse(BRUT) : 0;
                            Nouveau.MONTANT_REINVESTI = !string.IsNullOrEmpty(MONTANT_REINVESTI) ? decimal.Parse(MONTANT_REINVESTI) : 0;
                            Nouveau.RETENUE_IRPP = !string.IsNullOrEmpty(RETENUE_IRPP) ? decimal.Parse(RETENUE_IRPP) : 0;
                            Nouveau.RETENUE_20 = !string.IsNullOrEmpty(RETENUE_20) ? decimal.Parse(RETENUE_20) : 0;
                            Nouveau.REDEVANCE = !string.IsNullOrEmpty(REDEVANCE) ? decimal.Parse(REDEVANCE) : 0;
                            Nouveau.CONTRIBUTION_CONJONTURELLE = !string.IsNullOrEmpty(CONTRIBUTION_CONJONTURELLE) ? decimal.Parse(CONTRIBUTION_CONJONTURELLE) : 0;
                            Nouveau.NET_SERVI = !string.IsNullOrEmpty(NET_SERVI) ? decimal.Parse(NET_SERVI) : 0;
                            ListeAnnexe1.Add(Nouveau);
                        }
                    }
                    Session["LISTE_ANNEXE_1_" + Session.SessionID] = ListeAnnexe1;
                }
                catch (Exception)
                {
                }
                finally
                {
                };
            }
            #endregion
            #region ANNEXE_2
            if (annexe == "ANNEXE_2")
            {
                List<ANNEXE_2> ListeAnnexe2 = new List<ANNEXE_2>();
                try
                {
                    for (int iRow = 0; iRow < listTable.Rows.Count; iRow++)
                    {
                        string TYPE_BENEFICAIRE = listTable.Rows[iRow][0] != null ? Convert.ToString(listTable.Rows[iRow][0]) : "1";
                        string IDENTIFIANT = listTable.Rows[iRow][1] != null ? Convert.ToString(listTable.Rows[iRow][1]) : string.Empty;
                        string FULLNAME = listTable.Rows[iRow][2] != null ? Convert.ToString(listTable.Rows[iRow][2]) : string.Empty;
                        string ACTIVITE = listTable.Rows[iRow][3] != null ? Convert.ToString(listTable.Rows[iRow][3]) : string.Empty;
                        string ADRESSE = listTable.Rows[iRow][4] != null ? Convert.ToString(listTable.Rows[iRow][4]) : string.Empty;
                        string TYPE_MONTANT = listTable.Rows[iRow][5] != null ? Convert.ToString(listTable.Rows[iRow][5]) : "1";
                        string HONORAIRE_NON_COMMERCIALE = listTable.Rows[iRow][6] != null ? Convert.ToString(listTable.Rows[iRow][6]) : "0";
                        string HONORAIRE_SOCIETE = listTable.Rows[iRow][7] != null ? Convert.ToString(listTable.Rows[iRow][7]) : "0";
                        string JETON = listTable.Rows[iRow][8] != null ? Convert.ToString(listTable.Rows[iRow][8]) : string.Empty;
                        string REMUNERATION = listTable.Rows[iRow][9] != null ? Convert.ToString(listTable.Rows[iRow][9]) : "0";
                        string PLUS_VALUE_IMMOBILIERE = listTable.Rows[iRow][10] != null ? Convert.ToString(listTable.Rows[iRow][10]) : "0";
                        string HOTEL = listTable.Rows[iRow][11] != null ? Convert.ToString(listTable.Rows[iRow][11]) : "0";
                        string ARTISTES = listTable.Rows[iRow][12] != null ? Convert.ToString(listTable.Rows[iRow][12]) : "0";
                        string BUREAU_ETUDE_EXPORTATEUR = listTable.Rows[iRow][13] != null ? Convert.ToString(listTable.Rows[iRow][13]) : "0";
                        string TYPE_MONTANT_OPERATION_EXPORTATION = listTable.Rows[iRow][14] != null ? Convert.ToString(listTable.Rows[iRow][14]) : "0";
                        string AUTRES = listTable.Rows[iRow][15] != null ? Convert.ToString(listTable.Rows[iRow][15]) : "0";
                        string RETENUE = listTable.Rows[iRow][16] != null ? Convert.ToString(listTable.Rows[iRow][16]) : "0";
                        string REDEVANCE_CGC = listTable.Rows[iRow][17] != null ? Convert.ToString(listTable.Rows[iRow][17]) : "0";
                        string NET_SERVI = listTable.Rows[iRow][18] != null ? Convert.ToString(listTable.Rows[iRow][18]) : "0";
                        if (!string.IsNullOrEmpty(IDENTIFIANT))
                        {
                            ANNEXE_2 Nouveau = new ANNEXE_2();
                            Nouveau.TYPE = int.Parse(TYPE_BENEFICAIRE);
                            Nouveau.IDENTIFIANT = IDENTIFIANT;
                            Nouveau.NOM_PRENOM = FULLNAME;
                            Nouveau.ACTIVITE = ACTIVITE;
                            Nouveau.ADRESSE = ADRESSE;
                            Nouveau.TYPE_MONTANT = int.Parse(TYPE_MONTANT);
                            Nouveau.HONORAIRE_NON_COMMERCIALE = !string.IsNullOrEmpty(HONORAIRE_NON_COMMERCIALE) ? decimal.Parse(HONORAIRE_NON_COMMERCIALE) : 0;
                            Nouveau.HONORAIRE_SOCIETE = !string.IsNullOrEmpty(HONORAIRE_SOCIETE) ? decimal.Parse(HONORAIRE_SOCIETE) : 0;
                            Nouveau.JETON = !string.IsNullOrEmpty(JETON) ? decimal.Parse(JETON) : 0;
                            Nouveau.REMUNERATION = !string.IsNullOrEmpty(REMUNERATION) ? decimal.Parse(REMUNERATION) : 0;
                            Nouveau.PLUS_VALUE_IMMOBILIERE = !string.IsNullOrEmpty(PLUS_VALUE_IMMOBILIERE) ? decimal.Parse(PLUS_VALUE_IMMOBILIERE) : 0;
                            Nouveau.HOTEL = !string.IsNullOrEmpty(HOTEL) ? decimal.Parse(HOTEL) : 0;
                            Nouveau.ARTISTES = !string.IsNullOrEmpty(ARTISTES) ? decimal.Parse(ARTISTES) : 0;
                            Nouveau.BUREAU_ETUDE_EXPORTATEUR = !string.IsNullOrEmpty(BUREAU_ETUDE_EXPORTATEUR) ? decimal.Parse(BUREAU_ETUDE_EXPORTATEUR) : 0;
                            Nouveau.AUTRES = !string.IsNullOrEmpty(AUTRES) ? decimal.Parse(AUTRES) : 0;
                            Nouveau.TYPE_MONTANT_OPERATION_EXPORTATION = !string.IsNullOrEmpty(TYPE_MONTANT_OPERATION_EXPORTATION) ? int.Parse(TYPE_MONTANT_OPERATION_EXPORTATION) : 0;
                            Nouveau.RETENUE = !string.IsNullOrEmpty(RETENUE) ? decimal.Parse(RETENUE) : 0;
                            Nouveau.REDEVANCE_CGC = !string.IsNullOrEmpty(REDEVANCE_CGC) ? decimal.Parse(REDEVANCE_CGC) : 0;
                            Nouveau.NET_SERVI = !string.IsNullOrEmpty(NET_SERVI) ? decimal.Parse(NET_SERVI) : 0;
                            ListeAnnexe2.Add(Nouveau);
                        }
                    }
                    Session["LISTE_ANNEXE_2_" + Session.SessionID] = ListeAnnexe2;
                }
                catch (Exception)
                {
                }
                finally
                {
                };
            }
            #endregion
            #region ANNEXE_3
            if (annexe == "ANNEXE_3")
            {
                List<ANNEXE_3> listeAnnexe3 = new List<ANNEXE_3>();
                try
                {
                    for (int iRow = 0; iRow < listTable.Rows.Count; iRow++)
                    {
                        string TYPE_BENEFICAIRE = listTable.Rows[iRow][0] != null ? Convert.ToString(listTable.Rows[iRow][0]) : "4";
                        string IDENTIFIANT = listTable.Rows[iRow][1] != null ? Convert.ToString(listTable.Rows[iRow][1]) : string.Empty;
                        string FULLNAME = listTable.Rows[iRow][2] != null ? Convert.ToString(listTable.Rows[iRow][2]) : string.Empty;
                        string ACTIVITE = listTable.Rows[iRow][3] != null ? Convert.ToString(listTable.Rows[iRow][3]) : string.Empty;
                        string ADRESSE = listTable.Rows[iRow][4] != null ? Convert.ToString(listTable.Rows[iRow][4]) : string.Empty;
                        string INTERET_COMPTES_CENT = listTable.Rows[iRow][5] != null ? Convert.ToString(listTable.Rows[iRow][5]) : "1";
                        string INTERET_CAPITAUX_MOBILIERES = listTable.Rows[iRow][6] != null ? Convert.ToString(listTable.Rows[iRow][6]) : "0";
                        string INTERET_PRETS = listTable.Rows[iRow][7] != null ? Convert.ToString(listTable.Rows[iRow][7]) : "0";
                        string MONTANT_RETENUE = listTable.Rows[iRow][8] != null ? Convert.ToString(listTable.Rows[iRow][8]) : string.Empty;
                        string REDEVANCE_CGC = listTable.Rows[iRow][9] != null ? Convert.ToString(listTable.Rows[iRow][9]) : "0";
                        string NET_SERVI = listTable.Rows[iRow][10] != null ? Convert.ToString(listTable.Rows[iRow][10]) : "0";
                        if (!string.IsNullOrEmpty(IDENTIFIANT))
                        {
                            ANNEXE_3 Nouveau = new ANNEXE_3();
                            Nouveau.TYPE = int.Parse(TYPE_BENEFICAIRE);
                            Nouveau.IDENTIFIANT = IDENTIFIANT;
                            Nouveau.NOM_PRENOM = FULLNAME;
                            Nouveau.ACTIVITE = ACTIVITE;
                            Nouveau.ADRESSE = ADRESSE;
                            Nouveau.INTERET_COMPTES_CENT = !string.IsNullOrEmpty(INTERET_COMPTES_CENT) ? decimal.Parse(INTERET_COMPTES_CENT) : 0;
                            Nouveau.INTERET_CAPITAUX_MOBILIERES = !string.IsNullOrEmpty(INTERET_CAPITAUX_MOBILIERES) ? decimal.Parse(INTERET_CAPITAUX_MOBILIERES) : 0;
                            Nouveau.INTERET_PRETS = !string.IsNullOrEmpty(INTERET_PRETS) ? decimal.Parse(INTERET_PRETS) : 0;
                            Nouveau.MONTANT_RETENUE = !string.IsNullOrEmpty(MONTANT_RETENUE) ? decimal.Parse(MONTANT_RETENUE) : 0;
                            Nouveau.REDEVANCE_CGC = !string.IsNullOrEmpty(REDEVANCE_CGC) ? decimal.Parse(REDEVANCE_CGC) : 0;
                            Nouveau.NET_SERVI = !string.IsNullOrEmpty(NET_SERVI) ? decimal.Parse(NET_SERVI) : 0;
                            listeAnnexe3.Add(Nouveau);
                        }
                    }
                    Session["LISTE_ANNEXE_3_" + Session.SessionID] = listeAnnexe3;
                }
                catch (Exception)
                {
                }
                finally
                {
                };
            }
            #endregion
            #region ANNEXE_4
            if (annexe == "ANNEXE_4")
            {
                List<ANNEXE_4> ListeAnnexe4 = new List<ANNEXE_4>();
                try
                {
                    for (int iRow = 0; iRow < listTable.Rows.Count; iRow++)
                    {
                        string TYPE_BENEFICAIRE = listTable.Rows[iRow][0] != null ? Convert.ToString(listTable.Rows[iRow][0]) : "4";
                        string IDENTIFIANT = listTable.Rows[iRow][1] != null ? Convert.ToString(listTable.Rows[iRow][1]) : string.Empty;
                        string FULLNAME = listTable.Rows[iRow][2] != null ? Convert.ToString(listTable.Rows[iRow][2]) : string.Empty;
                        string ACTIVITE = listTable.Rows[iRow][3] != null ? Convert.ToString(listTable.Rows[iRow][3]) : string.Empty;
                        string ADRESSE = listTable.Rows[iRow][4] != null ? Convert.ToString(listTable.Rows[iRow][4]) : string.Empty;
                        string TYPE_MONTANT = listTable.Rows[iRow][5] != null ? Convert.ToString(listTable.Rows[iRow][5]) : "1";
                        string TAUX_MONTANT_HONORAIRE = listTable.Rows[iRow][6] != null ? Convert.ToString(listTable.Rows[iRow][6]) : "0";
                        string MONTANT_BRUT_HONORAIRE = listTable.Rows[iRow][7] != null ? Convert.ToString(listTable.Rows[iRow][7]) : "0";
                        string TAUX_HONORAIRE_6_MOIS = listTable.Rows[iRow][8] != null ? Convert.ToString(listTable.Rows[iRow][8]) : string.Empty;
                        string MONTANT_HONORAIRE_6_MOIS = listTable.Rows[iRow][9] != null ? Convert.ToString(listTable.Rows[iRow][9]) : "0";
                        string TAUX_PLUS_VALUE_IMMOBILIERE = listTable.Rows[iRow][10] != null ? Convert.ToString(listTable.Rows[iRow][10]) : "0";
                        string MONTANT_PLUS_VALUE_IMMOBILIERE = listTable.Rows[iRow][11] != null ? Convert.ToString(listTable.Rows[iRow][11]) : "0";
                        string TAUX_REVENUES_IMMOBILIERE = listTable.Rows[iRow][12] != null ? Convert.ToString(listTable.Rows[iRow][12]) : "0";
                        string MONTANT_REVENUE_IMMOBILIERE = listTable.Rows[iRow][13] != null ? Convert.ToString(listTable.Rows[iRow][13]) : "0";
                        string TAUX_CESSION_ACTION = listTable.Rows[iRow][14] != null ? Convert.ToString(listTable.Rows[iRow][14]) : "0";
                        string MONTANT_CESSION_ACTION = listTable.Rows[iRow][15] != null ? Convert.ToString(listTable.Rows[iRow][15]) : "0";
                        string MONTANT_RETENUE = listTable.Rows[iRow][16] != null ? Convert.ToString(listTable.Rows[iRow][16]) : "0";
                        string TYPE_MONTANT_OP_EXPORTATION = listTable.Rows[iRow][17] != null ? Convert.ToString(listTable.Rows[iRow][17]) : "0";
                        string MONTANT_OP_EXPORTATION = listTable.Rows[iRow][18] != null ? Convert.ToString(listTable.Rows[iRow][18]) : "0";
                        string MONTANT_PARADIS_FISCAUX = listTable.Rows[iRow][19] != null ? Convert.ToString(listTable.Rows[iRow][19]) : "0";
                        string NET_SERVI = listTable.Rows[iRow][20] != null ? Convert.ToString(listTable.Rows[iRow][20]) : "0";

                        if (!string.IsNullOrEmpty(IDENTIFIANT))
                        {
                            ANNEXE_4 Nouveau = new ANNEXE_4();
                            Nouveau.TYPE_BENEFICIAIRE = int.Parse(TYPE_BENEFICAIRE);
                            Nouveau.IDENTIFIANT = IDENTIFIANT;
                            Nouveau.NOM_PRENOM = FULLNAME;
                            Nouveau.ACTIVITE = ACTIVITE;
                            Nouveau.ADRESSE = ADRESSE;
                            Nouveau.TYPE_MONTANT = int.Parse(TYPE_MONTANT);
                            Nouveau.TAUX_MONTANT_HONORAIRE = !string.IsNullOrEmpty(TAUX_MONTANT_HONORAIRE) ? decimal.Parse(TAUX_MONTANT_HONORAIRE) : 0;
                            Nouveau.MONTANT_BRUT_HONORAIRE = !string.IsNullOrEmpty(MONTANT_BRUT_HONORAIRE) ? decimal.Parse(MONTANT_BRUT_HONORAIRE) : 0;
                            Nouveau.TAUX_HONORAIRE_6_MOIS = !string.IsNullOrEmpty(TAUX_HONORAIRE_6_MOIS) ? decimal.Parse(TAUX_HONORAIRE_6_MOIS) : 0;
                            Nouveau.MONTANT_HONORAIRE_6_MOIS = !string.IsNullOrEmpty(MONTANT_HONORAIRE_6_MOIS) ? decimal.Parse(MONTANT_HONORAIRE_6_MOIS) : 0;
                            Nouveau.TAUX_PLUS_VALUE_IMMOBILIERE = !string.IsNullOrEmpty(TAUX_PLUS_VALUE_IMMOBILIERE) ? decimal.Parse(TAUX_PLUS_VALUE_IMMOBILIERE) : 0;
                            Nouveau.MONTANT_PLUS_VALUE_IMMOBILIERE = !string.IsNullOrEmpty(MONTANT_PLUS_VALUE_IMMOBILIERE) ? decimal.Parse(MONTANT_PLUS_VALUE_IMMOBILIERE) : 0;
                            Nouveau.TAUX_REVENUES_IMMOBILIERE = !string.IsNullOrEmpty(TAUX_REVENUES_IMMOBILIERE) ? decimal.Parse(TAUX_REVENUES_IMMOBILIERE) : 0;
                            Nouveau.MONTANT_REVENUE_IMMOBILIERE = !string.IsNullOrEmpty(MONTANT_REVENUE_IMMOBILIERE) ? decimal.Parse(MONTANT_REVENUE_IMMOBILIERE) : 0;
                            Nouveau.TAUX_CESSION_ACTION = !string.IsNullOrEmpty(TAUX_CESSION_ACTION) ? decimal.Parse(TAUX_CESSION_ACTION) : 0;
                            Nouveau.MONTANT_CESSION_ACTION = !string.IsNullOrEmpty(MONTANT_CESSION_ACTION) ? decimal.Parse(MONTANT_CESSION_ACTION) : 0;
                            Nouveau.MONTANT_RETENUE = !string.IsNullOrEmpty(MONTANT_RETENUE) ? decimal.Parse(MONTANT_RETENUE) : 0;
                            Nouveau.TYPE_MONTANT_OP_EXPORTATION = !string.IsNullOrEmpty(TYPE_MONTANT_OP_EXPORTATION) ? int.Parse(TYPE_MONTANT_OP_EXPORTATION) : 0;
                            Nouveau.MONTANT_OP_EXPORTATION = !string.IsNullOrEmpty(MONTANT_OP_EXPORTATION) ? decimal.Parse(MONTANT_OP_EXPORTATION) : 0;
                            Nouveau.MONTANT_PARADIS_FISCAUX = !string.IsNullOrEmpty(MONTANT_PARADIS_FISCAUX) ? decimal.Parse(MONTANT_PARADIS_FISCAUX) : 0;
                            Nouveau.NET_SERVI = !string.IsNullOrEmpty(NET_SERVI) ? decimal.Parse(NET_SERVI) : 0;
                            ListeAnnexe4.Add(Nouveau);
                        }
                    }
                    Session["LISTE_ANNEXE_4_" + Session.SessionID] = ListeAnnexe4;
                }
                catch (Exception)
                {
                }
                finally
                {
                };
            }
            #endregion
            #region ANNEXE_5
            if (annexe == "ANNEXE_5")
            {
                List<ANNEXE_5> ListeAnnexe5 = new List<ANNEXE_5>();
                try
                {
                    for (int iRow = 0; iRow < listTable.Rows.Count; iRow++)
                    {
                        string TYPE_BENEFICAIRE = listTable.Rows[iRow][0] != null ? Convert.ToString(listTable.Rows[iRow][0]) : "1";
                        string IDENTIFIANT = listTable.Rows[iRow][1] != null ? Convert.ToString(listTable.Rows[iRow][1]) : string.Empty;
                        string FULLNAME = listTable.Rows[iRow][2] != null ? Convert.ToString(listTable.Rows[iRow][2]) : string.Empty;
                        string ACTIVITE = listTable.Rows[iRow][3] != null ? Convert.ToString(listTable.Rows[iRow][3]) : string.Empty;
                        string ADRESSE = listTable.Rows[iRow][4] != null ? Convert.ToString(listTable.Rows[iRow][4]) : string.Empty;
                        string MONTANT_MARCHES = listTable.Rows[iRow][5] != null ? Convert.ToString(listTable.Rows[iRow][5]) : "1";
                        string RETENUE = listTable.Rows[iRow][6] != null ? Convert.ToString(listTable.Rows[iRow][6]) : "0";
                        string TVA_ETAT = listTable.Rows[iRow][7] != null ? Convert.ToString(listTable.Rows[iRow][7]) : "0";
                        string REVENUE_TVA = listTable.Rows[iRow][8] != null ? Convert.ToString(listTable.Rows[iRow][8]) : string.Empty;
                        string MONTANT_ETABLISSEMENTS_PUBLICS = listTable.Rows[iRow][9] != null ? Convert.ToString(listTable.Rows[iRow][9]) : "0";
                        string RETENUE_ETABLISSEMENT_PUBLICS = listTable.Rows[iRow][10] != null ? Convert.ToString(listTable.Rows[iRow][10]) : "0";
                        string MONTANT_MARCHES_ETRANGES = listTable.Rows[iRow][11] != null ? Convert.ToString(listTable.Rows[iRow][11]) : "0";
                        string RETENUE_MARCHES_ETRANGES = listTable.Rows[iRow][12] != null ? Convert.ToString(listTable.Rows[iRow][12]) : "0";
                        string REDEVANCE_CGC = listTable.Rows[iRow][13] != null ? Convert.ToString(listTable.Rows[iRow][13]) : "0";
                        string NET_SERVI = listTable.Rows[iRow][14] != null ? Convert.ToString(listTable.Rows[iRow][14]) : "0";
                        {
                            ANNEXE_5 Nouveau = new ANNEXE_5();
                            Nouveau.TYPE = int.Parse(TYPE_BENEFICAIRE);
                            Nouveau.IDENTIFIANT = IDENTIFIANT;
                            Nouveau.NOM_PRENOM = FULLNAME;
                            Nouveau.ACTIVITE = ACTIVITE;
                            Nouveau.ADRESSE = ADRESSE;
                            Nouveau.MONTANT_MARCHES = !string.IsNullOrEmpty(MONTANT_MARCHES) ? decimal.Parse(MONTANT_MARCHES) : 0;
                            Nouveau.RETENUE = !string.IsNullOrEmpty(RETENUE) ? decimal.Parse(RETENUE) : 0;
                            Nouveau.TVA_ETAT = !string.IsNullOrEmpty(TVA_ETAT) ? decimal.Parse(TVA_ETAT) : 0;
                            Nouveau.REVENUE_TVA = !string.IsNullOrEmpty(REVENUE_TVA) ? decimal.Parse(REVENUE_TVA) : 0;
                            Nouveau.MONTANT_ETABLISSEMENTS_PUBLICS = !string.IsNullOrEmpty(MONTANT_ETABLISSEMENTS_PUBLICS) ? decimal.Parse(MONTANT_ETABLISSEMENTS_PUBLICS) : 0;
                            Nouveau.RETENUE_ETABLISSEMENT_PUBLICS = !string.IsNullOrEmpty(RETENUE_ETABLISSEMENT_PUBLICS) ? decimal.Parse(RETENUE_ETABLISSEMENT_PUBLICS) : 0;
                            Nouveau.MONTANT_MARCHES_ETRANGES = !string.IsNullOrEmpty(MONTANT_MARCHES_ETRANGES) ? decimal.Parse(MONTANT_MARCHES_ETRANGES) : 0;
                            Nouveau.RETENUE_MARCHES_ETRANGES = !string.IsNullOrEmpty(RETENUE_MARCHES_ETRANGES) ? decimal.Parse(RETENUE_MARCHES_ETRANGES) : 0;
                            Nouveau.REDEVANCE_CGC = !string.IsNullOrEmpty(REDEVANCE_CGC) ? decimal.Parse(REDEVANCE_CGC) : 0;
                            Nouveau.NET_SERVI = !string.IsNullOrEmpty(NET_SERVI) ? decimal.Parse(NET_SERVI) : 0;

                            ListeAnnexe5.Add(Nouveau);
                        }
                    }
                    Session["LISTE_ANNEXE_5_" + Session.SessionID] = ListeAnnexe5;
                }
                catch (Exception)
                {
                }
                finally
                {
                };
            }
            #endregion
            #region ANNEXE_6
            if (annexe == "ANNEXE_6")
            {
                List<ANNEXE_6> listeAnnexe6 = new List<ANNEXE_6>();
                try
                {
                    for (int iRow = 0; iRow < listTable.Rows.Count; iRow++)
                    {
                        string TYPE_BENEFICAIRE = listTable.Rows[iRow][0] != null ? Convert.ToString(listTable.Rows[iRow][0]) : "4";
                        string IDENTIFIANT = listTable.Rows[iRow][1] != null ? Convert.ToString(listTable.Rows[iRow][1]) : string.Empty;
                        string FULLNAME = listTable.Rows[iRow][2] != null ? Convert.ToString(listTable.Rows[iRow][2]) : string.Empty;
                        string ACTIVITE = listTable.Rows[iRow][3] != null ? Convert.ToString(listTable.Rows[iRow][3]) : string.Empty;
                        string ADRESSE = listTable.Rows[iRow][4] != null ? Convert.ToString(listTable.Rows[iRow][4]) : string.Empty;
                        string MONTANT_RISTOURNES = listTable.Rows[iRow][5] != null ? Convert.ToString(listTable.Rows[iRow][5]) : "1";
                        string MONTANT_VENTE_PP = listTable.Rows[iRow][6] != null ? Convert.ToString(listTable.Rows[iRow][6]) : "0";
                        string MONTANT_AVANCE_VENTE_PP = listTable.Rows[iRow][7] != null ? Convert.ToString(listTable.Rows[iRow][7]) : "0";
                        string MONTANT_ESPECES_MARCHANDISES = listTable.Rows[iRow][8] != null ? Convert.ToString(listTable.Rows[iRow][8]) : string.Empty;
                        if (!string.IsNullOrEmpty(IDENTIFIANT))
                        {
                            ANNEXE_6 Nouveau = new ANNEXE_6();
                            Nouveau.TYPE_BENEFICIAIRE = int.Parse(TYPE_BENEFICAIRE);
                            Nouveau.IDENTIFIANT = IDENTIFIANT;
                            Nouveau.NOM_PRENOM = FULLNAME;
                            Nouveau.ACTIVITE = ACTIVITE;
                            Nouveau.ADRESSE = ADRESSE;
                            Nouveau.MONTANT_RISTOURNES = !string.IsNullOrEmpty(MONTANT_RISTOURNES) ? decimal.Parse(MONTANT_RISTOURNES) : 0;
                            Nouveau.MONTANT_VENTE_PP = !string.IsNullOrEmpty(MONTANT_VENTE_PP) ? decimal.Parse(MONTANT_VENTE_PP) : 0;
                            Nouveau.MONTANT_AVANCE_VENTE_PP = !string.IsNullOrEmpty(MONTANT_AVANCE_VENTE_PP) ? decimal.Parse(MONTANT_AVANCE_VENTE_PP) : 0;
                            Nouveau.MONTANT_ESPECES_MARCHANDISES = !string.IsNullOrEmpty(MONTANT_ESPECES_MARCHANDISES) ? decimal.Parse(MONTANT_ESPECES_MARCHANDISES) : 0;
                            listeAnnexe6.Add(Nouveau);
                        }
                    }
                    Session["LISTE_ANNEXE_6_" + Session.SessionID] = listeAnnexe6;
                }
                catch (Exception)
                {
                }
                finally
                {
                };
            }
            #endregion
            #region ANNEXE_7
            if (annexe == "ANNEXE_7")
            {
                List<ANNEXE_7> listeAnnexe7 = new List<ANNEXE_7>();
                try
                {
                    for (int iRow = 0; iRow < listTable.Rows.Count; iRow++)
                    {
                        string TYPE_BENEFICAIRE = listTable.Rows[iRow][0] != null ? Convert.ToString(listTable.Rows[iRow][0]) : "4";
                        string IDENTIFIANT = listTable.Rows[iRow][1] != null ? Convert.ToString(listTable.Rows[iRow][1]) : string.Empty;
                        string FULLNAME = listTable.Rows[iRow][2] != null ? Convert.ToString(listTable.Rows[iRow][2]) : string.Empty;
                        string ACTIVITE = listTable.Rows[iRow][3] != null ? Convert.ToString(listTable.Rows[iRow][3]) : string.Empty;
                        string ADRESSE = listTable.Rows[iRow][4] != null ? Convert.ToString(listTable.Rows[iRow][4]) : string.Empty;
                        string TYPE_MONTANT = listTable.Rows[iRow][5] != null ? Convert.ToString(listTable.Rows[iRow][5]) : "1";
                        string MONTANT_PAYES = listTable.Rows[iRow][6] != null ? Convert.ToString(listTable.Rows[iRow][6]) : "0";
                        string RETENUE_SOURCE = listTable.Rows[iRow][7] != null ? Convert.ToString(listTable.Rows[iRow][7]) : "0";
                        string NET_SERVI = listTable.Rows[iRow][8] != null ? Convert.ToString(listTable.Rows[iRow][8]) : "0";
                        if (!string.IsNullOrEmpty(IDENTIFIANT))
                        {
                            ANNEXE_7 Nouveau = new ANNEXE_7();
                            Nouveau.TYPE_BENEFICIAIRE = int.Parse(TYPE_BENEFICAIRE);
                            Nouveau.IDENTIFIANT = IDENTIFIANT;
                            Nouveau.NOM_PRENOM = FULLNAME;
                            Nouveau.ACTIVITE = ACTIVITE;
                            Nouveau.ADRESSE = ADRESSE;
                            Nouveau.TYPE_MONTANT = !string.IsNullOrEmpty(TYPE_MONTANT) ? int.Parse(TYPE_MONTANT) : 0;
                            Nouveau.MONTANT_PAYES = !string.IsNullOrEmpty(MONTANT_PAYES) ? decimal.Parse(MONTANT_PAYES) : 0;
                            Nouveau.RETENUE_SOURCE = !string.IsNullOrEmpty(RETENUE_SOURCE) ? decimal.Parse(RETENUE_SOURCE) : 0;
                            Nouveau.NET_SERVI = !string.IsNullOrEmpty(NET_SERVI) ? decimal.Parse(NET_SERVI) : 0;
                            listeAnnexe7.Add(Nouveau);
                        }
                    }
                    Session["LISTE_ANNEXE_7_" + Session.SessionID] = listeAnnexe7;
                }
                catch (Exception)
                {
                }
                finally
                {
                };
            }
            #endregion
            #region RECAP
            if (annexe == "RECAP")
            {
                string Montant_Ligne1 = listTable.Rows[0][1] != null ? Convert.ToString(listTable.Rows[0][1]) : "0";
                string Retenu_Ligne1 = listTable.Rows[0][2] != null ? Convert.ToString(listTable.Rows[0][2]) : "0";

                string Montant_Ligne2 = listTable.Rows[1][1] != null ? Convert.ToString(listTable.Rows[1][1]) : "0";
                string Retenu_Ligne2 = listTable.Rows[1][2] != null ? Convert.ToString(listTable.Rows[1][2]) : "0";

                string Montant_Ligne3 = listTable.Rows[2][1] != null ? Convert.ToString(listTable.Rows[2][1]) : "0";
                string Retenu_Ligne3 = listTable.Rows[2][2] != null ? Convert.ToString(listTable.Rows[2][2]) : "0";

                string Montant_Ligne4 = listTable.Rows[3][1] != null ? Convert.ToString(listTable.Rows[3][1]) : "0";
                string Retenu_Ligne4 = listTable.Rows[3][2] != null ? Convert.ToString(listTable.Rows[3][2]) : "0";

                string Montant_Ligne5 = listTable.Rows[4][1] != null ? Convert.ToString(listTable.Rows[4][1]) : "0";
                string Retenu_Ligne5 = listTable.Rows[4][2] != null ? Convert.ToString(listTable.Rows[4][2]) : "0";

                string Montant_Ligne6 = listTable.Rows[5][1] != null ? Convert.ToString(listTable.Rows[5][1]) : "0";
                string Retenu_Ligne6 = listTable.Rows[5][2] != null ? Convert.ToString(listTable.Rows[5][2]) : "0";

                string Montant_Ligne7 = listTable.Rows[6][1] != null ? Convert.ToString(listTable.Rows[6][1]) : "0";
                string Retenu_Ligne7 = listTable.Rows[6][2] != null ? Convert.ToString(listTable.Rows[6][2]) : "0";

                string Montant_Ligne8 = listTable.Rows[7][1] != null ? Convert.ToString(listTable.Rows[7][1]) : "0";
                string Retenu_Ligne8 = listTable.Rows[7][2] != null ? Convert.ToString(listTable.Rows[7][2]) : "0";

                string Montant_Ligne9 = listTable.Rows[8][1] != null ? Convert.ToString(listTable.Rows[8][1]) : "0";
                string Retenu_Ligne9 = listTable.Rows[8][2] != null ? Convert.ToString(listTable.Rows[8][2]) : "0";

                string Montant_Ligne10 = listTable.Rows[9][1] != null ? Convert.ToString(listTable.Rows[9][1]) : "0";
                string Retenu_Ligne10 = listTable.Rows[9][2] != null ? Convert.ToString(listTable.Rows[9][2]) : "0";

                string Montant_Ligne11 = listTable.Rows[10][1] != null ? Convert.ToString(listTable.Rows[10][1]) : "0";
                string Retenu_Ligne11 = listTable.Rows[10][2] != null ? Convert.ToString(listTable.Rows[10][2]) : "0";

                string Montant_Ligne12 = listTable.Rows[11][1] != null ? Convert.ToString(listTable.Rows[11][1]) : "0";
                string Retenu_Ligne12 = listTable.Rows[11][2] != null ? Convert.ToString(listTable.Rows[11][2]) : "0";

                string Montant_Ligne13 = listTable.Rows[12][1] != null ? Convert.ToString(listTable.Rows[12][1]) : "0";
                string Retenu_Ligne13 = listTable.Rows[12][2] != null ? Convert.ToString(listTable.Rows[12][2]) : "0";

                string Montant_Ligne14 = listTable.Rows[13][1] != null ? Convert.ToString(listTable.Rows[13][1]) : "0";
                string Retenu_Ligne14 = listTable.Rows[13][2] != null ? Convert.ToString(listTable.Rows[13][2]) : "0";

                string Montant_Ligne15 = listTable.Rows[14][1] != null ? Convert.ToString(listTable.Rows[14][1]) : "0";
                string Retenu_Ligne15 = listTable.Rows[14][2] != null ? Convert.ToString(listTable.Rows[14][2]) : "0";

                string Montant_Ligne16 = listTable.Rows[15][1] != null ? Convert.ToString(listTable.Rows[15][1]) : "0";
                string Retenu_Ligne16 = listTable.Rows[15][2] != null ? Convert.ToString(listTable.Rows[15][2]) : "0";

                string Montant_Ligne17 = listTable.Rows[16][1] != null ? Convert.ToString(listTable.Rows[16][1]) : "0";
                string Retenu_Ligne17 = listTable.Rows[16][2] != null ? Convert.ToString(listTable.Rows[16][2]) : "0";

                string Montant_Ligne18 = listTable.Rows[17][1] != null ? Convert.ToString(listTable.Rows[17][1]) : "0";
                string Retenu_Ligne18 = listTable.Rows[17][2] != null ? Convert.ToString(listTable.Rows[17][2]) : "0";

                string Montant_Ligne19 = listTable.Rows[18][1] != null ? Convert.ToString(listTable.Rows[18][1]) : "0";
                string Retenu_Ligne19 = listTable.Rows[18][2] != null ? Convert.ToString(listTable.Rows[18][2]) : "0";

                string Montant_Ligne20 = listTable.Rows[19][1] != null ? Convert.ToString(listTable.Rows[19][1]) : "0";
                string Retenu_Ligne20 = listTable.Rows[19][2] != null ? Convert.ToString(listTable.Rows[19][2]) : "0";

                string Montant_Ligne21 = listTable.Rows[20][1] != null ? Convert.ToString(listTable.Rows[20][1]) : "0";
                string Retenu_Ligne21 = listTable.Rows[20][2] != null ? Convert.ToString(listTable.Rows[20][2]) : "0";

                string Montant_Ligne22 = listTable.Rows[21][1] != null ? Convert.ToString(listTable.Rows[21][1]) : "0";
                string Retenu_Ligne22 = listTable.Rows[21][2] != null ? Convert.ToString(listTable.Rows[21][2]) : "0";

                string Montant_Ligne23 = listTable.Rows[22][1] != null ? Convert.ToString(listTable.Rows[22][1]) : "0";
                string Retenu_Ligne23 = listTable.Rows[22][2] != null ? Convert.ToString(listTable.Rows[22][2]) : "0";

                string Montant_Ligne24 = listTable.Rows[23][1] != null ? Convert.ToString(listTable.Rows[23][1]) : "0";
                string Retenu_Ligne24 = listTable.Rows[23][2] != null ? Convert.ToString(listTable.Rows[23][2]) : "0";

                string Montant_Ligne25 = listTable.Rows[24][1] != null ? Convert.ToString(listTable.Rows[24][1]) : "0";
                string Retenu_Ligne25 = listTable.Rows[24][2] != null ? Convert.ToString(listTable.Rows[24][2]) : "0";

                string Montant_Ligne26 = listTable.Rows[25][1] != null ? Convert.ToString(listTable.Rows[25][1]) : "0";
                string Retenu_Ligne26 = listTable.Rows[25][2] != null ? Convert.ToString(listTable.Rows[25][2]) : "0";

                string Montant_Ligne27 = listTable.Rows[26][1] != null ? Convert.ToString(listTable.Rows[26][1]) : "0";
                string Retenu_Ligne27 = listTable.Rows[26][2] != null ? Convert.ToString(listTable.Rows[26][2]) : "0";

                string Montant_Ligne28 = listTable.Rows[27][1] != null ? Convert.ToString(listTable.Rows[27][1]) : "0";
                string Retenu_Ligne28 = listTable.Rows[27][2] != null ? Convert.ToString(listTable.Rows[27][2]) : "0";

                string Montant_Ligne29 = listTable.Rows[28][1] != null ? Convert.ToString(listTable.Rows[28][1]) : "0";
                string Retenu_Ligne29 = listTable.Rows[28][2] != null ? Convert.ToString(listTable.Rows[28][2]) : "0";

                string Montant_Ligne30 = listTable.Rows[29][1] != null ? Convert.ToString(listTable.Rows[29][1]) : "0";
                string Retenu_Ligne30 = listTable.Rows[29][2] != null ? Convert.ToString(listTable.Rows[29][2]) : "0";

                string Montant_Ligne31 = listTable.Rows[30][1] != null ? Convert.ToString(listTable.Rows[30][1]) : "0";
                string Retenu_Ligne31 = listTable.Rows[30][2] != null ? Convert.ToString(listTable.Rows[30][2]) : "0";

                string Montant_Ligne32 = listTable.Rows[31][1] != null ? Convert.ToString(listTable.Rows[31][1]) : "0";
                string Retenu_Ligne32 = listTable.Rows[31][2] != null ? Convert.ToString(listTable.Rows[31][2]) : "0";

                string Montant_Ligne33 = listTable.Rows[32][1] != null ? Convert.ToString(listTable.Rows[32][1]) : "0";
                string Retenu_Ligne33 = listTable.Rows[32][2] != null ? Convert.ToString(listTable.Rows[32][2]) : "0";

                string Montant_Ligne34 = listTable.Rows[33][1] != null ? Convert.ToString(listTable.Rows[33][1]) : "0";
                string Retenu_Ligne34 = listTable.Rows[33][2] != null ? Convert.ToString(listTable.Rows[33][2]) : "0";
                RECAPS RECAP = new RECAPS();
                RECAP.Montant_Ligne1 = !string.IsNullOrEmpty(Montant_Ligne1) ? decimal.Parse(Montant_Ligne1) : 0;
                RECAP.Retenu_Ligne1 = !string.IsNullOrEmpty(Retenu_Ligne1) ? decimal.Parse(Retenu_Ligne1) : 0;

                RECAP.Montant_Ligne2 = !string.IsNullOrEmpty(Montant_Ligne2) ? decimal.Parse(Montant_Ligne2) : 0;
                RECAP.Retenu_Ligne2 = !string.IsNullOrEmpty(Retenu_Ligne2) ? decimal.Parse(Retenu_Ligne2) : 0;

                RECAP.Montant_Ligne3 = !string.IsNullOrEmpty(Montant_Ligne3) ? decimal.Parse(Montant_Ligne3) : 0;
                RECAP.Retenu_Ligne3 = !string.IsNullOrEmpty(Retenu_Ligne3) ? decimal.Parse(Retenu_Ligne3) : 0;

                RECAP.Montant_Ligne4 = !string.IsNullOrEmpty(Montant_Ligne4) ? decimal.Parse(Montant_Ligne4) : 0;
                RECAP.Retenu_Ligne4 = !string.IsNullOrEmpty(Retenu_Ligne4) ? decimal.Parse(Retenu_Ligne4) : 0;

                RECAP.Montant_Ligne5 = !string.IsNullOrEmpty(Montant_Ligne5) ? decimal.Parse(Montant_Ligne5) : 0;
                RECAP.Retenu_Ligne5 = !string.IsNullOrEmpty(Retenu_Ligne5) ? decimal.Parse(Retenu_Ligne5) : 0;

                RECAP.Montant_Ligne6 = !string.IsNullOrEmpty(Montant_Ligne6) ? decimal.Parse(Montant_Ligne6) : 0;
                RECAP.Retenu_Ligne6 = !string.IsNullOrEmpty(Retenu_Ligne6) ? decimal.Parse(Retenu_Ligne6) : 0;

                RECAP.Montant_Ligne7 = !string.IsNullOrEmpty(Montant_Ligne7) ? decimal.Parse(Montant_Ligne7) : 0;
                RECAP.Retenu_Ligne7 = !string.IsNullOrEmpty(Retenu_Ligne7) ? decimal.Parse(Retenu_Ligne7) : 0;

                RECAP.Montant_Ligne8 = !string.IsNullOrEmpty(Montant_Ligne8) ? decimal.Parse(Montant_Ligne8) : 0;
                RECAP.Retenu_Ligne8 = !string.IsNullOrEmpty(Retenu_Ligne8) ? decimal.Parse(Retenu_Ligne8) : 0;

                RECAP.Montant_Ligne9 = !string.IsNullOrEmpty(Montant_Ligne9) ? decimal.Parse(Montant_Ligne9) : 0;
                RECAP.Retenu_Ligne9 = !string.IsNullOrEmpty(Retenu_Ligne9) ? decimal.Parse(Retenu_Ligne9) : 0;

                RECAP.Montant_Ligne10 = !string.IsNullOrEmpty(Montant_Ligne10) ? decimal.Parse(Montant_Ligne10) : 0;
                RECAP.Retenu_Ligne10 = !string.IsNullOrEmpty(Retenu_Ligne10) ? decimal.Parse(Retenu_Ligne10) : 0;

                RECAP.Montant_Ligne11 = !string.IsNullOrEmpty(Montant_Ligne11) ? decimal.Parse(Montant_Ligne11) : 0;
                RECAP.Retenu_Ligne11 = !string.IsNullOrEmpty(Retenu_Ligne11) ? decimal.Parse(Retenu_Ligne11) : 0;

                RECAP.Montant_Ligne12 = !string.IsNullOrEmpty(Montant_Ligne12) ? decimal.Parse(Montant_Ligne12) : 0;
                RECAP.Retenu_Ligne12 = !string.IsNullOrEmpty(Retenu_Ligne12) ? decimal.Parse(Retenu_Ligne12) : 0;

                RECAP.Montant_Ligne13 = !string.IsNullOrEmpty(Montant_Ligne13) ? decimal.Parse(Montant_Ligne13) : 0;
                RECAP.Retenu_Ligne13 = !string.IsNullOrEmpty(Retenu_Ligne13) ? decimal.Parse(Retenu_Ligne13) : 0;

                RECAP.Montant_Ligne14 = !string.IsNullOrEmpty(Montant_Ligne14) ? decimal.Parse(Montant_Ligne14) : 0;
                RECAP.Retenu_Ligne14 = !string.IsNullOrEmpty(Retenu_Ligne14) ? decimal.Parse(Retenu_Ligne14) : 0;

                RECAP.Montant_Ligne15 = !string.IsNullOrEmpty(Montant_Ligne15) ? decimal.Parse(Montant_Ligne15) : 0;
                RECAP.Retenu_Ligne15 = !string.IsNullOrEmpty(Retenu_Ligne15) ? decimal.Parse(Retenu_Ligne15) : 0;

                RECAP.Montant_Ligne16 = !string.IsNullOrEmpty(Montant_Ligne16) ? decimal.Parse(Montant_Ligne16) : 0;
                RECAP.Retenu_Ligne16 = !string.IsNullOrEmpty(Retenu_Ligne16) ? decimal.Parse(Retenu_Ligne16) : 0;

                RECAP.Montant_Ligne17 = !string.IsNullOrEmpty(Montant_Ligne17) ? decimal.Parse(Montant_Ligne17) : 0;
                RECAP.Retenu_Ligne17 = !string.IsNullOrEmpty(Retenu_Ligne17) ? decimal.Parse(Retenu_Ligne17) : 0;

                RECAP.Montant_Ligne18 = !string.IsNullOrEmpty(Montant_Ligne18) ? decimal.Parse(Montant_Ligne18) : 0;
                RECAP.Retenu_Ligne18 = !string.IsNullOrEmpty(Retenu_Ligne18) ? decimal.Parse(Retenu_Ligne18) : 0;

                RECAP.Montant_Ligne19 = !string.IsNullOrEmpty(Montant_Ligne19) ? decimal.Parse(Montant_Ligne19) : 0;
                RECAP.Retenu_Ligne19 = !string.IsNullOrEmpty(Retenu_Ligne19) ? decimal.Parse(Retenu_Ligne19) : 0;

                RECAP.Montant_Ligne20 = !string.IsNullOrEmpty(Montant_Ligne20) ? decimal.Parse(Montant_Ligne20) : 0;
                RECAP.Retenu_Ligne20 = !string.IsNullOrEmpty(Retenu_Ligne20) ? decimal.Parse(Retenu_Ligne20) : 0;

                RECAP.Montant_Ligne21 = !string.IsNullOrEmpty(Montant_Ligne21) ? decimal.Parse(Montant_Ligne21) : 0;
                RECAP.Retenu_Ligne21 = !string.IsNullOrEmpty(Retenu_Ligne21) ? decimal.Parse(Retenu_Ligne21) : 0;

                RECAP.Montant_Ligne22 = !string.IsNullOrEmpty(Montant_Ligne22) ? decimal.Parse(Montant_Ligne22) : 0;
                RECAP.Retenu_Ligne22 = !string.IsNullOrEmpty(Retenu_Ligne22) ? decimal.Parse(Retenu_Ligne22) : 0;

                RECAP.Montant_Ligne23 = !string.IsNullOrEmpty(Montant_Ligne23) ? decimal.Parse(Montant_Ligne23) : 0;
                RECAP.Retenu_Ligne23 = !string.IsNullOrEmpty(Retenu_Ligne23) ? decimal.Parse(Retenu_Ligne23) : 0;

                RECAP.Montant_Ligne24 = !string.IsNullOrEmpty(Montant_Ligne24) ? decimal.Parse(Montant_Ligne24) : 0;
                RECAP.Retenu_Ligne24 = !string.IsNullOrEmpty(Retenu_Ligne24) ? decimal.Parse(Retenu_Ligne24) : 0;

                RECAP.Montant_Ligne25 = !string.IsNullOrEmpty(Montant_Ligne25) ? decimal.Parse(Montant_Ligne25) : 0;
                RECAP.Retenu_Ligne25 = !string.IsNullOrEmpty(Retenu_Ligne25) ? decimal.Parse(Retenu_Ligne25) : 0;

                RECAP.Montant_Ligne26 = !string.IsNullOrEmpty(Montant_Ligne26) ? decimal.Parse(Montant_Ligne26) : 0;
                RECAP.Retenu_Ligne26 = !string.IsNullOrEmpty(Retenu_Ligne26) ? decimal.Parse(Retenu_Ligne26) : 0;

                RECAP.Montant_Ligne27 = !string.IsNullOrEmpty(Montant_Ligne27) ? decimal.Parse(Montant_Ligne27) : 0;
                RECAP.Retenu_Ligne27 = !string.IsNullOrEmpty(Retenu_Ligne27) ? decimal.Parse(Retenu_Ligne27) : 0;

                RECAP.Montant_Ligne28 = !string.IsNullOrEmpty(Montant_Ligne28) ? decimal.Parse(Montant_Ligne28) : 0;
                RECAP.Retenu_Ligne28 = !string.IsNullOrEmpty(Retenu_Ligne28) ? decimal.Parse(Retenu_Ligne28) : 0;

                RECAP.Montant_Ligne29 = !string.IsNullOrEmpty(Montant_Ligne29) ? decimal.Parse(Montant_Ligne29) : 0;
                RECAP.Retenu_Ligne29 = !string.IsNullOrEmpty(Retenu_Ligne29) ? decimal.Parse(Retenu_Ligne29) : 0;

                RECAP.Montant_Ligne30 = !string.IsNullOrEmpty(Montant_Ligne30) ? decimal.Parse(Montant_Ligne30) : 0;
                RECAP.Retenu_Ligne30 = !string.IsNullOrEmpty(Retenu_Ligne30) ? decimal.Parse(Retenu_Ligne30) : 0;

                RECAP.Montant_Ligne31 = !string.IsNullOrEmpty(Montant_Ligne31) ? decimal.Parse(Montant_Ligne31) : 0;
                RECAP.Retenu_Ligne31 = !string.IsNullOrEmpty(Retenu_Ligne31) ? decimal.Parse(Retenu_Ligne31) : 0;

                RECAP.Montant_Ligne32 = !string.IsNullOrEmpty(Montant_Ligne32) ? decimal.Parse(Montant_Ligne32) : 0;
                RECAP.Retenu_Ligne32 = !string.IsNullOrEmpty(Retenu_Ligne32) ? decimal.Parse(Retenu_Ligne32) : 0;

                RECAP.Montant_Ligne33 = !string.IsNullOrEmpty(Montant_Ligne33) ? decimal.Parse(Montant_Ligne33) : 0;
                RECAP.Retenu_Ligne33 = !string.IsNullOrEmpty(Retenu_Ligne33) ? decimal.Parse(Retenu_Ligne33) : 0;

                RECAP.Montant_Ligne34 = !string.IsNullOrEmpty(Montant_Ligne34) ? decimal.Parse(Montant_Ligne34) : 0;
                RECAP.Retenu_Ligne34 = !string.IsNullOrEmpty(Retenu_Ligne34) ? decimal.Parse(Retenu_Ligne34) : 0;
                Session["RECAP" + Session.SessionID] = RECAP;
            }
            #endregion
        }
    }
    public class RECAPS
    {
        public decimal Montant_Ligne1;
        public decimal Retenu_Ligne1;

        public decimal Montant_Ligne2;
        public decimal Retenu_Ligne2;

        public decimal Montant_Ligne3;
        public decimal Retenu_Ligne3;

        public decimal Montant_Ligne4;
        public decimal Retenu_Ligne4;

        public decimal Montant_Ligne5;
        public decimal Retenu_Ligne5;

        public decimal Montant_Ligne6;
        public decimal Retenu_Ligne6;

        public decimal Montant_Ligne7;
        public decimal Retenu_Ligne7;

        public decimal Montant_Ligne8;
        public decimal Retenu_Ligne8;

        public decimal Montant_Ligne9;
        public decimal Retenu_Ligne9;

        public decimal Montant_Ligne10;
        public decimal Retenu_Ligne10;

        public decimal Montant_Ligne11;
        public decimal Retenu_Ligne11;

        public decimal Montant_Ligne12;
        public decimal Retenu_Ligne12;

        public decimal Montant_Ligne13;
        public decimal Retenu_Ligne13;

        public decimal Montant_Ligne14;
        public decimal Retenu_Ligne14;

        public decimal Montant_Ligne15;
        public decimal Retenu_Ligne15;

        public decimal Montant_Ligne16;
        public decimal Retenu_Ligne16;

        public decimal Montant_Ligne17;
        public decimal Retenu_Ligne17;

        public decimal Montant_Ligne18;
        public decimal Retenu_Ligne18;

        public decimal Montant_Ligne19;
        public decimal Retenu_Ligne19;

        public decimal Montant_Ligne20;
        public decimal Retenu_Ligne20;

        public decimal Montant_Ligne21;
        public decimal Retenu_Ligne21;

        public decimal Montant_Ligne22;
        public decimal Retenu_Ligne22;

        public decimal Montant_Ligne23;
        public decimal Retenu_Ligne23;

        public decimal Montant_Ligne24;
        public decimal Retenu_Ligne24;

        public decimal Montant_Ligne25;
        public decimal Retenu_Ligne25;

        public decimal Montant_Ligne26;
        public decimal Retenu_Ligne26;

        public decimal Montant_Ligne27;
        public decimal Retenu_Ligne27;

        public decimal Montant_Ligne28;
        public decimal Retenu_Ligne28;

        public decimal Montant_Ligne29;
        public decimal Retenu_Ligne29;

        public decimal Montant_Ligne30;
        public decimal Retenu_Ligne30;

        public decimal Montant_Ligne31;
        public decimal Retenu_Ligne31;

        public decimal Montant_Ligne32;
        public decimal Retenu_Ligne32;

        public decimal Montant_Ligne33;
        public decimal Retenu_Ligne33;

        public decimal Montant_Ligne34;
        public decimal Retenu_Ligne34;
        public RECAPS()
        { }
    }
}
