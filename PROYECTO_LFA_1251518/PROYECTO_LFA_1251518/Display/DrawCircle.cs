using System.Drawing;

namespace PROYECTO_LFA_1251518
{
    public class DrawCircle
    {
        private Color circleColor;
        private string name;
        private Point position;
        private int size;

        public DrawCircle(string value, int x, int y)
        {
            this.name = value;
            this.circleColor = Color.DarkRed;
            this.position = new Point(x, y);
            this.size = 35;
        }

        public void Draw(Graphics Canvas)
        {
            Brush brush1 = (Brush)new SolidBrush(this.circleColor);
            Brush brush2 = (Brush)new SolidBrush(Color.White);
            Font font = new Font("Arial", 12f);
            RectangleF rect = new RectangleF((float)this.position.X, (float)this.position.Y, (float)(this.size * this.name.Length), 30f);
            Canvas.FillEllipse(brush1, rect);
            Canvas.DrawString(this.name, font, brush2, (float)(this.position.X + 8), (float)(this.position.Y + 8));
        }
    }
}
