using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestionCommerciale.Models
{
    public class DECLARATIONS_FACS
    {
        public DECLARATIONS_FACS()
        {
            this.LIGNES_DECLARATIONS_FACS = new HashSet<LIGNES_DECLARATIONS_FACS>();
        }

        public int ID { get; set; }
        public int ANNEE { get; set; }
        public int TRIMESTRE { get; set; }
        public int SOCIETE { get; set; }
        public byte[] DATA { get; set; }
        public string CODE { get; set; }
        public DateTime DATE { get; set; }
        public Nullable<bool> VALIDE { get; set; }

        [ForeignKey("SOCIETE")]
        public virtual DECLARATIONS DECLARATIONS { get; set; }
        public virtual ICollection<LIGNES_DECLARATIONS_FACS> LIGNES_DECLARATIONS_FACS { get; set; }
    }
}