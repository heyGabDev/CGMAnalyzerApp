using CGMAnalyzerCore.Context;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands
{
    public enum SDRType
    {
        SDR = 1,
        CI = 2,
        CD = 3,
        N = 4,
        E = 5,
        I = 6,
        Reserved = 7,
        IF8 = 8,
        IF16 = 9,
        IF32 = 10,
        IX = 11,
        R = 12,
        S = 13,
        SF = 14,
        VC = 15,
        VDC = 16,
        CCO = 17,
        UI8 = 18,
        UI32 = 19,
        BS = 20,
        CL = 21,
        UI16 = 22
    }

    public class ApplicationStructureCommand : BaseCgmCommand
    {
        //public string AttributeType { get; private set; }
        //public object AttributeValue { get; private set; }
        //public SDRType DataType { get; private set; }
        //public int DataCount { get; private set; }
        //public List<object> DataValues { get; private set; }

        public string Id { get; private set; }
        public string Type { get; private set; }

        public ApplicationStructureCommand(int ec, int eid, int l,CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[ApplicationStructureCommand] EC={ec} EID={eid} ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(command);

                if (Args != null && Args.Length > 0)
                {
                   Id = argReader.MakeString();
                   Type = argReader.MakeString();
                   Debug.WriteLine($"[ApplicationStructureCommand] Id={Id} Type={Type}");

                    // LAYERS : Incrémenter le layer ID si c'est un LAYER
                    if (Type.Equals("LAYER", StringComparison.OrdinalIgnoreCase))
                    {
                        CgmContext.CurrentLayerId++;
                        Debug.WriteLine($"[ApplicationStructure] - LAYER détecté - LayerId={CgmContext.CurrentLayerId} Name='{Id}'");
                    }

                    //// Créer un aperçu des données brutes restantes
                    //int remaining = command.RemainingArgs();
                    //if (remaining > 0)
                    //{
                    //    int previewLength = Math.Min(remaining, 10);
                    //    var preview = string.Join(" ", Args.Skip(command.CurrentArg).Take(previewLength).Select(a => $"{a:X2}"));
                    //    RawDataPreview = remaining > 10 ? $"{preview}... ({remaining} bytes)" : preview;
                    //}
                    //else
                    //{
                    //    RawDataPreview = "(empty)";
                    //}
                }
                else
                {
                    Id = "Unknown";
                    Type = "Unknown";
                }

                // Marquer tous les arguments comme lus (même si on n'a pas tout parsé)
                CurrentArg = Args?.Length ?? 0;

                // ANCIEN CODE POUR PARSER LE SDR
                // Lire le SDR (Structured Data Record)
                //ParseSDR(argReader);
                ValidateArgumentsRead("ApplicationStructureCommand");

            }
            catch (Exception)
            {
                Id = "Error";
                Type = "Error";
                HasReadErrors = true;

                // En cas d'erreur, consommer tous les octets pour éviter désynchronisation
                CurrentArg = Args?.Length ?? 0;

                // Anciens codes en commentaire
                //AttributeType = null;
                //AttributeValue = null;
                //DataType = SDRType.Reserved;
                //DataCount = 0;
                //DataValues = new List<object>();
                //HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            string layerInfo = Type?.Equals("LAYER", StringComparison.OrdinalIgnoreCase) == true
                 ? $" [LAYER: {Id}]"
                 : "";

            return $"APPLICATION_STRUCTURE: Id={Id}, Type={Type}{layerInfo}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }

        //private void ParseSDR(ExtractedArgumentReader reader)
        //{
        //    DataValues = new List<object>();

        //    // Lire le type de données et le nombre d'éléments
        //    int typeAndCount = (int)reader.MakeIndex();

        //    // Les 7 bits de poids fort contiennent le type de données
        //    DataType = (SDRType)((typeAndCount >> 9) & 0x7F);

        //    // Les 9 bits de poids faible contiennent le nombre d'éléments
        //    DataCount = typeAndCount & 0x1FF;

        //    // Si le nombre est 255, le nombre réel suit sur 16 bits
        //    if (DataCount == 255)
        //    {
        //        DataCount = reader.MakeInt();
        //    }

        //    // Lire les données selon le type
        //    for (int i = 0; i < DataCount; i++)
        //    {
        //        switch (DataType)
        //        {
        //            case SDRType.SDR:
        //                // SDR imbriqué - appel récursif
        //                ParseSDR(reader);
        //                break;

        //            case SDRType.CI:
        //                // Indexed colour
        //                DataValues.Add(reader.MakeColorIndex());
        //                break;

        //            case SDRType.CD:
        //                // Direct colour
        //                DataValues.Add(reader.MakeDirectColor());
        //                break;

        //            case SDRType.N:
        //                // Name (string)
        //                DataValues.Add(reader.MakeString());
        //                break;

        //            case SDRType.E:
        //                // Enumerated (16-bit)
        //                DataValues.Add(reader.MakeEnum());
        //                break;

        //            case SDRType.I:
        //                // Integer
        //                DataValues.Add(reader.MakeInt());
        //                break;

        //            case SDRType.IF8:
        //                // Integer 8-bit
        //                DataValues.Add(reader.MakeByte());
        //                break;

        //            case SDRType.IF16:
        //                // Integer 16-bit
        //                DataValues.Add(reader.MakeInt16());
        //                break;

        //            case SDRType.IF32:
        //                // Integer 32-bit
        //                DataValues.Add(reader.MakeInt32());
        //                break;

        //            case SDRType.IX:
        //                // Index
        //                DataValues.Add(reader.MakeIndex());
        //                break;

        //            case SDRType.R:
        //                // Real
        //                DataValues.Add(reader.MakeReal());
        //                break;

        //            case SDRType.S:
        //                // String
        //                DataValues.Add(reader.MakeString());
        //                break;

        //            case SDRType.SF:
        //                // String fixed
        //                DataValues.Add(reader.MakeFixedString());
        //                break;

        //            case SDRType.VC:
        //                // VDC value(s) as appropriate
        //                DataValues.Add(reader.MakeVdc());
        //                break;

        //            case SDRType.VDC:
        //                // VDC (Virtual Device Coordinates)
        //                DataValues.Add(reader.MakeVdc());
        //                break;

        //            case SDRType.CCO:
        //                // Colour component
        //                DataValues.Add(reader.MakeColorValue());
        //                break;

        //            case SDRType.UI8:
        //                // Unsigned 8-bit integer
        //                DataValues.Add((byte)reader.MakeByte());
        //                break;

        //            case SDRType.UI16:
        //                // Unsigned 16-bit integer
        //                DataValues.Add((ushort)reader.MakeInt16());
        //                break;

        //            case SDRType.UI32:
        //                // Unsigned 32-bit integer
        //                DataValues.Add((uint)reader.MakeInt32());
        //                break;

        //            case SDRType.BS:
        //                // Bit stream
        //                DataValues.Add(reader.MakeBitStream());
        //                break;

        //            case SDRType.CL:
        //                // Colour list
        //                DataValues.Add(reader.MakeColorList());
        //                break;

        //            case SDRType.Reserved:
        //            default:
        //                // Type réservé ou inconnu - lire comme octets bruts
        //                DataValues.Add(reader.MakeByte());
        //                break;
        //        }
        //    }

        //    // Si c'est une structure d'application typique, extraire le type d'attribut
        //    if (DataValues.Count > 0 && DataValues[0] is string)
        //    {
        //        AttributeType = DataValues[0] as string;
        //        if (DataValues.Count > 1)
        //        {
        //            AttributeValue = DataValues[1];
        //        }
        //    }
        //}

        //public string GetElement()
        //{
        //    return AttributeType ?? $"SDR_{DataType}";
        //}

        //public override string ToString()
        //{
        //    var sb = new StringBuilder();
        //    sb.AppendLine($"Application Structure: Type={DataType}, Count={DataCount}");

        //    if (!string.IsNullOrEmpty(AttributeType))
        //    {
        //        sb.AppendLine($"  Attribute Type: {AttributeType}");
        //        if (AttributeValue != null)
        //        {
        //            sb.AppendLine($"  Attribute Value: {AttributeValue}");
        //        }
        //    }

        //    if (DataValues.Count > 0)
        //    {
        //        sb.AppendLine($"  Data Values ({DataValues.Count} items):");
        //        for (int i = 0; i < Math.Min(DataValues.Count, 10); i++)
        //        {
        //            sb.AppendLine($"    [{i}]: {DataValues[i]}");
        //        }
        //        if (DataValues.Count > 10)
        //        {
        //            sb.AppendLine($"    ... and {DataValues.Count - 10} more");
        //        }
        //    }

        //    return sb.ToString();
        //}
    }
}
