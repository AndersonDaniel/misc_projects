function do3dstuff() {
	var scene = new THREE.Scene();
	var camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);
	
	var renderer = new THREE.WebGLRenderer();
	renderer.setSize(window.innerWidth / 2., window.innerHeight / 2.);
	document.body.appendChild(renderer.domElement);


	var geometry = new THREE.SphereGeometry(2, 32, 32);
	var material = new THREE.MeshBasicMaterial({color: 0x00ff00});
	var globe = new THREE.Mesh(geometry, material);
	scene.add(globe);


	var loader = new THREE.TextureLoader();
	loader.load('earth_texture.jpg', function(texture) {
		globe.material.map = texture;
		globe.material.overdraw = 0.5;
		globe.material.color = undefined;
		globe.material.needsUpdate = true;
	});

	var light = new THREE.PointLight(0xffffff, 1, 100);
	light.position.set(3, 3, 3);
	scene.add(light);

	camera.position.z = 5;

	function animate() {
		globe.rotation.x += 0.01;
		globe.rotation.y += 0.01;
		requestAnimationFrame(animate);
		renderer.render(scene, camera);
	}

	animate();
}



window.onload = function() {
	do3dstuff();
	var body = document.getElementsByTagName('body')[0];
	var peer = new Peer();
	var collected = [];
	peer.on('open', function(id) {
		console.log('Peer id: ', id);
		peer.on('connection', function(conn) {
			conn.on('data', function(data) {
				// console.log('Received: ', data);
				// var new_div = document.createElement('div');
				// new_div.innerText = data;
				// body.appendChild(new_div);
				collected.push(data);
			});

		});
	});
}
