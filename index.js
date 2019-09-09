window.onload = function() {
	var body = document.getElementsByTagName('body')[0];
	var peer = new Peer();
	peer.on('open', function(id) {
		console.log('Peer id: ', id);
		peer.on('connection', function(conn) {
			conn.on('data', function(data) {
				console.log('Received: ', data);
				var new_div = document.createElement('div');
				new_div.innerText = data;
				body.appendChild(new_div);
			});

		});
	});
}
