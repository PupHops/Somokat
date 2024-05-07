let center = [48.8866527839977, 2.34310679732974];
let lat = 0;
let long = 0;
const options = {
    enableHighAccuracy: true,
    timeout: 5000,
    maximumAge: 0,
};


function success(pos) {
    const crd = pos.coords;

    console.log("Your current position is:");
    console.log(`Latitude : ${crd.latitude}`);
    console.log(`Longitude: ${crd.longitude}`);
    center = [crd.latitude, crd.longitude];
    lat = crd.latitude;
    long = crd.longitude;

}

function error(err) {
    console.warn(`ERROR(${err.code}): ${err.message}`);
}

navigator.geolocation.getCurrentPosition(success, error, options);


// Создаем подложку для блокировки пространства за всплывающим окном
var overlayElement = $('<div id="overlay"></div>')
    .css({
        position: 'fixed',
        top: '0',
        left: '0',
        width: '100%',
        height: '100%',
        background: 'rgba(0, 0, 0, 0.5)', // Полупрозрачный цвет
        zIndex: 9998, // Устанавливаем z-index, чтобы подложка была ниже всплывающего окна
        display: 'none'
    })
    .appendTo('body');



function showPopup() {
    $('#popup').fadeIn();
    $('#overlay').fadeIn();

}

function hidePopup() {
    $('#popup').fadeOut();
    $('#overlay').fadeOut();
    localStorage.setItem('isOpen', 0);

}



$(document).on('click', '#overlay', function () {
    hidePopup();
});

function init() {
    var geolocation = ymaps.geolocation;
    let map = new ymaps.Map('map-test', {
        center: center,
        zoom: 17
    });


    jQuery.getJSON('https://localhost:7209/Somokat', function (json) {
        /** Сохраним ссылку на геообъекты на случай, если понадобится какая-либо постобработка.
         * @see https://api.yandex.ru/maps/doc/jsapi/2.1/ref/reference/GeoQueryResult.xml
        */
        var geoObjects = ymaps.geoQuery(json)
            .addToMap(map)
            .applyBoundsToMap(map, {

                checkZoomRange: true
            });
        geoObjects.setOptions({
            openBalloonOnClick: false,
            hasBalloon: false,
            iconLayout: 'default#image',
            iconImageHref: 'https://localhost:7099/images/scooter.svg',
            iconImageSize: [60, 67],
            iconImageOffset: [-25, -52]
        });
        geoObjects.each(function (geoObject) {
            geoObject.events.add('click', function (e) {


                var balloonContent = geoObject.properties.get('balloonContent');
                var contentParts = balloonContent.split(';');
                var chargeLevel = contentParts[0];
                var deviceState = contentParts[1];
                var scooterName = "AbobaScooter"; 
                var scooterNumber = geoObject.properties.get('hintContent'); 
                var imageUrl = 'https://localhost:7099//images/Samokat.png'; 
                localStorage.setItem('chargeLevel', contentParts[0]);
                localStorage.setItem('scooterName', scooterName);
                localStorage.setItem('imageUrl', imageUrl);

                var phoneNumber = localStorage.getItem('phoneNumber');


                $('#scooter-image').attr('src', localStorage.getItem('imageUrl'));
                $('#scooter-name').text(localStorage.getItem('scooterName'));
                $('#charge-progress').attr('value', localStorage.getItem('chargeLevel'));
                localStorage.setItem('isOpen', 1);

                localStorage.setItem('targetScooter', contentParts[2]);
                showPopup();

            });
        });
    }).fail(function (jqxhr, textStatus, error) {
        // Обработка ошибок загрузки JSON
        var err = textStatus + ", " + error;
        console.error("Request Failed: " + err);
    });

    var a = document.getElementById('geo');

    a.onclick = function () {
        geolocation.get({
            provider: 'auto',
            mapStateAutoApply: false
        }).then(function (result) {

            result.geoObjects.options.set('preset', 'islands#blueCircleIcon');
            map.geoObjects.remove(result.geoObjects);
            map.geoObjects.add(result.geoObjects);
            var coordinates = result.geoObjects.get(0).geometry.getCoordinates();
            console.log(coordinates);
            map.setCenter(coordinates, 17); 

        });
    }

    // map.controls.remove('geolocationControl');
    map.controls.remove('searchControl'); // удаляем поиск
    map.controls.remove('trafficControl'); // удаляем контроль трафика
    map.controls.remove('typeSelector'); // удаляем тип
    map.controls.remove('fullscreenControl'); // удаляем кнопку перехода в полноэкранный режим
    map.controls.remove('zoomControl'); // удаляем контрол зуммирования
    map.controls.remove('rulerControl'); // удаляем контрол правил
    map.controls.remove('geolocationControl'); // удаляем контрол правил



}

window.onload = function () {

    var balance = localStorage.getItem('balance');
    var phoneNumber = localStorage.getItem('phoneNumber');

    if (localStorage.getItem('isOpen') == 1) {
        $('#scooter-image').attr('src', localStorage.getItem('imageUrl'));
        $('#scooter-name').text(localStorage.getItem('scooterName'));
        $('#charge-progress').attr('value', localStorage.getItem('chargeLevel'));


        showPopup();
    }
    if (localStorage.getItem('isOpen') == 2) {
        $('#scooter-image').attr('src', localStorage.getItem('imageUrl'));
        $('#scooter-name').text(localStorage.getItem('scooterName'));
        $('#charge-progress').attr('value', localStorage.getItem('chargeLevel'));
        showPopup();

        document.getElementById("rentButton").style.display = "none";
        document.getElementById("endTripButton").style.display = "flex";
        document.getElementById("overlay2").style.display = "block";

    }
  

    if (balance !== null) {
        // Выводим баланс на странице Home
        document.getElementById('balanceDisplay').innerText = balance + "₽";
        document.getElementById('balanceDisplay1').innerText = balance + "₽";
        document.getElementById('MenuPhoneNumber').innerText = "+ "+ phoneNumber;
    }
};



$(document).on('click', '#rentButton', function () {
    var balance = localStorage.getItem('balance');
    if (balance > 0) {
        rentScooter();
    } else {
        alert("Недостаточно средств на счету!");

    }
});
function startTimer() {
    let seconds = 0;

    function tick() {
        
        seconds++;
        var userId = localStorage.getItem('userId');

        var data = {
            userId
        };

        $.ajax({
            url: 'https://localhost:7209/Authorization/CheckMoney',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (response, textStatus, xhr) {

                if (xhr.status === 200) {

                    localStorage.setItem('balance', JSON.stringify(response.bonus));
                    var balance = localStorage.getItem('balance');
                    document.getElementById('balanceDisplay').innerText = balance + "₽";
                    document.getElementById('balanceDisplay1').innerText = balance + "₽";

                }
            },

        });
    }

    setInterval(tick, 1000);
}

startTimer();
$(document).on('click', '#endTripButton', function () {
    endTrip();
    localStorage.setItem('isOpen', 1);

    console.log('Кончаем')
});




function rentScooter() {
    $.ajax({
        url: 'https://localhost:7209/Somokat/Rent',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            userId: localStorage.getItem('userId'),
            targetScooter: localStorage.getItem('targetScooter')
        }),
        success: function (response) {
            document.getElementById("rentButton").style.display = "none";
            document.getElementById("endTripButton").style.display = "inline";
            document.getElementById("overlay2").style.display = "block";
            localStorage.setItem('isOpen', 2);
            console.log('поездка закончилась')
        },
        error: function () {
            alert('Ошибка аренды самоката.');
        }
    });
}

function endTrip() {
    console.log('Точно Кончаем')

    $.ajax({
        url: 'https://localhost:7209/Somokat/EndTrip',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            userId: localStorage.getItem('userId'),
            targetScooter: localStorage.getItem('targetScooter')
        }), success: function (response) {
            document.getElementById("rentButton").style.display = "flex";
            document.getElementById("endTripButton").style.display = "none";
            document.getElementById("overlay2").style.display = "none";

        },
        error: function () {
            alert('Ошибка завершения поездки.');
        }
    });
}

ymaps.ready(init);



