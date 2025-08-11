app.controller('HoSoJs', function ($scope, $http, AlertService) {
    $scope.hoSo = {};
    $scope.hoSo.NgayTiepNhan = new Date().toISOString().slice(0, 10);
    $scope.selectedFiles = [];

    // Danh sách động cho các select
    $scope.listPhongBan = [];
    $scope.listLinhVuc = [];
    $scope.listLoaiHoSo = [];

    // Lấy danh sách phòng ban
    $scope.loadPhongBan = function () {
        $http.get('/PhongBan/GetList').then(function (res) {
            $scope.listPhongBan = res.data;
        });
    };

    // Lấy danh sách lĩnh vực theo phòng ban
    $scope.onPhongBanChange = function () {
        $scope.listLinhVuc = [];
        $scope.listLoaiHoSo = [];
        $scope.hoSo.IdLinhVuc = null;
        $scope.hoSo.IdLoaiHoSo = null;
        if ($scope.hoSo.IdPhongBan) {
            $http.get('/LinhVuc/GetLinhVucByPhongBan', { params: { idPhongBan: $scope.hoSo.IdPhongBan } })
                .then(function (res) {
                    $scope.listLinhVuc = res.data;
                });
        }
    };

    // Lấy danh sách loại hồ sơ theo lĩnh vực
    $scope.onLinhVucChange = function () {
        $scope.listLoaiHoSo = [];
        $scope.hoSo.IdLoaiHoSo = null;
        if ($scope.hoSo.IdLinhVuc) {
            $http.get('/LoaiHoSo/GetLoaiHoSoByLinhVuc', { params: { idLinhVuc: $scope.hoSo.IdLinhVuc } })
                .then(function (res) {
                    $scope.listLoaiHoSo = res.data;
                });
        }
    };

    // Nộp hồ sơ
    $scope.nopHoSo = function () {
        var form = document.forms['nopHoSoForm'];
        if (!form.checkValidity()) {
            form.classList.add('was-validated');
            return;
        }

        // Chuyển đổi các trường ngày về yyyy-MM-dd
        if ($scope.hoSo.NgayTiepNhan) {
            $scope.hoSo.NgayTiepNhan = formatDate($scope.hoSo.NgayTiepNhan);
        }
        if ($scope.hoSo.HanXuLy) {
            $scope.hoSo.HanXuLy = formatDate($scope.hoSo.HanXuLy);
        }

        var formData = new FormData();
        for (var key in $scope.hoSo) {
            if ($scope.hoSo.hasOwnProperty(key)) {
                formData.append(key, $scope.hoSo[key]);
            }
        }
        for (var i = 0; i < $scope.selectedFiles.length; i++) {
            formData.append('FileDinhKem', $scope.selectedFiles[i]);
        }
        $http.post('/HoSo/AddHoSo', formData, {
            headers: { 'Content-Type': undefined },
            transformRequest: angular.identity
        }).then(function (res) {
            if (res.data.success) {
                AlertService && AlertService.show('success', 'Nộp hồ sơ thành công!');
                $scope.hoSo = {};
                $scope.hoSo.NgayTiepNhan = new Date().toISOString().slice(0, 10);
                $scope.selectedFiles = [];
                form.reset();
            } else {
                AlertService && AlertService.show('danger', 'Nộp hồ sơ thất bại!');
            }
        }, function () {
            AlertService && AlertService.show('danger', 'Lỗi máy chủ!');
        });
    };

    // Xử lý chọn file upload
    $scope.onFileChange = function (files) {
        for (var i = 0; i < files.length; i++) {
            var exists = $scope.selectedFiles.some(function (f) { return f.name === files[i].name && f.size === files[i].size; });
            if (!exists) {
                $scope.selectedFiles.push(files[i]);
            }
        }
        $scope.$applyAsync();
    };

    // Xóa file khỏi danh sách upload
    $scope.removeFile = function (index) {
        $scope.selectedFiles.splice(index, 1);
        if ($scope.selectedFiles.length === 0) {
            document.getElementById('fileDinhKem').value = '';
        }
    };

    // Định dạng ngày yyyy-MM-dd
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

    // Chuyển tên công dân sang chữ hoa
    $scope.capitalizeName = function () {
        if ($scope.hoSo.TenCongDan) {
            $scope.hoSo.TenCongDan = $scope.hoSo.TenCongDan.toUpperCase();
        }
    };

    // Khởi tạo dữ liệu động
    $scope.loadPhongBan();
});
