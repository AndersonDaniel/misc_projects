function connect() {
	var remote_id = document.getElementById("remote_id").value;
	var peer = new Peer();
	peer.on('open', function(id) {
		var conn = peer.connect(remote_id);
		conn.on('open', function() {
			conn.on('data', function(data) {
				console.log('Received: ', data);
			});
			
			var t1 = new Date();
			conn.send('Hello, host!');
			window.addEventListener('deviceorientation', function(event) {
				var t2 = new Date();
				conn.send(JSON.stringify({alpha: event.alpha, beta: event.beta, gamma: event.gamma, dt_ms: t2 - t1}));
			}, true);

		});
	});
}

window.onload = function() {
	console.log('ook client')
}
