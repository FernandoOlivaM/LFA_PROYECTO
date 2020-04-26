using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO_LFA_1251518.Automat
{
    public class AutomatStatus
    {
        public int CurrentStatus;
        public bool Acceptation;
        public int Token;
        public List<Transition> TransitionsList;

        public AutomatStatus()
        {
            this.TransitionsList = new List<Transition>();
        }
    }
}

