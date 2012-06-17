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
        public static readonly float BLOCK_RENDER_SIZE = 0.5f;
        public static readonly int CHUNK_SIZE_1D = 16;
        public static readonly int CHUNK_SIZE_3D;
        BlockType[,,] blocks = new BlockType[CHUNK_SIZE_1D, CHUNK_SIZE_1D, CHUNK_SIZE_1D];

        static Chunk()
        {
            CHUNK_SIZE_3D = (int)Math.Pow(CHUNK_SIZE_1D, 3);
        }

        public Chunk()
        {
        }
        
        short[] indices;

        int indicesHandle = 0;
        int verticesHandle = 0;

        List<Vector3> vertexData = new List<Vector3>();
        public void Generate()
        {
            GL.GenBuffers(1, out verticesHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, verticesHandle);

            for (int x = 0; x < CHUNK_SIZE_1D; x++)
                for (int y = 0; y < CHUNK_SIZE_1D; y++)
                    for (int z = 0; z < CHUNK_SIZE_1D; z++)
                        blocks[x, y, z] = BlockType.STONE;

            for (int x = 0; x < CHUNK_SIZE_1D; x++)
            {
                for (int y = 0; y < CHUNK_SIZE_1D; y++)
                {
                    for (int z = 0; z < CHUNK_SIZE_1D; z++)
                    {
                        BlockType blockType = blocks[x, y, z];
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

            GL.BufferData(BufferTarget.ArrayBuffer,
                (IntPtr)((vertexData.Count) * Vector3.SizeInBytes),
                vertexData.ToArray(), BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out indicesHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indicesHandle);
            indices = new short[(vertexData.Count / 4) * 6];
            for (uint i = 0, j = 0; i < indices.Length; i += 6, j += 4)
            {
                indices[i]     = (short)j;
                indices[i + 1] = (short)(j + 1);
                indices[i + 2] = (short)(j + 2);

                indices[i + 3] = (short)(j + 2);
                indices[i + 4] = (short)(j + 3);
                indices[i + 5] = (short)j;
            }
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(short)), indices, BufferUsageHint.StaticDraw);
        }

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
            
            //123 341
            //012 230
            if (posX)
            {
                vertexData.Add(XYZ);
                vertexData.Add(XYz);
                vertexData.Add(Xyz);
                vertexData.Add(XyZ);
            }
            if (negX)
            {
                vertexData.Add(xyz);
                vertexData.Add(xYz);
                vertexData.Add(xYZ);
                vertexData.Add(xyZ);
            }

            if (posY)
            {
                vertexData.Add(xYz);
                vertexData.Add(XYz);
                vertexData.Add(XYZ);
                vertexData.Add(xYZ);
            }
            if (negY)
            {
                vertexData.Add(XyZ);
                vertexData.Add(Xyz);
                vertexData.Add(xyz);
                vertexData.Add(xyZ);
            }

            if (posZ)
            {
                vertexData.Add(XyZ);
                vertexData.Add(xyZ);
                vertexData.Add(xYZ);
                vertexData.Add(XYZ);
            }
            if (negZ)
            {
                vertexData.Add(xYz);
                vertexData.Add(xyz);
                vertexData.Add(Xyz);
                vertexData.Add(XYz);
            }
        }

        public void Draw()
        {
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, verticesHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indicesHandle);

            GL.VertexPointer(3, VertexPointerType.Float, BlittableValueType.StrideOf(vertexData.ToArray()), IntPtr.Zero);
            GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedShort, IntPtr.Zero);
        }
    }
}
