namespace lib.LowLevel.FlightSim
{
    public enum EEvent
    {
        /// <summary>
        /// Use this to cast event id's to enum
        /// 
        /// i.e 
        /// EEvents.Base + myEventId
        /// </summary>
        Base = 9

    };

    public static class EventId
    {
        private static int current = (int)EEvent.Base;

        /// <summary>
        /// For globally unique event id's.
        /// 
        /// Not thread safe.
        /// </summary>
        public static EEvent Next()
        {
            current += 1;
            return EEvent.Base + current;
        }
    }
}
