using Unity.Mathematics;

namespace RawTextureDataProcessingExamples
{
    public struct RG32
    {
        public ushort R, G;

        public static explicit operator int2(RG32 val) => new int2 { x = val.R, y = val.G };
        public static explicit operator RG32(int2 val) => new RG32 { R = (ushort)val.x, G = (ushort)val.y };

        public static RG32 operator +(RG32 lhs, RG32 rhs) => (RG32)((int2)lhs + (int2)rhs);
        public static RG32 operator -(RG32 lhs, RG32 rhs) => (RG32)((int2)lhs - (int2)rhs);
    }
}
