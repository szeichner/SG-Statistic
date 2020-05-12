using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
        private ToleranceUnits ToleranceUnits { get; set; }
        ///<summary> Mass of peak </summary>
        private double Mass { get; set; }
        ///<summary> High mass based on  input mass and  mass tolerance </summary>
        private double HighMass { get; set; }
        ///<summary> Low mass based on  input mass and  mass tolerance </summary>
        private double LowMass { get; set; }

        //Header info
        /// <summary>
        /// Mass resolution of the experiment
        /// </summary>
        public double MassResolution { get; set; }

        //Chromatogram Data
        /// <summary>
        /// List of scans from the analysis
        /// </summary>
        public int[] ScanArray { get; set; }
        /// <summary>
        /// Time positions of the chromatogram
        /// </summary>
        public double[] ChromatogramPeakPositionsArray { get; set; }
        /// <summary>
        /// Intensities of the chromatogram
        /// </summary>
        public double[] ChromatogramIntensitiesArray { get; set; }

        //SpectrumData
        /// <summary>
        /// TIC from mass spectrum
        /// </summary>
        public double[] SpectraTIC { get; set; }
        /// <summary>
        /// Retention time of each scan from the mass spectrum for the peak of interest
        /// </summary>
        public double[] SpectraRetentionTime { get; set; }
        /// <summary>
        /// Intensity of the base peak from the mass spectrum for the mass range of interest, based on input mass and tolerance
        /// </summary>
        public double[] SpectraBasePeakIntensities { get; set; }
        /// <summary>
        /// m/z of the input peaks for analysis
        /// </summary>
        public double[] SpectraMasses { get; set; }
        /// <summary>
        /// Resolution of each scan in the mass spectra
        /// </summary>
        public double[] SpectraResolutions { get; set; }
        /// <summary>
        /// Noise for each scan in the mass spectra
        /// </summary>
        public double[] SpectraNoise { get; set; }

        /// <summary>
        /// Instantiates and populates a raw data object based on input raw file and specified method
        /// <param name="inputRawFile"></param>
        /// <param name="formula"></param>
        /// <param name="mass"></param>
        /// <param name="tolerance"></param>
        /// <param name="toleranceUnits"></param>
        public RawDataObject(string inputRawFile, string formula,  double mass, double tolerance, ToleranceUnits toleranceUnits)
        {
            CalculateHighAndLowMasses(mass, tolerance, toleranceUnits);
            IRawDataPlus rawFile = GetSetRawFileData(inputRawFile);
            GetChromatogramData(rawFile);
            GetSpectrumData(rawFile);
            CleanUpRawFile(rawFile);

        }

        /// <summary>
        /// Reads the information from the method file and populates fields based on specified peak and tolerance
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="tolerance"></param>
        /// <param name="toleranceUnits"></param>
        private void CalculateHighAndLowMasses(double mass, double tolerance, ToleranceUnits toleranceUnits)
        {
            Mass = mass;
            ToleranceUnits = toleranceUnits;

            //convert tolerance into the correct units
            Tolerance = Convert.ToDouble(tolerance); // set to converted tolerance;

            // ToleranceUnits = toleranceUnits;
            if (ToleranceUnits == ToleranceUnits.mmu)
            {
                HighMass = mass + (tolerance / 1000);
                LowMass = mass - (tolerance / 1000);

            }
            else if (ToleranceUnits == ToleranceUnits.ppm)
            {
                HighMass = mass * (1 + tolerance / 1000000);
                LowMass = mass * (1 - tolerance / 1000000);
            }
            else
            {
                Console.WriteLine("Error in the mass tolerance unit.");
            }
        }
 
        /// <summary>
        /// Get and set general information from the raw file
        /// </summary>
        /// <param name="inputRawFile"></param>
        /// <returns></returns>
        private IRawDataPlus GetSetRawFileData(string inputRawFile)
        {
            // Create the IRawDataPlus object for accessing the RAW file
            IRawDataPlus rawFile = RawFileReaderAdapter.FileFactory(inputRawFile);

            if (!rawFile.IsOpen || rawFile.IsError)
            {
                Console.WriteLine("Unable to access the RAW file using the RawFileReader class!");
            }

            // Check for any errors in the RAW file
            if (rawFile.IsError)
            {
                Console.WriteLine("Error opening ({0})", rawFile.FileError);
            }

            // Check if the RAW file is being acquired
            if (rawFile.InAcquisition)
            {
                Console.WriteLine("RAW file still being acquired...");

            }

            // Select instrument we are intereted in looking at data from
            rawFile.SelectInstrument(Device.MS, 1);

            return rawFile;
        }
        /// <summary>
        /// Get and set chromatogram data from the raw file
        /// </summary>
        /// <param name="inputRawFile"></param>
        private void GetChromatogramData(IRawDataPlus inputRawFile)
        {
            try
            {
                //Get mass resolution for the run from the header
                MassResolution = inputRawFile.RunHeaderEx.MassResolution;

                //Set up settings to extract the data
                ChromatogramTraceSettings traceSettings =
                new ChromatogramTraceSettings(TraceType.MassRange)
                {
                    Filter = FILTER_MS,
                    MassRanges = new[] { new Range(LowMass, HighMass) }
                };
                IChromatogramSettings[] allSettings = { traceSettings };

                MassOptions massOptions = new MassOptions() { Tolerance = Tolerance, ToleranceUnits = ToleranceUnits };
                var chromatogramData = inputRawFile.GetChromatogramData(allSettings, -1, -1, massOptions); //get all chromatogram data

                // Get data from the chromatogram
                ScanArray = chromatogramData.ScanNumbersArray[0];
                ChromatogramPeakPositionsArray = chromatogramData.PositionsArray[0]; //get positions  of each scan
                ChromatogramIntensitiesArray = chromatogramData.IntensitiesArray[0]; //get list of intensities for scan numbers
            }
            catch(Exception ex)
            {
                Console.Write("Error in accessing chromatogram data: " + ex);
            }
            
        }

        /// <summary>
        /// Get and set data from the mass spectrum
        /// </summary>
        /// <param name="inputRawFile"></param>
        private void GetSpectrumData(IRawDataPlus inputRawFile)
        {
            // Get the first and last scan from the RAW file
            int firstScanNumber = inputRawFile.RunHeaderEx.FirstSpectrum;
            int lastScanNumber = inputRawFile.RunHeaderEx.LastSpectrum;

            // Set up empty variables to populate with mass spectrum data
            SpectraTIC = new double[lastScanNumber];
            SpectraRetentionTime = new double[lastScanNumber];
            SpectraBasePeakIntensities = new double[lastScanNumber];
            SpectraMasses = new double[lastScanNumber];
            SpectraNoise = new double[lastScanNumber];
            SpectraResolutions = new double[lastScanNumber];

            try
            {
                //get out information from mass spectra for each scan
                for (int scan = 0; scan <= (ScanArray.Length - 1); scan++)
                {
                    int thisScan = ScanArray[scan];
                    // get the scan statistics for this scan number
                    ScanStatistics scanStatistic = inputRawFile.GetScanStatsForScanNumber(thisScan);

                    //get the TIC
                    SpectraTIC[scan] = scanStatistic.TIC;
                    //get retention time
                    SpectraRetentionTime[scan] = inputRawFile.RetentionTimeFromScanNumber(thisScan);
                    //TODO: get set IT

                    //Check to see if there is high resolution data
                    if (scanStatistic.IsCentroidScan)
                    {
                        // Get the centroid (inputRawFile) data from the RAW file for this scan
                        var centroidStream = inputRawFile.GetCentroidStream(ScanArray[scan], false); //don't include reference and exception peaks

                        SpectraMasses[scan] = centroidStream.BasePeakMass;
                        SpectraBasePeakIntensities[scan] = centroidStream.BasePeakIntensity;
                        SpectraNoise[scan] = centroidStream.BasePeakNoise;
                        SpectraResolutions[scan] = centroidStream.BasePeakResolution;
                        //TODO: handle for there being more than one peak in this range, and alert for that

                    }
                    else
                    {
                        //If no high-res data, then populate with low resolution data
                        var segmentedScan = inputRawFile.GetSegmentedScanFromScanNumber(ScanArray[scan], scanStatistic);

                        SpectraMasses[scan] = segmentedScan.Positions[0];
                        SpectraBasePeakIntensities[scan] = segmentedScan.Intensities[0];
                        //TODO: noise
                        //TODO: resolution?
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write("Error in accessing spectrum data: " + ex);
            }
           

            }

        /// <summary>
        /// Dispose of the raw file object to handle memory situation
        /// </summary>
        /// <param name="rawFile"></param>
        private void CleanUpRawFile(IRawDataPlus rawFile)
        {
            rawFile.Dispose();
        }


    }

        
    }
