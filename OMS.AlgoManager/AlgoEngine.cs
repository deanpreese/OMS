using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

using OMS.Core.Models;
using OMS.Core.Logging;

using OMS.AlgoManager.Algos;

namespace OMS.AlgoManager;

public class Engine
{

    internal class AlgoData
    {
        public string algoclass {get;set;}
        public string algoname {get;set;}
        public string longname {get; set;}
        public string shortname {get;set;}
        public int group {get;set;}
        public bool usedebugging {get;set;}
        public bool backtesting {get;set;}
    }
    string algosToLoad ;
    public List<IAlgo> AlgoList {get;set;}

    public Engine(string algoJson)
    {
        algosToLoad = algoJson;
        AlgoList = new List<IAlgo>();            
    }

    public void AddAlgos()
    {
        string jsonString = File.ReadAllText(algosToLoad);
        List<AlgoData> algos = JsonSerializer.Deserialize<List<AlgoData>>(jsonString);

        foreach( AlgoData a in algos)
        {
            Console.WriteLine("Loading " + a.algoclass);

            try
            {
                var algoClass = Type.GetType(a.algoclass + ",Algorithms", true);
                IAlgo algo = (IAlgo)Activator.CreateInstance(algoClass);

                algo.AlgoGroupNumber = a.group;
                algo.AlgoTraderLong = a.longname;
                algo.AlgoTraderShort = a.shortname;
                algo.UsingDebugging = a.usedebugging;
                algo.IsBackTesting = a.backtesting;
                
                //algo.SetupAlgoTraders(dwl, aom);

                AlgoList.Add(algo);

                Console.WriteLine("Long Name " + a.longname );
                Console.WriteLine("Short Name " + a.shortname );
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString()) ;
            }
        }
    }

}
