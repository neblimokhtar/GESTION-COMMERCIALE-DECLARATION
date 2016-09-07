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
    public partial class LIGNES_BONS_RECEPTIONS_FOURNISSEURS
    {
        public int ID { get; set; }
        public Nullable<int> PRODUIT { get; set; }
        public string CODE_PRODUIT { get; set; }
        public string DESIGNATION_PRODUIT { get; set; }
        public Nullable<int> QUANTITE { get; set; }
        public Nullable<decimal> PRIX_UNITAIRE_HT { get; set; }
        public Nullable<decimal> REMISE { get; set; }
        public Nullable<decimal> TOTALE_REMISE_HT { get; set; }
        public Nullable<decimal> TOTALE_HT { get; set; }
        public Nullable<int> TVA { get; set; }
        public Nullable<decimal> TOTALE_TTC { get; set; }
        public Nullable<int> BON_RECEPTION_FOURNISSEUR { get; set; }
        [ForeignKey("BON_RECEPTION_FOURNISSEUR")]
        public virtual BONS_RECEPTIONS_FOURNISSEURS BONS_RECEPTIONS_FOURNISSEURS { get; set; }
        [ForeignKey("PRODUIT")]
        public virtual PRODUITS PRODUITS { get; set; }
    }

}
