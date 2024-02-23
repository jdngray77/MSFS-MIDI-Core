using NAudio.Midi;
using System;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;
using Windows.Foundation;

namespace lib.LowLevel.Midi
{
    public class MidiDevice : IDisposable
    {
        public MidiInCapabilities Device { get; private set; }
        public MidiIn MidiIn { get; private set; }

        public MidiDevice(MidiInCapabilities device, MidiIn midiIn)
        {
            Device = device;
            MidiIn = midiIn;
        }

        public override string ToString()
        {
            return Device.ProductName;
        }

        public void Dispose()
        {
            MidiIn.Dispose();
        }
    }
}
