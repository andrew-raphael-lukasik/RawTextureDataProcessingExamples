using Unity.Mathematics;
using Unity.Collections;

[Unity.Burst.BurstCompile]
public struct InvertRGBA32Job : Unity.Jobs.IJobParallelFor
{
	public NativeArray<RGBA32> data;
	void Unity.Jobs.IJobParallelFor.Execute ( int i )
	{
		var color = data[i];
		data[i] = new RGBA32{ R=Invert(color.R) , G=Invert(color.G) , B=Invert(color.B) , A=color.A };
	}
	byte Invert ( byte b ) => (byte)(byte.MaxValue-b);
}
