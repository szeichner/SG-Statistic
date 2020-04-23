using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SGStatistic
{
    public class RawDataProcessor
    {
        public string FolderPath { get; set; }
        public string MethodFileName { get; set; }
        public MethodFile MethodFile { get; set; }
        public RawDataObject[] RawDataObjectArray { get; set; }

        public RawDataProcessor(string folderPath, List<string> rawFileNames, string methodFileName)
        {
            //instantiate all the necessary properties to process data
            FolderPath = folderPath;
            MethodFileName = methodFileName;
            MethodFile = new MethodFile(methodFileName);
            RawDataObjectArray = new RawDataObject[rawFileNames.Count];
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
