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

        /// <summary>Indique si la commande a eu des erreurs de lecture</summary>
        public bool HasReadErrors { get; protected set; } = false;

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
            if(Args == null)
            {
                Debug.WriteLine($"[CGM ERROR] {GetType().Name} : {ElementClass} - Args is null");
                HasReadErrors = true;
                return 0;
            }

            if (CurrentArg >= Args.Length)
            {
                // Au lieu de lancer une exception, retourner 0 et logger => Valeur par défaut sûre
                Debug.WriteLine($"[CGM] Tentative de lecture au-delà des arguments disponibles dans {GetType().Name} : {ElementClass}  ({CurrentArg}/{Args.Length})");
                HasReadErrors = true;
                return 0;
            }
            return Args[CurrentArg++];
        }

        /// <summary>
        /// Version sécurisée de NextArg qui retourne un bool au lieu de throw
        /// </summary>
        public bool TryNextArg(out int value)
        {
            if (Args == null || CurrentArg >= Args.Length)
            {
                value = 0;
                HasReadErrors = true;
                return false;
            }

            value = Args[CurrentArg++];
            return true;
        }

        /// <summary>
        /// Lit plusieurs arguments d'un coup avec validation
        /// </summary>
        public int[] ReadArgs(int count)
        {
            if (Args == null || CurrentArg + count > Args.Length)
            {
                Debug.WriteLine($"[CGM ERROR] {GetType().Name} - Impossible de lire {count} arguments. " +
                               $"Disponible: {RemainingArgs()}");
                HasReadErrors = true;
                return new int[count]; // Retourne un tableau vide
            }

            var result = new int[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = Args[CurrentArg++];
            }
            return result;
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
            return Args != null && CurrentArg < Args?.Length;
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
            HasReadErrors = false;
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
        protected bool ValidateArgumentCount(int required, string context="")
        {
            if (Args == null)
            {
                Debug.WriteLine($"[CGM VALIDATION] {GetType().Name} {context} - Arguments est null");
                HasReadErrors = true;
                return false;
            }

            if (Args.Length < required)
            {
                Debug.WriteLine($"[CGM VALIDATION] {GetType().Name} {context} - Arguments insuffisants: {Args?.Length ?? 0}/{required} requis.");
                HasReadErrors = true;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Valide qu'il reste assez d'arguments pour une lecture future
        /// </summary>
        public bool ValidateRemainingArgs(int required, string context = "")
        {
            int remaining = RemainingArgs();
            if (remaining < required)
            {
                Debug.WriteLine($"[CGM VALIDATION] {GetType().Name} {context} - " +
                               $"Arguments restants insuffisants: {remaining}/{required} requis");
                HasReadErrors = true;
                return false;
            }
            return true;
        }

        ///// <summary>
        ///// Essaie de lire un argument de façon sûre
        ///// </summary>
        //[Obsolete("Utiliser TryNextArg() à la place")]
        //protected bool TryReadArg(out int value)
        //{
        //    return TryNextArg(out value);
        //}

        /// <summary>
        /// Valide que tous les arguments ont été lus
        /// </summary>
        protected void ValidateArgumentsRead(string commandName)
        {
            commandName ??= GetType().Name;

            if (Args == null) return;

            int remaining = Args.Length - CurrentArg;

            // TOLERANCE : 3 octets de padding
            if (remaining > 3)
            {
                Debug.WriteLine($"[CGM] Warning: {commandName} Arguments non lus {CurrentArg}/{Args.Length} ({remaining} octets restants)");
            }
            else if (remaining > 0)
            {
                Debug.WriteLine($"[CGM] Info: {commandName} {remaining} octets de padding ignorés");
            }
            else if (CurrentArg > Args.Length)
            {
                Debug.WriteLine($"[CGM ERROR] {commandName} Lecture au-delà des arguments {CurrentArg}/{Args.Length}");
                HasReadErrors = true;
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
            var errorFlag = HasReadErrors ? " [ERROR]" : "";
            Debug.WriteLine($"[CGM] Command {GetType().Name} - EC:{ElementClass} EID:{ElementId} " +
                     $"ArgsTotal:{Args?.Length ?? 0} CurrentArg:{CurrentArg} " +
                     $"Remaining:{RemainingArgs()} HasMore:{HasMoreArgs()}");

            if (Args != null && Args.Length > 0 && Args.Length <= 20)
            {
                // Pour les petites commandes, afficher les arguments
                var argsStr = string.Join(" ", Args.Select(a => $"{a:X2}"));
                Debug.WriteLine($"Args: {argsStr}");
            }
            else if (Args != null && Args.Length > 20)
            {
                var preview = string.Join(" ", Args.Take(10).Select(a => $"{a:X2}"));
                Debug.WriteLine($"Args preview (10 / {Args.Length}) : {preview} ...");
            }
        }

        /// <summary>
        /// Représentation textuelle pour debug/logs.
        /// </summary>
        public override string ToString()
        {
            var errorFlag = HasReadErrors ? " [ERROR]" : "";
            return $"{GetType().Name} [EC={ElementClass}, EID={ElementId}, Length={Length}] {errorFlag}";
        }
        #endregion
       
    }
}

