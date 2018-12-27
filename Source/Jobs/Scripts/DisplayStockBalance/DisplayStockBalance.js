StockBalanceDisplay = angular.module('StockBalanceDisplay', ['ngTouch', 'ui.grid', 'ui.grid.resizeColumns', 'ui.grid.moveColumns',
    'ui.grid.exporter', 'ui.grid.cellNav', 'ui.grid.pinning', 'ui.grid.treeView'])



StockBalanceDisplay.controller('MainCtrl', ['$rootScope', '$scope', '$log', '$http', 'modal', 'uiGridConstants', 'uiGridExporterConstants', 'uiGridExporterService', '$timeout',



function ($rootScope, $scope, $log, $http, modal, uiGridConstants, uiGridExporterConstants, uiGridExporterService, $timeout) {
    $scope.gridOptions = {
        enableGridMenu: true,
        enableFiltering: true,
        showColumnFooter: true,
        enableGridMenu: true,
        fastwatch: true,
        enableFullRowSelection: true,
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


    $scope.GetColumnIndexFromName = function (ColumnName) {
        var colindex = 0;
        for (var i = 0 ; i <= $scope.gridApi.grid.columns.length - 1; i++) {
            if ($scope.gridApi.grid.columns[i].name == ColumnName)
                colindex = i;
        }
        return colindex;
    };


    var FocusCellArr = []
    var ProductFilterArr = []
    var Dimension1FilterArr = []
    var Dimension2FilterArr = []
    var Dimension3FilterArr = []
    var Dimension4FilterArr = []

    var Count = 0;
    $scope.ShowDetail = function () {
        var rowCol = $scope.gridApi.cellNav.getFocusedCell();
        $("#TextHidden").val(rowCol.col.name);
        $("#Product").val(rowCol.row.entity.Product);
        if (rowCol.row.entity.ProductId != null) {
            $("#Product").select2('data', { id: rowCol.row.entity.ProductId, text: rowCol.row.entity.ProductName });
        }
        if (rowCol.row.entity.FromDate != null) {
            $("#FromDate").val(rowCol.row.entity.FromDate);
        }
        if (rowCol.row.entity.ToDate != null) {
            $("#ToDate").val(rowCol.row.entity.ToDate);
        }
        if (rowCol.row.entity.ProductTypeId != null) {
            $("#ProductType").select2('data', { id: rowCol.row.entity.ProductTypeId, text: rowCol.row.entity.ProductTypeName });
        }
        if (rowCol.row.entity.ProductNature != null) {
            $("#ProductNature").select2('data', { id: rowCol.row.entity.ProductNature, text: rowCol.row.entity.ProductNatureName });
        }
        if (rowCol.row.entity.ProductCategoryId != null) {

            $("#ProductCategory").select2('data', { id: rowCol.row.entity.ProductCategoryId, text: rowCol.row.entity.ProductCategoryName });
        }
        if (rowCol.row.entity.ProductGroupId != null) {

            $("#ProductGroup").select2('data', { id: rowCol.row.entity.ProductGroupId, text: rowCol.row.entity.ProductGroupName });
        }
        if (rowCol.row.entity.SiteIds != null) {
            $("#SiteIds").select2('data', { id: rowCol.row.entity.SiteIds, text: rowCol.row.entity.SiteName })
        }
        if (rowCol.row.entity.Division != null) {
            $("#DivisionIds").select2('data', { id: rowCol.row.entity.DivisionId, text: rowCol.row.entity.DivisionName })
        }
        if (rowCol.row.entity.ShowBalance != null) {
            $("#ShowBalance").select2('data', { id: rowCol.row.entity.ShowBalance })
        }
        if (rowCol.row.entity.GroupOnId != null) {
            $("#GroupOn").select2('data', { id: rowCol.row.entity.GroupOnId, text: rowCol.row.entity.GroupOnName })
        }
        if (rowCol.row.entity.ShowOpening != null) {

            $("#ShowOpening ").select2('data', { id: rowCol.row.entity.ShowOpening })
        }

        if (rowCol.row.entity.GodownId != null) {
            $("#Godown").select2('data', { id: rowCol.row.entity.GodownId, text: rowCol.row.entity.GodownName })
        }
        if (rowCol.row.entity.PersonId != null) {
            $("#Name").select2('data', { id: rowCol.row.entity.PersonId, text: rowCol.row.entity.Name })
        }
        if (rowCol.row.entity.ProcessId != null) {
            $("#Process").select2('data', { id: rowCol.row.entity.ProcessId, text: rowCol.row.entity.ProcessName })
        }
        if (rowCol.row.entity.Dimension1Id != null) {
            $("#Dimension1Name").select2('data', { id: rowCol.row.entity.Dimension1Id, text: rowCol.row.entity.Dimension1Name })
        }
        if (rowCol.row.entity.Dimension2Id != null) {
            $("#Dimension2Name").select2('data', { id: rowCol.row.entity.Dimension2Id, text: rowCol.row.entity.Dimension2Name })
        }
        if (rowCol.row.entity.Dimension3Id != null) {
            $("#Dimension3Name").select2('data', { id: rowCol.row.entity.Dimension3Id, text: rowCol.row.entity.Dimension3Name })
        }
        if (rowCol.row.entity.Dimension4Id != null) {
            $("#Dimension4Name").select2('data', { id: rowCol.row.entity.Dimension4Id, text: rowCol.row.entity.Dimension4Name })
        }
        var Hdn = rowCol.row.entity.name;
        var DocTypeId = parseInt(rowCol.row.entity.DocTypeId);
        var DNo = rowCol.row.entity.DocNo;

        var ProductId = parseInt(rowCol.row.entity.ProductId);
        var DocId = parseInt(rowCol.row.entity.DocHeaderId);// StockHeaderId
        if (DNo == 'Opening') {
            return;
        }

        if (rowCol.row.entity.DocHeaderId != null) {
            window.open('/DisplayStockBalance/DocumentMenu/?DocTypeId=' + DocTypeId + '&DocId=' + DocId, '_blank');
            return;

        }
        //  *************

        if (rowCol != null)
            FocusCellArr.push(rowCol);

        if (rowCol.row.entity.ProductName != null)
            ProductFilterArr.push($scope.gridApi.grid.columns[$scope.GetColumnIndexFromName("ProductName")].filters[0].term);

        if (rowCol.row.entity.Dimension1Name != null)
            Dimension1FilterArr.push($scope.gridApi.grid.columns[$scope.GetColumnIndexFromName("Dimension1Name")].filters[0].term);
        if (rowCol.row.entity.Dimension2Name != null)
            Dimension2FilterArr.push($scope.gridApi.grid.columns[$scope.GetColumnIndexFromName("Dimension2Name")].filters[0].term);
        if (rowCol.row.entity.Dimension3Name != null)
            Dimension3FilterArr.push($scope.gridApi.grid.columns[$scope.GetColumnIndexFromName("Dimension3Name")].filters[0].term);
        if (rowCol.row.entity.Dimension4Name != null)
            Dimension4FilterArr.push($scope.gridApi.grid.columns[$scope.GetColumnIndexFromName("Dimension4Name")].filters[0].term);





        //// end **********

        $.ajax({
            async: false,
            cache: false,
            type: "POST",
            url: '/DisplayStockBalance/SaveCurrentSetting',
            success: function (data) {
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //  alert('Failed to retrieve product details.' + thrownError);
                return;
            }
        });
        Count = Count + 1;
        $scope.BindData();
    };

    var myModal = new modal();
    $rootScope.gridOptions = {
        onRegisterApi: function (gridApi) {
            $rootScope.gridApi = gridApi;
        }

    };

    var IsEscapeButtonPressed = false;

    $(document).keyup(function (e) {
        if (Count > 0) {
            if (e.keyCode == 27) { // escape key maps to keycode `27`
                $.ajax({
                    async: false,
                    cache: false,
                    type: "POST",
                    url: '/DisplayStockBalance/GetParameterSettingsForLastDisplay',
                    success: function (result) {
                        $("#FromDate").val(result.FromDate);
                        $("#ToDate").val(result.ToDate);
                        $("#ProductNature").val(result.ProductNature);
                        $("#ProductType").val(result.ProductType);
                        $("#ProductCategory").val(result.ProductCategory);
                        $("#GroupOn").val(result.GroupOn);
                        $("#Product").val(result.Product);
                        $("#SiteIds").val(result.SiteId);
                        $("#DivisionIds").val(result.DivisionId);
                        $("#ProductGroup").val(result.ProductGroup)
                        $("#TextHidden").val(result.TextHidden);

                        $("#Process").val(result.Process);
                        $("#Godown").val(result.Godown);
                        $("#Name").val(result.Name);
                        $("#Dimension1Name").val(result.Dimension1Name)
                        $("#Dimension2Name").val(result.Dimension2Name)
                        $("#Dimension3Name").val(result.Dimension3Name)
                        $("#Dimension4Name").val(result.Dimension4Name)


                        CustomSelectFunction($("#DivisionIds"), '/ComboHelpList/GetDivision', '/ComboHelpList/SetSingleDivision', ' ', false, 0);
                        CustomSelectFunction($("#SiteIds"), '/ComboHelpList/GetSite', '/ComboHelpList/SetSingleSite', ' ', false, 0);

                        CustomSelectFunction($("#ProductType"), '/ComboHelpList/GetProductType', '/ComboHelpList/SetSingleProductType', ' ', false, 0);
                        //    CustomSelectFunction($("#JobWorker"), '/ComboHelpList/GetPerson', '/ComboHelpList/SetSinglePerson', ' ', false, 0);
                        //   CustomSelectFunction($("#Format"), '/Display_StockBalance/GetFilterFormat', '/Display_StockBalance/SetFilterFormat', ' ', false, 0);
                        //    CustomSelectFunction($("#Process"), '/ComboHelpList/GetProcess', '/ComboHelpList/SetSingleProcess', ' ', false, 0);
                        CustomSelectFunction($("#ProductCategory"), '/ComboHelpList/GetProductCategory', '/ComboHelpList/SetSingleProductCategory', ' ', false, 0);
                        CustomSelectFunction($("#ProductGroup"), '/ComboHelpList/GetProductGroup', '/ComboHelpList/SetSingleProductGroup', ' ', false, 0);
                        //   CustomSelectFunction($("#Product"), '/ComboHelpList/GetProduct', '/ComboHelpList/SetSingleProduct', ' ', false, 0);
                        // CustomSelectFunction($("#Godown"), '/ComboHelpList/GetGodown', '/ComboHelpList/SetSingleGodown', ' ', false, 0);
                        CustomSelectFunction($("#GroupOn"), '/DisplayStockBalance/GetGroupOn', '/DisplayStockBalance/SetGroupOn', ' ', true, 0);
                        CustomSelectFunction($("#ShowBalance"), '/DisplayStockBalance/GetShowBalence', '/DisplayStockBalance/SetShowBalence', ' ', false, 0);
                        ///    ////
                        CustomSelectFunction($("#ProductNature"), '/ComboHelpList/GetProductNature', '/ComboHelpList/SetSingleProductNature', ' ', false, 0);
                        CustomSelectFunction($("#Process"), '/ComboHelpList/GetProcess', '/ComboHelpList/SetSingleProcess', ' ', false, 0);
                        CustomSelectFunction($("#Godown"), '/ComboHelpList/GetGodown', '/ComboHelpList/SetSingleGodown', ' ', false, 0);
                        CustomSelectFunction($("#Name"), '/ComboHelpList/SetJobWorkers', '/ComboHelpList/SetSingleJobWorker', ' ', false, 0);
                        CustomSelectFunction($("#Product"), '/ComboHelpList/GetProducts', '/ComboHelpList/SetSingleProducts', ' ', false, 0);
                        CustomSelectFunction($("#Dimension1Name"), '/ComboHelpList/GetDimension1', '/ComboHelpList/SetSingleDimension1', ' ', false, 0);
                        CustomSelectFunction($("#Dimension2Name"), '/ComboHelpList/GetDimension2', '/ComboHelpList/SetSingleDimension2', ' ', false, 0);
                        CustomSelectFunction($("#Dimension3Name"), '/ComboHelpList/GetDimension3', '/ComboHelpList/SetSingleDimension3', ' ', false, 0);
                        CustomSelectFunction($("#Dimension4Name"), '/ComboHelpList/GetDimension4', '/ComboHelpList/SetSingleDimension4', ' ', false, 0);

                        //  if (result.FromDate == null)
                        //     $("#FromDate").select2('data', { id: '', text: '' });
                        //  if (result.todate == null)
                        //      $("#Todate").select2('data', { id: '', text: '' });
                        if (result.ProductNature == null)
                            $("#ProductNature").select2('data', { id: '', text: '' });
                        if (result.ProductType == null)
                            $("#ProductType").select2('data', { id: '', text: '' });
                        if (result.SiteId == null)
                            $("#SiteIds").select2('data', { id: '', text: '' });
                        if (result.DivisionId == null)
                            $("#DivisionIds").select2('data', { id: '', text: '' });
                        if (result.ProductCategory == null)
                            $("ProductCategory").select2('data', { id: '', text: '' });
                        if (result.ProductGroup == null)
                            $("ProductGroup").select2('data', { id: '', text: '' });
                        // if (result.ShowBalance == null)
                        //    $("#ShowBalance").select2('data', { id: '', text: '' });
                        if (result.GroupOn == null)
                            $("#GroupOn").select2('data', { id: '', text: '' });
                        //   if (result.ShowOpening == null)
                        //    $("#ShowOpening").select2('data', { id: '', text: '' });
                        if (result.Product == null) {
                            $("#Product").select2('data', { id: '', text: '' });

                        }
                        // else
                        //     $("#product").select2('data', {id:result.ProductId,text:result.ProductName});
                        if (result.GodownName == null)
                            $("#Godown").select2('data', { id: '', text: '' });
                        if (result.Person == null)
                            $("#Name").select2('data', { id: '', text: '' });
                        if (result.Process == null)
                            $("#Process").select2('data', { id: '', text: '' });
                        if (result.Dimension1Name == null)
                            $("#Dimension1Name").select2('data', { id: '', text: '' });
                        if (result.Dimension2Name == null)
                            $("#Dimension2Name").select2('data', { id: '', text: '' });
                        if (result.Dimension3Name == null)
                            $("#Dimension3Name").select2('data', { id: '', text: '' });
                        if (result.Dimension4Name == null)
                            $("#Dimension4Name").select2('data', { id: '', text: '' });

                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        // alert('Failed to retrieve product details.' + thrownError);

                        return;
                    }
                });

                IsEscapeButtonPressed = true;
                $scope.BindData();
                IsEscapeButtonPressed = false;
                Count = Count - 1;

                var timeoutPeriod = 1000;

                $timeout(function () {
                    if (ProductFilterArr.length > 0) {
                        $scope.gridApi.grid.columns[$scope.GetColumnIndexFromName("ProductName")].filters[0].term = ProductFilterArr[ProductFilterArr.length - 1];
                        ProductFilterArr.pop();
                    }
                    if (Dimension1FilterArr.length > 0) {
                        $scope.gridApi.grid.columns[$scope.GetColumnIndexFromName("Dimension1Name")].filters[0].term = Dimension1FilterArr[Dimension1FilterArr.length - 1];
                        Dimension1FilterArr.pop();
                    }
                    if (Dimension2FilterArr.length > 0) {
                        $scope.gridApi.grid.columns[$scope.GetColumnIndexFromName("Dimension2Name")].filters[0].term = Dimension2FilterArr[Dimension2FilterArr.length - 1];
                        Dimension2FilterArr.pop();
                    }
                    if (Dimension3FilterArr.length > 0) {
                        $scope.gridApi.grid.columns[$scope.GetColumnIndexFromName("Dimension3Name")].filters[0].term = Dimension3FilterArr[Dimension3FilterArr.length - 1];
                        Dimension3FilterArr.pop();
                    }
                    if (Dimension4FilterArr.length > 0) {
                        $scope.gridApi.grid.columns[$scope.GetColumnIndexFromName("Dimension4Name")].filters[0].term = Dimension4FilterArr[Dimension4FilterArr.length - 1];
                        Dimension4FilterArr.pop();
                    }


                    if (FocusCellArr.length > 0) {
                        var row = FocusCellArr[FocusCellArr.length - 1].row;
                        var col = FocusCellArr[FocusCellArr.length - 1].col;

                        var RowIndex = 0;
                        for (var i = 0; i <= $scope.gridOptions.data.length - 1; i++) {
                            if ($scope.gridOptions.data[i].ProductId != null)
                                if ($scope.gridOptions.data[i].ProductId == row.entity.ProductId)
                                    RowIndex = i;

                            if ($scope.gridOptions.data[i].Dimension1Id != null)
                                if ($scope.gridOptions.data[i].Dimension1Id == row.entity.Dimension1Id)
                                    RowIndex = i;

                            if ($scope.gridOptions.data[i].Dimension2Id != null)
                                if ($scope.gridOptions.data[i].Dimension2Id == row.entity.Dimension2Id)
                                    RowIndex = i;

                            if ($scope.gridOptions.data[i].Dimension3Id != null)
                                if ($scope.gridOptions.data[i].Dimension3Id == row.entity.Dimension3Id)
                                    RowIndex = i;

                            if ($scope.gridOptions.data[i].Dimension4Id != null)
                                if ($scope.gridOptions.data[i].Dimension4Id == row.entity.Dimension4Id)
                                    RowIndex = i;
                        }

                        if ($scope.gridOptions.data[RowIndex].ProductId != null)
                            $scope.gridApi.cellNav.scrollToFocus($scope.gridOptions.data[RowIndex], $scope.gridApi.grid.columns[$scope.GetColumnIndexFromName("ProductName")]);

                        if ($scope.gridOptions.data[RowIndex].Dimension1Id != null)
                            $scope.gridApi.cellNav.scrollToFocus($scope.gridOptions.data[RowIndex], $scope.gridApi.grid.columns[$scope.GetColumnIndexFromName("Dimension1Name")]);

                        if ($scope.gridOptions.data[RowIndex].Dimension2Id != null)
                            $scope.gridApi.cellNav.scrollToFocus($scope.gridOptions.data[RowIndex], $scope.gridApi.grid.columns[$scope.GetColumnIndexFromName("Dimension2Name")]);

                        if ($scope.gridOptions.data[RowIndex].Dimension3Id != null)
                            $scope.gridApi.cellNav.scrollToFocus($scope.gridOptions.data[RowIndex], $scope.gridApi.grid.columns[$scope.GetColumnIndexFromName("Dimension3Name")]);

                        if ($scope.gridOptions.data[RowIndex].Dimension4Id != null)
                            $scope.gridApi.cellNav.scrollToFocus($scope.gridOptions.data[RowIndex], $scope.gridApi.grid.columns[$scope.GetColumnIndexFromName("Dimension4Name")]);




                        FocusCellArr.pop();
                    }
                }, timeoutPeriod)
            }
        }


        if (e.keyCode == 13) {
            // escape key maps to keycode `27`
            if ($scope.gridApi.cellNav.getFocusedCell() != null) {
                $scope.ShowDetail();
            }
        }
    });



    var Dimension1Caption = "Dimension1";
    var Dimension2Caption = "Dimension2";
    var Dimension3Caption = "Dimension3";
    var Dimension4Caption = "Dimension4";
    function SetCaptions() {
        $.ajax({
            async: false,
            cache: false,
            type: "POST",
            url: '/DisplayStockBalance/GetDivisionSettings',
            success: function (result) {
                Dimension1Caption = result.Dimension1Caption;
                Dimension2Caption = result.Dimension2Caption;
                Dimension3Caption = result.Dimension3Caption;
                Dimension4Caption = result.Dimension4Caption;
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert('Failed to retrieve product details.' + thrownError);
            }
        });
    }



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
                    && $scope.gridOptions.columnDefs[i].field != "DealUnitName"
                    && $scope.gridOptions.columnDefs[i].field != "BalanceDealQty") {
                    for (var j = 0; j <= removeColumns.length - 1 ; j++) {
                        if (removeColumns[j].columnname == $scope.gridOptions.columnDefs[i].field) {
                            removeColumns.splice(j, 1);
                        }
                    }
                }

                if ($scope.gridOptions.data[k]["DealUnitName"] != $scope.gridOptions.data[k]["UnitName"]) {
                    for (var j = 0; j <= removeColumns.length - 1 ; j++) {
                        if (removeColumns[j].columnname == "DealUnit" || removeColumns[j].columnname == "BalanceDealQty") {
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


    var i = 0;
    $scope.BindData = function () {
        //SetCaptions();

        //  saveProductId();
        $scope.myData = [];


        $.ajax({
            url: '/DisplayStockBalance/DisplayStockBalanceFill/' + $(this).serialize(),
            async: (IsEscapeButtonPressed == true ? false : true),
            type: "POST",
            data: $("#registerSubmit").serialize(),
            success: function (result) {
                Lock = false;
                if (result.Success == true) {
                    $scope.gridOptions.columnDefs = new Array();

                    var NetBalance = 0;
                    if (result.Data != null) {
                        NetBalance = result.Data[0]["NetBalance"];
                    }

                    $scope.gridOptions.columnDefs.push({ field: 'DocHeaderId', width: 50, visible: false });
                    $scope.gridOptions.columnDefs.push({ field: 'DocTypeId', width: 50, visible: false });
                    $scope.gridOptions.columnDefs.push({ field: 'Product', width: 50, visible: false });
                    $scope.gridOptions.columnDefs.push({ field: 'PersonId', width: 50, visible: false });
                    $scope.gridOptions.columnDefs.push({ field: 'ProcessId', width: 50, visible: false }); //Dimension3Id
                    $scope.gridOptions.columnDefs.push({ field: 'GodownId', width: 50, visible: false });
                    $scope.gridOptions.columnDefs.push({ field: 'Dimension1Id', width: 50, visible: false });
                    $scope.gridOptions.columnDefs.push({ field: 'Dimension2Id', width: 50, visible: false });
                    $scope.gridOptions.columnDefs.push({ field: 'Dimension3Id', width: 50, visible: false });
                    $scope.gridOptions.columnDefs.push({ field: 'Dimension4Id', width: 50, visible: false });
                    $scope.gridOptions.columnDefs.push({ field: 'ProductId', width: 50, visible: false, });

                    $scope.gridOptions.columnDefs.push({ field: 'DocNo', width: 100, cellClass: 'cell-text', headerCellClass: 'header-text', cellTemplate: '<div class="ui-grid-cell-contents my-cell ng-binding ng-scope " ng-dblclick="grid.appScope.ShowDetail()"  ng-bind-html="COL_FIELD | trusted">  </div>', });
                    $scope.gridOptions.columnDefs.push({ field: 'DocDate', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text', cellTemplate: '<div class="ui-grid-cell-contents my-cell ng-binding ng-scope " ng-dblclick="grid.appScope.ShowDetail()"  ng-bind-html="COL_FIELD | trusted">  </div>', });
                    $scope.gridOptions.columnDefs.push({ field: 'DocumentTypeName', width: 200, cellClass: 'cell-text ', headerCellClass: 'header-text', cellTemplate: '<div class="ui-grid-cell-contents my-cell ng-binding ng-scope " ng-dblclick="grid.appScope.ShowDetail()"  ng-bind-html="COL_FIELD | trusted">  </div>', });
                    $scope.gridOptions.columnDefs.push({ field: 'PartyName', width: 200, visible: true, cellClass: 'cell-text', headerCellClass: 'header-text', cellTemplate: '<div class="ui-grid-cell-contents my-cell ng-binding ng-scope " ng-dblclick="grid.appScope.ShowDetail()"  ng-bind-html="COL_FIELD | trusted">  </div>', });
                    $scope.gridOptions.columnDefs.push({ field: 'ProductName', headerCellClass: $scope.highlightFilteredHeader, width: 200, visible: true, cellClass: 'cell-text', headerCellClass: 'header-text', cellTemplate: '<div class="ui-grid-cell-contents my-cell ng-binding ng-scope " ng-dblclick="grid.appScope.ShowDetail()"  ng-bind-html="COL_FIELD | trusted">  </div>', });
                    $scope.gridOptions.columnDefs.push({ field: 'Dimension1Name', displayName: 'Size', width: 150, visible: true, aggregationHideLabel: true, headerCellClass: 'header-text', footerCellClass: 'text-right ', cellClass: 'cell-text', cellTemplate: '<div class="ui-grid-cell-contents my-cell ng-binding ng-scope " ng-dblclick="grid.appScope.ShowDetail()"  ng-bind-html="COL_FIELD | trusted">  </div>', });
                    $scope.gridOptions.columnDefs.push({ field: 'Dimension2Name', displayName: 'Design', width: 150, visible: true, aggregationHideLabel: true, headerCellClass: 'header-text', footerCellClass: 'text-right ', cellClass: 'cell-text', cellTemplate: '<div class="ui-grid-cell-contents my-cell ng-binding ng-scope " ng-dblclick="grid.appScope.ShowDetail()"  ng-bind-html="COL_FIELD | trusted">  </div>', });
                    $scope.gridOptions.columnDefs.push({ field: 'Dimension3Name', displayName: 'Shade', width: 150, visible: true, aggregationHideLabel: true, headerCellClass: 'header-text', footerCellClass: 'text-right ', cellClass: 'cell-text', cellTemplate: '<div class="ui-grid-cell-contents my-cell ng-binding ng-scope " ng-dblclick="grid.appScope.ShowDetail()"  ng-bind-html="COL_FIELD | trusted">  </div>', });
                    $scope.gridOptions.columnDefs.push({ field: 'Dimension4Name', displayName: 'Fabric', width: 150, visible: true, aggregationHideLabel: true, headerCellClass: 'header-text', footerCellClass: 'text-right ', cellClass: 'cell-text', cellTemplate: '<div class="ui-grid-cell-contents my-cell ng-binding ng-scope " ng-dblclick="grid.appScope.ShowDetail()"  ng-bind-html="COL_FIELD | trusted">  </div>', });
                    $scope.gridOptions.columnDefs.push({ field: 'ProcessName', width: 100, cellClass: 'cell-text', headerCellClass: 'header-text', cellTemplate: '<div class="ui-grid-cell-contents my-cell ng-binding ng-scope " ng-dblclick="grid.appScope.ShowDetail()"  ng-bind-html="COL_FIELD | trusted">  </div>', });
                    $scope.gridOptions.columnDefs.push({ field: 'LotNo', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                    $scope.gridOptions.columnDefs.push({ field: 'PlanNo', width: 100, cellClass: 'cell-text ', headerCellClass: 'header-text' });
                    $scope.gridOptions.columnDefs.push({ field: 'GodownLocation', displayName: 'Godown/Location', width: 250, cellClass: 'cell-text ', headerCellClass: 'header-text', cellTemplate: '<div class="ui-grid-cell-contents my-cell ng-binding ng-scope " ng-dblclick="grid.appScope.ShowDetail()"  ng-bind-html="COL_FIELD | trusted">  </div>', });
                    $scope.gridOptions.columnDefs.push({ field: 'Name', width: 200, visible: false, cellClass: 'cell-text', headerCellClass: 'header-text', cellTemplate: '<div class="ui-grid-cell-contents my-cell ng-binding ng-scope " ng-dblclick="grid.appScope.ShowDetail()"  ng-bind-html="COL_FIELD | trusted">  </div>', });
                    $scope.gridOptions.columnDefs.push({ field: 'Opening', displayName: 'Opening', width: 150, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text', });
                    $scope.gridOptions.columnDefs.push({ field: 'RecQty', displayName: 'Receive', width: 150, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text', });
                    $scope.gridOptions.columnDefs.push({ field: 'IssQty', displayName: 'Issue', width: 150, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text', });
                    $scope.gridOptions.columnDefs.push({ field: 'BalQty', displayName: 'Balance', width: 150, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text', });
                    $scope.gridOptions.columnDefs.push({
                        field: 'Balance', displayName: 'Balance', width: 150, aggregationType: uiGridConstants.aggregationTypes.sum, aggregationHideLabel: true, headerCellClass: 'text-right header-text', footerCellClass: 'text-right ', cellClass: 'text-right cell-text',
                        footerCellTemplate: '<div class="ui-grid-cell-contents" >' + NetBalance + '</div>',
                    });


                    $scope.gridOptions.data = result.Data;
                    $scope.gridApi.grid.refresh();
                    $scope.removeNoDataColumns();

                }
                else if (!result.Success) {
                    // alert('Something went wrong');
                    return;
                }

            },
            error: function () {
                Lock: false;
                // alert('Something went wrong');
                return;
            }
        });
    }

}
]);

StockBalanceDisplay.filter('trusted', function ($sce) {
    return function (value) {
        return $sce.trustAsHtml(value);
    }
});

StockBalanceDisplay.factory('modal', ['$compile', '$rootScope', function ($compile, $rootScope) {
    return function () {
        var elm;
        //   var rowCol = $scope.gridApi.cellNav.getFocusedCell();
        var modal = {
            open: function () {

                var html = '<div class="modal" ng-style="modalStyle">{{modalStyle}}<div class="modal-dialog"><div class="modal-content"><div class="modal-header"><b style="color:red ;margin:80px"> Due to non opening ,We can not move to entry page</b></div><div class="modal-footer"><button id="buttonClose" class="btn btn-primary" ng-click="close()">Close</button></div></div></div></div>';
                elm = angular.element(html);
                angular.element(document.body).prepend(elm);

                $rootScope.close = function () {
                    modal.close();
                };

                $rootScope.modalStyle = { "display": "block" };

                $compile(elm)($rootScope);
            },
            close: function () {
                if (elm) {
                    elm.remove();
                }
            }
        };

        return modal;
    };
}]);













