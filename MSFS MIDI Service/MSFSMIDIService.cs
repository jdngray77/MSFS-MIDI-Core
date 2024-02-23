using lib.Configuration;
using lib.LowLevel.FlightSim;
using lib.Mapping;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace lib.MSFS_MIDI_Service
{
    public class MSFSMIDIService : IDisposable
    {
        private Task Task;
        public bool IsRunning = false;

        public readonly ConfigManager configManager;
        public readonly ConfigFile configuration;
        public readonly SimConnector simConnector;
        public readonly EventMappingManager eventMapper;
        public readonly MidiSimDeviceManager deviceManager;

        public MSFSMIDIService()
        {
            // Midi input event flow:
            // MidiDevice -> MidiSimDeviceManager -> EventMappingManager -> SimConnector -> SimConnect -> FlightSim

            // Config manager handles loading of the configuration file.
            configManager = new ConfigManager();

            // Load the config file from disk.
            configuration = configManager.LoadMappingFile("./Properties/config.xml");

            Console.WriteLine("Loaded configuration : ");
            Console.WriteLine(configuration);

            // Set-up the sim connector.
            SimConnectConfig simConnectConfig = configuration.SimConnectConfig;
            simConnector = new SimConnector(                     // N.B : 'using', connection to sim
                simConnectConfig.ClientName,                     //       should be disposed of gracefully.
                simConnectConfig.Location,
                simConnectConfig.Port);

            // The event Mapping Manager stores the mappings and
            // forwards midi events to the simulator.
            eventMapper = new EventMappingManager(simConnector);


            // Configure the eventMapper with mappings so it knows what commands
            // to send for a given midi input.
            // However, does not yet register our events with the sim,
            // As 1) we need to know the devices to associate with event group id's
            // and 2) we're not connected with the sim until the service starts.
            configManager.ApplyConfiguration(eventMapper, configuration);

            // After it's conigured, we'll give it to the MidiSimDeviceManager
            // which will use it to handle midi inputs.
            deviceManager = new MidiSimDeviceManager(simConnector, eventMapper); // N.B : 'using', MIDI device connections
                                                                                 //         should be of disposed gracefully.
        }

        private void Run()
        {
            deviceManager.StartListening();

            // Connect to the sim and register our client data.
            simConnector.Connect();
            eventMapper.EnsureAllMappingsRegistered(deviceManager);

            while (IsRunning)
            {
                // Keep the service running.

                try
                {
                    Thread.Sleep(30000);
                }
                catch (ThreadInterruptedException)
                {
                    break;
                }
            }

            IsRunning = false;
            deviceManager.StopListening();
            simConnector.Disconnect();
        }

        public void Start()
        {
            if (Task != null)
            {
                return;
            }

            IsRunning = true;
            Task = new Task(Run);
            Task.Start();
        }

        public void Stop()
        {
            // Tell service to stop.
            IsRunning = false;

            // Wait for service to stop.
            while (!Task.IsCompleted);
        }

        public void Dispose()
        {
            deviceManager.StopListening();
            deviceManager.Dispose();
            simConnector.Dispose();
        }
    }
}
