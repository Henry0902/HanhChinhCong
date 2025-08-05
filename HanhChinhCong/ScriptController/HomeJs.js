app.controller('HomeJs', function ($scope, $http, AlertService) {
    $scope.message = "Chào mừng bạn đến với Phần mềm hành chính công";
    const listRoles = [
        { value: 0, label: 'Admin' },
        { value: 1, label: 'Cán bộ tiếp nhận' },
        { value: 2, label: 'Cán bộ xử lý' },
        { value: 3, label: 'Lãnh đạo' },
    ]

    

    $scope.listData = [];

    var getData = function () {
        return $http({
            url: "/Home/GetList",
            method: "GET",
        });
    }

    const init = function () {
        getData().then(function (res) {
            $scope.listData = res.data.map(function (user) {
                let userNew = { ...user };
                const findRole = listRoles.filter((role) => user.VaiTro == role.value);
                userNew.VaiTroText = findRole[0].label;
                return userNew;
            });
            console.log($scope.listData);
        }).catch(function (err) {
            console.log(err);
        })
    }

    init();

    $scope.addUser = function () {
        $http({
            url: "/Home/Create",
            method: "POST",
        }).then(function (res) {
            console.log(res);
        }).catch(function (err) {
            console.log(err);
        })
    }
});