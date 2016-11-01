from ctypes import *
import os, random, time, math
class POINT(Structure):
	_fields_ = [('x', c_int), ('y', c_int)]
MAX_X, MAX_Y, MIN_X, MIN_Y = windll.user32.GetSystemMetrics(0) - 100, windll.user32.GetSystemMetrics(1) - 100, 20, 20
TIME_WAIT = 0.01
NORMAL_MOVE_FREQ = 20
NORMAL_SPEED, SCARE_SPEED = 15, 50
SCARE_DIST, WIN_DIST = 320.0, 100.0
desk = windll.user32.GetWindow(windll.user32.GetWindow(windll.user32.FindWindowW('ProgMan', 0), 5), 5)
LVM_GETITEMCOUNT, LVM_SETITEMPOSITION = 0x1000 + 4, 0x1000 + 15
total = windll.user32.SendMessageW(desk, LVM_GETITEMCOUNT, 0, 0)
locs = [(random.randint(0, MAX_X), random.randint(0, MAX_Y)) for i in range(total)]
mouseLoc = POINT()
cx,cy = sum([p[0] for p in locs]) / total, sum([p[1] for p in locs]) / total
max_dist = max([math.sqrt((p[0] - cx) ** 2 + (p[1] - cy) ** 2) for p in locs])
while max_dist > WIN_DIST:
	for i in range(total):
		locx = locs[i][0]
		locy = locs[i][1]
		windll.user32.GetCursorPos(byref(mouseLoc))
		angle = speed = 0
		if (math.sqrt((locx - mouseLoc.x) ** 2 + (locy - mouseLoc.y) ** 2) <= SCARE_DIST):	
			angle = math.atan2(locy - mouseLoc.y, locx - mouseLoc.x)
			speed = SCARE_SPEED
		elif (random.randint(0, 20) == 19):
			angle = random.random() * 2 * math.pi
			speed = NORMAL_SPEED
		locx += int(speed * math.cos(angle))
		locy += int(speed * math.sin(angle))
		if (locx < MIN_X):
			locx = MAX_X - 1
		elif (locx >= MAX_X):
			locx = MIN_X
		if (locy < MIN_Y):
			locy = MAX_Y - 1
		elif (locy >= MAX_Y):
			locy = MIN_Y
		locs[i] = (locx, locy)
		windll.user32.SendMessageW(desk, LVM_SETITEMPOSITION, i, locs[i][1] << 16 | locs[i][0])
	cx,cy = sum([p[0] for p in locs]) / total, sum([p[1] for p in locs]) / total
	max_dist = max([math.sqrt((p[0] - cx) ** 2 + (p[1] - cy) ** 2) for p in locs])
r = total * 45.0 / (2 * math.pi)
for i in range(total):
	angle = i * (2 * math.pi) / total
	x = int((MAX_X + 100) / 2 + r * math.cos(angle))
	y = int((MAX_Y + 100) / 2 + r * math.sin(angle))
	windll.user32.SendMessageW(desk, LVM_SETITEMPOSITION, i, y << 16 | x)