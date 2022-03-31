using Unity.Collections;

namespace RawTextureDataProcessingExamples
{
    [Unity.Burst.BurstCompile]
    public struct InvertRGB48Job : Unity.Jobs.IJobParallelFor
    {
        public NativeArray<RGB48> data;
        void Unity.Jobs.IJobParallelFor.Execute(int i)
        {
            var color = data[i];
            data[i] = new RGB48 { R = Invert(color.R), G = Invert(color.G), B = Invert(color.B) };
        }
        ushort Invert(ushort u) => (ushort)(ushort.MaxValue - u);
    }
}
