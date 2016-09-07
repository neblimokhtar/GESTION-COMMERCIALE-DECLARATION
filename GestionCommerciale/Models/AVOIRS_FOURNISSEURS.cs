//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionCommerciale.Models
{
    public partial class AVOIRS_FOURNISSEURS
    {
        public AVOIRS_FOURNISSEURS()
        {
            this.LIGNES_AVOIRS_FOURNISSEURS = new HashSet<LIGNES_AVOIRS_FOURNISSEURS>();
        }
    
        public int ID { get; set; }
        public string CODE { get; set; }
        public System.DateTime DATE { get; set; }
        public string MODE_PAIEMENT { get; set; }
        public int FOURNISSEUR { get; set; }
        public decimal THT { get; set; }
        public decimal TTVA { get; set; }
        public decimal NHT { get; set; }
        public decimal TTC { get; set; }
        public decimal TNET { get; set; }
        public bool VALIDER { get; set; }
        public decimal REMISE { get; set; }
        public Nullable<int> FACTURE_FOURNISSEUR { get; set; }

        [ForeignKey("FACTURE_FOURNISSEUR")]
        public virtual FACTURES_FOURNISSEURS FACTURES_FOURNISSEURS { get; set; }
        public virtual ICollection<LIGNES_AVOIRS_FOURNISSEURS> LIGNES_AVOIRS_FOURNISSEURS { get; set; }
        [ForeignKey("FOURNISSEUR")]
        public virtual FOURNISSEURS FOURNISSEURS { get; set; }
    }
    
}
