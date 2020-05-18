###Import JSON and check for catastophic errors
###Tim Csernica, May 13, 2020
import json
import pandas as pd

def readFile(fileName):
    '''
    Takes a JSON output file from the .RAW file reader and turns it into a list of dataFrames. There is one dataFrame for 
    each peak extracted from the .RAW file. 
    Inputs:
        fileName: A string specifying the path of the file to read in
        
    Outputs:
        dfList: A list of dataframes, with one dataframe for each peak. 
    '''
    with open(fileName) as f:
        data = json.load(f)
        
        dfList = []
        for peak in data:
            df = pd.DataFrame.from_dict(peak)
            dfList.append(df)
            
    return dfList

def checkNullPeak(dfList):
    '''
    Checks to see if any peak extracted from the .RAW file reader has an intensity of 0. If it does, prints a warning.
    Inputs:
        dfList: A list of dataframes, with one dataframe for each peak.
        
    Outputs:
        failed: True if there is a null peak, False otherwise. Prints a warning if there is a null peak. 
    '''
    failed = False
    for peak in dfList:
        if peak['ChromatogramIntensitiesArray'].sum() == 0:
            print("Null peak found at mass " + peak["SpectraMasses"][0])
            failed = True          

    return failed

def checkMeasurementRejection(dfList):
    '''
    Runs a variety of tests on measurements to see if they should be rejected out of hand. 
    Inputs: 
        dfList: A list of dataframes, with one dataframe for each peak.
        
    Outputs: 
        failed: True if the measurement fails our tests, False otherwise. Currently tests just if there is a null peak, but
        others may be added in the future.
    '''
    failed = False
    if checkNullPeak(dfList) == True:
        failed = True
    
    return failed