using Unity.Mathematics;
using Unity.Collections;

[Unity.Burst.BurstCompile]
public struct InvertRGB24Job : Unity.Jobs.IJobParallelFor
{
	public NativeArray<RGB24> data;
	void Unity.Jobs.IJobParallelFor.Execute ( int i )
	{
		var color = data[i];
		data[i] = new RGB24{ R=Invert(color.R) , G=Invert(color.G) , B=Invert(color.B) };
	}
	byte Invert ( byte b ) => (byte)(byte.MaxValue-b);
}
