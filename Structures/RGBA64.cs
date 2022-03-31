using Unity.Mathematics;

namespace RawTextureDataProcessingExamples
{
    public struct RGBA64
    {
        public ushort R, G, B, A;

        public static explicit operator int4(RGBA64 val) => new int4 { x = val.R, y = val.G, z = val.B, w = val.A };
        public static explicit operator RGBA64(int4 val) => new RGBA64 { R = (ushort)val.x, G = (ushort)val.y, B = (ushort)val.z, A = (ushort)val.w };

        public static RGBA64 operator +(RGBA64 lhs, RGBA64 rhs) => (RGBA64)((int4)lhs + (int4)rhs);
        public static RGBA64 operator -(RGBA64 lhs, RGBA64 rhs) => (RGBA64)((int4)lhs - (int4)rhs);
    }
}
