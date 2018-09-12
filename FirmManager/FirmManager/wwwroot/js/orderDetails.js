var geocoder;
var map;
var markers = [];

function initialize() {
    geocoder = new google.maps.Geocoder();
    var latlng = new google.maps.LatLng(39.989428, -98.373697);
    var mapOptions = {
        zoom: 8,
        center: latlng
    };
    map = new google.maps.Map(document.getElementById('map'), mapOptions);

    var address = document.getElementById('city').innerHTML + " " + document.getElementById('state').innerHTML;
    geocoder.geocode({ 'address': address }, function (results, status) {
        if (status === 'OK') {
            map.setCenter(results[0].geometry.location);
            var marker = new google.maps.Marker({
                map: map,
                position: results[0].geometry.location
            });
        } else {
            alert('Geocode was not successful for the following reason: ' + status);
        }
    });

    var directionsService = new google.maps.DirectionsService();
    var directionsDisplay = new google.maps.DirectionsRenderer();
    directionsDisplay.setMap(map);

    var depositArr = ["1652-1632 V St NW Washington, DC 20009", "2601-2605 NV-147 North Las Vegas, NV 89030", "S 31st Ave Minneapolis, MN 55406", "4517-4501 Gaston Ave Dallas, TX 75246"];

    for (var i = 0; i < depositArr.length; i++) {
        geocoder.geocode({ 'address': depositArr[i] }, function (results, status) {
            if (status === 'OK') {
                var marker = new google.maps.Marker({
                    map: map,
                    position: results[0].geometry.location,
                    icon: '../../images/deposit.png'
                });
                markers.push(marker);

            } else {
                alert('Geocode was not successful for the following reason: ' + status);
            }
        });
    }
}

function calcRoute() {
    var directionsService = new google.maps.DirectionsService();
    var directionsDisplay = new google.maps.DirectionsRenderer();
    directionsDisplay.setMap(map);
    var end = document.getElementById('address').innerHTML + " " + document.getElementById('city').innerHTML + " " + document.getElementById('state').innerHTML;
    geocoder = new google.maps.Geocoder();

    var endCoordinates;
    geocoder.geocode({ 'address': end }, function (results, status) {
        endCoordinates = results[0].geometry.location;

        var distances = [];
        var closest = -1;
        for (i = 0; i < markers.length; i++) {
            var d = google.maps.geometry.spherical.computeDistanceBetween(markers[i].position, endCoordinates);
            distances[i] = d;
            if (closest === -1 || d < distances[closest]) {
                closest = i;
            }
        }

        var request = {
            origin: markers[closest].position,
            destination: end,
            travelMode: 'DRIVING'
        };
        directionsService.route(request, function (response, status) {
            if (status === 'OK') {
                directionsDisplay.setDirections(response);
                var point = response.routes[0].legs[0];
                $("#routeDuration").html('<div class="alert alert-info" style="float: right" role="alert">Estimated time: ' + point.duration.text + ' (' + point.distance.text + ')</div>');
            }
        });
    });
}