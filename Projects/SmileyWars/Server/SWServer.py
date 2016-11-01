import os, json, datetime
import http.server
import socketserver
import SWWorld

w = SWWorld.World()

class myHTTPRequestHandler(http.server.SimpleHTTPRequestHandler):
	def __init__(self, request, client_address, server):
		self.world = w
		self.c_address = client_address
		super().__init__(request, client_address, server)
	def do_GET(self):
		if ('client' in self.path.lower()):
			super().do_GET();
		elif ('add' in self.path.lower()):
			col = self.world.add(200, 200, self.c_address[0])
			self.wfile.write(col.encode())
	
	def do_POST(self):
		if ('logic' in self.path.lower()):
			if ('Content-Length' in self.headers):
				dataLength = int(self.headers['Content-Length']);
				bOK = True
				strMessage = ''
				try:
					objData = json.loads((self.rfile.read(dataLength)).decode('utf-8'))
					self.world.setLogic(int(objData['index']), objData['code'])
				except BaseException as ex:
					bOK = False
					self.world.clearLogic(int(objData['index']))
					strMessage = str(type(ex))[8:-2] + ': ' + str(ex)
				finally:
					pass
				
				self.wfile.write(json.dumps({'status': bOK, 'message': strMessage}).encode())
		else:
			dataLength = int(self.headers['Content-Length'])
			fLastUpdate = float(self.rfile.read(dataLength).decode('utf-8'))
			if (fLastUpdate == 0.0):
				self.wfile.write(json.dumps(self.world.getState(mes=self.c_address)).encode());
			else:
				self.wfile.write(json.dumps(self.world.getState(datetime.datetime.fromtimestamp(fLastUpdate / 1000.0 + 0.001), mes=self.c_address)).encode())
			
			#self.wfile.write(json.dumps([self.world.digest() for i in range(30)]).encode());
		
	def do_HEAD(self):
		print('HEAD');

	
print('Initializng...');

os.chdir('..\\..');

httpd = socketserver.TCPServer(("", 8080), myHTTPRequestHandler);

print('Initialized!');

httpd.serve_forever();

os.system('PAUSE');