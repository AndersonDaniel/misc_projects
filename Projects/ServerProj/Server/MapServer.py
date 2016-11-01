# MapServer.py

import json, os, http.server, socketserver
import MapLogic

# Extending Python basic web server
class myHTTPRequestHandler(http.server.SimpleHTTPRequestHandler):
	def __init__(self, request, client_address, server):
		super().__init__(request, client_address, server)
		
	# Handle GET request
	def do_GET(self):
		# Get client files (via base classes)
		if ('client' in self.path.lower()):
			super().do_GET()
		# Get junction locations
		elif 'getLocs' in self.path:
			objFile = open('ServerProj\\Server\\data\\locs.json')
			strData = objFile.read()
			objFile.close()
			self.wfile.write(strData.encode())
		# Get roads
		elif 'getRoads' in self.path:
			objFile = open('ServerProj\\Server\\data\\roads.json')
			strData = objFile.read()
			objFile.close()
			self.wfile.write(strData.encode())
	
	# Handle POST request
	def do_POST(self):
		# Get path from junction to junction
		if 'getPath' in self.path:
			if ('Content-Length' in self.headers):
				nDataLength = int(self.headers['Content-Length']);
				objData = json.loads((self.rfile.read(nDataLength)).decode('utf-8'))
				objLocsFile = open('ServerProj\\Server\\data\\locs.json')
				objRoadsFile = open('ServerProj\\Server\\data\\roads.json')
				locs = json.loads(objLocsFile.read())
				roads = json.loads(objRoadsFile.read())
				objLocsFile.close()
				objRoadsFile.close()
				self.wfile.write(json.dumps(MapLogic.DijkstraShortestPath(locs, roads, objData['from'], objData['to'])).encode())


# Change current working directory to make client files easily accessible
os.chdir('..\\..');

# Setup web server and start serving
httpd = socketserver.TCPServer(("", 8080), myHTTPRequestHandler);
print('Ready to serve!')
httpd.serve_forever();