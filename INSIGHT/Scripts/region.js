
function region(id)
 {
   //
    $("#crow1").empty();
    $("#crow2").empty();
    $("#crow3").empty();
    $("#crow4").empty();
    $("#crow5").empty();


    $("#brow1").empty();
    $("#brow2").empty();
    $("#brow3").empty();
    $("#brow4").empty();
    $("#brow5").empty();

    $("#monthtab").hide();


   

    $("#chart_company").empty();
    $("#chart_company1").empty();

    $("#chart_month").empty();
    $("#chart_month1").empty();



    $.getJSON("/StandardShipment/Companies", { Reg: id, confdat: $("#ddlYear").val(), motr: $("#ddlMode").val(), eori: $("#ddlDirection").val() },


                          function (allcmpnyshpmntcnt1) {

                              var nshpmnt = new Array();
                              var yshpmnt = new Array();
                              var comname = new Array();
                              var comname1 = new Array();


                              var i = 1;
                              var j = 1;



                              $.each(allcmpnyshpmntcnt1, function (index, itemdata) {

                                  if (itemdata.st2 == "Y") {
                                      yshpmnt[i] = itemdata.st1;
                                      comname[i] = itemdata.compy;

                                      i++;
                                  }
                                  if (itemdata.st2 == "N") {
                                      nshpmnt[j] = itemdata.st1;
                                      comname1[j] = itemdata.compy;

                                      j++;
                                  }

                              });

                              var ct;
                              if (i > j) {
                                  ct = i;
                              }
                              else {
                                  ct = j;

                              }

                              $(".companytab #crow1").append("<td>Region--" + id + "</td>");
                              $(".companytab #crow2").append("<td>With Standard Shipment</td>");
                              $(".companytab #crow3").append("<td>Without Standard Shipment</td>");

                              for (c1 = 1; c1 < ct; c1++) {
                                  $(".companytab #crow1").append("<td><a href='#'  onclick='branch(" + "\"" + id + "\"" + " , " + "\"" + comname1[c1] + "\"" + ")'>" + comname1[c1] + "</a></td>");

                                  $(".companytab #crow2").append("<td>" + yshpmnt[c1] + "</td>");
                                  $(".companytab #crow3").append("<td>" + nshpmnt[c1] + "</td>");
                              }

                              var totlshpment = new Array();
                              totlshpment[0] = "Total Shipment";

                              $(".companytab #crow4").append("<td>" + totlshpment[0] + "</td>");
                              // 
                              for (t = 1; t < ct; t++) {
                                  totlshpment[t] = nshpmnt[t] + yshpmnt[t];
                                  if (isNaN(totlshpment[t]) == true) {
                                      totlshpment[t] = 0;
                                  }
                                  $(".companytab #crow4").append("<td>" + totlshpment[t] + "</td>");
                              }



                              var percentshpmnt = new Array();

                              $(".companytab #crow5").append("<td>% Where Standard Shipment Used</td>");
                              for (p = 1; p < ct; p++) {

                                  percentshpmnt[p] = ((yshpmnt[p] / totlshpment[p]) * 100).toFixed(1) + "%";
                                  if (percentshpmnt[p] == "NaN%") {
                                      percentshpmnt[p] = 0 + "%";
                                  }

                                  $(".companytab #crow5").append("<td>" + percentshpmnt[p] + "</td>");


                              }
                             
                              var strchart = '<chart yAxisName="" caption="Standard Shipment Usage " numberPrefix="" showBorder="1" imageSave="1" >'

                              for (k = 1; k < ct; k++) {
                                  strchart = strchart + '<set label="' + comname1[k] + '" value="' + totlshpment[k] + '"/>';


                              }
                              strchart = strchart + '</chart>';

                              var chart = new FusionCharts("../../FusionCharts/Column3D.swf", "strchart", "900", "500", "0", "0");
                              chart.setXMLData(strchart);
                              chart.render("chart_company1");

                          });

}