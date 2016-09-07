using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestionCommerciale.Models
{
    public partial class ANNEXE_1
    {
        public int ID { get; set; }
        public Nullable<int> DECLARATION { get; set; }
        public Nullable<int> EMPLOYEE { get; set; }
        public DateTime DATE_DEBUT { get; set; }
        public DateTime DATE_FIN { get; set; }
        public int DUREE { get; set; }
        public decimal REVENU_IMPOSABLE { get; set; }
        public decimal AVANTAGE_EN_NATURE { get; set; }
        public decimal BRUT { get; set; }
        public decimal MONTANT_REINVESTI { get; set; }
        public decimal RETENUE_IRPP { get; set; }
        public decimal RETENUE_20 { get; set; }
        public decimal REDEVANCE { get; set; }
        public decimal CONTRIBUTION_CONJONTURELLE { get; set; }
        public decimal NET_SERVI { get; set; }
        public int TYPE { get; set; }
        [ForeignKey("DECLARATION")]
        public virtual DECLARATIONS_EMPLOYEURS DECLARATIONS_EMPLOYEURS { get; set; }
        [ForeignKey("EMPLOYEE")]
        public virtual EMPLOYEES EMPLOYEES { get; set; }
    }
}