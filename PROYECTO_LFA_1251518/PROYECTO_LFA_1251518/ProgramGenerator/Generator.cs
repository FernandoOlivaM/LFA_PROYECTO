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
        
            var function = "public string buscarEnSets(string token)\r\n{\r\nforeach (var item in diccionarioSets)\r\n{\r\n";
            function += "var aux = string.Empty;\r\nif (item.Value.Contains(" + Convert.ToChar(34) + "CHR" + Convert.ToChar(34) + "))\r\n{\r\n";
            function += "aux = item.Value.Replace(" + Convert.ToChar(34) + ".." + Convert.ToChar(34) + ", " + Convert.ToChar(34) + " - " + Convert.ToChar(34) + ").Replace(" + Convert.ToChar(34) + "CHR" + Convert.ToChar(34) + ", " + Convert.ToChar(34) + Convert.ToChar(34) + ").Replace(" + Convert.ToChar(34) + "(" + Convert.ToChar(34) + ", " + Convert.ToChar(34) + Convert.ToChar(34) + ").Replace(" + Convert.ToChar(34) + ")" + Convert.ToChar(34) + ", " + Convert.ToChar(34) + Convert.ToChar(34) + ");";
            function += "\r\n}\r\nelse\r\n{\r\naux = item.Value.Replace(" + Convert.ToChar(34) + "'" + Convert.ToChar(34) + ", " + Convert.ToChar(34) + Convert.ToChar(34) + ").Replace(" + Convert.ToChar(34) + ".." + Convert.ToChar(34) + "," + Convert.ToChar(34) + "-" + Convert.ToChar(34) + ");";
            function += "\r\n}\r\nstring[] aux2 = aux.Split('+');\r\nfor (int i = 0; i < aux2.Count(); i++){\r\n   if (aux2[i].Contains(" + Convert.ToChar(34) + "-" + Convert.ToChar(34) + ")) ";
            function += "\r\n{\r\n var min = aux2[i][0];\r\nvar max = aux2[i][2];\r\nif ((int)Convert.ToChar(token) > min - 1 && (int)Convert.ToChar(token) < max + 1){\r\nvar value = diccionarioSets.FirstOrDefault(x => x.Value == item.Value).Key;\r\nreturn value;\r\n}\r\n";
            function += "else if (aux2[i].Length == 1){\r\nif ((int)Convert.ToChar(token) == Convert.ToChar(aux2[i]))\r\n{\r\nvar value = diccionarioSets.FirstOrDefault(x => x.Value == item.Value).Key;\r\nreturn value;\r\n}\r\n}\r\n}\r\n}\r\n}\r\nreturn " + Convert.ToChar(34) + Convert.ToChar(34) + ";\r\n}\r\n";
            function += "\r\n\r\npublic string buscar(string texto)\r\n{\r\nbool rechazado = true;\r\nDictionary<string, int> diccionarioFinal = new Dictionary<string, int>();\r\nvar value = string.Empty;\r\nstring[] tokens = texto.Split(' '); \r\n";
            function += "for (int i = 0; i < tokens.Count(); i++)\r\n{\r\nforeach (var item in diccionarioTokensActions){\r\nvar aux = item.Value;\r\n";
            function += "if (item.Value.Replace(" + Convert.ToChar(34) + "'" + Convert.ToChar(34) + ", " + Convert.ToChar(34) + Convert.ToChar(34) + ").Replace(" + Convert.ToChar(34) + " " + Convert.ToChar(34) + "," + Convert.ToChar(34) + Convert.ToChar(34) + ") == tokens[i])";
            function += "\r\n{\r\nif (!diccionarioFinal.ContainsKey((tokens[i])))\r\ndiccionarioFinal.Add(tokens[i], item.Key);\r\n rechazado = false;\r\nbreak;\r\n}\r\n}\r\nif (!rechazado)\r\n{\r\nforeach (var item in diccionarioTokensActions)\r\n{\r\nif (tokens[i].Length == 1 && item.Value.Contains(buscarEnSets(tokens[i])))\r\n{\r\n";
            function += "if (!diccionarioFinal.ContainsKey((tokens[i])))\r\ndiccionarioFinal.Add(tokens[i], item.Key);\r\nbreak;\r\n}\r\n}\r\n}\r\n}";
            function += "\r\nforeach (var item in diccionarioFinal)\r\n{\r\nvalue += item.Key + " + Convert.ToChar(34) + "=" + Convert.ToChar(34) + " + item.Value + " + Convert.ToChar(34) + "enter" + Convert.ToChar(34) + ";\r\n}\r\nreturn value;\r\n}\r\n";
            return "using System; \r\nusing System.Collections.Generic;\r\nusing System.ComponentModel;\r\nusing System.Data;\r\nusing System.Drawing;\r\nusing System.Linq;\r\nusing System.Text;\r\nusing System.Windows.Forms;\r\n\r\nnamespace Generico\r\n{\r\npublic class evaluar\r\n{\r\nstatic List<string> tkn = new List<string>();\r\n" + dictionary + function + "public bool verificar(string texto)\r\n{\r\nbool enc;\r\nint estado = 0;\r\n bool aceptado=false; string tokens = texto.Trim();\r\nfor (int i = 0; i < tokens.Length; i++) {\r\nstring token = tokens[i].ToString();\r\nenc = false;\r\nswitch(estado) { \r\n" + content + "\r\n} \r\n}return aceptado;}} \r\n\r\n}";

        }

    }
}
