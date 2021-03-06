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
    public partial class COMMANDES_FOURNISSEURS
    {
        public COMMANDES_FOURNISSEURS()
        {
            this.BONS_RECEPTIONS_FOURNISSEURS = new HashSet<BONS_RECEPTIONS_FOURNISSEURS>();
            this.FACTURES_FOURNISSEURS = new HashSet<FACTURES_FOURNISSEURS>();
            this.LIGNES_COMMANDES_FOURNISSEURS = new HashSet<LIGNES_COMMANDES_FOURNISSEURS>();
        }
    
        public int ID { get; set; }
        public string CODE { get; set; }
        public System.DateTime DATE { get; set; }
        public string MODE_PAIEMENT { get; set; }
        public int FOURNISSEUR { get; set; }
        public decimal THT { get; set; }
        public decimal TTVA { get; set; }
        public decimal NHT { get; set; }
        public decimal REMISE { get; set; }
        public decimal TTC { get; set; }
        public decimal TNET { get; set; }
        public bool VALIDER { get; set; }
    
        public virtual ICollection<BONS_RECEPTIONS_FOURNISSEURS> BONS_RECEPTIONS_FOURNISSEURS { get; set; }
        [ForeignKey("FOURNISSEUR")]
        public virtual FOURNISSEURS FOURNISSEURS { get; set; }
        public virtual ICollection<FACTURES_FOURNISSEURS> FACTURES_FOURNISSEURS { get; set; }
        public virtual ICollection<LIGNES_COMMANDES_FOURNISSEURS> LIGNES_COMMANDES_FOURNISSEURS { get; set; }
    }
    
}
