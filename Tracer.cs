using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace OpenTKTest
{
    public class Tracer
    {
        private Vector3 size = new Vector3();
        private Vector3 off = new Vector3();
        private Vector3 pos = new Vector3();
        private Vector3 direction = new Vector3();

        private Vector3i index = new Vector3i();

        private Vector3 delta = new Vector3();
        private Vector3i sign = new Vector3i();
        private Vector3 max = new Vector3();

        private int limit;
        private int plotted;

        public Tracer(float offx, float offy, float offz, float width, float height, float depth)
        {
            off = new Vector3(offx, offy, offz); //The origin??
            size = new Vector3(width, height, depth); //The size of a voxel/cube
        }

        public void plot(Vector3 position, Vector3 direction, int cells)
        {
            limit = cells; //The maximum distance a ray should be traced

            pos = position;
            this.direction = direction;
            this.direction.Normalize();

            delta = size;
            delta = new Vector3(delta.X / this.direction.X, delta.Y / this.direction.Y, delta.Z / this.direction.Z);

            sign.X = (this.direction.X > 0) ? 1 : (this.direction.X < 0 ? -1 : 0);
            sign.Y = (this.direction.Y > 0) ? 1 : (this.direction.Y < 0 ? -1 : 0);
            sign.Z = (this.direction.Z > 0) ? 1 : (this.direction.Z < 0 ? -1 : 0);

            reset();
        }


        public bool next()
        {
            if (plotted++ > 0)
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
            return (plotted <= limit);
        }

        public void reset()
        {
            plotted = 0;

            index.X = Game.Round ? (int)Math.Round((pos.X - off.X) / size.X) : (int)((pos.X - off.X) / size.X);
            index.Y = Game.Round ? (int)Math.Round((pos.Y - off.Y) / size.Y) : (int)((pos.Y - off.Y) / size.Y);
            index.Z = Game.Round ? (int)Math.Round((pos.Z - off.Z) / size.Z) : (int)((pos.Z - off.Z) / size.Z);

            float ax = index.X * size.X + off.X;
            float ay = index.Y * size.Y + off.Y;
            float az = index.Z * size.Z + off.Z;

            max.X = (sign.X > 0) ? ax + size.X - pos.X : pos.X - ax;
            max.Y = (sign.Y > 0) ? ay + size.Y - pos.Y : pos.Y - ay;
            max.Z = (sign.Z > 0) ? az + size.Z - pos.Z : pos.Z - az;
            max = new Vector3(max.X / direction.X, max.Y / direction.Y, max.Z / direction.Z);
        }

        public void end()
        {
            plotted = limit + 1;
        }


        public Vector3i get()
        {
            return index;
        }

        public Vector3 actual()
        {
            return new Vector3(index.X * size.X + off.X,
                    index.Y * size.Y + off.Y,
                    index.Z * size.Z + off.Z);
        }
    }
}
