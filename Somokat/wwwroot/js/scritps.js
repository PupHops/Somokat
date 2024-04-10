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



// Функция для показа всплывающего окна
function showPopup() {
    $('#popup').fadeIn();
    $('#overlay').fadeIn();
}

// Функция для скрытия всплывающего окна
function hidePopup() {
    console.log('hui');
    $('#popup').fadeOut();
    $('#overlay').fadeOut();
}

// Обработчик клика на подложку для закрытия всплывающего окна
$('#overlay').click(function () {
    console.log('hui');

    hidePopup();
});

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
                var scooterName = "AbobaScooter"; // Замените на реальное название самоката
                var scooterNumber = deviceState; // Замените на реальный номер самоката
                var imageUrl = 'https://localhost:7099//images/Samokat.png'; // Замените на реальный URL картинки
                $('#scooter-image').attr('src', imageUrl);
                $('#scooter-name').text(scooterName);
                $('#charge-progress').attr('value', chargeLevel);

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
            provider: 'browser',
            mapStateAutoApply: true
        }).then(function (result) {
            
            result.geoObjects.options.set('preset', 'islands#blueCircleIcon');
            map.geoObjects.remove(result.geoObjects);
            map.geoObjects.add(result.geoObjects);
        });
    }

    // map.controls.remove('geolocationControl');
    map.controls.remove('searchControl'); // удаляем поиск
    map.controls.remove('trafficControl'); // удаляем контроль трафика
    map.controls.remove('typeSelector'); // удаляем тип
    map.controls.remove('fullscreenControl'); // удаляем кнопку перехода в полноэкранный режим
    map.controls.remove('zoomControl'); // удаляем контрол зуммирования
    map.controls.remove('rulerControl'); // удаляем контрол правил



}
window.onload = function () {
    var balance = localStorage.getItem('balance');
    if (balance !== null) {
        // Выводим баланс на странице Home
        document.getElementById('balanceDisplay').innerText = balance + "₽";
        document.getElementById('balanceDisplay1').innerText = balance + "₽";
    }
};
ymaps.ready(init);



