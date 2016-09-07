using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestionCommerciale.Models
{
    public class ANNEXE_6
    {
        public int ID { get; set; }
        public Nullable<int> DECLARATION { get; set; }
        public int TYPE_BENEFICIAIRE { get; set; }
        public string IDENTIFIANT { get; set; }
        public string NOM_PRENOM { get; set; }
        public string ACTIVITE { get; set; }
        public string ADRESSE { get; set; }
        public decimal MONTANT_RISTOURNES { get; set; }
        public decimal MONTANT_VENTE_PP { get; set; }
        public decimal MONTANT_AVANCE_VENTE_PP { get; set; }
        public decimal MONTANT_ESPECES_MARCHANDISES { get; set; }
        [ForeignKey("DECLARATION")]
        public virtual DECLARATIONS_EMPLOYEURS DECLARATIONS_EMPLOYEURS { get; set; }
    }
}