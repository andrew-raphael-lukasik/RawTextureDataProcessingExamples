using Unity.Collections;

namespace RawTextureDataProcessingExamples
{
    [Unity.Burst.BurstCompile]
    public struct InvertRG32Job : Unity.Jobs.IJobParallelFor
    {
        public NativeArray<RG32> data;
        void Unity.Jobs.IJobParallelFor.Execute(int i)
        {
            var color = data[i];
            data[i] = new RG32 { R = Invert(color.R), G = Invert(color.G) };
        }
        ushort Invert(ushort u) => (ushort)(ushort.MaxValue - u);
    }
}
