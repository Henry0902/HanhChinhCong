app.controller('AccountJs', function ($scope, $http, AlertService) {
    $scope.username = '';
    $scope.password = '';
    $scope.errorMessage = '';
    //$scope.currentHoTen = JSON.parse(window.currentHoTen);
    $scope.registerError = '';
    $scope.registerSuccess = '';

    $scope.login = function () {
        $http.post('/Login/AjaxLogin', {
            username: $scope.username,
            password: $scope.password
        }).then(function (res) {
            var result = res.data;
            if (result.success) {
                // Lưu thông tin user/quyền vào localStorage/sessionStorage nếu cần
                sessionStorage.setItem('userRole', result.role);
                debugger
                sessionStorage.setItem('userRoles', JSON.stringify(result.roles));

                // Điều hướng theo role
                var roles = result.roles || [result.role];
                if (roles.includes(1)) {
                    window.location.href = '/HoSo/QuanLyHoSo';
                } else if (roles.includes(2)) {
                    window.location.href = '/HoSo/Index';
                } else if (roles.includes(3)) {
                    window.location.href = '/HoSo/XuLy';
                } else if (roles.includes(4)) {
                    window.location.href = '/HoSo/PhanCong';
                } else if (roles.includes(5)) {
                    window.location.href = '/HoSo/TraKetQua';
                } else if (roles.includes(6)) {
                    window.location.href = '/HoSo/Index';
                }
            } else {
                $scope.errorMessage = result.message || 'Đăng nhập thất bại!';
            }
        }, function () {
            $scope.errorMessage = 'Lỗi hệ thống!';
        });
    };

    // đang ký người dùng là công dân
    $scope.register = function ($event) {
        if ($event) $event.preventDefault();

        $scope.registerError = '';
        $scope.registerSuccess = '';

        if (!$scope.reg_username || !$scope.reg_password || !$scope.reg_conf_password) {
            $scope.registerError = 'Vui lòng nhập đầy đủ thông tin!';
            return;
        }
        if ($scope.reg_password !== $scope.reg_conf_password) {
            $scope.registerError = 'Mật khẩu nhập lại không khớp!';
            return;
        }

        $http.post('/User/Register', {
            UserName: $scope.reg_username,
            PassWord: $scope.reg_password,
            HoTen: $scope.reg_hoten,
        }).then(function (res) {
            if (res.data.success) {
                $scope.registerSuccess = 'Đăng ký thành công! Bạn có thể đăng nhập.';
                $scope.reg_username = '';
                $scope.reg_password = '';
                $scope.reg_conf_password = '';
                $scope.reg_hoten = '';
                window.location.reload();

            } else {
                $scope.registerError = res.data.message || 'Đăng ký thất bại!';
                window.location.reload();
            }
        }, function () {
            $scope.registerError = 'Lỗi hệ thống!';
            window.location.reload();
        });
    };




});
