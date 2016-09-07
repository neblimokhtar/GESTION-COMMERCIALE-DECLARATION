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
    public class AvanceController : Controller
    {
        //
        // GET: /Avance/
        GestionCommercialeEntity BD = new GestionCommercialeEntity();
        public ActionResult Index()
        {
            List<AVANCES> Liste = BD.AVANCES.OrderByDescending(Element=>Element.DATE).ToList();
            return View(Liste);
        }
        public ActionResult AvanceUser()
        {
            return View();

        }
        public ActionResult Filtrer(string CODE, string START, string END)
        {
            decimal TOTAL = 0;
            List<AVANCES> Liste = BD.AVANCES.ToList();
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
            foreach (AVANCES Element in Liste)
            {
                TOTAL += Element.MONTANT;
            }
            ViewBag.TOTAL = TOTAL.ToString("F3");
            return PartialView(Liste);
        }
        public ActionResult PrintFilter(string CODE, string START, string END)
        {

            List<AVANCES> Liste = BD.AVANCES.ToList();
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
            dynamic dt = from Element in Liste
                         select new {
                             FULLNAME=Element.EMPLOYEES.FULLNAME,
                             NUMERO = Element.EMPLOYEES.NUMERO,
                             MONTANT=Element.MONTANT,
                             DATE=Element.DATE.ToShortDateString(),
                             DU=START,
                             AU=END,
                             TYPE=Element.TYPE
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/PRINT_FILTER_AVANCE.rpt");
            rptH.Load(FileName);
            //rptH.SummaryInfo.ReportTitle = "ANNEXE 1 ";
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult Print(string Filter)
        {
            int ID = int.Parse(Filter);
            List<AVANCES> Liste = BD.AVANCES.Where(Element=>Element.ID==ID).ToList();
            dynamic dt = from Element in Liste
                         select new
                         {
                             FULLNAME = Element.EMPLOYEES.FULLNAME,
                             NUMERO = Element.EMPLOYEES.NUMERO,
                             MONTANT = Element.MONTANT,
                             DATE = Element.DATE.ToShortDateString(),
                             TYPE = Element.TYPE
                         };
            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/PRINT_AVANCE.rpt");
            rptH.Load(FileName);
            rptH.SetDataSource(dt);
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public ActionResult Upload()
        {
            return View();
        }
        public ActionResult Download()
        {
            PARAMETRES Parametre = BD.PARAMETRES.FirstOrDefault();
            if (Parametre.MODELE_AVANCE == null)
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(@"~/MODELES EXCEL/MODELE AVANCE.xlsx"));
                Parametre.MODELE_AVANCE = fileBytes;
                BD.SaveChanges();
            }
            return View();
        }
        public FileResult GetFile()
        {
            var fileToRetrieve = BD.PARAMETRES.FirstOrDefault();
            return File(fileToRetrieve.MODELE_AVANCE, System.Net.Mime.MediaTypeNames.Application.Octet, "MODELE SAISIE AVANCE.xls");
        }
        public ActionResult Form(string Mode, int Code)
        {
            AVANCES Element = new AVANCES();
            if (Mode == "Create")
            {
                ViewBag.FULLNAME = string.Empty;
                ViewBag.MATRICULE = string.Empty;
                ViewBag.TITRE = "NOUVELLE DEMANDE";
                Element.DATE = DateTime.Today;
            }
            if (Mode == "Edit")
            {
                Element = BD.AVANCES.Find(Code);
                ViewBag.FULLNAME = Element.EMPLOYEES.FULLNAME;
                ViewBag.MATRICULE = Element.EMPLOYEES.NUMERO;
                ViewBag.TITRE = "MODIFIER UNE DEMANDE";
            }
            ViewBag.Mode = Mode;
            ViewBag.Code = Code;
            return View(Element);
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
        private void InsertIntoList(Stream fStream, DataTable listTable)
        {
            try
            {
                for (int iRow = 0; iRow < listTable.Rows.Count; iRow++)
                {
                    string MATRICULE = listTable.Rows[iRow][0] != null ? Convert.ToString(listTable.Rows[iRow][0]) : "";
                    string FULLENAME = listTable.Rows[iRow][1] != null ? Convert.ToString(listTable.Rows[iRow][1]) : "";
                    string MONTANT = listTable.Rows[iRow][2] != null ? Convert.ToString(listTable.Rows[iRow][2]) : "0";
                    string DATE = listTable.Rows[iRow][3] != null ? Convert.ToString(listTable.Rows[iRow][3]) : DateTime.Today.ToShortDateString();
                    string TYPE = listTable.Rows[iRow][4] != null ? Convert.ToString(listTable.Rows[iRow][4]) : "";
                    EMPLOYEES SelectedEmploye = BD.EMPLOYEES.Where(Element => Element.NUMERO == MATRICULE).FirstOrDefault();

                    if (SelectedEmploye == null)
                    {
                        SelectedEmploye = new EMPLOYEES();
                        SelectedEmploye.FULLNAME = FULLENAME;
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
                        DateTime NewDate = DateTime.Parse(DATE);
                        AVANCES nouvelleAvance = new AVANCES();
                        nouvelleAvance.EMPLOYEE = SelectedEmploye.ID;
                        nouvelleAvance.EMPLOYEES = SelectedEmploye;
                        nouvelleAvance.MONTANT = !string.IsNullOrEmpty(MONTANT) ? decimal.Parse(MONTANT) : 0;
                        nouvelleAvance.DATE = NewDate;
                        nouvelleAvance.MOIS = NewDate.Month;
                        nouvelleAvance.ANNEE = NewDate.Year;
                        nouvelleAvance.TYPE = TYPE;
                        BD.AVANCES.Add(nouvelleAvance);
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
        public string GetEmployeeByID(int ID)
        {
            EMPLOYEES Employee = BD.EMPLOYEES.Find(ID);
            string Result = string.Empty;
            if (Employee != null)
                Result = Employee.NUMERO;
            return Result;
        }
        public JsonResult GetAllEmployee()
        {
            BD.Configuration.ProxyCreationEnabled = false;
            List<EMPLOYEES> ListeEmployee = BD.EMPLOYEES.ToList();
            return Json(ListeEmployee, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SendForm(string Mode, string Code)
        {
            string MATRICULE = Request.Params["MATRICULE"] != null ? Request.Params["MATRICULE"].ToString() : string.Empty;
            string employee = Request.Params["employee"] != null ? Request.Params["employee"].ToString() : string.Empty;
            string MONTANT = Request.Params["MONTANT"] != null ? Request.Params["MONTANT"].ToString() : "0";
            string DATE = Request.Params["DATE"] != null ? Request.Params["DATE"].ToString() : string.Empty;
            string TYPE = Request.Params["TYPE"] != null ? Request.Params["TYPE"].ToString() : string.Empty;

            DateTime SelectedDate = DateTime.Parse(DATE);
            int ID = int.Parse(employee);
            EMPLOYEES SelectedEmployee = BD.EMPLOYEES.Find(ID);
            if (Mode == "Create")
            {
                AVANCES NouvelleAvance = new AVANCES();
                NouvelleAvance.DATE = SelectedDate;
                NouvelleAvance.ANNEE = SelectedDate.Year;
                NouvelleAvance.MOIS = SelectedDate.Month;
                NouvelleAvance.MONTANT = decimal.Parse(MONTANT);
                NouvelleAvance.EMPLOYEES = SelectedEmployee;
                NouvelleAvance.EMPLOYEE = ID;
                NouvelleAvance.TYPE = TYPE;
                BD.AVANCES.Add(NouvelleAvance);
                BD.SaveChanges();
            }
            if (Mode == "Edit")
            {
                int SeletedAvanceID = int.Parse(Code);
                AVANCES SelectedAvance = BD.AVANCES.Find(SeletedAvanceID);
                SelectedAvance.DATE = SelectedDate;
                SelectedAvance.ANNEE = SelectedDate.Year;
                SelectedAvance.MOIS = SelectedDate.Month;
                SelectedAvance.MONTANT = decimal.Parse(MONTANT);
                SelectedAvance.EMPLOYEES = SelectedEmployee;
                SelectedAvance.EMPLOYEE = ID;
                SelectedAvance.TYPE = TYPE;
                BD.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int Code)
        {
            AVANCES Selected = BD.AVANCES.Find(Code);
            BD.AVANCES.Remove(Selected);
            BD.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
