app.controller('UserJs', function ($scope, $http, AlertService) {
    $scope.message = "Chào mừng bạn đến với Phần mềm hành chính công";

    $scope.searchName = '';
    $scope.page = 1;
    $scope.pageSize = 5;
    $scope.totalRows = 0;
    $scope.totalPages = 1;


    $scope.listRoles = [
        { value: 0, label: 'Admin' },
        { value: 1, label: 'Cán bộ tiếp nhận' },
        { value: 2, label: 'Cán bộ xử lý' },
        { value: 3, label: 'Lãnh đạo' },
    ]
    $scope.searchVaiTro = '';

    $scope.listData = [];

    //var getData = function () {
    //    return $http({
    //        url: "/User/GetList",
    //        method: "GET",
    //    }).then(function (res) {
    //        $scope.listData = res.data.map(function (user) {
    //            let userNew = { ...user };
    //            const findRole = $scope.listRoles.filter((role) => user.VaiTro == role.value);
    //            userNew.VaiTroText = findRole.length > 0 ? findRole[0].label : 'Không xác định';
    //            return userNew;
    //        });
          
    //    }).catch(function (err) {
    //        console.log(err);
    //    });
    //}

    //getData(); 

    $scope.newUser = {};

    $scope.addUser = function () {
        // Validate and send $scope.newUser to backend
        $http.post('/User/AddUser', $scope.newUser)
            .then(function (response) {
                if (response.data.success) {
                    $('#addUserModal').modal('hide');
                    $scope.newUser = {};
                    $scope.loadUsers();
                    AlertService.show('success', 'Thêm mới thành công!');
                } else {
                    AlertService.show('danger', 'Thêm mới thất bại!');
                }
            }, function (error) {
                AlertService.show('danger', 'Lỗi máy chủ!');
            });
    };

    $scope.openEditUserModal = function (user, index) {
        $scope.editingUser = angular.copy(user);
        $scope.editingIndex = index;
        $('#editUserModal').modal('show');
    };

    $scope.editUserSubmit = function () {
        $http.post('/User/EditUser', $scope.editingUser)
            .then(function (response) {
                if (response.data.success) {
                    $('#editUserModal').modal('hide');
                    $scope.editingUser = null;
                    $scope.editingIndex = null;
                    $scope.loadUsers();
                    AlertService.show('success', 'Sửa thành công!');
                } else {
                    AlertService.show('danger', 'Sửa thất bại!');
                }
            }, function (error) {
                AlertService.show('danger', 'Lỗi máy chủ!');
            });
    };


    $('#myModal').on('hidden.bs.modal', function () {
        // Reset form
        document.forms['userForm'].reset();
        $('.needs-validation').removeClass('was-validated');
        // Reset biến sửa
        $scope.editingUser = null;
        $scope.editingIndex = null;
        $scope.selectedRole = '';
        $scope.$apply();
    });


    $scope.loadUsers = function () {
        $http.get('/User/GetPagedUsers', {
            params: {
                searchName: $scope.searchName,
                searchVaiTro: $scope.searchVaiTro,
                page: $scope.page,
                pageSize: $scope.pageSize
            }
          
        }).then(function (res) {
            $scope.listData = res.data.data.map(function (user) {
                let userNew = { ...user };
                const findRole = $scope.listRoles.find(role => user.VaiTro == role.value);
                userNew.VaiTroText = findRole ? findRole.label : 'Không xác định';
                return userNew;
            });
            $scope.totalRows = res.data.totalRows;
            $scope.totalPages = Math.ceil($scope.totalRows / $scope.pageSize);
        });
    };

    $scope.search = function () {
        $scope.page = 1;
        $scope.loadUsers();
    };

    $scope.goToPage = function (p) {
        if (p >= 1 && p <= $scope.totalPages) {
            $scope.page = p;
            $scope.loadUsers();
        }
    };

    // Initial load
    $scope.loadUsers();
});
