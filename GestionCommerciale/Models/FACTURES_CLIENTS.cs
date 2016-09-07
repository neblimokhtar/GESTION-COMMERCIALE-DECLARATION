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
    public partial class FACTURES_CLIENTS
    {
        public FACTURES_CLIENTS()
        {
            this.AVOIRS_CLIENTS = new HashSet<AVOIRS_CLIENTS>();
            this.LIGNES_FACTURES_CLIENTS = new HashSet<LIGNES_FACTURES_CLIENTS>();
        }
    
        public int ID { get; set; }
        public string CODE { get; set; }
        public System.DateTime DATE { get; set; }
        public string MODE_PAIEMENT { get; set; }
        public int CLIENT { get; set; }
        public decimal THT { get; set; }
        public decimal TTVA { get; set; }
        public decimal NHT { get; set; }
        public decimal TTC { get; set; }
        public decimal TNET { get; set; }
        public decimal REMISE { get; set; }
        public decimal TIMBRE { get; set; }
        public bool VALIDER { get; set; }
        public bool PAYEE { get; set; }
        public Nullable<int> COMMANDE_CLIENT { get; set; }
        public Nullable<int> DEVIS_CLIENT { get; set; }
        public Nullable<int> BON_LIVRAISON_CLIENT { get; set; }
    
        public virtual ICollection<AVOIRS_CLIENTS> AVOIRS_CLIENTS { get; set; }
        [ForeignKey("BON_LIVRAISON_CLIENT")]
        public virtual BONS_LIVRAISONS_CLIENTS BONS_LIVRAISONS_CLIENTS { get; set; }
        [ForeignKey("CLIENT")]
        public virtual CLIENTS CLIENTS { get; set; }
        [ForeignKey("COMMANDE_CLIENT")]
        public virtual COMMANDES_CLIENTS COMMANDES_CLIENTS { get; set; }
        [ForeignKey("DEVIS_CLIENT")]
        public virtual DEVIS_CLIENTS DEVIS_CLIENTS { get; set; }
        public virtual ICollection<LIGNES_FACTURES_CLIENTS> LIGNES_FACTURES_CLIENTS { get; set; }
    }
    
}
