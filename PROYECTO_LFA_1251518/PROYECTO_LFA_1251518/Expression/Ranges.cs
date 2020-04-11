using System.Collections.Generic;

namespace PROYECTO_LFA_1251518
{
    public class Ranges
    {
        public string simbol;
        public List<int> low;
        public List<int> high;
        public List<int> single;

        public Ranges()
        {
            this.simbol = string.Empty;
            this.low = new List<int>();
            this.high = new List<int>();
            this.single = new List<int>();
        }

        public Ranges(string receivedSimbol)
        {
            this.simbol = receivedSimbol;
            this.low = new List<int>();
            this.high = new List<int>();
            this.single = new List<int>();
        }
    }
}
