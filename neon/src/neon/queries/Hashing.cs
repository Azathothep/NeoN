namespace neon
{
    public class Hashing
    {
        public static long HashPair(int a, int b)
        {
            var A = (ulong)(a >= 0 ? 2 * (long)a : -2 * (long)a - 1);
            var B = (ulong)(b >= 0 ? 2 * (long)b : -2 * (long)b - 1);
            var C = (long)((A >= B ? A * A + A + B : A + B * B) / 2);
            return a < 0 && b < 0 || a >= 0 && b >= 0 ? C : -C - 1;
        }

        public static int HashPair(short a, short b)
        {
            var A = (uint)(a >= 0 ? 2 * a : -2 * a - 1);
            var B = (uint)(b >= 0 ? 2 * b : -2 * b - 1);
            var C = (int)((A >= B ? A * A + A + B : A + B * B) / 2);
            return a < 0 && b < 0 || a >= 0 && b >= 0 ? C : -C - 1;
        }
    }
}
