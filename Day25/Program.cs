using System;

namespace Day25
{
    class Program
    {
        static void Main(string[] args)
        {
            Part1(PROBLEM_PUBLIC_KEY_1, PROBLEM_PUBLIC_KEY_2);
            Console.ReadLine();
        }

        private static void Part1(long publickKey1, long publicKey2)
        {
            var gotLoopSize = TryGetLoopSizeForTargetTransformNumber(7, publicKey2, out var key2Loops);
            var encryptionKey = TransformNumber(publickKey1, key2Loops);
            Console.WriteLine(encryptionKey);
        }

        private static long TransformNumber(long number, int loops)
        {
            long val = 1;
            for (int i = 0; i < loops; i++)
            {
                val *= number;
                val %= 20201227;
            }

            return val;
        }

        private static bool TryGetLoopSizeForTargetTransformNumber(long number, long target, out int loops)
        {
            long val = 1;
            int i = 0;
            while (val != target)
            {
                val *= number;
                val %= 20201227;
                i++;
                // Just in case
                if (i >= int.MaxValue)
                {
                    loops = -1;
                    return false;
                }
            }
            loops = i;
            return true;
        }

        const long EXAMPLE_KEY_1 = 5764801;
        const long EXAMPLE_KEY_2 = 17807724;

        const long PROBLEM_PUBLIC_KEY_1 = 11239946;
        const long PROBLEM_PUBLIC_KEY_2 = 10464955;

    }
}
