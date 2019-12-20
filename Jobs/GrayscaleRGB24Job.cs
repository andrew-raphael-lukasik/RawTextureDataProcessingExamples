using Unity.Mathematics;
using Unity.Collections;

[Unity.Burst.BurstCompile]
public struct GrayscaleRGB24Job : Unity.Jobs.IJobParallelFor
{
	public NativeArray<RGB24> data;
	void Unity.Jobs.IJobParallelFor.Execute ( int i )
	{
		var color = data[i];
		float product = math.mul( new float3{ x=color.R , y=color.G , z=color.B } , new float3{ x=0.3f , y=0.59f , z=0.11f } );
		byte b = (byte)product;
		data[i] = new RGB24{ R=b , G=b , B=b };
	}
}
