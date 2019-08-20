# RawTextureData Processing Examples
Documentation gives little helpful information on this topic so I created this repo to document how to create matching data structure + how to read & process it in a IJobParallelFor.

I choose invert color operation due to it's simplicity and self-testing property (invalid invert won't reproduce original colors).

NOTE: You can process NativeArray in some different type of job or even completely outside any. It's simply fastest this way.
