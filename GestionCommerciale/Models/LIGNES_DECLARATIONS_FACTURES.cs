using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestionCommerciale.Models
{
    public class LIGNES_DECLARATIONS_FACTURES
    {
        public int ID { get; set; }
        public string NUMERO_AUTORISATION { get; set; }
        public string NUMERO_BC { get; set; }
        public DateTime DATE_BC { get; set; }
        public Nullable<int> FOURNISSEUR { get; set; }
        public string NUMERO_FACTURE { get; set; }
        public DateTime DATE_FACTURE { get; set; }
        public decimal PRIX_HT { get; set; }
        public int TVA { get; set; }
        public decimal MONTANT_TVA { get; set; }
        public string OBJET { get; set; }
        public Nullable<int> DECLARATION_FACTURE { get; set; }
        public int NUMERO_ORDRE { get; set; }
        [ForeignKey("DECLARATION_FACTURE")]
        public virtual DECLARATIONS_FACTURES DECLARATIONS_FACTURES { get; set; }
        [ForeignKey("FOURNISSEUR")]
        public virtual FOURNISSEURS FOURNISSEURS { get; set; }
    }
}