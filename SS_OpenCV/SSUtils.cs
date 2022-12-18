using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS_OpenCV
{
    struct Vector2D
    {
        public double x, y;
    }

    internal class SSUtils
    {
        public static double CrossProd(Vector2D v1, Vector2D v2)
        {
            return v1.x * v2.y - v1.y * v2.x;
        }

        public static double Norm(Vector2D v)
        {
            return Math.Sqrt(v.x * v.x + v.y * v.y);
        }

        public static Vector2D Normalize(Vector2D v)
        {
            double n = Norm(v);
            return new Vector2D() { x=v.x/n, y=v.y/n};
        }

        // Returns the smallest angle between v1 and v2, including the direction of rotation, represented by
        // the sign of the returned value. + being counter-clock-wise and - being clock-wise. Therefore, this
        // represents the angle that rotates v1 to v2
        public static double AngleFromV1ToV2(Vector2D v1, Vector2D v2)
        {
            if((v1.x == 0 && v2.x == 0) || (v1.y == 0 && v2.y == 0) || v1.x/v2.x == v1.y/v2.y)
            {
                if (v1.x * v2.x < 0)
                    return Math.PI;
                else
                    return 0;
            }

            return Math.Asin(CrossProd((Normalize(v1)), (Normalize(v2))) );
        }

        public static Vector2D ScaleVector(Vector2D v, double scale)
        {
            return new Vector2D() { x=v.x*scale, y=v.y*scale };
        }

        public static Vector2D AddVectors(Vector2D v1, Vector2D v2)
        {
            return new Vector2D() { x=v1.x+v2.x, y=v1.y+v2.y };
        }

        public static Vector2D SubVectors(Vector2D v1, Vector2D v2)
        {
            return new Vector2D() { x = v1.x - v2.x, y = v1.y - v2.y };
        }

        public static Vector2D RotateVector(Vector2D v, double angle)
        {
            return new Vector2D() {
                x = v.x * Math.Cos(angle) - v.y * Math.Sin(angle),
                y = v.x * Math.Sin(angle) + v.y * Math.Cos(angle)
            };
        }

        public static Vector2D RotateVectorAroundPoint(Vector2D v, double angle, Vector2D center)
        {
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);

            // translate point back to origin:
            double x = v.x - center.x;
            double y = v.y - center.y;

            // rotate point
            double xnew = x * cos - y * sin;
            double ynew = x * sin + y * cos;

            // translate point back:
            x = xnew + center.x;
            y = ynew + center.y;

            return new Vector2D()
            {
                x = x,
                y = y
                //x = ((v.x - center.x) * Math.Cos(angle)) - ((center.y - v.y) * Math.Sin(angle)) + center.x,
                //y = center.y - ((center.y - v.y) * Math.Cos(angle)) + ((v.x - center.x) * Math.Sin(angle))
            };
        }

        public static Vector2D ShearVector(Vector2D v, double x_shear, double y_shear)
        {
            return new Vector2D()
            {
                x = v.x + x_shear * v.y,
                y = y_shear * v.x + v.y
            };
        }
    }
}
