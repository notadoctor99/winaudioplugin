namespace NotADoctor99.WinAudioPlugin
{
    using System;
    using System.Runtime.InteropServices;

    using NAudio.CoreAudioApi;
    using NAudio.CoreAudioApi.Interfaces;

    // https://github.com/File-New-Project/EarTrumpet/blob/master/EarTrumpet/Interop/MMDeviceAPI/IPolicyConfig.cs

    [Guid("F8679F50-850A-41CF-9C72-430F290290C8")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPolicyConfigWin7
    {
        void Unused1();
        void Unused2();
        void Unused3();
        void Unused4();
        void Unused5();
        void Unused6();
        void Unused7();
        void Unused8();
        void GetPropertyValue([MarshalAs(UnmanagedType.LPWStr)] String wszDeviceId, ref PropertyKey pkey, ref PropVariant pv);
        void SetPropertyValue([MarshalAs(UnmanagedType.LPWStr)] String wszDeviceId, ref PropertyKey pkey, ref PropVariant pv);
        void SetDefaultEndpoint([MarshalAs(UnmanagedType.LPWStr)] String wszDeviceId, Role eRole);
        void SetEndpointVisibility([MarshalAs(UnmanagedType.LPWStr)] String wszDeviceId, [MarshalAs(UnmanagedType.I2)] Int16 isVisible);
    }

    // https://github.com/File-New-Project/EarTrumpet/blob/master/EarTrumpet/Interop/MMDeviceAPI/PolicyConfigClient.cs

    [ComImport]
    [Guid("870AF99C-171D-4F9E-AF0D-E63DF40C2BC9")]
    public class PolicyConfigClient { }

    public class AutoPolicyConfigClientWin7
    {
        readonly IPolicyConfigWin7 _policyClient = (IPolicyConfigWin7)new PolicyConfigClient();

        public void SetEndpointVisibility(String deviceId, Boolean isVisible) => this._policyClient.SetEndpointVisibility(deviceId, isVisible ? (Int16)1 : (Int16)0);

        public void SetDefaultEndpoint(String deviceId, Role role = Role.Multimedia) => this._policyClient.SetDefaultEndpoint(deviceId, role);
    }
}
