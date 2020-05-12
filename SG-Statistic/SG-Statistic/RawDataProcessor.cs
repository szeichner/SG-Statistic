using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SGStatistic
{
    /// <summary>
    /// Process Raw Data files within a folder path, given an input method and list of raw files
    /// </summary>
    public class RawDataProcessor
    {
        /// <summary>
        /// Folder path with the method file and all the raw files to process
        /// </summary>
        public string FolderPath { get; set; }
        /// <summary>
        /// list of raw files to process
        /// </summary>
        public List<string> RawFilePathNames { get; set; }
        /// <summary>
        /// method file object created from input method.txt file
        /// </summary>
        public MethodFile MethodFile { get; set; }

        /// <summary>
        /// Create a RawDataProcessor object with input folder 
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="methodFileName"></param>
        public RawDataProcessor(string folderPath, string methodFileName)
        {
            FolderPath = folderPath;

            try
            {
                //read in all the files with the path name .RAW, add to an array
                RawFilePathNames = new List<string>(Directory.EnumerateFiles(FolderPath, "*.RAW"));

                //set up the method file to read in the raw files
                if (File.Exists(folderPath + "/" + methodFileName))
                {
                    string methodFile = folderPath + "/" + methodFileName;
                    MethodFile = new MethodFile(methodFile);
                }
                else
                {
                    Console.WriteLine("Method file does not exist, please create one to process .RAW files.");
                }

            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

        }

        /// <summary>
        /// Process a list of files in a specified folder using a specified method 
        /// </summary>
        /// <param name="fileNames"></param>
        /// <param name="methodFile"></param>
        /// <param name="exportFileName"></param>
        public void ProcessRawFiles(List<string> fileNames, MethodFile methodFile, string exportFileName)
        {
            //loop through each file and output a list of RawFileObjects
            for (int i = 0; i < fileNames.Count; i++)
            {
                List<RawDataObject> rawDataObjects = new List<RawDataObject>();
                for (int j = 0; j < methodFile.Masses.Length; j++)
                {
                    rawDataObjects.Add(new RawDataObject(fileNames[i], methodFile.ChemicalFormulas[j], methodFile.Masses[j], methodFile.MassTolerances[j], methodFile.MassToleranceUnits[j]));
                }

                try
                {
                    //write to json object to export
                    string rtnJsonObject = JsonConvert.SerializeObject(rawDataObjects);
                    //write json object to the file
                    System.IO.File.WriteAllText(fileNames[i] + exportFileName, rtnJsonObject);

                    //write to CSV if toggled on
                    WriteCSV(rawDataObjects, ",", "csv_output.csv");
                    
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }

            }

        }

        /// <summary>
        /// Write the object to a csv
        /// </summary>
        /// <param name="items"></param>
        /// <param name="delimator"></param>
        /// <param name="outputPath"></param>
        public void WriteCSV(IEnumerable<RawDataObject> items, string delimator, string outputPath)
        {
            Type itemType = typeof(RawDataObject);
            var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .OrderBy(p => p.Name);


            using (var writer = new StreamWriter(outputPath))
            {
                //write header
                writer.WriteLine(string.Join(delimator, props.Select(p => p.Name)));

                //writer.WriteLine("HighMass: " + Convert.ToString(rawDataObject.HighMass));
                //writer.WriteLine("LowMass: " + Convert.ToString(rawDataObject.LowMass));
                //writer.WriteLine("Tolerance: " + Convert.ToString(rawDataObject.Tolerance) + Convert.ToString(rawDataObject.ToleranceUnits));
                //writer.WriteLine("Resolution: " + Convert.ToString(rawDataObject.MassResolution));

                //write out the objects
                foreach (var item in items)
                {

                    writer.WriteLine(string.Join(", ", props.Select(p => p.GetValue(item, null))));

                    //TODO: Fix how this is writing out to CSV
                }
            }
        }

    }
}
