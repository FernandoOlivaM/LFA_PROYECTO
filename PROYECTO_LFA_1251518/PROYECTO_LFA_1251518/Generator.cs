using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO_LFA_1251518
{
    public class Generator
    {
        public static string generate(string content, string dictionary)
        {
            return "using System; \r\nusing System.Collections.Generic;\r\nusing System.ComponentModel;\r\nusing System.Data;\r\nusing System.Drawing;\r\nusing System.Linq;\r\nusing System.Text;\r\nusing System.Windows.Forms;\r\n\r\nnamespace Generico\r\n{\r\npublic class evaluar\r\n{\r\nstatic List<string> tkn = new List<string>();\r\n" + dictionary+ "public bool verificar(string texto)\r\n{\r\nbool enc;\r\nint estado = 0;\r\n bool aceptado=false; string tokens = texto.Trim();\r\nfor (int i = 0; i < tokens.Length; i++) {\r\nstring token = tokens[i].ToString();\r\nenc = false;\r\nswitch(estado) { \r\n" + content + "\r\n} \r\n}return aceptado;}} \r\n\r\n}";
        }


    }
}
