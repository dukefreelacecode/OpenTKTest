using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace OpenTKTest
{
    class ChunkManager
    {
        struct Coord3i
        {
            public int X, Y, Z;
            public Coord3i(int x, int y, int z) {
                X = x; Y = y; Z = z;
            }
        }

        Dictionary<Coord3i, Chunk> chunks = new Dictionary<Coord3i, Chunk>();
        public ChunkManager()
        {
            Chunk c1 = new Chunk(), c2 = new Chunk();
            c1.Generate(); c2.Generate();
            chunks[new Coord3i(0, 0, 0)] = c1;
            chunks[new Coord3i(1, 0, 0)] = c2;
        }

        public void DrawChunks()
        {
            foreach (var kvp in chunks)
            {
                GL.Translate(kvp.Key.X * Chunk.CHUNK_SIZE_1D,
                    kvp.Key.Y * Chunk.CHUNK_SIZE_1D,
                    kvp.Key.Z * Chunk.CHUNK_SIZE_1D);
                kvp.Value.Draw();
            }
        }
    }
}
