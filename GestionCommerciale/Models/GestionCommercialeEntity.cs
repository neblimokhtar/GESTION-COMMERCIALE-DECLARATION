using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace GestionCommerciale.Models
{
    public class GestionCommercialeEntity : DbContext
    {
        public DbSet<AVOIRS_CLIENTS> AVOIRS_CLIENTS { get; set; }
        public DbSet<AVOIRS_FOURNISSEURS> AVOIRS_FOURNISSEURS { get; set; }
        public DbSet<BONS_LIVRAISONS_CLIENTS> BONS_LIVRAISONS_CLIENTS { get; set; }
        public DbSet<BONS_RECEPTIONS_FOURNISSEURS> BONS_RECEPTIONS_FOURNISSEURS { get; set; }
        public DbSet<CLIENTS> CLIENTS { get; set; }
        public DbSet<COMMANDES_CLIENTS> COMMANDES_CLIENTS { get; set; }
        public DbSet<COMMANDES_FOURNISSEURS> COMMANDES_FOURNISSEURS { get; set; }
        public DbSet<DEVIS_CLIENTS> DEVIS_CLIENTS { get; set; }
        public DbSet<FACTURES_CLIENTS> FACTURES_CLIENTS { get; set; }
        public DbSet<FACTURES_FOURNISSEURS> FACTURES_FOURNISSEURS { get; set; }
        public DbSet<FAMILLES_PRODUITS> FAMILLES_PRODUITS { get; set; }
        public DbSet<FOURNISSEURS> FOURNISSEURS { get; set; }
        public DbSet<LIGNES_AVOIRS_CLIENTS> LIGNES_AVOIRS_CLIENTS { get; set; }
        public DbSet<LIGNES_AVOIRS_FOURNISSEURS> LIGNES_AVOIRS_FOURNISSEURS { get; set; }
        public DbSet<LIGNES_BONS_LIVRAISONS_CLIENTS> LIGNES_BONS_LIVRAISONS_CLIENTS { get; set; }
        public DbSet<LIGNES_BONS_RECEPTIONS_FOURNISSEURS> LIGNES_BONS_RECEPTIONS_FOURNISSEURS { get; set; }
        public DbSet<LIGNES_COMMANDES_CLIENTS> LIGNES_COMMANDES_CLIENTS { get; set; }
        public DbSet<LIGNES_COMMANDES_FOURNISSEURS> LIGNES_COMMANDES_FOURNISSEURS { get; set; }
        public DbSet<LIGNES_DEVIS_CLIENTS> LIGNES_DEVIS_CLIENTS { get; set; }
        public DbSet<LIGNES_FACTURES_CLIENTS> LIGNES_FACTURES_CLIENTS { get; set; }
        public DbSet<LIGNES_FACTURES_FOURNISSEURS> LIGNES_FACTURES_FOURNISSEURS { get; set; }
        public DbSet<MOUVEMENETS_PRODUITS> MOUVEMENETS_PRODUITS { get; set; }
        public DbSet<PRODUITS> PRODUITS { get; set; }
        public DbSet<SOCIETES> SOCIETES { get; set; }
        public DbSet<DECLARATIONS> DECLARATIONS { get; set; }
        public DbSet<EMPLOYEES> EMPLOYEES { get; set; }
        public DbSet<SAISIES> SAISIES { get; set; }
        public DbSet<GENERATIONS> GENERATIONS { get; set; }
        public DbSet<LIGNES_GENERATIONS> LIGNES_GENERATIONS { get; set; }
        public DbSet<DECLARATIONS_FACTURES> DECLARATIONS_FACTURES { get; set; }
        public DbSet<LIGNES_DECLARATIONS_FACTURES> LIGNES_DECLARATIONS_FACTURES { get; set; }
        public DbSet<DECLARATIONS_FACS> DECLARATIONS_FACS { get; set; }
        public DbSet<LIGNES_DECLARATIONS_FACS> LIGNES_DECLARATIONS_FACS { get; set; }
        public DbSet<PARAMETRES> PARAMETRES { get; set; }
        public DbSet<DECLARATIONS_EMPLOYEURS> DECLARATIONS_EMPLOYEURS { get; set; }
        public DbSet<ANNEXE_1> ANNEXE_1 { get; set; }
        public DbSet<ANNEXE_2> ANNEXE_2 { get; set; }
        public DbSet<ANNEXE_3> ANNEXE_3 { get; set; }
        public DbSet<ANNEXE_4> ANNEXE_4 { get; set; }
        public DbSet<ANNEXE_5> ANNEXE_5 { get; set; }
        public DbSet<ANNEXE_6> ANNEXE_6 { get; set; }
        public DbSet<ANNEXE_7> ANNEXE_7 { get; set; }
        public DbSet<AVANCES> AVANCES { get; set; }
        public DbSet<PRETS> PRETS { get; set; }
        public DbSet<TRANCHES_PRETS> TRANCHES_PRETS { get; set; }
        public DbSet<MOUVEMENTS_COMPTABLES> MOUVEMENTS_COMPTABLES { get; set; }

    }
}