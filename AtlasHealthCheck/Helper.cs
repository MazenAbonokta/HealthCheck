using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlasHealthCheck
{
    public class Helper
    {

        public string FormatBytes(ulong bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            double size = bytes;

            while (size >= 1024)
            {
                size /= 1024;
                counter++;
            }

            return $"{size:n1} {suffixes[counter]}";
        }
        public string FormatKiloBytes(ulong bytes)
        {
            string[] suffixes = { "KB", "MB", "GB", "TB" };
            int counter = 0;
            double size = bytes;

            while (size >= 1024)
            {
                size /= 1024;
                counter++;
            }

            return $"{size:n1} {suffixes[counter]}";
        }
    }
}
