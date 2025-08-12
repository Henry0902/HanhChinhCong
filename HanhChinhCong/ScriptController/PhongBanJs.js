app.controller('PhongBanJs', function ($scope, $http, AlertService) {
    $scope.message = "Quản lý phòng ban";

    $scope.listData = [];
    $scope.searchName = '';
    $scope.page = 1;
    $scope.pageSize = 5;
    $scope.totalRows = 0;
    $scope.totalPages = 1;

    $scope.editingPhongBan = null;
    $scope.editingIndex = null;

    $scope.editPhongBan = function (item, index) {
        $scope.editingPhongBan = angular.copy(item);
        $scope.editingIndex = index;
        $('#TenPhongBan').val(item.TenPhongBan);
    };

    $scope.addModal = function () {
        var form = document.forms['phongBanForm'];
        if (!form.checkValidity()) {
            form.classList.add('was-validated');
            return;
        }
        var phongBanData = {
            Id: $scope.editingPhongBan ? $scope.editingPhongBan.Id : 0,
            TenPhongBan: $('#TenPhongBan').val()
        };
        if ($scope.editingPhongBan) {
            // Sửa phòng ban
            $http.post('/PhongBan/EditPhongBan', phongBanData)
                .then(function (response) {
                    if (response.data.success) {
                        $('#myModal').modal('hide');
                        $scope.editingPhongBan = null;
                        $scope.editingIndex = null;
                        $scope.loadPhongBan();
                        AlertService && AlertService.show('success', 'Sửa thành công!');
                    } else {
                        AlertService && AlertService.show('danger', 'Sửa thất bại!');
                    }
                }, function (error) {
                    AlertService && AlertService.show('danger', 'Lỗi máy chủ!');
                });
        } else {
            // Thêm mới phòng ban
            $http.post('/PhongBan/AddPhongBan', phongBanData)
                .then(function (response) {
                    if (response.data.success) {
                        $('#myModal').modal('hide');
                        $scope.loadPhongBan();
                        AlertService && AlertService.show('success', 'Thêm mới thành công!');
                    } else {
                        AlertService && AlertService.show('danger', 'Thêm mới thất bại!');
                    }
                }, function (error) {
                    AlertService && AlertService.show('danger', 'Lỗi máy chủ!');
                });
        }
    };

    $scope.deleteItem = function (id) {
        $scope.deleteItemId = id;
        $('#confirmDeleteModal').modal('show');
    };

    $scope.confirmDelete = function () {
        $('#confirmDeleteModal').modal('hide');
        $http.post('/PhongBan/DeletePhongBan', { id: $scope.deleteItemId })
            .then(function (response) {
                if (response.data.success) {
                    $scope.loadPhongBan(); // hoặc $scope.search() nếu có
                    AlertService.show('success', 'Xóa thành công!');
                } else {
                    AlertService.show('danger', response.data.message || 'Xóa thất bại!');
                }
            }, function () {
                AlertService.show('danger', 'Đã có lỗi xảy ra!');
            });
    };

    $scope.loadPhongBan = function () {
        $http.get('/PhongBan/GetPagedPhongBan', {
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
        $scope.loadPhongBan();
    };

    $scope.goToPage = function (p) {
        if (p >= 1 && p <= $scope.totalPages) {
            $scope.page = p;
            $scope.loadPhongBan();
        }
    };

    // Reset form khi đóng modal
    $('#myModal').on('hidden.bs.modal', function () {
        document.forms['phongBanForm'].reset();
        $('.needs-validation').removeClass('was-validated');
        $scope.editingPhongBan = null;
        $scope.editingIndex = null;
        $scope.$apply();
    });

    // Initial load
    $scope.loadPhongBan();
});