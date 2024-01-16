let center = [48.8866527839977, 2.34310679732974];
let lat = 0;
let long = 0;
const options = {
    enableHighAccuracy: true,
    timeout: 5000,
    maximumAge: 0,
};
const express = require('express');
const app = express();

app.use((req, res, next) => {
    res.header('Access-Control-Allow-Origin', '*'); // Разрешить доступ всем доменам (не безопасно для продакшена)
    // Другие заголовки и настройки могут быть добавлены по необходимости
    next();
});
app.use(cors());

function success(pos) {
    const crd = pos.coords;

    console.log("Your currrrrrrrrrrrrrrrrrrent position is:");
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
    var secondButton = new ymaps.control.Button({
        data: {
            // Иконка имеет размер 16х16 пикселей.
            image: 'images/center.svg'

        },
        options: {
            // Поскольку кнопка будет менять вид в зависимости от размера карты,
            // зададим ей три разных значения maxWidth в массиве.
            
            maxWidth: [28, 150, 178]
        }
    });
    map.controls.add(secondButton);
   

    jQuery.getJSON('http://localhost:5141/Somokat', function (json) {
        /** Сохраним ссылку на геообъекты на случай, если понадобится какая-либо постобработка.
         * @see https://api.yandex.ru/maps/doc/jsapi/2.1/ref/reference/GeoQueryResult.xml
        */
        var geoObjects = ym.geoQuery(json)
            .addToMap(map)
            .applyBoundsToMap(map, {
                checkZoomRange: true
            });
    });
 


    map.controls.remove('searchControl'); // удаляем поиск
    map.controls.remove('trafficControl'); // удаляем контроль трафика
    map.controls.remove('typeSelector'); // удаляем тип
    map.controls.remove('fullscreenControl'); // удаляем кнопку перехода в полноэкранный режим
    map.controls.remove('zoomControl'); // удаляем контрол зуммирования
    map.controls.remove('rulerControl'); // удаляем контрол правил


}

ymaps.ready(init);



