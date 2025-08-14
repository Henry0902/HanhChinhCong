app.controller('XuLyHoSoJs', function ($scope, $http, AlertService) {
    $scope.listData = [];
    $scope.searchName = '';
    $scope.searchTenCongDan = '';
    $scope.searchCMND_CCCD = '';
    $scope.searchMaHoSo = '';
    $scope.page = 1;
    $scope.pageSize = 5;
    $scope.totalRows = 0;
    $scope.totalPages = 1;

    $scope.detailHoSo = {};
    $scope.detailQuaTrinh = [];

    $scope.yeuCauSuaDoiBoSungItem = null;
    $scope.yeuCauSuaDoiBoSungGhiChu = '';

    // Khi load lần đầu
    $scope.searchSapHetHan = null;

    // Danh sách file xử lý
    $scope.selectedFilesXuLy = [];

    // Hiển thị modal xử lý hồ sơ
    $scope.showXuLyModal = function (item) {
        $scope.editingHoSo = angular.copy(item);
        $scope.editingHoSo.GhiChuXuLy = '';
        $scope.selectedFilesXuLy = [];
        $('#xuLyModal').modal('show');
    };

    // yêu cầu sửa đổi bổ sung
    $scope.showYeuCauSuaDoiBoSungModal = function (item) {
        $scope.yeuCauSuaDoiBoSungItem = item;
        $scope.yeuCauSuaDoiBoSungGhiChu = '';
        $('#xacNhanSuaDoiBoSungModal').modal('show');
    };

    $scope.xacNhanYeuCauSuaDoiBoSung = function () {
        if (!$scope.yeuCauSuaDoiBoSungGhiChu) {
            AlertService && AlertService.show('danger', 'Vui lòng điền ghi chú!');
            return;
        }
        $http.post('/HoSo/YeuCauSuaDoiBoSung', {
            hoSoId: $scope.yeuCauSuaDoiBoSungItem.Id,
            ghiChu: $scope.yeuCauSuaDoiBoSungGhiChu
        }).then(function (res) {
            if (res.data.success) {
                AlertService && AlertService.show('success', 'Phân công hồ sơ thành công!');
                $('#xacNhanSuaDoiBoSungModal').modal('hide');
                $scope.loadData();
                // Reset input file
                var fileInput = document.getElementById('fileTraKetQua');
                if (fileInput) fileInput.value = '';
            } else {
                alert('Có lỗi xảy ra: ' + res.data.message);
            }
        });
    };


    // Xử lý khi chọn file
    $scope.onFileXuLyChange = function (files) {
        for (var i = 0; i < files.length; i++) {
            var exists = $scope.selectedFilesXuLy.some(function (f) { return f.name === files[i].name && f.size === files[i].size; });
            if (!exists) {
                $scope.selectedFilesXuLy.push(files[i]);
            }
        }
        $scope.$applyAsync();
    };

    // Xóa file khỏi danh sách
    $scope.removeFileXuLy = function (index) {
        $scope.selectedFilesXuLy.splice(index, 1);
        if ($scope.selectedFilesXuLy.length === 0) {
            document.getElementById('fileXuLy').value = '';
        }
    };

    // Gửi xác nhận xử lý hồ sơ lên server
    $scope.xacNhanXuLyHoSo = function () {
        var formData = new FormData();
        formData.append('hoSoId', $scope.editingHoSo.Id);
        formData.append('ghiChuXuLy', $scope.editingHoSo.GhiChuXuLy);

        // Thêm file xử lý
        if ($scope.selectedFilesXuLy && $scope.selectedFilesXuLy.length > 0) {
            for (var i = 0; i < $scope.selectedFilesXuLy.length; i++) {
                formData.append('FileXuLy', $scope.selectedFilesXuLy[i]);
            }
        }

        $http.post('/HoSo/XacNhanXuLyHoSo', formData, {
            headers: { 'Content-Type': undefined },
            transformRequest: angular.identity
        }).then(function (res) {
            if (res.data.success) {
                $('#xuLyModal').modal('hide');
                $scope.loadHoSo();
                // Reset input file
                var fileInput = document.getElementById('fileTraKetQua');
                if (fileInput) fileInput.value = '';
                AlertService && AlertService.show('success', 'Xử lý hồ sơ thành công!');
            } else {
                AlertService && AlertService.show('danger', 'Xử lý hồ sơ thất bại!');
            }
        });
    };

    // Phân trang và tìm kiếm chỉ lấy hồ sơ đã phân công cho cán bộ xử lý hiện tại
    $scope.loadHoSo = function () {
        $http.get('/HoSo/GetHoSoPhanCongXuLy', {
            params: {
                searchMaHoSo: $scope.searchMaHoSo,
                searchName: $scope.searchName,
                searchTenCongDan: $scope.searchTenCongDan,
                searchCMND_CCCD: $scope.searchCMND_CCCD,
                searchSapHetHan: $scope.searchSapHetHan,
                searchIdTrangThai: 2, // hoặc để rỗng nếu muốn lấy tất cả trạng thái đã phân công
                page: $scope.page,
                pageSize: $scope.pageSize
            }
        }).then(function (res) {
            var dataArr = [];
            if (res.data && Array.isArray(res.data.data)) {
                dataArr = res.data.data;
                $scope.totalRows = res.data.totalRows || dataArr.length;
            } else if (Array.isArray(res.data)) {
                dataArr = res.data;
                $scope.totalRows = dataArr.length;
            } else {
                dataArr = [];
                $scope.totalRows = 0;
            }

            // Xử lý ngày và đánh dấu sắp hết hạn cho từng item
            dataArr.forEach(function (item) {
                // Chuyển đổi ngày tiếp nhận
                if (item.NgayTiepNhan) {
                    var d = parseDotNetDate(item.NgayTiepNhan);
                    item.NgayTiepNhan = (d instanceof Date && !isNaN(d.getTime())) ? formatDate(d) : "";
                }
                // Chuyển đổi hạn xử lý và tính sắp hết hạn
                if (item.HanXuLy) {
                    var d = parseDotNetDate(item.HanXuLy);
                    item.HanXuLy = (d instanceof Date && !isNaN(d.getTime())) ? formatDate(d) : "";
                    var hanXuLyDate = new Date(item.HanXuLy);
                    var now = new Date();
                    var hanXuLyDateOnly = new Date(hanXuLyDate.getFullYear(), hanXuLyDate.getMonth(), hanXuLyDate.getDate());
                    var nowDateOnly = new Date(now.getFullYear(), now.getMonth(), now.getDate());
                    var diffDaysOnly = (hanXuLyDateOnly - nowDateOnly) / (1000 * 60 * 60 * 24);
                    item.SapHetHan = (
                        (diffDaysOnly === 0 || (diffDaysOnly < 1 && diffDaysOnly > 0) || diffDaysOnly === 1)
                        && diffDaysOnly >= 0
                        && item.IdTrangThai != 5
                    );
                } else {
                    item.SapHetHan = false;
                }
            });

            $scope.listData = dataArr;
            $scope.totalPages = Math.ceil($scope.totalRows / $scope.pageSize);
        });
    };



    $scope.showDetailModal = function (item) {
        $scope.detailHoSo = angular.copy(item);
        $scope.detailFiles = [];
        $http.get('/HoSo/GetFilesByHoSoId', { params: { hoSoId: item.Id } })
            .then(function (res) {
                $scope.detailFiles = res.data;
            });
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

    $scope.getFileName = function (filePath) {
        if (!filePath) return '';
        // Lấy phần sau cùng của đường dẫn
        var parts = filePath.split('/');
        return parts[parts.length - 1];
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

    // Định dạng ngày
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

    // Khởi tạo
    $scope.loadHoSo();
});
