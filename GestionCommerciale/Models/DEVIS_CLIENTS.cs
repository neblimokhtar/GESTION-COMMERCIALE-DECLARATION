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
    public partial class DEVIS_CLIENTS
    {
        public DEVIS_CLIENTS()
        {
            this.COMMANDES_CLIENTS = new HashSet<COMMANDES_CLIENTS>();
            this.FACTURES_CLIENTS = new HashSet<FACTURES_CLIENTS>();
            this.LIGNES_DEVIS_CLIENTS = new HashSet<LIGNES_DEVIS_CLIENTS>();
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
        public bool VALIDER { get; set; }
        public decimal REMISE { get; set; }
        [ForeignKey("CLIENT")]
        public virtual CLIENTS CLIENTS { get; set; }
        public virtual ICollection<COMMANDES_CLIENTS> COMMANDES_CLIENTS { get; set; }
        public virtual ICollection<FACTURES_CLIENTS> FACTURES_CLIENTS { get; set; }
        public virtual ICollection<LIGNES_DEVIS_CLIENTS> LIGNES_DEVIS_CLIENTS { get; set; }
    }

}
