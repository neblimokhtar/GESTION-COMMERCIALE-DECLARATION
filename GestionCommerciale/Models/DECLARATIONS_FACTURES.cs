using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestionCommerciale.Models
{
    public class DECLARATIONS_FACTURES
    {
        public DECLARATIONS_FACTURES()
        {
            this.LIGNES_DECLARATIONS_FACTURES = new HashSet<LIGNES_DECLARATIONS_FACTURES>();
        }

        public int ID { get; set; }
        public int ANNEE { get; set; }
        public int TRIMESTRE { get; set; }
        public Nullable<int> SOCIETE { get; set; }
        public byte[] DATA { get; set; }
        public string CODE { get; set; }
        public DateTime DATE { get; set; }
        public Boolean VALIDE { get; set; }

        [ForeignKey("SOCIETE")]
        public virtual DECLARATIONS DECLARATIONS { get; set; }
        public virtual ICollection<LIGNES_DECLARATIONS_FACTURES> LIGNES_DECLARATIONS_FACTURES { get; set; }
    }
}