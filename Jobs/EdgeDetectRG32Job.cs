using Unity.Collections;
using Unity.Mathematics;

namespace RawTextureDataProcessingExamples
{
    [Unity.Burst.BurstCompile]
    public struct EdgeDetectRG32Job : Unity.Jobs.IJobParallelFor
    {
        [DeallocateOnJobCompletion] [NativeDisableParallelForRestriction] NativeArray<RG32> copy;
        readonly int Last;
        readonly int Width;
        [WriteOnly] NativeArray<RG32> results;
        public EdgeDetectRG32Job(NativeArray<RG32> data, int texture_width)
        {
            results = data;
            copy = new NativeArray<RG32>(data, Allocator.TempJob);
            Last = results.Length - 1;
            Width = texture_width;
        }
        void Unity.Jobs.IJobParallelFor.Execute(int i)
        {
            var px = copy[i];
            var pr = copy[(i + 1) % Width == 0 ? i : i + 1];
            var pb = copy[i + Width > Last ? i : i + Width];

            ushort f = (ushort)math.max(math.abs((px.R + px.G) - (pr.R + pr.G)), math.abs((px.R + px.G) - (pb.R + pb.G)));

            results[i] = new RG32 { R = f, G = f };
        }
    }
}
