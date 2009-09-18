﻿using System;
using System.IO;
using System.Reflection;
using TiviT.NCloak.CloakTasks;

namespace TiviT.NCloak.Console
{
    public class CommandLineArgumentParser
    {
        private readonly InitialisationSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineArgumentParser"/> class.
        /// </summary>
        public CommandLineArgumentParser()
        {
            settings = new InitialisationSettings();
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public InitialisationSettings Settings
        {
            get { return settings; }
        }

        /// <summary>
        /// Parses the specified command line arguments.
        /// </summary>
        /// <param name="args">The args.</param>
        public void Parse(string[] args)
        {
            //If we have no arguments then get out of here
            if (args == null)
                return;

            //Go through and parse the arguments
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-") || args[i].StartsWith("/"))
                {
                    //Standard argument
                    switch (args[i].Substring(1))
                    {
                        case "out":
                            //Set the output directory
                            if (i + 1 < args.Length)
                                SetOutputDirectory(args[++i]);
                            else
                                DisplayError("Unrecognised number of arguments for out parameter");
                            break;
                        
                        case "full":
                            //Apply full obfuscation to public members
                            settings.ObfuscateAllModifiers = true;
                            break;

                        default:
                            DisplayError("Unrecognised command line option");
                            break;
                    }
                }
                else
                {
                    //Likely to be an assembly to obfuscate
                    settings.AssembliesToObfuscate.Add(args[i]);
                }
            }
            
            //If we haven't set the output directory then do it automatically
            if (String.IsNullOrEmpty(Settings.OutputDirectory))
            {
                //Set the output directory to the current directory + Obfuscated
                string currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (String.IsNullOrEmpty(currentDir))
                {
                    System.Console.Error.WriteLine("Unable to determine current directory...");
                    Environment.Exit(1);
                    return;
                }

                //Build the output directory
                string outputDir = Path.Combine(currentDir, "Obfuscated");
                SetOutputDirectory(outputDir);
            }
        }

        /// <summary>
        /// Outputs an initialisation error.
        /// </summary>
        /// <param name="message">The message.</param>
        private static void DisplayError(string message)
        {
            System.Console.WriteLine(message);
            Environment.Exit(1);
        }

        private void SetOutputDirectory(string outputDir)
        {
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            //Set it
            Settings.OutputDirectory = outputDir;
        }

        /// <summary>
        /// Configures the specified manager.
        /// </summary>
        /// <param name="manager">The manager.</param>
        public void Configure(CloakManager manager)
        {
            //For now we'll just register the basic tasks
            manager.RegisterTask<MappingTask>();
            manager.RegisterTask<ObfuscationTask>();
        }
    }
}