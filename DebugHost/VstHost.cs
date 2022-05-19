using AudioPlugSharp;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DebugHost
{
    internal class VstHost : IAudioHost
    {
        private IAudioPlugin AudioPlugin;

        public VstHost(IAudioPlugin audioPlugin)
        {
            audioPlugin.Host = this;
            audioPlugin.Processor.Initialize();
            AudioPlugin = audioPlugin;
        }

        AudioFileReader AudioFile;
        VSTSampleProvider VSTProvider;
        WaveOutEvent OutputDevice = new WaveOutEvent();

        public void PlayAudio(string filename, bool loop = false)
        {
            // audio file
            AudioFile = new AudioFileReader(filename);
            SampleRate = AudioFile.WaveFormat.SampleRate;

            // vst
            VSTProvider = new VSTSampleProvider(AudioPlugin);
            VSTProvider.Init(AudioFile);

            // init output
            OutputDevice = new WaveOutEvent();
            OutputDevice.Init(VSTProvider);
            OutputDevice.Play();

            // play loop
            if (loop)
            {
                OutputDevice.PlaybackStopped += (object? sender, StoppedEventArgs e) =>
                {
                    AudioFile.Position = 0;
                    OutputDevice.Play();
                };
            }
        }

        public double SampleRate { get; set; } = 48000;

        public uint MaxAudioBufferSize => 1024;

        public EAudioBitsPerSample BitsPerSample => EAudioBitsPerSample.Bits32;

        public double BPM => 120;

        public void BeginEdit(int parameter)
        {
            Trace.WriteLine($"BeginEdit: {parameter}");
        }

        public void EndEdit(int parameter)
        {
            Trace.WriteLine($"EndEdit: {parameter}");
        }

        public void PerformEdit(int parameter, double normalizedValue)
        {
            Trace.WriteLine($"PerformEdit: {parameter} ({normalizedValue})");
        }

        public void SendNoteOff(int noteNumber, float velocity)
        {
            Trace.WriteLine($"SendNoteOff: {noteNumber} ({velocity})");
        }

        public void SendNoteOn(int noteNumber, float velocity)
        {
            Trace.WriteLine($"SendNoteOn: {noteNumber} ({velocity})");
        }

        public void SendPolyPressure(int noteNumber, float pressure)
        {
            Trace.WriteLine($"SendNoteOn: {noteNumber} ({noteNumber})");
        }
    }
}
