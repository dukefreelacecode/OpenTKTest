using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace OpenTKTest
{
    public class Chunk
    {
        private static readonly int BLOCK_RENDER_SIZE = 1;

        public static readonly int CHUNK_SIZE_1D = 32;
        public static readonly int CHUNK_SIZE_3D;
        BlockType[, ,] blocks = new BlockType[CHUNK_SIZE_1D, CHUNK_SIZE_1D, CHUNK_SIZE_1D];

        static Chunk()
        {
            CHUNK_SIZE_3D = (int)Math.Pow(CHUNK_SIZE_1D, 3);
        }

        public Chunk()
        {
        }

        List<Vector3> vertexData = new List<Vector3>();
        public void Generate()
        {
            GL.GenBuffers(2, VBOid);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid[0]);

            for (var x = 0; x < CHUNK_SIZE_1D; x++)
                for (var y = 0; y < CHUNK_SIZE_1D; y++)
                    for (var z = 0; z < CHUNK_SIZE_1D; z++)
                        blocks[x, y, z] = BlockType.STONE;

            for (var x = 0; x < CHUNK_SIZE_1D; x++)
            {
                for (var y = 0; y < CHUNK_SIZE_1D; y++)
                {
                    for (var z = 0; z < CHUNK_SIZE_1D; z++)
                    {
                        var blockType = blocks[x, y, z];
                        if (blockType == BlockType.AIR) continue;
                        if (!blockType.IsSolid())
                        {
                            Console.WriteLine(String.Format("Non-solid block at ({0}, {1}, {2}). Skipping rendering.", x, y, z));
                            continue;
                        }

                        bool posX, negX, posY, negY, posZ, negZ; //decide whether or not to render faces in those directions

                        if (x == 0)
                        {
                            negX = true;
                            posX = !blocks[x + 1, y, z].IsSolid();
                        }
                        else if (x == CHUNK_SIZE_1D - 1)
                        {
                            posX = true;
                            negX = !blocks[x - 1, y, z].IsSolid();
                        }
                        else
                        {
                            posX = !blocks[x + 1, y, z].IsSolid();
                            negX = !blocks[x - 1, y, z].IsSolid();
                        }


                        if (y == 0)
                        {
                            negY = true;
                            posY = !blocks[x, y + 1, z].IsSolid();
                        }
                        else if (y == CHUNK_SIZE_1D - 1)
                        {
                            posY = true;
                            negY = !blocks[x, y - 1, z].IsSolid();
                        }
                        else
                        {
                            posY = !blocks[x, y + 1, z].IsSolid();
                            negY = !blocks[x, y - 1, z].IsSolid();
                        }


                        if (z == 0)
                        {
                            negZ = true;
                            posZ = !blocks[x, y, z + 1].IsSolid();
                        }
                        else if (z == CHUNK_SIZE_1D - 1)
                        {
                            posZ = true;
                            negZ = !blocks[x, y, z - 1].IsSolid();
                        }
                        else
                        {
                            posZ = !blocks[x, y, z + 1].IsSolid();
                            negZ = !blocks[x, y, z - 1].IsSolid();
                        }

                        AddVoxel(blockType, x, y, z, posX, negX, posY, negY, posZ, negZ);
                    }
                }
            }

            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertexData.Count * 12), vertexData.ToArray(), BufferUsageHint.StaticDraw);            
        }

        uint[] VBOid = new uint[2];
        private void AddVoxel(BlockType type, int x, int y, int z, bool posX, bool negX, bool posY, bool negY, bool posZ, bool negZ)
        {
            Vector3 XYZ = new Vector3(x + BLOCK_RENDER_SIZE, y + BLOCK_RENDER_SIZE, z + BLOCK_RENDER_SIZE);
            Vector3 xyz = new Vector3(x - BLOCK_RENDER_SIZE, y - BLOCK_RENDER_SIZE, z - BLOCK_RENDER_SIZE);
            Vector3 xYZ = new Vector3(x - BLOCK_RENDER_SIZE, y + BLOCK_RENDER_SIZE, z + BLOCK_RENDER_SIZE);
            Vector3 XyZ = new Vector3(x + BLOCK_RENDER_SIZE, y - BLOCK_RENDER_SIZE, z + BLOCK_RENDER_SIZE);
            Vector3 XYz = new Vector3(x + BLOCK_RENDER_SIZE, y + BLOCK_RENDER_SIZE, z - BLOCK_RENDER_SIZE);
            Vector3 xyZ = new Vector3(x - BLOCK_RENDER_SIZE, y - BLOCK_RENDER_SIZE, z + BLOCK_RENDER_SIZE);
            Vector3 Xyz = new Vector3(x + BLOCK_RENDER_SIZE, y - BLOCK_RENDER_SIZE, z - BLOCK_RENDER_SIZE);
            Vector3 xYz = new Vector3(x - BLOCK_RENDER_SIZE, y + BLOCK_RENDER_SIZE, z - BLOCK_RENDER_SIZE);


            //123341

            //all clockwise when viewed outside cube
            if (posX)
            {
                //TOP LEFT
                vertexData.Add(XYZ);
                vertexData.Add(XYz);
                vertexData.Add(Xyz);
                //TOP RIGHT
                vertexData.Add(Xyz);
                vertexData.Add(XyZ);
                vertexData.Add(XYZ);
            }
            if (negX)
            {
                vertexData.Add(xyz);
                vertexData.Add(xYz);
                vertexData.Add(xYZ);
                vertexData.Add(xyZ);

                ////1, 2, 3, 3, 4, 1


                //vertexData.Add(xyz);
                //vertexData.Add(xYz);
                //vertexData.Add(xYZ);

                //vertexData.Add(xYZ);
                //vertexData.Add(xyZ);
                //vertexData.Add(xyz);
            }

            if (posY)
            {
                vertexData.Add(xYz);
                vertexData.Add(XYz);
                vertexData.Add(XYZ);

                vertexData.Add(XYZ);
                vertexData.Add(xYZ);
                vertexData.Add(xYz);
            }
            if (negY)
            {
                vertexData.Add(XyZ);
                vertexData.Add(Xyz);
                vertexData.Add(xyz);

                vertexData.Add(xyz);
                vertexData.Add(xyZ);
                vertexData.Add(XyZ);
            }

            if (posZ)
            {
                vertexData.Add(XyZ);
                vertexData.Add(xyZ);
                vertexData.Add(xYZ);

                vertexData.Add(xYZ);
                vertexData.Add(XYZ);
                vertexData.Add(XyZ);
            }
            if (negZ)
            {
                vertexData.Add(xYz);
                vertexData.Add(xyz);
                vertexData.Add(Xyz);

                vertexData.Add(Xyz);
                vertexData.Add(XYz);
                vertexData.Add(xYz);
            }
        }

        public void Draw()
        {
            GL.DrawArrays(BeginMode.Triangles, 0, 1);
        }
    }
}
