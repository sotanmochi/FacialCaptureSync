#if UNITY_2020_3_OR_NEWER
#define UNITY_ENGINE
#endif

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace FacialCaptureSync
{
    /// <summary>
    /// An implemention of facial capture data receiver for iFacialMocap and Facemotion3d.<br/>
    /// https://www.ifacialmocap.com/<br/>
    /// https://www.facemotion3d.info/<br/>
    /// </summary>
    public sealed class FacialCaptureReceiver : IDisposable
    {
        private readonly IFacialCaptureSource _captureSource;

        private FacialCapture _receivedDataBuffer = new();

        private bool _isRunning = false;
        private CancellationTokenSource _cancellationTokenSource;
        private UdpClient _udpClient;
        private IPAddress _captureDeviceAddress;

        public event Action<FacialCapture> OnDataReceived;

        public int Port => _captureSource.ReceiverPort;
        public bool IsRunning => _isRunning;

        public FacialCaptureReceiver(IFacialCaptureSource captureDataSource, int bufferSize = 4)
        {
            _captureSource = captureDataSource;
        }
        
        public void Dispose()
        {
            Disconnect();
            Stop();
        }

        public void Start()
        {
            if (_isRunning) { return; }
            
            _isRunning = true;

            _cancellationTokenSource = new CancellationTokenSource();
            RunReceiverLoop(_cancellationTokenSource.Token);
        }
        
        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            
            _udpClient?.Close();
            _udpClient?.Dispose();
            _udpClient = null;
            
            _isRunning = false;
        }
        
        public void ConnectToCaptureSource(string address)
        {
            if (!_isRunning) { Start(); }
            
            if (_captureSource.UseIndirectConnection) { return; }
            
            if (IPAddress.TryParse(address, out _captureDeviceAddress))
            {
                var data = System.Text.Encoding.UTF8.GetBytes(_captureSource.HandshakeMessage);
                _udpClient.Send(data, data.Length, new IPEndPoint(_captureDeviceAddress, _captureSource.HandshakePort));
                DebugLog($"ConnectToCaptureDevice | {_captureDeviceAddress}:{_captureSource.HandshakePort} |");
            }
            else
            {
                LogError($"Cannot parse the address: {address}");
            }
        }
        
        public void Disconnect()
        {
            if (_captureSource.UseIndirectConnection) { return; }
            var data = System.Text.Encoding.UTF8.GetBytes(_captureSource.StopStreamingMessage);
            _udpClient.Send(data, data.Length, new IPEndPoint(_captureDeviceAddress, _captureSource.HandshakePort));
        }
        
        private async void RunReceiverLoop(CancellationToken cancellationToken = default)
        {
            _udpClient = new UdpClient(_captureSource.ReceiverPort);
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await _udpClient.ReceiveAsync().ConfigureAwait(false);
                OnReceived(result.Buffer);
            }
        }
        
        private void OnReceived(byte[] data)
        {
            try
            {
                var message = System.Text.Encoding.ASCII.GetString(data);
                if (_captureSource.TryParse(message, ref _receivedDataBuffer))
                {
                    OnDataReceived?.Invoke(_receivedDataBuffer);
                }
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }
        
        private static void LogError(object message)
        {
#if UNITY_ENGINE
            UnityEngine.Debug.LogError($"[{nameof(FacialCaptureReceiver)}] {message}");
#else
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{nameof(FacialCaptureReceiver)}] {message}");
            Console.ResetColor();
#endif
        }
        
        [
            System.Diagnostics.Conditional("DEBUG"),
            System.Diagnostics.Conditional("DEVELOPMENT_BUILD"),
            System.Diagnostics.Conditional("UNITY_EDITOR"),
        ]
        private static void DebugLog(object message)
        {
#if UNITY_ENGINE
            UnityEngine.Debug.Log($"[{nameof(FacialCaptureReceiver)}] {message}");
#else
            Console.WriteLine($"[{nameof(FacialCaptureReceiver)}] {message}");
#endif
        }
    }
}