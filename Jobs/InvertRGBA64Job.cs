using Unity.Collections;

namespace RawTextureDataProcessingExamples
{
    [Unity.Burst.BurstCompile]
    public struct InvertRGBA64Job : Unity.Jobs.IJobParallelFor
    {
        public NativeArray<RGBA64> data;
        void Unity.Jobs.IJobParallelFor.Execute(int i)
        {
            var color = data[i];
            data[i] = new RGBA64 { R = Invert(color.R), G = Invert(color.G), B = Invert(color.B), A = color.A };
        }
        ushort Invert(ushort u) => (ushort)(ushort.MaxValue - u);
    }
}
