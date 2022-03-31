using Unity.Collections;
using Unity.Mathematics;

// src: http://blog.ivank.net/fastest-gaussian-blur.html

namespace RawTextureDataProcessingExamples
{
    [Unity.Burst.BurstCompile]
    public struct BoxBlurRGB48Job : Unity.Jobs.IJob
    {
        [DeallocateOnJobCompletion] NativeArray<RGB48> copy;
        readonly int w, h, r;
        NativeArray<RGB48> results;
        public BoxBlurRGB48Job(NativeArray<RGB48> data, int texture_width, int texture_height, int radius)
        {
            this.results = data;
            this.copy = new NativeArray<RGB48>(data, Allocator.TempJob);
            this.w = texture_width;
            this.h = texture_height;
            this.r = radius;
        }
        void Unity.Jobs.IJob.Execute()
        {
            BoxBlurHorizontal(results, copy);
            BoxBlurTotal(copy, results);
        }
        void BoxBlurHorizontal(NativeArray<RGB48> src, NativeArray<RGB48> dst)
        {
            float iarr = 1f / (r + r + 1);
            for (int i = 0; i < h; i++)
            {
                int ti = i * w;
                int li = ti;
                int ri = ti + r;
                float3 fv = (int3)src[ti];
                float3 lv = (int3)src[ti + w - 1];
                float3 val = (r + 1) * fv;
                for (var j = 0; j < r; j++)
                    val += (int3)src[ti + j];
                for (var j = 0; j <= r; j++)
                {
                    val += (int3)src[ri++] - fv;
                    dst[ti++] = (RGB48)(int3)math.round(val * iarr);
                }
                for (var j = r + 1; j < w - r; j++)
                {
                    val += (int3)src[ri++] - (int3)src[li++];
                    dst[ti++] = (RGB48)(int3)math.round(val * iarr);
                }
                for (var j = w - r; j < w; j++)
                {
                    val += lv - (int3)src[li++];
                    dst[ti++] = (RGB48)(int3)math.round(val * iarr);
                }
            }
        }
        void BoxBlurTotal(NativeArray<RGB48> src, NativeArray<RGB48> dst)
        {
            float3 iarr = 1f / (r + r + 1);
            for (int i = 0; i < w; i++)
            {
                int ti = i;
                int li = ti;
                int ri = ti + r * w;
                float3 fv = (int3)src[ti];
                float3 lv = (int3)src[ti + w * (h - 1)];
                float3 val = (r + 1) * fv;
                for (var j = 0; j < r; j++)
                    val += (int3)src[ti + j * w];
                for (var j = 0; j <= r; j++)
                {
                    val += (int3)src[ri] - fv;
                    dst[ti] = (RGB48)(int3)math.round(val * iarr);
                    ri += w;
                    ti += w;
                }
                for (var j = r + 1; j < h - r; j++)
                {
                    val += (int3)src[ri] - (int3)src[li];
                    dst[ti] = (RGB48)(int3)math.round(val * iarr);
                    li += w;
                    ri += w;
                    ti += w;
                }
                for (var j = h - r; j < h; j++)
                {
                    val += lv - (int3)src[li];
                    dst[ti] = (RGB48)(int3)math.round(val * iarr);
                    li += w;
                    ti += w;
                }
            }
        }
    }
}