public struct RGB24
{
	public byte R, G, B;
	
	public static explicit operator Unity.Mathematics.int3 ( RGB24 val ) => new Unity.Mathematics.int3{ x=val.R , y=val.G , z=val.B };

}
