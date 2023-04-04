namespace NotADoctor99.WinAudioPlugin
{
    using System;

    public class OutputDefaultDeviceEventArgs : EventArgs
    {

        public String OldDeviceId;

        public String NewDeviceId;

        public OutputDefaultDeviceEventArgs(String oldDeviceId, String newDeviceId)
        {
            this.OldDeviceId = oldDeviceId;
            this.NewDeviceId = newDeviceId;
        }
    }
}
