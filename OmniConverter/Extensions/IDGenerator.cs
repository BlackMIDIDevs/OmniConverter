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
        public static string GetID() => rnd.Next(int.MinValue, int.MaxValue).ToString("X8");
    }
}
