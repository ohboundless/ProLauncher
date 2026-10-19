using OffiUtils;
using UAssetEditor.Unreal.Readers;
using ZstdSharp;

namespace UAssetEditor.Compression;

public static class CompressionHandler
{
    public static byte[] HandleDecompression(UnrealFileReader reader, int compressionIndex, byte[] data, int uncompressedSize)
    {
        var compressionMethod = reader.CompressionMethods[compressionIndex];

        return compressionMethod switch
        {
            "None" => data,
            "Oodle" => Oodle.Decompress(data, uncompressedSize),
            _ => throw new NotImplementedException($"'{compressionMethod}' is not implemented!")
        };
    }

    public static IDecompressor BuildZstdDecompressor()
    {
        return DecompressorBuilder.Default
            .Add(CompressionAlgorithm.Zstd, static (source, destination, out written) =>
                {
                    using var decompressor = new Decompressor();
                    return decompressor.TryUnwrap(source, destination, out written);
                }, replace: true)
                .Build();
    }
}