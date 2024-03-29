﻿let center = [48.8866527839977, 2.34310679732974];
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
                // Получение содержимого балуна
                var balloonContent = geoObject.properties.get('balloonContent');
                map.panTo(geoObject.geometry.getCoordinates(), {
                    flying: true
                });

                // Обновление содержимого всплывающего окна на вашем сайте
              //  updatePopupContent(balloonContent);
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
            // Синим цветом пометим положение, полученное через браузер.
            // Если браузер не поддерживает эту функциональность, метка не будет добавлена на карту.

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



