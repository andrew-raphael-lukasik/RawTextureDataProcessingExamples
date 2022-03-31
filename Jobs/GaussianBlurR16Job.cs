using Unity.Collections;
using Unity.Mathematics;

// src: http://blog.ivank.net/fastest-gaussian-blur.html

namespace RawTextureDataProcessingExamples
{
    [Unity.Burst.BurstCompile]
    public struct GaussianBlurR16Job : Unity.Jobs.IJob
    {
        [DeallocateOnJobCompletion] NativeArray<ushort> copy;
        NativeArray<ushort> results;
        readonly int w, h;
        readonly float sigma;
        [DeallocateOnJobCompletion] NativeArray<float> boxes;
        public GaussianBlurR16Job(NativeArray<ushort> data, int texture_width, int texture_height, float sigma)
        {
            this.results = data;
            this.copy = new NativeArray<ushort>(data, Allocator.TempJob);
            this.w = texture_width;
            this.h = texture_height;
            this.boxes = new NativeArray<float>(3, Allocator.TempJob);
            this.sigma = sigma;
        }
        void Unity.Jobs.IJob.Execute()
        {
            BoxesForGauss(sigma, boxes);
            BoxBlur(copy, results, (int)((boxes[0] - 1f) / 2f));
            BoxBlur(results, copy, (int)((boxes[1] - 1f) / 2f));
            BoxBlur(copy, results, (int)((boxes[2] - 1f) / 2f));
        }

        void BoxBlur(NativeArray<ushort> src, NativeArray<ushort> dst, int r)
        {
            BoxBlurHorizontal(dst, src, r);
            BoxBlurTotal(src, dst, r);
        }
        void BoxBlurHorizontal(NativeArray<ushort> src, NativeArray<ushort> dst, int r)
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
        void BoxBlurTotal(NativeArray<ushort> src, NativeArray<ushort> dst, int r)
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

        void BoxesForGauss(float sigma, NativeArray<float> boxes)
        {
            int n = boxes.Length;
            var wIdeal = math.sqrt((12 * sigma * sigma / n) + 1);
            var wl = math.floor(wIdeal); if (wl % 2 == 0) wl--;
            var wu = wl + 2;

            var mIdeal = (12 * sigma * sigma - n * wl * wl - 4 * n * wl - 3 * n) / (-4 * wl - 4);
            var m = math.round(mIdeal);

            for (int i = 0; i < n; i++)
                boxes[i] = i < m ? wl : wu;
        }
    }
}
