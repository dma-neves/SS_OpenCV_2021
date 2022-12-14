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

    struct Vector3D
    {
        public double x, y, z;
    }

    internal class SSUtils
    {
        public static Vector3D CrossProd(Vector3D v1, Vector3D v2)
        {
            Vector3D cp = new Vector3D { x=0,y=0,z=0};
            cp.x = v1.y * v2.z - v1.z * v2.y;
            cp.y = v1.z * v2.x - v1.x * v2.z;
            cp.z = v1.x * v2.y - v1.y * v2.y;

            return cp;
        }

        public static Vector3D ToVector3D(Vector2D v)
        {
            return new Vector3D { x = v.x, y = v.y, z = 0.0 };
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
            if(v1.x/v2.x == v1.y/v2.y && v1.x * v2.x < 0)
                return Math.PI;

            Vector3D cp = CrossProd(ToVector3D(Normalize(v1)), ToVector3D(Normalize(v2)));
            return Math.Asin(cp.z);
        }

        public static Vector2D ScaleVector(Vector2D v, double scale)
        {
            return new Vector2D() { x=v.x*scale, y=v.y*scale };
        }

        public static Vector2D AddVectors(Vector2D v1, Vector2D v2)
        {
            return new Vector2D() { x=v1.x+v2.x, y=v1.y+v2.y };
        }
    }
}
