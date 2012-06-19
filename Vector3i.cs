using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace OpenTKTest
{
    public class Vector3i
    {

        public static Vector3i Zero = new Vector3i(0, 0, 0);

        public int X=0, Y=0, Z=0;

        public Vector3i()
        {
        }

        public Vector3i(int x, int y, int z)
        {
            X = x; Y = y; Z = z;
        }

        /// <summary>
        /// Performs integer division on all components of the vector.
        /// </summary>
        /// <param name="operand">Vector to be changed.</param>
        /// <param name="scalar">Integer to divide the coordinate by.</param>
        /// <returns>New coordinate.</returns>
        public static Vector3i operator /(Vector3i operand, int scalar)
        {
            return new Vector3i(operand.X / scalar, operand.Y / scalar, operand.Z / scalar);
        }

        public static Vector3i operator %(Vector3i operand, int scalar)
        {
            return new Vector3i(operand.X % scalar, operand.Y % scalar, operand.Z % scalar);
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }

        public override string ToString()
        {
            return String.Format("({0}, {1}, {2})", X, Y, Z);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector3i)) return false;
            Vector3i c = (Vector3i)obj;
            return this.X == c.X && this.Y == c.Y && this.Z == c.Z;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }
    }

    public static class V3Extension
    {
        public static Vector3i FloorToVector3i(this Vector3 vector3)
        {
            return new Vector3i((int)vector3.X, (int)vector3.Y, (int)vector3.Z);
        }
    }
}
