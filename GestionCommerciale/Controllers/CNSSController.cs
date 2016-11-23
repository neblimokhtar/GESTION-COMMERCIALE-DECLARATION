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

namespace GestionCommerciale.Controllers
{
    public class CNSSController : Controller
    {
        //
        // GET: /CNSS/
        GestionCommercialeEntity BD = new GestionCommercialeEntity();
        List<CnssInfo> Liste = new List<CnssInfo>();
        public ActionResult Index()
        {
            List<DECLARATIONS> Liste = BD.DECLARATIONS.ToList();
            return View(Liste);
        }
        public ActionResult Societes()
        {
            List<DECLARATIONS> Liste = BD.DECLARATIONS.ToList();
            return View(Liste);
        }
        public ActionResult Saisie()
        {
            List<DECLARATIONS> Liste = BD.DECLARATIONS.ToList();
            return View(Liste);
        }
        public ActionResult Employee(string Code)
        {
            int ID = int.Parse(Code);
            List<EMPLOYEES> Liste = BD.EMPLOYEES.Where(Emp => Emp.SOCIETES.ID == ID).ToList();
            string SOCIETE = string.Empty;
            SOCIETE = BD.DECLARATIONS.Where(SOC => SOC.ID == ID).FirstOrDefault().SOCIETE;
            ViewBag.SOCIETE = SOCIETE;
            ViewBag.CODE_SOCIETE = Code;
            return View(Liste);
        }
        public ActionResult All()
        {
            List<GENERATIONS> Liste = BD.GENERATIONS.ToList();
            return View(Liste);
        }
        public ActionResult DetailDeclaration(int Code)
        {
            GENERATIONS Declaration = BD.GENERATIONS.Find(Code);
            ViewBag.DECLARATION = Declaration.CODE;
            ViewBag.TRIMESTRE = Declaration.TRIMESTRE;
            ViewBag.ANNEE = Declaration.ANNEE;
            ViewBag.CODE = Code;
            List<LIGNES_GENERATIONS> Liste = BD.LIGNES_GENERATIONS.Where(Ligne => Ligne.GENERATIONS.ID == Code).ToList();
            return View(Liste);
        }
        public ActionResult Historique()
        {
            return View();
        }
        public ActionResult HistoriqueEmployee(string CIN)
        {
            List<LIGNES_GENERATIONS> Liste = BD.LIGNES_GENERATIONS.Where(Ligne => Ligne.EMPLOYEES.CIN == CIN).ToList();
            var Result = (from e in Liste
                          select new
                          {
                              ID = e.ID,
                              SOCIETE = e.GENERATIONS.DECLARATIONS.SOCIETE,
                              CODE = e.GENERATIONS.CODE,
                              TRIMESTRE = e.GENERATIONS.TRIMESTRE,
                              ANNEE = e.GENERATIONS.ANNEE,
                              SALAIRE1 = e.SALAIRE_MOIS_1,
                              SALAIRE2 = e.SALAIRE_MOIS_2,
                              SALAIRE3 = e.SALAIRE_MOIS_3
                          }).AsEnumerable().Select(c => c.ToExpando());
            return PartialView("HistoriqueEmployee", Result);
        }
        public ActionResult GetFileByID(int id)
        {
            var fileToRetrieve = BD.GENERATIONS.Find(id);
            return File(fileToRetrieve.DATA, System.Net.Mime.MediaTypeNames.Application.Octet, fileToRetrieve.CODE + ".TXT");
        }
        public ActionResult GetSalaryByFilter(string Societe, string Annee, string Mois)
        {
            int SOCIETE = int.Parse(Societe);
            int ANNEE = int.Parse(Annee);
            int MOIS = int.Parse(Mois);
            List<EMPLOYEES> ListeEmployee = BD.EMPLOYEES.Where(Emp => Emp.SOCIETES.ID == SOCIETE && Emp.ACTIF == true).ToList();
            foreach (EMPLOYEES Employee in ListeEmployee)
            {
                SAISIES UneSaisie = BD.SAISIES.Where(Sais => Sais.SOCIETES.ID == SOCIETE && Sais.ANNEE == ANNEE && Sais.MOIS == MOIS && Sais.EMPLOYEES.ID == Employee.ID).FirstOrDefault();
                if (UneSaisie == null)
                {
                    SAISIES NouvelleSaisie = new SAISIES();
                    NouvelleSaisie.ANNEE = ANNEE;
                    NouvelleSaisie.EMPLOYEE = Employee.ID;
                    NouvelleSaisie.EMPLOYEES = Employee;
                    NouvelleSaisie.MOIS = MOIS;
                    NouvelleSaisie.SALAIRE = 0;
                    if (MOIS >= 1 && MOIS < 4) NouvelleSaisie.TRIMESTRE = 1;
                    if (MOIS >= 4 && MOIS < 7) NouvelleSaisie.TRIMESTRE = 2;
                    if (MOIS >= 7 && MOIS < 10) NouvelleSaisie.TRIMESTRE = 3;
                    if (MOIS >= 10) NouvelleSaisie.TRIMESTRE = 4;
                    NouvelleSaisie.SOCIETE = SOCIETE;
                    NouvelleSaisie.SOCIETES = BD.DECLARATIONS.Where(Dec => Dec.ID == SOCIETE).FirstOrDefault();
                    BD.SAISIES.Add(NouvelleSaisie);
                    BD.SaveChanges();
                }
            }
            List<SAISIES> Liste = BD.SAISIES.Where(Sais => Sais.SOCIETES.ID == SOCIETE && Sais.ANNEE == ANNEE && Sais.MOIS == MOIS).ToList();
            var Result = (from e in Liste
                          select new
                          {
                              ID = e.ID,
                              CIN = e.EMPLOYEES.CIN,
                              FULLNAME = e.EMPLOYEES.FULLNAME,
                              MOIS = Mois,
                              ANNEE = Annee,
                              SALAIRE = e.SALAIRE
                          }).AsEnumerable().Select(c => c.ToExpando());
            return PartialView("AfficherSalaire", Result);
        }
        public string UpdateData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            SAISIES UneSaisie = BD.SAISIES.Where(Saisie => Saisie.ID == id).FirstOrDefault();
            value = value.Trim();
            double AncienSalaire = (double)UneSaisie.SALAIRE;
            double Valeur;
            if (double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out Valeur))
            {
                double NouveauSalaire = double.Parse(value, CultureInfo.InvariantCulture);
                NouveauSalaire = Convert.ToDouble(NouveauSalaire.ToString("F3"));
                UneSaisie.SALAIRE = (double)NouveauSalaire;
                BD.SaveChanges();
            }
            return value;
        }
        public ActionResult FormSociete(string Mode, string Code)
        {
            DECLARATIONS Societe = new DECLARATIONS();
            if (Mode == "Create")
            {
                ViewBag.Type = "Nouvelle Société";
            }
            if (Mode == "Edit")
            {
                ViewBag.Type = "Modifier une Société";
                int ID = int.Parse(Code);
                Societe = BD.DECLARATIONS.Where(Soc => Soc.ID == ID).FirstOrDefault();
            }
            ViewBag.Code = Code;
            ViewBag.Mode = Mode;
            return View(Societe);
        }
        public ActionResult FormEmployee(string Mode, string Code, string Societe)
        {
            EMPLOYEES Employee = new EMPLOYEES();
            if (Mode == "Create")
            {
                ViewBag.Type = "Nouveau Employee";
                ViewBag.date = DateTime.Today.ToShortDateString();
            }
            if (Mode == "Edit")
            {
                ViewBag.Type = "Modifier un Employee";
                int ID = int.Parse(Code);
                Employee = BD.EMPLOYEES.Where(emp => emp.ID == ID).FirstOrDefault();
                ViewBag.date = Employee.DEMARRAGE != null ? Employee.DEMARRAGE.Value.ToShortDateString() : DateTime.Today.ToShortDateString();
            }
            ViewBag.Code = Code;
            ViewBag.Mode = Mode;
            ViewBag.Societe = Societe;
            int ID_SOCIETE = int.Parse(Societe);
            string SOCIETE = BD.DECLARATIONS.Where(Soc => Soc.ID == ID_SOCIETE).FirstOrDefault().SOCIETE;
            ViewBag.SOCIETE_NAME = SOCIETE;

            return View(Employee);
        }
        [HttpPost]
        public ActionResult SendFile(string Filter)
        {
            HttpPostedFileBase FILE = Request.Files["FILE"];
            if (FILE != null && FILE.ContentLength > 0)
            {
                string path = Path.Combine(Server.MapPath("~/Images/"), FILE.FileName);
                FILE.SaveAs(path);
                Uploadfile(FILE.InputStream, path, Filter);
            }
            return RedirectToAction("Employee", "CNSS", new { @Code = Filter });
        }
        public void Uploadfile(Stream fStream, string fileName, string Filter)
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
                    InsertIntoList(fStream, dt, Filter);
                }
            }
        }
        private void InsertIntoList(Stream fStream, DataTable listTable, string Filter)
        {
            try
            {
                for (int iRow = 0; iRow < listTable.Rows.Count; iRow++)
                {
                    string MATRICULE = listTable.Rows[iRow][0] != null ? Convert.ToString(listTable.Rows[iRow][0]) : "";
                    string SALAIRE = listTable.Rows[iRow][1] != null ? Convert.ToString(listTable.Rows[iRow][1]) : "";
                    string NOM = listTable.Rows[iRow][2] != null ? Convert.ToString(listTable.Rows[iRow][2]) : "";
                    string PRENOM = listTable.Rows[iRow][3] != null ? Convert.ToString(listTable.Rows[iRow][3]) : "";
                    string CIN = listTable.Rows[iRow][4] != null ? Convert.ToString(listTable.Rows[iRow][4]) : "";
                    string QUALIFICATION = listTable.Rows[iRow][5] != null ? Convert.ToString(listTable.Rows[iRow][5]) : "";
                    EMPLOYEES SelectedEmploye = BD.EMPLOYEES.Where(Element => Element.NUMERO == MATRICULE).FirstOrDefault();
                    while (CIN.Length < 8)
                    {
                        CIN = "0" + CIN;
                    }
                    if (SelectedEmploye == null)
                    {
                        SelectedEmploye = new EMPLOYEES();
                        SelectedEmploye.NUMERO = MATRICULE;
                        SelectedEmploye.FULLNAME = PRENOM + " " + NOM;
                        SelectedEmploye.CIN = CIN;
                        SelectedEmploye.QUALIFICATION = QUALIFICATION;
                        SelectedEmploye.SALAIRE = 0;
                        decimal value;
                        if (decimal.TryParse(SALAIRE, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                        {
                            SelectedEmploye.SALAIRE = decimal.Parse(SALAIRE);
                        }
                        SelectedEmploye.SOCIETE = int.Parse(Filter);
                        int ID = int.Parse(Filter);
                        DECLARATIONS SOCIETE = BD.DECLARATIONS.Where(Soc => Soc.ID == ID).FirstOrDefault();
                        SelectedEmploye.SOCIETES = SOCIETE;
                        SelectedEmploye.ACTIF = true;
                        BD.EMPLOYEES.Add(SelectedEmploye);
                        BD.SaveChanges();
                    }
                    else
                    {
                        SelectedEmploye.NUMERO = MATRICULE;
                        SelectedEmploye.FULLNAME = PRENOM + " " + NOM;
                        SelectedEmploye.CIN = CIN;
                        SelectedEmploye.QUALIFICATION = QUALIFICATION;
                        SelectedEmploye.SALAIRE = 0;
                        decimal value;
                        if (decimal.TryParse(SALAIRE, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                        {
                            SelectedEmploye.SALAIRE = decimal.Parse(SALAIRE);
                        }
                        SelectedEmploye.SOCIETE = int.Parse(Filter);
                        int ID = int.Parse(Filter);
                        DECLARATIONS SOCIETE = BD.DECLARATIONS.Where(Soc => Soc.ID == ID).FirstOrDefault();
                        SelectedEmploye.SOCIETES = SOCIETE;
                        SelectedEmploye.ACTIF = true;
                        BD.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
            };
        }
        public ActionResult ImporterEmployee(string Societe)
        {
            ViewBag.Societe = Societe;
            int ID_SOCIETE = int.Parse(Societe);
            string SOCIETE = BD.DECLARATIONS.Where(Soc => Soc.ID == ID_SOCIETE).FirstOrDefault().SOCIETE;
            ViewBag.SOCIETE_NAME = SOCIETE;
            return View();
        }
        public ActionResult DeleteEmp(string Code, string Filter)
        {
            int ID = int.Parse(Code);
            EMPLOYEES Employee = BD.EMPLOYEES.Where(emp => emp.ID == ID).FirstOrDefault();
            BD.EMPLOYEES.Remove(Employee);
            BD.SaveChanges();
            return RedirectToAction("Employee", "CNSS", new { @Code = Filter });
        }
        [HttpPost]
        public ActionResult SendFormEmployee(string Mode, string Code, string Filter)
        {
            string CIN = Request.Params["CIN"] != null ? Request.Params["CIN"].ToString() : string.Empty;
            string FULLNAME = Request.Params["FULLNAME"] != null ? Request.Params["FULLNAME"].ToString() : string.Empty;
            string NUM_ASS_SOC = Request.Params["NUM_ASS_SOC"] != null ? Request.Params["NUM_ASS_SOC"].ToString() : string.Empty;
            string ACTIF = Request.Params["ACTIF"] != null ? "true" : "false";
            string QUALIFICATION = Request.Params["QUALIFICATION"] != null ? Request.Params["QUALIFICATION"].ToString() : string.Empty;
            string NUMERO = Request.Params["NUMERO"] != null ? Request.Params["NUMERO"].ToString() : string.Empty;

            string DATE = Request.Params["DATE"] != null ? Request.Params["DATE"].ToString() : string.Empty;
            string SALAIRE = Request.Params["SALAIRE"] != null ? Request.Params["SALAIRE"].ToString() : string.Empty;
            string CIVILITE = Request.Params["CIVILITE"] != null ? Request.Params["CIVILITE"].ToString() : string.Empty;

            Boolean Etat = Boolean.Parse(ACTIF);
            if (Mode == "Create")
            {
                EMPLOYEES Employee = new EMPLOYEES();
                Employee.CIN = CIN.Trim();
                Employee.FULLNAME = FULLNAME;
                Employee.NUM_ASS_SOC = NUM_ASS_SOC;
                Employee.SOCIETE = int.Parse(Filter);
                int ID = int.Parse(Filter);
                DECLARATIONS SOCIETE = BD.DECLARATIONS.Where(Soc => Soc.ID == ID).FirstOrDefault();
                Employee.SOCIETES = SOCIETE;
                Employee.ACTIF = Etat;
                Employee.NUMERO = NUMERO;
                Employee.QUALIFICATION = QUALIFICATION;
                Employee.DEMARRAGE = DateTime.Parse(DATE);
                Employee.CIVILITE = CIVILITE;
                Employee.SALAIRE = 0;
                decimal value;
                if (decimal.TryParse(SALAIRE, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                {
                    Employee.SALAIRE = decimal.Parse(SALAIRE, CultureInfo.InvariantCulture);
                }
                BD.EMPLOYEES.Add(Employee);
                BD.SaveChanges();
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                EMPLOYEES Employee = BD.EMPLOYEES.Where(Emp => Emp.ID == ID).FirstOrDefault();
                Employee.CIN = CIN.Trim();
                Employee.FULLNAME = FULLNAME;
                Employee.NUM_ASS_SOC = NUM_ASS_SOC;
                Employee.ACTIF = Etat;
                Employee.NUMERO = NUMERO;
                Employee.QUALIFICATION = QUALIFICATION;
                Employee.DEMARRAGE = DateTime.Parse(DATE);
                Employee.CIVILITE = CIVILITE;
                Employee.SALAIRE = 0;
                decimal value;
                if (decimal.TryParse(SALAIRE, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                {
                    Employee.SALAIRE = decimal.Parse(SALAIRE, CultureInfo.InvariantCulture);
                }
                BD.SaveChanges();
            }
            return RedirectToAction("Employee", "CNSS", new { @Code = Filter });
        }
        [HttpPost]
        public ActionResult SendFormSociete(string Mode, string Code)
        {
            ViewBag.MODE = Mode;

            string SOCIETE = Request.Params["SOCIETE"] != null ? Request.Params["SOCIETE"].ToString() : string.Empty;
            string MATRICULE = Request.Params["MATRICULE"] != null ? Request.Params["MATRICULE"].ToString() : string.Empty;
            string ADRESSE = string.Empty;
            string MATRICULE_FISCAL = Request.Params["MATRICULE_FISCAL"] != null ? Request.Params["MATRICULE_FISCAL"].ToString() : string.Empty;
            string CODE_EXPLOITATION = Request.Params["CODE_EXPLOITATION"] != null ? Request.Params["CODE_EXPLOITATION"].ToString() : string.Empty;
            string BR = Request.Params["BR"] != null ? Request.Params["BR"].ToString() : string.Empty;
            string VILLE = Request.Params["VILLE"] != null ? Request.Params["VILLE"].ToString() : string.Empty;
            string RUE = Request.Params["RUE"] != null ? Request.Params["RUE"].ToString() : string.Empty;
            string NUMERO = Request.Params["NUMERO"] != null ? Request.Params["NUMERO"].ToString() : string.Empty;
            string CODE_POSTAL = Request.Params["CODE_POSTAL"] != null ? Request.Params["CODE_POSTAL"].ToString() : string.Empty;
            string ACTIVITE = Request.Params["ACTIVITE"] != null ? Request.Params["ACTIVITE"].ToString() : string.Empty;
            MATRICULE_FISCAL = MATRICULE_FISCAL.Replace("-", "");
            ADRESSE = VILLE + " " + RUE + " " + NUMERO + " " + CODE_POSTAL;
            if (Mode == "Create")
            {
                DECLARATIONS SOC = new DECLARATIONS();
                SOC.SOCIETE = SOCIETE;
                SOC.CODE = MATRICULE;
                SOC.MATRICULE = MATRICULE_FISCAL;
                SOC.CODE_EXPLOITATION = CODE_EXPLOITATION;
                SOC.ADRESSE = ADRESSE;
                SOC.BR = BR;
                SOC.ACTIVITE = ACTIVITE;
                SOC.VILLE = VILLE;
                SOC.RUE = RUE;
                SOC.NUMERO = NUMERO;
                SOC.CODE_POSTAL = CODE_POSTAL;
                BD.DECLARATIONS.Add(SOC);
                BD.SaveChanges();
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                DECLARATIONS SOC = BD.DECLARATIONS.Where(soc => soc.ID == ID).FirstOrDefault();
                SOC.SOCIETE = SOCIETE;
                SOC.CODE = MATRICULE;
                SOC.MATRICULE = MATRICULE_FISCAL;
                SOC.CODE_EXPLOITATION = CODE_EXPLOITATION;
                SOC.ADRESSE = ADRESSE;
                SOC.BR = BR;
                SOC.ACTIVITE = ACTIVITE;
                SOC.VILLE = VILLE;
                SOC.RUE = RUE;
                SOC.NUMERO = NUMERO;
                SOC.CODE_POSTAL = CODE_POSTAL;
                BD.SaveChanges();
            }
            return RedirectToAction("Societes");
        }
        [HttpPost]
        public FileResult Upload()
        {
            FileInfo info = new FileInfo("Fichier vide.TXT");

            string trimestre = Request.Params["TRIMESTRE"] != null ? Request.Params["TRIMESTRE"].ToString() : string.Empty;
            string annee = Request.Params["ANNEE"] != null ? Request.Params["ANNEE"].ToString() : string.Empty;
            string societe = Request.Params["SOCIETE"] != null ? Request.Params["SOCIETE"].ToString() : string.Empty;

            string WithSave = Request["WithSave"] != null ? Request["WithSave"].ToString() : "false";
            if (string.IsNullOrEmpty(WithSave)) WithSave = "false";
            Boolean Save = Boolean.Parse(WithSave);

            int ID = int.Parse(societe);
            int TRIMESTRE = int.Parse(trimestre);
            int ANNEE = int.Parse(annee);
            DECLARATIONS SelectedSociete = BD.DECLARATIONS.Where(Soc => Soc.ID == ID).FirstOrDefault();
            societe = SelectedSociete.CODE;
            var nom = "DS" + societe + "0000." + trimestre + annee + ".TXT";
            GENERATIONS NouvelleGeneration = new GENERATIONS();
            if (Save)
            {
                NouvelleGeneration.ANNEE = ANNEE;
                NouvelleGeneration.CODE = "DS" + societe + "0000." + trimestre + annee;
                NouvelleGeneration.DATE = DateTime.Today;
                NouvelleGeneration.DECLARATIONS = SelectedSociete;
                NouvelleGeneration.SOCIETE = ID;
                NouvelleGeneration.TRIMESTRE = TRIMESTRE;
                BD.GENERATIONS.Add(NouvelleGeneration);
                BD.SaveChanges();
            }
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase fichierExcel = Request.Files["FileUploadExcel"];
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);

                    //var path = Path.Combine(Server.MapPath("~/Images/"), nom + ".TXT");
                    var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    file.SaveAs(path);
                    var fileContents = System.IO.File.ReadAllText(Server.MapPath(@"~/Images/" + fileName));
                    string[] lignes = fileContents.Split('\n');
                    info = new FileInfo(Server.MapPath(@"~/Images/" + nom));
                    var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fileStream.Close();
                    //using (var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    //{
                    using (StreamWriter writer = info.CreateText())
                    {
                        int LineNumber = 0;
                        int FileNumber = 1;
                        for (int i = 0; i < lignes.Count() - 1; i++)
                        {
                            if (LineNumber == 12)
                            {
                                LineNumber = 1;
                                FileNumber++;
                            }
                            else
                                LineNumber++;
                            string NumAssureSociel = lignes[i].Substring(10, 10);
                            string IdentiteAssuree = lignes[i].Substring(20, 60);
                            string Cin = lignes[i].Substring(80, 8);
                            string Salaire = lignes[i].Substring(lignes[i].Length - 11, 10);
                            Salaire = Salaire.Replace(",", "");
                            Salaire = Salaire.Replace(" ", "0");
                            while (Salaire.Length < 10)
                            {
                                Salaire = "0" + Salaire;
                            }
                            writer.WriteLine(societe + "0000" + trimestre + annee + FileNumber.ToString("000") + LineNumber.ToString("00") + NumAssureSociel + IdentiteAssuree + Cin + Salaire + "0000000000");
                            string SalaireMois1 = lignes[i].Substring(91, 12);
                            string SalaireMois2 = lignes[i].Substring(103, 12);
                            string SalaireMois3 = lignes[i].Substring(115, 12);
                            SalaireMois1 = SalaireMois1.Trim();
                            SalaireMois2 = SalaireMois2.Trim();
                            SalaireMois3 = SalaireMois3.Trim();
                            if (Save)
                            {
                                EMPLOYEES Employee = BD.EMPLOYEES.Where(Emp => Emp.CIN == Cin).FirstOrDefault();
                                if (Employee != null)
                                {
                                    LIGNES_GENERATIONS NouvelleLigne = new LIGNES_GENERATIONS();
                                    NouvelleLigne.EMPLOYEE = Employee.ID;
                                    NouvelleLigne.EMPLOYEES = Employee;
                                    NouvelleLigne.GENERATION = NouvelleGeneration.ID;
                                    NouvelleLigne.GENERATIONS = NouvelleGeneration;
                                    NouvelleLigne.SALAIRE_MOIS_1 = double.Parse(SalaireMois1);
                                    NouvelleLigne.SALAIRE_MOIS_2 = double.Parse(SalaireMois2);
                                    NouvelleLigne.SALAIRE_MOIS_3 = double.Parse(SalaireMois3);
                                    BD.LIGNES_GENERATIONS.Add(NouvelleLigne);
                                    BD.SaveChanges();
                                }
                                else
                                {
                                    EMPLOYEES NouveauEmployee = new EMPLOYEES();
                                    NouveauEmployee.FULLNAME = IdentiteAssuree.TrimEnd();
                                    NouveauEmployee.CIN = Cin;
                                    NouveauEmployee.ACTIF = true;
                                    NumAssureSociel = NumAssureSociel.Insert(8, "-");
                                    NouveauEmployee.NUM_ASS_SOC = NumAssureSociel;
                                    NouveauEmployee.SOCIETE = ID;
                                    NouveauEmployee.SOCIETES = SelectedSociete;
                                    BD.EMPLOYEES.Add(NouveauEmployee);
                                    BD.SaveChanges();
                                    LIGNES_GENERATIONS NouvelleLigne = new LIGNES_GENERATIONS();
                                    NouvelleLigne.EMPLOYEE = NouveauEmployee.ID;
                                    NouvelleLigne.EMPLOYEES = NouveauEmployee;
                                    NouvelleLigne.GENERATION = NouvelleGeneration.ID;
                                    NouvelleLigne.GENERATIONS = NouvelleGeneration;
                                    NouvelleLigne.SALAIRE_MOIS_1 = double.Parse(SalaireMois1);
                                    NouvelleLigne.SALAIRE_MOIS_2 = double.Parse(SalaireMois2);
                                    NouvelleLigne.SALAIRE_MOIS_3 = double.Parse(SalaireMois3);
                                    BD.LIGNES_GENERATIONS.Add(NouvelleLigne);
                                    BD.SaveChanges();
                                }

                            }
                        }
                    }
                    // }
                }

                if (fichierExcel != null && fichierExcel.ContentLength > 0)
                {
                    string path = Path.Combine(Server.MapPath("~/Images/"), fichierExcel.FileName);
                    fichierExcel.SaveAs(path);
                    info = new FileInfo(Server.MapPath(@"~/Images/" + nom));
                    Uploadfile(fichierExcel.InputStream, path, societe, trimestre, annee);
                    var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fileStream.Close();
                    //using (var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    //{
                    using (StreamWriter writer = info.CreateText())
                    {
                        foreach (CnssInfo cnss in Liste)
                        {
                            writer.WriteLine(societe + "0000" + trimestre + annee + cnss.page.ToString("000") + cnss.Line.ToString("00") + cnss.NumSoc + cnss.FullName + cnss.Cin + cnss.Salaire + "0000000000");
                        }
                    }
                    //}
                }
                if (file.ContentLength <= 0 && fichierExcel.ContentLength <= 0)
                {
                    var fileStream = System.IO.File.Open(Server.MapPath(@"~/Images/" + nom), FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fileStream.Close();
                    info = new FileInfo(Server.MapPath(@"~/Images/" + nom));
                    List<EMPLOYEES> Liste = BD.EMPLOYEES.Where(Emp => Emp.SOCIETES.ID == ID && Emp.ACTIF == true).ToList();
                    using (StreamWriter writer = info.CreateText())
                    {
                        int LineNumber = 0;
                        int FileNumber = 1;
                        foreach (EMPLOYEES Employee in Liste)
                        {
                            double SalaireMois1 = 0;
                            double SalaireMois2 = 0;
                            double SalaireMois3 = 0;
                            List<SAISIES> ListeSaisie = BD.SAISIES.Where(Saisie => Saisie.EMPLOYEES.ID == Employee.ID && Saisie.ANNEE == ANNEE && Saisie.TRIMESTRE == TRIMESTRE).ToList();
                            if (ListeSaisie.Count == 1)
                            {
                                SalaireMois1 = (double)ListeSaisie.ElementAt(0).SALAIRE;
                            }
                            if (ListeSaisie.Count == 2)
                            {
                                SalaireMois1 = (double)ListeSaisie.ElementAt(0).SALAIRE;
                                SalaireMois2 = (double)ListeSaisie.ElementAt(1).SALAIRE;
                            }
                            if (ListeSaisie.Count == 3)
                            {
                                SalaireMois1 = (double)ListeSaisie.ElementAt(0).SALAIRE;
                                SalaireMois2 = (double)ListeSaisie.ElementAt(1).SALAIRE;
                                SalaireMois3 = (double)ListeSaisie.ElementAt(2).SALAIRE;
                            }
                            double Salaire = 0;
                            foreach (SAISIES UneSaisie in ListeSaisie)
                            {
                                Salaire += (double)UneSaisie.SALAIRE;
                            }
                            if (LineNumber == 12)
                            {
                                LineNumber = 1;
                                FileNumber++;
                            }
                            else
                                LineNumber++;
                            string fullname = Employee.FULLNAME;
                            while (fullname.Length < 60)
                                fullname += " ";
                            string SalaireString = Salaire.ToString("F3");
                            SalaireString = SalaireString.Replace(",", "");
                            SalaireString = SalaireString.Replace(" ", "0");
                            while (SalaireString.Length < 10)
                            {
                                SalaireString = "0" + SalaireString;
                            }
                            writer.WriteLine(societe + "0000" + trimestre + annee + FileNumber.ToString("000") + LineNumber.ToString("00") + Employee.NUM_ASS_SOC.Replace("-", "") + fullname + Employee.CIN + SalaireString + "0000000000");
                            if (Save)
                            {
                                EMPLOYEES Employeee = BD.EMPLOYEES.Where(Emp => Emp.CIN == Employee.CIN).FirstOrDefault();
                                if (Employeee != null)
                                {
                                    LIGNES_GENERATIONS NouvelleLigne = new LIGNES_GENERATIONS();
                                    NouvelleLigne.EMPLOYEE = Employeee.ID;
                                    NouvelleLigne.EMPLOYEES = Employeee;
                                    NouvelleLigne.GENERATION = NouvelleGeneration.ID;
                                    NouvelleLigne.GENERATIONS = NouvelleGeneration;
                                    NouvelleLigne.SALAIRE_MOIS_1 = (double)SalaireMois1;
                                    NouvelleLigne.SALAIRE_MOIS_2 = (double)SalaireMois2;
                                    NouvelleLigne.SALAIRE_MOIS_3 = (double)SalaireMois3;
                                    BD.LIGNES_GENERATIONS.Add(NouvelleLigne);
                                    BD.SaveChanges();
                                }

                            }
                        }
                    }

                }
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Images/" + nom));
            NouvelleGeneration.DATA = fileBytes;
            BD.SaveChanges();
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, nom);
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
        private void InsertIntoList(Stream fStream, DataTable listTable, string societe, string trimestre, string annee)
        {
            try
            {
                int LineNumber = 0;
                int FileNumber = 1;
                for (int iRow = 0; iRow < listTable.Rows.Count; iRow++)
                {
                    string fullName = listTable.Rows[iRow][0] != null ? Convert.ToString(listTable.Rows[iRow][0]) : string.Empty;
                    string cin = listTable.Rows[iRow][1] != null ? Convert.ToString(listTable.Rows[iRow][1]) : string.Empty;
                    string cnss = listTable.Rows[iRow][2] != null ? Convert.ToString(listTable.Rows[iRow][2]) : string.Empty;
                    string salaire = listTable.Rows[iRow][3] != null ? Convert.ToString(listTable.Rows[iRow][3]) : string.Empty;

                    if (LineNumber == 12)
                    {
                        LineNumber = 1;
                        FileNumber++;
                    }
                    else
                        LineNumber++;
                    string NumAssureSociel = cnss;
                    string IdentiteAssuree = fullName;
                    string Cin = cin;
                    double Sal = double.Parse(salaire);
                    string Salaire = Sal.ToString("#.000");
                    Salaire = Salaire.Replace(",", "");
                    Salaire = Salaire.Replace(" ", "0");

                    while (Salaire.Length < 10)
                    {
                        Salaire = "0" + Salaire;
                    }
                    NumAssureSociel = NumAssureSociel.Replace("-", string.Empty);
                    while (Cin.Length < 8)
                    {
                        Cin = "0" + Cin;
                    }
                    while (IdentiteAssuree.Length < 60)
                    {
                        IdentiteAssuree = IdentiteAssuree + " ";
                    }
                    CnssInfo Info = new CnssInfo();
                    Info.NumSoc = NumAssureSociel;
                    Info.FullName = IdentiteAssuree;
                    Info.Cin = Cin;
                    Info.Salaire = Salaire;
                    Info.Line = LineNumber;
                    Info.page = FileNumber;
                    Liste.Add(Info);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
            };
        }
        public void Uploadfile(Stream fStream, string fileName, string societe, string trimestre, string annee)
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
                    InsertIntoList(fStream, dt, societe, trimestre, annee);
                }
            }
        }
        #region PRINT
        public ActionResult PrintAllSociete()
        {
            List<DECLARATIONS> Liste = BD.DECLARATIONS.ToList();
            dynamic dt = from Element in Liste
                         select new
                         {
                             SOCIETE = Element.SOCIETE,
                             CODE = Element.CODE,
                             MATRICULE = Element.MATRICULE,
                             ADRESSE = Element.ADRESSE,
                             BR = Element.BR,
                             CODE_EXPLOITATION = Element.CODE_EXPLOITATION
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/PRINT_SOCIETES.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Liste des sociétés";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintAllEmployee(int SOCIETE)
        {
            string SelectedSociete = BD.DECLARATIONS.Find(SOCIETE).SOCIETE;
            List<EMPLOYEES> Liste = BD.EMPLOYEES.Where(Emp => Emp.SOCIETES.ID == SOCIETE).ToList();
            dynamic dt = from Element in Liste
                         select new
                         {
                             CIN = Element.CIN,
                             FULLNAME = CleanString(Element.FULLNAME),
                             NUM_ASS_SOC = Element.NUM_ASS_SOC,
                             ACTIF = Element.ACTIF ? "ACTIF" : "NON ACTIF",
                             QUALIFICATION = Element.QUALIFICATION,
                             NUMERO = Element.NUMERO
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/PRINT_EMPLOYEE.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Liste des employées (" + SelectedSociete + ")";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintSeizure(int SOCIETE, int MOIS, int ANNEE)
        {
            string SelectedSociete = BD.DECLARATIONS.Find(SOCIETE).SOCIETE;
            List<SAISIES> Liste = BD.SAISIES.Where(Saisie => Saisie.SOCIETES.ID == SOCIETE && Saisie.ANNEE == ANNEE && Saisie.MOIS == MOIS).ToList();
            dynamic dt = from Element in Liste
                         select new
                         {
                             CIN = Element.EMPLOYEES.CIN,
                             FULLNAME = Element.EMPLOYEES.FULLNAME,
                             SALAIRE = Element.SALAIRE,
                             MOIS = MOIS,
                             ANNEE = ANNEE,
                             SOCIETE = SelectedSociete
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/PRINT_SAISIE.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Saisie Mensuelle des salaires";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintAllDeclaration()
        {
            List<GENERATIONS> Liste = BD.GENERATIONS.ToList();
            dynamic dt = from Element in Liste
                         select new
                         {
                             SOCIETE = Element.DECLARATIONS.SOCIETE,
                             CODE = Element.CODE,
                             ANNEE = Element.ANNEE,
                             TRIMESTRE = Element.TRIMESTRE,
                             DATE = Element.DATE.ToShortDateString(),
                             MONTANT = GetAmount(Element.ID)
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/PRINT_DECLARATION.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Liste des déclarations CNSS";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintHistorique(string CIN)
        {
            EMPLOYEES Employee = BD.EMPLOYEES.Where(Emp => Emp.CIN == CIN).FirstOrDefault();
            List<LIGNES_GENERATIONS> Liste = BD.LIGNES_GENERATIONS.Where(Emp => Emp.EMPLOYEES.ID == Employee.ID).ToList();
            dynamic dt = from Element in Liste
                         select new
                         {
                             CIN = Employee.CIN,
                             FULLNAME = CleanString(Employee.FULLNAME),
                             NUM_ASS_SOC = Employee.NUM_ASS_SOC,
                             QUALIFICATION = Employee.QUALIFICATION,
                             NUMERO = Employee.NUMERO,
                             SOCIETE = Element.GENERATIONS.DECLARATIONS.SOCIETE,
                             CODE = Element.GENERATIONS.CODE,
                             TRIMESTRE = Element.GENERATIONS.TRIMESTRE,
                             ANNEE = Element.GENERATIONS.ANNEE,
                             SALAIRE_MOIS_3 = Element.SALAIRE_MOIS_3,
                             SALAIRE_MOIS_2 = Element.SALAIRE_MOIS_2,
                             SALAIRE_MOIS_1 = Element.SALAIRE_MOIS_1
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/PRINT_HISTORIQUE_CNSS.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Historique déclaration CNSS";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintDeclarationDetail(int CODE)
        {
            GENERATIONS Declaration = BD.GENERATIONS.Find(CODE);
            List<LIGNES_GENERATIONS> Liste = BD.LIGNES_GENERATIONS.Where(Emp => Emp.GENERATIONS.ID == CODE).ToList();
            dynamic dt = from Element in Liste
                         select new
                         {
                             ANNEE = Declaration.ANNEE,
                             CODE = Declaration.DECLARATIONS.CODE,
                             SOCIETE = Declaration.DECLARATIONS.SOCIETE,
                             MATRICULE = Declaration.DECLARATIONS.MATRICULE,
                             ADRESSE = Declaration.DECLARATIONS.ADRESSE,
                             CODE_EXPLOITATION = Declaration.DECLARATIONS.CODE_EXPLOITATION,
                             BR = Declaration.DECLARATIONS.BR,
                             TRIMESTRE = Declaration.TRIMESTRE,
                             NUM_ASS_SOC = Element.EMPLOYEES.NUM_ASS_SOC,
                             FULLNAME = Element.EMPLOYEES.FULLNAME,
                             QUALIFICATION = Element.EMPLOYEES.QUALIFICATION,
                             NUMERO = Element.EMPLOYEES.NUMERO,
                             SALAIRE_MOIS_1 = Element.SALAIRE_MOIS_1,
                             SALAIRE_MOIS_2 = Element.SALAIRE_MOIS_2,
                             SALAIRE_MOIS_3 = Element.SALAIRE_MOIS_3
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/DECLARATION.rpt");
            rptH.Load(FileName);
            rptH.SummaryInfo.ReportTitle = "Détails du déclaration " + Declaration.CODE;
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        #endregion
        public decimal GetAmount(int ID)
        {
            decimal Result = 0;
            List<LIGNES_GENERATIONS> Liste = BD.LIGNES_GENERATIONS.Where(Ligne => Ligne.GENERATIONS.ID == ID).ToList();
            foreach (LIGNES_GENERATIONS Ligne in Liste)
            {
                if (Ligne.SALAIRE_MOIS_1 != 0)
                    Result += (decimal)Ligne.SALAIRE_MOIS_1;
                if (Ligne.SALAIRE_MOIS_2 != 0)
                    Result += (decimal)Ligne.SALAIRE_MOIS_2;
                if (Ligne.SALAIRE_MOIS_3 != 0)
                    Result += (decimal)Ligne.SALAIRE_MOIS_3;
            }
            return Result;
        }
        public string CleanString(string input)
        {
            string output = string.Empty;
            output = input.TrimEnd();
            output = output.Replace(" ", "()").Replace(")(", "").Replace("()", " ");
            return output;
        }
    }
    public class CnssInfo
    {
        public string NumSoc;
        public string Cin;
        public string FullName;
        public string Salaire;
        public int Line;
        public int page;
    }
}
