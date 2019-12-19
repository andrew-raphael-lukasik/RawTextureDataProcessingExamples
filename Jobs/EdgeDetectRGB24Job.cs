using Unity.Mathematics;
using Unity.Collections;

[Unity.Burst.BurstCompile]
public struct EdgeDetectRGB24Job : Unity.Jobs.IJobParallelFor
{
	[DeallocateOnJobCompletion][NativeDisableParallelForRestriction] NativeArray<RGB24> copy;
	readonly int Last;
	readonly int Width;
	NativeArray<RGB24> results;
	public EdgeDetectRGB24Job ( NativeArray<RGB24> data , int texture_width )
	{
		results = data;
		copy = new NativeArray<RGB24>( data , Allocator.TempJob );
		Last = results.Length-1;
		Width = texture_width;
	}
	void Unity.Jobs.IJobParallelFor.Execute ( int i )
	{
		var px = copy[i];
		var pr = copy[ math.min( i+1 , Last ) ];
		var pb = copy[ math.min( i+Width , Last ) ];

		byte f = (byte)math.max( math.abs((px.R+px.G+px.B)-(pr.R+pr.G+pr.B)) , math.abs((px.R+px.G+px.B)-(pb.R+pb.G+pb.B)) );
		
		results[i] = new RGB24{ R=f , G=f , B=f };
	}
}
