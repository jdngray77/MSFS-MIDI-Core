using lib.LowLevel.FlightSim;
using lib.LowLevel.Midi;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace lib.Mapping
{
    /// <summary>
    /// Handles storage and translation of midi events to sim events.
    /// 
    /// Handles execution of translated events.
    /// </summary>
    public class EventMappingManager
    {
        /// <summary>
        /// Connection to external simulator.
        /// </summary>
        private readonly SimConnector sim;

        public EventMappingManager(SimConnector sim)
        {
            this.sim = sim;
        }

        /// <summary>
        /// Mappings from midi events to sim commands.
        /// </summary>
        public Dictionary<string, List<MidiEventToSimCommandMapping>> Mappings = new Dictionary<string, List<MidiEventToSimCommandMapping>>();

        #region Event Execution
        public bool TryRaise(string deviceName, MidiEvent midiEvent)
        {
            if (TryGetMapping(deviceName, midiEvent, out var mapping))
            {
                Console.WriteLine($"Mapped {midiEvent.GetAsShortMessage()} to {mapping.SimCommand}.");
                mapping.SimCommand.Execute(sim, mapping.Group);
                return true;
            }

            Console.WriteLine("Unmapped.");
            return false;
        }

        #endregion

        #region Mapping

        /// <summary>
        /// Maps a raw midi tri byte to a string based sim event with a static parameter or no parameter.
        /// </summary>
        public void MapEvent(MidiSimDevice device, byte channel, byte command, byte data, string simcommand, string staticParameter)
        {
            MidiEvent @event = MidiEvent.FromRawMessage((int)channel + (int)command + (int)data);
            MapEvent(device, @event, simcommand, staticParameter);
        }

        /// <summary>
        /// Maps a midi event to a string based sim event with a static parameter or no parameter.
        /// </summary>
        public void MapEvent(MidiSimDevice device, MidiEvent midiEvent, string simcommand, string staticParam)
        {
            uint? param = null;
            if (!string.IsNullOrEmpty(staticParam))
            {
                param = uint.Parse(staticParam);
            }

            MapEvent(device, midiEvent, simcommand, param);
        }

        /// <summary>
        /// Maps a midi event to a string based sim event with a static parameter or no parameter.
        /// </summary>
        public void MapEvent(MidiSimDevice device, MidiEvent midiEvent, string simcommand, uint? staticParam)
        {
            SimEventCommand simEvent;
            if (staticParam == null)
            {
                simEvent = new SimEventCommand(simcommand, EEvent.Base + Mappings.Count);
            }
            else
            {
                simEvent = new SimEventWithStaticParamMapping(simcommand, staticParam.Value, EventId.Next());
            }

            MapEvent(device, midiEvent, simEvent);
        }

        /// <summary>
        /// Maps a midi event to any type of SimMapping.
        /// </summary>
        public void MapEvent(MidiSimDevice device, MidiEvent midiEvent, SimEventCommand SimEvent)
        {
            sim.RegisterEventMapping(device.Group, SimEvent);
            new MidiEventToSimCommandMapping(midiEvent, SimEvent, device.Group);
        }


        /// <summary>
        /// N.B : Only adds the mapping. Does not register the event with the sim
        /// as without the device we can't determine the group id.
        /// 
        /// Call EnsureAllMappingsRegistered later to register them in the sim aginst group id's.
        /// 
        /// Note that Map Event does not need EnsureAllMappingsRegistered to be called,
        /// as it knows about the midi device already.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="mapping"></param>
        public void AddMappingFor(string device, MidiEventToSimCommandMapping mapping)
        {
            GetMappingsFor(device).Add(mapping);
        }

        public void EnsureAllMappingsRegistered(MidiSimDeviceManager manager)
        {
            foreach (var key in Mappings.Keys)
            {
                var mappings = Mappings[key];
                var device = manager.Devices.First(d => d.MidiDevice.Device.ProductName == key);
                EEventGroup eventGroupId = device.Group;

                foreach (var mapping in mappings.Where(it => it.SimCommand is SimEventCommand))
                {
                    mapping.Group = device.Group;
                    sim.RegisterEventMapping(eventGroupId, mapping.SimCommand as SimEventCommand);
                }
            }
        }

        public List<MidiEventToSimCommandMapping> GetMappingsFor(string deviceName)
        {
            if (Mappings.TryGetValue(deviceName, out var map))
            {
                return map;
            } else
            {
                List<MidiEventToSimCommandMapping> mappings = new List<MidiEventToSimCommandMapping>();
                Mappings.Add(deviceName, mappings);
                return mappings;
            }
        }

        public MidiEventToSimCommandMapping GetMapping(string deviceName, MidiEvent @event)
        {
            return GetMappingsFor(deviceName).First(m => m.MidiEvent.MidiEventDataEquals(@event));
        }

        public bool TryGetMapping(string deviceName, MidiEvent @event, out MidiEventToSimCommandMapping mapping)
        {
            if (Mappings.TryGetValue(deviceName, out var map))
            {
                mapping = map.FirstOrDefault(m => m.MidiEvent.MidiEventDataEquals(@event));
                return mapping != null;
            }
            else
            {
                mapping = null;
                return false;
            }
        }

        #endregion
    }
}
