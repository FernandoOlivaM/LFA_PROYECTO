using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO_LFA_1251518
{
    public class AutomatStatus
    {
        public int current;
        public bool acceptation;
        public int token;
        public List<Transition> transitionsList;

        public AutomatStatus()
        {
            this.transitionsList = new List<Transition>();
        }
    }
}
