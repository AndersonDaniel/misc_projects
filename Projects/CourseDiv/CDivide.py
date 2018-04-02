#CDivide.py

import math, random, functools, copy

CONST_NUM_FREQ = 10
CONST_SURVIVAL_SIZE = 60
CONST_MUTATIONS = 250
CONST_OFFSPRINGS = 250
CONST_GENERATIONS = 2000

# Init random
random.seed()

# Does everything
def main():
	strFileName = input('Enter file name: ')
	nDivisions = int(input('How many divisions? '))
	DivideCourses(LoadDataFromFile(strFileName), nDivisions)

# Reads a file
def LoadDataFromFile(strFileName):
	objFile = open(strFileName, 'r')
	strData = objFile.read()
	objFile.close()
	return (strData)

# Formats and saves course division to a file ('res.csv' in the script's directory)
def SaveDivisionToFile(dicDivision, strIDField):
	objFile = open('res.csv', 'w')
	objFile.write('ID,div\n')
	
	# For each division, create a list that contains for each student in the division his ID and the division number. Then unite all the lists
	# (with functools.reduce), then iterate over the entire list and create the line 'ID,division' for each student. Write all the lines to the file.
	objFile.write('\n'.join([str(s[strIDField]) + ',' + str(s['#']) for s in functools.reduce(lambda l1,l2: l1+l2, [[{strIDField: s[strIDField], '#': k} for s in v] for k,v in dicDivision.items()])]))
	objFile.close()

# Divides the students to the number of required divisions, and then saves the results to a file.
def DivideCourses(data, divisions):
	# Get data matrix from the text. The text contains lines of students, with a comma between each two criteria values.
	# The first line contains the names of the criteria.
	dataMatrix = [line.split(',') for line in data.replace('\r', '').split('\n') if line != '']
	
	# Get criteria list
	lstCrits = dataMatrix[0]
	
	# Slice the matrix and take only data (without criteria)
	dataMatrix = dataMatrix[1:]
	
	# Get list of students from matrix; each student is represented as a dictionary of his criteria values
	lstAllStudents = [{lstCrits[i]: int(student[i]) if i > 0 and student[i].isnumeric() else student[i] for i in range(len(student))} for student in dataMatrix]
	
	# Get ID field name (first field in the criteria list)
	strIDField = lstCrits[0]
	
	# Get field name of the field that contains the fixed division (last field in the criteria list)
	strFixedField = lstCrits[len(lstCrits) - 1]
	
	# Get only criteria that is relevant for the division
	lstCrits = lstCrits[1:-1]
	
	# Calculate how many students should be in each division
	lstPerDivision = [math.floor(len(lstAllStudents) / divisions) + (1 if i < len(lstAllStudents) - divisions * math.floor(len(lstAllStudents) / divisions) else 0) for i in range(divisions)]
	
	# Call the genetic algorithm and save the solution to a file
	SaveDivisionToFile(GenAlgo(lstAllStudents, lstPerDivision, strIDField, strFixedField, lstCrits), strIDField)

# Genetic algorithm that finds a division solution
def GenAlgo(lstStudents, lstPerDivision, strIDField, strFixedField, lstCrits):
	# Generate many candidate solutions
	lstCandidates = [GenCandidate(lstStudents, lstPerDivision, strIDField, strFixedField) for i in range(CONST_MUTATIONS + CONST_OFFSPRINGS)]
	
	# Sort candidates according to grade (by criteria list)
	lstCandidates.sort(key=GenSolutionGrader(strIDField, lstCrits))
	
	# Run some generations
	for i in range(CONST_GENERATIONS):
		# Get best candidates + some random candidates
		lstPicked = lstCandidates[:CONST_SURVIVAL_SIZE] + random.sample(lstCandidates[CONST_SURVIVAL_SIZE:], int(CONST_SURVIVAL_SIZE / 4))
		lstCandidates = copy.deepcopy(lstPicked)
		
		# Generate some mutations according to selected candidates
		for i in range(CONST_MUTATIONS):
			lstCandidates.append(Mutate(random.choice(lstPicked), strFixedField))
			
		# Generate some offsprings (matching) from selected candidates
		for i in range(CONST_OFFSPRINGS):
			lstPair = random.sample(lstPicked, 2)
			lstCandidates.append(Match(lstPair[0], lstPair[1], strIDField, strFixedField))
		
		# Sort candidates according to grade (by criteria list)
		lstCandidates.sort(key=GenSolutionGrader(strIDField, lstCrits))
		
	# Get best solution (with lowest grade)
	return (lstCandidates[0])

# Generates a random division solution
def GenCandidate(lstStudents, lstPerDivision, strIDField, strFixedField):
	# Generate initial dictionary to contain division - start with students whose division is fixed
	dicDivisions = {i+1: [objStudent for objStudent in lstStudents if objStudent[strFixedField] == i + 1] for i in range(len(lstPerDivision))}
	
	# Place each unplaced student in a random division
	for objStudent in [objStudent for objStudent in lstStudents if objStudent not in functools.reduce(lambda l1,l2: l1+l2, dicDivisions.values())]:
		nDiv = random.randint(0, len(lstPerDivision) - 1)
		while len(dicDivisions[nDiv + 1]) == lstPerDivision[nDiv]:
			nDiv = random.randint(0, len(lstPerDivision) - 1)
		dicDivisions[nDiv + 1].append(objStudent)
	return dicDivisions

# Creates a function that grades a solution according to criteria order
def GenSolutionGrader(strIDField, lstCrits):
	# Grade a solution according to criteria order
	def GradeSolution(dicDivisions):
		nGrade = 0
		
		# Initially the weight is 10^#CRITERIA, and for each less important criteria it decreases by an order of magnitude
		nWeight = math.pow(10, len(lstCrits))
		for strField in lstCrits:
			nGrade += nWeight * MeasureField(dicDivisions, strField)
			nWeight /= 10
		
		return nGrade
	return GradeSolution

# Measures how well a certain field is divided among the divisions
def MeasureField(dicDivisions, strField):
	# Get for each division the list of the specified field's values
	dicDivisionValues = {k: [objStud[strField] for objStud in v] for k, v in dicDivisions.items()}
	
	# Get a united list of all field values
	lstAllValues = functools.reduce(lambda l1, l2: l1 + l2, dicDivisionValues.values())
	
	# ...
	nMin = min(lstAllValues)
	nMax = max(lstAllValues)
	
	# Define the interval in which we want to measure the frequency
	fInterval = (nMax - nMin) / CONST_NUM_FREQ
	
	# Get frequency table of the field in the divisions
	dicDivisionFreq = {k: GetFreqTable(nMin, fInterval, CONST_NUM_FREQ, v) for k, v in dicDivisionValues.items()}
	
	# Calculate average frequency table
	dicAverageFreq = {k: sum([dicDivision[k] for dicDivision in dicDivisionFreq.values()]) / len(dicDivisionFreq) for k in list(dicDivisionFreq.values())[0].keys()}
	
	# The grade is the sum of the distances between each division's frequency table to the average frequency table
	return (sum([sum([abs(dicDivision[k] - dicAverageFreq[k]) for k in dicAverageFreq.keys()]) for dicDivision in dicDivisionFreq.values()]))

# ...
def GetFreqTable(nStart, fInterval, nNumValues, lstValues):
	return {nStart + i * fInterval: len([ nValue for nValue in lstValues if nStart + i * fInterval <= nValue < nStart + (i + 1) * fInterval ]) for i in range(nNumValues) }

# Take a solution and change it a bit
# We mutate by swapping two students between their divisions
def Mutate(dicSolution, strFixedField):
	dicRes = copy.deepcopy(dicSolution)
	
	# Pick divisions from which to swap
	divFirst, divSecond = random.sample(list(dicRes.values()), 2)
	
	# Pick students to swap - remember to take the fixed division into account
	objFirstStud = random.choice([s for s in divFirst if s[strFixedField] == ''])
	objSecondStud = random.choice([s for s in divSecond if s[strFixedField] == ''])
	
	# Swap and return mutated solution
	divFirst.remove(objFirstStud)
	divFirst.append(objSecondStud)
	divSecond.remove(objSecondStud)
	divSecond.append(objFirstStud)
	return (dicRes)
	
# Merges between two solutions
# We merge by taking one solution entirely, then placing a random student
# in the division he is in the second solution
def Match(dicFirstSol, dicSecondSol, strIDField, strFixedField):
	# Randomly select "main" solution
	temp = [dicFirstSol, dicSecondSol]
	random.shuffle(temp)
	dicFirstSol, dicSecondSol = temp
	
	# ...
	dicRes = copy.deepcopy(dicFirstSol)
	
	# Choose student to swap
	nFirstDiv = random.choice(list(dicFirstSol.keys()))
	nSecondDiv = None
	objStud = random.choice([s for s in dicRes[nFirstDiv] if s[strFixedField] == ''])
	
	# We try to select a division in the second solution which doesn't have the selected student;
	# otherwise, the student is in the same division in both solutions, in which case we need
	# to select a different student
	while nSecondDiv == None:
		try:
			nSecondDiv = random.choice([nDiv for nDiv in list(dicSecondSol.keys()) if len([s for s in dicSecondSol[nDiv] if s[strIDField] == objStud[strIDField]]) == 0 and nDiv != nFirstDiv])
		except:
			objStud = random.choice([s for s in dicRes[nFirstDiv] if s[strFixedField] == ''])
	
	# Swap student with a randomly selected student from the second division, and return solution
	objSecondStud = random.choice([s for s in dicRes[nSecondDiv] if s[strFixedField] == ''])
	dicRes[nFirstDiv].remove(objStud)
	dicRes[nFirstDiv].append(objSecondStud)
	dicRes[nSecondDiv].remove(objSecondStud)
	dicRes[nSecondDiv].append(objStud)
	return (dicRes)

# Do everything
main()
