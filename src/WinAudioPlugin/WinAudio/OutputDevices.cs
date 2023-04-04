namespace NotADoctor99.WinAudioPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Loupedeck;
    using NAudio.CoreAudioApi;
    using NAudio.CoreAudioApi.Interfaces;

    public sealed class OutputDevices : IDisposable, IMMNotificationClient
    {
        private AutoPolicyConfigClientWin7 _policyConfigClient;

        private MMDeviceEnumerator _deviceEnumerator;

        private readonly DictionaryNoCase<OutputDevice> _devices = new DictionaryNoCase<OutputDevice>();

        private String _defaultDeviceId;

        public IEnumerable<OutputDevice> Devices => this._devices.Values;

        public event EventHandler<OutputDefaultDeviceEventArgs> DefaultDeviceChanged;

        public event EventHandler<OutputDevicesEventArgs> DeviceListChanged;

        public OutputDevices()
        {
        }

        public void Dispose() => this.Stop();

        public Boolean Start()
        {
            try
            {
                this._deviceEnumerator = new MMDeviceEnumerator();

                this.UpdateDevices();

                var result = this._deviceEnumerator.RegisterEndpointNotificationCallback(this);
                if (result != 0)
                {
                    PluginLog.Warning($"IMMDeviceEnumerator::RegisterEndpointNotificationCallback failed with error {result}");
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Cannot start OutputDevices");
            }

            return true;
        }

        public Boolean Stop()
        {
            if (this._deviceEnumerator != null)
            {
                try
                {
                    var result = this._deviceEnumerator.UnregisterEndpointNotificationCallback(this);
                    if (result != 0)
                    {
                        PluginLog.Warning($"IMMDeviceEnumerator::UnregisterEndpointNotificationCallback failed with error {result}");
                    }

                    try
                    {
                        this._deviceEnumerator.Dispose();
                    }
                    catch (Exception ex)
                    {
                        PluginLog.Error(ex, "Cannot dispose MMDeviceEnumerator");

                        // in case of exception in Dispose() method GC.SuppressFinalize() is not called by NAudio
                        GC.SuppressFinalize(this._deviceEnumerator);
                    }
                }
                catch (Exception ex)
                {
                    PluginLog.Error(ex, "Cannot stop OutputDevices");
                }

                this._deviceEnumerator = null;
            }

            return true;
        }

        public OutputDevice GetDefaultDevice() => this.TryGetDevice(this._defaultDeviceId, out var defaultDevice) ? defaultDevice : null;

        public Boolean TryGetDevice(String deviceId, out OutputDevice device) => this._devices.TryGetValueSafe(deviceId, out device);

        public IEnumerable<OutputDevice> EnumerateDevices() => this._devices.Values;

        public Boolean SetDefaultDevice(String deviceId)
        {
            try
            {
                if (null == this._policyConfigClient)
                {
                    this._policyConfigClient = new AutoPolicyConfigClientWin7();
                }

                this._policyConfigClient.SetDefaultEndpoint(deviceId, Role.Multimedia);
                this._policyConfigClient.SetDefaultEndpoint(deviceId, Role.Communications);

                return true;
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Cannot set default output device");
                return false;
            }
        }

        public Boolean MuteAllDevices()
        {
            try
            {
                var devices = this._deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

                foreach (var device in devices)
                {
                    device.AudioEndpointVolume.Mute = true;
                }

                return true;
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Cannot mute all output devices");
                return false;
            }
        }

        private void UpdateDevices()
        {
            try
            {
                var stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                this._devices.Clear();

                var defaultDevice = this._deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                this._defaultDeviceId = defaultDevice.ID;

                var devices = this._deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
                this._devices.AddRange(devices.ToDictionary(device => device.ID, device => new OutputDevice(device, this._defaultDeviceId)));

                this.DeviceListChanged?.BeginInvoke(this, new OutputDevicesEventArgs());

                stopwatch.Stop();
                System.Diagnostics.Trace.WriteLine($"--- {stopwatch.Elapsed.TotalMilliseconds:N0} ms");
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Cannot update output devices");
            }
        }

        // IMMNotificationClient

        public void OnDefaultDeviceChanged(DataFlow dataFlow, Role role, String newDefaultDeviceId)
        {
            try
            {
                if ((DataFlow.Render == dataFlow) && (Role.Multimedia == role))
                {
                    PluginLog.Info($"IMMNotificationClient::OnDefaultDeviceChanged {dataFlow} {role} '{newDefaultDeviceId}'");

                    var oldDefaultDeviceId = this._defaultDeviceId;
                    this._defaultDeviceId = newDefaultDeviceId;

                    foreach (var device in this.Devices)
                    {
                        device.SetDefaultDevice(newDefaultDeviceId);
                    }

                    this.DefaultDeviceChanged?.BeginInvoke(this, new OutputDefaultDeviceEventArgs(oldDefaultDeviceId, newDefaultDeviceId));
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Error in IMMNotificationClient::OnDefaultDeviceChanged handling");
            }
        }

        public void OnDeviceAdded(String deviceId)
        {
        }

        public void OnDeviceRemoved(String deviceId)
        {
        }

        public void OnDeviceStateChanged(String deviceId, DeviceState newState)
        {
        }

        public void OnPropertyValueChanged(String deviceId, PropertyKey propertyKey)
        {
        }
    }
}
