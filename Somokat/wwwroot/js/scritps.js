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

    geolocation.get({
            provider: 'browser',
            mapStateAutoApply: true
    }).then(function (result) {
            // Синим цветом пометим положение, полученное через браузер.
            // Если браузер не поддерживает эту функциональность, метка не будет добавлена на карту.
            result.geoObjects.options.set('preset', 'islands#blueCircleIcon');
            map.geoObjects.add(result.geoObjects);
    });

    objectManager = new ymaps.ObjectManager({
        // Чтобы метки начали кластеризоваться, выставляем опцию.
        clusterize: true,
        // ObjectManager принимает те же опции, что и кластеризатор.
        gridSize: 32,
        clusterDisableClickZoom: true
    });

    // Чтобы задать опции одиночным объектам и кластерам,
    // обратимся к дочерним коллекциям ObjectManager.
    objectManager.objects.options.set('preset', 'islands#greenDotIcon');
    objectManager.clusters.options.set('preset', 'islands#greenClusterIcons');
    map.geoObjects.add(objectManager);

    $.ajax({
        url: "data.json"
    }).done(function (data) {
        objectManager.add(data);
    });

    map.controls.remove('searchControl'); // удаляем поиск
    map.controls.remove('trafficControl'); // удаляем контроль трафика
    map.controls.remove('typeSelector'); // удаляем тип
    map.controls.remove('fullscreenControl'); // удаляем кнопку перехода в полноэкранный режим
    map.controls.remove('zoomControl'); // удаляем контрол зуммирования
    map.controls.remove('rulerControl'); // удаляем контрол правил


}

ymaps.ready(init);



