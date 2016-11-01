import math, time, ctypes
cx, cy, rx, ry = 670, 500, 400, 200
for i in range(100000000000000000000000000):
	print(str(cx + rx * math.cos(i)) + ', ' + str(cy + ry * math.sin(i)))
	ctypes.windll.user32.SetCursorPos(int(cx + rx * math.cos(i * 0.1)), int(cy + ry * math.sin(i * 0.2)))
	time.sleep(0.05)