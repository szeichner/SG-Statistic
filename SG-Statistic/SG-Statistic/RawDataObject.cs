using System;
using Newtonsoft.Json;

using ThermoFisher.CommonCore.Data;
using ThermoFisher.CommonCore.Data.Business;
using ThermoFisher.CommonCore.Data.FilterEnums;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.MassPrecisionEstimator;
using ThermoFisher.CommonCore.RawFileReader;

namespace SGStatistic
{
    /// <summary>
    /// This object gets populated with input data for one peak from a RAW data file, based on input peak mass and mass tolerance.
    /// </summary>
    [Serializable]
    public class RawDataObject
    {
        //Constants
        ///<summary> Type of filter </summary>
        private const string FILTER_MS = "ms";

        //Properties from the Method file
        ///<summary> Mass Tolerance </summary>
        private double Tolerance { get; set; }
        private string ToleranceUnits { get; set; }
        ///<summary> Mass of peak </summary>
        private double Mass { get; set; }
        ///<summary> High mass based on  input mass and  mass tolerance </summary>
        private double HighMass { get; set; }
        ///<summary> Low mass based on  input mass and  mass tolerance </summary>
        private double LowMass { get; set; }

        //Header info
        public double MassResolution { get; set; }
        private IRawDataPlus RawFile { get; set; }

        //Chromatogram Data
        public int[] ScanArray { get; set; }
        public double[] ChromatogramPeakPositionsArray { get; set; }
        public double[] ChromatogramIntensitiesArray { get; set; }

        //SpectrumData
        public double[] SpectraTIC { get; set; }
        public double[] SpectraRetentionTime { get; set; }
        public double[] SpectraBasePeakIntensities { get; set; }
        public double[] SpectraMasses { get; set; }
        public double[] SpectraResolutions { get; set; }
        public double[] SpectraNoise { get; set; }

        /// <summary>
        /// Instantiates and populates a raw data object based on input raw file and specified method
        /// </summary>
        /// <param name="inputRawFile">Raw file to analyze</param>
        /// <param name="inputMethodFile">Method file describing how to export the data</param>
        /// <returns></returns>
        public RawDataObject(string inputRawFile, string formula,  double mass, double tolerance, string toleranceUnits)
        {
            CalculateHighAndLowMasses(mass, tolerance, toleranceUnits);
            GetSetRawFileData(inputRawFile);
            GetChromatogramData();
            GetSpectrumData();
        }

        /// <summary>
        /// Reads the information from the method file and populates fields based on specified peak and tolerance
        /// </summary>
        /// <param name="methodFile">Method file describing how to export the data</param>
        /// <returns></returns>
        private void CalculateHighAndLowMasses(double mass, double tolerance, string toleranceUnits)
        {
            Mass = mass;
            ToleranceUnits = toleranceUnits;

            //convert tolerance into the correct units
            Tolerance = Convert.ToDouble(tolerance); // set to converted tolerance;

            // ToleranceUnits = toleranceUnits;
            if (ToleranceUnits == "mmu")
            {
                HighMass = mass + (tolerance / 1000);
                LowMass = mass - (tolerance / 1000);

            }
            else if (ToleranceUnits == "ppm")
            {
                HighMass = mass * (1 + tolerance / 1000000);
                LowMass = mass * (1 - tolerance / 1000000);
            }
            else
            {
                Console.WriteLine("Error in the mass tolerance unit!");
            }
        }
 
        /// <summary>
        /// Get and set general information from the raw file
        /// </summary>
        /// <param name="inputRawFile"></param>
        /// <returns></returns>
        private void GetSetRawFileData(string inputRawFile)
        {
            // Create the IRawDataPlus object for accessing the RAW file
            RawFile = RawFileReaderAdapter.FileFactory(inputRawFile);

            // Select instrument we are intereted in looking at data from
            RawFile.SelectInstrument(Device.MS, 1);
        }

        /// <summary>
        /// Get and set chromatogram data from the raw file
        /// </summary>
        /// <returns></returns>
        private void GetChromatogramData()
        {

            //Get mass resolution for the run from the header
            MassResolution = RawFile.RunHeaderEx.MassResolution;

            //Set up settings to extract the data
            ChromatogramTraceSettings traceSettings =
            new ChromatogramTraceSettings(TraceType.MassRange)
            {
                Filter = FILTER_MS,
                MassRanges = new[] { new Range(LowMass, HighMass) }
            };
            IChromatogramSettings[] allSettings = { traceSettings };

            MassOptions massOptions = new MassOptions() { Tolerance = Tolerance, ToleranceUnits = ToleranceUnits.amu };
            var chromatogramData = RawFile.GetChromatogramData(allSettings, -1, -1, massOptions); //get all chromatogram data

            // Get data from the chromatogram
            ScanArray = chromatogramData.ScanNumbersArray[0];
            ChromatogramPeakPositionsArray = chromatogramData.PositionsArray[0]; //get positions  of each scan
            ChromatogramIntensitiesArray = chromatogramData.IntensitiesArray[0]; //get list of intensities for scan numbers

        }

        /// <summary>
        /// Get and set data from the mass spectrum
        /// </summary>
        /// <returns></returns>
        private void GetSpectrumData()
        {
            // Get the first and last scan from the RAW file
            int firstScanNumber = RawFile.RunHeaderEx.FirstSpectrum;
            int lastScanNumber = RawFile.RunHeaderEx.LastSpectrum;

            // Set up empty variables to populate with mass spectrum data
            SpectraTIC = new double[lastScanNumber];
            SpectraRetentionTime = new double[lastScanNumber];
            SpectraBasePeakIntensities = new double[lastScanNumber];
            SpectraMasses = new double[lastScanNumber];
            SpectraNoise = new double[lastScanNumber];
            SpectraResolutions = new double[lastScanNumber];

            //get out information from mass spectra for each scan
            for (int scan = 0; scan <= (ScanArray.Length - 1); scan++)
            {
                int thisScan = ScanArray[scan];
                // get the scan statistics for this scan number
                ScanStatistics scanStatistic = RawFile.GetScanStatsForScanNumber(thisScan);

                //get the TIC
                SpectraTIC[scan] = scanStatistic.TIC;
                //get retention time
                SpectraRetentionTime[scan] = RawFile.RetentionTimeFromScanNumber(thisScan);
                //TODO: get set IT

                //Check to see if there is high resolution data
                if (scanStatistic.IsCentroidScan)
                {
                    // Get the centroid (label) data from the RAW file for this scan
                    var centroidStream = RawFile.GetCentroidStream(ScanArray[scan], false); //don't include reference and exception peaks

                    SpectraMasses[scan] = centroidStream.BasePeakMass;
                    SpectraBasePeakIntensities[scan] = centroidStream.BasePeakIntensity;
                    SpectraNoise[scan] = centroidStream.BasePeakNoise;
                    SpectraResolutions[scan] = centroidStream.BasePeakResolution;
                    //TODO: handle for there being more than one peak in this range, and alert for that

                }
                else
                {
                    //If no high-res data, then populate with low resolution data

                    var segmentedScan = RawFile.GetSegmentedScanFromScanNumber(ScanArray[scan], scanStatistic);

                    //TODO: input base peak information
                    SpectraMasses[scan] = segmentedScan.Positions[0];
                    SpectraBasePeakIntensities[scan] = segmentedScan.Intensities[0];

                    //TODO : get low res data instead (i.e., segmented scans)

                }

            }

        }
    }
}
