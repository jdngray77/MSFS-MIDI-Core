using NAudio.Midi;

namespace lib.LowLevel.Midi
{
    public static class Helper
    {
        public static bool MidiEventDataEquals(this MidiEvent a, MidiEvent b)
        {
            return a.GetAsShortMessage().Equals(b.GetAsShortMessage());
        }
    }
}
