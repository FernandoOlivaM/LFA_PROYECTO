using System;

namespace PROYECTO_LFA_1251518
{
    public static class ErrorMessages
    {
        private static string[] messages = new string[20]
        {
      "Se esperaba la palabra ERROR. Fila ",
      "Se esperaba la palabra TOKENS. Fila ",
      "Se esperaba la declaración de un ERROR. Fila: ",
      "Se esperaba un identificador. Fila: ",
      "Se esperaba una llave de apertura \"{\". Fila: ",
      "Se esperaba una llave de cerradura \"}\". Fila: ",
      "Se esperaba un signo \"=\". Fila: ",
      "Se esperaba un identificador entre apostrofes. Fila: ",
      "Se esperaba un número entero. Fila: ",
      "Número de TOKEN repetido. Fila: ",
      "Se esperaba expresión regular. Fila: ",
      "Se esperaba paréntesis cerrado \")\". Fila: ",
      "No hay paréntesis correspondiente abierto  \"(\". Fila: ",
      "Se esperaba un caracter. Fila: ",
      "Se esperaba un apóstrofe. Fila: ",
      "Uso de SET no definido. Fila: ",
      "Se esperaba la palabra TOKEN. Fila: ",
      "Se esperaba la declaración de un SET. Fila: ",
      "Identificador de SET repetido. Fila: ",
      "Se esperaba llave cerrada \"}\". Fila: "
        };
        static string row, column;

        public static string Message(string error)
        {
            string[] errorArray = error.Split('|');
            if (errorArray.Length != 3)
                return error;
            ErrorMessages.row = errorArray[1];
            ErrorMessages.column = errorArray[2];
            return ErrorMessages.messages[Convert.ToInt32(errorArray[0])] + ErrorMessages.row + " ,columna: " + ErrorMessages.column;
        }
    }
}
