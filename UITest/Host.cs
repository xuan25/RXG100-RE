using AudioPlugSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UITest
{
    internal class Host : IAudioHost
    {
        public double SampleRate => 48000;

        public uint MaxAudioBufferSize => 1024;

        public EAudioBitsPerSample BitsPerSample => EAudioBitsPerSample.Bits64;

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
