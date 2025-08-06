app.controller('LoaiHoSoJs', function ($scope, $http, AlertService) {
    $scope.message = "Quản lý loại hồ sơ";

    $scope.listData = [];
    $scope.searchName = '';
    $scope.page = 1;
    $scope.pageSize = 5;
    $scope.totalRows = 0;
    $scope.totalPages = 1;

    $scope.listLinhVuc = [];

    $scope.loadLinhVucDropdown = function () {
        $http.get('/LinhVuc/GetList')
            .then(function (res) {
                $scope.listLinhVuc = res.data;
            });
    };

    // Gọi khi mở modal thêm mới
    $scope.openAddModal = function () {
        $scope.editingLoaiHoSo = null;
        $scope.loadLinhVucDropdown();
        $('#myModal').modal('show');
    };

    $scope.editingLoaiHoSo = null;
    $scope.editingIndex = null;

    $scope.editLoaiHoSo = function (item, index) {
        $scope.editingLoaiHoSo = angular.copy(item);
        $scope.editingIndex = index;
        $('#TenLoaiHoSo').val(item.TenLoaiHoSo);
        $scope.selectedLinhVuc = item.IdLinhVuc;
        $('#IdLinhVuc').val(item.IdLinhVuc);
        $scope.loadLinhVucDropdown();
        $('#myModal').modal('show');
    };

    $scope.addModal = function () {
        var form = document.forms['loaiHoSoForm'];
        if (!form.checkValidity()) {
            form.classList.add('was-validated');
            return;
        }
        var loaiHoSoData = {
            Id: $scope.editingLoaiHoSo ? $scope.editingLoaiHoSo.Id : 0,
            TenLoaiHoSo: $('#TenLoaiHoSo').val(),
            IdLinhVuc: $scope.selectedLinhVuc
        };
        if ($scope.editingLoaiHoSo) {
            // Sửa loại hồ sơ
            $http.post('/LoaiHoSo/EditLoaiHoSo', loaiHoSoData)
                .then(function (response) {
                    if (response.data.success) {
                        $('#myModal').modal('hide');
                        $scope.editingLoaiHoSo = null;
                        $scope.editingIndex = null;
                        $scope.loadLoaiHoSo();
                        AlertService && AlertService.show('success', 'Sửa thành công!');
                    } else {
                        AlertService && AlertService.show('danger', 'Sửa thất bại!');
                    }
                }, function (error) {
                    AlertService && AlertService.show('danger', 'Lỗi máy chủ!');
                });
        } else {
            // Thêm mới loại hồ sơ
            $http.post('/LoaiHoSo/AddLoaiHoSo', loaiHoSoData)
                .then(function (response) {
                    if (response.data.success) {
                        $('#myModal').modal('hide');
                        $scope.loadLoaiHoSo();
                        AlertService && AlertService.show('success', 'Thêm mới thành công!');
                    } else {
                        AlertService && AlertService.show('danger', 'Thêm mới thất bại!');
                    }
                }, function (error) {
                    AlertService && AlertService.show('danger', 'Lỗi máy chủ!');
                });
        }
    };

    $scope.deleteLoaiHoSo = function (id) {
        $http.post('/LoaiHoSo/DeleteLoaiHoSo', { id: id })
            .then(function (res) {
                if (res.data.success) {
                    $scope.loadLoaiHoSo();
                    AlertService && AlertService.show('success', 'Xóa thành công!');
                } else {
                    AlertService && AlertService.show('danger', 'Xóa thất bại!');
                }
            });
    };

    $scope.loadLoaiHoSo = function () {
        $http.get('/LoaiHoSo/GetPagedLoaiHoSo', {
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
        $scope.loadLoaiHoSo();
    };

    $scope.goToPage = function (p) {
        if (p >= 1 && p <= $scope.totalPages) {
            $scope.page = p;
            $scope.loadLoaiHoSo();
        }
    };

    // Reset form khi đóng modal
    $('#myModal').on('hidden.bs.modal', function () {
        document.forms['loaiHoSoForm'].reset();
        $('.needs-validation').removeClass('was-validated');
        $scope.editingLoaiHoSo = null;
        $scope.editingIndex = null;
        $scope.selectedLinhVuc = null;
        $scope.$apply();
    });

    // Initial load
    $scope.loadLoaiHoSo();
});
