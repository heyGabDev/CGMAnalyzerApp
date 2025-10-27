using CGMAnalyzerCore.Context; // Ajouté pour VDCTypeEnum
using CGMAnalyzerCore.Parser;
using System.Diagnostics;

namespace CGMAnalyzerCore.Commands.GraphicCommands.Control
{
    public class VDCTypeCommand : BaseCgmCommand
    {
        public enum VDCTypeEnum
        {
            INTEGER = 0,
            REAL = 1
        }

        public static VDCTypeEnum CurrentVDCType { get; private set; }
        private const VDCTypeEnum DEFAULT_VDC_TYPE = VDCTypeEnum.REAL;

        public VDCTypeEnum Type { get; } = VDCTypeEnum.REAL;// Valeur par défaut en cas d'erreur

        public VDCTypeCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[VDCTypeCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                int vdcTypeCode = argReader.NextArg();

                Type = vdcTypeCode switch
                {
                    0 => VDCTypeEnum.INTEGER,
                    1 => VDCTypeEnum.REAL,
                    _ => throw new InvalidDataException($"Unknown VDC type: {vdcTypeCode}")
                };

                Debug.WriteLine($"[VDCTypeCommand] File says Type={Type}");

                // Mettre à jour le contexte CGM  & le contexte global

                if (Type == VDCTypeEnum.INTEGER)
                {
                    Debug.WriteLine("[VDCTypeCommand] ATTENTION: Le fichier revendique INTEGER mais les données sont REAL. Passage forcé en REAL.");
                    Type = VDCTypeEnum.REAL;
                }

                CgmContext.SetVdcType(Type);
                CurrentVDCType = Type;
                CgmContext.SetVdcType(Type);

                Debug.WriteLine($"[VDCTypeCommand] Type={Type}");
                ValidateArgumentsRead("VDCTypeCommand");
            }
            catch (Exception)
            {
                Debug.WriteLine($"[VDCTypeCommand ERROR] Erreur lors de la lecture de VDCTypeCommand");
                Type = DEFAULT_VDC_TYPE; 
                CurrentVDCType = Type;
                CgmContext.SetVdcType(Type);
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"VDC_TYPE : {Type}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }      
    }
}
