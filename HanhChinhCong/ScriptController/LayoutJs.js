app.controller('LayoutJs', function ($scope, $http) {
    $scope.currentHoTen = window.currentHoTen;
    $scope.currentUsername = window.currentUsername;
    $scope.currentUserRoles = window.currentUserRoles;
    $scope.allRoles = window.allRoles;

    $scope.userRoles = [];
    var userRolesStr = sessionStorage.getItem('userRoles');
    if (userRolesStr && userRolesStr !== 'undefined') {
        $scope.userRoles = JSON.parse(userRolesStr);
    }


    $scope.getRoleNames = function () {
        if (!$scope.currentUserRoles || !$scope.allRoles) return '';
        // Nếu currentUserRoles là số, chuyển thành mảng
        var roleIds = Array.isArray($scope.currentUserRoles) ? $scope.currentUserRoles : [$scope.currentUserRoles];
        var names = $scope.allRoles
            .filter(function (role) { return roleIds.indexOf(role.Id) !== -1; })
            .map(function (role) { return role.Name; });
        return names.join(', ');
    };

    
    $scope.hasRole = function (roleId) {
        //debugger;
        var roles = window.currentUserRoles || [];
        for (var i = 0; i < arguments.length; i++) {
            if (roles.indexOf(arguments[i]) !== -1) return true;
        }
        return false;
    };
    

    $scope.logout = function () {
        sessionStorage.removeItem('userRole');
        $http.get('/Login/Logout').then(function () {
            window.location.href = '/Login/Index';
        });
    };
});
