//TODO: add header comments

using System;
using System.Collections.Generic;
using ThermoFisher.CommonCore.Data;

namespace SGStatistic
{
    /// <summary>
    /// Populate properties based on method file that will be used to process .RAW data files
    /// </summary>
    public class MethodFile
    {
        //Different strings used to parse the method file inputs
        private string inputPeakNumberString = "peakNumber";
        private string chemicalFormulaString = "chemicalFormula";
        private string massString = "mass";
        private string toleranceString = "tolerance";
        private string toleranceUnitsString = "toleranceUnits";

        public int InputPeakNumber { get; set; }
        public string[] ChemicalFormulas { get; set; }
        public double[] Masses { get; set; }
        public double[] MassTolerances { get; set; }
        public string[] MassToleranceUnits { get; set; }

        ///<summary> Instantiate method file using input .txt file path</summary>
        public MethodFile(string inputMethodFile)
        {
            //Read lines in from the method file .txt
            string[] methodLines = System.IO.File.ReadAllLines(inputMethodFile);
            Dictionary<string, string> methodDictionary = new Dictionary<string, string>(methodLines.Length);

            //parse the input method file
            for (int i = 0; i < methodLines.Length; i++)
            {
                string[] splitValues = methodLines[i].Split('=');

                methodDictionary.Add(splitValues[0], splitValues[1]);
            }

            //parse the other inputs of the method file, and set the properties of the Method File object
            //get the number of peaks
            methodDictionary.TryGetValue(inputPeakNumberString, out string peakNumber);
            InputPeakNumber = Convert.ToInt32(peakNumber);

            //get the list of chemical formulas
            methodDictionary.TryGetValue(chemicalFormulaString, out string chemFormulas);
            string[] formulaArray = chemFormulas.Split(',');
            ChemicalFormulas = new string[formulaArray.Length];
            for (int i = 0; i < formulaArray.Length; i++)
            {
                ChemicalFormulas[i] = formulaArray[i];
            }
            
            //get the list of masses
            methodDictionary.TryGetValue(massString, out string masses);
            Masses = new double[masses.Length];
            for (int i = 0; i < masses.Length; i++)
            {
                Masses[i] = Convert.ToDouble(masses[i]);
            }

            //get the list of mass tolerances
            methodDictionary.TryGetValue(toleranceString, out string tolerances);
            MassTolerances = new double[tolerances.Length];
            for (int i = 0; i < tolerances.Length; i++)
            {
                MassTolerances[i] = Convert.ToDouble(tolerances);
            }

            methodDictionary.TryGetValue(toleranceUnitsString, out string toleranceUnits);
            string[] toleranceUnitsArray = toleranceUnits.Split(',');
            ToleranceUnits = new string[toleranceUnits.Length];
            for (int i = 0; i < toleranceUnitsArray.Length; i++)
            {
                ToleranceUnits[i] = toleranceUnitsArray[i];
            }


        }

    }


    }

