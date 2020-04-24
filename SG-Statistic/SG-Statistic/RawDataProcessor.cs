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
      
        public RawDataProcessor(string folderPath, string methodFileName)
        {
            FolderPath = folderPath;

            try
            {
                //read in all the files with the path name .RAW, add to an array
                List<string> rawFileNames = new List<string>(Directory.EnumerateFiles(FolderPath, "*.RAW"));

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
                    System.IO.File.WriteAllText(fileNames[i]+exportFileName, rtnJsonObject);
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }

            }



        }

    }
}
