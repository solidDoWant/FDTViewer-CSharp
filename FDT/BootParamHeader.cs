using System;
using System.Text;

namespace libfdt
{
    public class BootParamHeader
    {
        public UInt32 magic { get; set; }                  /* magic word OF_DT_HEADER */
        public UInt32 totalsize { get; set; }              /* total size of DT block */
        public UInt32 off_dt_struct { get; set; }          /* offset to structure */
        public UInt32 off_dt_strings { get; set; }         /* offset to strings */
        public UInt32 off_mem_rsvmap { get; set; }         /* offset to memory reserve map
                                        */
        public UInt32 version { get; set; }                /* format version */
        public UInt32 last_comp_version { get; set; }      /* last compatible version */

        /* version 2 fields below */
        public UInt32 boot_cpuid_phys { get; set; }        /* Which physical CPU id we're
                                        booting on */
        /* version 3 fields below */
        public UInt32 size_dt_strings { get; set; }        /* size of the strings block */

        /* version 17 fields below */
        public UInt32 size_dt_struct { get; set; }     /* size of the DT structure block */

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Magic: 0x{0:X8}\r\n", magic);
            sb.AppendFormat("DTB Total ValueSize: 0x{0:X8}\r\n", totalsize);
            sb.AppendFormat("Device Tree Structure Offset: 0x{0:X8}\r\n", off_dt_struct);
            sb.AppendFormat("Device Tree Strings Offset: 0x{0:X8}\r\n", off_dt_strings);
            sb.AppendFormat("Memory Reserve Map Offset: 0x{0:X8}\r\n", off_mem_rsvmap);
            sb.AppendFormat("DTB Version: {0}\r\n", version);
            sb.AppendFormat("Last Compatible Version: {0}\r\n", last_comp_version);
            sb.AppendFormat("Boot CPU ID: {0}\r\n", boot_cpuid_phys);
            sb.AppendFormat("Device Tree Strings ValueSize: 0x{0:X8}\r\n", size_dt_strings);
            sb.AppendFormat("Device Tree Stucture ValueSize: 0x{0:X8}\r\n", size_dt_struct);

            return sb.ToString();
        }
    }
}
