using AudioPlugSharp;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DebugHost
{
    public class VSTSampleProvider : ISampleProvider
    {
        ISampleProvider UpstreamProvider;
        IAudioPlugin AudioPlugin;

        public WaveFormat WaveFormat => UpstreamProvider.WaveFormat;

        public VSTSampleProvider(IAudioPlugin audioPlugin)
        {
            AudioPlugin = audioPlugin;
        }

        public void Init(ISampleProvider sampleProvider)
        {
            UpstreamProvider = sampleProvider;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int numInChan = UpstreamProvider.WaveFormat.Channels;
            int numSamples = count / numInChan;

            float[] inRawBuffer = new float[buffer.Length];
            int numRead = UpstreamProvider.Read(inRawBuffer, offset, count);

            // create vst buffers
            // TODO: reuse buffers

            float[][] inputBuffer = new float[numInChan][];
            for (int i = 0; i < numInChan; i++)
            {
                inputBuffer[i] = new float[numSamples];
            }
            float[][] outputBuffer = new float[numInChan][];
            for (int i = 0; i < numInChan; i++)
            {
                outputBuffer[i] = new float[numSamples];
            }

            // push samples to vst buffer

            for (int chan = 0; chan < numInChan; chan++)
            {
                for (int i = 0; i < numSamples; i++)
                {
                    inputBuffer[chan][i] = inRawBuffer[i * numInChan + chan];
                }
            }

            // pin and set vst buffers

            IntPtr inputBufferPtr = GetBufferPtr(inputBuffer, out List<GCHandle> inputBufferHandles);
            IntPtr outputBufferPtr = GetBufferPtr(outputBuffer, out List<GCHandle> outputBufferHandles);

            AudioPlugin.Processor.InputPorts[0].SetAudioBufferPtrs(inputBufferPtr, EAudioBitsPerSample.Bits32, (uint)numSamples);
            AudioPlugin.Processor.OutputPorts[0].SetAudioBufferPtrs(outputBufferPtr, EAudioBitsPerSample.Bits32, (uint)numSamples);

            // process

            AudioPlugin.Processor.Process();

            // free vst buffers

            inputBufferHandles.Reverse();
            foreach (GCHandle handle in inputBufferHandles)
            {
                handle.Free();
            }
            outputBufferHandles.Reverse();
            foreach (GCHandle handle in outputBufferHandles)
            {
                handle.Free();
            }

            // pull data from vst buffer

            for (int i = 0; i < numSamples; i++)
            {
                for (int chan = 0; chan < numInChan; chan++)
                {
                    buffer[i * numInChan + chan] = outputBuffer[chan][i];
                }
            }

            return numRead;
        }

        IntPtr GetBufferPtr(float[][] buffer, out List<GCHandle> handles)
        {
            // fix channels
            handles = new List<GCHandle>();
            for (int i = 0; i < buffer.Length; i++)
            {
                GCHandle chanHandle = GCHandle.Alloc(buffer[i], GCHandleType.Pinned);
                handles.Add(chanHandle);
            }

            // fix buffer
            IntPtr[] bufferChanPtr = new IntPtr[buffer.Length];
            for (int chan = 0; chan < bufferChanPtr.Length; chan++)
            {
                bufferChanPtr[chan] = handles[chan].AddrOfPinnedObject();
            }
            GCHandle bufferHandle = GCHandle.Alloc(bufferChanPtr, GCHandleType.Pinned);
            handles.Add(bufferHandle);

            IntPtr bufferPtr = bufferHandle.AddrOfPinnedObject();
            return bufferPtr;
        }

    }
}
