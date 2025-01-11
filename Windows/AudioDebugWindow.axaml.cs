using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NAudio.Wave;
using Miku_UI_Music_Center.Utils;
using OpenTK.Audio.OpenAL;
using System.Runtime.InteropServices;

namespace Miku_UI_Music_Center;

public partial class AudioDebugWindow : Window
{
    // Audio Configuration
    private const int SampleRate = 48000;
    private const int Channels = 2; // Stereo, Windows only
    private const int BitsPerSample = 16; // 16-bit, Windows only
    private const int MAX_BUFFER_SIZE = 15360; // 14208 for 44.1kHz, 16-bit, 2-channel audio, 15360 for 48kHz, 16-bit, 2-channel audio

    // Network Configuration
    private const int AudioPort = 39392;
    // This is the IP address of the local machine
    private IPAddress localIpAddress = IPAddress.Parse(IPTools.GetLocalIPv4());

    // NAudio (Windows only)
    private BufferedWaveProvider waveProvider = null!;
    private WaveOutEvent waveOut = null!;

    // OpenAL (Linux)
    private const int BufferCount = 8;
    private int[] alBuffers = new int[BufferCount];
    private int source;
    private const ALFormat format = ALFormat.Stereo16; // Stereo, 16-bit

    public AudioDebugWindow()
    {
        InitializeComponent();
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
        DebugInfoText.Text = $"Listening on {localIpAddress}:{ipEndPoint.Port}...";
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

                // Write received data into waveProvider
                waveProvider.AddSamples(buffer, 0, receivedBytes);
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

                    // Update UI Display
                    if (latency > 0)
                    {
                        DebugInfoText.Text = $"Connected! Latency: {latency}ms";
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
                    DebugInfoText.Text = "Connection lost. Reconnecting...";
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
}
