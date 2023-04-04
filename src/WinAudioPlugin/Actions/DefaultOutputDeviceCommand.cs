namespace NotADoctor99.WinAudioPlugin
{
    using System;

    using Loupedeck;

    public class DefaultOutputDeviceCommand : PluginTwoStateDynamicCommand
    {
        private readonly DictionaryNoCase<String> _deviceIds = new DictionaryNoCase<String>();
        private readonly DictionaryNoCase<String> _actionParameters = new DictionaryNoCase<String>();

        public DefaultOutputDeviceCommand()
        {
            this.GroupName = "Set Default Output Device";
            this.Description = "Sets this device as default output device";

            this.SetOffStateDisplayName("Non-default output device");
            this.SetOnStateDisplayName("Default output device");

            this.AddToggleCommand("Set default device").SetDescription(this.Description);
        }

        protected override Boolean OnLoad()
        {
            WinAudioPlugin.OutputDevices.DeviceListChanged += this.OnDeviceListChanged;
            WinAudioPlugin.OutputDevices.DefaultDeviceChanged += this.OnDefaultDeviceChanged;

            return true;
        }

        protected override Boolean OnUnload()
        {
            WinAudioPlugin.OutputDevices.DeviceListChanged -= this.OnDeviceListChanged;
            WinAudioPlugin.OutputDevices.DefaultDeviceChanged -= this.OnDefaultDeviceChanged;

            return true;
        }

        protected override void RunCommand(String actionParameter)
        {
            if (this.TryGetDeviceId(actionParameter, out var deviceId))
            {
                WinAudioPlugin.OutputDevices.SetDefaultDevice(deviceId);
            }
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
            => this.TryGetDeviceId(actionParameter, out var deviceId) ? DeviceHelpers.GetCommandImage(deviceId) : null;

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) => actionParameter;

        private Boolean TryGetDeviceId(String actionParameter, out String deviceId) => this._deviceIds.TryGetValueSafe(actionParameter, out deviceId);

        private void UpdateParameters()
        {
            this.RemoveAllParameters();

            foreach (var device in WinAudioPlugin.OutputDevices.Devices)
            {
                this._deviceIds[device.LongDisplayName] = device.Id;
                this._actionParameters[device.Id] = device.LongDisplayName;
                this.AddParameter(device.LongDisplayName, device.LongDisplayName, this.GroupName);
                this.SetCurrentState(device.LongDisplayName, device.IsDefault ? 1 : 0);
            }

            this.ParametersChanged();
            this.ActionImageChanged(null);
        }

        private void OnDeviceListChanged(Object sender, OutputDevicesEventArgs e) => this.UpdateParameters();

        private void OnDefaultDeviceChanged(Object sender, OutputDefaultDeviceEventArgs e)
        {
            this.ActionImageChangedByDeviceId(e.OldDeviceId, false);
            this.ActionImageChangedByDeviceId(e.NewDeviceId, true);
        }

        private void ActionImageChangedByDeviceId(String deviceId, Boolean isDefault)
        {
            if (this._actionParameters.TryGetValueSafe(deviceId, out var actionParameter))
            {
                this.SetCurrentState(actionParameter, isDefault ? 1 : 0);
                this.ActionImageChanged(actionParameter);
            }
        }
    }
}
