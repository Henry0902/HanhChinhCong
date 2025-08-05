app.factory('AlertService', function ($rootScope) {
    $rootScope.alert = {
        type: '',
        message: '',
        show: false
    };

    return {
        show: function (type, message) {
            $rootScope.alert.type = type;
            $rootScope.alert.message = message;
            $rootScope.alert.show = true;
            setTimeout(function () {
                $rootScope.alert.show = false;
                $rootScope.$apply();
            }, 3000);
        }
    };
});
