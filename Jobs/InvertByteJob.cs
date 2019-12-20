using Unity.Mathematics;
using Unity.Collections;

[Unity.Burst.BurstCompile]
public struct InvertByteJob : Unity.Jobs.IJobParallelFor
{
	public NativeArray<byte> data;
	void Unity.Jobs.IJobParallelFor.Execute ( int i ) => data[i] = (byte)( byte.MaxValue - data[i] );
}
