using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Honeypot.Helpers
{
    public static class RandomPasswordGenerator
    {
        private static Random _random = new Random();

        private static char[] _letterArray = { 'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
                                               'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z' };
        private static char[] _numberArray = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private static char[] _symbolArray = { '!', '@', '#', '$', '%', '&', '*' };

        public static string GeneratePassword(bool letter, bool number, bool symbol, int length)
        {
            List<char> randomList = new List<char>();
            try
            {
                if (letter)
                {
                    randomList.AddRange(_letterArray);
                }
                if (number)
                {
                    randomList.AddRange(_numberArray);
                }
                if (symbol)
                {
                    randomList.AddRange(_symbolArray);
                }

                int arrayCount = randomList.Count;

                if (arrayCount <= 0)
                {
                    return "";
                }

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < length; i++)
                {
                    int index = _random.Next(arrayCount);
                    sb.Append(randomList[index]);
                }
                return sb.ToString();
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
            finally
            {
                randomList.Clear();
            }
            return "";
        }
    }
}
