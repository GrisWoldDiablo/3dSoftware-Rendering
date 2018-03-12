using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace _3dSoftware_Rendering
{
    public class Stars3D
    {
        private float m_spread;
        private float m_speed;
        private float m_FOV;
        private float[] m_starX;
        private float[] m_starY;
        private float[] m_starZ;
        public int times = 0;

        Random rand;
        public Stars3D(int numStars, float spread, float speed, float FOV)
        {

            m_spread = spread;
            m_speed = speed;
            m_FOV = FOV;

            m_starX = new float[numStars];
            m_starY = new float[numStars];
            m_starZ = new float[numStars];
            rand = new Random();
            for (int i = 0; i < m_starX.Length; i++)
            {
                InitStar(i);
            }
        }

        private void InitStar(int i)
        {
            m_starX[i] = 2 * ((float)rand.NextDouble() - 0.5f) * m_spread;
            m_starY[i] = 2 * ((float)rand.NextDouble() - 0.5f) * m_spread;
            m_starZ[i] = ((float)rand.NextDouble() + 0.00001f) * m_spread;
        }

        public void UpdateAndRender(ref Bitmap target, float delta)
        {
            
            target = new Bitmap(target.Width, target.Height);
            
            float tanHalfFOV = (float)Math.Tan((Math.PI * (m_FOV / 2.0) / 180.0));
            float halfWidth = target.Width / 2.0f;
            float halfHeight = target.Height / 2.0f;
            int triangleBuilderCounter = 0;

            int x1 = 0;
            int y1 = 0;
            int x2 = 0;
            int y2 = 0;


            for (int i = 0; i < m_starX.Length; i++)
            {
                m_starZ[i] -= (delta * m_speed);
                if (m_starZ[i] <= 0)
                {
                    InitStar(i);
                }

                int x = (int)((m_starX[i]/(m_starZ[i] * tanHalfFOV)) * halfWidth + halfWidth);
                int y = (int)((m_starY[i]/(m_starZ[i] * tanHalfFOV)) * halfHeight + halfHeight);

                if ((x < 0 || x >= target.Width) || (y < 0 || y >= target.Height))
                {
                    InitStar(i);
                    continue;
                }
                //else
                //{
                //    target.SetPixel(x, y, Color.FromArgb(255, 255, 250, 255));
                //}
                triangleBuilderCounter++;
                if (triangleBuilderCounter == 1)
                {
                    x1 = x;
                    y1 = y;
                }
                else if (triangleBuilderCounter == 2)
                {
                    x2 = x;
                    y2 = y;
                }
                else if (triangleBuilderCounter == 3)
                {
                    triangleBuilderCounter = 0;
                    //Vertex v1 = new Vertex(x1, y1,0);
                    //Vertex v2 = new Vertex(x2, y2,0);
                    //Vertex v3 = new Vertex(x, y,0);

                    //Display.FillTriangle(v1, v2, v3);
                }
            }
            //return target;

        }
    }
}
