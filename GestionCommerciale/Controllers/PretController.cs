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
    public class PretController : Controller
    {
        //
        // GET: /Pret/
        GestionCommercialeEntity BD = new GestionCommercialeEntity();
        public ActionResult Index()
        {
            List<PRETS> Liste = BD.PRETS.OrderByDescending(Element => Element.DATE).ToList();
            return View(Liste);
        }
        public ActionResult Form(string Mode, int Code)
        {
            PRETS Element = new PRETS();
            if (Mode == "Create")
            {
                ViewBag.FULLNAME = string.Empty;
                ViewBag.MATRICULE = string.Empty;
                ViewBag.TITRE = "NOUVELLE DEMANDE";
                Element.DATE = DateTime.Today;
                Element.DATE_ECHEANCE = DateTime.Today;
                ViewBag.NBR = 1;
                int NewCode = 1;
                if (BD.PRETS.Count() > 0)
                {
                    List<PRETS> liste = BD.PRETS.ToList();
                    foreach (PRETS Pret in liste)
                    {
                        int Value = int.Parse(Pret.CODE);
                        if (Value >= NewCode) NewCode = Value;
                    }
                }
                ViewBag.CODE_PRET = NewCode;
            }
            if (Mode == "Edit")
            {
                Element = BD.PRETS.Find(Code);
                ViewBag.FULLNAME = Element.EMPLOYEES.FULLNAME;
                ViewBag.MATRICULE = Element.EMPLOYEES.NUMERO;
                ViewBag.TITRE = "MODIFIER UNE DEMANDE";
                ViewBag.NBR = Element.NBR_MOIS;
                ViewBag.CODE_PRET = Element.CODE;
            }
            ViewBag.Mode = Mode;
            ViewBag.Code = Code;
            return View(Element);
        }
        [HttpPost]
        public ActionResult SendForm(string Mode, string Code)
        {
            string CODE_PRET = Request.Params["CODE_PRET"] != null ? Request.Params["CODE_PRET"].ToString() : string.Empty;
            string MATRICULE = Request.Params["MATRICULE"] != null ? Request.Params["MATRICULE"].ToString() : string.Empty;
            string employee = Request.Params["employee"] != null ? Request.Params["employee"].ToString() : "0";
            string MONTANT = Request.Params["MONTANT"] != null ? Request.Params["MONTANT"].ToString() : string.Empty;
            string DATE = Request.Params["DATE"] != null ? Request.Params["DATE"].ToString() : string.Empty;
            //string DATE_ECHEANCE = Request.Params["DATE_ECHEANCE"] != null ? Request.Params["DATE_ECHEANCE"].ToString() : string.Empty;
            string NBR_MOIS = Request.Params["NBR_MOIS"] != null ? Request.Params["NBR_MOIS"].ToString() : string.Empty;
            string TYPE = Request.Params["TYPE"] != null ? Request.Params["TYPE"].ToString() : string.Empty;

            DateTime SelectedDate = DateTime.Parse(DATE);
            //DateTime SelectedEcheDate = DateTime.Parse(DATE_ECHEANCE);
            int ID = int.Parse(employee);
            EMPLOYEES SelectedEmployee = BD.EMPLOYEES.Find(ID);
            if (Mode == "Create")
            {
                PRETS Unpret = new PRETS();
                Unpret.CODE = CODE_PRET;
                Unpret.EMPLOYEE = ID;
                Unpret.EMPLOYEES = SelectedEmployee;
                Unpret.MONTANT = decimal.Parse(MONTANT);
                Unpret.RECU = 0;
                Unpret.RESTE = decimal.Parse(MONTANT);
                Unpret.DATE = SelectedDate;
                //Unpret.DATE_ECHEANCE = SelectedEcheDate;
                Unpret.NBR_MOIS = int.Parse(NBR_MOIS);
                Unpret.TYPE = TYPE;
                Unpret.STATUT = "NON CLOTURE";
                BD.PRETS.Add(Unpret);
                BD.SaveChanges();
                decimal MontantApayer = decimal.Parse(MONTANT);
                for (int i = 0; i < int.Parse(NBR_MOIS); i++)
                {
                    TRANCHES_PRETS UneTranche = new TRANCHES_PRETS();
                    UneTranche.PRET = Unpret.ID;
                    UneTranche.PRETS = Unpret;
                    UneTranche.MONTANT = MontantApayer / int.Parse(NBR_MOIS);
                    UneTranche.DATE = Unpret.DATE.AddMonths(i);
                    UneTranche.STATUT = "1";
                    BD.TRANCHES_PRETS.Add(UneTranche);
                    BD.SaveChanges();
                }
            }
            if (Mode == "Edit")
            {
                int SeletedPretID = int.Parse(Code);
                PRETS Unpret = BD.PRETS.Find(SeletedPretID);
                Unpret.CODE = CODE_PRET;
                Unpret.EMPLOYEE = ID;
                Unpret.EMPLOYEES = SelectedEmployee;
                Unpret.MONTANT = decimal.Parse(MONTANT);
                Unpret.RECU = 0;
                Unpret.RESTE = decimal.Parse(MONTANT);
                Unpret.DATE = SelectedDate;
                //Unpret.DATE_ECHEANCE = SelectedEcheDate;
                Unpret.NBR_MOIS = int.Parse(NBR_MOIS);
                Unpret.TYPE = TYPE;
                Unpret.STATUT = "NON CLOTURE";
                BD.SaveChanges();
                BD.TRANCHES_PRETS.Where(p => p.PRETS.ID == SeletedPretID).ToList().ForEach(p => BD.TRANCHES_PRETS.Remove(p));
                BD.SaveChanges();
                decimal MontantApayer = decimal.Parse(MONTANT);
                for (int i = 0; i < int.Parse(NBR_MOIS); i++)
                {
                    TRANCHES_PRETS UneTranche = new TRANCHES_PRETS();
                    UneTranche.PRET = Unpret.ID;
                    UneTranche.PRETS = Unpret;
                    UneTranche.MONTANT = MontantApayer / int.Parse(NBR_MOIS);
                    UneTranche.DATE = Unpret.DATE.AddMonths(i);
                    UneTranche.STATUT = "1";
                    BD.TRANCHES_PRETS.Add(UneTranche);
                    BD.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
        public ActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SendFile(string Mode, string Code)
        {
            HttpPostedFileBase FILE = Request.Files["FILE"];
            if (FILE != null && FILE.ContentLength > 0)
            {
                string path = Path.Combine(Server.MapPath("~/Images/"), FILE.FileName);
                FILE.SaveAs(path);
                Uploadfile(FILE.InputStream, path);
            }
            return RedirectToAction("Index");
        }
        public void Uploadfile(Stream fStream, string fileName)
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
                    InsertIntoList(fStream, dt);
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
        private void InsertIntoList(Stream fStream, DataTable listTable)
        {
            try
            {
                for (int iRow = 0; iRow < listTable.Rows.Count; iRow++)
                {
                    string NUMERO_PRET = listTable.Rows[iRow][0] != null ? Convert.ToString(listTable.Rows[iRow][0]) : "";
                    string MATRICULE = listTable.Rows[iRow][1] != null ? Convert.ToString(listTable.Rows[iRow][1]) : "";
                    string NOM = listTable.Rows[iRow][2] != null ? Convert.ToString(listTable.Rows[iRow][2]) : "";
                    string PRENOM = listTable.Rows[iRow][3] != null ? Convert.ToString(listTable.Rows[iRow][3]) : "";
                    string MONTANT = listTable.Rows[iRow][4] != null ? Convert.ToString(listTable.Rows[iRow][4]) : "0";
                    string DATE = listTable.Rows[iRow][5] != null ? Convert.ToString(listTable.Rows[iRow][5]) : DateTime.Today.ToShortDateString();
                    string NBR_ECHAEANCE = listTable.Rows[iRow][6] != null ? Convert.ToString(listTable.Rows[iRow][6]) : "1";
                    string NUMERO_ECHEANCE = listTable.Rows[iRow][7] != null ? Convert.ToString(listTable.Rows[iRow][7]) : "1";
                    string MONTANT_ECHEANCE = listTable.Rows[iRow][8] != null ? Convert.ToString(listTable.Rows[iRow][8]) : "0";
                    string DATE_ECHEANCE = listTable.Rows[iRow][9] != null ? Convert.ToString(listTable.Rows[iRow][9]) : DateTime.Today.ToShortDateString();
                    string STAUT_ECHEANCE = listTable.Rows[iRow][10] != null ? Convert.ToString(listTable.Rows[iRow][10]) : "";
                    string TYPE_PRET = listTable.Rows[iRow][11] != null ? Convert.ToString(listTable.Rows[iRow][11]) : "01";
                    TRANCHES_PRETS SelectedTranche = BD.TRANCHES_PRETS.Where(Element => Element.NUMERO == NUMERO_ECHEANCE && Element.PRETS.CODE == NUMERO_PRET).FirstOrDefault();
                    PRETS SelectedPret = BD.PRETS.Where(Element => Element.CODE == NUMERO_PRET).FirstOrDefault();
                    if (SelectedTranche == null)
                    {

                        if (SelectedPret == null)
                        {
                            EMPLOYEES SelectedEmploye = BD.EMPLOYEES.Where(Element => Element.NUMERO == MATRICULE).FirstOrDefault();
                            if (SelectedEmploye == null)
                            {
                                SelectedEmploye = new EMPLOYEES();
                                SelectedEmploye.FULLNAME = PRENOM + " " + NOM;
                                SelectedEmploye.NUMERO = MATRICULE;
                                if (BD.DECLARATIONS.FirstOrDefault() != null)
                                {
                                    SelectedEmploye.SOCIETES = BD.DECLARATIONS.FirstOrDefault();
                                    SelectedEmploye.SOCIETE = BD.DECLARATIONS.FirstOrDefault().ID;
                                }
                                BD.EMPLOYEES.Add(SelectedEmploye);
                                BD.SaveChanges();
                            }
                            DateTime dateValue;
                            if (DateTime.TryParse(DATE, out dateValue))
                            {
                                decimal montant = !string.IsNullOrEmpty(MONTANT) ? decimal.Parse(MONTANT) : 0;
                                DateTime NewDate = DateTime.Parse(DATE);
                                SelectedPret = new PRETS();
                                SelectedPret.CODE = NUMERO_PRET;
                                SelectedPret.EMPLOYEE = SelectedEmploye.ID;
                                SelectedPret.EMPLOYEES = SelectedEmploye;
                                SelectedPret.MONTANT = montant;
                                SelectedPret.DATE = NewDate;
                                SelectedPret.DATE_ECHEANCE = NewDate;
                                SelectedPret.NBR_MOIS = !string.IsNullOrEmpty(NBR_ECHAEANCE) ? int.Parse(NBR_ECHAEANCE) : 1;
                                SelectedPret.RECU = 0;
                                SelectedPret.RESTE = montant;
                                SelectedPret.TYPE = TYPE_PRET;
                                SelectedPret.STATUT = "NON CLOTURE";
                                BD.PRETS.Add(SelectedPret);
                                BD.SaveChanges();
                            }
                        }
                        SelectedTranche = new TRANCHES_PRETS();
                        decimal montantTranche = !string.IsNullOrEmpty(MONTANT_ECHEANCE) ? decimal.Parse(MONTANT_ECHEANCE) : 0;
                        SelectedTranche.MONTANT = montantTranche;
                        SelectedTranche.NUMERO = NUMERO_ECHEANCE;
                        SelectedTranche.DATE = DateTime.Parse(DATE_ECHEANCE);
                        SelectedTranche.STATUT = STAUT_ECHEANCE;
                        SelectedTranche.PRET = SelectedPret.ID;
                        SelectedTranche.PRETS = SelectedPret;
                        BD.TRANCHES_PRETS.Add(SelectedTranche);
                        BD.SaveChanges();
                    }
                    else
                    {
                        decimal montantTranche = !string.IsNullOrEmpty(MONTANT_ECHEANCE) ? decimal.Parse(MONTANT_ECHEANCE) : 0;
                        SelectedTranche.MONTANT = montantTranche;
                        SelectedTranche.NUMERO = NUMERO_ECHEANCE;
                        SelectedTranche.DATE = DateTime.Parse(DATE_ECHEANCE);
                        SelectedTranche.STATUT = STAUT_ECHEANCE;
                        BD.SaveChanges();
                    }
                    if (SelectedTranche.STATUT == "2")
                    {
                        SelectedPret.RECU += SelectedTranche.MONTANT;
                        SelectedPret.RESTE -= SelectedTranche.MONTANT;
                        if (SelectedPret.RESTE <= 0) SelectedPret.STATUT = "CLOTURE";
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
        public ActionResult Delete(int Code)
        {
            PRETS Selected = BD.PRETS.Find(Code);
            BD.TRANCHES_PRETS.Where(p => p.PRETS.ID == Code).ToList().ForEach(p => BD.TRANCHES_PRETS.Remove(p));
            BD.SaveChanges();
            BD.PRETS.Remove(Selected);
            BD.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Tranches(int Code)
        {
            List<TRANCHES_PRETS> Liste = BD.TRANCHES_PRETS.Where(Element => Element.PRETS.ID == Code).ToList();
            ViewBag.Filter = Code;
            PRETS Selected = BD.PRETS.Find(Code);
            ViewBag.CODE_PRET = Selected.CODE;
            ViewBag.MONTANT_PRET = Selected.MONTANT;
            ViewBag.RECU_PRET = Selected.RECU;
            ViewBag.RESTE_PRET = Selected.RESTE;
            return View(Liste);
        }
        public ActionResult FormTranche(string Mode, int Code, int Filter)
        {
            TRANCHES_PRETS Element = new TRANCHES_PRETS();
            if (Mode == "Create")
            {
                ViewBag.TITRE = "NOUVELLE TRANCHE";
                Element.DATE = DateTime.Today;
            }
            if (Mode == "Edit")
            {
                Element = BD.TRANCHES_PRETS.Find(Code);
                ViewBag.TITRE = "MODIFIER UNE TRANCHE";
            }
            ViewBag.Mode = Mode;
            ViewBag.Code = Code;
            ViewBag.Filter = Filter;
            return View(Element);
        }
        [HttpPost]
        public ActionResult SendFormTranche(string Mode, string Code, string Filter)
        {
            string MONTANT = Request.Params["MONTANT"] != null ? Request.Params["MONTANT"].ToString() : string.Empty;
            string DATE = Request.Params["DATE"] != null ? Request.Params["DATE"].ToString() : string.Empty;
            string STATUT = Request.Params["STATUT"] != null ? Request.Params["STATUT"].ToString() : string.Empty;
            DateTime SelectedDate = DateTime.Parse(DATE);
            int SelectedPretId = int.Parse(Filter);
            PRETS SelectedPret = BD.PRETS.Find(SelectedPretId);
            if (Mode == "Create")
            {
                TRANCHES_PRETS NouvelleTranche = new TRANCHES_PRETS();
                NouvelleTranche.PRET = SelectedPretId;
                NouvelleTranche.PRETS = SelectedPret;
                NouvelleTranche.MONTANT = decimal.Parse(MONTANT);
                NouvelleTranche.DATE = SelectedDate;
                NouvelleTranche.STATUT = STATUT;
                BD.TRANCHES_PRETS.Add(NouvelleTranche);
                BD.SaveChanges();
                //SelectedPret.RESTE -= decimal.Parse(MONTANT);
                //SelectedPret.RECU += decimal.Parse(MONTANT);
                //BD.SaveChanges();
            }
            if (Mode == "Edit")
            {
                int SeletedTrancheId = int.Parse(Code);
                TRANCHES_PRETS SelectedTranche = BD.TRANCHES_PRETS.Find(SeletedTrancheId);
                SelectedTranche.PRET = SelectedPretId;
                SelectedTranche.PRETS = SelectedPret;
                SelectedTranche.MONTANT = decimal.Parse(MONTANT);
                SelectedTranche.DATE = SelectedDate;
                SelectedTranche.STATUT = STATUT;
                BD.SaveChanges();
                //SelectedPret.RESTE -= decimal.Parse(MONTANT);
                //SelectedPret.RECU += decimal.Parse(MONTANT);
                //BD.SaveChanges();
            }
            return RedirectToAction("Tranches", "Pret", new { @Code = Filter });
        }
        public ActionResult DeleteTranche(int Code, int Filter)
        {
            TRANCHES_PRETS Selected = BD.TRANCHES_PRETS.Find(Code);
            BD.TRANCHES_PRETS.Remove(Selected);
            BD.SaveChanges();
            return RedirectToAction("Tranches", "Pret", new { @Code = Filter });
        }
        [HttpPost]
        public ActionResult Encaissement()
        {
            string MONTANT = Request.Params["MONTANT"] != null ? Request.Params["MONTANT"].ToString() : string.Empty;
            string DATE = Request.Params["DATE"] != null ? Request.Params["DATE"].ToString() : string.Empty;
            string Filter = Request.Params["Filter"] != null ? Request.Params["Filter"].ToString() : string.Empty;
            string Code = Request.Params["Code"] != null ? Request.Params["Code"].ToString() : string.Empty;
            int SelectedTrancheID = int.Parse(Code);
            int SelectedPretID = int.Parse(Filter);
            TRANCHES_PRETS SelectedTranche = BD.TRANCHES_PRETS.Find(SelectedTrancheID);
            PRETS SelectedPret = BD.PRETS.Find(SelectedPretID);
            SelectedTranche.MONTANT = decimal.Parse(MONTANT);
            SelectedTranche.DATE = DateTime.Parse(DATE);
            SelectedTranche.STATUT = "ARRETE";
            BD.SaveChanges();
            SelectedPret.RESTE -= decimal.Parse(MONTANT);
            SelectedPret.RECU += decimal.Parse(MONTANT);
            if (SelectedPret.RECU == SelectedPret.MONTANT)
            {
                SelectedPret.STATUT = "CLOTURE";
            }
            BD.SaveChanges();
            return RedirectToAction("Tranches", "Pret", new { @Code = Filter });
        }
        public ActionResult Download()
        {
            PARAMETRES Parametre = BD.PARAMETRES.FirstOrDefault();
            if (Parametre.MODELE_PRET == null)
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(@"~/MODELES EXCEL/MODELE SAISIE PRET.xlsx"));
                Parametre.MODELE_PRET = fileBytes;
                BD.SaveChanges();
            }
            return View();
        }
        public FileResult GetFile()
        {
            var fileToRetrieve = BD.PARAMETRES.FirstOrDefault();
            return File(fileToRetrieve.MODELE_PRET, System.Net.Mime.MediaTypeNames.Application.Octet, "MODELE SAISIE PRET.xlsx");
        }
        public ActionResult PretUser()
        {
            return View();
        }
        public ActionResult Filtrer(string CODE, string START, string END)
        {
            decimal TOTAL = 0;
            decimal RECU = 0;
            decimal RESTE = 0;
            List<PRETS> Liste = BD.PRETS.ToList();
            if (!string.IsNullOrEmpty(CODE))
            {
                int SelectedUser = int.Parse(CODE);
                Liste = Liste.Where(Element => Element.EMPLOYEES.ID == SelectedUser).ToList();
            }
            if (!string.IsNullOrEmpty(START))
            {
                DateTime StartDate = DateTime.Parse(START);
                Liste = Liste.Where(Element => Element.DATE >= StartDate).ToList();
            }
            if (!string.IsNullOrEmpty(END))
            {
                DateTime EndDate = DateTime.Parse(END);
                Liste = Liste.Where(Element => Element.DATE <= EndDate).ToList();
            }
            foreach (PRETS Element in Liste)
            {
                TOTAL += Element.MONTANT;
            }
            foreach (PRETS Element in Liste)
            {
                RECU += Element.RECU;
            }
            foreach (PRETS Element in Liste)
            {
                RESTE += Element.RESTE;
            }
            ViewBag.TOTAL = TOTAL.ToString("F3");
            ViewBag.RECU = RECU.ToString("F3");
            ViewBag.RESTE = RESTE.ToString("F3");
            return PartialView(Liste);
        }
        public string CheckTranche(string Code)
        {
            string Result = string.Empty;
            int SelectedId = int.Parse(Code);
            TRANCHES_PRETS Tranche = BD.TRANCHES_PRETS.Find(SelectedId);
            if (Tranche.STATUT == "ARRETE")
                Result = "FALSE";
            return Result;
        }
        public ActionResult PrintFilter(string CODE, string START, string END, string Mode)
        {
            dynamic dt = null;

            if (Mode == "DETAIL")
            {
                List<PRETS> Liste = BD.PRETS.ToList();
                if (!string.IsNullOrEmpty(CODE))
                {
                    int SelectedUser = int.Parse(CODE);
                    Liste = Liste.Where(Element => Element.EMPLOYEES.ID == SelectedUser).ToList();
                }
                if (!string.IsNullOrEmpty(START))
                {
                    DateTime StartDate = DateTime.Parse(START);
                    Liste = Liste.Where(Element => Element.DATE >= StartDate).ToList();
                }
                if (!string.IsNullOrEmpty(END))
                {
                    DateTime EndDate = DateTime.Parse(END);
                    Liste = Liste.Where(Element => Element.DATE <= EndDate).ToList();
                }
                dt = from Element in Liste
                     select new
                     {
                         CODE = Element.CODE,
                         FULLNAME = Element.EMPLOYEES.FULLNAME,
                         NUMERO = Element.EMPLOYEES.NUMERO,
                         MONTANT = Element.MONTANT,
                         RECU = Element.RECU,
                         RESTE = Element.RESTE,
                         DU = START,
                         DATE = Element.DATE.ToShortDateString(),
                         AU = END,
                     };
            }
            if (Mode == "GROUP")
            {
                List<EMPLOYEES> Liste = BD.EMPLOYEES.ToList();
                dt = from Element in Liste
                     select new
                     {
                         CODE = string.Empty,
                         FULLNAME = Element.FULLNAME,
                         NUMERO = Element.NUMERO,
                         MONTANT = GetMontant(Element, START, END),
                         RECU = GetRecu(Element, START, END),
                         RESTE = GetReste(Element, START, END),
                         DU = START,
                         DATE = string.Empty,
                         AU = END,
                     };
            }
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/PRINT_FILTER_PRET.rpt");
            rptH.Load(FileName);
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public decimal GetMontant(EMPLOYEES Employee, string Start, string End)
        {
            decimal Result = 0;
            List<PRETS> Liste = BD.PRETS.Where(Elment => Elment.EMPLOYEES.ID == Employee.ID).ToList();
            if (!string.IsNullOrEmpty(Start))
            {
                DateTime StartDate = DateTime.Parse(Start);
                Liste = Liste.Where(Element => Element.DATE >= StartDate).ToList();
            }
            if (!string.IsNullOrEmpty(End))
            {
                DateTime EndDate = DateTime.Parse(End);
                Liste = Liste.Where(Element => Element.DATE <= EndDate).ToList();
            }
            foreach (PRETS Element in Liste)
            {
                Result += Element.MONTANT;
            }
            return Result;
        }
        public decimal GetRecu(EMPLOYEES Employee, string Start, string End)
        {
            decimal Result = 0;
            List<PRETS> Liste = BD.PRETS.Where(Elment => Elment.EMPLOYEES.ID == Employee.ID).ToList();
            if (!string.IsNullOrEmpty(Start))
            {
                DateTime StartDate = DateTime.Parse(Start);
                Liste = Liste.Where(Element => Element.DATE >= StartDate).ToList();
            }
            if (!string.IsNullOrEmpty(End))
            {
                DateTime EndDate = DateTime.Parse(End);
                Liste = Liste.Where(Element => Element.DATE <= EndDate).ToList();
            }
            foreach (PRETS Element in Liste)
            {
                Result += Element.RECU;
            }
            return Result;
        }
        public decimal GetReste(EMPLOYEES Employee, string Start, string End)
        {
            decimal Result = 0;
            List<PRETS> Liste = BD.PRETS.Where(Elment => Elment.EMPLOYEES.ID == Employee.ID).ToList();
            if (!string.IsNullOrEmpty(Start))
            {
                DateTime StartDate = DateTime.Parse(Start);
                Liste = Liste.Where(Element => Element.DATE >= StartDate).ToList();
            }
            if (!string.IsNullOrEmpty(End))
            {
                DateTime EndDate = DateTime.Parse(End);
                Liste = Liste.Where(Element => Element.DATE <= EndDate).ToList();
            }
            foreach (PRETS Element in Liste)
            {
                Result += Element.RESTE;
            }
            return Result;
        }
    }

}
