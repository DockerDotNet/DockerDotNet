using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace DockerDotNet.Shared
{
    public class WebSocketStream : Stream
    {
        private readonly WebSocket _socket;
        private readonly byte[] _buffer = new byte[8192];
        private int _bufferOffset = 0;
        private int _bufferCount = 0;

        public WebSocketStream(WebSocket socket) => _socket = socket;

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (_bufferCount == 0)
            {
                var result = await _socket.ReceiveAsync(new ArraySegment<byte>(_buffer), cancellationToken);
                if (result.MessageType == WebSocketMessageType.Close || result.Count == 0)
                    return 0;

                _bufferOffset = 0;
                _bufferCount = result.Count;
            }

            int bytesToCopy = Math.Min(count, _bufferCount);
            Array.Copy(_buffer, _bufferOffset, buffer, offset, bytesToCopy);
            _bufferOffset += bytesToCopy;
            _bufferCount -= bytesToCopy;

            return bytesToCopy;
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await _socket.SendAsync(new ArraySegment<byte>(buffer, offset, count), WebSocketMessageType.Binary, true, cancellationToken);
        }


        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return ReadAsync(buffer, offset, count).GetAwaiter().GetResult();
        }


        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            WriteAsync(buffer, offset, count).GetAwaiter().GetResult();
        }

    }
}
