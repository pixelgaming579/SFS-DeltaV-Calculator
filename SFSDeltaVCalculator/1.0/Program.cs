using System;
using System.Collections.Generic;
using System.Linq;

namespace SFSDeltaVCalculator
{
    class locationDestinationPair
    {
        public string location, destination;
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
        public static int selection, numOfSelection; // selection = selectNumber as an int, set after selected number is verified as valid. numOfSelection = used for the muber of entries in trip list.
        public static Dictionary<int, string> currentOrbit; // currentOrbit = the selectable locations to travel to, int is for number selection.
        public static string currentLocation; // 
        public static float sunSGP = 99225000000000000;
        public static Dictionary<string, planetParameters> allPlanets = new Dictionary<string, planetParameters>();
        // selectedNumber = the number typed by the user during selection of a planet / moon, e.g "4".
        static void Main(string[] args)
        {
            Console.Title = "SFS Delta V calculator";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("Welcome to the SFS Delta V calculator, made by u/pixelgaming579.");
            Console.WriteLine("Use this tool to calculate the minimum required delta V to transfer between planets.");
            Console.WriteLine("This tool doesn't include landings and taking off\nbecause differences in drag and landing technique would change the Delta V significantly.");
            Console.WriteLine("This tool also bases transfers on circular orbits.\nCircular orbits around planets and moons are measured as 15km above time acceleration height.");
            Console.WriteLine("\nDo you want to import a custom solar system,\nuse the default solar system, or calculate how much DeltaV your rocket has?");
            Console.WriteLine("\n1 - custom solar system\n2 - default solar system");
            selectSystemType(Console.ReadLine());
            WriteDestinations();
            SelectDestination(Console.ReadLine());
        }
        static void selectSystemType(string typeSelection)
        {
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
            }
            else
            {
                Console.WriteLine("Enter a valid selection.");
                selectSystemType(Console.ReadLine());
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
    }
}