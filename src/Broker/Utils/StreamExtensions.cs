namespace Broker.Utils;

public static class StreamExtensions
{
    public static async Task<byte[]> ToByteArrayAsync(this Stream stream)
    {
        using var ms = await stream.ToMemoryStreamAsync();
        return ms.ToArray();
    }

    public static async Task<MemoryStream> ToMemoryStreamAsync(this Stream stream)
    {
        if (stream is not MemoryStream memoryStream)
        {
            memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
        }

        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }
}