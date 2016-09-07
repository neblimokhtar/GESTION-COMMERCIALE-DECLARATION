using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestionCommerciale.Models
{
    public class ANNEXE_7
    {
        public int ID { get; set; }
        public Nullable<int> DECLARATION { get; set; }
        public int TYPE_BENEFICIAIRE { get; set; }
        public string IDENTIFIANT { get; set; }
        public string NOM_PRENOM { get; set; }
        public string ACTIVITE { get; set; }
        public string ADRESSE { get; set; }
        public int TYPE_MONTANT { get; set; }
        public decimal MONTANT_PAYES { get; set; }
        public decimal RETENUE_SOURCE { get; set; }
        public decimal NET_SERVI { get; set; }
        [ForeignKey("DECLARATION")]
        public virtual DECLARATIONS_EMPLOYEURS DECLARATIONS_EMPLOYEURS { get; set; }
    }
}