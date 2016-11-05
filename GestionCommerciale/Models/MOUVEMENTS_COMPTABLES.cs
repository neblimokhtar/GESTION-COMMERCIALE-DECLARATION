using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestionCommerciale.Models
{
    public class MOUVEMENTS_COMPTABLES
    {
        public int ID { get; set; }
        public int NUMERO { get; set; }
        public Nullable<int> EMPLOYEE { get; set; }
        public System.DateTime DATE { get; set; }
        public System.DateTime DATE_AFFECATION { get; set; }
        public string JOURNAL { get; set; }
        public string NUM_PIECE { get; set; }
        public string LIBELLE { get; set; }
        public decimal MONTANT { get; set; }
        public string ACTION { get; set; }

        [ForeignKey("EMPLOYEE")]
        public virtual EMPLOYEES EMPLOYEES { get; set; }
    }
}