namespace NotADoctor99.WinAudioPlugin
{
    using System;

    using Loupedeck;

    public class MuteAllOutputDevicesCommand : PluginDynamicCommand
    {
        private readonly DictionaryNoCase<String> _deviceIds = new DictionaryNoCase<String>();
        private readonly DictionaryNoCase<String> _actionParameters = new DictionaryNoCase<String>();

        public MuteAllOutputDevicesCommand()
            : base("Mute all output devices", "Mutes all available output devices", "")
        {
        }
        protected override void RunCommand(String actionParameter) => WinAudioPlugin.OutputDevices.MuteAllDevices();

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) => PluginResources.ReadImage("MuteAllOutputDevices.png");
    }
}
