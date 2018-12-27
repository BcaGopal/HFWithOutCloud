$(function () {

    'use strict';

    /* ChartJS
     * -------
     * Here we will create a few charts using ChartJS
     */



    //-------------
    //- Sale PIE CHART -
    //-------------
    // Get context with jQuery - using jQuery's .get() method.
    var SalepieChartCanvas = $("#SalePieChart").get(0).getContext("2d");
    var SalepieChart = new Chart(SalepieChartCanvas);
    var SalePieChartDataArray = null
    GetSalePieChartData();

    function GetSalePieChartData() {
        $.ajax({
            async: false,
            cache: false,
            type: "POST",
            url: '/DashBoardAuto/GetSalePieChartData',
            success: function (result) {
                SalePieChartDataArray = result.Data;
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert('Failed to retrieve product details.' + thrownError);
            }
        });
    }


    var SalePieChartHint = '<ul class="chart-legend clearfix">'
    SalePieChartDataArray.forEach(function (value) {
        SalePieChartHint = SalePieChartHint + '<li><i class="fa fa-circle-o" style="color:' + value.color + '"></i> ' + value.label + '</li>'
    });
    SalePieChartHint = SalePieChartHint + '</ul>'

    $('#SalePieChartHint').html(SalePieChartHint)

    var SalePieChartOptions = {
        //Boolean - Whether we should show a stroke on each segment
        segmentShowStroke: true,
        //String - The colour of each segment stroke
        segmentStrokeColor: "#fff",
        //Number - The width of each segment stroke
        segmentStrokeWidth: 1,
        //Number - The percentage of the chart that we cut out of the middle
        percentageInnerCutout: 50, // This is 0 for Pie charts
        //Number - Amount of animation steps
        animationSteps: 100,
        //String - Animation easing effect
        animationEasing: "easeOutBounce",
        //Boolean - Whether we animate the rotation of the Doughnut
        animateRotate: true,
        //Boolean - Whether we animate scaling the Doughnut from the centre
        animateScale: false,
        //Boolean - whether to make the chart responsive to window resizing
        responsive: true,
        // Boolean - whether to maintain the starting aspect ratio or not when responsive, if set to false, will take up entire container
        maintainAspectRatio: false,
        //String - A legend template
        legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<segments.length; i++){%><li><span style=\"background-color:<%=segments[i].fillColor%>\"></span><%if(segments[i].label){%><%=segments[i].label%><%}%></li><%}%></ul>",
        //String - A tooltip template
        tooltipTemplate: "<%=value %> <%=label%>"
    };
    //Create pie or douhnut chart
    // You can switch between pie and douhnut using the method below.
    SalepieChart.Doughnut(SalePieChartDataArray, SalePieChartOptions);
    //-----------------
    //- END  Sale PIE CHART -
    //-----------------



    //-------------
    //- Sale BAR CHART -
    //-------------
    var SalebarChartCanvas = $("#SaleBarChart").get(0).getContext("2d");
    var SalebarChart = new Chart(SalebarChartCanvas);
    

    var SaleChartDataArray = null
    GetSaleChartData();

    function GetSaleChartData() {
        $.ajax({
            async: false,
            cache: false,
            type: "POST",
            url: '/DashBoardAuto/GetSaleChartData',
            success: function (result) {
                SaleChartDataArray = result.Data;
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert('Failed to retrieve product details.' + thrownError);
            }
        });
    }

    var labels_SalesChart = [], data_SaleChart = []
    SaleChartDataArray.forEach(function (value) {
        labels_SalesChart.push(value.Month);
        data_SaleChart.push(value.Amount);
    });


    var SalesChartData = {
        labels: labels_SalesChart,
        datasets: [
          {
              label: "Amount",
              fillColor: "rgba(210, 214, 222, 1)",
              strokeColor: "rgba(210, 214, 222, 1)",
              pointColor: "rgba(210, 214, 222, 1)",
              pointStrokeColor: "#c1c7d1",
              pointHighlightFill: "#fff",
              pointHighlightStroke: "rgba(210, 214, 222, 1)",
              data: data_SaleChart
          }
        ]
    };

    var SalebarChartData = SalesChartData;
    SalebarChartData.datasets[0].fillColor = "#00c0ef";
    SalebarChartData.datasets[0].strokeColor = "#00c0ef";
    SalebarChartData.datasets[0].pointColor = "#00c0ef";
    var SalebarChartOptions = {
        //Boolean - Whether the scale should start at zero, or an order of magnitude down from the lowest value
        scaleBeginAtZero: true,
        //Boolean - Whether grid lines are shown across the chart
        scaleShowGridLines: true,
        //String - Colour of the grid lines
        scaleGridLineColor: "rgba(0,0,0,.05)",
        //Number - Width of the grid lines
        scaleGridLineWidth: 1,
        //Boolean - Whether to show horizontal lines (except X axis)
        scaleShowHorizontalLines: true,
        //Boolean - Whether to show vertical lines (except Y axis)
        scaleShowVerticalLines: true,
        //Boolean - If there is a stroke on each bar
        barShowStroke: true,
        //Number - Pixel width of the bar stroke
        barStrokeWidth: 1,
        //Number - Spacing between each of the X value sets
        barValueSpacing: 5,
        //Number - Spacing between data sets within X values
        barDatasetSpacing: 1,
        //String - A legend template
        legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].fillColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>",
        //Boolean - whether to make the chart responsive
        responsive: true,
        maintainAspectRatio: true
    };

    SalebarChartOptions.datasetFill = false;
    SalebarChart.Bar(SalesChartData, SalebarChartOptions);


    //-------------------------------------------------------------------------------------------------




});

//----------------------Set Single Value------------------------------------------------

function SetSingleValue(functionname, Div_Id) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: '/DashBoardAuto/' + functionname,
        success: function (result) {
            $(Div_Id).text(FormatValues(result.Data[0].Value));
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Failed to retrieve product details.' + thrownError);
        }
    });
}

//-----------------------------End Single Value Function---------------------------------------------------------


//----------------------Set Double Value------------------------------------------------

function SetDoubleValue(functionname, Div_Id_Value1, Div_Id_Value2) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: '/DashBoardAuto/' + functionname,
        success: function (result) {
            $(Div_Id_Value1).text(FormatValues(result.Data[0].Value1));
            $(Div_Id_Value2).text(FormatValues(result.Data[0].Value2));
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Failed to retrieve product details.' + thrownError);
        }
    });
}

//-----------------------------End Double Value Function---------------------------------------------------------



//----------------------Start For Readymande Table Design-------------------------------


function DesignTable(functionname, Head_Caption, Value_Caption, Div_Id) {
    var TableHTML = '<div class="box-body" style="overflow-y:auto; height: 400px;"> ' +
                                ' <table class="table table-bordered"> '
    TableHTML = TableHTML + '<tr> ' +
                                '<th style="width: 200px">' + Head_Caption + '</th> ' +
                                '<th style="width: 100px">' + Value_Caption + '</th> ' +
                            '</tr>'

    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: '/DashBoardAuto/' + functionname,
        success: function (result) {
            result.Data.forEach(function (value) {
                TableHTML = TableHTML + '<tr> ' +
                        ' <td style="width: 200px">' + value.Head + '</td> ' +
                        ' <td style="width: 100px">' + FormatValues(value.Value) + '</td> ' +
                        ' </tr>'
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Failed to retrieve product details.' + thrownError);
        }
    });
    TableHTML = TableHTML + '</table></div>'
    $(Div_Id).html(TableHTML)
}
//----------------------End For Readymande Table Design-------------------------------





//----------------------Start For Readymande Table Design-------------------------------


function DesignTable_ThreeColumns(functionname, Head_Caption, Value1_Caption, Value2_Caption, Div_Id) {
    var TableHTML = '<div class="box-body" style="overflow-y:auto; height: 400px;"> ' +
                                ' <table class="table table-bordered"> '
    TableHTML = TableHTML + '<tr> ' +
                                '<th style="width: 200px">' + Head_Caption + '</th> ' +
                                '<th style="width: 100px">' + Value1_Caption + '</th> ' +
                                '<th style="width: 100px">' + Value2_Caption + '</th> ' +
                            '</tr>'

    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: '/DashBoardAuto/' + functionname,
        success: function (result) {
            result.Data.forEach(function (value) {
                TableHTML = TableHTML + '<tr> ' +
                        ' <td style="width: 200px">' + value.Head + '</td> ' +
                        ' <td style="width: 100px">' + value.Value1 + '</td> ' +
                        ' <td style="width: 100px">' + FormatValues(value.Value2) + '</td> ' +
                        ' </tr>'
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Failed to retrieve product details.' + thrownError);
        }
    });
    TableHTML = TableHTML + '</table></div>'
    $(Div_Id).html(TableHTML)
}
//----------------------End For Readymande Table Design-------------------------------

//---------------------------------Start For Formet Values-------------------------------------

function FormatValues(Value) {
    if (Math.abs(Value) < 1000)
        return parseFloat(Value).toFixed(2);
    if (Math.abs(Value) < 100000)
        return parseFloat(Value / 1000).toFixed(2) + ' Thousand';
    else if (Math.abs(Value) < 10000000)
        return parseFloat(Value / 100000).toFixed(2) + ' Lakh';
    else if (Math.abs(Value) >= 10000000)
        return parseFloat(Value / 10000000).toFixed(2) + ' Crore';
}

//---------------------------------End Formet Values-------------------------------------


$(document).ready(function () {
    SetDoubleValue('GetSaleOrder', '#SaleOrderAmount', '#SaleOrderAmount_Today')
    SetDoubleValue('GetSale', '#SaleAmount', '#SaleAmount_Today')
    SetDoubleValue('GetPurchase', '#PurchaseAmount', '#PurchaseAmount_Today')
    SetDoubleValue('GetPacking', '#PackingAmount', '#PackingAmount_Today')

    SetSingleValue('GetSaleOrderBalance', '#SaleOrderBalanceQty')
    SetSingleValue('GetPackedButNotShipped', '#PackedButNotDispatchedQty')
    SetSingleValue('GetJobOrderBalance', '#JobOrderBalanceQty')
    SetSingleValue('GetProcessReceive', '#ProcessReceive')


    DesignTable_ThreeColumns('GetSaleOrderDetailProductGroupWise', 'Group', "Qty", 'Amount', '#SaleOrderDetailProductGroupWise');
    DesignTable_ThreeColumns('GetSaleDetailProductGroupWise', 'Group', "Qty", 'Amount', '#SaleDetailProductGroupWise');
    DesignTable_ThreeColumns('GetPurchaseDetailProductGroupWise', 'Group', "Qty", 'Amount', '#PurchaseDetailProductGroupWise');
    DesignTable_ThreeColumns('GetPackingDetailProductGroupWise', 'Group', "Qty", 'Amount', '#PackingDetailProductGroupWise');

    DesignTable_ThreeColumns('GetSaleOrderBalanceDetailProductGroupWise', 'Group', "Qty", 'Amount', '#SaleOrderBalanceDetailProductGroupWise');
    DesignTable_ThreeColumns('GetPackedButNotShippedDetailProductGroupWise', 'Group', "Qty", 'Amount', '#PackedButNotShippedDetailProductGroupWise');
    DesignTable_ThreeColumns('GetJobOrderBalanceDetailProductGroupWise', 'Group', "Qty", 'Amount', '#JobOrderBalanceDetailProductGroupWise');
    DesignTable_ThreeColumns('GetProcessReceiveDetailProductGroupWise', 'Group', "Qty", 'Amount', '#ProcessReceiveDetailProductGroupWise');
});

