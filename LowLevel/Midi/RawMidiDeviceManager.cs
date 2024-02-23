using lib.Configuration;
using NAudio.Midi;
using System.Collections.Generic;

namespace lib.LowLevel.Midi
{
    /// <summary>
    /// For internal use only
    /// 
    /// Manages fetching raw MIDI devices from NAudio.
    /// 
    /// Access via MidiSimDeviceManager.
    /// </summary>
    internal static class RawMidiDeviceManager
    {
        private static List<MidiDevice> midiDevices = null;
        public static List<MidiDevice> MidiDevices 
        { 
            get
            {
                if (midiDevices == null)
                {
                    midiDevices = EnumerateMidiDevices();
                }
                else
                {
                    RefreshMidiDevices();
                }

                return midiDevices;
            }
        }

        private static List<MidiDevice> EnumerateMidiDevices()
        {
            var devices = new List<MidiDevice>();
            for (int device = 0; device < MidiIn.NumberOfDevices; device++)
            {
                devices.Add(new MidiDevice(MidiIn.DeviceInfo(device), new MidiIn(device)));
            }

            return devices;
        }

        private static void RefreshMidiDevices()
        {
            if (MidiIn.NumberOfDevices == midiDevices.Count) 
            {
                return;
            }

            for (int device = midiDevices.Count - 1; device < MidiIn.NumberOfDevices; device++)
            {
                midiDevices.Add(new MidiDevice(MidiIn.DeviceInfo(device), new MidiIn(device)));
            }
        }


    }
}
