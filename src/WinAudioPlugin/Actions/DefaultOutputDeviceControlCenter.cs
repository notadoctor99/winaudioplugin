namespace NotADoctor99.WinAudioPlugin
{
    using System;
    using System.Collections.Generic;

    using Loupedeck;

    public class DefaultOutputDeviceControlCenter : PluginDynamicFolder
    {
        public DefaultOutputDeviceControlCenter()
        {
            this.DisplayName = "Change Default Output Device";
            this.Description = "Sets any available device as default output device";
        }

        public override PluginDynamicFolderNavigation GetNavigationArea(DeviceType _) => PluginDynamicFolderNavigation.ButtonArea;

        public override Boolean Load()
        {
            WinAudioPlugin.OutputDevices.DefaultDeviceChanged += this.OnDefaultDeviceChanged;

            return true;
        }

        public override Boolean Unload()
        {
            WinAudioPlugin.OutputDevices.DefaultDeviceChanged -= this.OnDefaultDeviceChanged;

            return true;
        }

        public override BitmapImage GetButtonImage(PluginImageSize imageSize) => PluginResources.ReadImage("ChangeDefaultOutputDevice.png");

        public override IEnumerable<String> GetButtonPressActionNames(DeviceType deviceType)
        {
            foreach (var device in WinAudioPlugin.OutputDevices.EnumerateDevices())
            {
                yield return this.CreateCommandName(device.Id);
            }
        }

        public override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) => DeviceHelpers.GetCommandImage(actionParameter);

        public override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) => DeviceHelpers.GetCommandDisplayName(actionParameter);

        public override void RunCommand(String actionParameter) => WinAudioPlugin.OutputDevices.SetDefaultDevice(actionParameter);

        private void OnDefaultDeviceChanged(Object sender, OutputDefaultDeviceEventArgs e)
        {
            this.CommandImageChanged(e.OldDeviceId);
            this.CommandImageChanged(e.NewDeviceId);
        }
    }
}

