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
        private Vector4f m_pos;

        //private float m_x;
        //private float m_y;

        public float GetX() { return m_pos[0]; }
        public float GetY() { return m_pos[1]; }

        //public void SetX(float x) { m_pos[0] = x; }
        //public void SetY(float y) { m_pos[1] = y; }
        

        private Color color;
        public Color GetColor() { return color; }

        public Vertex(float x, float y, float z)
        {
            m_pos = new Vector4f(x, y, z, 1);
            Random rand = new Random();
            color = Color.FromArgb(255, 255, 0, 0);
            //m_x = x;
            //m_y = y;
        }

        public Vertex(Vector4f pos)
        {
            m_pos = pos;
            color = Color.FromArgb(255, 255, 0, 0);
        }

        public Vertex Transform(Matrix4f transform)
        {
            return new Vertex(transform * m_pos);
        }

        public Vertex PerspectiveDivide()
        {
            return new Vertex
                (
                    new Vector4f
                    (
                        m_pos[0] / m_pos[3],
                        m_pos[1] / m_pos[3],
                        m_pos[2] / m_pos[3],
                        m_pos[3]
                    )
                );
        }

        public float TriangleAreaTimesTwo(Vertex b,Vertex c)
        {
            float x1 = b.GetX() - m_pos[0];
            float y1 = b.GetY() - m_pos[1];

            float x2 = c.GetX() - m_pos[0];
            float y2 = c.GetY() - m_pos[1];

            return (x1 * x2 - x2 * y1);
        }
    }
}
