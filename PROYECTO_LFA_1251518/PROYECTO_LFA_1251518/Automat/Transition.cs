using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO_LFA_1251518.Automat
{
    public class Transition
    {
        private string simbol;
        private int status;

        public Transition(string receivedStatus, int receivedSimbol)
        {
            this.Simbol = receivedStatus;
            this.Status = receivedSimbol;
        }

        public string Simbol
        {
            set
            {
                this.simbol = value;
            }
            get
            {
                return this.simbol;
            }
        }

        public int Status
        {
            set
            {
                this.status = value;
            }
            get
            {
                return this.status;
            }
        }
    }
}
