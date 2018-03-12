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

        public static Bitmap myBitmap, texture;
        public Stars3D stars;
        public long previoustime;
        public long currenttime;
        float rotSpeed;
        Mesh mesh;

        Vertex minYVert = new Vertex(new Vector4f(-1, -1, 0, 1),
                                     new Vector4f(0.0f, 0.0f, 0.0f, 0.0f));
        Vertex midYVert = new Vertex(new Vector4f(0, 1, 0, 1),
                                     new Vector4f(0.5f, 1.0f, 0.0f, 0.0f));
        Vertex maxYVert = new Vertex(new Vector4f(1, -1, 0, 1),
                                     new Vector4f(1.0f, 0.0f, 0.0f, 0.0f));

        Matrix4f projection;

        float rotCounterX;
        float rotCounterY;
        float delta;

        public Display()
        { 
            InitializeComponent();

            SetScene();
            //stars = new Stars3D(3, 64.0f, 10.0f,70.0f);
            pictureBox1.Image = myBitmap;
             
            FrameRate();
            
        }

        private void SetScene()
        {
            
            rotSpeed = 1.0f;
            pictureBox1.Width = this.Width;
            pictureBox1.Height = this.Height;
            //pictureBox1.Location = new Point(0, 0);
            myBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            projection = Matrix4f.InitPerspective((float)ToRadian(70.0f),
                (float)(pictureBox1.Width / pictureBox1.Height), 0.1f, 1000.0f);
            //texture = new Bitmap(64, 64);
            //Random rand = new Random();
            //for (int i = 0; i < texture.Height; i++)
            //{
            //    for (int j = 0; j < texture.Width; j++)
            //    {
            //        //if ((i + j) % 2 == 0)
            //        //{
            //        //    texture.SetPixel(j, i, Color.FromArgb(255, 0, 0));
            //        //}
            //        //else
            //        //{
            //        //    texture.SetPixel(j, i, Color.FromArgb(255, 0, 255));
            //        //}
            //        texture.SetPixel(j, i,
            //            Color.FromArgb(
            //                rand.Next(1, 255),
            //                rand.Next(1, 255),
            //                rand.Next(1, 255)
            //            ));
            //    }
            //}

            //texture = new Bitmap("../../GW_avatar.jpg");
            
            texture = new Bitmap("../../Horizon.jpg");
            //texture = new Bitmap("../../BricksPattern.png");
            mesh = new Mesh("../../icosphere.obj");

            //rotCounter = 0.0f;
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

            currenttime = DateTime.Now.Ticks * 100;
            delta = (float)((currenttime - previoustime)/1000000000.0f);
            previoustime = currenttime;


            rotCounterY += delta * rotSpeed;
            //rotCounterX -= delta * rotSpeed;
            Matrix4f translation = Matrix4f.Translation(0.0f, 0.0f, 3.0f);
            Matrix4f rotation = Matrix4f.InitRotation(rotCounterX, rotCounterY, 0.0f);
            Matrix4f transform = projection * translation * rotation;
            myBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            DrawMesh(mesh, transform, texture);
            //FillTriangle(maxYVert.Transform(transform),
            //             midYVert.Transform(transform),
            //             minYVert.Transform(transform), texture);


            return myBitmap;
        }

        private void Display_SizeChanged(object sender, EventArgs e)
        {
            SetScene();
        }


        public void DrawMesh(Mesh mesh, Matrix4f transform, Bitmap texture)
        {
            for (int i = 0; i < mesh.GetNumIndices(); i+=3)
            {
                FillTriangle(mesh.GetVertex(mesh.GetIndex(i)).Transform(transform),
                             mesh.GetVertex(mesh.GetIndex(i + 1)).Transform(transform),
                             mesh.GetVertex(mesh.GetIndex(i + 2)).Transform(transform),
                             texture);
            }
        }

        public void FillTriangle(Vertex v1, Vertex v2, Vertex v3, Bitmap texture)
        {
            Matrix4f screenSpaceTransform = Matrix4f.InitScreenSpaceTransform(pictureBox1.Width / 2, pictureBox1.Height / 2);
            Vertex minYVert = v1.Transform(screenSpaceTransform).PerspectiveDivide();
            Vertex midYVert = v2.Transform(screenSpaceTransform).PerspectiveDivide();
            Vertex maxYVert = v3.Transform(screenSpaceTransform).PerspectiveDivide();

            if (minYVert.TriangleAreaTimesTwo(maxYVert, midYVert) >= 0)
            {
                return;
            }

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


            ScanTriangle(minYVert, midYVert, maxYVert, 
                            minYVert.TriangleAreaTimesTwo(maxYVert, midYVert) >= 0,
                            texture);

        }

        private static void ScanTriangle(Vertex minYVert, Vertex midYVert, Vertex maxYVert, bool handedness, Bitmap texture)
        {
            Gradients gradients = new Gradients(minYVert, midYVert, maxYVert);
            Edge topToBottom    = new Edge(gradients, minYVert, maxYVert, 0);
            Edge topToMiddle    = new Edge(gradients, minYVert, midYVert, 0);
            Edge middleToBottom = new Edge(gradients, midYVert, maxYVert, 1);
            
            
            ScanEdges(topToBottom, topToMiddle, handedness, texture);
            ScanEdges(topToBottom, middleToBottom, handedness, texture);

        }

        private static void ScanEdges(Edge a, Edge b, bool handedness, Bitmap texture)
        {
            Edge left  = a;
            Edge right = b;
            if (handedness)
            {
                Edge temp = left;
                left = right;
                right = temp;
            }

            int yStart = b.GetYStart();
            int yEnd   = b.GetYEnd();
            for (int j = yStart; j < yEnd; j++)
            {
                DrawScanLine(left, right, j, texture);
                left.Step();
                right.Step();
            }
        }

        private static void DrawScanLine(Edge left, Edge right, int j, Bitmap texture)
        {
            int xMin = (int)Math.Ceiling(left.GetX());
            int xMax = (int)Math.Ceiling(right.GetX());
            float xPrestep = xMin - left.GetX();

            float xDist = right.GetX() - left.GetX();
            float texCoordXXStep = (right.GetTexCoordX() - left.GetTexCoordX()) / xDist;
            float texCoordYXStep = (right.GetTexCoordY() - left.GetTexCoordY()) / xDist;
            float oneOverZXStep  = (right.GetOneOverZ()  - left.GetOneOverZ())  / xDist;

            float texCoordX = left.GetTexCoordX() + texCoordXXStep * xPrestep;
            float texCoordY = left.GetTexCoordY() + texCoordYXStep * xPrestep;
            float oneOverZ  = left.GetOneOverZ()  + oneOverZXStep  * xPrestep;

            for (int i = xMin; i < xMax; i++)
            {
                float z = 1.0f / oneOverZ;
                int srcX = (int)((texCoordX * z) * (float)(texture.Width - 1) /*+ 0.5f*/);
                int srcY = (int)((texCoordY * z) * (float)(texture.Height - 1) /*+ 0.5f*/);

                //if (srcX >= texture.Width)
                //{
                //    MessageBox.Show("srcX >= texture.Width");
                //}
                
                try
                {
                   myBitmap.SetPixel(i, j, texture.GetPixel(srcX, srcY));
                }
                catch (Exception)
                {

                }
                oneOverZ  += oneOverZXStep;
                texCoordX += texCoordXXStep;
                texCoordY += texCoordYXStep;

            }
        }

        public static double ToRadian(double angle)
        {
            return (Math.PI * angle) / 180;
        }

        private void Display_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D)
            {
                rotCounterY += delta * rotSpeed;
            }
            else if (e.KeyCode == Keys.A)
            {
                rotCounterY -= delta * rotSpeed;
            }

            if (e.KeyCode == Keys.W)
            {
                rotCounterX += delta * rotSpeed;
            }
            else if (e.KeyCode == Keys.S)
            {
                rotCounterX -= delta * rotSpeed;
            }

            if (e.KeyCode == Keys.Q)
            {
                rotSpeed -= 0.1f;
            }
            else if (e.KeyCode == Keys.E)
            {
                rotSpeed += 0.1f;
            }
        }

        public static double ToDegree(double radian)
        {
            return radian * (180 / Math.PI);
        }

    }
}
