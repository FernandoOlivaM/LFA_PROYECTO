using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO_LFA_1251518
{
    public class Transition
    {
        private string simbol;
        private int status;

        public Transition(string strSimbol, int intStatus)
        {
            this.Simbol = strSimbol;
            this.Status = intStatus;
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
