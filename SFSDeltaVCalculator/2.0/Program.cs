using System;
using System.Collections.Generic;

namespace SFSDeltaVCalculator
{
    class locationDestinationPair
    {
        public string location, destination;
    }
    class Stage
    {
        public float wetMass, dryMass, deltaV;
        public List<Engine> engines = new List<Engine>();
    }
    class Engine
    {
        public float ISP, thrust;
        public string name;
    }
    class planetParameters
    {
        public float sgp, orbitHeight, centralVelocity, centralHeight;
        public bool isMoon;
        public string moonOf;
        public Dictionary<int, string> selectableDestinations = new Dictionary<int, string>();
    }
    class Program
    {
        public static Dictionary<int, Engine> defaultEngines = new Dictionary<int, Engine>()
        {
            {1, new Engine{name = "Hawk Engine", ISP = 240, thrust = 120}},
            {2, new Engine{name = "Valiant Engine", ISP = 280, thrust = 40}},
            {3, new Engine{name = "Kolibri Engine", ISP = 260, thrust = 15}},
            {4, new Engine{name = "Titan Engine", ISP = 240, thrust = 400}},
            {5, new Engine{name = "Frontier Engine", ISP = 260, thrust = 15}},
            {6, new Engine{name = "ION Engine", ISP = 1020, thrust = 1.5f}}
        };
        public static Dictionary<string, planetParameters> defaultPlanets = new Dictionary<string, planetParameters>()
        {
            {"Mercury", new planetParameters{
                sgp = 47088000000,
                orbitHeight = 137500,
                centralVelocity = 8920.256f,
                centralHeight = 3117500000,
                isMoon = false
            }},
            {"Venus", new planetParameters{
                sgp = 798300000000,
                orbitHeight = 350000,
                centralVelocity = 6969,
                centralHeight = 5107500000,
                isMoon = false
            }},
            {"Earth", new planetParameters{
                sgp = 972410000000,
                orbitHeight = 355000,
                centralVelocity = 5758.770f,
                centralHeight = 7480000000,
                isMoon = false
            }},
            {"The Moon", new planetParameters{
                sgp = 10625000000,
                orbitHeight = 106500,
                centralVelocity = 333,
                centralHeight = 21925000,
                isMoon = true,
                moonOf = "Earth"
            }},
            {"Mars", new planetParameters{
                sgp = 103720000000,
                orbitHeight = 199500,
                centralVelocity = 4665.771f,
                centralHeight = 11395000000,
                isMoon = false
            }},
            {"Phobos", new planetParameters{
                sgp = 3132000,
                orbitHeight = 23000,
                centralVelocity = 278,
                centralHeight = 3350000,
                isMoon = true,
                moonOf = "Mars"
            }},
            {"Deimos", new planetParameters{
                sgp = 570380,
                orbitHeight = 19750,
                centralVelocity = 161,
                centralHeight = 9925000,
                isMoon = true,
                moonOf = "Mars"
            }},
            {"Jupiter", new planetParameters{
                sgp = 306250000000000,
                orbitHeight = 3500009900000000000,
                centralVelocity = 2524.447f,
                centralHeight = 38942500000000000,
                isMoon = false
            }},
            {"Io", new planetParameters{
                sgp = 63773000000,
                orbitHeight = 141500,
                centralVelocity = 4260.948f,
                centralHeight = 16868000,
                isMoon = true,
                moonOf = "Jupiter"
            }},
            {"Europa", new planetParameters{
                sgp = 25093000000,
                orbitHeight = 124000,
                centralVelocity = 3378.152f,
                centralHeight = 67090000,
                isMoon = true,
                moonOf = "Jupiter"
            }},
            {"Ganymede", new planetParameters{
                sgp = 179150000000,
                orbitHeight = 1770600,
                centralVelocity = 2674,
                centralHeight = 107040000,
                isMoon = true,
                moonOf = "Jupiter"
            }},
            {"Callisto", new planetParameters{
                sgp = 109910000000,
                orbitHeight = 181000,
                centralVelocity = 2018,
                centralHeight = 188000000,
                isMoon = true,
                moonOf = "Jupiter"
            }}
        };
        public static Dictionary<int ,locationDestinationPair> tripList = new Dictionary<int ,locationDestinationPair>();
        public static int selection, numOfSelection, stagesOfRocket = 0, currentStage = 0; // selection = selectNumber as an int, set after selected number is verified as valid. numOfSelection = used for the muber of entries in trip list.
        public static Dictionary<int, string> currentOrbit; // currentOrbit = the selectable locations to travel to, int is for number selection.
        public static string currentLocation;
        public static Engine selectionEngine = new Engine();
        public static Dictionary<int, Stage> allStages = new Dictionary<int, Stage>();
        public static float sunSGP = 99225000000000000, rocketDeltaV = 0, numOfEngines;
        public static Dictionary<string, planetParameters> allPlanets = new Dictionary<string, planetParameters>();
        public static Dictionary<int, Engine> engineTypes = new Dictionary<int, Engine>();
        // selectedNumber = the number typed by the user during selection of a planet / moon, e.g "4".







        static void Main(string[] args)
        {
            Console.Title = "SFS Delta V calculator";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("Welcome to the SFS Delta V calculator, made by u/pixelgaming579.");
            Console.WriteLine("Use this tool to calculate the rough required delta V to transfer between planets,\nAs well as calsculating how much DeltaV your rocket has.");
            Console.WriteLine("This tool doesn't include landings and taking off\nbecause differences in drag and landing technique would change the Delta V significantly.");
            Console.WriteLine("This tool also bases transfers on circular orbits.\nCircular orbits around planets and moons are measured as 15km above time acceleration height.");
            Console.WriteLine("\nDo you want to import a custom solar system,\nuse the default solar system, or calculate how much DeltaV your rocket has?");
            Console.WriteLine("\n1 - custom solar system\n2 - default solar system\n3 - calculate how much DeltaV your rocket has - custom engines\n4 - calculate how much DeltaV your rocket has - default engines");
            selectProgramType(Console.ReadLine());
        }









        static void selectProgramType(string typeSelection)
        {
            // Planets
            if (typeSelection == "1")
            {
                // Add file reading to set allPlanets to. For now will continue to use default system.
                Console.WriteLine("\nCustom systems will be added later. Starting from LEO, go to?\n");
                allPlanets = defaultPlanets;
                foreach (KeyValuePair<string, planetParameters> planet in allPlanets)
                {
                    planet.Value.selectableDestinations = selectablePlanets(planet.Key);
                }
                currentLocation = "Earth";
                currentOrbit = allPlanets["Earth"].selectableDestinations;
                WriteDestinations();
                SelectDestination(Console.ReadLine());
            }
            else if (typeSelection == "2")
            {
                Console.WriteLine("\nStarting from LEO, go to?\n");
                allPlanets = defaultPlanets;
                foreach (KeyValuePair<string, planetParameters> planet in allPlanets)
                {
                    planet.Value.selectableDestinations = selectablePlanets(planet.Key);
                }
                currentLocation = "Earth";
                currentOrbit = allPlanets["Earth"].selectableDestinations;
                WriteDestinations();
                SelectDestination(Console.ReadLine());
            }







            // Rockets
            else if (typeSelection == "3")
            {
                Console.WriteLine("Custom engines will be added later. how many stages does your rocket have?");
                engineTypes = defaultEngines;
                GetStages(Console.ReadLine());
                for (int stage = 1; stage <= stagesOfRocket; stage++)
                {
                    allStages.Add(stage, new Stage());
                }
                foreach (KeyValuePair<int, Stage> stage in allStages)
                {
                    Console.WriteLine("How much wet mass does stage " + stage.Key + " have?\nThe wet mass is the mass of the rocket including fuel, found at the bottom of the screen in build mode.");
                    GetWetMass(Console.ReadLine(), stage.Value);

                    Console.WriteLine("What is the dry mass of stage " + stage.Key + "?\nThe dry mass is the mass of the rocket excluding fuel, found at the bottom of the screen in build mode after you remove the fuel from the fuel tanks.");
                    GetDryMass(Console.ReadLine(), stage.Value.wetMass, stage.Value);

                    Console.WriteLine("What engines does stage " + stage.Key + " have? More engines can be added later.");
                    WriteEngines();
                    FindEngine(Console.ReadLine());

                    Console.WriteLine("How many " + selectionEngine.name + "/s does the stage have?");
                    NumOfEngines(Console.ReadLine());
                    for (int amount = 1; amount <= numOfEngines; amount++)
                    {
                        stage.Value.engines.Add(selectionEngine);
                    }

                    Console.WriteLine("Add more/different engines?\n1 - No\n2 - Yes");
                    AskForMoreEngines(Console.ReadLine(), stage.Value, stage.Key);
                }
                CalcFinalRocketDeltaV();
            }
            else
            {
                Console.WriteLine("Enter a valid selection.");
                selectProgramType(Console.ReadLine());
            }
        }












        static Dictionary<int, string> selectablePlanets(string current)
        {
            List<string> selectionsPlanets = new List<string>();
            List<string> selectionsMoons = new List<string>();
            Dictionary<int, string> selectionsSorted = new Dictionary<int, string>();
            int amount = 1;
            foreach (KeyValuePair<string, planetParameters> planet_moon in allPlanets)
            {
                if (planet_moon.Key != current)
                {
                    if (planet_moon.Value.isMoon) // if planet_moon is a moon.
                    {
                        if (planet_moon.Value.moonOf == current) // if planet_moon is a moon of current.
                        {
                            selectionsMoons.Add(planet_moon.Key);
                        }
                        else if (planet_moon.Value.moonOf == allPlanets[current].moonOf) // if planet_moon is a moon in the same system of current.
                        {
                            selectionsMoons.Add(planet_moon.Key);
                        }
                    }
                    else if (!planet_moon.Value.isMoon) // if planet_moon is a planet.
                    {
                        if (!allPlanets[current].isMoon) // if current is also a planet.
                        {
                            selectionsPlanets.Add(planet_moon.Key);
                        }
                        else if (allPlanets[current].isMoon && allPlanets[current].moonOf == planet_moon.Key) // if current is a moon, only add planet_moon if it is the central planet.
                        {
                            selectionsPlanets.Add(planet_moon.Key);
                        }
                    }
                }
            }
            foreach (string moon in selectionsMoons)
            {
                selectionsSorted.Add(amount, moon);
                amount ++;
            }
            foreach (string planet in selectionsPlanets)
            {
                selectionsSorted.Add(amount, planet);
                amount ++;
            }
            selectionsSorted.Add(amount, "End mission");
            // foreach (KeyValuePair<int, string> pair in selectionsSorted)
            // {
            //     Console.WriteLine(pair.Key + ": " + pair.Value);
            // }
            return selectionsSorted;
        }
        static void WriteDestinations()
        {
            foreach (KeyValuePair<int, string> location in currentOrbit)
            {
                Console.WriteLine(location.Key + " - " + location.Value);
            }
        }
        static void SelectDestination(string selectedNumber)
        {
            locationDestinationPair pair = new locationDestinationPair();
            selection = 0;
            while (selection == 0)
            {
                if (selectedNumber != null)
                {
                    foreach (KeyValuePair<int, string> location in currentOrbit)
                    {
                        if (selectedNumber == location.Key.ToString())
                        {
                            if (location.Value == "End mission")
                            {
                                Console.WriteLine("Calculating final Delta V...");
                                // Function for final Delta V calculation.
                                Console.WriteLine("final DeltaV = " + DeltaV().ToString() + "m/s");
                                selection = -1;
                            }
                            else
                            {
                                Console.WriteLine("Value accepted, going to " + location.Value + " ->\nNext location?");
                                selection = 1; // location acception key
                                // add entry to tripList
                                pair.location = currentLocation;
                                pair.destination = location.Value;
                                tripList.Add(numOfSelection, pair);
                                numOfSelection++;
                                
                                currentLocation = location.Value; // set currentLocation for use in next tripList entry.
                                currentOrbit = allPlanets[location.Value].selectableDestinations; // set currentOrbit for use in writing and selection of next desicion
                                WriteDestinations();
                                SelectDestination(Console.ReadLine());
                            }
                        }
                    }
                    if (selection == 0)
                    {
                        Console.WriteLine("Value denied, write the number next to the destination you want to go to and press enter.");
                        SelectDestination(Console.ReadLine());
                    }
                }
            }
        }
        static float DeltaV()
        {
            float deltaV = 0, r1, r2;
            planetParameters location = new planetParameters();
            planetParameters destination = new planetParameters();
            foreach (KeyValuePair<int ,locationDestinationPair> pair in tripList)
            {
                location = allPlanets[pair.Value.location];
                destination = allPlanets[pair.Value.destination];
                if (destination.isMoon == true && location.isMoon == true)
                {
                    if (location.centralHeight > destination.centralHeight)
                    {
                        r1 = destination.centralHeight;
                        r2 = location.centralHeight;
                    }
                    else
                    {
                        r1 = location.centralHeight;
                        r2 = destination.centralHeight;
                    }
                    // escape
                    deltaV += MathF.Sqrt((2 * location.sgp) / location.orbitHeight);
                    // transfer
                    deltaV += MathF.Sqrt(allPlanets[location.moonOf].sgp / r1) * (MathF.Sqrt((2 * r2) / (r1 + r2)) - 1);
                    // circularise
                    deltaV += MathF.Sqrt(destination.sgp / destination.orbitHeight);
                }
                else if (destination.isMoon == true)
                {
                    r1 = location.orbitHeight;
                    r2 = destination.centralHeight;
                    // escape
                    deltaV += MathF.Sqrt((2 * location.sgp) / location.orbitHeight);
                    // transfer
                    deltaV += MathF.Sqrt(location.sgp / r1) * (MathF.Sqrt((2 * r2) / (r1 + r2)) - 1);
                    // circularise
                    deltaV += MathF.Sqrt(destination.sgp / destination.orbitHeight);
                }
                else if (location.isMoon == true)
                {
                    r1 = location.centralHeight;
                    r2 = destination.orbitHeight;
                    // escape
                    deltaV += MathF.Sqrt((2 * location.sgp) / location.orbitHeight);
                    // transfer
                    deltaV += MathF.Sqrt(destination.sgp / r1) * (MathF.Sqrt((2 * r2) / (r1 + r2)) - 1);
                    // circularise
                    deltaV += MathF.Sqrt(destination.sgp / destination.orbitHeight);
                }
                else
                {
                    if (location.centralHeight > destination.centralHeight)
                    {
                        r1 = destination.centralHeight;
                        r2 = location.centralHeight;
                    }
                    else
                    {
                        r1 = location.centralHeight;
                        r2 = destination.centralHeight;
                    }
                    // escape
                    deltaV += MathF.Sqrt((2 * location.sgp) / location.orbitHeight);
                    // transfer
                    deltaV += MathF.Sqrt(sunSGP / r1) * (MathF.Sqrt((2 * r2) / (r1 + r2)) - 1);
                    // circularise
                    deltaV += MathF.Sqrt(destination.sgp / destination.orbitHeight);
                }
                Console.Write(pair.Key + ": current location: " + pair.Value.location + ", going to selection: " + pair.Value.destination);
                Console.WriteLine(" - " + deltaV.ToString() + " m/s");
            }
            return deltaV;
        }











        static int StringToInt(string input)
        {
            int output = 0;
            try
            {
                output = Convert.ToInt32(input);
            }
            catch (FormatException)
            {
                Console.WriteLine("Input is not a number.");
                StringToInt(Console.ReadLine());
            }
            catch (OverflowException)
            {
                Console.WriteLine("Number is too large.");
                StringToInt(Console.ReadLine());
            }
            return output;
        }
        static float StringToFloat(string input)
        {
            float output = 0;
            try
            {
                output = Convert.ToSingle(input);
            }
            catch (FormatException)
            {
                Console.WriteLine("Input is not a number.");
                StringToFloat(Console.ReadLine());
            }
            catch (OverflowException)
            {
                Console.WriteLine("Number is too large.");
                StringToFloat(Console.ReadLine());
            }
            return output;
        }
        static void GetStages(string input)
        {
            if (StringToInt(input) < 0.5)
            {
                Console.WriteLine("Number must be above 1.");
                GetStages(Console.ReadLine());
            }
            else
            {
                stagesOfRocket = StringToInt(input);
            }
        }
        static void GetWetMass(string input, Stage stage)
        {
            if (StringToFloat(input) <= 0)
            {
                Console.WriteLine("Number cannot be 0 or less.");
                GetWetMass(Console.ReadLine(), stage);
            }
            else
            {
                stage.wetMass = StringToFloat(input);
            }
        }
        static void GetDryMass(string input, float wetMass, Stage stage)
        {
            if (StringToFloat(input) <= 0)
            {
                Console.WriteLine("Number cannot be 0 or less.");
                GetDryMass(Console.ReadLine(), wetMass, stage);
            }
            else if (StringToFloat(input) >= wetMass)
            {
                Console.WriteLine("Number cannot be less than the wet mass.");
                GetDryMass(Console.ReadLine(), wetMass, stage);
            }
            else
            {
                stage.dryMass = StringToFloat(input);
            }
        }
        static void WriteEngines()
        {
            foreach (KeyValuePair<int, Engine> engine in engineTypes)
            {
                Console.WriteLine(engine.Key + " - " + engine.Value.name);
            }
        }
        static void FindEngine(string input)
        {
            int inputAsInt = StringToInt(input);
            try
            {
                selectionEngine = engineTypes[inputAsInt];
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine("Input is not a valid selection");
                FindEngine(Console.ReadLine());
            }
        }
        static void NumOfEngines(string input)
        {
            if (StringToInt(input) <= 0.5)
            {
                Console.WriteLine("Number cannot be 0 or less.");
                NumOfEngines(Console.ReadLine());
            }
            else
            {
                numOfEngines = StringToInt(input);
            }
        }
        static void AskForMoreEngines(string input, Stage stage, int key)
        {
            if (input == "2")
            {
                MoreEngines(stage, key);
            }
            else if (input != "1")
            {
                Console.WriteLine("Input is not valid.");
                AskForMoreEngines(Console.ReadLine(), stage, key);
            }
        }
        static void MoreEngines(Stage stage, int key)
        {
            Console.WriteLine("What engines does stage " + key + " have? More engines can be added later.");
            WriteEngines();
            FindEngine(Console.ReadLine());

            Console.WriteLine("How many " + selectionEngine.name + "/s does the stage have?");
            NumOfEngines(Console.ReadLine());
            for (int amount = 1; amount <= numOfEngines; amount++)
            {
                stage.engines.Add(selectionEngine);
            }
        }
        static void CalcFinalRocketDeltaV()
        {
            Console.WriteLine("Calculating final DeltaV...\n");
            foreach (KeyValuePair<int, Stage> stage in allStages)
            {
                float thrustOverISP = 0, combinedThrust = 0, finalISP = 0;
                foreach (Engine engine in stage.Value.engines)
                {
                    thrustOverISP += (engine.thrust / engine.ISP);
                    combinedThrust += engine.thrust;
                }
                finalISP = combinedThrust / thrustOverISP;
                Console.WriteLine("stage " + stage.Key + ": finalISP " + finalISP + ", combinedThrust " + combinedThrust + ", thrustOverISP " + thrustOverISP);
                stage.Value.deltaV = finalISP * 9.80665f * MathF.Log(stage.Value.wetMass / stage.Value.dryMass);
                Console.WriteLine("Stage " + stage.Key + ": " + stage.Value.deltaV + " m/s\n");
                rocketDeltaV += stage.Value.deltaV;
            }
            Console.WriteLine("\nFinal DeltaV = " + rocketDeltaV + " m/s.");
        }












    //     static float GetFloatThroughString(string input)
    //     {
    //         float output = 0;
    //         try
    //         {
    //             output = Convert.ToSingle(input);
    //         }
    //         catch (FormatException)
    //         {
    //             Console.WriteLine("Input is not a number.");
    //             GetFloatThroughString(Console.ReadLine());
    //         }
    //         catch (OverflowException)
    //         {
    //             Console.WriteLine("Number is too large.");
    //             GetFloatThroughString(Console.ReadLine());
    //         }
    //         return output;
    //     }
    //     static int GetIntThroughString(string input)
    //     {
    //         int output = 0;
    //         try
    //         {
    //             output = Convert.ToInt32(input);
    //         }
    //         catch (FormatException)
    //         {
    //             Console.WriteLine("Input is not a number.");
    //             GetIntThroughString(Console.ReadLine());
    //         }
    //         catch (OverflowException)
    //         {
    //             Console.WriteLine("Number is too large.");
    //             GetIntThroughString(Console.ReadLine());
    //         }
    //         return output;
    //     }
    //     static void AddEnginesToStage(Stage stage)
    //     {
    //         Console.WriteLine("Adding another engine, select an engine by typing it's number.");
    //         foreach (KeyValuePair<int, Engine> engine in engineTypes)
    //         {
    //             Console.WriteLine(engine.Key.ToString() + " - " + engine.Value.name);
    //         }
    //         Engine engineSelection = SearchForEngine(AskNewIfBelowNum(GetIntThroughString(Console.ReadLine()), 1));
    //         Engine selectedEngine = engineSelection;
    //         Console.WriteLine("Now type the number of those engines you want on the stage.");
    //         int amountOfEngines = AskNewIfBelowNum(GetIntThroughString(Console.ReadLine()), 1);
    //         for (int i = 0; i < amountOfEngines; i++)
    //         {
    //             stage.engines.Add(selectedEngine);
    //         }
    //         Console.WriteLine("Add more engines?");
    //         Console.WriteLine("1 - no\n2 - yes");
    //         AskMoreEngines(Console.ReadLine(), stage);
    //     }
    //     static void AskMoreEngines(string input, Stage stage)
    //     {
    //         if (GetIntThroughString(input) == 2)
    //         {
    //             AddEnginesToStage(stage);
    //         }
    //         else if (GetIntThroughString(input) != 1)
    //         {
    //             Console.WriteLine("Input is not valid.");
    //             AskMoreEngines(Console.ReadLine(), stage);
    //         }
    //     }
    //     static int AskNewIfBelowNum(int input, float num)
    //     {
    //         if (input < (num - 0.5f))
    //         {
    //             Console.WriteLine("Number is below 1.");
    //             int output = AskNewIfBelowNum(GetIntThroughString(Console.ReadLine()), num);
    //             return output;
    //         }
    //         else
    //         {
    //             return input;
    //         }
    //     }
    //     static float AskFloatNewIfBelowNum(float input, float num)
    //     {
    //         if (input < num)
    //         {
    //             Console.WriteLine("Number is below 1.");
    //             float output = AskFloatNewIfBelowNum(GetIntThroughString(Console.ReadLine()), num);
    //             return output;
    //         }
    //         else
    //         {
    //             return input;
    //         }
    //     }
    //     static Engine SearchForEngine(int input)
    //     {
    //         Engine output = new Engine();
    //         try
    //         {
    //             output = engineTypes[input];
    //         }
    //         catch (KeyNotFoundException)
    //         {
    //             Console.WriteLine("Input is not a valid selection.");
    //             SearchForEngine(AskNewIfBelowNum(GetIntThroughString(Console.ReadLine()), 1));
    //         }
    //         return output;
    //     }
    //     static float DryBelowWet(float wet, float dry)
    //     {
    //         if (dry > wet)
    //         {
    //             Console.WriteLine("Input must be below wet mass.");
    //             return DryBelowWet(wet, AskFloatNewIfBelowNum(GetIntThroughString(Console.ReadLine()), 1));
    //         }
    //         else
    //         {
    //             return dry;
    //         }
            
    //     }
    //     static void FinalRocketDeltaV()
    //     {
    //         float thrustOverISP = 0, combinedThrust = 0, finalISP = 0;
    //         foreach (KeyValuePair<int, Stage> stage in allStages)
    //         {
    //             foreach (Engine engine in stage.Value.engines)
    //             {
    //                 thrustOverISP += (engine.ISP / engine.thrust);
    //                 combinedThrust += engine.thrust;
    //             }
    //             finalISP = combinedThrust / thrustOverISP;
    //             Console.WriteLine(finalISP);
    //             stage.Value.deltaV = finalISP * 9.80665f * MathF.Log(stage.Value.wetMass / stage.Value.dryMass);
    //             rocketDeltaV += stage.Value.deltaV;
    //         }
    //     }
    }
}






//                 Console.WriteLine("Custom engines will be added later. How many stages does your rocket have?");
//                 engineTypes = defaultEngines;
//                 stagesOfRocket = AskNewIfBelowNum(GetIntThroughString(Console.ReadLine()), 1);
//                 for (int stage = 1; stage <= stagesOfRocket; stage++)
//                 {
//                     allStages.Add(stage, new Stage());
//                 }
//                 foreach (KeyValuePair<int, Stage> stage in allStages)
//                 {
//                     Console.WriteLine("What is the wet mass of stage " + stage.Key + "?\nThe wet mass is the mass of the rocket including fuel, found at the bottom of the screen in build mode.");
//                     stage.Value.wetMass = AskFloatNewIfBelowNum(GetFloatThroughString(Console.ReadLine()), 0);
//                     Console.WriteLine("What is the dry mass of stage " + stage.Key + "?\nThe dry mass is the mass of the rocket excluding fuel, found at the bottom of the screen in build mode after you remove the fuel from the fuel tanks.");
//                     stage.Value.dryMass = DryBelowWet(stage.Value.wetMass, AskFloatNewIfBelowNum(GetFloatThroughString(Console.ReadLine()), 0));
//                     Console.WriteLine("Stage: " + stage.Key + ", What engine/s does stage " + stage.Key + " have? Additional and different engines can be added after this.");
//                     // AddMoreEngines continues from here.
//                     foreach (KeyValuePair<int, Engine> engine in engineTypes)
//                     {
//                         Console.WriteLine(engine.Key.ToString() + " - " + engine.Value.name);
//                     }
//                     Engine engineSelection = SearchForEngine(AskNewIfBelowNum(GetIntThroughString(Console.ReadLine()), 1));
//                     Engine selectedEngine = engineSelection;
//                     Console.WriteLine("Now type the number of those engines you want on the stage.");
//                     int amountOfEngines = AskNewIfBelowNum(GetIntThroughString(Console.ReadLine()), 1);
//                     for (int i = 0; i < amountOfEngines; i++)
//                     {
//                         stage.Value.engines.Add(selectedEngine);
//                     }
//                     Console.WriteLine("Add more engines?");
//                     Console.WriteLine("1 - no\n2 - yes");
//                     AskMoreEngines(Console.ReadLine(), stage.Value);
//                     // foreach (Engine engine in stage.Value.engines)
//                     // {
//                     //     Console.WriteLine(engine.name + " - thrust: " + engine.thrust + " tonnes.");
//                     // }
//                 }
//                 Console.WriteLine("Calculating final DeltaV...");
//                 FinalRocketDeltaV();
//                 foreach (KeyValuePair<int, Stage> stage in allStages)
//                 {
//                     Console.WriteLine("\nDeltaV" + stage.Key + " = " + stage.Value.deltaV + " m/s.");
//                 }
//                 Console.WriteLine("\nFinal DeltaV = " + rocketDeltaV + " m/s.");