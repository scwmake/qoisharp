using System;
using System.IO;
using System.Text;

namespace QoiSharp
{
    class BytesReader : IBytesReader
    {
        private readonly BinaryReader _reader;
        private readonly Stream _baseStream;

        public bool IsEOF
        {
            get { return _reader.BaseStream.Position == _reader.BaseStream.Length; }
        }

        public long Position { get { return _reader.BaseStream.Position; } }
        public long Length { get { return _reader.BaseStream.Length; } }

        public BytesReader(Stream stream)
        {
            _reader = new BinaryReader(stream);
            _baseStream = stream;
        }

        public BytesReader(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            _reader = new BinaryReader(stream);
            _baseStream = stream;
        }

        // Little endian //

        public virtual int ReadInt32()
        {
            return _reader.ReadInt32();
        }
        public virtual uint ReadUInt32()
        {
            return _reader.ReadUInt32();
        }

        // Big endian //

        public virtual int ReadInt32BE()
        {
            var temp = _reader.ReadBytes(4);
            Array.Reverse(temp);
            return BitConverter.ToInt32(temp);
        }

        public virtual uint ReadUInt32BE()
        {
            var temp = _reader.ReadBytes(4);
            Array.Reverse(temp);
            return BitConverter.ToUInt32(temp);
        }

        // Other
        public virtual sbyte ReadChar()
        {
            return _reader.ReadSByte();
        }
        public virtual byte ReadUChar()
        {
            return _reader.ReadByte();
        }

        public virtual byte[] ReadBytes(int count)
        {
            return _reader.ReadBytes(count);
        }

        public string ReadCString(int count)
        {
            return Encoding.UTF8.GetString(ReadBytes(count));
        }
    }
}
