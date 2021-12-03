namespace QoiSharp
{
    public interface IBytesReader
    {
        bool IsEOF { get; }
        long Position { get; }
        long Length { get; }

        // Little endian //

        int ReadInt32();
        uint ReadUInt32();

        // Big endian //

        int ReadInt32BE();
        uint ReadUInt32BE();

        // Other

        sbyte ReadChar();
        byte ReadUChar();
        byte[] ReadBytes(int count);
        string ReadCString(int count);
    }
}
