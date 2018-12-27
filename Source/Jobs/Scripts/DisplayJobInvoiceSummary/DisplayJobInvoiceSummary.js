DisplayJobInvoiceSummary = angular.module('DisplayJobInvoiceSummary', ['ngTouch', 'ui.grid', 'ui.grid.resizeColumns', 'ui.grid.moveColumns',
    'ui.grid.exporter', 'ui.grid.cellNav', 'ui.grid.pinning'])



DisplayJobInvoiceSummary.controller('MainCtrl', ['$scope', '$log', '$http', 'uiGridConstants', 'uiGridExporterConstants', 'uiGridExporterService',


  function ($scope, $log, $http, uiGridConstants, uiGridExporterConstants, uiGridExporterService, uiGridTreeViewConstants) {
      $scope.gridOptions = {
          enableGridMenu: true,
          enableFiltering: true,
          showColumnFooter: true,
          enableGridMenu: true,

          //enableGridMenu: true,
          //exporterCsvFilename: 'myFile.csv',
          //exporterPdfDefaultStyle: { fontSize: 9 },
          //exporterPdfTableStyle: { margin: [30, 30, 30, 30] },
          //exporterPdfTableHeaderStyle: { fontSize: 10, bold: true, italics: true, color: 'red' },
          //exporterPdfHeader: { text: "My Header", style: 'headerStyle' },
          //exporterPdfFooter: function (currentPage, pageCount) {
          //    return { text: currentPage.toString() + ' of ' + pageCount.toString(), style: 'footerStyle' };
          //},
          //exporterPdfCustomFormatter: function (docDefinition) {
          //    docDefinition.styles.headerStyle = { fontSize: 22, bold: true };
          //    docDefinition.styles.footerStyle = { fontSize: 10, bold: true };
          //    return docDefinition;
          //},
          //exporterPdfOrientation: 'portrait',
          //exporterPdfPageSize: 'LETTER',
          //exporterPdfMaxGridWidth: 500,



          //enableHorizontalScrollbar: uiGridConstants.scrollbars.ALWAYS,
          //showTreeExpandNoChildren : false,
          //enableFiltering: true,
          ////enableTreeView : true,
          //showColumnFooter: true,
          //enableGridMenu: true,
          //exporterCsvLinkElement: angular.element(document.querySelectorAll(".custom-csv-link-location")),
          //exporterCsvFilename: 'myFile.csv',
          exporterMenuPdf: false,
          gridMenuShowHideColumns: true,
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




      


      var Cnt = 0;
      var rc = null;
      $scope.ShowDetail = function () {
          var rowCol = $scope.gridApi.cellNav.getFocusedCell();
          rc = rowCol;
          
         //alert(rowCol.col.name);

          //$("#TextHidden").val(rowCol.col.name);
          
          //alert($("#Format").val(rowCol.row.entity.Format));
          if (rowCol.row.entity.Format != null)
          {
              $("#Format").select2('data', { id: rowCol.row.entity.Format, text: rowCol.row.entity.Format });
          }
          if (rowCol.row.entity.ReportType != null)
          {
              $("#ReportType").select2('data', { id: rowCol.row.entity.ReportType, text: rowCol.row.entity.ReportType });
          }
          if (rowCol.row.entity.FromDate != null)
          {
              $("#FromDate").val(rowCol.row.entity.FromDate);
          }
          if (rowCol.row.entity.ToDate != null)
          {
              $("#ToDate").val(rowCol.row.entity.ToDate);
          }          
          if (rowCol.row.entity.JobWorkerId != null)
          {
              $("#JobWorker").select2('data', { id: rowCol.row.entity.JobWorkerId, text: rowCol.row.entity.GroupOnText });
          }
          //if (rowCol.row.entity.ProcessId != null)
          //{
          //    $("#Process").select2('data', { id: rowCol.row.entity.ProcessId, text: rowCol.row.entity.ProcessName });
          //}
          if (rowCol.row.entity.ProductId != null)
          {
              $("#Product").select2('data', { id: rowCol.row.entity.ProductId, text: rowCol.row.entity.GroupOnText });
          }
          if (rowCol.row.entity.ProductGroupId != null) {
              $("#Product").select2('data', { id: rowCol.row.entity.ProductGroupId, text: rowCol.row.entity.GroupOnText });
          }
          

          //$scope.gridOptions.data = someData;
          //$timeout(function () {
          //    $scope.gridApi.selection.selectRow($scope.gridOptions.data[0]);
          //},
          //100)
          
        
          var DocTypeId = parseInt(rowCol.row.entity.DocTypeId);
          var DocId = parseInt(rowCol.row.entity.JobInvoiceHeaderId);
           if (rowCol.row.entity.JobInvoiceHeaderId != null)
          {
               window.open('/Display_JobInvoiceSummary/DocumentMenu/?DocTypeId=' + DocTypeId + '&DocId=' + DocId, '_blank');
              return;
          }
           

          $.ajax({
              async : false,
              cache: false,
              type: "POST",
              url: '/Display_JobInvoiceSummary/SaveCurrentSetting',
              success: function (data) {                
                                   
              },
              error: function (xhr, ajaxOptions, thrownError) {
                  alert('Failed to retrieve product details.' + thrownError);
              }
          });
          Cnt = Cnt + 1;
          $scope.BindData();

      };
      
      
      $(document).keyup(function (e) {
          if (Cnt > 0)
              {
          if (e.keyCode == 27) { // escape key maps to keycode `27`
              $.ajax({
                    async: false,
                    cache: false,
                    type: "POST",
                    url: '/Display_JobInvoiceSummary/GetParameterSettingsForLastDisplay',
                   success: function (result) {
                   $("#Format").val(result.Format);
                   $("#ReportType").val(result.ReportType);
                   $("#FromDate").val(result.FromDate);
                   $("#ToDate").val(result.ToDate);
                   $("#JobWorker").val(result.JobWorker);
                   $("#Process").val(result.Process);
                   $("#SiteIds").val(result.SiteId);
                   $("#DivisionIds").val(result.DivisionId);
                   $("#Product").val(result.Product);
                   $("#ProductGroup").val(result.ProductGroup);
                   //alert(result.TextHidden);
                   $("#TextHidden").val(result.TextHidden);
                  

                   CustomSelectFunction($("#Process"), '/ComboHelpList/GetProcessWithChildProcess', '/ComboHelpList/SetSingleProcess', ' ', false, 0);
                   CustomSelectFunction($("#DivisionIds"), '/ComboHelpList/GetDivision', '/ComboHelpList/SetSingleDivision', ' ', false, 0);
                   CustomSelectFunction($("#SiteIds"), '/ComboHelpList/GetSite', '/ComboHelpList/SetSingleSite', ' ', false, 0);
                   CustomSelectFunction($("#JobWorker"), '/ComboHelpList/GetPerson', '/ComboHelpList/SetSinglePerson', ' ', false, 0);
                   CustomSelectFunction($("#ProductGroup"), '/ComboHelpList/GetProductGroup', '/ComboHelpList/SetSingleProductGroup', ' ', false, 0);
                   CustomSelectFunction($("#Product"), '/ComboHelpList/GetProducts', '/ComboHelpList/SetSingleProducts', ' ', false, 0);
                   CustomSelectFunction($("#Format"), '/Display_JobInvoiceSummary/GetFilterFormat', '/Display_JobOrderBalance/SetFilterFormat', ' ', false, 0);
                   CustomSelectFunction($("#ReportType"), '/Display_JobInvoiceSummary/GetFilterReportType', '/Display_JobInvoiceSummary/SetFilterFormat', ' ', false, 0);
                   if (result.Format != null)
                   {
                       $("#Format").select2('data', { id: result.Format, text: result.Format });
                   }
                   if(result.ReportType !=null)
                   {
                       $("#ReportType").select2('data', { id: result.ReportType, text: result.ReportType });
                   }
                
                   //if (result.JobWorker == null)
                   //{
                   //    $("#JobWorker").select2('data', { id: '', text: '' });
                   //}
                   //if (result.SiteId == null)
                   //{
                   //    $("#SiteIds").select2('data', { id: '', text: '' });
                   //}
                   //if (result.DivisionId == null)
                   //{
                   //    $("#DivisionIds").select2('data', { id: '', text: '' });
                   //}                       
                   //if (result.Process == null)
                   //{
                   //    $("#Process").select2('data', { id: '', text: '' });
                   //}
                   //if(result.Product==null)
                   //{
                   //    $("#Product").select2('data', { id: '', text: '' });
                   //}
                   //if (result.ProductGroup == null)
                   //{
                   //    $("#ProductGroup").select2('data', { id: '', text: '' });
                   //}
                  
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //alert('Failed to retrieve product details.' + thrownError);
                    return false;
                }
              });
              Cnt = Cnt - 1;

            //    var selectedRow = null;
            //gridApi.cellNav.on.navigate($scope, function(selected) {
            //if ('.ui-grid-cell-focus ') {
            //selectedRow = selected.row.uid;
            //gridApi.core.notifyDataChange(uiGridConstants.dataChange.COLUMN);
            //}
            //});
             
              $scope.BindData();
              
          }
         }

          if (e.keyCode == 13) {
              // escape key maps to keycode `27`
              if ($scope.gridApi.cellNav.getFocusedCell() != null)
              {
                  $scope.ShowDetail();
                
              }
          }
      });

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

      $scope.removeNoDataColumns = function () {
          var removeColumns = new Array();

          for (var i = 0; i <= $scope.gridOptions.columnDefs.length - 1 ; i++) {
              removeColumns.push({ columnname: $scope.gridOptions.columnDefs[i].field })
          }

          for (var i = 0; i <= $scope.gridOptions.columnDefs.length - 1 ; i++) {
              for (var k = 0; k <= $scope.gridOptions.data.length - 1 ; k++) {
                  if ($scope.gridOptions.data[k][$scope.gridOptions.columnDefs[i].field] != 0
                      && $scope.gridOptions.data[k][$scope.gridOptions.columnDefs[i].field] != ""
                      && $scope.gridOptions.data[k][$scope.gridOptions.columnDefs[i].field] != null
                      && $scope.gridOptions.columnDefs[i].field != "DealUnit"
                      && $scope.gridOptions.columnDefs[i].field != "DealQty") {
                      for (var j = 0; j <= removeColumns.length - 1 ; j++) {
                          if (removeColumns[j].columnname == $scope.gridOptions.columnDefs[i].field) {
                              removeColumns.splice(j, 1);
                          }
                      }
                  }

                  if ($scope.gridOptions.data[k]["DealUnit"] != $scope.gridOptions.data[k]["UnitName"]) {
                      for (var j = 0; j <= removeColumns.length - 1 ; j++) {
                          if (removeColumns[j].columnname == "DealUnit" || removeColumns[j].columnname == "DealQty") {
                              removeColumns.splice(j, 1);
                          }
                      }
                  }
              }
          }

          if (removeColumns.length != $scope.gridOptions.columnDefs.length) {
              for (var i = 0; i <= removeColumns.length - 1 ; i++) {
                  for (var j = 0; j <= $scope.gridOptions.columnDefs.length - 1 ; j++) {
                      if ($scope.gridOptions.columnDefs[j].field == removeColumns[i].columnname) {
                          $scope.gridOptions.columnDefs.splice(j, 1);
                      }
                  }
              }
          }
      }

      //$scope.getCurrentSelection = function () {
      //    var values = [];
      //    var currentSelection = $scope.gridApi.cellNav.getCurrentSelection();
      //    for (var i = 0; i < currentSelection.length; i++) {
      //        values.push(currentSelection[i].row.entity[currentSelection[i].col.name])
      //    }
      //    $scope.printSelection = values.toString();
      //};



      var i = 0;
      $scope.BindData = function ()
      {
          $scope.myData = [];

          $.ajax({
              url: '/Display_JobInvoiceSummary/DisplayJobInvoiceSummaryFill/' + $(this).serialize(),
              type: "POST",
              data: $("#registerSubmit").serialize(),
              success: function (result) {
                  Lock = false;
                  if (result.Success == true) {
                      $scope.gridOptions.columnDefs = new Array();
                      

                      var Title = "";
                      if ($("#Format").val() == "Job Worker Wise Summary")
                      {
                          Title = "Job Worker";
                          $scope.gridOptions.columnDefs.push({ field: 'JobWorkerId', width: 50, visible: false });
                          
                      }
                      else if ($("#Format").val() == "Month Wise Summary")
                      {
                          Title = "Month";
                          $scope.gridOptions.columnDefs.push({ field: 'FromDate', width: 50, visible: false });
                          $scope.gridOptions.columnDefs.push({ field: 'ToDate', width: 50, visible: false });
                      }
                      else if ($("#Format").val() == "Product Wise Summary")
                      {
                          Title = "Product";
                          $scope.gridOptions.columnDefs.push({ field: 'ProductId', width: 50, visible: false });
                      }
                      else if ($("#Format").val() == "Product Group Wise Summary")
                      {
                          Title = "Product Group";
                          $scope.gridOptions.columnDefs.push({ field: 'ProductGroupId', width: 50, visible: false });
                      }
                      if (($("#Format").val() != "" || $("#Format").val() != null) && $("#Format").val() != "Detail")
                      {
                          $scope.gridOptions.columnDefs.push({ field: 'Format', width: 50, visible: false });
                          $scope.gridOptions.columnDefs.push({ field: 'GroupOnText', allowCellFocus : true, displayName: Title, width:300, cellClass: 'cell-text', headerCellClass: 'header-text', cellTemplate: '<div class="ui-grid-cell-contents my-cell ng-binding ng-scope " ng-dblclick="grid.appScope.ShowDetail()"  ng-bind-html="COL_FIELD | trusted">  </div>', });
                          $scope.gridOptions.columnDefs.push({ field: 'Qty', width: 70, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'UnitName', displayName: 'Unit', width: 60, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'DealQty', width: 80, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'DealUnit', width: 90, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Amount', width: 90, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'TaxableAmount', width: 135, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'IGST', width: 70, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'CGST', width: 70, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                         // $scope.gridOptions.columnDefs.push({ field: 'SGST', width: 70, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'SGST', width: 70, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'InvoiceAmount', width: 135, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                        
                      }
                      else
                      {
                          $scope.gridOptions.columnDefs.push({ field: 'JobInvoiceHeaderId', width: 50, visible: false });
                          $scope.gridOptions.columnDefs.push({ field: 'DocTypeId', width: 50, visible: false });
                          $scope.gridOptions.columnDefs.push({ field: 'DocNo', width: 80, cellClass: 'cell-text', headerCellClass: 'header-text', cellTemplate: '<div class="ui-grid-cell-contents my-cell ng-binding ng-scope " ng-dblclick="grid.appScope.ShowDetail()"  ng-bind-html="COL_FIELD | trusted">  </div>', });
                          $scope.gridOptions.columnDefs.push({ field: 'DocDate', width: 90, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Name', width: 200, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'ProductName', width: 180, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Size', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Style', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Shade', width: 150, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Fabric', width: 150, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Qty', width: 70, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'UnitName',displayName: 'Unit',width: 60, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'DealQty',  width: 80, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'DealUnit', width: 80, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Rate', width: 70, cellClass: 'text-right cell-text', headerCellClass: 'text-right header-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'Amount', width: 80, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'TaxableAmount', width: 135, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'IGST', width: 60, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'CGST', width: 60, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                          $scope.gridOptions.columnDefs.push({ field: 'SGST', width: 60, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text' });
                      }
                 
                      $scope.gridOptions.data = result.Data;
                      $scope.gridApi.grid.refresh();
                      $scope.removeNoDataColumns();
                      //$($element.find(".ui-grid-cell")[0]).find("div")[0].focus();
                     // uiGridCtrl.cellNav.broadcastCellNav(rc);
                      //$scope.getCurrentSelection();
                    

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

DisplayJobInvoiceSummary.filter('trusted', function ($sce) {
    return function (value) {
        return $sce.trustAsHtml(value);
    }
});




