using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestionCommerciale.Models
{
    public class LIGNES_DECLARATIONS_FACS
    {
        public int ID { get; set; }
        public string NUMERO_AUTORISATION { get; set; }
        public DateTime DATE_AUTORISATION { get; set; }
        public int TYPE_CLIENT { get; set; }
        public Nullable<int> CLIENT { get; set; }
        public string NUMERO_FACTURE { get; set; }
        public DateTime DATE_FACTURE { get; set; }
        public decimal PRIX_HT { get; set; }
        public int TVA { get; set; }
        public decimal MONTANT_TVA { get; set; }
        public string OBJET { get; set; }
        public Nullable<int> DECLARATION_FAC { get; set; }
        public int NUMERO_ORDRE { get; set; }
        public int FODEC { get; set; }
        public decimal MONTANT_FODEC { get; set; }
        public int DROIT_CONSOMMATION { get; set; }
        public decimal MONTANT_DROIT_CONSOMMATION { get; set; }
        [ForeignKey("CLIENT")]
        public virtual CLIENTS CLIENTS { get; set; }
        [ForeignKey("DECLARATION_FAC")]
        public virtual DECLARATIONS_FACS DECLARATIONS_FACS { get; set; }
    }
}