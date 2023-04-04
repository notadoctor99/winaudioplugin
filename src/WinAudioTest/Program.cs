namespace NotADoctor99.WinAudioPlugin
{
    using System;
    using System.Diagnostics;

    internal class Program
    {
        static void Main(String[] _)
        {
            var outputDevices = new OutputDevices();
            outputDevices.DefaultDeviceChanged += OnDefaultOutputDeviceChanged;
            outputDevices.DeviceListChanged += OutputDeviceListChanged;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            outputDevices.Start();

            Console.WriteLine($"> Start in {stopwatch.Elapsed.TotalMilliseconds:N0} ms");

            String device1Id = null;
            String device2Id = null;

            foreach (var device in outputDevices.EnumerateDevices())
            {
                Console.WriteLine($"{device.LongDisplayName} | {device.Id}");

                if (null == device1Id)
                {
                    device1Id = device.Id;
                }
                else
                {
                    device2Id = device2Id ?? device.Id;
                }
            }

            var defaultDevice = outputDevices.GetDefaultDevice();
            Console.WriteLine($"* {defaultDevice.LongDisplayName}");

            outputDevices.SetDefaultDevice(device1Id);
            System.Threading.Thread.Sleep(1_000);

            outputDevices.SetDefaultDevice(defaultDevice.Id);
            System.Threading.Thread.Sleep(1_000);

            outputDevices.SetDefaultDevice(device2Id);
            System.Threading.Thread.Sleep(1_000);

            outputDevices.SetDefaultDevice(defaultDevice.Id);
            System.Threading.Thread.Sleep(1_000);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey(true);

            outputDevices.Stop();
            outputDevices.DefaultDeviceChanged -= OnDefaultOutputDeviceChanged;
            outputDevices.DeviceListChanged += OutputDeviceListChanged;
        }

        private static void OnDefaultOutputDeviceChanged(Object sender, OutputDefaultDeviceEventArgs e) => Console.WriteLine($"* {(sender as OutputDevices)?.GetDefaultDevice().LongDisplayName}");

        private static void OutputDeviceListChanged(Object sender, OutputDevicesEventArgs e) => Console.WriteLine($"? Device list changed");
    }
}
