using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestionCommerciale.Models
{
    public partial class ANNEXE_4
    {
        public int ID { get; set; }
        public Nullable<int> DECLARATION { get; set; }
        public int TYPE_BENEFICIAIRE { get; set; }
        public string IDENTIFIANT { get; set; }
        public string NOM_PRENOM { get; set; }
        public string ACTIVITE { get; set; }
        public string ADRESSE { get; set; }
        public int TYPE_MONTANT { get; set; }
        public decimal TAUX_MONTANT_HONORAIRE { get; set; }
        public decimal MONTANT_BRUT_HONORAIRE { get; set; }
        public decimal TAUX_HONORAIRE_6_MOIS { get; set; }
        public decimal MONTANT_HONORAIRE_6_MOIS { get; set; }
        public decimal TAUX_PLUS_VALUE_IMMOBILIERE { get; set; }
        public decimal MONTANT_PLUS_VALUE_IMMOBILIERE { get; set; }
        public decimal TAUX_REVENUES_IMMOBILIERE { get; set; }
        public decimal MONTANT_REVENUE_IMMOBILIERE { get; set; }
        public decimal TAUX_CESSION_ACTION { get; set; }
        public decimal MONTANT_CESSION_ACTION { get; set; }
        public decimal MONTANT_RETENUE { get; set; }
        public int TYPE_MONTANT_OP_EXPORTATION { get; set; }
        public decimal MONTANT_OP_EXPORTATION { get; set; }
        public decimal MONTANT_PARADIS_FISCAUX { get; set; }
        public decimal NET_SERVI { get; set; }
        [ForeignKey("DECLARATION")]
        public virtual DECLARATIONS_EMPLOYEURS DECLARATIONS_EMPLOYEURS { get; set; }
    }
}