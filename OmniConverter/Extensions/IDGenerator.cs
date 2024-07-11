using System;

namespace OmniConverter
{
    internal static class IDGenerator
    {
        private static Random rnd = new();
        public static string GetID() => rnd.Next(int.MinValue, int.MaxValue).ToString("X8");
    }
}
