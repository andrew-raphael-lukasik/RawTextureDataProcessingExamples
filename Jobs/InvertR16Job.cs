using Unity.Mathematics;
using Unity.Collections;

[Unity.Burst.BurstCompile]
public struct InvertR16Job : Unity.Jobs.IJobParallelFor
{
	public NativeArray<ushort> data;
	void Unity.Jobs.IJobParallelFor.Execute ( int i )
	{
		data[i] = (ushort)( ushort.MaxValue - data[i] );
	}
}
