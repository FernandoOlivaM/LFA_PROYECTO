using System.Drawing;

namespace PROYECTO_LFA_1251518
{
    public class Conector
    {
        private static Point beginPosition = new Point();
        private static Point endPosition = new Point();
        private Color conectorColor;

        public Conector(int beginX, int beginY, int endX, int endY)
        {
            Conector.beginPosition.X = beginX;
            Conector.beginPosition.Y = beginY;
            Conector.endPosition.X = endX;
            Conector.endPosition.Y = endY;
            this.conectorColor = Color.Black;
        }

        public void Draw(Graphics Canvas)
        {
            Pen pen = new Pen(this.conectorColor);
            Canvas.DrawLine(pen, Conector.beginPosition.X, Conector.beginPosition.Y, Conector.endPosition.X, Conector.endPosition.Y);
        }
    }
}
