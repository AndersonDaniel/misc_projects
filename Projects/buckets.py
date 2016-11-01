# Node is tuple ([0], [1])
# [0] = amount of water in big bucket
# [1] = amount of water in small bucket
def neighbours(node, big_max, small_max):
	first_to_second = min(node[0], small_max - node[1])
	second_to_first = min(node[1], big_max - node[0])
	return [
		(node[0], 0),
		(0, node[1]),
		(node[0], small_max),
		(big_max, node[1]),
		(node[0] - first_to_second, node[1] + first_to_second),
		(node[0] + second_to_first, node[1] - second_to_first)
	]

def find_opts(state, prev, big, small):
	return list(set([opt for opt in neighbours(state, big, small) if str(opt) not in prev and opt != state]))

def trace(state, prev):
	path = [state]
	while prev[str(state)] != None:
		state = prev[str(state)]
		path.insert(0, state)
	return path
	
a, b = map(int, [input('Enter first bucket size: '), input('Enter second bucket size: ')])
big, small = max(a, b), min(a, b)
dest = int(input('Enter destination: '))

state = (0, 0)
prev = {str(state) : None}
to_examine = [state]
while len(to_examine) > 0 and state[0] != dest and state[1] != dest:
	state = to_examine.pop(0)
	new_opts = find_opts(state, prev, big, small)
	for new_opt in new_opts:
		prev[str(new_opt)] = state
	to_examine += new_opts

if (state[0] != dest and state[1] != dest):
	print('Impossible!')
else:
	for node in trace(state, prev):
		print(node, end = ' -> ')
	print('Done!')
input('Press ENTER to exit')