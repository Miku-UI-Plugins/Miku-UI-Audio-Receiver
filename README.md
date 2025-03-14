# Miku-UI-Audio-Receiver
Hereinafter referred to as **MUAR**

## What is this?
This is a program that receives audio forwarded from the Miku UI Audio Forwarder.

Oh, it can also receives the lyrics!

## How does it work?
By setting up a socket server locally, MUAR will be able to receive data sent via socket from devices that have the Miku UI Lyrics Stub (Hereinafter referred to as **MULS**) installed.

## How to use?
### For Windows User
1. Launch MULS from your device.
2. Go to Settings and enable **Remote lyric**.
3. Click **Remote IP** below, and enter the IP address of your Windows PC.
4. Restart MULS.
5. Click **Forward audio** in **Settings**, and enter the IP address of your Windows PC, then click **Connect**.
6. Download & Extract zip of MUAR release.
7. Launch `win-{ARCH}/Miku UI Music Center.exe` from your Windows PC.
8. Enjoy!

### For Linux User

Before we start, ensure you have .NET Runtime and OpenAL installed, if not, install them first.

`sudo apt-get install -y dotnet-runtime-8.0`

`sudo apt-get install libopenal-dev`

1. Launch MULS from your device.
2. Go to Settings and enable **Remote lyric**.
3. Click **Remote IP** below, and enter the IP address of your Linux PC.
4. Restart MULS.
5. Click **Forward audio** in **Settings**, and enter the IP address of your Linux PC, then click **Connect**.
6. Download & Extract zip of MUAR release.
7. Open terminal and `chmod +x 'linux-{ARCH}/Miku UI Music Center'`
8. `'linux-{ARCH}/Miku UI Music Center'`
9. Enjoy!

### For MacOS User (x64 only currently, but it works on Apple M series as well)

**MacOS support starting from v2.1.0**

Before we start, ensure you have .NET Runtime installed, if not, install it first.

You can install it via [official website](https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-8.0.14-macos-x64-installer)

1. Launch MULS from your device.
2. Go to Settings and enable **Remote lyric**.
3. Click **Remote IP** below, and enter the IP address of your Linux PC.
4. Restart MULS.
5. Click **Forward audio** in **Settings**, and enter the IP address of your Linux PC, then click **Connect**.
6. Download & Extract zip of MUAR release.
7. Open terminal and `chmod +x 'osx-{ARCH}/Miku UI Music Center'`
8. `'osx-{ARCH}/Miku UI Music Center'` or you could just double click the binary.
9. Enjoy!

note: Due to security policies of MacOS, you may need to perform additional actions to use MUAR.

plus: MacOS has an exclusive menu bar lyrics feature!

## TODO:
- ~~Audio forward~~(Done)
- ~~Lyric forward~~(Done)
- Media Control
