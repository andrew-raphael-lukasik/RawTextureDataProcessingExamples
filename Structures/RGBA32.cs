public struct RGBA32
{

	public byte R, G, B, A;

	public static explicit operator Unity.Mathematics.int4 ( RGBA32 val ) => new Unity.Mathematics.int4{ x=val.R , y=val.G , z=val.B , w=val.A };

}
