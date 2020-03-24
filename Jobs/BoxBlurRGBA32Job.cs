using Unity.Mathematics;
using Unity.Collections;

[Unity.Burst.BurstCompile]
public struct BoxBlurRGBA32Job : Unity.Jobs.IJobParallelFor
{
	[DeallocateOnJobCompletion][NativeDisableParallelForRestriction] NativeArray<RGBA32> copy;
	readonly int Last;
	readonly int Width;
	NativeArray<RGBA32> results;
	public BoxBlurRGBA32Job ( NativeArray<RGBA32> data , int texture_width )
	{
		results = data;
		copy = new NativeArray<RGBA32>( data , Allocator.TempJob );
		Last = results.Length-1;
		Width = texture_width;
	}
	void Unity.Jobs.IJobParallelFor.Execute ( int i )
	{
		int4 upperRow;
		{
			int4 c0 = (int4) copy[ math.clamp( i-Width-1 ,0,Last) ];
			int4 c1 = (int4) copy[ math.clamp( i-Width ,0,Last) ];
			int4 c2 = (int4) copy[ math.clamp( i-Width+1 ,0,Last) ];
			upperRow = c0 + c1 + c2;
		}
		int4 middleRow;
		{
			int4 c0 = (int4) copy[ math.clamp( i-1 ,0,Last) ];
			int4 c1 = (int4) copy[ math.clamp( i ,0,Last) ];
			int4 c2 = (int4) copy[ math.clamp( i+1 ,0,Last) ];
			middleRow = c0 + c1 + c2;
		}
		int4 bottomRow;
		{
			int4 c0 = (int4) copy[ math.clamp( i+Width-1 ,0,Last) ];
			int4 c1 = (int4) copy[ math.clamp( i+Width ,0,Last) ];
			int4 c2 = (int4) copy[ math.clamp( i+Width+1 ,0,Last) ];
			bottomRow = c0 + c1 + c2;
		}
		int4 result = ( upperRow + middleRow + bottomRow ) / 9;
		results[i] = new RGBA32{
			R = (byte) result.x ,
			G = (byte) result.y ,
			B = (byte) result.z ,
			A = (byte) result.w
		};
	}
}
