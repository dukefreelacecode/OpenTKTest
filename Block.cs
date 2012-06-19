using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTKTest
{
    public class Block
    {
        public Vector3i Location;
        public BlockType Type;
        public Block(Vector3i location, BlockType type)
        {
            Type = type;
            Location = location;
        }

        public override string ToString()
        {
            return String.Format("Location: {0}, Type: {1}", Location, Type);
        }
    }
}
