//TODO: add header comments

using System;
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
        public enum IsotopologueType
        {
            Base,
            Substituted
        }//TODO:13C,D,17O,18O,15N,...

        public enum MeasurementType
        {
            GCreservoir,
            ESIsyringePump,
            GCdirectElution
        }

        public MeasurementType MsmtType { get; set; }
        public int PeakNumber { get; set; }
        //public IsotopologueType[] PeakType { get; set; }
        public ChemicalFormula[] Formula { get; set; }
        public double[] ExactMass { get; set; }
        public double[] MassTolerance { get; set; }
        public ToleranceUnits[] MassToleranceUnit { get; set; }
        public double[] HighMass { get; set; }
        public double[] LowMass { get; set; }

        public double StartTime { get; set; }
        public double StopTime { get; set; }
        public double MinAbsIntensityPercentage { get; set; }




        public MethodFile(int inputMeasurementType, int inputPeakNumber)
        {
            switch (inputMeasurementType)
            {
                case 1:
                    MsmtType = MeasurementType.GCreservoir;
                    break;
                case 2:
                    MsmtType = MeasurementType.ESIsyringePump;
                    break;
                case 3:
                    MsmtType = MeasurementType.GCdirectElution;
                    break;
            }
            PeakNumber = inputPeakNumber;
            //PeakType = new IsotopologueType[PeakNumber];
            Formula = new ChemicalFormula[PeakNumber];
            ExactMass = new double[PeakNumber];
            MassTolerance = new double[PeakNumber];//TODO:default value?
            MassToleranceUnit = new ToleranceUnits[PeakNumber];//TODO:default value?
            HighMass = new double[PeakNumber];
            LowMass = new double[PeakNumber];
        }

        public void DefineMassToleranceUnit(int PeakNum, int inputMassToleranceUnit)
        {
            if (inputMassToleranceUnit == 1)
            {
                MassToleranceUnit[PeakNum] = ToleranceUnits.ppm;
            }
            else if (inputMassToleranceUnit == 2)
            {
                MassToleranceUnit[PeakNum] = ToleranceUnits.mmu;
            }
            else
            {
                //TODO: handle input other than 1 or 2
            }
        }

        public void CalculateHighLowMass()
        {
            for (int i = 0; i < PeakNumber; i++)
            {
                if (MassToleranceUnit[i] == ToleranceUnits.mmu)
                {
                    HighMass[i] = ExactMass[i] + (MassTolerance[i] / 1000);
                    LowMass[i] = ExactMass[i] - (MassTolerance[i] / 1000);
                    Console.WriteLine("Exact {0:0.00000}, High {1:0.00000}, Low {2:0.00000}", ExactMass[i], HighMass[i], LowMass[i]);
                }
                else if (MassToleranceUnit[i] == ToleranceUnits.ppm)
                {
                    HighMass[i] = ExactMass[i] * (1 + MassTolerance[i] / 1000000);
                    LowMass[i] = ExactMass[i] * (1 - MassTolerance[i] / 1000000);
                    Console.WriteLine("Exact {0:0.00000}, High {1:0.00000}, Low {2:0.00000}", ExactMass[i], HighMass[i], LowMass[i]);
                }
                else
                {
                    //Console.WriteLine("Error in the mass tolerance unit!");
                }
            }
        }

        //DONE: method to read in the file and parse and set values for all the parts of the method file


    }

}
