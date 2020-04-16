using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.ExceptionServices;

using ThermoFisher.CommonCore.Data;
using ThermoFisher.CommonCore.Data.Business;
using ThermoFisher.CommonCore.Data.FilterEnums;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.MassPrecisionEstimator;
using ThermoFisher.CommonCore.RawFileReader;

namespace SGStatistic
{
    internal static class Program
    {

        public static void Main()
        {
            //TODO: add exception handling

            //(1) Input information to make the method file

            //Console.WriteLine("Welcome to OrbiMS Isotope Data Processor, aka SG_Statistic, aka Sarah-Guannan_Statistic!");
            //Console.WriteLine();
            //Console.WriteLine("Tell us something about your measurements.");
            //Console.WriteLine();
            //Console.WriteLine("Which type of measurement do you want to process?");
            //Console.WriteLine("Press '1' for a GC-reservoir measurement;");
            //Console.WriteLine("Press '2' for a ESI-syringe-pump measurement;");
            //Console.WriteLine("Press '3' for a GC-direct-elution measurement.");

            //int measurementTypeInput = Convert.ToInt16(Console.ReadLine());
            ////TODO: handle error input (other than 1-3)

            //Console.WriteLine("Now, let's talk about what you want to know from the measurements.");
            //Console.WriteLine();
            //Console.WriteLine("How many isotopologues are you interested in? Hint: ≥2");

            //int peakNumberInput = Convert.ToInt16(Console.ReadLine());
            ////TODO: handle error input <2

            //Console.WriteLine("Creating a method file for parsing out the isotopologues...");

            //// take the type of measurements and number of isotopologues and create method file
            //MethodFile thisMethod = new MethodFile(measurementTypeInput, peakNumberInput);

            //// Input isotopologues/peaks
            ////TODO: make this a module?? like a function under the main()?
            //for (int i = 0; i < thisMethod.PeakNumber; i++)
            //{
            //    if (i == 0)
            //    {
            //        //thisMethod.PeakType[i] = MethodFile.IsotopologueType.Base;
            //        //TODO: is peak type useful in RawDataObject? If not, delete it from the method file

            //        Console.WriteLine("Enter chemical formula of the most abundant ion of interest (base peak):");
            //        Console.WriteLine("(some examples: SO4, C5H11, and C5N5H6)");

            //        string basePeakInput = Console.ReadLine();
            //        thisMethod.Formula[i] = basePeakInput;

            //        // calculate mass of the base peak
            //        thisMethod.ExactMass[i] = thisMethod.Formula[i].MonoisotopicMass;

            //        Console.WriteLine("Base peak {0}, mass {1} amu.", thisMethod.Formula[i], thisMethod.ExactMass[i]);
            //        Console.WriteLine();

            //        Console.WriteLine("Starting to define mass tolerance of the base peak...");
            //        Console.WriteLine("Recommended mass tolerance: 5 ppm, or 0.5 mmu.");
            //        Console.WriteLine("Select mass tolerance unit:");
            //        Console.WriteLine("Press '1' for ppm;");
            //        Console.WriteLine("Press '2' for mmu.");

            //        int massTolUnitInput = Convert.ToInt16(Console.ReadLine());
            //        thisMethod.DefineMassToleranceUnit(i, massTolUnitInput);

            //        Console.WriteLine("Enter mass tolerance of the base peak in {0}:", thisMethod.MassToleranceUnit[i]);

            //        thisMethod.MassTolerance[i] = Convert.ToDouble(Console.ReadLine());
            //        //TODO: add warning for too large or too small mass tolerance 

            //        Console.WriteLine("Base peak {0}, mass {1} amu, mass tolerance {2} {3}.", thisMethod.Formula[i], thisMethod.ExactMass[i], thisMethod.MassTolerance[i], thisMethod.MassToleranceUnit[i]);
            //        Console.WriteLine();

            //        //TODO: parse chemical formula, compute singly- and doubly-substituted isotopologues?
            //    }

            //    else
            //    {
            //        //thisMethod.PeakType[i] = MethodFile.IsotopologueType.Substituted;
            //        //TODO: same as in the if

            //        Console.WriteLine("Enter chemical formula of another ion of interest (substituted peak {0}):", i);//TODO: 1st, 2nd, 3rd
            //        Console.WriteLine("(some examples: SO{18}4, C{13}C4H11, and C5N5H5D)");

            //        string substPeakInput = Console.ReadLine();
            //        thisMethod.Formula[i] = substPeakInput;

            //        // calculate mass of the substituted peak
            //        thisMethod.ExactMass[i] = thisMethod.Formula[i].MonoisotopicMass;

            //        Console.WriteLine("Substituted peak {0}, mass {1} amu.", thisMethod.Formula[i], thisMethod.ExactMass[i]);
            //        Console.WriteLine();

            //        Console.WriteLine("Define mass tolerance of the substituted peak {0}:", i);
            //        Console.WriteLine("Recommended mass tolerance: 5 ppm, or 0.5 mmu.");
            //        Console.WriteLine("Select mass tolerance unit:");
            //        Console.WriteLine("Press '1' for ppm;");
            //        Console.WriteLine("Press '2' for mmu.");

            //        int massTolUnitInput2 = Convert.ToInt16(Console.ReadLine());
            //        thisMethod.DefineMassToleranceUnit(i, massTolUnitInput2);

            //        Console.WriteLine("Enter mass tolerance of the substituted peak {0} in {1}:", i, thisMethod.Formula[i]);

            //        thisMethod.MassTolerance[i] = Convert.ToDouble(Console.ReadLine());
            //        //TODO: add warning if the mass tolerance is too large or too small

            //        Console.WriteLine("Substituted peak {0}, mass {1} amu, mass tolerance {2} {3}.", thisMethod.Formula[i], thisMethod.ExactMass[i], thisMethod.MassTolerance[i], thisMethod.MassToleranceUnit[i]);
            //        Console.WriteLine();
            //    }
            //}

            //Console.WriteLine("Calculating acceptable mass ranges of isotopologues...");
            //Console.WriteLine();

            //thisMethod.CalculateHighLowMass();

            //Console.WriteLine("Finished input of {0} isotopologues of interest.", thisMethod.PeakNumber);
            //Console.WriteLine();

            //// Input culling criteria

            //Console.WriteLine("Starting to define culling criteria (in develope)...");
            //Console.WriteLine();
            //Console.WriteLine("Define the lowest acceptable signal intensity...");
            //Console.WriteLine("(Recommended value: 10 % of the maximum signal intensity)");
            //Console.WriteLine("Enter minimum acceptable intensity of base peak in percentage of the maximum value.");

            //thisMethod.MinAbsIntensityPercentage = Convert.ToDouble(Console.ReadLine());


            //DONE: create and populate each peak object, with raw file data


            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //(2) Use the inputs to populate and process the raw files

            //find the raw files
            string filename = "/Users/sarahzeichner/Documents/Caltech/2019-2020/Research/Code/perchlorateRawFiles/USGS37_end_10-28-2019.RAW";

            //TODO: create a reference file with natural abundances of interest ?? Talk to John about this
            //double Cl37_NATURAL_ABUNDANCE = 24.23;
            //double Cl35_NATURAL_ABUNDANCE = 75.77;

            //TODO: pass in method file with inputs to get out data, for now just set things
            //will  iterate through this code and create an object for each peak, so only need to do one

            string TIME_RANGE = "";
            string CHEMICAL_FORMULA = "ClO4";
            string EXACT_MASS = "98.94791485";
            string MASS_TOLERANCE = "0.005";

            //convert input strings from method file to usable numbers
            //TODO: parse info from method file as an input
            MethodFile thisMethod = new MethodFile(2, 2);

            //TODO: use the method file to get these things
            double inputChemicalMass = double.Parse(EXACT_MASS);
            double inputTolerance = double.Parse(MASS_TOLERANCE);

            thisMethod.ExactMass[0] = inputChemicalMass;
            thisMethod.MassTolerance[0] = inputTolerance;

            //TODO: extract a lot of this stuff out into a raw data object
            // Create a Raw Data Object to pass data extracted from Raw Data file
            RawDataObject rtnObject = new RawDataObject(filename, thisMethod);

            //TODO: return the object, or output the information to CSV (or both!)
            Console.Write("done");

            //TODO: dispose of the data objects

        }
    }
}
