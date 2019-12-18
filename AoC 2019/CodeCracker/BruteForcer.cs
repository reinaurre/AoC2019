using System;
using System.Collections.Generic;

namespace CodeCracker
{
    public class BruteForcer
    {
        public int GetNumberOfValidCodes(int start, int end, bool useStrictRules = false)
        {
            return this.BruteForce(start, end, useStrictRules).Length;
        }

        public int[] BruteForce(int start, int end, bool useStrictRules = false)
        {
            List<int> result = new List<int>();

            for(int i = start; i <= end; i++)
            {
                if (this.IsValid(i, useStrictRules))
                {
                    result.Add(i);
                }
            }

            return result.ToArray();
        }

        private bool IsValid(int code, bool useStrictRules)
        {
            bool result = true;
            int[] codeArr = code.ToIntArray();
            bool hasDouble = false;
            int doubleValue = -1;
            int sameCounter = 1;

            for (int i = 1; i < codeArr.Length && result == true; i++)
            {
                if(codeArr[i] < codeArr[i-1])
                {
                    result = false;
                }
                else if (codeArr[i] == codeArr[i - 1])
                {
                    sameCounter++;

                    if (!useStrictRules)
                    {
                        hasDouble = true;
                    }
                    else if(sameCounter == 2 && !hasDouble)
                    {
                        hasDouble = true;
                        doubleValue = codeArr[i];
                    }
                    else if(sameCounter > 2 && doubleValue == codeArr[i])
                    {
                        hasDouble = false;
                        doubleValue = -1;
                    }
                }
                else if (codeArr[i] > codeArr[i - 1] && sameCounter > 1 && useStrictRules)
                {
                    if (sameCounter == 2)
                    {
                        hasDouble = true;
                    }
                }

                if (codeArr[i] > codeArr[i - 1])
                {
                    sameCounter = 1;
                }
            }

            return hasDouble ? result : false;
        }
    }

    public static class BruteForcerEx
    {
        public static int[] ToIntArray(this int n)
        {
            var result = new int[numDigits(n)];
            for (int i = result.Length - 1; i >= 0; i--)
            {
                result[i] = n % 10;
                n /= 10;
            }
            return result;
        }

        private static int numDigits(int n)
        {
            if (n < 0)
            {
                n = (n == Int32.MinValue) ? Int32.MaxValue : -n;
            }
            if (n < 10) return 1;
            if (n < 100) return 2;
            if (n < 1000) return 3;
            if (n < 10000) return 4;
            if (n < 100000) return 5;
            if (n < 1000000) return 6;
            if (n < 10000000) return 7;
            if (n < 100000000) return 8;
            if (n < 1000000000) return 9;
            return 10;
        }
    }
}
