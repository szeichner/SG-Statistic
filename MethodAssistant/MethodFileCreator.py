#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Created on Sun May 10 13:37:35 2020

@author: guannandong
"""

def MethodFileCreator(measurement,polarity,peakCounter,peakGroup,cullingCriteria):
    # method file including everything
    f = open("method.txt","w")
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

    f.write("measurementType={0}\n".format(measurement))
    f.write("polarityMode={0}\n".format(polarity))
    if measurement == 'GC-reservoir':
        f.write("minIntensity={0}\n".format(cullingCriteria.minIntensity))
    elif measurement == 'ESI-syringe pump':
        f.write("startTime={0}\n".format(cullingCriteria.startTime))
        f.write("stopTime={0}\n".format(cullingCriteria.stopTime))
    elif measurement == 'GC direct elution':
        f.write("arrivalTime={0}\n".format(cullingCriteria.arrivalTime))
        f.write("exitTime={0}\n".format(cullingCriteria.exitTime))    
    elif measurement == 'small sample':
        f.write("minIntrTIC={0}\n".format(cullingCriteria.minIntrTIC))
    
    f.close()

    ### f = open("method.txt","r")
    ### print(f.read())
    