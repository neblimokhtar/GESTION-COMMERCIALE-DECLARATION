using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestionCommerciale.Models
{
    public class TRANCHES_PRETS
    {
        public int ID { get; set; }
        public Nullable<int> PRET { get; set; }
        public decimal MONTANT { get; set; }
        public DateTime DATE { get; set; }
        public string STATUT { get; set; }
        [ForeignKey("PRET")]
        public virtual PRETS PRETS { get; set; }
    }
}