using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libfdt
{
    public class Node
    {
        public List<Node> ChildNodes { get; set; }
        public List<KeyValuePair<string, object>> Properties { get; set; }
        public string Name { get; set; }

        private static int indentLevel = 0;

        public Node()
        {
            Properties = new List<KeyValuePair<string, object>>();
            ChildNodes = new List<Node>();
        }

        public static Node ParseNode(byte[] fdt, UInt32 stringTableOffset, ref UInt32 startPosition)
        {
            //* token OF_DT_BEGIN_NODE (that is 0x00000001)
            if (fdt.PopUInt32(ref startPosition) != FDT.OF_DT_BEGIN_NODE)
            {
                Console.WriteLine("Invalid start position for node");
                return null;
            }

            //* for version 1 to 3, this is the node full path as a zero
            //  terminated string, starting with "/".For version 16 and later,
            //  this is the node unit name only(or an empty string for the
            //  root node)
            string parsedName = fdt.PopString(ref startPosition);

            Node parsedNode = new Node
            {
                Name = parsedName.Length > 0 ? parsedName : "device-tree"
            };

            //* [align gap to next 4 bytes boundary]
            if (startPosition % 4 == 0)
            {
                //startPosition += 4;
            }
            else
            {
                while (startPosition % 4 != 0)
                {
                    startPosition++;
                }
            }

            // * for each property:
            //    * token OF_DT_PROP (that is 0x00000003)
            while (fdt.ReadUInt32(startPosition) == FDT.OF_DT_PROP)
            {
                parsedNode.Properties.Add(ParseProperty(fdt, stringTableOffset, ref startPosition));
            }

            while (fdt.ReadUInt32(startPosition) == FDT.OF_DT_BEGIN_NODE)
            {
                parsedNode.ChildNodes.Add(ParseNode(fdt, stringTableOffset, ref startPosition));
            }

            startPosition += 4;

            return parsedNode;
        }

        public static Node ParseNode(byte[] fdt, UInt32 stringTableOffset, UInt32 startPosition)
        {
            return ParseNode(fdt, stringTableOffset, ref startPosition);
        }

        public static KeyValuePair<string, object> ParseProperty(byte[] fdt, UInt32 stringTableOffset, ref UInt32 startPosition)
        {
            if (fdt.PopUInt32(ref startPosition) != FDT.OF_DT_PROP)
            {
                Console.WriteLine("Invalid start position for property");
                throw new ArgumentException(
                    $"{startPosition} does not match the property start token, {FDT.OF_DT_PROP}");
            }

            var valueSize = fdt.PopUInt32(ref startPosition);
            var stringOffset = fdt.PopUInt32(ref startPosition);

            string name = fdt.ReadString(stringTableOffset + stringOffset);
            object value = null;

            if (valueSize > 0)
            {
                byte[] rawValue = new byte[valueSize];
                Array.Copy(fdt, startPosition, rawValue, 0, valueSize);

                value = GetPropertyValue(valueSize, rawValue);

                startPosition += valueSize;
            }

            //* [align gap to next 4 bytes boundary]
            while (startPosition % 4 != 0)
            {
                startPosition++;
            }

            return new KeyValuePair<string, object>(name, value);
        }

        private static object GetPropertyValue(UInt32 valueSize, byte[] rawValue)
        {
            if (rawValue.Last() != 0 || rawValue.Length <= 1)
            {
                return rawValue;
            }

            for (var i = 1; i < valueSize; i++)
            {
                if (rawValue[i] == 0 && rawValue[i - 1] == 0) //Two null terminators in a row probably means binary
                {
                    return rawValue;
                }
            }

            List<string> values = new List<string>(1);

            UInt32 valuePosition = 0;

            while (valuePosition != valueSize)
            {
                values.Add(rawValue.PopString(ref valuePosition));
            }

            if (values.Count == 1)
            {
                return values[0];
            }

            return values;

        }

        public string PropertyToString(KeyValuePair<string, object> property)
        {
            StringBuilder sb = new StringBuilder();

            for (var i = 1; i < indentLevel; i++)
            {
                sb.Append("  |");
            }

            sb.AppendFormat("  |- {0}", property.Key);

            if (property.Value != null)
            {
                sb.Append(" = ");

                switch (property.Value)
                {
                    case byte[] _:
                        sb.Append("<");

                        string[] formattedNumbers = new string[((byte[])property.Value).Length / 4];
                        for (var i = 0; i < ((byte[])property.Value).Length; i += 4)
                        {
                            formattedNumbers[i / 4] = $"0x{((byte[]) property.Value).ReadUInt32((UInt32) i):X8}";
                        }

                        sb.Append(string.Join(" ", formattedNumbers));

                        sb.Append(">");
                        break;
                    case string _:
                        sb.AppendFormat("\"{0}\"", (string)property.Value);
                        break;
                    case List<string> _:
                        sb.AppendFormat("\"{0}\"", string.Join("\", \"", (List<string>)property.Value));
                        break;
                }
            }

            sb.AppendFormat("\r\n");

            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (indentLevel == 0)
            {
                sb.Append("/");
            }

            for (var i = 0; i < indentLevel; i++)
            {
                sb.Append("  |");
            }

            sb.AppendFormat("  o {0}\r\n", Name);

            indentLevel++;

            foreach (KeyValuePair<string, object> nodeProperty in Properties)
            {
                sb.Append(PropertyToString(nodeProperty));
            }

            foreach (Node childNode in ChildNodes)
            {
                sb.Append(childNode);
            }

            indentLevel--;

            return sb.ToString();
        }
    }
}