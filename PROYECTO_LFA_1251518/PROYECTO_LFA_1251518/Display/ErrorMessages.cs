using System;

namespace PROYECTO_LFA_1251518
{
    public static class ErrorMessages
    {
        private static string[] messages = new string[20]
        {
      "Se esperaba la palabra ERROR. Fila ",
      "Se esperaba la palabra TOKENS. Fila ",
      "Se esperaba un \"ERROR\". Fila: ",
      "Se esperaba un identificador. Fila: ",
      "Se esperaba \"{\". Fila: ",
      "Se esperaba \"}\". Fila: ",
      "Se esperaba un signo \"=\". Fila: ",
      "Se esperaba un identificador entre apostrofes. Fila: ",
      "Se esperaba un número de TOKEN. Fila: ",
      "El número de TOKEN ya existe. Fila: ",
      "Se esperaba una expresión regular. Fila: ",
      "Se esperaba \")\". Fila: ",
      "No hay paréntesis de apertura. Fila: ",
      "Se esperaba un caracter. Fila: ",
      "Se esperaba un apóstrofe. Fila: ",
      "El SET no esta definido. Fila: ",
      "Se esperaba la palabra TOKEN. Fila: ",
      "Se esperaba un SET. Fila: ",
      "El SET ya existe. Fila: ",
      "Se esperaba \"}\". Fila: "
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
