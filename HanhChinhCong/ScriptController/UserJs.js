app.controller('UserJs', function ($scope, $http, AlertService) {
    $scope.message = "Chào mừng bạn đến với Phần mềm hành chính công";

    $scope.searchName = '';
    $scope.page = 1;
    $scope.pageSize = 5;
    $scope.totalRows = 0;
    $scope.totalPages = 1;

    $scope.listRoles = []; // Danh sách vai trò động
    $scope.listData = [];

    $scope.searchRole = null;
    $scope.newUser = null;
    $scope.editingUser = null;

    $http.get('/User/GetAllRoles').then(function (res) {
        $scope.listRoles = res.data;
    });

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


    $scope.roles = []; // Danh sách role lấy từ backend

    $scope.loadUsers = function () {
        // Lấy danh sách role trước, sau đó lấy danh sách user
        $http.get('/User/GetAllRoles').then(function (roleRes) {
            $scope.roles = roleRes.data;

            $http.get('/User/GetPagedUsers', {
                params: {
                    searchName: $scope.searchName,
                    searchRole: $scope.searchRole,
                    page: $scope.page,
                    pageSize: $scope.pageSize
                }
            }).then(function (res) {
                $scope.listData = res.data.data.map(function (user) {
                    let userNew = { ...user };
                    // Tìm role theo Id
                    const findRole = $scope.roles.find(role => user.Role == role.Id);
                    userNew.RoleText = findRole ? findRole.Name : 'Không xác định';
                    return userNew;
                });
                $scope.totalRows = res.data.totalRows;
                $scope.totalPages = Math.ceil($scope.totalRows / $scope.pageSize);
            });
        });
    };


    $scope.deleteUser = function (id) {
        $scope.deleteUserId = id;
        $('#confirmDeleteModal').modal('show');
    };

    $scope.confirmDelete = function () {
        $('#confirmDeleteModal').modal('hide');
        $http.post('/User/Delete', { id: $scope.deleteUserId })
            .then(function (response) {
                if (response.data.success) {
                    $scope.search();
                    AlertService.show('success', 'Xóa thành công!');
                } else {
                    AlertService.show('danger', response.data.message || 'Xóa thất bại!');
                }
            }, function () {
                AlertService.show('danger', 'Đã có lỗi xảy ra!');
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
