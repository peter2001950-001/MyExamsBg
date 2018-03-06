using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.TestProcessing
{
    public class RandomString
    {
        private static readonly object padlock = new object();

        private static RandomString instance = null;
        private static Random random;

        public static RandomString Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new RandomString();
                        random = new Random();
                    }
                    return instance;
                }
            }
        }
        public string GetString(int length)
        {
            lock (padlock)
            {
                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

                return new string(Enumerable.Repeat(chars, length)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            }

        }
    }
}
