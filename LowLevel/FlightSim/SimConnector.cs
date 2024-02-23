using System;
using System.Collections.Generic;
using System.Linq;
using CTrue.FsConnect;

namespace lib.LowLevel.FlightSim
{
    public partial class SimConnector : IDisposable
    {
        private readonly SimConnectFileLocation simConnectFileLocation = SimConnectFileLocation.Local;
        private readonly FsConnect fsConnect;
        private readonly string clientName;
        private readonly string simUri;
        private readonly uint simPort;
        private List<SimEventCommand> registeredMappings = new List<SimEventCommand>();

        public SimConnector(string clientName, string simUri, uint simPort)
        {
            this.clientName = clientName;
            this.simUri = simUri;
            this.simPort = simPort;

            fsConnect = new FsConnect();

            // Specify where the SimConnect.cfg should be written to
            fsConnect.SimConnectFileLocation = simConnectFileLocation;

            fsConnect.FsDataReceived += OnSimSentData;
            fsConnect.FsError += OnSimError;
        }

        public void RegisterEventMapping(EEventGroup eventGroup, SimEventCommand mapping)
        {
            if (HasMapping(mapping))
            {
                return;
            }

            registeredMappings.Add(mapping);
            fsConnect.MapClientEventToSimEvent(eventGroup, mapping.Event, mapping.SimEventName);
            fsConnect.SetNotificationGroupPriority(eventGroup);
        }

        public void RaiseEvent(SimEventCommand @event, EEventGroup eventGroup)
        {
            RaiseEvent(@event, eventGroup, 0);
        }

        public void RaiseEvent(SimEventCommand @event, EEventGroup eventGroup, uint parameter)
        {
            Console.WriteLine($"Raising event {@event} with parameter {parameter}");
            fsConnect.TransmitClientEvent(@event.Event, @event.Data, eventGroup);
        }

        public bool HasMapping(SimEventCommand mapping)
        {
            return registeredMappings.Any(m => m.Equals(mapping));
        }

        /// <summary>
        /// Occours when the simulator sends the client some data.
        /// </summary>
        private void OnSimError(object sender, FsErrorEventArgs e)
        {
            Console.WriteLine($"The simulator responded with an error : '{e.ExceptionDescription}'");
        }

        /// <summary>
        /// Occours when the simulator sends the client an error in response to something we've done.
        /// </summary>
        private static void OnSimSentData(object sender, FsDataReceivedEventArgs e)
        {
        }

        public void Dispose()
        {
            Disconnect();
        }

        internal void Connect()
        {
            // Creates a SimConnect.cfg and connect to Flight Simulator using this configuration.
            fsConnect.Connect(clientName, simUri, simPort, SimConnectProtocol.Ipv4);
        }

        internal void Disconnect()
        {
            fsConnect.Disconnect();
        }
    }
}
