using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using PatchalyzerCore;
using PatchalyzerCore.Config.Repository;
using PatchalyzerCore.IO;
using Semver;

namespace PatchalyzerConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            if(Directory.Exists("..\\Tests\\ExampleRepo\\"))
                Directory.Delete("..\\Tests\\ExampleRepo\\", true);

            Patchalyzer proj = Patchalyzer.InitRepo("..\\Tests\\ExampleRepo\\", "ExampleRepo");
            proj.AddVersion(new SemVersion(1), "..\\Tests\\ExampleProject\\v1", "Initial Release", "The first release");
            proj.AddVersion(new SemVersion(1, 1), "..\\Tests\\ExampleProject\\v2", "Update", "We changed logo.png and added a new file");
            proj.SaveRepo();


            // Wait for user input
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}