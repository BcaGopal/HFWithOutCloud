JobConsumptionAdjustment = angular.module('JobConsumptionAdjustment', ['ngTouch', 'ui.grid', 'ui.grid.resizeColumns', 'ui.grid.moveColumns',
    'ui.grid.exporter', 'ui.grid.cellNav', 'ui.grid.pinning', 'ui.grid.edit', 'ui.grid.validate'])


JobConsumptionAdjustment.controller('MainCtrl', ['$scope', '$window', '$log', '$http', 'uiGridConstants', 'uiGridExporterConstants', 'uiGridExporterService', '$timeout',

  function ($scope, $window, $log, $http, uiGridConstants, uiGridExporterConstants, uiGridExporterService, uiGridTreeViewConstants, $timeout) {
      // $scope.msg = false;
      var PersonId = 0;
      $scope.gridOptions = {
          enableGridMenu: true,
          enableFiltering: false,
          enableGridMenu: true,
          enableFullRowSelection: false,
          enableRowSelection: true,
          enableCellEditOnFocus: true,
          enableGridMenu: false,
          enableColumnMenus: false,
          enableCellEdit: false,
          exporterMenuCsv: false,
          onRegisterApi: function (gridApi) {
              $scope.gridApi = gridApi;
              gridApi.edit.on.afterCellEdit($scope, function (rowEntity, colDef, newValue, oldValue) {
                  var Bal = parseInt(rowEntity.BalQty);
                  var CnQty = parseInt(rowEntity.ConsumeQty);
                  if (Bal < CnQty) {
                      rowEntity.ConsumeQty = 0;
                  }

              });
        }
      };

      $scope.Post = function () {
          $.ajax({
              cache: false,
              url: '/JobConsumptionAdsustment/Posted/',
              type: 'Post',
              data: $("#registerSubmit").serialize(),
              datatype: 'json',
              success: function (result) {
                  lock = false;
                  if (result.Success == true) {
                      $scope.gridOptions.columnDefs = new Array();
                      $scope.gridOptions.columnDefs.push({ field: 'ProductId', width: 50, visible: false });
                      $scope.gridOptions.columnDefs.push({ field: 'StockProcessId', width: 50, visible: false });
                      $scope.gridOptions.columnDefs.push({ field: 'ProcessId', width: 50, visible: false });
                      $scope.gridOptions.columnDefs.push({ field: 'PersonId', width: 50, visible: false });
                      $scope.gridOptions.columnDefs.push({ field: 'Dimension1Id', width: 50, visible: false });
                      $scope.gridOptions.columnDefs.push({ field: 'Dimension2Id', width: 50, visible: false });
                      $scope.gridOptions.columnDefs.push({ field: 'Dimension3Id', width: 50, visible: false });
                      $scope.gridOptions.columnDefs.push({ field: 'Dimension4Id', width: 50, visible: false });
                      $scope.gridOptions.columnDefs.push({ field: 'DocNo', width: 50, visible: false });
                      $scope.gridOptions.columnDefs.push({ field: 'ProcessName', width: 120, cellClass: 'cell-text', headerCellClass: 'header-text', });
                      $scope.gridOptions.columnDefs.push({ field: 'JobWorker', width: 200, cellClass: 'cell-text ', headerCellClass: 'header-text', });
                      $scope.gridOptions.columnDefs.push({ field: 'ProductName', width: 150, cellClass: 'cell-text ', headerCellClass: 'header-text', });
                      $scope.gridOptions.columnDefs.push({ field: 'Dimension1Name', name: 'Size', width: 80, visible: true, cellClass: 'cell-text', headerCellClass: 'header-text', });
                      $scope.gridOptions.columnDefs.push({ field: 'Dimension2Name', name: 'Style', width: 90, visible: true, cellClass: 'cell-text', headerCellClass: 'header-text', });
                      $scope.gridOptions.columnDefs.push({ field: 'Dimension3Name', name: 'Shade', width: 90, visible: true, cellClass: 'cell-text', headerCellClass: 'header-text', });
                      $scope.gridOptions.columnDefs.push({ field: 'Dimension4Name', name: 'Fabric', width: 90, visible: true, cellClass: 'cell-text', headerCellClass: 'header-text', });
                      $scope.gridOptions.columnDefs.push({ field: 'CostCenter', name: 'CostCenter', width: 90, visible: true, cellClass: 'cell-text', headerCellClass: 'header-text', });
                      $scope.gridOptions.columnDefs.push({ name: 'Balance Qty', field: 'BalQty', type: 'number', width: 80, visible: true, cellClass: 'cell-text', headerCellClass: 'header-text', enableCellEdit: false, enableCellEditOnFocus: true, validators: { required: true, datatype: 'int' }, });
                      $scope.gridOptions.columnDefs.push({ name: 'Consume Qty', field: 'ConsumeQty', type: 'number', width: 80, visible: true, cellClass: 'cell-text-c', headerCellClass: 'header-text', enableCellEdit: true, enableCellEditOnFocus: true, validators: { required: true, datatype: 'int' }, cellTemplate: 'ui-grid/cellTitleValidator' });
                      $scope.gridOptions.columnDefs.push({ field: 'UnitName', width: 70, visible: true, cellClass: 'cell-text', headerCellClass: 'header-text', enableCellEdit: false });
                      $scope.gridOptions.data = result.Data;
                      $scope.gridApi.grid.refresh();
                  }
                  else if (!result.Success) {
                      alert('Something went wrong');
                  }

              },
              error: function (xhr, ajaxOptions, thrownError) {
                  alert('Failed to retrive calculation footer' + thrownError);
              },
          });
      };

      
      $scope.SaveRecord = function () {
          var i = 0;
          var PostedViewModel = [];
          $scope.gridApi.core.getVisibleRows($scope.gridApi.grid).some(function (rowItem) {
              var mySelectedRow = jQuery.extend({}, rowItem.entity);
              PostedViewModel[i] = mySelectedRow;
              //PostedViewModel[i].DocTypeId = $('#DocTypeId').val();
              //PostedViewModel[i].EntryNo = $('#EntryNo').val();
              //PostedViewModel[i].EntryDate = $('#EntryDate').val();
              //PostedViewModel[i].Remark = $('#Remark').val();
              PostedViewModel[i].DocTypeId = $('#DocTypeId').val();
              PostedViewModel[i].EntryNo = $('#EntryNo').val();
              PostedViewModel[i].EntryDate = $('#EntryDate').val();
              PostedViewModel[i].Remark = $('#Remark').val();
              
              i++;
          });

          PostedViewModel = JSON.stringify({
              'PostedViewModel': PostedViewModel
          });

          $.ajax({
              url: '/JobConsumptionAdsustment/SaveRecord/' + $(this).serialize(),
              contentType: 'application/json',
              type: 'Post',
              data: PostedViewModel,
              datatype: 'json',
              success: function (result) {
                  if (result.success == true) {
                      return;
                  }
                  else {
                  }
              },
              error: function (xhr, ajaxOptions, thrownError) {
                  return;
              },
          });
      }
      $scope.closeAlert = function (index) {
          $scope.alerts.splice(index, 1);
      };
  }
]);

JobConsumptionAdjustment.directive('ngBlur', function () {
    return function (scope, elem, attrs) {
        elem.bind('blur', function () {
            scope.$apply(attrs.ngBlur);
        });
    };
})

JobConsumptionAdjustment.filter('trusted', function ($sce) {
    return function (value) {
        return $sce.trustAsHtml(value);
    }
});



