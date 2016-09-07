using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestionCommerciale.Models
{
    public partial class GENERATIONS
    {
        public GENERATIONS()
        {
            this.LIGNES_GENERATIONS = new HashSet<LIGNES_GENERATIONS>();
        }

        public int ID { get; set; }
        public string CODE { get; set; }
        public int SOCIETE { get; set; }
        public int ANNEE { get; set; }
        public int TRIMESTRE { get; set; }
        public DateTime DATE { get; set; }
        public byte[] DATA { get; set; }
        [ForeignKey("SOCIETE")]
        public virtual DECLARATIONS DECLARATIONS { get; set; }
        public virtual ICollection<LIGNES_GENERATIONS> LIGNES_GENERATIONS { get; set; }
    }
}