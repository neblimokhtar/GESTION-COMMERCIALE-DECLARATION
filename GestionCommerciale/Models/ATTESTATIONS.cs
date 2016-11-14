using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestionCommerciale.Models
{
    public class ATTESTATIONS
    {
        public int ID { get; set; }
        public string LOCALITE { get; set; }
        public DateTime DATE { get; set; }
        public int NUMERO { get; set; }
        public string REFERENCE { get; set; }
        public string SIGNE_PAR { get; set; }
        public string QUALIFICATION { get; set; }
        public bool DECHARGE { get; set; }
        public string COMMENTAIRE { get; set; }
        public Nullable<int> MODELE { get; set; }
        public Nullable<int> EMPLOYE { get; set; }
        [ForeignKey("EMPLOYE")]
        public virtual EMPLOYEES EMPLOYEES { get; set; }
        [ForeignKey("MODELE")]
        public virtual MODELES_ATTESTATIONS MODELES_ATTESTATIONS { get; set; }
    }
}