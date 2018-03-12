using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;



namespace _3dSoftware_Rendering
{
    public partial class Display : Form
    {
        
        public static Bitmap myBitmap;
        public Stars3D stars;
        public long previoustime;
        public long currenttime;
        public static int[] m_scanBuffer;

        Vertex minYVert = new Vertex(-1, -1, 0);
        Vertex midYVert = new Vertex( 0,  1, 0);
        Vertex maxYVert = new Vertex( 1, -1, 0);

        Matrix4f projection;

        float rotCounter;

        public Display()
        { 
            InitializeComponent();

            SetScene();
            stars = new Stars3D(3, 64.0f, 10.0f,70.0f);
            pictureBox1.Image = myBitmap;
             
            FrameRate();
            
        }

        private void SetScene()
        {
            pictureBox1.Width = this.Width;
            pictureBox1.Height = this.Height;
            pictureBox1.Location = new Point(0, 0);
            m_scanBuffer = new int[pictureBox1.Height * 2];
            myBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            projection =  Matrix4f.InitPerspective((float)ToRadian(70.0f), (float)(myBitmap.Width / myBitmap.Height), 0.1f, 1000.0f);
            rotCounter = 0.0f;
        }

        private async void FrameRate()
        {
            previoustime = DateTime.Now.Ticks * 100; //DateTime.Now.Millisecond;
            while (true)
            {
                pictureBox1.Image = await DoWork();
                //panel1.Refresh();
                //button1.Text = stars.times.ToString();
            }

        }
        private async Task<Bitmap> DoWork()
        {
            // Do actual work
            await Task.Delay(1);

            currenttime = DateTime.Now.Ticks * 100;//DateTime.Now.Millisecond;
            float delta = (float)((currenttime - previoustime)/1000000000.0f);
            previoustime = currenttime;
            //stars.UpdateAndRender(ref myBitmap, delta);

            rotCounter += delta;
            Matrix4f translation = Matrix4f.Translation(0.0f, 0.0f, 3.0f);
            Matrix4f rotation = Matrix4f.InitRotation(0.0f, rotCounter, 0.0f);
            Matrix4f transform = projection * (translation * rotation);
            myBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            FillTriangle(maxYVert.Transform(transform), midYVert.Transform(transform), minYVert.Transform(transform));


            return myBitmap;
        }

        private void Display_SizeChanged(object sender, EventArgs e)
        {
            SetScene();
        }

        public static void DrawScanBuffer(int yCoord, int xMin, int xMax)
        {
            m_scanBuffer[yCoord * 2    ] = xMin;
            m_scanBuffer[yCoord * 2 + 1] = xMax;
        }

        
        public static void FillShape(int yMin, int yMax, Color colorChoice)
        {
            
            
            for (int j = yMin; j < yMax; j++)
            {
                if (j >= myBitmap.Height)
                {
                    break;
                }
                int xMin = m_scanBuffer[j * 2];
                int xMax = m_scanBuffer[j * 2 + 1];

                for (int i = xMin; i < xMax; i++)
                {
                    if (i >= myBitmap.Width)
                    {
                        break;
                    }
                    myBitmap.SetPixel(i, j, Color.FromArgb(255, colorChoice));
                }
            }
        }

        public static void FillTriangle(Vertex v1,Vertex v2, Vertex v3)
        {
            Matrix4f screenSpaceTransform = Matrix4f.InitScreenSpaceTransform(myBitmap.Width / 2, myBitmap.Height / 2);
            Vertex minYVert = v1.Transform(screenSpaceTransform).PerspectiveDivide();
            Vertex midYVert = v2.Transform(screenSpaceTransform).PerspectiveDivide();
            Vertex maxYVert = v3.Transform(screenSpaceTransform).PerspectiveDivide();

            if (maxYVert.GetY() < midYVert.GetY())
            {
                Vertex temp = maxYVert;
                maxYVert = midYVert;
                midYVert = temp;
            }

            if (midYVert.GetY() < minYVert.GetY())
            {
                Vertex temp = midYVert;
                midYVert = minYVert;
                minYVert = temp;
            }

            if (maxYVert.GetY() < midYVert.GetY())
            {
                Vertex temp = maxYVert;
                maxYVert = midYVert;
                midYVert = temp;
            }


            float area = minYVert.TriangleAreaTimesTwo(maxYVert, midYVert);
            int handedness = area >= 0 ? 1 : 0;

            ScanConvertTriangle(minYVert, midYVert, maxYVert, handedness);
            FillShape((int)minYVert.GetY(),(int)maxYVert.GetY(),minYVert.GetColor());
        }

        private static void ScanConvertTriangle(Vertex minYVert, Vertex midYVert, Vertex maxYVert, int handedness)
        {
            ScanConvertLine(minYVert, maxYVert, 0 + handedness);
            ScanConvertLine(minYVert, midYVert, 1 - handedness);
            ScanConvertLine(midYVert, maxYVert, 1 - handedness);
        }

        private static void ScanConvertLine(Vertex minYVert, Vertex maxYVert, int whichSide)
        {
            int yStart = (int)minYVert.GetY();
            int yEnd   = (int)maxYVert.GetY();
            int xStart = (int)minYVert.GetX();
            int xEnd   = (int)maxYVert.GetX();

            int yDist = yEnd - yStart;
            int xDist = xEnd - xStart;

            if (yDist <= 0)
            {
                return;
            }

            float xStep = (float)xDist / (float)yDist;
            float curX = (float)xStart;

            for (int j = yStart; j < yEnd; j++)
            {
                if (j >= myBitmap.Height)
                {
                    break;
                }
                m_scanBuffer[j * 2 + whichSide] = (int)curX;
                curX += xStep;
            }

        }

        public static double ToRadian(double angle)
        {
            return (Math.PI * angle) / 180;
        }

        public static double ToDegree(double radian)
        {
            return radian * (180 / Math.PI);
        }

    }
}
