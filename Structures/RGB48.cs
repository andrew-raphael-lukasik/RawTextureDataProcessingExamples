using Unity.Mathematics;

namespace RawTextureDataProcessingExamples
{
    public struct RGB48
    {
        public ushort R, G, B;

        public static explicit operator int3(RGB48 val) => new int3 { x = val.R, y = val.G, z = val.B };
        public static explicit operator RGB48(int3 val) => new RGB48 { R = (ushort)val.x, G = (ushort)val.y, B = (ushort)val.z };

        public static RGB48 operator +(RGB48 lhs, RGB48 rhs) => (RGB48)((int3)lhs + (int3)rhs);
        public static RGB48 operator -(RGB48 lhs, RGB48 rhs) => (RGB48)((int3)lhs - (int3)rhs);
    }
}
