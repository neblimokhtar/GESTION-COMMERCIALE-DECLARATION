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
using System.ComponentModel.DataAnnotations;

namespace GestionCommerciale.Models
{
    public partial class PARAMETRES
    {
        [Key]
        public string LOGIN { get; set; }
        public string PASSWORD { get; set; }
        public byte[] ANNEXE_1 { get; set; }
        public byte[] ANNEXE_2 { get; set; }
        public byte[] ANNEXE_3 { get; set; }
        public byte[] ANNEXE_4 { get; set; }
        public byte[] ANNEXE_5 { get; set; }
        public byte[] ANNEXE_6 { get; set; }
        public byte[] ANNEXE_7 { get; set; }
        public byte[] RECAP { get; set; }
        public byte[] MODELE_AVANCE { get; set; }
        public byte[] MODELE_PRET { get; set; }

    }

}
