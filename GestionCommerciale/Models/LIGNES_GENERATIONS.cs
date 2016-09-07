using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestionCommerciale.Models
{
    public class LIGNES_GENERATIONS
    {
        public int ID { get; set; }
        public Nullable<int> GENERATION { get; set; }
        public Nullable<int> EMPLOYEE { get; set; }
        public double SALAIRE_MOIS_1 { get; set; }
        public double SALAIRE_MOIS_2 { get; set; }
        public double SALAIRE_MOIS_3 { get; set; }
        [ForeignKey("EMPLOYEE")]
        public virtual EMPLOYEES EMPLOYEES { get; set; }
        [ForeignKey("GENERATION")]
        public virtual GENERATIONS GENERATIONS { get; set; }
    }
}