# RawTextureData Processing Examples
Documentation gives little helpful information on this topic so I created this repo to document how to create matching data structure + how to read & process it in a IJobParallelFor.

This repository contains 3 example job types:
- Invert color
- Edge detection
- Blur
- Grayscale

Tester window available under MenuItem "Test/Raw Texture Data/Processing Example"

![Tester window](https://i.imgur.com/B8LlAQu.jpg)

NOTE: You can process NativeArray in some different type of IJob or even completely outside any. It's simply fastest this way.
