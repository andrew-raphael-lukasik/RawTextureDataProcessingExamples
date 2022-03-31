using Unity.Collections;
using Unity.Mathematics;

namespace RawTextureDataProcessingExamples
{
    [Unity.Burst.BurstCompile]
    public struct EdgeDetectRGB24Job : Unity.Jobs.IJobParallelFor
    {
        [DeallocateOnJobCompletion][NativeDisableParallelForRestriction] NativeArray<RGB24> copy;
        readonly int Last;
        readonly int Width;
        [WriteOnly] NativeArray<RGB24> results;
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
            var pr = copy[(i + 1) % Width == 0 ? i : i + 1];
            var pb = copy[i + Width > Last ? i : i + Width];

            byte f = (byte)math.max( math.abs((px.R+px.G+px.B)-(pr.R+pr.G+pr.B)) , math.abs((px.R+px.G+px.B)-(pb.R+pb.G+pb.B)) );
		
            results[i] = new RGB24{ R=f , G=f , B=f };
        }
    }
}
