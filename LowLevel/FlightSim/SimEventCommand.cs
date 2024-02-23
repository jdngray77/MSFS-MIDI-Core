namespace lib.LowLevel.FlightSim
{
    /// <summary>
    /// Represents some kind of action that can be executed on the simulator.
    /// 
    /// Could be a string based event, or change to a variable, or something else.
    /// </summary>
    public interface ISimCommand
    {
        void Execute(SimConnector simulator, EEventGroup eventGroup);
        public void Execute(SimConnector simulator, EEventGroup eventGroup, uint data);
    }

    /// <summary>
    /// Represents a command that, when executed, sends a string identified event to the simulator.
    /// 
    /// Stores a simulator event identified by name, i.e "AP_ALT_VAR_INC",
    /// and a client defined EEvent ID that we can use to trigger that event.
    /// 
    /// Must be registered with the simulator with SimConnector.RegisterEventMapping before use.
    /// </summary>
    public class SimEventCommand : ISimCommand
    {
        public SimEventCommand(string command, EEvent @event)
        {
            SimEventName = command;
            Event = @event;
        }

        /// <summary>
        /// Maps to an event name in the simulator.
        /// 
        /// Must be a valid event name in the simulator.
        /// 
        /// Refer to MSFS documentation for base simulator events.
        /// 
        /// For custom events (such as custom events added by A32NX)
        /// reffer to documentation for the specific addon.
        /// </summary>
        public string SimEventName { get; }

        /// <summary>
        /// A client-defined int ID casted to an event enum.
        /// We can specify this to be anything we'd like, but it seems the value
        /// has to be greater than 8.
        /// </summary>
        public EEvent Event { get; }

        /// <summary>
        /// optional parameter that will be sent with the event.
        /// </summary>
        public virtual uint Data { get; protected set; } = 0;

        /// <summary>
        /// Sends the event to the simulator.
        /// </summary>
        public void Execute(SimConnector simulator, EEventGroup eventGroup)
        {
            Execute(eventGroup, simulator, Data);
        }

        /// <summary>
        /// Sends the event to the simulator.
        /// </summary>
        public void Execute(SimConnector simulator, EEventGroup eventGroup, uint data)
        {
            Execute(EEventGroup.Default, simulator, data);
        }

        /// <summary>
        /// Sends the event to the simulator.
        /// </summary>
        public void Execute(EEventGroup eventGroup, SimConnector simulator, uint data)
        {
            simulator.RaiseEvent(this, eventGroup, data);
        }

        public override bool Equals(object obj)
        {
            return obj is SimEventCommand mapping &&
                   Event == mapping.Event &&
                   SimEventName == mapping.SimEventName;
        }

        public override string ToString()
        {
            return $"{SimEventName}";
        }
    }

    public class SimEventWithStaticParamMapping : SimEventCommand
    {
        public SimEventWithStaticParamMapping(
            string command, 
            uint data, 
            EEvent @event) 
            : base(command, @event)
        {
            Data = data;
        }

        public override string ToString()
        {
            return $"{base.ToString()} : {Data}";
        }
    }
}
