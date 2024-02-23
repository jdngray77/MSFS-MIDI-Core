using lib.LowLevel.FlightSim;
using NAudio.Midi;

namespace lib.Mapping
{
    public class MidiEventToSimCommandMapping
    {
        public ISimCommand SimCommand { get; }
        public MidiEvent MidiEvent { get; }

        public EEventGroup Group { get; internal set; } = EEventGroup.Default;

        public MidiEventToSimCommandMapping(MidiEvent midiEvent, ISimCommand simCommand)
        {
            SimCommand = simCommand;
            MidiEvent = midiEvent;
        }

        public MidiEventToSimCommandMapping(MidiEvent midiEvent, ISimCommand simCommand, EEventGroup group)
        {
            SimCommand = simCommand;
            MidiEvent = midiEvent;
            Group = group;
        }

        public override string ToString()
        {
            return $"{MidiEvent} -> {SimCommand}";
        }
    }
}
