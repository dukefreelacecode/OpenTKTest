using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTKTest
{
    public static class BlockTypeExtension
    {
        /// <summary>
        /// Gets the type as an integer ID. Alias to casting to int.
        /// </summary>
        /// <param name="type">Any block type</param>
        /// <returns>That type's ID</returns>
        public static int GetID(this BlockType type)
        {
            return (int)type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">Any block type</param>
        /// <returns>True if the block is opaque and fills an entire voxel space</returns>
        public static bool IsSolid(this BlockType type)
        {
            if (type == BlockType.AIR)
            {
                return false;
            }
            else
                return true;
        }
    }

    public enum BlockType
    {
        AIR     = 0,
        STONE   = 1, 
        DIRT    = 2, 
        GRASS   = 3
    };
}
