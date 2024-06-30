using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniConverter
{
    internal static class IDGenerator
    {
        private static Random rnd = new Random();
        public static string GetID() => rnd.Next(Int32.MinValue, Int32.MaxValue).ToString("X8");
    }
}
