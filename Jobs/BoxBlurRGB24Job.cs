using Unity.Mathematics;
using Unity.Collections;

[Unity.Burst.BurstCompile]
public struct BoxBlurRGB24Job : Unity.Jobs.IJobParallelFor
{
	[DeallocateOnJobCompletion][NativeDisableParallelForRestriction] NativeArray<RGB24> copy;
	readonly int Last;
	readonly int Width;
	NativeArray<RGB24> results;
	public BoxBlurRGB24Job ( NativeArray<RGB24> data , int texture_width )
	{
		results = data;
		copy = new NativeArray<RGB24>( data , Allocator.TempJob );
		Last = results.Length-1;
		Width = texture_width;
	}
	void Unity.Jobs.IJobParallelFor.Execute ( int i )
	{
		int3 upperRow;
		{
			int3 c0 = (int3) copy[ math.clamp( i-Width-1 ,0,Last) ];
			int3 c1 = (int3) copy[ math.clamp( i-Width ,0,Last) ];
			int3 c2 = (int3) copy[ math.clamp( i-Width+1 ,0,Last) ];
			upperRow = c0 + c1 + c2;
		}
		int3 middleRow;
		{
			int3 c0 = (int3) copy[ math.clamp( i-1 ,0,Last) ];
			int3 c1 = (int3) copy[ math.clamp( i ,0,Last) ];
			int3 c2 = (int3) copy[ math.clamp( i+1 ,0,Last) ];
			middleRow = c0 + c1 + c2;
		}
		int3 bottomRow;
		{
			int3 c0 = (int3) copy[ math.clamp( i+Width-1 ,0,Last) ];
			int3 c1 = (int3) copy[ math.clamp( i+Width ,0,Last) ];
			int3 c2 = (int3) copy[ math.clamp( i+Width+1 ,0,Last) ];
			bottomRow = c0 + c1 + c2;
		}
		int3 result = ( upperRow + middleRow + bottomRow ) / 9;
		results[i] = new RGB24{
			R = (byte) result.x ,
			G = (byte) result.y ,
			B = (byte) result.z
		};
	}
}
