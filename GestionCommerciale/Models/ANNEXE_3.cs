using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestionCommerciale.Models
{
    public class ANNEXE_3
    {
        public int ID { get; set; }
        public Nullable<int> DECLARATION { get; set; }
        public Nullable<int> TYPE { get; set; }
        public string IDENTIFIANT { get; set; }
        public string NOM_PRENOM { get; set; }
        public string ACTIVITE { get; set; }
        public string ADRESSE { get; set; }
        public decimal INTERET_COMPTES_CENT { get; set; }
        public decimal INTERET_CAPITAUX_MOBILIERES { get; set; }
        public decimal INTERET_PRETS { get; set; }
        public decimal MONTANT_RETENUE { get; set; }
        public decimal REDEVANCE_CGC { get; set; }
        public decimal NET_SERVI { get; set; }
        [ForeignKey("DECLARATION")]
        public virtual DECLARATIONS_EMPLOYEURS DECLARATIONS_EMPLOYEURS { get; set; }
    }
}