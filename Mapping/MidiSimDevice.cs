using lib.LowLevel.FlightSim;
using lib.LowLevel.Midi;
using NAudio.Midi;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace lib.Mapping
{
    /// <summary>
    /// A midi device that can map events to the sim.
    /// </summary>
    public class MidiSimDevice : IDisposable
    {
        EventMappingManager eventMapper;
        public EEventGroup Group { get; }
        public MidiDevice MidiDevice { get; }

        private bool enabled;
        public bool Enabled
        {
            get
            {
                return enabled;
            }

            set
            {
                if (value)
                {
                    MidiDevice.MidiIn.Start();
                }
                else
                {
                    MidiDevice.MidiIn.Stop();
                }

                enabled = value;
            }
        }

        public MidiSimDevice(
            MidiDevice midiDevice,
            EventMappingManager eventMapper)
        {
            this.MidiDevice = midiDevice;
            this.Group = EventGroup.Next();

            this.eventMapper = eventMapper;
            midiDevice.MidiIn.MessageReceived += MidiIn_MessageReceived;
        }
     
        private void MidiIn_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            Console.WriteLine($"{MidiDevice.Device.ProductName} sent [{e.RawMessage}]");
            eventMapper.TryRaise(MidiDevice.Device.ProductName, e.MidiEvent);
        }

        public override string ToString()
        {
            return MidiDevice.Device.ProductName;
        }

        public void Dispose()
        {
            MidiDevice.Dispose();
            MidiDevice.MidiIn.MessageReceived -= MidiIn_MessageReceived;
        }
    }
}
