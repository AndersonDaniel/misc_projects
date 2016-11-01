# MapLogic.py

import math, collections

# Implement Dijkstra (/ A*) search on the graph, given the points
def DijkstraShortestPath(lstLocs, lstRoads, nFrom, nTo):
	unknown = list(lstLocs)
	start = [node for node in lstLocs if node['index'] == int(nFrom)][0]
	end = [node for node in lstLocs if node['index'] == int(nTo)][0]
	unknown.remove(start)
	dists = collections.defaultdict(lambda: float('inf'))
	parents = {}
	for x in GetNeighbours(lstLocs, lstRoads, int(nFrom)):
		dists[x['index']] = Dist(start, x) + Dist(x, end)
		parents[x['index']] = start
	curr = start
	while (curr['index'] != int(nTo)):
		unknown.sort(key=lambda node: dists[node['index']])
		curr = unknown[0]
		for x in GetNeighbours(lstLocs, lstRoads, curr['index']):
			curr_dist = Dist(curr, x) + dists[curr['index']] + Dist(x, end)
			if (curr_dist < dists[x['index']]):
				dists[x['index']] = curr_dist
				parents[x['index']] = curr
		unknown.remove(curr)
	
	res = []
	while (curr['index'] != int(nFrom)):
		res.insert(0, curr)
		curr = parents[curr['index']]
	res.insert(0, start)
	return res;

# Get a node's neighbours
def GetNeighbours(lstLocs, lstRoads, nCurr):
	curr_node = [node for node in lstLocs if node['index'] == nCurr][0]
	nums = [r[0] for r in lstRoads if r[1] == curr_node['index']] + [r[1] for r in lstRoads if r[0] == curr_node['index']]
	return [node for node in lstLocs if node['index'] in nums]

# Calculate distance between nodes
def Dist(node1, node2):
	return (math.sqrt((node1['x'] - node2['x'])**2 + (node1['y'] - node2['y'])**2))