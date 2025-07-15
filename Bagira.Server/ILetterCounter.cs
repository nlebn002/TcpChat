using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagira.Server
{
    public interface ILetterCounter
    {
        void AddLetters(string text);
        Dictionary<char, int> GetStats();
    }
}
