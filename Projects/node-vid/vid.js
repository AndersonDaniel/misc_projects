var http = require('http'),
	fs = require('fs'),
	util = require('util');
	
//var path = 'KTBM.mp4';
var path = 'KTBM.ogg';
//var path = '2.mp4';
var port = 8888;
var host = '';

http.createServer(function(req, res) {
	var stat = fs.statSync(path);
	var total = stat.size;
	
	if (req.headers.range) {
		var range = req.headers.range;
		var parts = range.replace(/bytes=/, '').split('-');
		var partialstart = parts[0];
		var partialend = parts[1];
		var start = parseInt(partialstart, 10);
		var end = partialend? parseInt(partialend, 10) : total - 1;
		var chunksize = (end - start) + 1;
		console.log('RANGE: ' + start + ' - ' + end + ' = ' + chunksize);
		
		var file = fs.createReadStream(path, {start: start, end: end});
		res.writeHead(206, {'Content-Range': 'bytes ' + start + '-' + end + '/' + total,
							'Accept-Ranges': 'bytes',
							'Access-Control-Allow-Origin':'*',
							'Content-Length': chunksize,
							'Content-Type': 'video/ogg'});
		file.pipe(res);
	} else {
		console.log('ALL: ' + total);
		res.writeHead(200, {'Accept-Ranges': 'bytes',
							'Access-Control-Allow-Origin':'*',
							'Content-Length': total,
							'Content-Type': 'video/ogg'});
		fs.createReadStream(path).pipe(res);
	}
	
}).listen(port, host);

console.log('=====DONE======');