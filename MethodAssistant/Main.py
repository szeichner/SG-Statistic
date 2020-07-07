#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Created on Mon Apr 27 19:23:09 2020

@author: guannandong
"""

from molmass import Formula
import IsotopologueInput as IsoIn
import CullingCriteriaInput
import MethodFileCreator


#%% definitions

# measurement type tuple      
msmtType = ("GC-reservoir","ESI-syringe pump","GC direct elution","small sample")

# polarity mode tuple
polarType = ("positve","negative")


#%% "welcome"
print("Welcome to Data Processing Method Assistant!")

print("Tell us about your measurements - ")

#%% user input experiment info
print("Which type of measurement do you want to process?")
for x in msmtType:
    print("Press \'{0}\' for a {1} measurement.".format(msmtType.index(x),x))
msmt = int(input())
### msmt = 1

print("What is the polarity?")
polar = int(input("Type '0' for positive mode, '1' for negative mode: "))
### polar = 1

print(polarType[polar]+ " mode "+msmtType[msmt]+ " measurement chosen.")

#%% user input isotopologue peaks
print("Let's talk about what you want to know from the measurements - ")

peakCounter = 0

# make a list of IsotopologueInput objects
peakGroup = []

while True:
    # !!todo!!: what if 1 amu window msmt
    #  base peak input
    if peakCounter == 0:
        peakType = IsoIn.isoType[0]
        print("Enter the chemical formula of the most abundant ion of interest ({0} peak):".
              format(IsoIn.isoType[peakCounter]))
        print("(some examples: SO4, C5H11, and C5N5H6)")
    # substituted peak input
    else:
        peakType = IsoIn.isoType[-1]
        print("Enter the chemical formula of another ion of interest:")
        print("(some examples: SO3[18O], [13C]C4H11, and C5N5H5D)")

    peak = Formula(input())
    ### peak = Formula('[37Cl]O4')
    
    # counting number of peak inputted
    peakCounter = peakCounter + 1
    
    # calculate peak mass using molmass package, minus/plus the mass of an electron based on positive/negative mode    
    if polar == 0:
        mass = peak.isotope.mass-IsoIn.emass
    elif polar == 1:
        mass = peak.isotope.mass+IsoIn.emass       
    
    print("{0} peak {1}, mass {2} amu.".format(peakType,peak.formula,mass))
    
    # prompt mass tolerance
    print("Starting to define mass tolerance of the {0} peak...".format(peakType))
    print("Recommended mass tolerance: 5 ppm, or 0.5 mmu.")
    print("Select mass tolerance unit:")
    for x in IsoIn.mtolUnitType:
        print("Press \'{0}\' for {1}.".format(IsoIn.mtolUnitType.index(x),x))
    unit = IsoIn.mtolUnitType[int(input())]
    ### unit = IsoIn.mtolUnitType[0]
    mtol=float(input("Enter mass tolerance of the {0} peak in {1}: ".format(peakType,unit)))
    ### mtol = 0.5
    print("{0} peak {1}, mass {2} amu, mass tolerance {3} {4}.".
          format(peakType,peak.formula,mass,mtol,unit))
    
    # create an IsotopologueInput object using inputs, add the object to the list
    peakGroup.append(IsoIn.IsotopologueInput(peakType, peak, mass, mtol, unit))

    # !!todo!!: add warning for too large or too small mass tolerance
    # !!todo!!: come up with singly substituted isotopologues based on base peak
     
    finishChoice = input('Finish inputting all peaks of interest? (y/n) ').lower()
    if finishChoice == 'y':
        break
    
#%% prompt user for different culling criteria based on type of measurement
print("\nStarting to define culing criteria for the {0} measurement... (in development)".format(msmtType[msmt]))
cullingCriteriaInput = CullingCriteriaInput.CullingCriteriaInput()
if msmt == 0:
    cullingCriteriaInput.minIntensity = float(input("Enter minimum acceptable intensity of base peak in percentage of the maximum intensity (recommend: 10%): ").strip('%'))    
elif msmt == 1:
    cullingCriteriaInput.startTime = float(input("Select a stable time window for signal. Enter the start time in min: ").strip('min'))
    cullingCriteriaInput.stopTime = float(input("Enter the stop time in min: ").strip('min'))
    ### cullingCriteriaInput.startTime = 5.1
    ### cullingCriteriaInput.stopTime = 32
elif msmt == 2:
    cullingCriteriaInput.arrivalTime = float(input("Retention time of the chromatographic peak? Enter the arrival time in min: ").strip('min'))
    cullingCriteriaInput.exitTime = float(input("Enter the exit time in min: ").strip('min'))
elif msmt == 3:
    cullingCriteriaInput.minIntrTIC = float(input("Enter minimum acceptable intensity of base peak in percentage of TIC (total ion current) (recommend: 30%?): ").strip('%'))    

#%% prompt user for output data format
csvChoice = input('Do you want to save raw data in a Tab delimited Text file? (y/n) ').lower()

#%% write .txt file
print("\nWriting method files...")

MethodFileCreator.MethodFileCreator(msmtType[msmt],polarType[polar],peakCounter,peakGroup,cullingCriteriaInput,csvChoice)

print("Done!")
