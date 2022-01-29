# Graphics Libraries comparison
This comparison is specifically targeted find the best library to be used in the IoT context in .NET. The main driver for this is `System.Drawing` being [deprecated](https://docs.microsoft.com/en-us/dotnet/api/system.drawing?view=dotnet-plat-ext-5.0#remarks).

The main requisites are the following:

1. Working on both Linux and Windows on the ARM platform

2. A license compatible with the [dotnet/iot](https://github.com/dotnet/iot/) libraries

I started the investigation from this blog post dated 2017: https://devblogs.microsoft.com/dotnet/net-core-image-processing/

Excluded libraries:

- The [Magick](https://github.com/dlemstra/Magick.NET) library [does not work on linux/arm](https://github.com/dlemstra/Magick.NET/issues/839) 
- The [CoreCompact](https://github.com/orgs/CoreCompat) library disappeared from the repositories list
- The [MagicScaler](https://github.com/saucecontrol/PhotoSauce) library [does not work on linux](https://github.com/saucecontrol/PhotoSauce/blob/master/readme.md)

This restricts the list to [SkiaSharp](https://github.com/mono/SkiaSharp) and [ImageSharp](https://github.com/SixLabors/ImageSharp). The comparison includes `System.Drawing.Common` to evaluate the differences with the current dotnet/iot libraries.

