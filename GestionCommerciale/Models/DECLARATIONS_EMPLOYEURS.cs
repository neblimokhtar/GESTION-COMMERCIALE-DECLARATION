using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestionCommerciale.Models
{
    public partial class DECLARATIONS_EMPLOYEURS
    {
        public DECLARATIONS_EMPLOYEURS()
        {
            this.ANNEXE_1 = new HashSet<ANNEXE_1>();
            this.ANNEXE_2 = new HashSet<ANNEXE_2>();
            this.ANNEXE_3 = new HashSet<ANNEXE_3>();
            this.ANNEXE_4 = new HashSet<ANNEXE_4>();
            this.ANNEXE_5 = new HashSet<ANNEXE_5>();
            this.ANNEXE_6 = new HashSet<ANNEXE_6>();
            this.ANNEXE_7 = new HashSet<ANNEXE_7>();
        }

        public int ID { get; set; }
        public int ANNEE { get; set; }
        public Nullable<int> SOCIETE { get; set; }
        public byte[] DATA { get; set; }
        public string CODE { get; set; }
        public System.DateTime DATE { get; set; }
        public Nullable<bool> VALIDE { get; set; }
        public int CODE_ACTE { get; set; }
        public byte[] ANNEXE_1_DATA { get; set; }
        public byte[] ANNEXE_2_DATA { get; set; }
        public byte[] ANNEXE_3_DATA { get; set; }
        public byte[] ANNEXE_4_DATA { get; set; }
        public byte[] ANNEXE_5_DATA { get; set; }
        public byte[] ANNEXE_6_DATA { get; set; }
        public byte[] ANNEXE_7_DATA { get; set; }


        public virtual ICollection<ANNEXE_1> ANNEXE_1 { get; set; }
        public virtual ICollection<ANNEXE_2> ANNEXE_2 { get; set; }
        public virtual ICollection<ANNEXE_3> ANNEXE_3 { get; set; }
        public virtual ICollection<ANNEXE_4> ANNEXE_4 { get; set; }
        public virtual ICollection<ANNEXE_5> ANNEXE_5 { get; set; }
        public virtual ICollection<ANNEXE_6> ANNEXE_6 { get; set; }
        public virtual ICollection<ANNEXE_7> ANNEXE_7 { get; set; }
        [ForeignKey("SOCIETE")]
        public virtual DECLARATIONS DECLARATIONS { get; set; }
    }
}