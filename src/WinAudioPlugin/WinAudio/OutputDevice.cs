namespace NotADoctor99.WinAudioPlugin
{
    using System;

    using Loupedeck;
    using NAudio.CoreAudioApi;

    public class OutputDevice
    {
        public String Id { get; }

        public String LongDisplayName { get; }

        public String ShortDisplayName { get; }

        public Byte[] LargeIcon { get; }

        public Byte[] SmallIcon { get; }

        public Boolean IsDefault { get; private set; }

        public OutputDevice(MMDevice device, String defaultDeviceId)
        {
            this.Id = device.ID;
            this.LongDisplayName = device.FriendlyName;
            this.ShortDisplayName = device.DeviceFriendlyName;
            
            if (DeviceHelpers.ExtractIcon(device.IconPath, out var largeIconBytes, out var smallIconBytes))
            {
                this.LargeIcon = largeIconBytes;
                this.SmallIcon = smallIconBytes;
            }

            this.SetDefaultDevice(defaultDeviceId);
        }

        public void SetDefaultDevice(String defaultDeviceId) => this.IsDefault = this.IsSameAs(defaultDeviceId);

        public Boolean IsSameAs(String deviceId) => this.Id.EqualsNoCase(deviceId);
    }
}
