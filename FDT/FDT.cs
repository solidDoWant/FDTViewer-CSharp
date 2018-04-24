using System;

namespace libfdt
{
    public class FDT
    {
    public static UInt32 OF_DT_HEADER => 0xd00dfeed;
    public static UInt32 OF_DT_BEGIN_NODE => 0x1;
    public static UInt32 OF_DT_END_NODE => 0x2;
    public static UInt32 OF_DT_PROP => 0x3;
    public static UInt32 OF_DT_END => 0x9;

        public BootParamHeader Header { get; set; } = new BootParamHeader();

        public Node RootNode { get; set; }

        public static FDT LoadFDT(byte[] fdtBytes)
        {
            UInt32 position = 0;

            FDT parsedFdt = new FDT
            {
                Header =
                {
                    magic = fdtBytes.PopUInt32(ref position),
                    totalsize = fdtBytes.PopUInt32(ref position),
                    off_dt_struct = fdtBytes.PopUInt32(ref position),
                    off_dt_strings = fdtBytes.PopUInt32(ref position),
                    off_mem_rsvmap = fdtBytes.PopUInt32(ref position),

                    version = fdtBytes.PopUInt32(ref position),
                    last_comp_version = fdtBytes.PopUInt32(ref position)
                }
            };

            if (parsedFdt.Header.version >= 2)
            {
                parsedFdt.Header.boot_cpuid_phys = fdtBytes.PopUInt32(ref position);

                if (parsedFdt.Header.version >= 3)
                {
                    parsedFdt.Header.size_dt_strings = fdtBytes.PopUInt32(ref position);

                    if (parsedFdt.Header.version >= 17)
                    {
                        parsedFdt.Header.size_dt_struct = fdtBytes.PopUInt32(ref position);
                    }
                }
            }

            //DTS stuff
            parsedFdt.RootNode = Node.ParseNode(fdtBytes, parsedFdt.Header.off_dt_strings, parsedFdt.Header.off_dt_struct);

            return parsedFdt;
        }

        public override string ToString()
        {
            string toReturn = "";

            toReturn += Header.ToString() + "\r\n";

            toReturn += RootNode.ToString();

            return toReturn;
        }
    }
}