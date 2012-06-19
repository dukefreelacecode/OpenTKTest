using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace OpenTKTest
{
    public class NTracer
    {
        private Vector3 pos = new Vector3();
        private Vector3 direction = new Vector3();

        private Vector3i index = new Vector3i();

        private Vector3 delta = new Vector3();
        private Vector3i sign = new Vector3i();
        private Vector3 max = new Vector3();

        private int limit;

        public NTracer()
        {
        }

        public void plot(Vector3 position, Vector3 direction, int cells)
        {
            limit = cells; //The maximum distance a ray should be traced

            pos = position;

            this.direction = direction;
            this.direction.Normalize();

            delta = new Vector3(1 / this.direction.X, 1 / this.direction.Y, 1 / this.direction.Z);

            sign.X = (this.direction.X > 0) ? 1 : (this.direction.X < 0 ? -1 : 0);
            sign.Y = (this.direction.Y > 0) ? 1 : (this.direction.Y < 0 ? -1 : 0);
            sign.Z = (this.direction.Z > 0) ? 1 : (this.direction.Z < 0 ? -1 : 0);

            reset();
        }


        public void next()
        {
            float mx = sign.X * max.X;
            float my = sign.Y * max.Y;
            float mz = sign.Z * max.Z;

            if (mx < my && mx < mz)
            {
                max.X += delta.X;
                index.X += sign.X;
            }
            else if (mz < my && mz < mx)
            {
                max.Z += delta.Z;
                index.Z += sign.Z;
            }
            else
            {
                max.Y += delta.Y;
                index.Y += sign.Y;
            }
        }

        public void reset()
        {
            index.X = (int)(pos.X - 0.5f);
            index.Y = (int)(pos.Y - 0.5f);
            index.Z = (int)(pos.Z - 0.5f);

            float ax = index.X + 0.5f;
            float ay = index.Y + 0.5f;
            float az = index.Z + 0.5f;

            max.X = (sign.X > 0) ? ax + 1 - pos.X : pos.X - ax;
            max.Y = (sign.Y > 0) ? ay + 1 - pos.Y : pos.Y - ay;
            max.Z = (sign.Z > 0) ? az + 1 - pos.Z : pos.Z - az;
            max = new Vector3(max.X / direction.X, max.Y / direction.Y, max.Z / direction.Z);
        }

        public Vector3i get()
        {
            return index;
        }
    }
}