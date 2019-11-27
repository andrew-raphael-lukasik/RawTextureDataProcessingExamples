using Unity.Mathematics;
using Unity.Collections;

[Unity.Burst.BurstCompile]
public struct EdgeDetectRGB24Job : Unity.Jobs.IJobParallelFor
{
	[DeallocateOnJobCompletion][NativeDisableParallelForRestriction] NativeArray<RGB24> copy;
	readonly int Length;
	NativeArray<RGB24> results;
	public EdgeDetectRGB24Job ( NativeArray<RGB24> data )
	{
		results = data;
		copy = new NativeArray<RGB24>( data , Allocator.TempJob );
		Length = results.Length-1;
	}
    void Unity.Jobs.IJobParallelFor.Execute ( int i )
    {
        var a = results[i];
		var b = results[ math.min( i+1 , Length ) ];
        results[i] =
			math.abs((a.R+a.G+a.B)-(b.R+b.G+b.B))<32
			? new RGB24{ R=255 , G=255 , B=255 }
			: default(RGB24);
    }
}
