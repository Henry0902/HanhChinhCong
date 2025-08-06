app.controller('LinhVucJs', function ($scope, $http, AlertService) {
    $scope.message = "Quản lý lĩnh vực";

    $scope.listData = [];
    $scope.searchName = '';
    $scope.page = 1;
    $scope.pageSize = 5;
    $scope.totalRows = 0;
    $scope.totalPages = 1;

    var getData = function () {
        return $http({
            url: "/LinhVuc/GetList",
            method: "GET",
        }).then(function (res) {
            $scope.listData = res.data;
        }).catch(function (err) {
            console.log(err);
        });
    }
    getData();

    $scope.listPhongBan = [];

    $scope.loadPhongBanDropdown = function () {
        $http.get('/PhongBan/GetList')
            .then(function (res) {
                $scope.listPhongBan = res.data;
                
            });
    };

    // Gọi khi mở modal thêm mới
    $scope.openAddModal = function () {
        $scope.editingLinhVuc = null;
        $scope.loadPhongBanDropdown();
        $('#myModal').modal('show');
    };


    $scope.editingLinhVuc = null;
    $scope.editingIndex = null;

    $scope.editLinhVuc = function (item, index) {
        $scope.editingLinhVuc = angular.copy(item);
        $scope.editingIndex = index;
        $('#TenLinhVuc').val(item.TenLinhVuc);
        $('#IdPhongBan').val(item.IdPhongBan);
    };

    $scope.addModal = function () {
        var form = document.forms['linhVucForm'];
        if (!form.checkValidity()) {
            form.classList.add('was-validated');
            return;
        }
        var linhVucData = {
            Id: $scope.editingLinhVuc ? $scope.editingLinhVuc.Id : 0,
            TenLinhVuc: $('#TenLinhVuc').val(),
            IdPhongBan: $scope.selectedPhongBan
            
        };
        console.log("idphongban", $scope.selectedPhongBan);
        if ($scope.editingLinhVuc) {
            // Sửa lĩnh vực
            $http.post('/LinhVuc/EditLinhVuc', linhVucData)
                .then(function (response) {
                    if (response.data.success) {
                        $('#myModal').modal('hide');
                        $scope.editingLinhVuc = null;
                        $scope.editingIndex = null;
                        $scope.loadLinhVuc();
                        AlertService && AlertService.show('success', 'Sửa thành công!');
                    } else {
                        AlertService && AlertService.show('danger', 'Sửa thất bại!');
                    }
                }, function (error) {
                    AlertService && AlertService.show('danger', 'Lỗi máy chủ!');
                });
        } else {
            // Thêm mới lĩnh vực
            $http.post('/LinhVuc/AddLinhVuc', linhVucData)
                .then(function (response) {
                    if (response.data.success) {
                        $('#myModal').modal('hide');
                        $scope.loadLinhVuc();
                        AlertService && AlertService.show('success', 'Thêm mới thành công!');
                    } else {
                        AlertService && AlertService.show('danger', 'Thêm mới thất bại!');
                    }
                }, function (error) {
                    AlertService && AlertService.show('danger', 'Lỗi máy chủ!');
                });
        }
    };

    $scope.deleteLinhVuc = function (id) {
        $http.post('/LinhVuc/DeleteLinhVuc', { id: id })
            .then(function (res) {
                if (res.data.success) {
                    $scope.loadLinhVuc();
                    AlertService && AlertService.show('success', 'Xóa thành công!');
                } else {
                    AlertService && AlertService.show('danger', 'Xóa thất bại!');
                }
            });
    };

    $scope.loadLinhVuc = function () {
        $http.get('/LinhVuc/GetPagedLinhVuc', {
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
        $scope.loadLinhVuc();
    };

    $scope.goToPage = function (p) {
        if (p >= 1 && p <= $scope.totalPages) {
            $scope.page = p;
            $scope.loadLinhVuc();
        }
    };

    // Reset form khi đóng modal
    $('#myModal').on('hidden.bs.modal', function () {
        document.forms['linhVucForm'].reset();
        $('.needs-validation').removeClass('was-validated');
        $scope.editingLinhVuc = null;
        $scope.editingIndex = null;
        $scope.$apply();
    });

    // Initial load
    $scope.loadLinhVuc();
});
