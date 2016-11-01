selectedSource = null;
$(document).ready(function() {
	$('#loader').hide();
	$.ajax('../../server/getLocs',
			{
				type: 'GET',
				dataType: 'json',
				success: function(locsData) {
				lines = '';
				$.ajax('../../server/getRoads',
						{
							type: 'GET',
							dataType: 'json',
							success: function(dataRoads) {
								for (nIndex = 0; nIndex < dataRoads.length; nIndex++) {
									x1 = locsData[dataRoads[nIndex][0]].x;
									y1 = locsData[dataRoads[nIndex][0]].y;
									x2 = locsData[dataRoads[nIndex][1]].x;
									y2 = locsData[dataRoads[nIndex][1]].y;
									lines += '<line x1="' + x1 + '" y1 = "' + y1 + '" x2 = "' + x2 + '" y2 = "' + y2 + '" style="stroke:rgba(130,130,130,0.8);stroke-width:2" />';
								}
								
								$('body').append('<svg>' + lines + '</svg>');
								$('svg').height($('body').height());
								$('svg').width($('body').width());
								
								for (nIndex = 0; nIndex < locsData.length; nIndex++) {
									$('body').prepend('<div class="ook" id="' + locsData[nIndex].index + '"></div>');
									$('#' + locsData[nIndex].index).css('left', locsData[nIndex].x);
									$('#' + locsData[nIndex].index).css('top', locsData[nIndex].y);
								}
								
								$('.ook').click(function() {
									if (selectedSource == null) {
										selectedSource = this;
										$(this).addClass('shtut');
										$('#ookSVG').remove();
									} else {
										$('#loader').show(200);
										$.ajax('../../server/getPath',
												{
													type: 'POST',
													dataType: 'json',
													data: JSON.stringify({
														'from': selectedSource.id,
														'to' : this.id
													}),
													success: function(pathData) {
														
														$('#loader').hide(200);
														$(selectedSource).removeClass('shtut');
														selectedSource = null;
														
														lines = '<polyline points="'
														for (nIndex = 0; nIndex < pathData.length; nIndex++) {
															x = pathData[nIndex].x;
															y = pathData[nIndex].y;
															lines += ' ' + x + ',' + y;
														}
														lines += '" style="fill:none;stroke:blue;stroke-width:5;" />'
														$('body').prepend('<svg id="ookSVG">' + lines + '</svg>');
														$('#ookSVG').height($('body').height());
														$('#ookSVG').width($('body').width());
													}
												});
									}
								});
								
								$('.ook').dblclick(function() {
									alert(this.id);
								});
							}							
						});
					}
				});
});