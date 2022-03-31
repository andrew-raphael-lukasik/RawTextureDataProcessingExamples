using Unity.Collections;
using Unity.Mathematics;

namespace RawTextureDataProcessingExamples
{
    [Unity.Burst.BurstCompile]
    public struct GrayscaleRGB48Job : Unity.Jobs.IJobParallelFor
    {
        public NativeArray<RGB48> data;
        void Unity.Jobs.IJobParallelFor.Execute(int i)
        {
            var color = data[i];
            float product = math.mul(new float3 { x = color.R, y = color.G, z = color.B }, new float3 { x = 0.3f, y = 0.59f, z = 0.11f });
            ushort u = (ushort)product;
            data[i] = new RGB48 { R = u, G = u, B = u };
        }
    }
}
