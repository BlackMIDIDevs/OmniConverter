using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Midi;

namespace OmniConverter
{
    class DataCheck
    {
        public static string BytesToHumanReadableSize(UInt64 length)
        {
            string size;
            try
            {
                if (length >= 1099511627776)
                {
                    if (length >= 1099511627776 && length < 10995116277760)
                        size = ((((length / 1024f) / 1024f) / 1024f) / 1024f).ToString("0.00 TB");
                    else
                        size = ((((length / 1024f) / 1024f) / 1024f) / 1024f).ToString("0.0 TB");
                }
                else if (length >= 1073741824)
                {
                    if (length >= 1073741824 && length < 10737418240)
                        size = (((length / 1024f) / 1024f) / 1024f).ToString("0.00 GB");
                    else
                        size = (((length / 1024f) / 1024f) / 1024f).ToString("0.0 GB");
                }
                else if (length >= 1048576)
                {
                    if (length >= 1048576 && length < 10485760)
                        size = ((length / 1024f) / 1024f).ToString("0.00 MB");
                    else
                        size = ((length / 1024f) / 1024f).ToString("0.0 MB");
                }
                else if (length >= 1024)
                {
                    if (length >= 1024 && length < 10240)
                        size = (length / 1024f).ToString("0.00 KB");
                    else
                        size = (length / 1024f).ToString("0.0 KB");
                }
                else
                {
                    if (length >= 1 && length < 1024)
                        size = (length).ToString("0.00 B");
                    else
                        size = (length / 1024f).ToString("0.0 B");
                }
            }
            catch { size = "-"; }

            if (length > 0) return size;
            else return "Black hole";
        }
    }

    public static class InputExtensions
    {
        public static int LimitToRange(
            this int value, int inclusiveMinimum, int inclusiveMaximum)
        {
            if (value < inclusiveMinimum) { return inclusiveMinimum; }
            if (value > inclusiveMaximum) { return inclusiveMaximum; }
            return value;
        }
    }
}
