using AudioPlugSharp;
using AudioPlugSharpWPF;
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
        }

        public override void Process()
        {
            base.Process();

            //double gain = GetParameter("gain").Value;
            //double linearGain = Math.Pow(10.0, 0.05 * gain);

            //double pan = GetParameter("pan").Value;

            monoInput.ReadData();

            double[] inSamples = monoInput.GetAudioBuffers()[0];

            double[] outLeftSamples = stereoOutput.GetAudioBuffers()[0];
            double[] outRightSamples = stereoOutput.GetAudioBuffers()[1];

            //for (int i = 0; i < inSamples.Length; i++)
            //{
            //    outLeftSamples[i] = inSamples[i] * linearGain * (1 - pan);
            //    outRightSamples[i] = inSamples[i] * linearGain * (1 + pan);
            //}

            stereoOutput.WriteData();
        }

        public override UserControl GetEditorView()
        {
            return new EditorView(this);
        }


    }
}
