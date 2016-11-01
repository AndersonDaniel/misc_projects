var scene, camera, renderer, particleSystem;
var CAM_RADIUS;
var p = {};
var objects = [];
var nInverted = 1;
mouse = new THREE.Vector2();
raycaster = new THREE.Raycaster();
CONST_SPEED_FACTOR = 0.25;

function updateScene(planets) {
	planets.earth.rotation.y += 0.1;
	for (planet in planets) {
		if (planets.hasOwnProperty(planet) && planet != 'sun') {
			planets[planet] = planets[planet];
			if (!planets[planet].hasOwnProperty('radius')) {
				planets[planet].radius = 
					Math.sqrt(Math.pow(planets.sun.position.x - planets[planet].position.x, 2) +
							  Math.pow(planets.sun.position.y - planets[planet].position.y, 2) + 
							  Math.pow(planets.sun.position.z - planets[planet].position.z, 2));
				planets[planet].angle = Math.atan2(planets[planet].position.z - planets.sun.position.z, planets[planet].position.x - planets.sun.position.x) * (360 / (2 * Math.PI));
				
				var lineMaterial = new THREE.LineBasicMaterial({color: 0xffffff});
				var lineGeometry = new THREE.CircleGeometry(planets[planet].radius, 64);
				lineGeometry.vertices.shift();
				var lineMesh = new THREE.Line(lineGeometry, lineMaterial)
				lineMesh.position.set(planets.sun.position.x, planets.sun.position.y, planets.sun.position.z);
				lineMesh.rotateOnAxis(new THREE.Vector3(1, 0, 0), Math.PI / 2);
				scene.add(lineMesh);
			}
			
			planets[planet].angle += (planets[planet].clockwise ? 1 : (-1)) * CONST_SPEED_FACTOR * planets[planet].d_ang;
			
			planets[planet].position.x = Math.cos((2 * Math.PI / 360) * planets[planet].angle) * planets[planet].radius + planets.sun.position.x;
			planets[planet].position.z = Math.sin((2 * Math.PI / 360) * planets[planet].angle) * planets[planet].radius + planets.sun.position.z;
		}
	}
	
	for (var nIndex = 0; nIndex < particleSystem.geometry.vertices.length; nIndex++) {
		var curr = particleSystem.geometry.vertices[nIndex];
		if (Math.sqrt(Math.pow(curr.x - planets.sun.position.x, 2) + 
					  Math.pow(curr.y - planets.sun.position.y, 2) + 
					  Math.pow(curr.z - planets.sun.position.z, 2)) > 500) {
			curr.v = planets.sun.position.clone().sub(curr).normalize().multiplyScalar(Math.random() * 0.5);
		}
		
		curr.x += curr.v.x;
		curr.y += curr.v.y;
		curr.z += curr.v.z;
		
		curr.v.x += Math.random() * 0.1 - 0.05;
		curr.v.y += Math.random() * 0.1 - 0.05;
		curr.v.z += Math.random() * 0.1 - 0.05;
	}
	
	particleSystem.geometry.verticesNeedUpdate = true;
}

function init() {
	scene = new THREE.Scene();
	var WIDTH = $(window).width() - 20, HEIGHT = $(window).height() - 50;
	renderer = new THREE.WebGLRenderer({antialias: true});
	renderer.setSize(WIDTH, HEIGHT);
	$('body').append(renderer.domElement);
	
	camera = new THREE.PerspectiveCamera(45, WIDTH / HEIGHT, 0.1, 20000);
	
	camera.position.set(0, 110, 50);
	camera.lookAt(new THREE.Vector3(0, 0, -200));
	
	scene.add(camera);
	
	//renderer.setClearColor(0x1C1C1C, 1);
	//renderer.setClearColor(0x272068, 1);
	renderer.setClearColor(0x1A0D4E, 1);
	
	var light = new THREE.PointLight(0xffffff);
	
	light.position.set(0, 100, 0);
	
	scene.add(light);
	
	p = makePlanets();
	
	for (var planet in p) {
		if (p.hasOwnProperty(planet)) {
			scene.add(p[planet]);
		}
	}

	CAM_RADIUS = Math.sqrt(Math.pow(p.sun.position.x - camera.position.x, 2) +
						   Math.pow(p.sun.position.y - camera.position.y, 2) +
						   Math.pow(p.sun.position.z - camera.position.z, 2));
						   
	createStars();
	
	return (p);
}

function createStars() {
	var starsAmount = 1250,
		starsG = new THREE.Geometry(),
		starsM = new THREE.ParticleBasicMaterial({
			color: 0xE1E1E1,
			size: 5,
			map: THREE.ImageUtils.loadTexture('particle2.png'),
			blending: THREE.AdditiveBlending,
			transparent: true
			});
	for (var p = 0; p < starsAmount; p++) {
		var pX = Math.random() * 500 - 250,
			pY = Math.random() * 500 - 250,
			pZ = Math.random() * 500 - 450,
			particle = new THREE.Vector3(pX, pY, pZ);
			particle.v = new THREE.Vector3(Math.random() - 0.5, Math.random() - 0.5, Math.random() - 0.5);
		
		starsG.vertices.push(particle);
	}
	
	particleSystem = new THREE.ParticleSystem(starsG, starsM);
	
	scene.add(particleSystem);
}

function makePlanetGen(radius, material, x, y, z, d_ang, name, materialCtor) {
	var geometry = new THREE.SphereGeometry(radius, 32, 32);
	geometry.computeFaceNormals();
	geometry.computeVertexNormals();
	var material = new materialCtor(material);
	planet = new THREE.Mesh(geometry, material);
	planet.position.set(x, y, z);
	planet.d_ang = d_ang;
	planet.name = name;
	objects.push(planet);
	return (planet);
}

function makePlanet(radius, color, x, y, z, d_ang, name) {
	//return (makePlanetGen(radius, {color: color}, x, y, z, d_ang, name, THREE.MeshLambertMaterial));
	return (makePlanetGen(radius, {color: color}, x, y, z, d_ang, name, THREE.MeshPhongMaterial));
}

function makePlanets() {
	var sunTexture = THREE.ImageUtils.loadTexture('sun.jpg');
	p.sun = makePlanetGen(15, {map: sunTexture}, 0, 0, -200, 0, 'sun', THREE.MeshBasicMaterial);
	p.mercury = makePlanet(2, 0x999999, 30, 0, -200, 2, 'mercury');
	p.venus = makePlanet(1.5, 0x996600, 35, 0, -200, -4, 'venus');
	p.earth = makePlanetGen(4, {map: THREE.ImageUtils.loadTexture('earth.jpg')}, 46, 0, -200, 2.4, 'earth', THREE.MeshPhongMaterial);
	p.earth.rotation.z = Math.PI / 6;
	p.mars = makePlanet(3, 0x800000, 55, 0, -200, 5, 'mars');
	p.jupiter = makePlanet(3.5, 0x009999, 62, 0, -200, -3, 'jupiter');
	p.saturn = makePlanet(4.2, 0xFF0000, 70, 0, -200, 2.5, 'saturn');
	p.uranus = makePlanet(2.5, 0x00CCFF, 80, 0, -200, 6, 'uranus');
	p.neptune = makePlanet(3.5, 0x66FFCC, 95, 0, -200, -2, 'neptune');
	p.pluto = makePlanet(2, 0xF9F9F9, 105, 0, -200, -5, 'pluto');
	
	return (p);
}

function render() {	
	requestAnimationFrame(render);
	renderer.render(scene, camera);
}

$(document).ready(function() {
	
	init();
	
	setInterval(function() {
		updateScene(p);
	}, 20);
	
	render();
});

$(window).on('mousewheel', function(e) {
	ZOOM_AMOUNT = 1.1;
	if (e.originalEvent.deltaY < 0) {
		camera.fov /= ZOOM_AMOUNT;
	} else {
		//camera.fov *= ZOOM_AMOUNT;
		camera.fov = Math.min(130, camera.fov * ZOOM_AMOUNT);
	}
	
	console.log(camera.fov);
	
	camera.updateProjectionMatrix();
});

function moveH(nHowMuch) {
	currRadius = Math.sqrt(Math.pow(p.sun.position.x - camera.position.x, 2) +
						   Math.pow(p.sun.position.z - camera.position.z, 2));
	currAng = Math.atan2(camera.position.z - p.sun.position.z,
						 camera.position.x - p.sun.position.x);
	
	nDiff = nHowMuch * (0.25) * (Math.PI / 180);
	currAng += nDiff;
	
	camera.position.x = Math.cos(currAng) * currRadius + p.sun.position.x;
	camera.position.z = Math.sin(currAng) * currRadius + p.sun.position.z;
	camera.lookAt(new THREE.Vector3(0, 0, -200));
}

function moveV(nHowMuch) {
	currAngPlain = Math.atan2(camera.position.z - p.sun.position.z,
							  camera.position.x - p.sun.position.x);
	
	c = Math.sqrt(Math.pow(CAM_RADIUS, 2) - 
				  Math.pow(camera.position.y, 2));
						  
	currAngY = Math.atan2(camera.position.y - p.sun.position.y,
						  c);
	
	nDiff = nHowMuch * (0.15) * (Math.PI / 180);
	
	currAngY += nDiff;
	currAngY = (Math.PI / 180) * (((180 / Math.PI) * currAngY) % 360);
	
	/*// ADDITION {
	
	if (Math.abs(currAngY) >= Math.PI / 2) {
		nInverted = -1;
		//console.log('inv');
	} else {
		nInverted = 1;
		//console.log('uninv');
	}
	
	// }  */
	
	camera.position.y = Math.sin(currAngY) * CAM_RADIUS + p.sun.position.y;
	newRadius = nInverted * Math.abs(Math.cos(currAngY) * CAM_RADIUS);
						  
	camera.position.x = Math.cos(currAngPlain) * newRadius + p.sun.position.x;
	camera.position.z = Math.sin(currAngPlain) * newRadius + p.sun.position.z;
	//camera.up.set(0, 1, 0);
	camera.lookAt(new THREE.Vector3(0, 0, -200));
	setCameraUp();
	/*if (Math.abs(camera.rotation.z) > 0.1) {
		camera.rotation.z = 0;
	}*/

	c = Math.sqrt(Math.pow(CAM_RADIUS, 2) - 
				  Math.pow(camera.position.y, 2));
						  
	newAngY = Math.atan2(camera.position.y - p.sun.position.y,
						  c);
	
	//console.log(Math.abs(currAngY - newAngY) * 180 / Math.PI);
}

function setCameraUp() {
		dAngle = Math.atan2(camera.position.z - p.sun.position.z,
							camera.position.x - p.sun.position.x) - Math.PI / 2;
		var temp = new THREE.Vector3(p.sun.position.x, p.sun.position.y, p.sun.position.z);
		temp.x += Math.cos(dAngle);
		temp.z += Math.sin(dAngle);
		
		temp.sub(p.sun.position);
		
		var toSun = camera.position.clone();
		toSun.sub(p.sun.position);
		
		temp.crossVectors(toSun, temp);
		
		temp.normalize();
		
		camera.up = temp;
}

function handleClickPlanet(x, y) {
	mouse.x = (x / renderer.domElement.width) * 2 - 1;
	mouse.y = - (y / renderer.domElement.height) * 2 + 1;
	
	raycaster.setFromCamera(mouse, camera);
	
	var intersects = raycaster.intersectObjects(objects);
	
	if (intersects.length > 0) {
		console.log(intersects[0].object.name);
	}
} 

$(window).on('mousedown', function(e) {
	startX = e.clientX;
	startY = e.clientY;
	handleClickPlanet(startX, startY);
	$(window).on('mousemove', function(e) {
		currX = e.clientX;
		currY = e.clientY;
		moveH(currX - startX);
		moveV(currY - startY);
		startX = currX;
		startY = currY;
	});
	$(window).on('mouseup', function() {
		$(window).off('mousemove');
	});
});