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
        private Vector4f m_texCoords;

        public float GetX() { return m_pos[0]; }
        public float GetY() { return m_pos[1]; }

        public Vector4f GetPosition() { return m_pos; }
        public Vector4f GetTexCoords() { return m_texCoords; }


        public Vertex(Vector4f pos, Vector4f texCoords)
        {
            m_pos = pos;
            m_texCoords = texCoords;
        }

        public Vertex Transform(Matrix4f transform)
        {
            return new Vertex(transform * m_pos, m_texCoords);
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
                    ),
                    m_texCoords
                );
        }

        public float TriangleAreaTimesTwo(Vertex b,Vertex c)
        {
            float x1 = b.GetX() - m_pos[0];
            float y1 = b.GetY() - m_pos[1];

            float x2 = c.GetX() - m_pos[0];
            float y2 = c.GetY() - m_pos[1];

            return (x1 * y2 - x2 * y1);
        }
    }
}
