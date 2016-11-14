using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestionCommerciale.Models;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;


namespace GestionCommerciale.Controllers
{
    public class AttestationController : Controller
    {
        //
        // GET: /Attestation/
        GestionCommercialeEntity BD = new GestionCommercialeEntity();

        public ActionResult Modeles()
        {
            List<MODELES_ATTESTATIONS> liste = BD.MODELES_ATTESTATIONS.ToList();
            dynamic Variable = (from Element in liste
                                select new
                                {
                                    ID = Element.ID,
                                    TITRE = Element.TITRE,
                                    DESCRIPTION = GetDescription(Element.ID)
                                }).AsEnumerable().Select(c => c.ToExpando());
            return View(Variable);
        }
        public string GetDescription(int ID)
        {
            string Result = string.Empty;
            List<CHAMPS_MODELES> liste = BD.CHAMPS_MODELES.Where(Element => Element.MODELES_ATTESTATIONS.ID == ID).OrderBy(Element => Element.ORDRE).ToList();
            foreach (CHAMPS_MODELES Element in liste)
            {
                if (Element.TYPE == "TEXT")
                {
                    if (Element.VALEUR.Trim() == string.Empty)
                    {
                        Result += Environment.NewLine;
                        Result += " ┘ ";
                    }

                    Result += " " + Element.VALEUR;
                }
                if (Element.TYPE == "TABLE")
                {
                    Result += " [" + Element.VALEUR + "]";
                }
            }
            return Result;
        }
        public ActionResult FormModele(string Mode, int Code)
        {
            MODELES_ATTESTATIONS Element = new MODELES_ATTESTATIONS();
            if (Mode == "Create")
            {
                ViewBag.TITRE_PAGE = "AJOUTER UN NOUVEAU MODELE";
            }
            if (Mode == "Edit")
            {
                Element = BD.MODELES_ATTESTATIONS.Find(Code);
                ViewBag.TITRE_PAGE = "MODIFIER UN  MODELE";
            }
            ViewBag.Mode = Mode;
            ViewBag.Code = Code;
            return View(Element);
        }
        [HttpPost]
        public ActionResult SendFormModele(string Mode, string Code)
        {
            string TITRE = Request.Params["TITRE"] != null ? Request.Params["TITRE"].ToString() : string.Empty;
            if (Mode == "Create")
            {
                MODELES_ATTESTATIONS NewElement = new MODELES_ATTESTATIONS();
                NewElement.TITRE = TITRE;
                BD.MODELES_ATTESTATIONS.Add(NewElement);
                BD.SaveChanges();
            }
            if (Mode == "Edit")
            {
                int ID = int.Parse(Code);
                MODELES_ATTESTATIONS SelectedElement = BD.MODELES_ATTESTATIONS.Find(ID);
                SelectedElement.TITRE = TITRE;
                BD.SaveChanges();
            }
            return RedirectToAction("Modeles");
        }
        public ActionResult Champs(string Filter)
        {
            int ID = int.Parse(Filter);
            List<CHAMPS_MODELES> liste = BD.CHAMPS_MODELES.Where(Element => Element.MODELES_ATTESTATIONS.ID == ID).ToList();
            ViewBag.TITRE = BD.MODELES_ATTESTATIONS.Find(ID).TITRE;
            ViewBag.Filter = Filter;
            return View(liste);
        }
        public ActionResult FormChamp(string Mode, int Code, int Filter)
        {
            CHAMPS_MODELES Element = new CHAMPS_MODELES();
            int ordre = 1;
            if (Mode == "Create")
            {
                ViewBag.TITRE_PAGE = "AJOUTER UN NOUVEAU CHAMP";
                List<CHAMPS_MODELES> liste = BD.CHAMPS_MODELES.Where(Elt => Elt.MODELES_ATTESTATIONS.ID == Filter).ToList();
                if (liste.Count > 0)
                {
                    ordre = liste.Select(Elt => Elt.ORDRE).Max();
                    ordre++;
                }
            }
            if (Mode == "Edit")
            {
                Element = BD.CHAMPS_MODELES.Find(Code);
                ViewBag.TITRE_PAGE = "MODIFIER UN CHAMP";
                ordre = Element.ORDRE;
            }
            ViewBag.Mode = Mode;
            ViewBag.Code = Code;
            ViewBag.ordre = ordre;

            ViewBag.Filter = Filter;
            return View(Element);
        }
        [HttpPost]
        public ActionResult SendFormChamp(string Mode, string Code, string Filter)
        {
            string ORDRE = Request.Params["ORDRE"] != null ? Request.Params["ORDRE"].ToString() : string.Empty;
            string TYPE = Request.Params["TYPE"] != null ? Request.Params["TYPE"].ToString() : string.Empty;
            string VALEUR = Request.Params["VALEUR"] != null ? Request.Params["VALEUR"].ToString() : string.Empty;
            int ID = int.Parse(Filter);
            MODELES_ATTESTATIONS MonModel = BD.MODELES_ATTESTATIONS.Find(ID);
            int ordre = int.Parse(ORDRE);
            if (Mode == "Create")
            {
                CHAMPS_MODELES NewElement = new CHAMPS_MODELES();
                NewElement.ORDRE = ordre;
                NewElement.TYPE = TYPE;
                NewElement.VALEUR = VALEUR;
                NewElement.MODELE = ID;
                NewElement.MODELES_ATTESTATIONS = MonModel;
                BD.CHAMPS_MODELES.Add(NewElement);
                BD.SaveChanges();
            }
            if (Mode == "Edit")
            {
                int IDEN = int.Parse(Code);
                CHAMPS_MODELES SelectedElement = BD.CHAMPS_MODELES.Find(IDEN);
                SelectedElement.ORDRE = ordre;
                SelectedElement.TYPE = TYPE;
                SelectedElement.VALEUR = VALEUR;
                SelectedElement.MODELE = ID;
                SelectedElement.MODELES_ATTESTATIONS = MonModel;
                BD.SaveChanges();
            }
            return RedirectToAction("Champs", "Attestation", new { Filter = Filter });
        }
        public ActionResult DeleteChamp(int Code, int Filter)
        {
            CHAMPS_MODELES Selected = BD.CHAMPS_MODELES.Find(Code);
            BD.CHAMPS_MODELES.Remove(Selected);
            BD.SaveChanges();
            return RedirectToAction("Champs", "Attestation", new { Filter = Filter });
        }
        public ActionResult DeleteModele(int Code, int Filter)
        {
            BD.CHAMPS_MODELES.Where(p => p.MODELES_ATTESTATIONS.ID == Code).ToList().ForEach(p => BD.CHAMPS_MODELES.Remove(p));
            BD.SaveChanges();
            MODELES_ATTESTATIONS Facture = BD.MODELES_ATTESTATIONS.Where(cmd => cmd.ID == Code).FirstOrDefault();
            BD.MODELES_ATTESTATIONS.Remove(Facture);
            BD.SaveChanges();
            return RedirectToAction("Modeles");
        }
        public ActionResult Index()
        {
            List<ATTESTATIONS> liste = BD.ATTESTATIONS.ToList();
            return View(liste);
        }
        public ActionResult FormAttestation(string Mode, int Code)
        {
            ATTESTATIONS Element = new ATTESTATIONS();
            if (Mode == "Create")
            {
                ViewBag.TITRE_PAGE = "AJOUTER UNE NOUVELLE ATTESTATION";
                ViewBag.DATE = DateTime.Today.ToShortDateString();
                int Max = 1;
                List<ATTESTATIONS> liste = BD.ATTESTATIONS.Where(Elt => Elt.DATE.Year == DateTime.Today.Year).ToList();
                if (liste.Count > 0)
                {
                    Max = liste.Select(Elt => Elt.NUMERO).Max();
                    Max++;
                }
                ViewBag.REFERENCE = "ATT" + Max.ToString("000") + "/" + DateTime.Today.Year;
            }
            if (Mode == "Edit")
            {
                Element = BD.ATTESTATIONS.Find(Code);
                ViewBag.TITRE_PAGE = "MODIFIER UNE ATTESTATION";
                ViewBag.DATE = Element.DATE.ToShortDateString();
                ViewBag.REFERENCE = Element.REFERENCE;
            }
            ViewBag.Mode = Mode;
            ViewBag.Code = Code;
            return View(Element);
        }
        [HttpPost]
        public ActionResult SendFormAttestation(string Mode, string Code)
        {
            string REFERENCE = Request.Params["REFERENCE"] != null ? Request.Params["REFERENCE"].ToString() : string.Empty;
            string MODELE = Request.Params["MODELE"] != null ? Request.Params["MODELE"].ToString() : string.Empty;
            string employee = Request.Params["employee"] != null ? Request.Params["employee"].ToString() : string.Empty;
            string LOCALITE = Request.Params["LOCALITE"] != null ? Request.Params["LOCALITE"].ToString() : string.Empty;
            string DATE = Request.Params["DATE"] != null ? Request.Params["DATE"].ToString() : string.Empty;
            string SIGNE_PAR = Request.Params["SIGNE_PAR"] != null ? Request.Params["SIGNE_PAR"].ToString() : string.Empty;
            string DECHARGE = Request.Params["DECHARGE"] != null ? "true" : "false";
            string COMMENTAIRE = Request.Params["COMMENTAIRE"] != null ? Request.Params["COMMENTAIRE"].ToString() : "";


            DateTime SelectedDate = DateTime.Parse(DATE);
            int ID = int.Parse(employee);
            EMPLOYEES SelectedEmployee = BD.EMPLOYEES.Find(ID);

            int SelectedModel = int.Parse(MODELE);
            MODELES_ATTESTATIONS Model = BD.MODELES_ATTESTATIONS.Find(SelectedModel);
            if (Mode == "Create")
            {
                int Max = 1;
                List<ATTESTATIONS> liste = BD.ATTESTATIONS.Where(Elt => Elt.DATE.Year == DateTime.Today.Year).ToList();
                if (liste.Count > 0)
                {
                    Max = liste.Select(Elt => Elt.NUMERO).Max();
                    Max++;
                }

                ATTESTATIONS NewElement = new ATTESTATIONS();
                NewElement.EMPLOYE = ID;
                NewElement.EMPLOYEES = SelectedEmployee;
                NewElement.DATE = SelectedDate;
                NewElement.REFERENCE = REFERENCE;
                NewElement.NUMERO = Max;
                NewElement.MODELE = SelectedModel;
                NewElement.MODELES_ATTESTATIONS = Model;
                NewElement.LOCALITE = LOCALITE;
                NewElement.COMMENTAIRE = COMMENTAIRE != "undefined" ? COMMENTAIRE : string.Empty;
                NewElement.SIGNE_PAR = SIGNE_PAR;
                NewElement.DECHARGE = Boolean.Parse(DECHARGE);
                BD.ATTESTATIONS.Add(NewElement);
                BD.SaveChanges();
            }
            if (Mode == "Edit")
            {
                int SeletedPretID = int.Parse(Code);
                ATTESTATIONS NewElement = BD.ATTESTATIONS.Find(SeletedPretID);
                NewElement.EMPLOYE = ID;
                NewElement.EMPLOYEES = SelectedEmployee;
                NewElement.DATE = SelectedDate;
                //NewElement.REFERENCE = REFERENCE;
                //NewElement.NUMERO = Max;
                NewElement.MODELE = SelectedModel;
                NewElement.MODELES_ATTESTATIONS = Model;
                NewElement.LOCALITE = LOCALITE;
                NewElement.COMMENTAIRE = COMMENTAIRE != "undefined" ? COMMENTAIRE : string.Empty;
                NewElement.SIGNE_PAR = SIGNE_PAR;
                NewElement.DECHARGE = Boolean.Parse(DECHARGE);
                BD.SaveChanges();

            }
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int Code)
        {
            ATTESTATIONS Selected = BD.ATTESTATIONS.Find(Code);
            BD.ATTESTATIONS.Remove(Selected);
            BD.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult GetFileByID(string id)
        {
            int ID = int.Parse(id);
            ATTESTATIONS attestation = BD.ATTESTATIONS.Find(ID);
            dynamic dt = new
            {
                LOCALITE = attestation.LOCALITE,
                DATE = attestation.DATE.ToShortDateString(),
                REFERENCE = attestation.REFERENCE,
                TITRE = attestation.MODELES_ATTESTATIONS.TITRE,
                DESCRIPTION = GetDescriptionByEmployee(ID),
                SIGNE_PAR = attestation.SIGNE_PAR,
            };

            ReportDocument rptH = new ReportDocument();
            string FileName = Server.MapPath("/Reports/ATTESTATION.rpt");
            rptH.Load(FileName);
            rptH.SetDataSource(new[] { dt });
            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }
        public string GetDescriptionByEmployee(int id)
        {
            string Result = string.Empty;
            ATTESTATIONS attestation = BD.ATTESTATIONS.Find(id);
            EMPLOYEES employee = attestation.EMPLOYEES;
            MODELES_ATTESTATIONS model = attestation.MODELES_ATTESTATIONS;
            List<CHAMPS_MODELES> liste = BD.CHAMPS_MODELES.Where(Element => Element.MODELES_ATTESTATIONS.ID == model.ID).OrderBy(Element => Element.ORDRE).ToList();
            foreach (CHAMPS_MODELES Element in liste)
            {
                if (Element.TYPE == "TEXT")
                {
                    if (Element.VALEUR.Trim() == string.Empty)
                    {
                        Result += Environment.NewLine;
                        Result += Environment.NewLine;
                    }

                    Result += Element.VALEUR + " ";
                }
                if (Element.TYPE == "TABLE")
                {
                    if (Element.VALEUR == "CIN")
                    {
                        Result += employee.CIN + " ";
                    }
                    if (Element.VALEUR == "FULLNAME")
                    {
                        Result += employee.FULLNAME + " ";
                    }
                    if (Element.VALEUR == "NUM_ASS_SOC")
                    {
                        Result += employee.NUM_ASS_SOC + " ";
                    }
                    if (Element.VALEUR == "QUALIFICATION")
                    {
                        Result += employee.QUALIFICATION + " ";
                    }
                    if (Element.VALEUR == "NUMERO")
                    {
                        Result += employee.NUMERO + " ";
                    }
                    if (Element.VALEUR == "ADRESSE")
                    {
                        Result += employee.ADRESSE + " ";
                    }
                    if (Element.VALEUR == "CIVILITE")
                    {
                        Result += employee.CIVILITE + " ";
                    }
                    if (Element.VALEUR == "SALAIRE")
                    {
                        Result += employee.SALAIRE + " ";
                    }
                    if (Element.VALEUR == "DEMARRAGE")
                    {
                        Result += employee.DEMARRAGE + " ";
                    }
                }
            }
            return Result;
        }
    }
}
