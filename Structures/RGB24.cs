using Unity.Mathematics;

public struct RGB24
{
	
	public byte R, G, B;
	
	public static explicit operator int3 ( RGB24 val ) => new int3{ x=val.R , y=val.G , z=val.B };
	public static explicit operator RGB24 ( int3 val ) => new RGB24{ R=(byte)val.x , G=(byte)val.y , B=(byte)val.z };

	public static RGB24 operator + ( RGB24 lhs , RGB24 rhs ) => (RGB24)( (int3)lhs + (int3)rhs );
	public static RGB24 operator - ( RGB24 lhs , RGB24 rhs ) => (RGB24)( (int3)lhs - (int3)rhs );

}
