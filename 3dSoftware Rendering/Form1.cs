﻿using System;
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

        Vertex minYVert = new Vertex(100, 50);
        Vertex midYVert = new Vertex(0, 200);
        Vertex maxYVert = new Vertex(50, 300);

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
        }

        private async void FrameRate()
        {
            previoustime = DateTime.Now.Ticks * 100; //DateTime.Now.Millisecond;
            while (true)
            {
                pictureBox1.Image = await DoWork();
                //panel1.Refresh();
                button1.Text = stars.times.ToString();
            }

        }
        private async Task<Bitmap> DoWork()
        {
            // Do actual work
            await Task.Delay(1);

            currenttime = DateTime.Now.Ticks * 100;//DateTime.Now.Millisecond;
            float delta = (float)((currenttime - previoustime)/1000000000.0f);
            previoustime = currenttime;
            stars.UpdateAndRender(ref myBitmap, delta);

            //for (int j = 100; j < 200; j++)
            //{
            //    DrawScanBuffer(j, 300 - j, 300 + j);
            //}
            //myBitmap = new Bitmap(myBitmap.Width, myBitmap.Height);

            //FillTriangle(maxYVert, midYVert,minYVert);
            return myBitmap;
        }

        private void Display_SizeChanged(object sender, EventArgs e)
        {
            SetScene();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
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
                int xMin = m_scanBuffer[j * 2];
                int xMax = m_scanBuffer[j * 2 + 1];

                for (int i = xMin; i < xMax; i++)
                {
                    
                    myBitmap.SetPixel(i, j, Color.FromArgb(255, colorChoice));
                }
            }
        }

        public static void FillTriangle(Vertex v1,Vertex v2, Vertex v3)
        {
            Vertex minYVert = v1;
            Vertex midYVert = v2;
            Vertex maxYVert = v3;

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
                m_scanBuffer[j * 2 + whichSide] = (int)curX;
                curX += xStep;
            }

        }

    }
}