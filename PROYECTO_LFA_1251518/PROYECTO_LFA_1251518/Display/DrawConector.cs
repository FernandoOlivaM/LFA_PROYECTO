using System.Drawing;

namespace PROYECTO_LFA_1251518
{
    public class DrawConector
    {
        private static Point beginPosition = new Point();
        private static Point endPosition = new Point();
        private Color conectorColor;

        public DrawConector(int beginX, int beginY, int endX, int endY)
        {
            DrawConector.beginPosition.X = beginX;
            DrawConector.beginPosition.Y = beginY;
            DrawConector.endPosition.X = endX;
            DrawConector.endPosition.Y = endY;
            this.conectorColor = Color.Black;
        }

        public void Draw(Graphics Canvas)
        {
            Pen pen = new Pen(this.conectorColor);
            Canvas.DrawLine(pen, DrawConector.beginPosition.X, DrawConector.beginPosition.Y, DrawConector.endPosition.X, DrawConector.endPosition.Y);
        }
    }
}
