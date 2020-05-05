#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Created on Mon Apr 27 19:23:09 2020

@author: guannandong
"""

from molmass import Formula

#%% definitions

# measurement type tuple      
msmtType = ("GC-reservoir","ESI-syringe pump","GC direct elution","small sample")

# polarity mode tuple
polarType = ("positve","negative")

# mass tolerance unit tuple
mtolUnitType = ("ppm","mmu")

# isotopologue type tuple
isoType = ("Base","13C-substituted","D-substituted","18O-substituted",
            "15N-substituted","17O-substituted","Others")

# electron mass
emass = 0.00054858

# isotopologue class
class Isotopologue:
    def __init__(self, peakType, Formula, mass, mtol, unit):
        self.peakType = peakType
        self.chemicalFormula = Formula.formula
        self.mass = mass
        self.mtol = mtol
        self.unit = unit

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

# make a list of Isotopologue objects
peakGroup = []

while True:
    # !!todo!!: what if 1 amu window msmt
    #  base peak input
    if peakCounter == 0:
        peakType = isoType[0]
        print("Enter the chemical formula of the most abundant ion of interest ({0} peak):".
              format(isoType[peakCounter]))
        print("(some examples: SO4, C5H11, and C5N5H6)")
    # substituted peak input
    else:
        peakType = isoType[-1]
        print("Enter the chemical formula of another ion of interest:")
        print("(some examples: SO3[18O], [13C]C4H11, and C5N5H5D)")

    peak = Formula(input())
    ### peak = Formula('[37Cl]O4')
    
    # counting number of peak inputted
    peakCounter = peakCounter + 1
    
    # calculate peak mass using molmass package, minus/plus the mass of an electron based on positive/negative mode    
    if polar == 0:
        mass = peak.isotope.mass-emass
    elif polar == 1:
        mass = peak.isotope.mass+emass       
    # !!todo!!: verify electron mass issue
    
    print("{0} peak {1}, mass {2} amu.".format(peakType,peak.formula,mass))
    
    # prompt mass tolerance
    print("Starting to define mass tolerance of the {0} peak...".format(peakType))
    print("Recommended mass tolerance: 5 ppm, or 0.5 mmu.")
    unit = mtolUnitType[int(input("select mass tolerance unit ('0' for ppm, '1' for mmu'): "))]
    ### unit = mtolUnitType[0]
    mtol=float(input("Enter mass tolerance of the {0} peak in {1}: ".format(peakType,unit)))
    ### mtol = 0.5
    print("{0} peak {1}, mass {2} amu, mass tolerance {3} {4}.".
          format(peakType,peak.formula,mass,mtol,unit))
    
    # create an Isotopologue object using inputs, add the object to the list
    peakGroup.append(Isotopologue(peakType, peak, mass, mtol, unit))

    # !!todo!!: add warning for too large or too small mass tolerance
    # !!todo!!: come up with singly substituted isotopologues based on base peak
     
    finishChoice = input('Finish inputting all peaks of interest? (y/n) ').lower()
    if finishChoice == 'y':
        break
    
#%% prompt user for different culling criteria based on type of measurement
print("\nStarting to define culing criteria for the {0} measurement... (in development)".format(msmtType[msmt]))
if msmt == 0:
    minIntensity = float(input("Enter minimum acceptable intensity of base peak in percentage of the maximum intensity (recommend: 10%): ").strip('%'))    
elif msmt == 1:
    startTime = float(input("Select a stable time window for signal. Enter the start time in min: ").strip('min'))
    stopTime = float(input("Enter the stop time in min: ").strip('min'))
elif msmt == 2:
    arrivalTime = float(input("Retention time of the chromatographic peak? Enter the arrival time in min: ").strip('min'))
    exitTime = float(input("Enter the exit time in min: ").strip('min'))
elif msmt == 3:
    minIntrTIC = float(input("Enter minimum acceptable intensity of base peak in percentage of TIC (total ion current) (recommend: 30%?): ").strip('%'))    


#%% write .txt file
print("\nWriting method files...")

# method file for RAW file reader - stop after toleranceUnits
r = open("RAWReaderMethod.txt","w")
r.write("peakNumber={0}\n".format(peakCounter))

r.write("chemicalFormula=")
for Isotopologue in peakGroup:
    r.write("{0},".format(Isotopologue.chemicalFormula))
else:
    r.truncate(r.tell()-1)
    r.write("\n")

r.write("mass=")
for Isotopologue in peakGroup:
    r.write("{:.6f},".format(Isotopologue.mass))
else:
    r.truncate(r.tell()-1)
    r.write("\n")
    
r.write("tolerance=")
for Isotopologue in peakGroup:
    r.write("{0},".format(Isotopologue.mtol))
else:
    r.truncate(r.tell()-1)
    r.write("\n")
    
r.write("toleranceUnits=")
for Isotopologue in peakGroup:
    r.write("{0},".format(Isotopologue.unit))
else:
    r.truncate(r.tell()-1)
    
r.close()

# method file including everything
f = open("DataProcessorMethod.txt","w")
f.write("peakNumber={0}\n".format(peakCounter))

f.write("chemicalFormula=")
for Isotopologue in peakGroup:
    f.write("{0},".format(Isotopologue.chemicalFormula))
else:
    f.truncate(f.tell()-1)
    f.write("\n")

f.write("mass=")
for Isotopologue in peakGroup:
    f.write("{:.6f},".format(Isotopologue.mass))
else:
    f.truncate(f.tell()-1)
    f.write("\n")
    
f.write("tolerance=")
for Isotopologue in peakGroup:
    f.write("{0},".format(Isotopologue.mtol))
else:
    f.truncate(f.tell()-1)
    f.write("\n")
    
f.write("toleranceUnits=")
for Isotopologue in peakGroup:
    f.write("{0},".format(Isotopologue.unit))
else:
    f.truncate(f.tell()-1)
    f.write("\n")

f.write("peakType=")
for Isotopologue in peakGroup:
    f.write("{0},".format(Isotopologue.peakType))
else:
    f.truncate(f.tell()-1)
    f.write("\n")

f.write("measurementType={0}\n".format(msmtType[msmt]))
f.write("polarityMode={0}\n".format(polarType[polar]))
if msmt == 0:
    f.write("minIntensity={0}\n".format(minIntensity))
elif msmt == 1:
    f.write("startTime={0}\n".format(startTime))
    f.write("stopTime={0}\n".format(stopTime))
elif msmt == 2:
    f.write("arrivalTime={0}\n".format(arrivalTime))
    f.write("exitTime={0}\n".format(exitTime))    
elif msmt == 3:
    f.write("minIntrTIC={0}\n".format(minIntrTIC))
    
f.close()

### f = open("method.txt","r")
### print(f.read())

print("Done!")
