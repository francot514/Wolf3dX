using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Nexxt.Engine.MathUtilities
{
    public class MathUtils
    {
        const int RAND_MAX = 0x7fff;
        public static double RandFloat() { return ((new Random().Next()) / (RAND_MAX + 1.0)); }
        public static double RandomClamped() { return RandFloat() - RandFloat(); }

        /// <summary>
        /// Calculates the perpendicular vector to the given v vector
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 Perpendicular(Vector2 v)
        {
            return new Vector2(-v.Y, v.X);
        }

        /// <summary>
        /// Transforms a point from the agent's local space into world space
        /// </summary>
        /// <param name="point"></param>
        /// <param name="AgentHeading"></param>
        /// <param name="AgentSide"></param>
        /// <param name="AgentPosition"></param>
        /// <returns></returns>
        public static Vector2 PointToWorldSpace(Vector2 point,
                                  Vector2 AgentHeading,
                                  Vector2 AgentSide,
                                  Vector2 AgentPosition)
        {
            //make a copy of the point
            Vector2 TransPoint = point;
            Matrix matTransform = Matrix.Identity;
            C2DMatrixRotate(ref matTransform, AgentHeading, AgentSide);
            C2DMatrixTranslate(ref matTransform, AgentPosition.X, AgentPosition.Y);
            C2DMatrixTransformVector2Ds(matTransform, ref TransPoint);
            return TransPoint;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="AgentHeading"></param>
        /// <param name="AgentSide"></param>
        /// <param name="AgentPosition"></param>
        /// <returns></returns>
        public static Vector2 PointToLocalSpace(Vector2 point,
                             Vector2 AgentHeading,
                             Vector2 AgentSide,
                              Vector2 AgentPosition)
        {

            //make a copy of the point
            Vector2 TransPoint = point;

            //create a transformation matrix
            Matrix matTransform = Matrix.Identity;

            float Tx = -Vector2.Dot(AgentPosition, AgentHeading);
            float Ty = -Vector2.Dot(AgentPosition, AgentSide);

            //create the transformation matrix
            matTransform.M11 = AgentHeading.X; matTransform.M12 = AgentSide.X;
            matTransform.M21 = AgentHeading.Y; matTransform.M22 = AgentSide.Y;
            matTransform.M31 = Tx; matTransform.M32 = Ty;

            //now transform the vertices
            C2DMatrixTransformVector2Ds(matTransform, ref TransPoint);

            return TransPoint;
        }

        public static Vector2 VectorToWorldSpace(Vector2 vec,
                                   Vector2 AgentHeading,
                                   Vector2 AgentSide)
        {
            //make a copy of the point
            Vector2 TransVec = vec;

            //create a transformation matrix
            Matrix matTransform = Matrix.Identity;

            C2DMatrixRotate(ref matTransform, AgentHeading, AgentSide);
            C2DMatrixTransformVector2Ds(matTransform, ref TransVec);

            return TransVec;
        }

        public static void Vec2DRotateAroundOrigin(ref Vector2 v, double ang)
        {
            Matrix mat = Matrix.Identity;
            C2DMatrixRotate(ref mat, ang);
            C2DMatrixTransformVector2Ds(mat, ref v);
        }

        public static bool IsPointInTriangle(Vector2 point, Vector2 pa, Vector2 pb, Vector2 pc)
        {
            Vector2 e10 = pb - pa;
            Vector2 e20 = pc - pa;

            float a = Vector2.Dot(e10, e10);
            float b = Vector2.Dot(e10, e20);
            float c = Vector2.Dot(e20, e20);
            float ac_bb = (a * c) - (b * b);
            Vector2 vp = new Vector2(point.X - pa.X, point.Y - pa.Y);
            float d = Vector2.Dot(vp, e10);
            float e = Vector2.Dot(vp, e20);
            float x = (d * c) - (e * b);
            float y = (e * a) - (d * b);
            float z = x + y - ac_bb;
            uint res = ((uint)z & ~((uint)x | (uint)y));
            res = res & 0x80000000;
            return res > 0;
        }


        //create a rotation matrix from a 2D vector
        public static void C2DMatrixRotate(ref Matrix matrix, Vector2 fwd, Vector2 side)
        {
            Matrix mat = new Matrix(); ;

            mat.M11 = fwd.X; mat.M12 = fwd.Y; mat.M13 = 0;

            mat.M21 = side.X; mat.M22 = side.Y; mat.M23 = 0;

            mat.M31 = 0; mat.M32 = 0; mat.M33 = 1;

            matrix = Matrix.Multiply(matrix, mat);
        }

        public static void C2DMatrixRotate(ref Matrix matrix, double rot)
        {
            Matrix mat = Matrix.Identity;

            float Sin = (float)Math.Sin(rot);
            float Cos = (float)Math.Cos(rot);

            mat.M11 = Cos; mat.M12 = Sin; mat.M13 = 0;

            mat.M21 = -Sin; mat.M22 = Cos; mat.M23 = 0;

            mat.M31 = 0; mat.M32 = 0; mat.M33 = 1;

            //and multiply
            matrix = Matrix.Multiply(matrix, mat);
        }

        public static void C2DMatrixTranslate(ref Matrix matrix, float x, float y)
        {
            Matrix mat = Matrix.Identity; ;

            mat.M11 = 1; mat.M12 = 0; mat.M13 = 0;

            mat.M21 = 0; mat.M22 = 1; mat.M23 = 0;

            mat.M31 = x; mat.M32 = y; mat.M33 = 1;

            //and multiply
            matrix = Matrix.Multiply(matrix, mat);
        }

        public static void C2DMatrixTransformVector2Ds(Matrix matrix, ref Vector2 vPoint)
        {

            float tempX = (matrix.M11 * vPoint.X) + (matrix.M21 * vPoint.Y) + (matrix.M31);
            float tempY = (matrix.M12 * vPoint.X) + (matrix.M22 * vPoint.Y) + (matrix.M32);
            vPoint.X = tempX;
            vPoint.Y = tempY;
        }
    }
}
