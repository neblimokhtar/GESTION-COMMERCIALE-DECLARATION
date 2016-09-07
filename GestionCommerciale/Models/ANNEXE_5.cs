using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestionCommerciale.Models
{
    public partial class ANNEXE_5
    {
        public int ID { get; set; }
        public Nullable<int> DECLARATION { get; set; }
        public Nullable<int> TYPE_BENEFICIAIRE { get; set; }
        public string IDENTIFIANT { get; set; }
        public string NOM_PRENOM { get; set; }
        public string ACTIVITE { get; set; }
        public string ADRESSE { get; set; }
        public decimal MONTANT_MARCHES { get; set; }
        public decimal RETENUE { get; set; }
        public decimal TVA_ETAT { get; set; }
        public decimal REVENUE_TVA { get; set; }
        public decimal MONTANT_ETABLISSEMENTS_PUBLICS { get; set; }
        public decimal RETENUE_ETABLISSEMENT_PUBLICS { get; set; }
        public decimal MONTANT_MARCHES_ETRANGES { get; set; }
        public decimal RETENUE_MARCHES_ETRANGES { get; set; }
        public decimal REDEVANCE_CGC { get; set; }
        public decimal NET_SERVI { get; set; }
        
        public int TYPE { get; set; }
        [ForeignKey("DECLARATION")]
        public virtual DECLARATIONS_EMPLOYEURS DECLARATIONS_EMPLOYEURS { get; set; }
    }
}