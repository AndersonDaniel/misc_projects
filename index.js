var globe;
var degtorad = Math.PI / 180;
var renderer;

function euler2quaternion(alpha, beta, gamma) {
	var _x = beta  ? beta  * degtorad : 0; // beta value
	var _y = gamma ? gamma * degtorad : 0; // gamma value
	var _z = alpha ? alpha * degtorad : 0; // alpha value

	var cX = Math.cos( _x/2 );
	var cY = Math.cos( _y/2 );
	var cZ = Math.cos( _z/2 );
	var sX = Math.sin( _x/2 );
	var sY = Math.sin( _y/2 );
	var sZ = Math.sin( _z/2 );

	var w = cX * cY * cZ - sX * sY * sZ;
	var x = sX * cY * cZ - cX * sY * sZ;
	var y = cX * sY * cZ + sX * cY * sZ;
	var z = cX * cY * sZ + sX * sY * cZ;

	return new THREE.Quaternion(x, y, z, w);
}


function do3dstuff() {
	var scene = new THREE.Scene();
	var camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);
	
document.body.appendChild(renderer.domElement);	renderer = new THREE.WebGLRenderer();
	renderer.setSize(window.innerWidth, window.innerHeight);
	//document.body.appendChild(renderer.domElement);


	var geometry = new THREE.SphereGeometry(2, 32, 32);
	var material = new THREE.MeshBasicMaterial({color: 0x00ff00});
	globe = new THREE.Mesh(geometry, material);
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
	camera.up.set(0, 1, 0);
	camera.lookAt(0, 0, 0);


	function animate() {
		requestAnimationFrame(animate);
		renderer.render(scene, camera);
	}

	animate();
}

var collected = [];

function makeQR(data) {
	var typeNumber = 4;
	var errorCorrectionLevel = 'L';
	var qr = qrcode(typeNumber, errorCorrectionLevel);
	qr.addData(data);
	qr.make();
	document.getElementById('qrcode').innerHTML = qr.createImgTag(7);
}

window.onload = function() {
	do3dstuff();
	var body = document.getElementsByTagName('body')[0];
	var peer = new Peer();
	peer.on('open', function(id) {
		console.log('Peer id: ', id);
		makeQR('https://andersondaniel.github.io/misc_projects/client.html#hid=' + id);
		peer.on('connection', function(conn) {
			document.body.appendChild(renderer.domElement);
			conn.on('data', function(data) {
				try {
					data = JSON.parse(data);
					// console.log('Received: ', data);
					// var new_div = document.createElement('div');
					// new_div.innerText = data;
					// body.appendChild(new_div);
					// collected.push(data);
					// globe.rotation.x = data.beta * Math.PI / 180.;
					// globe.rotation.y = data.alpha * Math.PI / 180.;
					// globe.rotation.z = -data.gamma * Math.PI / 180.;
					// globe.setRotationFromMatrix(euler2matrix(data.alpha, data.beta, data.gamma));
					globe.setRotationFromQuaternion(euler2quaternion(data.alpha, data.beta, data.gamma));
				} catch(error) { console.log('ERROR', error); }
			});

		});
	});
}
