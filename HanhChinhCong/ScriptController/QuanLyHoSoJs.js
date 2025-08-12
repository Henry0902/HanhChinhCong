app.controller('QuanLyHoSoJs', function ($scope, $http, AlertService) {
    $scope.listData = [];
    $scope.page = 1;
    $scope.pageSize = 5;
    $scope.totalRows = 0;
    $scope.totalPages = 1;
    $scope.searchName = '';
    $scope.searchTenCongDan = '';
    $scope.searchCMND_CCCD = '';

    $scope.detailHoSo = {};
    $scope.detailQuaTrinh = [];
    $scope.editingHoSo = {};
    $scope.selectedFiles = [];
    $scope.attachedFiles = [];
    $scope.filesToDelete = [];

    $scope.trangThaiList = [];
    $scope.searchIdTrangThai = '';

    //lấy list trạng thái hồ sơ
    $http.get('/HoSo/GetTrangThaiHoSo').then(function (res) {
        $scope.trangThaiList = res.data;
    });


    $scope.loadHoSo = function () {
        $http.get('/HoSo/GetPagedHoSo', {
            params: {
                searchName: $scope.searchName,
                searchTenCongDan: $scope.searchTenCongDan,
                searchCMND_CCCD: $scope.searchCMND_CCCD,
                searchIdTrangThai: $scope.searchIdTrangThai,
                page: $scope.page,
                pageSize: $scope.pageSize
            }
        }).then(function (res) {
            res.data.data.forEach(function (item) {
                if (item.NgayTiepNhan) {
                    var d = parseDotNetDate(item.NgayTiepNhan);
                    item.NgayTiepNhan = (d instanceof Date && !isNaN(d.getTime())) ? formatDate(d) : "";
                }
                if (item.HanXuLy) {
                    var d = parseDotNetDate(item.HanXuLy);
                    item.HanXuLy = (d instanceof Date && !isNaN(d.getTime())) ? formatDate(d) : "";
                }
            });
            $scope.listData = res.data.data;
            $scope.totalRows = res.data.totalRows;
            $scope.totalPages = Math.ceil($scope.totalRows / $scope.pageSize);
        });
    };

    $scope.search = function () {
        $scope.page = 1;
        $scope.loadHoSo();
    };

    $scope.goToPage = function (p) {
        if (p >= 1 && p <= $scope.totalPages) {
            $scope.page = p;
            $scope.loadHoSo();
        }
    };

    $scope.showEditModal = function (item) {
        $scope.editingHoSo = angular.copy(item);
        if ($scope.editingHoSo.NgayTiepNhan) {
            $scope.editingHoSo.NgayTiepNhan = new Date($scope.editingHoSo.NgayTiepNhan);
        }
        if ($scope.editingHoSo.HanXuLy) {
            $scope.editingHoSo.HanXuLy = new Date($scope.editingHoSo.HanXuLy);
        }
        $scope.selectedFiles = [];
        $scope.attachedFiles = [];
        $scope.filesToDelete = [];
        $http.get('/HoSo/GetFilesByHoSoId', { params: { hoSoId: item.Id } })
            .then(function (res) {
                $scope.attachedFiles = res.data;
            });
        $('#hoSoModal').modal('show');
    };

    $scope.removeAttachedFile = function (file, index) {
        if (!$scope.filesToDelete) $scope.filesToDelete = [];
        $scope.filesToDelete.push(file);
        $scope.attachedFiles.splice(index, 1);
    };

    $scope.removeFile = function (index) {
        $scope.selectedFiles.splice(index, 1);
        if ($scope.selectedFiles.length === 0) {
            document.getElementById('fileDinhKem').value = '';
        }
    };

    $scope.editHoSo = function (hoSo) {
        if (hoSo.NgayTiepNhan) {
            hoSo.NgayTiepNhan = formatDate(hoSo.NgayTiepNhan);
        }
        if (hoSo.HanXuLy) {
            hoSo.HanXuLy = formatDate(hoSo.HanXuLy);
        }
        var formData = new FormData();
        for (var key in hoSo) {
            if (hoSo.hasOwnProperty(key)) {
                formData.append(key, hoSo[key]);
            }
        }
        for (var i = 0; i < $scope.selectedFiles.length; i++) {
            formData.append('FileDinhKem', $scope.selectedFiles[i]);
        }
        if ($scope.filesToDelete && $scope.filesToDelete.length > 0) {
            formData.append('FilesToDelete', JSON.stringify($scope.filesToDelete.map(f => f.TenFile)));
        }
        $http.post('/HoSo/EditHoSo', formData, {
            headers: { 'Content-Type': undefined },
            transformRequest: angular.identity
        }).then(function (res) {
            if (res.data.success) {
                $scope.loadHoSo();
                $('#hoSoModal').modal('hide');
                AlertService && AlertService.show('success', 'Sửa thành công!');
            } else {
                AlertService && AlertService.show('danger', 'Sửa thất bại!');
            }
        });
    };

    $scope.deleteItem = function (id) {
        $scope.deleteItemId = id;
        $('#confirmDeleteModal').modal('show');
    };

    $scope.confirmDelete = function () {
        $('#confirmDeleteModal').modal('hide');
        $http.post('/HoSo/DeleteHoSo', { id: $scope.deleteItemId })
            .then(function (response) {
                if (response.data.success) {
                    $scope.loadHoSo(); // hoặc $scope.search() nếu có
                    AlertService.show('success', 'Xóa thành công!');
                } else {
                    AlertService.show('danger', response.data.message || 'Xóa thất bại!');
                }
            }, function () {
                AlertService.show('danger', 'Đã có lỗi xảy ra!');
            });
    };


    $scope.showDetailModal = function (item) {
        $scope.detailHoSo = angular.copy(item);
        $http.get('/HoSo/GetQuaTrinhXuLyByHoSoId', { params: { hoSoId: item.Id } })
            .then(function (res) {
                // Chuyển đổi ngày cho từng bản ghi
                res.data.forEach(function (qt) {
                    if (qt.NgayThucHien) {
                        qt.NgayThucHien = parseDotNetDate(qt.NgayThucHien);
                    }
                });
                $scope.detailQuaTrinh = groupQuaTrinhXuLy(res.data);
                $('#hoSoDetailModal').modal('show');
            });
    };

    function groupQuaTrinhXuLy(list) {
        var grouped = [];
        list.forEach(function (item) {
            // Tìm xem đã có dòng nào cùng bước, ghi chú, thời gian, người thực hiện chưa
            var key = item.Buoc + '|' + item.GhiChu + '|' + item.NgayThucHien + '|' + item.IdNguoiThucHien;
            var found = grouped.find(function (g) {
                return g._key === key;
            });
            if (found) {
                // Nếu đã có, thêm file vào mảng
                if (item.FileDinhKem) {
                    found.Files.push(item.FileDinhKem);
                }
            } else {
                // Nếu chưa có, tạo mới
                grouped.push({
                    Buoc: item.Buoc,
                    GhiChu: item.GhiChu,
                    NgayThucHien: item.NgayThucHien,
                    IdNguoiThucHien: item.IdNguoiThucHien,
                    TenNguoiThucHien: item.TenNguoiThucHien,
                    Files: item.FileDinhKem ? [item.FileDinhKem] : [],
                    _key: key // dùng để so sánh, không hiển thị ra view
                });
            }
        });
        return grouped;
    }


    $scope.getFileIcon = function (filePath) {
        if (!filePath) return 'fa-solid fa-file';
        var ext = filePath.split('.').pop().toLowerCase();
        switch (ext) {
            case 'pdf': return 'fa-solid fa-file-pdf text-danger';
            case 'doc':
            case 'docx': return 'fa-solid fa-file-word text-primary';
            case 'xls':
            case 'xlsx': return 'fa-solid fa-file-excel text-success';
            case 'ppt':
            case 'pptx': return 'fa-solid fa-file-powerpoint text-warning';
            default: return 'fa-solid fa-file';
        }
    };

    $scope.getFileName = function (filePath) {
        if (!filePath) return '';
        var parts = filePath.split('/');
        return parts[parts.length - 1];
    };

    function formatDate(date) {
        var d = new Date(date);
        if (isNaN(d.getTime())) return "";
        var month = '' + (d.getMonth() + 1);
        var day = '' + d.getDate();
        var year = d.getFullYear();
        if (month.length < 2) month = '0' + month;
        if (day.length < 2) day = '0' + day;
        return [year, month, day].join('-');
    }

    function parseDotNetDate(dateStr) {
        var match = /\/Date\((\d+)\)\//.exec(dateStr);
        if (match) {
            var ms = parseInt(match[1], 10);
            return new Date(ms);
        }
        return dateStr;
    }

    $scope.onFileChange = function (files) {
        for (var i = 0; i < files.length; i++) {
            var exists = $scope.selectedFiles.some(function (f) { return f.name === files[i].name && f.size === files[i].size; });
            if (!exists) {
                $scope.selectedFiles.push(files[i]);
            }
        }
        $scope.$applyAsync();
    };

    $scope.loadHoSo();
});
