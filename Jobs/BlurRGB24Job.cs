using Unity.Mathematics;
using Unity.Collections;

[Unity.Burst.BurstCompile]
public struct BlurRGB24Job : Unity.Jobs.IJobParallelFor
{
	[DeallocateOnJobCompletion][NativeDisableParallelForRestriction] NativeArray<RGB24> copy;
	readonly int Last;
	readonly int Width;
	NativeArray<RGB24> results;
	public BlurRGB24Job ( NativeArray<RGB24> data , int texture_width )
	{
		results = data;
		copy = new NativeArray<RGB24>( data , Allocator.TempJob );
		Last = results.Length-1;
		Width = texture_width;
	}
	void Unity.Jobs.IJobParallelFor.Execute ( int i )
	{
		const int kernelSize = 5;

		var px = copy[i];//center
		var pxr = copy[ math.min( i+1 , Last ) ];//right neighbour
		var pxl = copy[ math.clamp( i-1 , 0 , Last ) ];//left neighbour
		var pxt = copy[ math.clamp( i-Width , 0 , Last ) ];//top neighbour
		var pxb = copy[ math.min( i+Width , Last ) ];//bottom neighbour

		byte R = (byte)( ( px.R + pxr.R + pxl.R + pxt.R + pxb.R ) / kernelSize );
		byte G = (byte)( ( px.G + pxr.G + pxl.G + pxt.G + pxb.G ) / kernelSize );
		byte B = (byte)( ( px.B + pxr.B + pxl.B + pxt.B + pxb.B ) / kernelSize );
		
		results[i] = new RGB24{ R=R , G=G , B=B };
	}
}
