import math, time

CONST_WIDTH = 300
CONST_HEIGHT = int(CONST_WIDTH / 2)

def is_black_2(width, height, row, col, depth):
	from_side = abs(width / 2 - col)
	return (row >= height / 2 and from_side < height - row)
	
def is_black(width, height, row, col, depth):
	height_part = height / math.pow(2, depth)
	width_part = width / math.pow(2, depth)
	row_index = int(row / height_part)
	col_index = int(col / width_part)
	from_side = min(abs(col - col_index * width_part), abs(col - (col_index + 1) * width_part))
	return (row_index % 2 == 1 and from_side < height_part - (row - row_index * height_part))

def draw_sier(max_depth):
	for i in range(CONST_HEIGHT):
		for j in range(CONST_WIDTH):
			from_side = abs(CONST_WIDTH / 2 - j)
			if (from_side > i or ([is_black(CONST_WIDTH, CONST_HEIGHT, i, j, k) for k in range(1, max_depth + 1)].count(True) > 0)):
				print(' ', end='')
			else:
				print('*', end='')
		print()
		
for i in range(7):
	draw_sier(i)
	time.sleep(2)