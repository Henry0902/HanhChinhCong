app.controller('RoleJs', function ($scope, $http, AlertService) {
    $scope.searchName = '';
    $scope.page = 1;
    $scope.pageSize = 5;
    $scope.totalRows = 0;
    $scope.totalPages = 1;
    $scope.listData = [];

    $scope.newRole = {};
    $scope.editingRole = null;
    $scope.editingIndex = null;

    $scope.loadRoles = function () {
        $http.get('/Role/GetPagedRoles', {
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
        $scope.loadRoles();
    };

    $scope.goToPage = function (p) {
        if (p >= 1 && p <= $scope.totalPages) {
            $scope.page = p;
            $scope.loadRoles();
        }
    };

    $scope.openAddRoleModal = function () {
        $scope.newRole = {};
        $('#addRoleModal').modal('show');
    };

    $scope.addRole = function () {
        $http.post('/Role/AddRole', $scope.newRole)
            .then(function (response) {
                if (response.data.success) {
                    $('#addRoleModal').modal('hide');
                    $scope.newRole = {};
                    $scope.loadRoles();
                    AlertService && AlertService.show('success', 'Thêm mới thành công!');
                } else {
                    AlertService && AlertService.show('danger', 'Thêm mới thất bại!');
                }
            }, function () {
                AlertService && AlertService.show('danger', 'Lỗi máy chủ!');
            });
    };

    $scope.openEditRoleModal = function (role, index) {
        $scope.editingRole = angular.copy(role);
        $scope.editingIndex = index;
        $('#editRoleModal').modal('show');
    };

    $scope.editRoleSubmit = function () {
        $http.post('/Role/EditRole', $scope.editingRole)
            .then(function (response) {
                if (response.data.success) {
                    $('#editRoleModal').modal('hide');
                    $scope.editingRole = null;
                    $scope.editingIndex = null;
                    $scope.loadRoles();
                    AlertService && AlertService.show('success', 'Sửa thành công!');
                } else {
                    AlertService && AlertService.show('danger', 'Sửa thất bại!');
                }
            }, function () {
                AlertService && AlertService.show('danger', 'Lỗi máy chủ!');
            });
    };

    $scope.deleteRole = function (id) {
        if (!confirm('Bạn có chắc chắn muốn xóa vai trò này?')) return;
        $http.post('/Role/DeleteRole', { id: id })
            .then(function (response) {
                if (response.data.success) {
                    $scope.loadRoles();
                    AlertService && AlertService.show('success', 'Xóa thành công!');
                } else {
                    AlertService && AlertService.show('danger', 'Xóa thất bại!');
                }
            }, function () {
                AlertService && AlertService.show('danger', 'Lỗi máy chủ!');
            });
    };

    // Initial load
    $scope.loadRoles();
});
