using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;


namespace _3dSoftware_Rendering
{
    
    public class Matrix4f
    {
        private float[,] matrix;

        public float[,] Matrix { get => matrix; set => matrix = value; }

        public float this[int i, int j]
        {
            get { return Matrix[i,j]; }
            set { Matrix[i,j] = value;}
        }

        public Matrix4f()
        {
            Matrix = new float[4, 4];
        }

        public Matrix4f(float value)
        {
            Matrix = new float[4, 4];
            Matrix[0, 0] = value;
            Matrix[1, 1] = value;
            Matrix[2, 2] = value;
            Matrix[3, 3] = value;
        }

        public Matrix4f(float[,] values)
        {
            Matrix = values; 
        }


        public static Matrix4f InitScreenSpaceTransform(float halfWidth, float halfHeight)
        {
            return new Matrix4f(new float[,]
            {
                { halfWidth, 0, 0, halfWidth },
                { 0, -halfHeight, 0, halfHeight },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 }
            });
        }

        public static Matrix4f operator *(Matrix4f left, Matrix4f right)
        {

            //Matrix4f result = new Matrix4f(new float[,]
            //{
            //    {
            //        (left[0,0] * right[0,0]) + (left[0,1] * right[1,0]) + (left[0,2] * right[2,0]) + (left[0,3] * right[3,0]),
            //        (left[0,0] * right[0,1]) + (left[0,1] * right[1,1]) + (left[0,2] * right[2,1]) + (left[0,3] * right[3,1]),
            //        (left[0,0] * right[0,2]) + (left[0,1] * right[1,2]) + (left[0,2] * right[2,2]) + (left[0,3] * right[3,2]),
            //        (left[0,0] * right[0,3]) + (left[0,1] * right[1,3]) + (left[0,2] * right[2,3]) + (left[0,3] * right[3,3])},                                                                                                                                                                                                                                                                                                                                   
            //    {
            //        (left[1,0] * right[0,0]) + (left[1,1] * right[1,0]) + (left[1,2] * right[2,0]) + (left[1,3] * right[3,0]),
            //        (left[1,0] * right[0,1]) + (left[1,1] * right[1,1]) + (left[1,2] * right[2,1]) + (left[1,3] * right[3,1]),
            //        (left[1,0] * right[0,2]) + (left[1,1] * right[1,2]) + (left[1,2] * right[2,2]) + (left[1,3] * right[3,2]),
            //        (left[1,0] * right[0,3]) + (left[1,1] * right[1,3]) + (left[1,2] * right[2,3]) + (left[1,3] * right[3,3])},                                                                                                                                                                                                                                                                                                                                   
            //    {
            //        (left[2,0] * right[0,0]) + (left[2,1] * right[1,0]) + (left[2,2] * right[2,0]) + (left[2,3] * right[3,0]),
            //        (left[2,0] * right[0,1]) + (left[2,1] * right[1,1]) + (left[2,2] * right[2,1]) + (left[2,3] * right[3,1]),
            //        (left[2,0] * right[0,2]) + (left[2,1] * right[1,2]) + (left[2,2] * right[2,2]) + (left[2,3] * right[3,2]),
            //        (left[2,0] * right[0,3]) + (left[2,1] * right[1,3]) + (left[2,2] * right[2,3]) + (left[2,3] * right[3,3])},                                                                                                                                                                                                                                                                                                                                   
            //    {
            //        (left[3,0] * right[0,0]) + (left[3,1] * right[1,0]) + (left[3,2] * right[2,0]) + (left[3,3] * right[3,0]),
            //        (left[3,0] * right[0,1]) + (left[3,1] * right[1,1]) + (left[3,2] * right[2,1]) + (left[3,3] * right[3,1]),
            //        (left[3,0] * right[0,2]) + (left[3,1] * right[1,2]) + (left[3,2] * right[2,2]) + (left[3,3] * right[3,2]),
            //        (left[3,0] * right[0,3]) + (left[3,1] * right[1,3]) + (left[3,2] * right[2,3]) + (left[3,3] * right[3,3])}
            //}
            //);

            Matrix4f result = new Matrix4f();
            for (int i = 0; i < 4; i++)//0
            {
                for (int j = 0; j < 4; j++)//0
                {
                    for (int k = 0; k < 4; k++)//0
                    {
                        result[i, j] += left[i, k] * right[k, j];
                    }
                }
            }

            return result;
        }

        public static Vector4f operator *(Matrix4f left, Vector4f right)
        {
            Vector4f result = new Vector4f(new float[]
            {
                (left[0,0] * right[0]) + (left[0,1] * right[1]) + (left[0,2] * right[2]) + (left[0,3] * right[3]),
                (left[1,0] * right[0]) + (left[1,1] * right[1]) + (left[1,2] * right[2]) + (left[1,3] * right[3]),
                (left[2,0] * right[0]) + (left[2,1] * right[1]) + (left[2,2] * right[2]) + (left[2,3] * right[3]),
                (left[3,0] * right[0]) + (left[3,1] * right[1]) + (left[3,2] * right[2]) + (left[3,3] * right[3])
            });

            //Vector4f result = new Vector4f();
            //for (int i = 0; i < 4; i++)
            //{
            //    for (int j = 0; j < 4; j++)
            //    {
            //        result[i] += (left[i, j] * right[j]);
            //    }
            //}
            return result;
        }

        public static Matrix4f Identify()
        {
            return new Matrix4f(new float[,] 
            { 
                { 1, 0, 0, 0 }, 
                { 0, 1, 0, 0 }, 
                { 0, 0, 1, 0 }, 
                { 0, 0, 0, 1 }
            });
        }

        public static Matrix4f Translation(float x,float y,float z)
        {
            return new Matrix4f(new float[,]
            {
                { 1, 0, 0, x },
                { 0, 1, 0, y },
                { 0, 0, 1, z },
                { 0, 0, 0, 1 }
            });
        }

        public static Matrix4f Scalar(float x, float y, float z)
        {
            return new Matrix4f(new float[,]
            {
                { x, 0, 0, 0 },
                { 0, y, 0, 0 },
                { 0, 0, z, 0 },
                { 0, 0, 0, 1 }
            });
        }

        public float Det2x2(float[,] m)
        {
            return (m[0, 0] * m[1, 1]) - (m[0, 1] * m[1, 0]);
        }

        public static Matrix4f InitPerspective(float fov, float aspectRatio, float zNear, float zFar)
        {
            float tanHalfFOV = (float)Math.Tan(fov / 2);
            float zRange = zNear - zFar;
            return new Matrix4f(new float[,]
            {
                { 1.0f / (tanHalfFOV * aspectRatio), 0                , 0                       , 0                         },
                { 0                                , 1.0f / tanHalfFOV, 0                       , 0                         },
                { 0                                , 0                , (-zNear - zFar) / zRange, 2 * zFar * zNear / zRange },
                { 0                                , 0                , 1                       , 0                         }
            });
        }

        public static Matrix4f InitRotation(float x, float y, float z, float angle)
        {
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);

            return new Matrix4f(new float[,]
            {
                { cos + x * x * (1 - cos)    , x * y * (1 - cos) - z * sin, x * z * (1 - cos) + y      , 0 },
                { y * x * (1 - cos) + z * sin, cos + y * y * (1 - cos)    , y * z * (1 - cos) - x * sin, 0 },
                { z * x * (1 - cos) - y * sin, z * y * (1 - cos) + x * sin, cos + z * z * (1 - cos)    , 0 },
                { 0                          , 0                          , 0                          , 1 }
            });

        }

        public static Matrix4f InitRotation(float x, float y, float z)
        {
            Matrix4f rx = new Matrix4f(new float[,]
            {
                { 1,                   0,                   0, 0 },
                { 0,  (float)Math.Cos(x), -(float)Math.Sin(x), 0 },
                { 0,  (float)Math.Sin(x),  (float)Math.Cos(x), 0 },
                { 0,                   0,                   0, 1 }
            });

            Matrix4f ry = new Matrix4f(new float[,]
            {
                { (float)Math.Cos(y), 0, -(float)Math.Sin(y), 0 },
                {                  0, 1,                   0, 0 },
                { (float)Math.Sin(y), 0,  (float)Math.Cos(y), 0 },
                {                  0, 0,                   0, 1 }
            });
            Matrix4f rz = new Matrix4f(new float[,]
            {
                { (float)Math.Cos(z), -(float)Math.Sin(z), 0, 0 },
                { (float)Math.Sin(z),  (float)Math.Cos(z), 0, 0 },
                {                  0,                   0, 1, 0 },
                {                  0,                   0, 0, 1 }
            });

            //m = rz.Mul(ry.Mul(rx)).GetM();

            return rz * ry * rx;
        }

    }
    
}
