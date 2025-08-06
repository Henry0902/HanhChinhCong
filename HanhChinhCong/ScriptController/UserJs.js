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
    //console.log("list role", listRoles)

    $scope.listData = [];

    var getData = function () {
        return $http({
            url: "/User/GetList",
            method: "GET",
        }).then(function (res) {
            $scope.listData = res.data.map(function (user) {
                let userNew = { ...user };
                const findRole = $scope.listRoles.filter((role) => user.VaiTro == role.value);
                userNew.VaiTroText = findRole[0].label;
                return userNew;
            });
            console.log($scope.listData);
        }).catch(function (err) {
            console.log(err);
        });
    }

    getData(); 

    $scope.editingUser = null;
    $scope.editingIndex = null;

    $scope.editUser = function (user, index) {

        $scope.selectedRole = '';
        $scope.editingUser = angular.copy(user);
        $scope.editingIndex = index;

        $('#HoTen').val(user.HoTen);
        $scope.selectedRole = user.VaiTro;
        console.log("ủe", user);
        console.log("vai tro", $scope.selectedRole);
        $('#UserName').val(user.UserName);
        $('#PassWord').val(user.PassWord);

        // Mở modal (nếu không dùng data-bs-toggle)
        // $('#myModal').modal('show');
    };

    $scope.addModal = function () {
        var form = document.forms['userForm'];
        if (!form.checkValidity()) {
            form.classList.add('was-validated');
            return;
        }

        var userData = {
            Id: $scope.editingUser ? $scope.editingUser.Id : 0,
            HoTen: $('#HoTen').val(),
            VaiTro: $scope.selectedRole,
            UserName: $('#UserName').val(),
            PassWord: $('#PassWord').val()
        };

        if ($scope.editingUser) {
            // Sửa user
            $http.post('/User/EditUser', userData)
                .then(function (response) {
                    if (response.data.success) {
                        $('#myModal').modal('hide');
                        $scope.editingUser = null;
                        $scope.editingIndex = null;
                        getData().then(function () {
                            AlertService.show('success', 'Sửa thành công!');
                        });
                    } else {
                        AlertService.show('danger', 'Sửa thất bại!');
                    }
                }, function (error) {
                    AlertService.show('danger', 'Lỗi máy chủ!');
                });
        } else {
            // Thêm mới user
            $http.post('/User/AddUser', userData)
                .then(function (response) {
                    if (response.data.success) {
                        $('#myModal').modal('hide');
                        getData().then(function () {
                            AlertService.show('success', 'Thêm mới thành công!');
                        });
                    } else {
                        AlertService.show('danger', 'Thêm mới thất bại!');
                    }
                }, function (error) {
                    AlertService.show('danger', 'Lỗi máy chủ!');
                });
        }
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
                page: $scope.page,
                pageSize: $scope.pageSize
            }
        }).then(function (res) {
            $scope.listData = res.data.data;
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
