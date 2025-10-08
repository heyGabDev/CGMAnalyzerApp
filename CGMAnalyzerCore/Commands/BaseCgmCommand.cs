using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands
{
    public abstract class BaseCgmCommand
    {
        // Propriété de base pour toutes les commandes CGM
        public int ElementClass { get; }
        public int ElementId { get; }
        public int Length { get; }
        /// <summary>Tous les arguments bruts (OCTETS, pas des mots 16 bits)</summary>
        protected internal int[] Args;
        /// <summary>Position actuelle dans le tableau Args pour la lecture séquentielle</summary>
        public int CurrentArg { get; set; } = 0;
        /// <summary>Position en bits dans l'octet actuel (pour lecture de bits)</summary>
        public int PosInArg { get; set; } = 0;

        protected BaseCgmCommand(int elementClass, int elementId, int length)
        {
            ElementClass = elementClass;
            ElementId = elementId;
            Length = length;
        }

        #region ===== MÉTHODES ABSTRAITES =====
        public abstract void Draw(Graphics g, Pen pen);
        #endregion

        #region ===== MÉTHODES VIRTUELLES (peuvent être surchargées) =====
        /// <summary>
        /// Lit les arguments binaires spécifiques à la commande.
        /// Cette méthode est appelée après l'identification EC/EID.
        /// </summary>
        /// <param name="reader">Le flux binaire d'entrée</param>
        public virtual void ReadArguments(BinaryReader reader) { }

        /// <summary>
        /// Permet aux commandes de nettoyer ou finaliser leurs arguments après lecture.
        /// </summary>
        public virtual void CleanUpArguments() { }
        #endregion

        #region ===== MÉTHODES DE GESTION DES ARGUMENTS =====
        public int NextArg()
        {
            if (CurrentArg >= Args.Length)
            {
                // Au lieu de lancer une exception, retourner 0 et logger => Valeur par défaut sûre
                Debug.WriteLine($"[CGM] Tentative de lecture au-delà des arguments disponibles dans {GetType().Name} ({CurrentArg}/{Args.Length})");
                return 0;
            }
            return Args[CurrentArg++];
        }

        public void SkipBits()
        {
            //if (PosInArg % 8 != 0) //this.posInArg
            if (PosInArg != 0)
            {
                // we read some bits from the current arg but aren't done, skip the rest
                PosInArg = 0; //this.posInArg = 0;
                CurrentArg++; // this.currentArg++;
            }
        }

        /// <summary>
        /// Vérifie s'il reste des arguments à lire
        /// </summary>
        public bool HasMoreArgs()
        {
            return CurrentArg < Args?.Length;
        }

        /// <summary>
        /// Nombre d'octets restants
        /// </summary>
        public int RemainingArgs()
        {
            return (Args?.Length ?? 0) - CurrentArg;
        }

        /// <summary>
        /// Réinitialise la position de lecture
        /// </summary>
        public void ResetPosition()
        {
            CurrentArg = 0;
            PosInArg = 0;
        }

        /// <summary>
        /// Vérifie si tous les arguments ont été lus
        /// </summary>
        public bool AllArgumentsRead => CurrentArg >= (Args?.Length ?? 0);
        #endregion

        #region ===== MÉTHODES DE VALIDATION =====
        /// <summary>
        /// Valide que la commande a suffisamment d'arguments.
        /// </summary>
        protected bool ValidateArgumentCount(int required)
        {
            if (Args == null || Args.Length < required)
            {
                Debug.WriteLine($"[CGM] {GetType().Name} - Arguments insuffisants: {Args?.Length ?? 0}/{required}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Essaie de lire un argument de façon sûre
        /// </summary>
        protected bool TryReadArg(out int value)
        {
            if (HasMoreArgs())
            {
                value = NextArg();
                return true;
            }

            Debug.WriteLine($"[CGM] {GetType().Name} tried to read past available args.");
            value = 0;
            return false;
        }

        /// <summary>
        /// Valide que tous les arguments ont été lus
        /// </summary>
        protected void ValidateArgumentsRead(string commandName)
        {
            if (CurrentArg != Args.Length)
            {
                Debug.WriteLine($"[CGM] Warning: {commandName} read {CurrentArg}/{Args.Length} arguments");
            }
        }
        #endregion

        #region ===== MÉTHODES DE MÉMOIRE =====
        /// <summary>
        /// Libère la référence aux arguments pour économiser la mémoire.
        /// Appelé automatiquement après le parsing complet si la commande le permet.
        /// </summary>
        protected void FreeArguments()
        {
            Args = null;
            Debug.WriteLine($"[CGM] Arguments libérés pour {GetType().Name}");
        }
        #endregion

        #region ===== MÉTHODES DE DEBUG =====
        /// <summary>
        /// 
        /// </summary>
        public virtual void LogCommandInfo()
       {
            Debug.WriteLine($"[CGM] Command {GetType().Name} - EC:{ElementClass} EID:{ElementId} " +
                     $"ArgsTotal:{Args?.Length ?? 0} CurrentArg:{CurrentArg} " +
                     $"Remaining:{RemainingArgs()} HasMore:{HasMoreArgs()}");

            if (Args != null && Args.Length > 0 && Args.Length <= 20)
            {
                // Pour les petites commandes, afficher les arguments
                var argsStr = string.Join(" ", Args.Select(a => $"{a:X2}"));
                Debug.WriteLine($"Args: {argsStr}");
            }
        }

        /// <summary>
        /// Représentation textuelle pour debug/logs.
        /// </summary>
        public override string ToString()
        {
            return $"{GetType().Name} [EC={ElementClass}, EID={ElementId}, Length={Length}]";
        }
        #endregion
       
    }
}

