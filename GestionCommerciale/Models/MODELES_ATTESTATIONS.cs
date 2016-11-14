using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionCommerciale.Models
{
    public class MODELES_ATTESTATIONS
    {
        public MODELES_ATTESTATIONS()
        {
            this.ATTESTATIONS = new HashSet<ATTESTATIONS>();
            this.CHAMPS_MODELES = new HashSet<CHAMPS_MODELES>();
        }

        public int ID { get; set; }
        public string TITRE { get; set; }

        public virtual ICollection<ATTESTATIONS> ATTESTATIONS { get; set; }
        public virtual ICollection<CHAMPS_MODELES> CHAMPS_MODELES { get; set; }
    }
}