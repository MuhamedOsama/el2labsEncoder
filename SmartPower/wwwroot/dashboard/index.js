//
// Requires
// - chartist.min.js
// - chartist-plugin-tooltip.js
//
// IE9 support requires
// - matchMedia.js
// - matchMedia.addListener.js
//

var $lineCharts = $('.js-ct-line-chart')
  , lineOptions =
    { axisX:
      { labelOffset: { x: -12 }
      }
    , axisY:
      { onlyInteger: true
      , scaleMinSpace: 50
      , labelOffset: { y: 5 }
      }
    , lineSmooth: false
    , low: 0
    , fullWidth: true
    , plugins: [ Chartist.plugins.tooltip() ]
    }
  , lineResponsiveOptions = [
  ['screen and (min-width: 640px) and (max-width: 1023px)', {
    axisY: {
      scaleMinSpace: 35
    }
  }],
  ['screen and (max-width: 639px)', {
    showPoint: false,
    axisX: {
      labelInterpolationFnc: function(value) {
        // Will return M, T, W etc. on small screens
        return value[0];
      }
    },
    axisY: {
      scaleMinSpace: 20
    }
  }]
];

$lineCharts.each(function (chart) {
  var $this = $(this)
    , data =
      { labels: $this.data('labels').split(', ')
      , series: $this.data('series')
      }
  
  new Chartist.Line($this[0], data, lineOptions, lineResponsiveOptions)
})