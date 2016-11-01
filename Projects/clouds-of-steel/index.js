var scene, camera, renderer;
var _planes = [];
var _missiles = [];
var CAM_RADIUS;
mouse = new THREE.Vector2();
raycaster = new THREE.Raycaster();
var CONST_SPEED_FACTOR = 0.25;
var CONST_MISSILE_G;
var CONST_CENTER = new THREE.Vector3(0, 0, -200);
var CONST_SPEED = 5, CONST_TICK = 0.03, CONST_G = 0.45, CONST_H = 25, CONST_SHOOTER_SPEED = 60, CONST_SHOOTER_ACCELERATION = 0.5;

var CONST_MISSILE_MATERIAL = new THREE.MeshBasicMaterial({map: THREE.ImageUtils.loadTexture('textures/missile.jpg')});
var CONST_SHOOT_SOURCES = [
		new THREE.Vector3(-12.99, 3.018, -221.0752),
		new THREE.Vector3(-7.618, 8.075, -173.25),
		new THREE.Vector3(-23.114, 3, -202.165),
		new THREE.Vector3(-22.647, 6.682, -161.270)
	];
var time_left;
var current_time = 0;
var options, spawnerOptions, particleSystem;
var clock = new THREE.Clock(true);
var tick = 0;

function getHeightData(img, scale) {
	if (scale == undefined) scale = 1;
	
	var canvas = document.createElement('canvas');
	canvas.width = img.width;
	canvas.height = img.height;
	var context = canvas.getContext('2d');
	var size = img.width * img.height;
	var data = new Float32Array(size);
	
	context.drawImage(img, 0, 0);
	for (var i = 0; i < size; i++) {
		data[i] = 0;
	}
	
	var imgd = context.getImageData(0, 0, img.width, img.height);
	
	var pix = imgd.data;
	
	var j = 0;
	for (var i = 0; i < pix.length; i += 4) {
		var all = pix[i] + pix[i+1] + pix[i + 2];
		data[j++] = all / (12 * scale);
	}
	
	return (data);
}

function updateScene() {
	current_time += CONST_TICK;
	var delta = clock.getDelta() * spawnerOptions.timeScale;
	tick += delta;
	if (tick < 0) tick = 0;
	
	for (var missileIndex = 0; missileIndex < _missiles.length; missileIndex++) {
		var exp = false;
		currMissile = _missiles[missileIndex]
		if (currMissile.attack_movement) {
			elapsed_time = current_time - currMissile.attack_movement.start_time;
			currMissile.attack_model.position.x = currMissile.attack_movement.start_position.x + currMissile.attack_movement.vx * elapsed_time;
			currMissile.attack_model.position.z = currMissile.attack_movement.start_position.z + currMissile.attack_movement.vz * elapsed_time;
			currMissile.attack_model.position.y = currMissile.attack_movement.start_position.y + currMissile.attack_movement.vy * elapsed_time - (CONST_G / 2) * Math.pow(elapsed_time, 2);
			
			var X = 238 + Math.floor((currMissile.attack_model.position.x - 100) * 238 / 200);
			var Y = 238 + Math.floor((currMissile.attack_model.position.z + 100) * 238 / 200);
			
			if (currMissile.attack_model.position.y < _planes[0].geometry.vertices[Y * 238 + X].z - 1/2) {
				exp = true;
				currMissile.attack_movement = undefined;
				currMissile.defense_movement = undefined;
				scene.remove(currMissile.attack_model);
				currMissile.attack_model.geometry.dispose();
				currMissile.attack_model.material.dispose();
			}
		}
		
		var shooter = currMissile.defense_movement;
		var shooter_model = currMissile.defense_model;
		if (shooter && current_time >= shooter.start_time) {
			scene.add(shooter_model);
			var t = current_time - shooter.start_time;
			var x = shooter.source.x + shooter.speed.x * t + shooter.acceleration.x / 2 * t * t;
			var y = shooter.source.y + shooter.speed.y * t + shooter.acceleration.y / 2 * t * t;
			var z = shooter.source.z + shooter.speed.z * t + shooter.acceleration.z / 2 * t * t;
			shooter_model.position.set(x, y, z);
			shooter_model.rotation.z = -shooter.ver_angle;
			shooter_model.rotation.y = Math.PI - shooter.hor_angle;
			
			if (current_time > shooter.end_time) {
				exp = true;
				currMissile.attack_movement = undefined;
				currMissile.defense_movement = undefined;
				scene.remove(currMissile.attack_model);
				currMissile.attack_model.geometry.dispose();
				currMissile.attack_model.material.dispose();
				scene.remove(shooter_model);
				shooter_model.geometry.dispose();
				shooter_model.material.dispose();
			}
		}
				
		if (currMissile.attack_movement && delta > 0) {
			my_ver_angle =
				Math.PI / 2 - Math.atan(Math.tan(currMissile.attack_movement.ver_angle) - CONST_G * elapsed_time / (CONST_SPEED * Math.cos(currMissile.attack_movement.ver_angle)));
			
			currMissile.attack_model.rotation.z = - Math.PI / 2 + my_ver_angle;
			currMissile.attack_model.rotation.y = Math.PI - currMissile.attack_movement.hor_angle;
			
			options.color = 0xff0000;
			options.position.x = currMissile.attack_model.position.x - 0.85 * Math.cos(currMissile.attack_movement.hor_angle) * Math.sin(my_ver_angle);
			options.position.y = currMissile.attack_model.position.y - 0.85 * Math.cos(my_ver_angle);
			options.position.z = currMissile.attack_model.position.z - 0.85 * Math.sin(currMissile.attack_movement.hor_angle) * Math.sin(my_ver_angle);
			options.velocity.set(-currMissile.attack_movement.vx, -(currMissile.attack_movement.vy - CONST_G * elapsed_time) / Math.abs(currMissile.attack_movement.vy), -currMissile.attack_movement.vz);
		
			for (var i = 0; i < spawnerOptions.spawnRate * delta; i++) {			
				currMissile.particle_sys.spawnParticle(options);
			}
			
			if (current_time >= shooter.start_time) {
				options.color = 0xffffff;
				options.position.x = shooter_model.position.x - 0.85 * Math.cos(shooter.hor_angle);
				options.position.y = shooter_model.position.y - 0.25 * Math.cos(shooter.ver_angle);
				options.position.z = shooter_model.position.z - 0.85 * Math.sin(shooter.hor_angle);
				options.velocity.set(0, 0, 0);
				for (var i = 0; i < spawnerOptions.spawnRate * delta; i++) {			
					currMissile.particle_sys.spawnParticle(options);
				}
			}
		}
		
		if (exp) {
			options.color = 0xFF6600;
			options.position.x = currMissile.attack_model.position.x;
			options.position.y = currMissile.attack_model.position.y
			options.position.z = currMissile.attack_model.position.z;
			options.velocityRandomness = 3;
			options.turbulence = 5;
			for (var i = 0; i < spawnerOptions.spawnRate * delta * 100; i++) {			
				currMissile.particle_sys.spawnParticle(options);
			}
			
			options.velocityRandomness = .5;
			options.turbulence = .2;
		}
		
		currMissile.particle_sys.update(tick);
	}
}

function loadPolys() {
	$.get('data.json', function(text, status) {
		data = JSON.parse(text);
		//console.log(JSON.stringify(data[0].vertices));
		for (var nIndex = 0; nIndex < data.length; nIndex++) {
			var poly = data[nIndex];
			var pos = poly.position;
			
			var vertices = poly.trace;
			mytracer = new THREE.Shape();
			mytracer.moveTo(0, 0);
			for (var j = 1; j < vertices.length - 1; j++) {
				mytracer.lineTo(vertices[j].x, vertices[j].y);
			}
			
			//mytracer.lineTo(0, 0);
			var myGeom = new THREE.ShapeGeometry( mytracer );
			
			
			
			/*var vertices = poly.vertices;
			var myGeom = new THREE.Geometry();
			myGeom.vertices = vertices;
			var l = myGeom.vertices.length;
			for (var i = 0; i < vertices.length; i++) {
				myGeom.faces.push(new THREE.Face3((i + 2) % l, (i + 1) % l, (i) % l));
				//myGeom.faces.push(new THREE.Face3((i + 2) % l, (i + 4) % l, (i) % l));
				//myGeom.faces.push(new THREE.Face3((i + 3) % l, (i + 6) % l, (i) % l));
			}
			
			myGeom.verticesNeedUpdate = true;*/
			var col = Math.random() * Math.pow(255, 3)
			var myMesh = 
				new THREE.Mesh( myGeom, 
								new THREE.MeshBasicMaterial({ 
									  /*color: Math.pow(16, 4) * (Math.random() * 255 + 0) +
											 Math.pow(16, 2) * (Math.random() * 255 + 0) + 
											 Math.pow(16, 0) * (Math.random() * 255 + 0),*/
									  //color: Math.random() * Math.pow(255, 3), 
									  color: col,
									  //color: 0x00CC99, 
									  //color: CONST_COLORS[Math.floor(Math.random() * CONST_COLORS.length)],
									  opacity: 0.7,
									  transparent: true, 
									  side: THREE.DoubleSide 
								} ) ) ;
			myMesh.rotation.x = Math.PI / 2;
			myMesh.position.set(pos.x, pos.y + 3, pos.z);
			myMesh.updateMatrixWorld();
			
			for (var i = 0; i < myMesh.geometry.vertices.length; i++) {
				worldVertex = myMesh.geometry.vertices[i].clone();
				worldVertex.applyMatrix4(myMesh.matrixWorld);
				var X = 238 + Math.floor((worldVertex.x - 100) * 238 / 200);
				var Y = 238 + Math.floor((worldVertex.z + 100) * 238 / 200);
				//myMesh.geometry.vertices[i].z += _planes[0].geometry.vertices[Y * 238 + X].z - myMesh.position.z;
				//_planes[0].geometry.vertices[Y * 238 + X].z = myMesh.position.z;
				//console.log(_planes[0].geometry.vertices[Y * 238 + X].z - myMesh.position.y);
				myMesh.geometry.vertices[i].z -= _planes[0].geometry.vertices[Y * 238 + X].z - myMesh.position.y + 6;
				//myMesh.geometry.vertices[i].z = myMesh.position.z;
			}
			
			myMesh.geometry.verticesNeedUpdate = true;
			//_planes[0].geometry.verticesNeedUpdate = true;
			
			//myMesh.position.set(0, 20, -200);
			
			scene.add(myMesh);
		}
	});
}

function loadTerrain(scene) {
	var img = new Image();
	img.onload = function() {
		var data = getHeightData(img, 5);
		var res = Math.floor(Math.sqrt(data.length));
		var geometry = new THREE.PlaneGeometry(200, 200, res - 1, res - 1);
		var texture = THREE.ImageUtils.loadTexture('images/map.bmp');
		var material = new THREE.MeshPhongMaterial({ map: texture, side: THREE.DoubleSide });
		var plane = new THREE.Mesh(geometry, material);
		
		for (var i = 0; i < plane.geometry.vertices.length; i++) {
			plane.geometry.vertices[i].z += data[i];
		}
		
		plane.position.set(0, 0, -200);
		plane.rotation.x = -Math.PI / 2;
		
		scene.add(plane);
		_planes.push(plane);
		
		loadPolys();
	};
	
	//img.src = 'images/heightmap.small.bmp';
	img.src = 'images/heightmap.tiny.bmp';
}

function loadBox(scene) {
	var planes = [];
	
	var geometry = new THREE.PlaneGeometry(200, 200, 2);
	
	for (var i = 0; i < 4; i++) {
		texture = THREE.ImageUtils.loadTexture('images/' + (i + 1) + '.bmp');
		planes.push(new THREE.Mesh(geometry, new THREE.MeshBasicMaterial({map: texture, side: THREE.DoubleSide})));
		scene.add(planes[planes.length - 1]);
	}
	
	planes[0].position.set(0, 100, -300);
	
	planes[1].position.set(0, 100, -100);
	planes[1].rotation.y = Math.PI;
	
	planes[2].position.set(100, 100, -200);
	planes[2].rotation.y = -Math.PI / 2;
	
	planes[3].position.set(-100, 100, -200);
	planes[3].rotation.y = Math.PI / 2;
}

function loadModels(scene) {
	var loader = new THREE.JSONLoader();
	loader.load('models/missile.js', function(geometry) {
		CONST_MISSILE_G = geometry;
		CONST_MISSILE_G.scale(0.05, 0.05, 0.05);
		CONST_MISSILE_G.computeBoundingBox();
		CONST_MISSILE_G.center();
	});
}

function loadParticleSystem() {
	options = {
        position: new THREE.Vector3(),
        positionRandomness: .3,
        velocity: new THREE.Vector3(),
        velocityRandomness: .5,
        color: 0xff0000,
        colorRandomness: .2,
        turbulence: .2,
		lifetime: 2,
        size: 5,
        sizeRandomness: 1
      };

      spawnerOptions = {
        spawnRate: 8000,
        horizontalSpeed: 1.5,
        verticalSpeed: 1.33,
        timeScale: 1
      };
}

function initScene() {
	scene = new THREE.Scene();
	var WIDTH = $(window).width() - 4, HEIGHT = $(window).height() - 4;
	renderer = new THREE.WebGLRenderer({antialias: true});
	renderer.setSize(WIDTH, HEIGHT);
	$('body').append(renderer.domElement);
	
	camera = new THREE.PerspectiveCamera(45, WIDTH / HEIGHT, 0.1, 20000);
	
	camera.position.set(0, 30, -120);
	camera.lookAt(new THREE.Vector3(0, 0, -200));
	
	scene.add(camera);
	renderer.setClearColor(0xFFFFFF, 1);
	
	var light = new THREE.PointLight(0xffffff);
	
	light.position.set(0, 400, -250);
	
	scene.add(light);

	CAM_RADIUS = Math.sqrt(Math.pow(CONST_CENTER.x - camera.position.x, 2) +
						   Math.pow(CONST_CENTER.y - camera.position.y, 2) +
						   Math.pow(CONST_CENTER.z - camera.position.z, 2));
}

function init() {
	initScene();
	
	loadTerrain(scene);
	
	loadBox(scene);
	
	loadModels(scene);
	
	loadParticleSystem();
}

function render() {	
	requestAnimationFrame(render);
	//updateScene();
	renderer.render(scene, camera);
}

$(document).ready(function() {
	
	init();
	
	/*setInterval(function() {
		updateScene();
	}, 20);*/
	
	render();
});

$(window).on('mousewheel', function(e) {
	ZOOM_AMOUNT = 1.025;
	if (e.originalEvent.deltaY < 0) {
		camera.fov /= ZOOM_AMOUNT;
	} else {
		camera.fov = Math.min(100, camera.fov * ZOOM_AMOUNT);
	}
	
	camera.updateProjectionMatrix();
});

function moveH(nHowMuch) {
	currRadius = Math.sqrt(Math.pow(CONST_CENTER.x - camera.position.x, 2) +
						   Math.pow(CONST_CENTER.z - camera.position.z, 2));
	currAng = Math.atan2(camera.position.z - CONST_CENTER.z,
						 camera.position.x - CONST_CENTER.x);
	
	nDiff = nHowMuch * (0.25) * (Math.PI / 180);
	currAng += nDiff;
	
	camera.position.x = Math.cos(currAng) * currRadius + CONST_CENTER.x;
	camera.position.z = Math.sin(currAng) * currRadius + CONST_CENTER.z;
	camera.lookAt(CONST_CENTER);
}

function moveV(nHowMuch) {
	currAngPlain = Math.atan2(camera.position.z - CONST_CENTER.z,
							  camera.position.x - CONST_CENTER.x);
	
	c = Math.sqrt(Math.pow(CAM_RADIUS, 2) - 
				  Math.pow(camera.position.y, 2));
						  
	currAngY = Math.atan2(camera.position.y - CONST_CENTER.y,
						  c);
	
	nDiff = nHowMuch * (0.15) * (Math.PI / 180);
	
	currAngY += nDiff;
	currAngY = (Math.PI / 180) * (((180 / Math.PI) * currAngY) % 360);
	
	var newY = Math.sin(currAngY) * CAM_RADIUS + CONST_CENTER.y;
	if (newY >= 20) {	
		camera.position.y = newY;
		newRadius = Math.abs(Math.cos(currAngY) * CAM_RADIUS);
							  
		camera.position.x = Math.cos(currAngPlain) * newRadius + CONST_CENTER.x;
		camera.position.z = Math.sin(currAngPlain) * newRadius + CONST_CENTER.z;
		camera.lookAt(CONST_CENTER);
		setCameraUp();

		c = Math.sqrt(Math.pow(CAM_RADIUS, 2) - 
					  Math.pow(camera.position.y, 2));
							  
		newAngY = Math.atan2(camera.position.y - CONST_CENTER.y,
							  c);
	}
}

function setCameraUp() {
		dAngle = Math.atan2(camera.position.z - CONST_CENTER.z,
							camera.position.x - CONST_CENTER.x) - Math.PI / 2;
		var temp = new THREE.Vector3(CONST_CENTER.x, CONST_CENTER.y, CONST_CENTER.z);
		temp.x += Math.cos(dAngle);
		temp.z += Math.sin(dAngle);
		
		temp.sub(CONST_CENTER);
		
		var toSun = camera.position.clone();
		toSun.sub(CONST_CENTER);
		
		temp.crossVectors(toSun, temp);
		
		temp.normalize();
		
		camera.up = temp;
}

$(window).on('mousedown', function(e) {
	startX = e.clientX;
	startY = e.clientY;
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

$(window).on('mousedown', function(e) {
	mouse.x = (e.pageX / renderer.domElement.width) * 2 - 1;
	mouse.y = - (e.pageY / renderer.domElement.height) * 2 + 1;
	
	raycaster.setFromCamera(mouse, camera);
	
	var intersects = raycaster.intersectObjects(_planes);
	
	/*if (intersects.length > 0) {
		var currMissile = {};
		currMissile.attack_model = new THREE.Mesh(CONST_MISSILE_G.clone(), CONST_MISSILE_MATERIAL);
		scene.add(currMissile.attack_model);
		//console.log(intersects[0].point.x, intersects[0].point.y, intersects[0].point.z);
		currMissile.attack_model.position.set(intersects[0].point.x, intersects[0].point.y, intersects[0].point.z);
		var ver_angle = Math.random() * (Math.PI / 2.25 - Math.PI / 2.8) + Math.PI / 2.8;
		var hor_angle = Math.random() * Math.PI * 2;
		currMissile.attack_movement = 
			{
				start_time: current_time,
				start_position: currMissile.attack_model.position.clone(),
				vy: CONST_SPEED * Math.sin(ver_angle),
				vx: CONST_SPEED * Math.cos(ver_angle) * Math.cos(hor_angle),
				vz: CONST_SPEED * Math.cos(ver_angle) * Math.sin(hor_angle),
				ver_angle: ver_angle,
				hor_angle: hor_angle
			};
			
		prepareShootdown(currMissile);
		
		currMissile.particle_sys = new THREE.GPUParticleSystem({
			maxParticles: 8300
		});
		
		scene.add(currMissile.particle_sys);
		
		_missiles.push(currMissile);
	}*/
});

function prepareShootdown(objMissile) {
	var movement = objMissile.attack_movement;
	time_left = (Math.sqrt(Math.pow(movement.vy, 2) + 2 * CONST_G * (objMissile.attack_model.position.y - CONST_H)) + movement.vy) / CONST_G;
	target = new THREE.Vector3(objMissile.attack_movement.start_position.x + time_left * objMissile.attack_movement.vx,
							   objMissile.attack_movement.start_position.y + time_left * objMissile.attack_movement.vy - (CONST_G / 2) * Math.pow(time_left, 2),
							   objMissile.attack_movement.start_position.z + time_left * objMissile.attack_movement.vz);
	source = CONST_SHOOT_SOURCES[Math.floor(Math.random() * CONST_SHOOT_SOURCES.length)];
	dist = Math.sqrt(Math.pow(source.x - target.x, 2) +
					 Math.pow(source.y - target.y, 2) + 
					 Math.pow(source.z - target.z, 2));
	flat_dist = Math.sqrt(Math.pow(source.x - target.x, 2) +
						  Math.pow(source.z - target.z, 2));
	air_time = (Math.sqrt(Math.pow(CONST_SHOOTER_SPEED, 2) + 2 * CONST_SHOOTER_ACCELERATION * dist) - CONST_SHOOTER_SPEED) / CONST_SHOOTER_ACCELERATION;
	start_time = movement.start_time + time_left - air_time;
	propX = (target.x - source.x) / dist;
	propY = (target.y - source.y) / dist;
	propZ = (target.z - source.z) / dist;
	objMissile.defense_movement = {
		start_time: start_time,
		end_time: start_time + air_time,
		speed: new THREE.Vector3(propX * CONST_SHOOTER_SPEED, propY * CONST_SHOOTER_SPEED, propZ * CONST_SHOOTER_SPEED),
		acceleration: new THREE.Vector3(propX * CONST_SHOOTER_ACCELERATION, propY * CONST_SHOOTER_ACCELERATION, propZ * CONST_SHOOTER_ACCELERATION),
		ver_angle: Math.atan2(target.y - source.y, dist),
		hor_angle: Math.atan2(target.z - source.z, target.x - source.x),
		source: source
	};
	
	objMissile.defense_model = new THREE.Mesh(CONST_MISSILE_G.clone(), new THREE.MeshPhongMaterial({color: 0x878787}));
	objMissile.defense_model.position.set(source.x, source.y, source.z);
}