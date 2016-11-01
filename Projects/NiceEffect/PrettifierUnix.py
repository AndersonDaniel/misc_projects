# Effect.py

import random
import time
import math

class Prettifier(object):
	def __init__(self):
		self.RADIUS = 10
		self.ANG_DIFF = 130
		self.ANG_ADV = 12
		self.MIN_DIST = 18
		self.nAngle = 0
		self.NORMAL = '\033[0m'
		self.BRIGHT = '\033[97m'
		self.DARK = '\033[90m'
	def GetNextLine(self):
		c = random.choice(['G', 'C', 'T', 'N'])
		while (c == chr(7) or c.isspace()):
			c = chr(random.randint(0, 255))
		nExtra = int(self.RADIUS * (math.sin(math.radians(self.nAngle)) + 1))
		c = c.rjust(nExtra + self.MIN_DIST)
		c = c.ljust(self.MIN_DIST + self.RADIUS * 2)
		l = list(c)
		sec_index = self.MIN_DIST + int(self.RADIUS * (math.sin(math.radians(self.nAngle) + self.ANG_DIFF) + 1))
		l[sec_index] = '\''
		####################################
		
		first = self.NORMAL
		if (abs(self.nAngle) <= 30):
			first = self.BRIGHT
		elif (abs(self.nAngle - 180) <= 30):
			first = self.DARK
		
		second = self.NORMAL
		if (abs((self.nAngle + self.ANG_DIFF) % 360) <= 30):
			second = self.BRIGHT
		elif (abs((self.nAngle + self.ANG_DIFF) % 360 - 180) <= 30):
			second = self.DARK
		
		l.insert(self.MIN_DIST + nExtra - 1, first)
		l.insert(self.MIN_DIST + nExtra + 1, self.NORMAL)
		
		insert_index = sec_index
		if (sec_index > nExtra + self.MIN_DIST):
			insert_index += 2
		
		l.insert(insert_index, second)
		l.insert(insert_index + 2, self.NORMAL)
		
		####################################
		c = ''.join(l)
		self.nAngle += self.ANG_ADV
		self.nAngle = self.nAngle % 360
		return c