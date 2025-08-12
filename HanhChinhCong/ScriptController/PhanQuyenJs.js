app.controller('PhanQuyenJs', function ($scope, $http) {
    $scope.users = [];
    $scope.roles = [];
    $scope.selectedUserId = null;
    $scope.successMessage = '';
    $scope.errorMessage = '';


   

    $http.get('/User/GetAllUsers').then(function (res) {
        $scope.users = res.data;
    });

    $http.get('/User/GetAllRoles').then(function (res) {
        $scope.roles = res.data;
        $scope.roles.forEach(function (r) { r.selected = false; });
    });

    $scope.loadUserRoles = function () {
        $scope.successMessage = '';
        $scope.errorMessage = '';
        if (!$scope.selectedUserId) return;
        $http.get('/User/GetUserRoles', { params: { userId: $scope.selectedUserId } })
            .then(function (res) {
                var userRoleIds = res.data;
                $scope.roles.forEach(function (r) {
                    r.selected = userRoleIds.indexOf(r.Id) !== -1;
                });
            });
    };

    $scope.clearMessages = function () {
        $scope.successMessage = '';
        $scope.errorMessage = '';
    };

    $scope.updateUserRoles = function () {
        var selectedRoleIds = $scope.roles.filter(r => r.selected).map(r => r.Id);
        $http.post('/User/UpdateUserRoles', { userId: $scope.selectedUserId, roleIds: selectedRoleIds })
            .then(function (res) {
                if (res.data.success) {
                    $scope.successMessage = 'Cập nhật phân quyền thành công!';
                    $scope.errorMessage = '';
                } else {
                    $scope.successMessage = '';
                    $scope.errorMessage = 'Cập nhật phân quyền thất bại!';
                }
            }, function () {
                $scope.successMessage = '';
                $scope.errorMessage = 'Lỗi máy chủ!';
            });
    };

   
});
