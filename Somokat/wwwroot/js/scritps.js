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


// Создаем скрытый элемент для всплывающего окна с информацией о самокате
var popupElement = $('<div id="scooterPopup"></div>')
    .css({
        position: 'fixed',
        bottom: '0',
        left: '0',
        width: '100%', // Установка ширины на половину экрана
        height: '50%', // Установка высоты на половину экрана
        background: '#fff',
        padding: '20px',
        boxShadow: '0 0 10px rgba(0, 0, 0, 0.3)',
        display: 'none',
        zIndex: 9999 // Устанавливаем z-index, чтобы окно было поверх остального контента
    })
    .appendTo('body');

// Доб  авляем кнопку закрытия
var closeButton = $('<button class="close-btn">Close</button>')
    .css({
        position: 'absolute',
        top: '10px',
        right: '10px',
        padding: '5px 10px',
        background: '#ccc',
        border: 'none',
        cursor: 'pointer'
    })
    .appendTo(popupElement);

// Функция для обновления содержимого всплывающего окна
function updatePopupContent(content) {
    popupElement.html(content);
}

// Функция для показа всплывающего окна
function showPopup() {
    popupElement.slideDown();
}

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

// Функция для показа подложки
function showOverlay() {
    overlayElement.fadeIn();
}

// Функция для скрытия подложки
function hideOverlay() {
    overlayElement.fadeOut();
}

// Обработчик клика на подложку для закрытия окна
overlayElement.on('click', function () {
    hidePopup();
    hideOverlay();
});
// Функция для показа всплывающего окна
function showPopup() {
    popupElement.fadeIn();
}

// Функция для скрытия всплывающего окна
function hidePopup() {
    popupElement.fadeOut();
}

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
            iconLayout: 'default#image',
            iconImageHref: 'https://localhost:7099/images/scooter.svg',
            iconImageSize: [60, 67],
            iconImageOffset: [-25, -52]
        });
        geoObjects.each(function (geoObject) {
            geoObject.events.add('click', function (e) {
                var balloonContent = geoObject.properties.get('balloonContent');
                map.panTo(geoObject.geometry.getCoordinates(), { flying: true });
                updatePopupContent(balloonContent);
                showPopup();
                showOverlay(); // Показываем подложку

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

ymaps.ready(init);



