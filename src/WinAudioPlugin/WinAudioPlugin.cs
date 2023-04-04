namespace NotADoctor99.WinAudioPlugin
{
    using System;

    using Loupedeck;

    public class WinAudioPlugin : Plugin
    {
        public override Boolean UsesApplicationApiOnly => true;

        public override Boolean HasNoApplication => true;

        public static OutputDevices OutputDevices { get; } = new OutputDevices();

        public WinAudioPlugin()
        {
            PluginLog.Init(this.Log);
            PluginResources.Init(this.Assembly);
        }

        public override void Load() => Helpers.StartNewTask(() => WinAudioPlugin.OutputDevices.Start());

        public override void Unload() => WinAudioPlugin.OutputDevices.Stop();
    }
}
