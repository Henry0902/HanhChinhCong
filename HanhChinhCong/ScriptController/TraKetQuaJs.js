app.controller('TraKetQuaJs', function ($scope, $http, AlertService) {
    $scope.listData = [];
    $scope.searchName = '';
    $scope.searchTenCongDan = '';
    $scope.searchCMND_CCCD = '';
    $scope.page = 1;
    $scope.pageSize = 5;
    $scope.totalRows = 0;
    $scope.totalPages = 1;

    $scope.selectedFilesTraKetQua = [];

    $scope.detailHoSo = {};
    $scope.detailQuaTrinh = [];

    $scope.showTraKetQuaModal = function (item) {
        $scope.editingHoSo = angular.copy(item);
        $scope.editingHoSo.GhiChuTraKetQua = '';
        $scope.selectedFilesTraKetQua = [];
        $('#traKetQuaModal').modal('show');
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

    $scope.getFileName = function (filePath) {
        if (!filePath) return '';
        var parts = filePath.split('/');
        return parts[parts.length - 1];
    };

    $scope.onFileTraKetQuaChange = function (files) {
        for (var i = 0; i < files.length; i++) {
            var exists = $scope.selectedFilesTraKetQua.some(function (f) { return f.name === files[i].name && f.size === files[i].size; });
            if (!exists) {
                $scope.selectedFilesTraKetQua.push(files[i]);
            }
        }
        $scope.$applyAsync();
    };

    $scope.removeFileTraKetQua = function (index) {
        $scope.selectedFilesTraKetQua.splice(index, 1);
        if ($scope.selectedFilesTraKetQua.length === 0) {
            document.getElementById('fileTraKetQua').value = '';
        }
    };

    $scope.traKetQuaHoSo = function () {
        var formData = new FormData();
        formData.append('hoSoId', $scope.editingHoSo.Id);
        formData.append('ghiChuTraKetQua', $scope.editingHoSo.GhiChuTraKetQua);

        if ($scope.selectedFilesTraKetQua && $scope.selectedFilesTraKetQua.length > 0) {
            for (var i = 0; i < $scope.selectedFilesTraKetQua.length; i++) {
                formData.append('FileTraKetQua', $scope.selectedFilesTraKetQua[i]);
            }
        }

        $http.post('/HoSo/TraKetQuaHoSo', formData, {
            headers: { 'Content-Type': undefined },
            transformRequest: angular.identity
        }).then(function (res) {
            if (res.data.success) {
                $('#traKetQuaModal').modal('hide');
                $scope.loadHoSo();
                AlertService && AlertService.show('success', 'Trả kết quả thành công!');
            } else {
                AlertService && AlertService.show('danger', 'Trả kết quả thất bại!');
            }
        });
    };

    $scope.loadHoSo = function () {
        $http.get('/HoSo/GetPagedHoSo', {
            params: {
                searchName: $scope.searchName,
                searchTenCongDan: $scope.searchTenCongDan,
                searchCMND_CCCD: $scope.searchCMND_CCCD,
                searchIdTrangThai: 4,
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

    $scope.getFileIcon = function (fileName) {
        var ext = fileName.split('.').pop().toLowerCase();
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

    $scope.loadHoSo();
});
