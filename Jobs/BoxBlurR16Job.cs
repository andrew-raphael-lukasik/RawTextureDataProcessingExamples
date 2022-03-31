using Unity.Collections;
using Unity.Mathematics;

// src: http://blog.ivank.net/fastest-gaussian-blur.html

namespace RawTextureDataProcessingExamples
{
    [Unity.Burst.BurstCompile]
    public struct BoxBlurR16Job : Unity.Jobs.IJob
    {
        [DeallocateOnJobCompletion] NativeArray<ushort> copy;
        readonly int w, h, r;
        NativeArray<ushort> results;
        public BoxBlurR16Job(NativeArray<ushort> data, int texture_width, int texture_height, int radius)
        {
            this.results = data;
            this.copy = new NativeArray<ushort>(data, Allocator.TempJob);
            this.w = texture_width;
            this.h = texture_height;
            this.r = radius;
        }
        void Unity.Jobs.IJob.Execute()
        {
            BoxBlurHorizontal(results, copy);
            BoxBlurTotal(copy, results);
        }
        void BoxBlurHorizontal(NativeArray<ushort> src, NativeArray<ushort> dst)
        {
            float iarr = 1f / (r + r + 1);
            for (int i = 0; i < h; i++)
            {
                int ti = i * w;
                int li = ti;
                int ri = ti + r;
                float fv = src[ti];
                float lv = src[ti + w - 1];
                float val = (r + 1) * fv;
                for (var j = 0; j < r; j++)
                    val += src[ti + j];
                for (var j = 0; j <= r; j++)
                {
                    val += src[ri++] - fv;
                    dst[ti++] = (ushort)math.round(val * iarr);
                }
                for (var j = r + 1; j < w - r; j++)
                {
                    val += src[ri++] - src[li++];
                    dst[ti++] = (ushort)math.round(val * iarr);
                }
                for (var j = w - r; j < w; j++)
                {
                    val += lv - src[li++];
                    dst[ti++] = (ushort)math.round(val * iarr);
                }
            }
        }
        void BoxBlurTotal(NativeArray<ushort> src, NativeArray<ushort> dst)
        {
            float iarr = 1f / (r + r + 1);
            for (int i = 0; i < w; i++)
            {
                int ti = i;
                int li = ti;
                int ri = ti + r * w;
                float fv = src[ti];
                float lv = src[ti + w * (h - 1)];
                float val = (r + 1) * fv;
                for (var j = 0; j < r; j++)
                    val += src[ti + j * w];
                for (var j = 0; j <= r; j++)
                {
                    val += src[ri] - fv;
                    dst[ti] = (ushort)math.round(val * iarr);
                    ri += w;
                    ti += w;
                }
                for (var j = r + 1; j < h - r; j++)
                {
                    val += src[ri] - src[li];
                    dst[ti] = (ushort)math.round(val * iarr);
                    li += w;
                    ri += w;
                    ti += w;
                }
                for (var j = h - r; j < h; j++)
                {
                    val += lv - src[li];
                    dst[ti] = (ushort)math.round(val * iarr);
                    li += w;
                    ti += w;
                }
            }
        }
    }
}
