using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagira.Server
{
    public class LetterCounter : ILetterCounter
    {
        private readonly Dictionary<char, int> _counts = new();

        public void AddLetters(string text)
        {
            foreach (var c in text.ToLowerInvariant())
            {
                if (char.IsLetter(c))
                {
                    if (_counts.ContainsKey(c)) _counts[c]++;
                    else _counts[c] = 1;
                }
            }
        }

        public Dictionary<char, int> GetStats() => new(_counts);
    }
}
