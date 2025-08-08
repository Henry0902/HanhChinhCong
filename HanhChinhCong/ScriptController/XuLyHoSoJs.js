app.controller('XuLyHoSoJs', function ($scope, $http, AlertService) {
    $scope.listData = [];
    $scope.searchName = '';
    $scope.searchTenCongDan = '';
    $scope.searchCMND_CCCD = '';
    $scope.page = 1;
    $scope.pageSize = 5;
    $scope.totalRows = 0;
    $scope.totalPages = 1;

    // Danh sách file xử lý
    $scope.selectedFilesXuLy = [];

    // Hiển thị modal xử lý hồ sơ
    $scope.showXuLyModal = function (item) {
        $scope.editingHoSo = angular.copy(item);
        $scope.editingHoSo.GhiChuXuLy = '';
        $scope.selectedFilesXuLy = [];
        $('#xuLyModal').modal('show');
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
                AlertService && AlertService.show('success', 'Xử lý hồ sơ thành công!');
            } else {
                AlertService && AlertService.show('danger', 'Xử lý hồ sơ thất bại!');
            }
        });
    };

    // Phân trang và tìm kiếm
    $scope.loadHoSo = function () {
        $http.get('/HoSo/GetPagedHoSo', {
            params: {
                searchName: $scope.searchName,
                searchTenCongDan: $scope.searchTenCongDan,
                searchCMND_CCCD: $scope.searchCMND_CCCD,
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
