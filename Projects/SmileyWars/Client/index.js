states = []
codes = {}
errors = {}

window.onkeypress = function(e) {
	if (e.keyCode == 10) {
		$('#apply').click()
	}
}

mycolor = null
selectedSmiley = null;
lastUpdate = null;

setInterval(function() {
	if (states.length < 20) {
		$.ajax(			
				'../../server/update', {
					type: 'POST',
					dataType: 'json',
					data: JSON.stringify(lastUpdate != null ? lastUpdate.getTime() : 0),
					success: function(data) {
						states = states.concat(data.what)
						lastUpdate = new Date(data.when);
					},
					error: function() {
						alert('Error communicating with server');
					}
				});
	}
}, 500);
setInterval(function() {
	if (states.length > 0) {
		currState = states.shift()
		for (i = 0; i < currState.data_list.length; i++) {
			if ($('#' + i).length == 0) {
				$("body").append("<div class='smiley' id='" + i + "'></div>");
				$('#' + i).addClass(currState.data_list[i].owner);
				if (mycolor == currState.data_list[i].owner) {
					$('#' + i).click(function(e) {
						e.stopPropagation()
						unselectSmiley()
						
						selectedSmiley = this;
						$('#code textarea').prop('disabled', false)
						
						$('#' + selectedSmiley.id).addClass('selected')
						
						$('#code textarea').css('background', 'rgba(58, 124, 202, 0.5)');
						if (codes.hasOwnProperty(selectedSmiley.id)) {
							$('#code textarea').prop('value', codes[selectedSmiley.id])
						}
						
						if (errors.hasOwnProperty(selectedSmiley.id)) {
							$('#code textarea').prop('value', $('#code textarea').prop('value') + '\n' + errors[selectedSmiley.id])
							$('#code textarea').css('background', 'rgba(100, 0, 0, 0.6)')
						}
						$('#code textarea').focus()
					});
				}
			}
			
			$('#' + i).css('left', currState.data_list[i].pos[0] * tot_width + 'px');
			$('#' + i).css('top', currState.data_list[i].pos[1] * tot_height + 'px');
		}
		
		for (var err_index in currState.errors) {
			if (mycolor == currState.data_list[err_index].owner) {
				setError(err_index, currState.errors[err_index])
			}
		}
		
		for (i = 0; i < currState.bullets.length; i++) {
			if ($('#b' + i).length == 0) {
				$('body').append("<div class='bullet' id=b" + i + "></div>")
			}
			
			$('#b' + i).removeClass();
			$('#b' + i).addClass('bullet');
			$('#b' + i).addClass('b_' + currState.bullets[i].owner);
			$('#b' + i).css('left', currState.bullets[i].pos[0] * tot_width + 'px');
			$('#b' + i).css('top', currState.bullets[i].pos[1] * tot_height + 'px');
		}
		
		$('.bullet').each(function(index, element) {
			if (parseInt(element.id.substring(1)) >= currState.bullets.length) {
				$(element).remove();
			}
		});
	}
}, 20);

function unselectSmiley() {
	if (selectedSmiley != null) {
			$('#' + selectedSmiley.id).css('border', 'none');
			$('#' + selectedSmiley.id).removeClass('selected');
			$('#code textarea').prop('disabled', true)
		}
		
	$('#code textarea').prop('value', null);
	$('#code textarea').css('background', 'none');
	selectedSmiley = null;
}

function setError(index, message) {
	errors[index] = message
	$('#' + index).addClass('error')
	if (selectedSmiley.id == index) {
		$('#code textarea').css('background', 'rgba(100, 0, 0, 0.6)')
		$('#code textarea').prop('value', $('#code textarea').prop('value') + '\n' + message)
	}
}

$(document).ready(function() {
	tot_width = $('body').width() - 100
	tot_height = $('body').height() - $('#code').height() - 100
	$("#add").click(function() {
		$.get('../../server/add', function(data) {
			mycolor = data 
		});
	})
	
	$('#apply').click(function(event) {
		event.stopPropagation()
		if (selectedSmiley != null) {	
			codes[selectedSmiley.id] = $('#code textarea').prop('value')
			$.post('../../server/logic',
				JSON.stringify({
					"index": selectedSmiley.id,
					"code": $('#code textarea').prop('value')
				}),
				function(data, status) {
					if (data.status) {
						delete errors[selectedSmiley.id]
						$('#' + selectedSmiley.id).removeClass('error')
						$('#code textarea').css('background', 'rgba(58, 124, 202, 0.5)');
						//unselectSmiley()
					} else {
						setError(selectedSmiley.id, data.message)
					}
				},
				"json");
		}
	});
	
	$('body').click(function() {
		unselectSmiley()
	})
	
	$('#code textarea').click(function(event) {
		event.stopPropagation()
	})
})
