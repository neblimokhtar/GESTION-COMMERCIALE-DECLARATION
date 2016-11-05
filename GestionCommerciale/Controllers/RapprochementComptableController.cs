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
using System.Globalization;

namespace GestionCommerciale.Controllers
{
    public class RapprochementComptableController : Controller
    {
        //
        // GET: /RapprochementComptable/
        GestionCommercialeEntity BD = new GestionCommercialeEntity();

        public ActionResult Index()
        {
            List<MOUVEMENTS_COMPTABLES> Liste = BD.MOUVEMENTS_COMPTABLES.OrderByDescending(Element => Element.DATE).ToList();
            return View(Liste);
        }
        public ActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SendFile()
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
                    string NUMERO = listTable.Rows[iRow][0] != null ? Convert.ToString(listTable.Rows[iRow][0]) : "";
                    string SMATRICULE = listTable.Rows[iRow][1] != null ? Convert.ToString(listTable.Rows[iRow][1]) : "";
                    string DATE = listTable.Rows[iRow][2] != null ? Convert.ToString(listTable.Rows[iRow][2]) : "";
                    string JOURNAL = listTable.Rows[iRow][3] != null ? Convert.ToString(listTable.Rows[iRow][3]) : "";
                    string NUM_PIECE = listTable.Rows[iRow][4] != null ? Convert.ToString(listTable.Rows[iRow][4]) : "";
                    string LIBELLE = listTable.Rows[iRow][5] != null ? Convert.ToString(listTable.Rows[iRow][5]) : "";
                    string DEBIT = listTable.Rows[iRow][7] != null ? Convert.ToString(listTable.Rows[iRow][7]) : "0";
                    string CREDIT = listTable.Rows[iRow][8] != null ? Convert.ToString(listTable.Rows[iRow][8]) : "0";
                    string MATRICULE = SMATRICULE.Substring(1);
                    EMPLOYEES SelectedEmploye = BD.EMPLOYEES.Where(Element => Element.NUMERO == MATRICULE).FirstOrDefault();
                    if (SelectedEmploye == null)
                    {
                        SelectedEmploye = new EMPLOYEES();
                        SelectedEmploye.NUMERO = MATRICULE;
                        SelectedEmploye.FULLNAME = string.Empty;
                        if (BD.DECLARATIONS.FirstOrDefault() != null)
                        {
                            SelectedEmploye.SOCIETES = BD.DECLARATIONS.FirstOrDefault();
                            SelectedEmploye.SOCIETE = BD.DECLARATIONS.FirstOrDefault().ID;
                        }
                        BD.EMPLOYEES.Add(SelectedEmploye);
                        BD.SaveChanges();
                    }
                    int Num = int.Parse(NUMERO);
                    MOUVEMENTS_COMPTABLES SelectedMouvemenet = BD.MOUVEMENTS_COMPTABLES.Where(Element => Element.NUMERO == Num).FirstOrDefault();
                    if (SelectedMouvemenet == null)
                    {
                        DateTime dateValue;
                        if (DateTime.TryParse(DATE, out dateValue))
                        {
                            decimal value;
                            MOUVEMENTS_COMPTABLES Mouvement = new MOUVEMENTS_COMPTABLES();
                            Mouvement.EMPLOYEE = SelectedEmploye.ID;
                            Mouvement.NUMERO = Num;
                            Mouvement.EMPLOYEES = SelectedEmploye;
                            Mouvement.DATE = DateTime.Parse(DATE);
                            Mouvement.JOURNAL = JOURNAL;
                            Mouvement.NUM_PIECE = NUM_PIECE;
                            Mouvement.LIBELLE = LIBELLE;
                            Mouvement.DATE_AFFECATION = DateTime.Parse(DATE);
                            if (JOURNAL == "A.N")
                            {
                                DateTime AnneeP = DateTime.Parse(DATE).AddYears(-1);
                                Mouvement.DATE_AFFECATION = new DateTime(AnneeP.Year, 12, 31);
                            }
                            if (decimal.TryParse(DEBIT, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                            {
                                decimal debit = decimal.Parse(DEBIT);
                                Mouvement.MONTANT = debit;
                                Mouvement.ACTION = "DEBIT";
                                BD.SaveChanges();
                            }
                            if (decimal.TryParse(CREDIT, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                            {
                                decimal credit = decimal.Parse(CREDIT);
                                Mouvement.MONTANT = -credit;
                                Mouvement.ACTION = "CREDIT";
                                BD.SaveChanges();
                            }
                            BD.MOUVEMENTS_COMPTABLES.Add(Mouvement);
                        }
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
        public ActionResult Recap()
        {
            return View();
        }
        public ActionResult Filtrer(string CODE, string START, string END)
        {
            decimal TOTAL = 0;
            decimal DEBIT = 0;
            decimal CREDIT = 0;
            List<MOUVEMENTS_COMPTABLES> Liste = BD.MOUVEMENTS_COMPTABLES.ToList();
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
            foreach (MOUVEMENTS_COMPTABLES Element in Liste)
            {
                TOTAL += Element.MONTANT;
                if (Element.MONTANT < 0) CREDIT += Element.MONTANT;
                if (Element.MONTANT > 0) DEBIT += Element.MONTANT;
            }
            ViewBag.TOTAL = TOTAL.ToString("F3");
            ViewBag.DEBIT = DEBIT.ToString("F3");
            ViewBag.CREDIT = CREDIT.ToString("F3");
            return PartialView(Liste);
        }
        public ActionResult PrintFilter(string CODE, string START, string END, string Mode)
        {
            dynamic dt = null;
            var Variable = new
            {
                DU = START,
                AU = END,
                DATE = string.Empty,
                MATRICULE = string.Empty,
                FULLNAME = string.Empty,
                JOURNAL = string.Empty,
                NUMERO_PIECE = string.Empty,
                LIBELLE = string.Empty,
                DEBIT = 0,
                CREDIT = 0,
            };
            if (Mode == "DETAIL")
            {
                List<MOUVEMENTS_COMPTABLES> Liste = BD.MOUVEMENTS_COMPTABLES.ToList();
                if (!string.IsNullOrEmpty(CODE))
                {
                    int SelectedUser = int.Parse(CODE);
                    Liste = Liste.Where(Element => Element.EMPLOYEES.ID == SelectedUser).ToList();
                }
                if (!string.IsNullOrEmpty(START))
                {
                    DateTime StartDate = DateTime.Parse(START);
                    Liste = Liste.Where(Element => Element.DATE_AFFECATION >= StartDate).ToList();


                }
                if (!string.IsNullOrEmpty(END))
                {
                    DateTime EndDate = DateTime.Parse(END);
                    Liste = Liste.Where(Element => Element.DATE_AFFECATION <= EndDate).ToList();

                }

                dt = from Element in Liste
                     select new
                     {
                         DU = START,
                         AU = END,
                         DATE = Element.DATE_AFFECATION.ToShortDateString(),
                         MATRICULE = Element.EMPLOYEES.NUMERO,
                         FULLNAME = Element.EMPLOYEES.FULLNAME,
                         JOURNAL = Element.JOURNAL,
                         NUMERO_PIECE = Element.NUM_PIECE,
                         LIBELLE = Element.LIBELLE,
                         DEBIT = Element.MONTANT > 0 ? Element.MONTANT : 0,
                         CREDIT = Element.MONTANT < 0 ? Element.MONTANT : 0,
                     };

                if (Liste.Count == 0)
                {

                    dt = new[] { Variable };
                }

            }
            if (Mode == "GROUP")
            {
                List<EMPLOYEES> Liste = BD.EMPLOYEES.ToList();
                dt = from Element in Liste
                     select new
                     {
                         DU = START,
                         AU = END,
                         DATE = string.Empty,
                         MATRICULE = Element.NUMERO,
                         FULLNAME = Element.FULLNAME,
                         JOURNAL = string.Empty,
                         NUMERO_PIECE = string.Empty,
                         LIBELLE = string.Empty,
                         DEBIT = GetDebit(Element, START, END),
                         CREDIT = GetCredit(Element, START, END),
                     };
                if (Liste.Count == 0)
                {
                    dt = new[] { Variable };
                }
            }
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/PRINT_FILTER_COMP.rpt");
            rptH.Load(FileName);
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult PrintRecap(string CODE, string START, string END)
        {

            List<EMPLOYEES> Liste = BD.EMPLOYEES.OrderBy(Element=>Element.NUMERO).ToList();
            dynamic dt = from Element in Liste
                         select new
                         {
                             DU = START,
                             AU = END,
                             MATRICULE = Element.NUMERO,
                             FULLNAME = Element.FULLNAME,
                             DEBIT = GetDebit(Element, START, END),
                             CREDIT = GetCredit(Element, START, END),
                             PRIS = GetMontant(Element, START, END),
                             RECU = GetRecu(Element, START, END),
                         };

            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/PRINT_RECAP.rpt");
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
        public decimal GetDebit(EMPLOYEES Employee, string Start, string End)
        {
            decimal Result = 0;
            List<MOUVEMENTS_COMPTABLES> Liste = BD.MOUVEMENTS_COMPTABLES.Where(Elment => Elment.EMPLOYEES.ID == Employee.ID).ToList();
            if (!string.IsNullOrEmpty(Start))
            {
                DateTime StartDate = DateTime.Parse(Start);
                Liste = Liste.Where(Element => Element.DATE_AFFECATION >= StartDate).ToList();
            }
            if (!string.IsNullOrEmpty(End))
            {
                DateTime EndDate = DateTime.Parse(End);
                Liste = Liste.Where(Element => Element.DATE_AFFECATION <= EndDate).ToList();
            }
            foreach (MOUVEMENTS_COMPTABLES Element in Liste)
            {
                Result += Element.MONTANT > 0 ? Element.MONTANT : 0;
            }
            return Result;
        }
        public decimal GetCredit(EMPLOYEES Employee, string Start, string End)
        {
            decimal Result = 0;
            List<MOUVEMENTS_COMPTABLES> Liste = BD.MOUVEMENTS_COMPTABLES.Where(Elment => Elment.EMPLOYEES.ID == Employee.ID).ToList();
            if (!string.IsNullOrEmpty(Start))
            {
                DateTime StartDate = DateTime.Parse(Start);
                Liste = Liste.Where(Element => Element.DATE_AFFECATION >= StartDate).ToList();
            }
            if (!string.IsNullOrEmpty(End))
            {
                DateTime EndDate = DateTime.Parse(End);
                Liste = Liste.Where(Element => Element.DATE_AFFECATION <= EndDate).ToList();
            }
            foreach (MOUVEMENTS_COMPTABLES Element in Liste)
            {
                Result += Element.MONTANT < 0 ? Element.MONTANT : 0;
            }
            return Result;
        }
        public ActionResult Delete(int Code)
        {
            MOUVEMENTS_COMPTABLES Selected = BD.MOUVEMENTS_COMPTABLES.Find(Code);
            BD.MOUVEMENTS_COMPTABLES.Remove(Selected);
            BD.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Form(string Mode, int Code)
        {
            MOUVEMENTS_COMPTABLES Element = new MOUVEMENTS_COMPTABLES();
            if (Mode == "Create")
            {
                ViewBag.TITRE = "AJOUTER UN NOUVEAU MOUVEMENT";
                Element.DATE = DateTime.Today;
            }
            if (Mode == "Edit")
            {
                Element = BD.MOUVEMENTS_COMPTABLES.Find(Code);
                ViewBag.TITRE = "MODIFIER UN  MOUVEMENT";
            }
            ViewBag.Mode = Mode;
            ViewBag.Code = Code;
            return View(Element);
        }
        [HttpPost]
        public ActionResult SendForm(string Mode, string Code)
        {
            string MATRICULE = Request.Params["MATRICULE"] != null ? Request.Params["MATRICULE"].ToString() : string.Empty;
            string employee = Request.Params["employee"] != null ? Request.Params["employee"].ToString() : string.Empty;
            string DATE = Request.Params["DATE"] != null ? Request.Params["DATE"].ToString() : string.Empty;
            string JOURNALE = Request.Params["JOURNALE"] != null ? Request.Params["JOURNALE"].ToString() : string.Empty;
            string NUM_PIECE = Request.Params["NUM_PIECE"] != null ? Request.Params["NUM_PIECE"].ToString() : string.Empty;
            string LIBELLE = Request.Params["LIBELLE"] != null ? Request.Params["LIBELLE"].ToString() : string.Empty;
            string MONTANT = Request.Params["MONTANT"] != null ? Request.Params["MONTANT"].ToString() : "0";

            DateTime SelectedDate = DateTime.Parse(DATE);
            int ID = int.Parse(employee);
            EMPLOYEES SelectedEmployee = BD.EMPLOYEES.Find(ID);
            if (Mode == "Create")
            {
                MOUVEMENTS_COMPTABLES NewElement = new MOUVEMENTS_COMPTABLES();
                NewElement.EMPLOYEE = ID;
                NewElement.EMPLOYEES = SelectedEmployee;
                NewElement.DATE = SelectedDate;
                NewElement.JOURNAL = JOURNALE;
                NewElement.NUM_PIECE = JOURNALE;
                NewElement.LIBELLE = LIBELLE;
                NewElement.MONTANT = decimal.Parse(MONTANT, NumberStyles.Any, CultureInfo.InvariantCulture);
                if (NewElement.MONTANT >= 0) NewElement.ACTION = "DEBIT";
                if (NewElement.MONTANT < 0) NewElement.ACTION = "CREDIT";
                BD.MOUVEMENTS_COMPTABLES.Add(NewElement);
                BD.SaveChanges();
            }
            if (Mode == "Edit")
            {
                int SeletedPretID = int.Parse(Code);
                MOUVEMENTS_COMPTABLES NewElement = BD.MOUVEMENTS_COMPTABLES.Find(SeletedPretID);
                NewElement.EMPLOYEE = ID;
                NewElement.EMPLOYEES = SelectedEmployee;
                NewElement.DATE = SelectedDate;
                NewElement.JOURNAL = JOURNALE;
                NewElement.NUM_PIECE = JOURNALE;
                NewElement.LIBELLE = LIBELLE;
                NewElement.MONTANT = decimal.Parse(MONTANT, NumberStyles.Any, CultureInfo.InvariantCulture);
                if (NewElement.MONTANT >= 0) NewElement.ACTION = "DEBIT";
                if (NewElement.MONTANT < 0) NewElement.ACTION = "CREDIT";
                BD.SaveChanges();

            }
            return RedirectToAction("Index");
        }

    }
}
