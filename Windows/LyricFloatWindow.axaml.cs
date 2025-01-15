using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Miku_UI_Music_Center.Utils;
using NAudio.Wave;
using OpenTK.Audio.OpenAL;

namespace Miku_UI_Music_Center;

public partial class LyricFloatWindow : Window
{
    // Audio Configuration
    private const int SampleRate = 48000;
    private const int Channels = 2; // Stereo, Windows only
    private const int BitsPerSample = 16; // 16-bit, Windows only
    private const int MAX_BUFFER_SIZE = 15360; // 14208 for 44.1kHz, 16-bit, 2-channel audio, 15360 for 48kHz, 16-bit, 2-channel audio

    // Network Configuration
    private const int LyricPort = 39393;
    private const int AudioPort = 39392;
    // This is the IP address of the local machine
    private IPAddress localIpAddress = IPAddress.Parse(IPTools.GetLocalIPv4());

    private const string EOM = "<|EOM|>"; // End of message for lyric

    // NAudio (Windows only)
    private BufferedWaveProvider waveProvider = null!;
    private WaveOutEvent waveOut = null!;

    // OpenAL (Linux)
    private const int BufferCount = 16;
    private int[] alBuffers = new int[BufferCount];
    private int source;
    private const ALFormat format = ALFormat.Stereo16; // Stereo, 16-bit

    public LyricFloatWindow(bool isAudioForwadEnabled)
    {
        InitializeComponent();
        InitListener();
        if (isAudioForwadEnabled)
        {
            if (OperatingSystem.IsWindows())
            {
                InitNAudioPlayer();
            }
            else
            {
                InitOpenALAudioPlayer();
            }
            InitAudioListener();

        }
    }

    private void InitNAudioPlayer()
    {
        waveProvider = new BufferedWaveProvider(new WaveFormat(SampleRate, BitsPerSample, Channels)); // Default to 48.0kHz, 16-bit, 2-channel audio
        waveOut = new WaveOutEvent();
        waveOut.Init(waveProvider);
        waveOut.Play();
    }

    private async void InitAudioListener()
    {

        IPEndPoint ipEndPoint = new(localIpAddress, AudioPort);

        using Socket listener = new(
            ipEndPoint.AddressFamily,
            SocketType.Dgram,
            ProtocolType.Udp
        );

        listener.Bind(ipEndPoint);
        await StartAudioListen(listener);
    }

    private void InitOpenALAudioPlayer()
    {
        ALDevice device = ALC.OpenDevice(null);
        ALContext context = ALC.CreateContext(device, (int[])null);
        ALC.MakeContextCurrent(context);

        AL.GenBuffers(BufferCount, alBuffers);
        source = AL.GenSource();
    }

    private async Task StartAudioListen(Socket listener)
    {
        byte[] buffer = new byte[MAX_BUFFER_SIZE];

        if (OperatingSystem.IsWindows())
        {
            while (true)
            {
                EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                SocketReceiveFromResult result = await listener.ReceiveFromAsync(new ArraySegment<byte>(buffer), SocketFlags.None, remoteEndPoint);

                int receivedBytes = result.ReceivedBytes;

                try
                {
                    // Write received data into waveProvider
                    waveProvider.AddSamples(buffer, 0, receivedBytes);
                } catch (System.InvalidOperationException)
                {
                    // Buffer overflow
                    waveProvider.ClearBuffer();
                }
            }
        }
        else
        {
            int bufferIndex = 0;

            while (true)
            {
                EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                long startTime = DateTime.Now.Millisecond;
                var receiveTask = listener.ReceiveFromAsync(new ArraySegment<byte>(buffer), SocketFlags.None, remoteEndPoint);
                if (await Task.WhenAny(receiveTask, Task.Delay(500)) == receiveTask)
                {
                    // Data received
                    SocketReceiveFromResult result = receiveTask.Result;

                    // Calculate latency
                    long currentTime = DateTime.Now.Millisecond;
                    long latency = currentTime - startTime;
                    int receivedBytes = result.ReceivedBytes;

                    // Write received data into OpenAL buffer
                    GCHandle pinnedArray = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    nint pointer = pinnedArray.AddrOfPinnedObject();
                    AL.BufferData(alBuffers[bufferIndex], format, pointer, receivedBytes, SampleRate);
                    pinnedArray.Free();

                    AL.SourceQueueBuffer(source, alBuffers[bufferIndex]);
                    bufferIndex = (bufferIndex + 1) % BufferCount;

                    // Start playing if not already playing
                    AL.GetSource(source, ALGetSourcei.SourceState, out int state);
                    if ((ALSourceState)state != ALSourceState.Playing)
                    {
                        AL.SourcePlay(source);
                    }

                    // Unqueue processed buffers
                    int processed;
                    AL.GetSource(source, ALGetSourcei.BuffersProcessed, out processed);
                    while (processed-- > 0)
                    {
                        AL.SourceUnqueueBuffer(source);
                    }
                }
                else
                {
                    // Timeout
                    await Task.Delay(1500); // Wait for 1.5 second before attempting to reconnect
                    listener.Close();
                    AL.SourceStop(source);
                    alBuffers = new int[BufferCount];
                    InitOpenALAudioPlayer(); // Reinitialize the audio player
                    InitAudioListener(); // Reinitialize the listener
                    break;
                }
            }
        }
    }

    private async void InitListener()
    {
        IPEndPoint ipEndPoint = new(localIpAddress, LyricPort);

        using Socket listener = new(
            ipEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp
        );

        listener.Bind(ipEndPoint);
        listener.Listen(100);
        //LyricText.Text = $"Listening on {localIpAddress}:{ipEndPoint.Port}...";
        await StartListen(listener);
    }

    private async Task StartListen(Socket socket)
    {
        var handler = await socket.AcceptAsync();
        while (true)
        {
            // Receive lyric.
            var buffer = new byte[1_024];
            var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer, 0, received);


            if (response.IndexOf(EOM) > -1 /* is end of Lyric */)
            {
                LyricText.Text = response.Replace(EOM, "");
                await StartListen(socket);
                break;
            }
        }
    }
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            BeginMoveDrag(e);
        }
    }
}