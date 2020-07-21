#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Created on Sun May 10 12:14:49 2020

@author: guannandong
"""

# isotopologue type tuple
isoType = ("Base","13C-substituted","D-substituted","18O-substituted",
            "15N-substituted","17O-substituted","Others")

# mass tolerance unit tuple
mtolUnitType = ("ppm","mmu")

# electron mass
emass = 0.00054858

# isotopologue input class
class Peak:
    def __init__(self, peakType, Formula, mass, mtol, unit):
        self.peakType = peakType
        self.chemicalFormula = Formula.formula
        self.mass = mass
        self.mtol = mtol
        self.unit = unit