using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionCommerciale.Controllers
{
    public class LigneProduit
    {
        public int ID;
        public string LIBELLE;
        public string DESIGNATION;
        public int QUANTITE;
        public decimal PRIX_VENTE_HT;
        public int REMISE;
        public decimal PTHT;
        public int TVA;
        public decimal TTC;
    }
}