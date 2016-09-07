using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestionCommerciale.Models
{
    public class AVANCES
    {
        public int ID { get; set; }
        public Nullable<int> EMPLOYEE { get; set; }
        public int MOIS { get; set; }
        public int ANNEE { get; set; }
        public DateTime DATE { get; set; }
        public decimal MONTANT { get; set; }
        public string TYPE { get; set; }
        [ForeignKey("EMPLOYEE")]
        public virtual EMPLOYEES EMPLOYEES { get; set; }
    }
}