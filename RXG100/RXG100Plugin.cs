using AudioLib;
using AudioLib.SplineLut;
using AudioLib.TF;
using AudioPlugSharp;
using AudioPlugSharpWPF;
using RXG100.DSP;
using System;
using System.Windows.Controls;

namespace RXG100
{
    public class RXG100Plugin : AudioPluginWPF
    {
        AudioIOPort monoInput;
        AudioIOPort stereoOutput;

        public RXG100Plugin()
        {
            Company = "Xuan25";
            Website = "xuan25.com";
            Contact = "contact@my.email";
            PluginName = "RXG-100";
            PluginCategory = "Fx";
            PluginVersion = "1.0.0";

            // Unique 64bit ID for the plugin
            PluginID = 0x1E92758E710B4941;

            HasUserInterface = true;
            EditorWidth = 1100;
            EditorHeight = 294;
        }

        public AudioPluginParameter ChannelParam { get; set; } = new AudioPluginParameter() { ID = "Channel", Name = "Channel", Type = EAudioPluginParameterType.Bool, DefaultValue = 0 };
        public AudioPluginParameter InputAParam { get; set; } = new AudioPluginParameter() { ID = "InputA", Name = "InputA", Type = EAudioPluginParameterType.Float };
        public AudioPluginParameter GainAParam { get; set; } = new AudioPluginParameter() { ID = "GainA", Name = "GainA", Type = EAudioPluginParameterType.Float };
        public AudioPluginParameter VolumnAParam { get; set; } = new AudioPluginParameter() { ID = "VolumnA", Name = "VolumnA", Type = EAudioPluginParameterType.Float };
        public AudioPluginParameter BassAParam { get; set; } = new AudioPluginParameter() { ID = "BassA", Name = "BassA", Type = EAudioPluginParameterType.Float };
        public AudioPluginParameter MidAParam { get; set; } = new AudioPluginParameter() { ID = "MidA", Name = "MidA", Type = EAudioPluginParameterType.Float };
        public AudioPluginParameter TrebleAParam { get; set; } = new AudioPluginParameter() { ID = "TrebleA", Name = "TrebleA", Type = EAudioPluginParameterType.Float };
        public AudioPluginParameter PresenceAParam { get; set; } = new AudioPluginParameter() { ID = "PresenceA", Name = "PresenceA", Type = EAudioPluginParameterType.Float };
        public AudioPluginParameter InputBParam { get; set; } = new AudioPluginParameter() { ID = "InputB", Name = "InputB", Type = EAudioPluginParameterType.Float };
        public AudioPluginParameter GainBParam { get; set; } = new AudioPluginParameter() { ID = "GainB", Name = "GainB", Type = EAudioPluginParameterType.Float };
        public AudioPluginParameter VolumnBParam { get; set; } = new AudioPluginParameter() { ID = "VolumnB", Name = "VolumnB", Type = EAudioPluginParameterType.Float };
        public AudioPluginParameter BoostBParam { get; set; } = new AudioPluginParameter() { ID = "BoostB", Name = "BoostB", Type = EAudioPluginParameterType.Bool, DefaultValue = 0 };
        public AudioPluginParameter BassBParam { get; set; } = new AudioPluginParameter() { ID = "BassB", Name = "BassB", Type = EAudioPluginParameterType.Float };
        public AudioPluginParameter MidBParam { get; set; } = new AudioPluginParameter() { ID = "MidB", Name = "MidB", Type = EAudioPluginParameterType.Float };
        public AudioPluginParameter TrebleBParam { get; set; } = new AudioPluginParameter() { ID = "TrebleB", Name = "TrebleB", Type = EAudioPluginParameterType.Float };
        public AudioPluginParameter PresenceBParam { get; set; } = new AudioPluginParameter() { ID = "PresenceB", Name = "PresenceB", Type = EAudioPluginParameterType.Float };


        // ------- Declare modules for processing ----------

        private Highpass1 HighpassInput;

        // Channel A
        private TF1 TF1A;
        private SplineInterpolator Stage1A;
        private TF2 TF2A;
        private SplineInterpolator Stage2A;
        private Highpass1 PostVolumeHpA;
        private TFPres TFPresA;
        private Tonestack TonestackA;

        // Channel B
        private TF1 TF1B;
        private TF12 TF1xB;
        private SplineInterpolator Stage1B;
        private TF2 TF2B;
        private SplineInterpolator Stage2B;
        private SplineInterpolator ClipperZenerB;
        private SplineInterpolator ClipperDiodeB;
        private TFVolume TFVolumeB;
        private TFPres TFPresB;
        private Tonestack TonestackB;

        // Hipass filters
        private Highpass1 H3;
        private Highpass1 HipassZenerB;

        private Lowpass1 LowpassOutput;

        double SampleRate;

        public override void Initialize()
        {
            InputPorts = new AudioIOPort[] { monoInput = new AudioIOPort("Mono Input", EAudioChannelConfiguration.Mono) };
            OutputPorts = new AudioIOPort[] { stereoOutput = new AudioIOPort("Stereo Output", EAudioChannelConfiguration.Stereo) };

            AddParameter(ChannelParam);

            AddParameter(InputAParam);
            AddParameter(GainAParam);
            AddParameter(VolumnAParam);
            AddParameter(BassAParam);
            AddParameter(MidAParam);
            AddParameter(TrebleAParam);
            AddParameter(PresenceAParam);

            AddParameter(InputBParam);
            AddParameter(GainBParam);
            AddParameter(VolumnBParam);
            AddParameter(BoostBParam);
            AddParameter(BassBParam);
            AddParameter(MidBParam);
            AddParameter(TrebleBParam);
            AddParameter(PresenceBParam);

            base.Initialize();

            // Parameter Value Changed event

            ChannelParam.ValueChanged += OnParameterValueChanged;

            InputAParam.ValueChanged += OnParameterValueChanged;
            GainAParam.ValueChanged += OnParameterValueChanged;
            VolumnAParam.ValueChanged += OnParameterValueChanged;
            BassAParam.ValueChanged += OnParameterValueChanged;
            MidAParam.ValueChanged += OnParameterValueChanged;
            TrebleAParam.ValueChanged += OnParameterValueChanged;
            PresenceAParam.ValueChanged += OnParameterValueChanged;

            InputBParam.ValueChanged += OnParameterValueChanged;
            GainBParam.ValueChanged += OnParameterValueChanged;
            VolumnBParam.ValueChanged += OnParameterValueChanged;
            BoostBParam.ValueChanged += OnParameterValueChanged;
            BassBParam.ValueChanged += OnParameterValueChanged;
            MidBParam.ValueChanged += OnParameterValueChanged;
            TrebleBParam.ValueChanged += OnParameterValueChanged;
            PresenceBParam.ValueChanged += OnParameterValueChanged;

            SampleRate = 48000;

            InitFilters();
        }

        public override UserControl GetEditorView()
        {
            return new EditorView(this);
        }

        public override void Process()
        {
            base.Process();

            if (SampleRate != Host.SampleRate)
            {
                SampleRate = Host.SampleRate;
                SetSampleRate(SampleRate);
            }

            monoInput.ReadData();

            double[] inSamples = monoInput.GetAudioBuffers()[0];

            double[] outLeftSamples = stereoOutput.GetAudioBuffers()[0];
            double[] outRightSamples = stereoOutput.GetAudioBuffers()[1];

            ProcessBuffer(inSamples, outLeftSamples);
            for (int i = 0; i < outLeftSamples.Length; i++)
            {
                outRightSamples[i] = outLeftSamples[i];
            }

            stereoOutput.WriteData();
        }

        private void ProcessBuffer(double[] input, double[] output)
        {
            // copy data to outBuffer
            for (int i = 0; i < input.Length; i++)
            {
                output[i] = input[i];
            }

            double[] signal = output;
            HighpassInput.ProcessInPlace(signal);

            // Channel A
            if (ChannelParam.Value <= 0.5)
            {
                var inputGain = Utils.ExpResponse(InputAParam.Value);
                Utils.GainInPlace(signal, inputGain);

                TF1A.ProcessInPlace(signal);
                Stage1A.ProcessInPlace(signal);

                TF2A.ProcessInPlace(signal);
                Stage2A.ProcessInPlace(signal);

                var volume = Utils.ExpResponse(VolumnAParam.Value) * 0.333; // match loudness of channels
                Utils.GainInPlace(signal, volume);

                PostVolumeHpA.ProcessInPlace(signal);
                TonestackA.ProcessInPlace(signal);
                TFPresA.ProcessInPlace(signal);
            }

            // Channel B
            if (ChannelParam.Value > 0.5f)
            {
                var inputGain = Utils.ExpResponse(InputBParam.Value) * 5;
                Utils.GainInPlace(signal, inputGain);

                TF1B.ProcessInPlace(signal);
                TF1xB.ProcessInPlace(signal);

                Stage1B.ProcessInPlace(signal);

                TF2B.ProcessInPlace(signal);
                Stage2B.ProcessInPlace(signal);

                HipassZenerB.ProcessInPlace(signal);
                ClipperZenerB.ProcessInPlace(signal);

                if (BoostBParam.Value > 0.5f)
                {
                    ClipperDiodeB.ProcessInPlace(signal);
                    Utils.GainInPlace(signal, 6);
                }

                TFVolumeB.ProcessInPlace(signal);
                TonestackB.ProcessInPlace(signal);
                TFPresB.ProcessInPlace(signal);
            }

            LowpassOutput.ProcessInPlace(signal);

            // prevent horrible noise in case something goes wrong
            Utils.SaturateInPlace(signal, -4, 4);
        }

        private void OnParameterValueChanged(AudioPluginParameter parameter)
        {
            UpdateFilterSettings(parameter.ParameterIndex);
        }

        public void SetSampleRate(double samplerate)
        {
            this.SampleRate = samplerate;

            HighpassInput.Fs = SampleRate;

            // Channel A
            TF1A.Fs = SampleRate;
            TF2A.Fs = SampleRate;
            PostVolumeHpA.Fs = SampleRate;
            TFPresA.Fs = SampleRate;
            TonestackA.Fs = SampleRate;

            // Channel B
            TF1B.Fs = SampleRate;
            TF1xB.Fs = SampleRate;
            TF2B.Fs = SampleRate;
            TFVolumeB.Fs = SampleRate;
            TFPresB.Fs = SampleRate;
            TonestackB.Fs = SampleRate;

            H3.Fs = SampleRate;
            HipassZenerB.Fs = SampleRate;

            LowpassOutput.Fs = SampleRate;

            UpdateFilterSettings(null);
        }

        private void InitFilters()
        {
            HighpassInput = new Highpass1((float)SampleRate);

            // Channel A
            TF1A = new TF1(SampleRate);
            Stage1A = new SplineInterpolator(DSP.Data.Splines.Stage2_2TF);

            TF2A = new TF2(SampleRate);
            Stage2A = new SplineInterpolator(DSP.Data.Splines.Stage2_2TF);

            PostVolumeHpA = new Highpass1((float)SampleRate);
            TFPresA = new TFPres(SampleRate);
            TonestackA = new Tonestack((float)SampleRate);

            // Channel B
            TF1B = new TF1((float)SampleRate);
            TF1xB = new TF12((float)SampleRate);
            Stage1B = new SplineInterpolator(DSP.Data.Splines.Stage1CapSimulatedTF);

            TF2B = new TF2((float)SampleRate);
            Stage2B = new SplineInterpolator(DSP.Data.Splines.Stage2SimulatedTF);

            ClipperZenerB = new SplineInterpolator(DSP.Data.Splines.ZenerTF);
            ClipperDiodeB = new SplineInterpolator(DSP.Data.Splines.D1N914TF);

            TFVolumeB = new TFVolume((float)SampleRate);
            TFPresB = new TFPres((float)SampleRate);
            TonestackB = new Tonestack((float)SampleRate);

            H3 = new Highpass1((float)SampleRate);
            HipassZenerB = new Highpass1((float)SampleRate);
            LowpassOutput = new Lowpass1((float)SampleRate);
            TonestackA.setComponents(0.200e-9f, 0.022e-6f, 0.02e-6f, 1e3f, 500e3f, 47e3f, 500e3f, 20e3f, 500e3f);
            TonestackB.setComponents(0.200e-9f, 0.022e-6f, 0.02e-6f, 1e3f, 500e3f, 47e3f, 500e3f, 20e3f, 500e3f);

            SetSampleRate(SampleRate);
        }

        private void UpdateFilterSettings(int? parameter)
        {
            if (parameter == null)
            {
                HighpassInput.SetParam(Highpass1.P_FREQ, 180);
                H3.SetParam(Highpass1.P_FREQ, 59.45f);
                HipassZenerB.SetParam(Highpass1.P_FREQ, 8);
                LowpassOutput.SetParam(Lowpass1.P_FREQ, 10000);

                // Channel A ------------------------------------------------------
                TF1A.Update();
                TF2A.SetParam(TF2.P_USE_R3, 0.0);
            }

            if (parameter == null || parameter == GainAParam.ParameterIndex)
                TF2A.SetParam(TF2.P_GAIN, Utils.ExpResponse(GainAParam.Value));

            if (parameter == null)
            {
                PostVolumeHpA.SetParam(Highpass1.P_FREQ, 30);
                //Stage1A.Bias = -0.5f;
                Stage1A.Bias = -0.14f;
                Stage2A.Bias = -0.15f;
            }

            if (parameter == null || parameter == BassAParam.ParameterIndex || parameter == MidAParam.ParameterIndex || parameter == TrebleAParam.ParameterIndex)
            {
                TonestackA.SetParam(Tonestack.P_BASS, Utils.ExpResponse(BassAParam.Value));
                TonestackA.SetParam(Tonestack.P_MID, MidAParam.Value);
                TonestackA.SetParam(Tonestack.P_TREBLE, TrebleAParam.Value);
            }

            if (parameter == null || parameter == PresenceAParam.ParameterIndex)
                TFPresA.SetParam(TFPres.P_PRES, PresenceAParam.Value);

            // Channel B ------------------------------------------------------

            if (parameter == null)
            {
                TF1B.Update();
                TF2B.SetParam(TF2.P_USE_R3, 1.0);
                TF1xB.Update();
            }

            if (parameter == null || parameter == GainBParam.ParameterIndex)
                TF2B.SetParam(TF2.P_GAIN, Utils.ExpResponse(GainBParam.Value));

            if (parameter == null || parameter == VolumnBParam.ParameterIndex)
                TFVolumeB.SetParam(TFVolume.P_VOL, Utils.ExpResponse(VolumnBParam.Value));

            if (parameter == null)
            {
                Stage1B.Bias = 0.47 - 0.5f;
                Stage2B.Bias = 0.63 - 0.5f;
            }

            if (parameter == null || parameter == BassBParam.ParameterIndex || parameter == MidBParam.ParameterIndex || parameter == TrebleBParam.ParameterIndex)
            {
                TonestackB.SetParam(Tonestack.P_BASS, Utils.ExpResponse(BassBParam.Value));
                TonestackB.SetParam(Tonestack.P_MID, MidBParam.Value);
                TonestackB.SetParam(Tonestack.P_TREBLE, TrebleBParam.Value);
            }

            if (parameter == null || parameter == PresenceBParam.ParameterIndex)
                TFPresB.SetParam(TFPres.P_PRES, PresenceBParam.Value);
        }
    }
}
