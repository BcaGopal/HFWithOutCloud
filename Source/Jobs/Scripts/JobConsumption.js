MaterialBalanceJobWorker = angular.module('MaterialBalanceJobWorker', ['ngTouch', 'ui.grid', 'ui.grid.resizeColumns', 'ui.grid.moveColumns',
    'ui.grid.exporter', 'ui.grid.cellNav', 'ui.grid.pinning', 'ui.grid.selection'])


MaterialBalanceJobWorker.controller('MainCtrl', ['$scope', '$log', '$http', '$httpParamSerializer', '$httpParamSerializerJQLike', 'uiGridConstants', 'uiGridExporterConstants', 'uiGridExporterService', '$timeout',

  function ($scope, $log, $http,$httpParamSerializer,$httpParamSerializerJQLike, uiGridConstants, uiGridExporterConstants, uiGridExporterService, uiGridTreeViewConstants, $timeout) {
     // $scope.IsVisible = false;
    $scope.gridOptions = {
      enableGridMenu: true,
      enableSelectAll: true,
      enableFiltering: false,
      showColumnFooter: true,
      enableGridMenu: true,
      fastwatch: true,
      enableFullRowSelection: false,
      enableRowSelection: true,
      exporterMenuPdf: false,
      gridMenuShowHideColumns: true,
      exporterPdfOrientation: 'portrait',
      exporterPdfPageSize: 'LETTER',
      exporterPdfMaxGridWidth: 500,
      exporterCsvLinkElement: angular.element(document.querySelectorAll(".custom-csv-link-location")),
      exporterCsvFilename: $("#ReportType").val() + '.csv',
      exporterMenuCsv: false,
      gridMenuCustomItems: [{
          title: 'Export Data As CSV',
          order: 100,
          action: function ($event) {
              $scope.gridApi.exporter.csvExport('visible', 'visible');
          }
      },
      {
          title: 'Export Data As PDF',
          order: 100,
          action: function ($event) {
              $scope.export();
          }
      }],

      onRegisterApi: function (gridApi) {
        $scope.gridApi = gridApi;
            
    }



    };
  $scope.Post = function () {
      var PostedViewModel = [];
      //$scope.gridApi.core.getVisibleRows($scope.gridApi.grid).some(function (rowItem) {
      //    if($scope.gridApi.selection.selectRow){
      //          var mySelectedRow = jQuery.extend({}, rowItem.entity);
      //          PostedViewModel[i] = mySelectedRow;
      //          i++;
      //      }
      //});

      $scope.gridApi.core.getVisibleRows($scope.gridApi.grid).some(function (rowItem) {
          if (rowItem.isSelected) {
                var mySelectedRow = jQuery.extend({}, rowItem.entity);
                PostedViewModel[i] = mySelectedRow;
                i++;
              
            }
      });

       PostedViewModel = JSON.stringify({ 'PostedViewModel': PostedViewModel });
       $.ajax(  {
            url: '/MaterialBalanceUpdate/Post/' + $(this).serialize(),
            contentType: 'application/json',
            type: 'Post',
            data:PostedViewModel,
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

        var rowCol = $scope.gridApi.cellNav.getFocusedCell();
        //var DocType = rowCol.row.entity.DocType;
        var DocType = 137;
      //  var DocNo = rowCol.row.entity.DocNo;
      //if (rowCol.row.entity.DocType != null) {
        if (DocType != null) {
            window.open('/JobConsumptionAdsustment/JobConsumptionAdsustment/?DocTypeId=' + DocType,  '_self');
            return;
          }

      };

       $scope.export = function () {
          var i = 0;
          var columns = $scope.gridApi.grid.options.showHeader ? uiGridExporterService.getColumnHeaders($scope.gridApi.grid, 'visible') : [];
           var pagewidth = 0;
          $.each(columns, function () {
              pagewidth = pagewidth + columns[i]["width"]
              i = i + 1;
          });
          var PageOrientation = 'p';
          var PageSize = 'A4';
          var ColumnFontSize = 10;
          var Inch = (pagewidth * 1 / 100);

          if (Inch < 8) {
              ColumnFontSize = 10;
          }
          else if ((Inch * 90 / 100) < 8) {
              ColumnFontSize = 9
          }
          else if ((Inch * 80 / 100) < 8) {
              ColumnFontSize = 8
          }
          else {
              PageOrientation = 'l';
              if (Inch < 11) {
                  ColumnFontSize = 10;
              }
              else if ((Inch * 90 / 100) < 11) {
                  ColumnFontSize = 9
              }
              else if ((Inch * 80 / 100) < 11) {
                  ColumnFontSize = 8
              }
              else if ((Inch * 70 / 100) < 11) {
                  ColumnFontSize = 7
              }
              else if ((Inch * 60 / 100) < 11) {
                  ColumnFontSize = 7
              }
              else {
                  PageSize = 'legal';
                  if (Inch < 13.5) {
                      ColumnFontSize = 10;
                  }
                  else if ((Inch * 90 / 100) < 13.5) {
                      ColumnFontSize = 9
                  }
                  else if ((Inch * 80 / 100) < 13.5) {
                      ColumnFontSize = 8
                  }
                  else if ((Inch * 70 / 100) < 13.5) {
                      ColumnFontSize = 7
                  }
                  else {
                      ColumnFontSize = 7
                  }
              }
          }



          var Rows = [];
          $scope.gridApi.core.getVisibleRows($scope.gridApi.grid).some(function (rowItem) {
              Rows[i] = rowItem.entity;
              i++;
          });



          var pdfColumns = new Array();
          var pdfColumnsStyle = {};
          var pdfColumnsHeaderStyle = {};
          i = 0;
          $.each(columns, function () {
              pdfColumns.push({ title: columns[i]["displayName"], dataKey: columns[i]["name"] });
              pdfColumnsStyle[columns[i]["name"]] = { columnWidth: columns[i]["width"] * ((ColumnFontSize / 1000) * 25), fontSize: ColumnFontSize };
              i = i + 1;
          });



          // Only pt supported (not mm or in) 
          var doc = new jsPDF(PageOrientation, 'mm', PageSize);


          doc.autoTable(pdfColumns, Rows, {
              addPageContent: function (data) {
                  // HEADER
                  doc.setFontSize(10);
                  doc.setFontStyle('bold');

                  if ($("#ReportHeaderCompanyDetail_LogoBlob").val() != null && $("#ReportHeaderCompanyDetail_LogoBlob").val() != "") {
                      var imgData = 'data:image/jpeg;base64,' + $("#ReportHeaderCompanyDetail_LogoBlob").val();
                      doc.addImage(imgData, 'JPEG', data.settings.margin.left, 6, 20, 18);
                  }
                  doc.text($("#ReportHeaderCompanyDetail_CompanyName").val(), data.settings.margin.left + 22, 10)
                  doc.setFontSize(9);
                  doc.setFontStyle('normal');
                  doc.text($("#ReportHeaderCompanyDetail_Address").val(), data.settings.margin.left + 22, 14)
                  doc.text($("#ReportHeaderCompanyDetail_CityName").val(), data.settings.margin.left + 22, 18)
                  doc.text($("#ReportHeaderCompanyDetail_Phone").val(), data.settings.margin.left + 22, 22)
              },

              margin: { top: 28 },
              columnStyles: pdfColumnsStyle,
              styles: {
                  overflow: 'linebreak',
                  tableWidth: 'auto',
              },
              headerStyles: {
                  fontSize: ColumnFontSize,
              },
          });
          //doc.save('table.pdf');
          var string = doc.output('datauristring');
          var iframe = "<iframe width='100%' height='100%' src='" + string + "'></iframe>"
          var x = window.open();
          x.document.open();
          x.document.write(iframe);
          x.document.close();

      };


      var i = 0;
      $scope.BindData = function () {
          $scope.myData = [];
          $.ajax({
              url: '/MaterialBalanceUpdate/MaterialBalanceFill/' + $(this).serialize(),
              type: "POST",
              data: $("#registerSubmit").serialize(),
              success: function (result) {
                  Lock = false;
                  if (result.Success == true) {
                      $scope.gridOptions.columnDefs = new Array();
                      $scope.gridOptions.columnDefs.push({ field: 'ProductId', width: 50, visible: false });
                      $scope.gridOptions.columnDefs.push({ field: 'StockProcessId', width: 50, visible: false });
                      $scope.gridOptions.columnDefs.push({ field: 'ProcessId', width: 50, visible: false });
                      $scope.gridOptions.columnDefs.push({ field: 'StockHeaderId', width: 50, visible: false });
                    //  $scope.gridOptions.columnDefs.push({ field: 'CostCenterId', width: 50, visible: false });
                      $scope.gridOptions.columnDefs.push({ field: 'PersonId', width: 50, visible: false });
                      $scope.gridOptions.columnDefs.push({ field: 'Dimension1Id', width: 50, visible: false });
                      $scope.gridOptions.columnDefs.push({ field: 'Dimension2Id', width: 50, visible: false });
                      $scope.gridOptions.columnDefs.push({ field: 'Dimension3Id', width: 50, visible: false });
                      $scope.gridOptions.columnDefs.push({ field: 'Dimension4Id', width: 50, visible: false });
                   //   $scope.gridOptions.columnDefs.push({ field: 'DocType', width: 50, visible: false });
                    //  $scope.gridOptions.columnDefs.push({ field: 'DocNo', width: 50, visible: false });
                    //  $scope.gridOptions.columnDefs.push({ field: 'Remark', width: 50, visible: false });
                      $scope.gridOptions.columnDefs.push({ field: 'DocDate', width: 50, visible: false });
                      $scope.gridOptions.columnDefs.push({ field: 'ProcessName', width:120 ,cellClass: 'cell-text', headerCellClass: 'header-text', });
                      $scope.gridOptions.columnDefs.push({ field: 'JobWorker', width: 200, cellClass: 'cell-text ', headerCellClass: 'header-text',  });
                      $scope.gridOptions.columnDefs.push({ field: 'ProductName', width: 150, cellClass: 'cell-text ', headerCellClass: 'header-text', });
                      $scope.gridOptions.columnDefs.push({ field: 'Dimension1Name',name:'Size', width: 80, visible: true, cellClass: 'cell-text', headerCellClass: 'header-text',});
                      $scope.gridOptions.columnDefs.push({ field: 'Dimension2Name',name:'Style' ,width: 90, visible: true, cellClass: 'cell-text', headerCellClass: 'header-text',});
                      $scope.gridOptions.columnDefs.push({ field: 'Dimension3Name',name:'Shade', width: 90, visible: true, cellClass: 'cell-text', headerCellClass: 'header-text',  });
                      $scope.gridOptions.columnDefs.push({ field: 'Dimension4Name', name: 'Fabric', width: 90, visible: true, cellClass: 'cell-text', headerCellClass: 'header-text', });
                      $scope.gridOptions.columnDefs.push({ field: 'CostCenter', name: 'CostCenter', width: 100, visible: true, cellClass: 'cell-text', headerCellClass: 'header-text',});
                      $scope.gridOptions.columnDefs.push({ name:  'Balance Qty', field: 'BalQty', type: 'number', width: 100, visible: true, cellClass: 'cell-text', headerCellClass: 'header-text', enableCellEdit:true, enableCellEditOnFocus: true, validators: { required: true, datatype: 'int' }, });
                      $scope.gridOptions.columnDefs.push({ field: 'UnitName', width: 70, visible: true, cellClass: 'cell-text-c', headerCellClass: 'header-text', enableCellEdit: false });
                    //  $scope.gridOptions.columnDefs.push({ field: 'Action', width: 80, visible: true, cellClass: 'cell-text', headerCellClass: 'header-text', cellTemplate: '<div class="ui-grid-cell-contents my-cell ng-binding ng-scope " ng-dblclick="grid.appScope.Post()"  ng-bind-html="COL_FIELD | trusted">  </div>' });
                      $scope.gridOptions.data = result.Data;
                      $scope.gridApi.grid.refresh();
                  }
                  else if (!result.Success) {
                      alert('Something went wrong');
                  }

              },
              error: function () {
                  Lock: false;
                  alert('Something went wrong');
              }
          });
      }

  }
]);

MaterialBalanceJobWorker.filter('trusted', function ($sce) {
    return function (value) {
        return $sce.trustAsHtml(value);
    }
});



