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
        public int ElementClass { get; }
        public int ElementId { get; }
        public int Length { get; }

        protected BaseCgmCommand(int elementClass, int elementId, int length)
        {
            ElementClass = elementClass;
            ElementId = elementId;
            Length = length;
        }

        public abstract void Draw(Graphics g, Pen pen);


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

        /// <summary>
        /// 
        /// </summary>
       public virtual void LogCommandInfo()
       {
            Debug.WriteLine($"[CGM] Command {GetType().Name} - EC:{ElementClass} EID:{ElementId}");
       }
        


        /// <summary>
        /// Représentation textuelle pour debug/logs.
        /// </summary>
        public override string ToString()
        {
            return $"{GetType().Name} [EC={ElementClass}, EID={ElementId}, Length={Length}]";
        }
    }
}

