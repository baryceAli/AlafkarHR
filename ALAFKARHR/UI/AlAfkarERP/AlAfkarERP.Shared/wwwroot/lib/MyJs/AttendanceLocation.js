window.attendanceLocation = {
    getCurrentPosition: function () {
        return new Promise(function (resolve) {
            if (!navigator.geolocation) {
                resolve({
                    success: false,
                    message: "Location is not supported on this device or browser."
                });
                return;
            }

            navigator.geolocation.getCurrentPosition(
                function (position) {
                    resolve({
                        success: true,
                        latitude: position.coords.latitude,
                        longitude: position.coords.longitude,
                        accuracyMeters: position.coords.accuracy
                    });
                },
                function (error) {
                    var message = "Unable to read your current location.";
                    if (error.code === error.PERMISSION_DENIED) {
                        message = "Location permission was denied. Allow location access to check in.";
                    } else if (error.code === error.POSITION_UNAVAILABLE) {
                        message = "Current location is unavailable. Try again from an area with GPS or network coverage.";
                    } else if (error.code === error.TIMEOUT) {
                        message = "Location lookup timed out. Try refreshing your location.";
                    }

                    resolve({
                        success: false,
                        message: message
                    });
                },
                {
                    enableHighAccuracy: true,
                    timeout: 15000,
                    maximumAge: 30000
                });
        });
    }
};
