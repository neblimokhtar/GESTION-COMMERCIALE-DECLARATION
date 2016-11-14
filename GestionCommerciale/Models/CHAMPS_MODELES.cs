using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestionCommerciale.Models
{
    public class CHAMPS_MODELES
    {
        public int ID { get; set; }
        public Nullable<int> MODELE { get; set; }
        public string TYPE { get; set; }
        public string VALEUR { get; set; }
        public int ORDRE { get; set; }
        [ForeignKey("MODELE")]
        public virtual MODELES_ATTESTATIONS MODELES_ATTESTATIONS { get; set; }
    }
}