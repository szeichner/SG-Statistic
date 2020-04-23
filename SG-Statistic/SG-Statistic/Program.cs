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
            //look for a method file in the input folder path

            Console.Write("Please input a folder path with .RAW files of interest for analysis");
            string folderPath = Console.ReadLine();
            string methodFileName = "method.txt";
            string exportFileName = "";

            //Try get method file and list of raw file names from folder path
            if(Directory.Exists(folderPath))
            {
                try
                {

                    //read in all the files with the path name .RAW, add to an array
                    List<string> rawFileNames = new List<string>(Directory.EnumerateFiles(folderPath, "*.RAW"));

                    if (File.Exists(folderPath + methodFileName))
                    {
                        string methodFile = folderPath + methodFileName;
                        RawDataProcessor thisProcessor = new RawDataProcessor(folderPath, rawFileNames, methodFile);
                        thisProcessor.ProcessRawFiles(rawFileNames, thisProcessor.MethodFile, exportFileName);

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

            Console.Write(".RAW file processing complete");

           
        }
    }
}
