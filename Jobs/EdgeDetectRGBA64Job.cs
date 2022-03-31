using Unity.Collections;
using Unity.Mathematics;

namespace RawTextureDataProcessingExamples
{
    [Unity.Burst.BurstCompile]
    public struct EdgeDetectRGBA64Job : Unity.Jobs.IJobParallelFor
    {
        [DeallocateOnJobCompletion] [NativeDisableParallelForRestriction] NativeArray<RGBA64> copy;
        readonly int Last;
        readonly int Width;
        [WriteOnly] NativeArray<RGBA64> results;
        public EdgeDetectRGBA64Job(NativeArray<RGBA64> data, int texture_width)
        {
            results = data;
            copy = new NativeArray<RGBA64>(data, Allocator.TempJob);
            Last = results.Length - 1;
            Width = texture_width;
        }
        void Unity.Jobs.IJobParallelFor.Execute(int i)
        {
            var px = copy[i];
            var pr = copy[(i + 1) % Width == 0 ? i : i + 1];
            var pb = copy[i + Width > Last ? i : i + Width];

            ushort f = (ushort)math.max(math.abs((px.R + px.G + px.B) - (pr.R + pr.G + pr.B)), math.abs((px.R + px.G + px.B) - (pb.R + pb.G + pb.B)));

            results[i] = new RGBA64 { R = f, G = f, B = f, A = px.A };
        }
    }
}
