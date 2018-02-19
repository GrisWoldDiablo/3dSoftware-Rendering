using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace _3dSoftware_Rendering
{
    public class Vertex
    {
        private float m_x;
        private float m_y;

        public float GetX() { return m_x; }
        public float GetY() { return m_y; }

        public void SetX(float x) { m_x = x; }
        public void SetY(float y) { m_y = y; }
        

        private Color color;
        public Color GetColor() { return color; }

        public Vertex(float x, float y)
        {
            Random rand = new Random();
            color = Color.FromArgb(255, 255, 0, 0);
            m_x = x;
            m_y = y;
        }

        public float TriangleAreaTimesTwo(Vertex b,Vertex c)
        {
            float x1 = b.GetX() - m_x;
            float y1 = b.GetY() - m_y;

            float x2 = c.GetX() - m_x;
            float y2 = c.GetY() - m_y;

            return (x1 * x2 - x2 * y1);
        }
    }
}
