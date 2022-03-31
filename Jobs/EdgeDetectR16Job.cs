using Unity.Collections;
using Unity.Mathematics;

namespace RawTextureDataProcessingExamples
{
    [Unity.Burst.BurstCompile]
    public struct EdgeDetectR16Job : Unity.Jobs.IJobParallelFor
    {
        [DeallocateOnJobCompletion] [NativeDisableParallelForRestriction] NativeArray<ushort> copy;
        readonly int Last;
        readonly int Width;
        [WriteOnly] NativeArray<ushort> results;
        public EdgeDetectR16Job(NativeArray<ushort> data, int texture_width)
        {
            results = data;
            copy = new NativeArray<ushort>(data, Allocator.TempJob);
            Last = results.Length - 1;
            Width = texture_width;
        }
        void Unity.Jobs.IJobParallelFor.Execute(int i)
        {
            var px = copy[i];
            var pr = copy[(i + 1) % Width == 0 ? i : i + 1];
            var pb = copy[i + Width > Last ? i : i + Width];

            ushort f = (ushort)math.max(math.abs(px - pr), math.abs(px - pb));

            results[i] = f;
        }
    }
}
