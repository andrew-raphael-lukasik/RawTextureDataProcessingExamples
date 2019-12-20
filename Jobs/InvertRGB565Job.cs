using Unity.Mathematics;
using Unity.Collections;

[Unity.Burst.BurstCompile]
public struct InvertRGB565Job : Unity.Jobs.IJobParallelFor
{
	public NativeArray<RGB565> data;
	void Unity.Jobs.IJobParallelFor.Execute ( int i )
	{
		var value = data[i];
		data[i] = new RGB565( Invert(value.R) , Invert(value.G) , Invert(value.B) );
	}
	byte Invert ( byte b ) => (byte)(byte.MaxValue-b);
}
