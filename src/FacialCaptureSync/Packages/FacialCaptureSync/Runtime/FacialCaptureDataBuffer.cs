namespace FacialCaptureSync
{
    public sealed class FacialCaptureDataBuffer
    {
        private readonly FacialCapture[] _captureBuffer;
        private readonly int _bufferSize;
        private readonly long _bufferMask;

        private long _bufferHead = 0;
        private long _bufferTail = 0;

        public FacialCaptureDataBuffer(int bufferSize = 2)
        {
            _bufferSize = MathUtils.CeilingPowerOfTwo(bufferSize); // Buffer size must be a power of two.
            _bufferMask = _bufferSize - 1;

            _captureBuffer = new FacialCapture[_bufferSize];
            for (var i = 0; i < _captureBuffer.Length; i++)
            {
                _captureBuffer[i] = new FacialCapture();
            }
        }

        public void Enqueue(FacialCapture capture)
        {
            var index = _bufferTail & _bufferMask;
            capture.CopyTo(_captureBuffer[index]);

            // Update the enqueue position to insert the next data.
            _bufferTail++;

            // When the buffer overflows, the head data (oldest data) is deleted.
            var bufferFreeCount = _bufferSize - (int)(_bufferTail - _bufferHead);
            if (bufferFreeCount <= 0)
            {
                _bufferHead++;
            }
        }

        public bool TryPeek(ref FacialCapture capture)
        {
            if (_bufferTail - _bufferHead <= 0) return false;

            var index = _bufferHead & _bufferMask;
            _captureBuffer[index].CopyTo(capture);

            return true;
        }

        public bool TryDequeue(ref FacialCapture capture)
        {
            if (_bufferTail - _bufferHead <= 0) return false;

            var index = _bufferHead & _bufferMask;
            _captureBuffer[index].CopyTo(capture);
            _bufferHead++;

            return true;
        }
    }
}
