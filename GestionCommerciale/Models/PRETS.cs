using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestionCommerciale.Models
{
    public class PRETS
    {
        public int ID { get; set; }
        public string CODE { get; set; }
        public Nullable<int> EMPLOYEE { get; set; }
        public decimal MONTANT { get; set; }
        public decimal RECU { get; set; }
        public decimal RESTE { get; set; }
        public DateTime DATE { get; set; }
        public DateTime DATE_ECHEANCE { get; set; }
        public int NBR_MOIS { get; set; }
        public string TYPE { get; set; }
        public string STATUT { get; set; }
        
        [ForeignKey("EMPLOYEE")]
        public virtual EMPLOYEES EMPLOYEES { get; set; }
        public virtual ICollection<TRANCHES_PRETS> TRANCHES_PRETS { get; set; }
    }
}