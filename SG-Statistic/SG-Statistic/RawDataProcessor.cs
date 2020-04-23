using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SGStatistic
{
    /// <summary>
    /// Process Raw Data files within a folder path, given an input method and list of raw files
    /// </summary>
    public class RawDataProcessor
    {
        public string FolderPath { get; set; }
        public List<string> RawFilePathNames { get; set; }
        public MethodFile MethodFile { get; set; }
        public RawDataObject[] RawDataObjectArray { get; set; }

        public RawDataProcessor(string folderPath, string methodFileName)
        {
            FolderPath = folderPath;

            try
            {
                //read in all the files with the path name .RAW, add to an array
                List<string> rawFileNames = new List<string>(Directory.EnumerateFiles(FolderPath, "*.RAW"));
                RawDataObjectArray = new RawDataObject[rawFileNames.Count];

                //set up the method file to read in the raw files
                if (File.Exists(folderPath + methodFileName))
                {
                    string methodFile = folderPath + methodFileName;
                    MethodFile = new MethodFile(methodFile);
                }
                else
                {
                    Console.WriteLine("Method file does not exist, please create one to process .RAW files.");
                }
               
            }
            catch(Exception ex)
            {
                Console.Write(ex);
            }
            
        }

        public void ProcessRawFiles(List<string> fileNames, MethodFile methodFile, string exportFileName)
        {
            for (int i = 0; i < fileNames.Count; i++)
            {
                RawDataObjectArray[i] = new RawDataObject(fileNames[i], methodFile.ChemicalFormulas[i], methodFile.Masses[i], methodFile.MassTolerances[i], methodFile.MassToleranceUnits[i] );
            }

            try
            {
                //write to json object to export
                string rtnJsonObject = JsonConvert.SerializeObject(RawDataObjectArray);
                //write json object to the file
                System.IO.File.WriteAllText(exportFileName, rtnJsonObject);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

        }

    }
}
