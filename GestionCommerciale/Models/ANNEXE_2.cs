using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestionCommerciale.Models
{
    public partial class ANNEXE_2
    {
        public int ID { get; set; }
        public Nullable<int> DECLARATION { get; set; }
        public int TYPE_MONTANT_OPERATION_EXPORTATION { get; set; }
        public string IDENTIFIANT { get; set; }
        public string NOM_PRENOM { get; set; }
        public string ACTIVITE { get; set; }
        public string ADRESSE { get; set; }
        public int TYPE_MONTANT { get; set; }
        public decimal HONORAIRE_NON_COMMERCIALE { get; set; }
        public decimal HONORAIRE_SOCIETE { get; set; }
        public decimal JETON { get; set; }
        public decimal REMUNERATION { get; set; }
        public decimal PLUS_VALUE_IMMOBILIERE { get; set; }
        public decimal HOTEL { get; set; }
        public decimal ARTISTES { get; set; }
        public decimal BUREAU_ETUDE_EXPORTATEUR { get; set; }
        public decimal AUTRES { get; set; }
        public decimal RETENUE { get; set; }
        public decimal REDEVANCE_CGC { get; set; }
        public decimal NET_SERVI { get; set; }
        public int TYPE { get; set; }
        [ForeignKey("DECLARATION")]
        public virtual DECLARATIONS_EMPLOYEURS DECLARATIONS_EMPLOYEURS { get; set; }
    }
}