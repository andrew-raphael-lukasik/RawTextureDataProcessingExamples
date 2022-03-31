# RawTextureData Processing Examples
Documentation gives little helpful information on this topic so I created this repo to document how to create matching data structure + how to read & process it in a IJobParallelFor.

This repository contains these kinds of processing examples:
- Invert color
- Edge detection
- Box Blur
- Gaussian Blur
- Grayscale

Tester window available under MenuItem "Test/Raw Texture Data/Processing Example"

![Tester window](https://i.imgur.com/I0RTpk9.jpg)

Note `GetRawTextureData<T>` will return all the raw texture data including data from the mipmaps should they be enabled. In such case the length of the array will be longer than `width * height`.
`GetPixelData<T>` does not suffer from this issue as you can pass in the desired mip level (usually 0).

---
# Quick lookup table

Valid `<T>` for `GetRawTextureData<T>` & `GetPixelData<T>` or  depending on `texture.format` property:

- `TextureFormat.Alpha8` - `<byte>`
- `TextureFormat.R8` - `<byte>`
- `TextureFormat.R16` - `<ushort>`,`<byte2>`
- `TextureFormat.RHalf` - `<half>`,`<byte2>`
- `TextureFormat.RFloat` - `<float>`,`<byte4>`
- `TextureFormat.RGB24` - `<byte3>`
- `TextureFormat.RGBA32` - `<Color32>`,`<byte4>`
- `TextureFormat.RGBAHalf` - `<half4>`,`<byte2x4>`
- `TextureFormat.RGBAFloat` - `<Color>`,`<Vector4>`,`<float4>`,`<byte4x4>`

Bestiary:
- `public struct byte2 { public byte x, y; }`
- `public struct byte3 { public byte x, y, z; }`
- `public struct byte4 { public byte x, y, z, w; }`
- `public struct byte2x4 { public byte2 c0, c1, c2, c3; }`
- `public struct byte4x4 { public byte4 c0, c1, c2, c3; }`
