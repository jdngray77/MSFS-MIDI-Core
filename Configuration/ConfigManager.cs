using lib.Configuration.SimCommands;
using lib.LowLevel.FlightSim;
using lib.Mapping;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Xml.Serialization;

namespace lib.Configuration
{
    public class ConfigManager
    {
        public ConfigFile LoadMappingFile(string path)
        {
            ConfigFile mappingFile;
            using (FileStream XmlConfig = File.OpenRead(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ConfigFile));
                mappingFile = (ConfigFile)serializer.Deserialize(XmlConfig);
            }

            ValidateMappingFile(mappingFile);

            return mappingFile;
        }

        public void ValidateMappingFile(ConfigFile mappingFile)
        {
            // TODO
        }

        public void SaveMappingFile(ConfigFile mappingFile, string path)
        {
            using (FileStream XmlConfig = File.OpenWrite(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ConfigFile));
                serializer.Serialize(XmlConfig, mappingFile);
            }
        }

        /// <summary>
        /// Applies the mapping file to the event mapping manager.
        /// </summary>
        /// <param name="eventMappingManager"></param>
        /// <param name="mappingFile"></param>
        public void ApplyConfiguration(
            EventMappingManager eventMappingManager,
            ConfigFile mappingFile)
        {
            // For every device
            foreach (var deviceMappingDefinition in mappingFile.Devices)
            {
                // Take each mapping
                foreach (var mapping in deviceMappingDefinition.Mappings)
                {
                    // Create object models from the data
                    // for both the midi trigger
                    NAudio.Midi.MidiEvent @event = NAudio.Midi.MidiEvent.FromRawMessage(mapping.MidiTrigger);

                    // and the resulting sim command
                    ISimCommand simCommand = CreateSimCommand(mapping.Result);

                    // Combine the trigger and result into a mapping
                    MidiEventToSimCommandMapping midiEventToSimCommandMapping = new MidiEventToSimCommandMapping(@event, simCommand);

                    // Then finally add it to the event mapping manager
                    eventMappingManager.AddMappingFor(deviceMappingDefinition.ProductName, midiEventToSimCommandMapping);
                }
            };
        }

        public ISimCommand CreateSimCommand(SimCommand desiredCommand)
        {
            if (desiredCommand.SimEvent != null)
            {
                return CreateSimEventCommand(desiredCommand.SimEvent);
            }

            // TODO LVar

            throw new Exception("Unknown sim command type");
        }

        public SimEventCommand CreateSimEventCommand(SimEvent desiredCommand)
        {
            if (desiredCommand.Parameter == null)
            {
                return new SimEventWithStaticParamMapping(
                    desiredCommand.Command,
                    0,
                    EventId.Next());
            }

            // Simulator event with static parameter
            if (desiredCommand.Parameter?.StaticParameter != null)
            {
                return new SimEventWithStaticParamMapping(
                    desiredCommand.Command,
                    desiredCommand.Parameter.StaticParameter.Value,
                    EventId.Next());
            }

            // Simulator event with dynamic parameter
            if (desiredCommand.Parameter is DynamicSimEventParameter)
            {
                throw new NotImplementedException("Dynamic parameters not implemented");
            }

            throw new Exception("Unknown sim event parameter type");
        }
    }
}
