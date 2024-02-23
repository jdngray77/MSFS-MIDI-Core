namespace lib.LowLevel.FlightSim
{
    public enum EEventGroup
    {
        Base = 0,
        Default = Base + 1,
    }

    public static class EventGroup
    {
        private static int current = (int)EEventGroup.Default;

        /// <summary>
        /// For globally unique event id's.
        /// 
        /// Not thread safe.
        /// </summary>
        public static EEventGroup Next()
        {
            current += 1;
            return EEventGroup.Default + current;
        }
    }
}
