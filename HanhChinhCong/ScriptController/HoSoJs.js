app.controller('HoSoJs', function ($scope, $http, AlertService) {
    $scope.listData = [];
    $scope.searchName = '';
    $scope.page = 1;
    $scope.pageSize = 5;
    $scope.totalRows = 0;
    $scope.totalPages = 1;

    $scope.listLinhVuc = [];
    $scope.listLoaiHoSo = [];
    $scope.listPhongBan = [];
    $scope.hoSo = {};
    $scope.hoSo.NgayTiepNhan = new Date().toISOString().slice(0, 10);

    //lấy danh sách phòng ban
    $scope.loadPhongBan = function () {
        $http.get('/PhongBan/GetList').then(function (res) {
            $scope.listPhongBan = res.data;
        });
    };

    //lấy danh sách linh vực theo phong ban
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

    //getLoaiHoSo by LinhVuc
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

    // Khởi tạo
    $scope.loadPhongBan();


    //Nộp hồ sơ
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
        // Nếu có trường ngày khác, xử lý tương tự

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

    function formatDate(date) {
        // Nếu là chuỗi, chuyển sang Date
        var d = new Date(date);
        if (isNaN(d.getTime())) return "";
        var month = '' + (d.getMonth() + 1);
        var day = '' + d.getDate();
        var year = d.getFullYear();
        if (month.length < 2) month = '0' + month;
        if (day.length < 2) day = '0' + day;
        return [year, month, day].join('-');
    }


    //phân trang
    $scope.loadHoSo = function () {
        $http.get('/HoSo/GetPagedHoSo', {
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
        $scope.loadHoSo();
    };

    $scope.goToPage = function (p) {
        if (p >= 1 && p <= $scope.totalPages) {
            $scope.page = p;
            $scope.loadHoSo();
        }
    };

    // sửa
    $scope.editHoSo = function (hoSo) {
        $http.post('/HoSo/EditHoSo', hoSo)
            .then(function (res) {
                if (res.data.success) {
                    $scope.loadHoSo();
                    AlertService && AlertService.show('success', 'Sửa thành công!');
                } else {
                    AlertService && AlertService.show('danger', 'Sửa thất bại!');
                }
            });
    };

    //xóa
    $scope.deleteHoSo = function (id) {
        $http.post('/HoSo/DeleteHoSo', { id: id })
            .then(function (res) {
                if (res.data.success) {
                    $scope.loadHoSo();
                    AlertService && AlertService.show('success', 'Xóa thành công!');
                } else {
                    AlertService && AlertService.show('danger', 'Xóa thất bại!');
                }
            });
    };

    // Initial load
    $scope.loadHoSo();

    //upload file
    $scope.selectedFiles = [];
    $scope.onFileChange = function (files) {
        // Thêm các file mới vào danh sách, không xóa file cũ
        for (var i = 0; i < files.length; i++) {
            // Kiểm tra trùng tên file (nếu muốn)
            var exists = $scope.selectedFiles.some(function (f) { return f.name === files[i].name && f.size === files[i].size; });
            if (!exists) {
                $scope.selectedFiles.push(files[i]);
            }
        }
        $scope.$applyAsync();
    };

    // Xóa file khỏi danh sách
    $scope.removeFile = function (index) {
        $scope.selectedFiles.splice(index, 1);
        // Reset input file nếu không còn file nào
        if ($scope.selectedFiles.length === 0) {
            document.getElementById('fileDinhKem').value = '';
        }
    };

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

    $scope.capitalizeName = function () {
        if ($scope.hoSo.TenCongDan) {
            $scope.hoSo.TenCongDan = $scope.hoSo.TenCongDan.toUpperCase();
        }
    };



});
