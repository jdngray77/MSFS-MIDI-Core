using lib.LowLevel.FlightSim;
using lib.LowLevel.Midi;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace lib.Mapping
{
    /// <summary>
    /// Manages midi devices that we can use with the sim.
    /// </summary>
    public class MidiSimDeviceManager : IDisposable
    {
        public ObservableCollection<MidiSimDevice> Devices { get; }

        private readonly EventMappingManager eventMappingManager;

        public MidiSimDeviceManager(SimConnector connector, EventMappingManager mapper)
        {
            eventMappingManager = new EventMappingManager(connector);
            Devices = new ObservableCollection<MidiSimDevice>(RawMidiDeviceManager.MidiDevices.Select(it => new MidiSimDevice(it, mapper)));
        }

        public void Dispose()
        {
            StopListening();
            foreach (var device in Devices)
            {
                device.Dispose();
            }
        }

        public void StartListening()
        {
            foreach (var device in Devices)
            {
                device.Enabled = true;
            }
        }

        public void StopListening()
        {
            foreach (var device in Devices)
            {
                device.Enabled = false;
            }
        }
    }
}