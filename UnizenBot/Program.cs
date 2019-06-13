using CommandLine;
using UnizenBot.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization.NamingConventions;

namespace UnizenBot
{
    /// <summary>
    /// The main class of the program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The entry point to the program.
        /// </summary>
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(StartBot)
                .WithNotParsed(HandleErrors);
        }

        /// <summary>
        /// Starts the bot after a successful startup with the provided options.
        /// </summary>
        /// <param name="options">The command line options.</param>
        public static void StartBot(CommandLineOptions options)
        {
            IStructuredStorage storage;
            string configFileExtension = options.ConfigPath.Substring(options.ConfigPath.LastIndexOf('.') + 1).ToLower();
            switch (configFileExtension)
            {
                case "yml":
                    {
                        storage = new YamlStorage(new UnderscoredNamingConvention());
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Invalid configuration file specified! Only YAML (.yml) storage is currently supported.");
                        return;
                    }
            }
            Bot bot = new Bot();
            using (FileStream stream = File.OpenRead(options.ConfigPath))
            {
                bot.Configure(options.ConfigPath, storage.Load<BotSettings>(stream));
            }
            bot.Start().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Handles command line errors on startup.
        /// </summary>
        /// <param name="errors">The errors.</param>
        public static void HandleErrors(IEnumerable<Error> errors)
        {
            // Nothing for now.
        }
    }

    /// <summary>
    /// Provided command line options.
    /// </summary>
    public class CommandLineOptions
    {
        /// <summary>
        /// The bot configuration file.
        /// </summary>
        [Option('c', "config", Default = "config.yml", HelpText = "The bot configuration file.")]
        public string ConfigPath { get; set; }
    }
}
