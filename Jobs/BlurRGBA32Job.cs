using Unity.Mathematics;
using Unity.Collections;

[Unity.Burst.BurstCompile]
public struct BlurRGBA32Job : Unity.Jobs.IJobParallelFor
{
	[DeallocateOnJobCompletion][NativeDisableParallelForRestriction] NativeArray<RGBA32> copy;
	readonly int Last;
	readonly int Width;
	NativeArray<RGBA32> results;
	public BlurRGBA32Job ( NativeArray<RGBA32> data , int texture_width )
	{
		results = data;
		copy = new NativeArray<RGBA32>( data , Allocator.TempJob );
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
		byte A = (byte)( ( px.A + pxr.A + pxl.A + pxt.A + pxb.A ) / kernelSize );
		
		results[i] = new RGBA32{ R=R , G=G , B=B , A=A };
	}
}
