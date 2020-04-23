//TODO: add header comments

using System;
using System.Collections.Generic;
using CSMSL.Chemistry;
using ThermoFisher.CommonCore.Data;

/// CSMSL (C# mass spec library) is a package(?) on github: https://github.com/dbaileychess/CSMSL
/// I used CSMSL to read chemical formula inputs and calculate the corresponding mass (2020/03/31).
/// For future expansion of handling the chemical structure, atoms and bonds in the ion, we can consider OEChem toolkit:
/// https://docs.eyesopen.com/toolkits/csharp/oechemtk/index.html


namespace SGStatistic
{
    public class MethodFile
    {
        //String to look for in text method file
        private string inputPeakNumberString = "peakNumber";
        private string chemicalFormulaString = "chemicalFormula";
        private string massString = "mass";
        private string toleranceString = "tolerance";

        public int InputPeakNumber { get; set; } //how many peaks there are
        //public IsotopologueType[] PeakType { get; set; }
        public string[] ChemicalFormulas { get; set; }
        public double[] Masses { get; set; }
        public double[] MassTolerances { get; set; }
        public ToleranceUnits[] MassToleranceUnits { get; set; }

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
        
        }

        }

        //DONE: method to read in the file and parse and set values for all the parts of the method file


    }

