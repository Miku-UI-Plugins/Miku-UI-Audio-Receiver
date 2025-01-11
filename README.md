# Miku-UI-Audio-Receiver
Hereinafter referred to as **MUAR**

## What is this?
This is a program that receives audio forwarded from the Miku UI Audio Forwarder.

Oh, it can also receives the lyrics!

## How does it work?
By setting up a socket server locally, MUAR will be able to receive data sent via socket from devices that have the Miku UI Lyrics Stub (Hereinafter referred to as **MULS**) installed.

## How to use?
1. Launch MULS from your device.
2. Go to Settings and enable **Remote lyric**.
3. Click **Remote IP** below, and enter the IP address of your Windows PC.
4. Restart MULS.
5. Click **Forward audio** in **Settings**, and enter the IP address of your Windows PC, then click **Connect**.
6. Launch MUAR from your Windows PC.
7. Enjoy!

## TODO:
- ~~Audio forward~~(Done)
- ~~Lyric forward~~(Done)
