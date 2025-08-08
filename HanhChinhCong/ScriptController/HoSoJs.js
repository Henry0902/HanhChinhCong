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

    $scope.searchTenCongDan = '';
    $scope.searchCMND_CCCD = '';

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
                searchTenCongDan: $scope.searchTenCongDan,
                searchCMND_CCCD: $scope.searchCMND_CCCD,
                page: $scope.page,
                pageSize: $scope.pageSize
            }
        }).then(function (res) {
            // Chuyển đổi ngày cho từng item
            res.data.data.forEach(function (item) {
                // Chuyển đổi từ /Date(xxx)/ về yyyy-MM-dd
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
            console.log("list data", $scope.listData);
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
    $scope.showEditModal = function (item) {
        // Tạo bản sao để tránh sửa trực tiếp trên listData
        $scope.editingHoSo = angular.copy(item);

        // Chuyển chuỗi yyyy-MM-dd về kiểu Date cho input type="date"
        if ($scope.editingHoSo.NgayTiepNhan) {
            $scope.editingHoSo.NgayTiepNhan = new Date($scope.editingHoSo.NgayTiepNhan);
        }
        if ($scope.editingHoSo.HanXuLy) {
            $scope.editingHoSo.HanXuLy = new Date($scope.editingHoSo.HanXuLy);
        }

        $scope.selectedFiles = [];
        $scope.attachedFiles = [];

        // Lấy file đã đính kèm từ server
        $http.get('/HoSo/GetFilesByHoSoId', { params: { hoSoId: item.Id } })
            .then(function (res) {
                $scope.attachedFiles = res.data;
            });
        // Hiển thị modal bằng Bootstrap JS
        $('#hoSoModal').modal('show');
    };

    // Xóa file đã đính kèm (chỉ đánh dấu, không xóa ngay trên server)
    $scope.removeAttachedFile = function (file, index) {
        if (!$scope.filesToDelete) $scope.filesToDelete = [];
        $scope.filesToDelete.push(file); // Đánh dấu file sẽ xóa khi lưu
        $scope.attachedFiles.splice(index, 1);
    };

    // Xóa file mới chọn
    $scope.removeFile = function (index) {
        $scope.selectedFiles.splice(index, 1);
        if ($scope.selectedFiles.length === 0) {
            document.getElementById('fileDinhKem').value = '';
        }
    };

    // Khi lưu sửa hồ sơ
    $scope.editHoSo = function (hoSo) {
        //console.log("hgoso", hoSo);
        // Chuyển đổi các trường ngày về yyyy-MM-dd
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
        // Thêm file mới
        for (var i = 0; i < $scope.selectedFiles.length; i++) {
            formData.append('FileDinhKem', $scope.selectedFiles[i]);
        }
        // Thêm danh sách file cần xóa
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

    function parseDotNetDate(dateStr) {
        // Kiểm tra có đúng định dạng /Date(xxx)/
        var match = /\/Date\((\d+)\)\//.exec(dateStr);
        if (match) {
            var ms = parseInt(match[1], 10);
            return new Date(ms);
        }
        return dateStr; // Nếu không đúng định dạng, trả về nguyên bản
    }



});
