
function monthcount(a, b, c) {

    
    $("#aa").empty();
    $("#monthtab").show();
    $("#monthtab >tbody").empty();

    $.getJSON("/StandardShipment/monthcnt", { reg: a, co: b, yea: c },
 function (shpmntcnt) {

     var ycnt = new Array();
     var ncnt = new Array();
     var ycnt1 = new Array();
     var ncnt1 = new Array();
     var ycnt2 = new Array();
     var ncnt2 = new Array();
     var pcnt = new Array();
     var pcnt1 = new Array();
     var pcnt2 = new Array();

     var i = 0;

     $.each(shpmntcnt, function (index, itemdata) {

         ncnt[i] = 0;
         ycnt[i] = 0;
         ncnt1[i] = 0;
         ycnt1[i] = 0;
         ncnt2[i] = 0;
         ycnt2[i] = 0;

         if (itemdata.mo == "1" && itemdata.st2 == "N") {
             ncnt[i] = itemdata.st1;
         }
         else if (itemdata.mo == "1" && itemdata.st2 == "Y") {
             ycnt[i] = itemdata.st1;
         }
         else if (itemdata.mo == "2" && itemdata.st2 == "N") {
             ncnt1[i] = itemdata.st1;
         }
         else if (itemdata.mo == "2" && itemdata.st2 == "Y") {
             ycnt1[i] = itemdata.st1;
         }
         else if (itemdata.mo == "3" && itemdata.st2 == "N") {
             ncnt2[i] = itemdata.st1;
         }
         else {
             ycnt2[i] = itemdata.st1;
         }

         pcnt[i] = (ycnt[i] / (ncnt[i] + ycnt[i])).toFixed(1);
         pcnt1[i] = (ycnt1[i] / (ncnt1[i] + ycnt1[i])).toFixed(1);
         pcnt2[i] = (ycnt2[i] / (ncnt2[i] + ycnt2[i])).toFixed(1);

         i++;
     });


     for (i = 0; i < shpmntcnt.length; i++) {
         if (ncnt[i] == null) {
             ncnt[i] = 0;
         }

         if (ycnt[i] == null) {
             ycnt[i] = 0;
         }
         if (pcnt[i] == "NaN") {
             pcnt[i] = 0;
         }

         if (ncnt1[i] == null) {
             ncnt1[i] = 0;
         }

         if (ycnt1[i] == null) {
             ycnt1[i] = 0;
         }
         if (pcnt1[i] == "NaN") {
             pcnt1[i] = 0;
         }
         if (ncnt2[i] == null) {
             ncnt2[i] = 0;
         }

         if (ycnt2[i] == null) {
             ycnt2[i] = 0;
         }
         if (pcnt2[i] == "NaN") {
             pcnt2[i] = 0;
         }



     }


     var br = shpmntcnt[0].br;

     var mon = shpmntcnt[0].con;
     if (mon == 01) {
         mon = "Month--" + "Jan";
     }

     else if (mon == 02) {
         mon = "Month--" + "Feb";
     }
     else if (mon == 03) {
         mon = "Month--" + "Mar";
     }
     else if (mon == 04) {
         mon = "Month--" + "Apr";
     }
     else if (mon == 05) {
         mon = "Month--" + "May";
     }
     else if (mon == 06) {
         mon = "Month--" + "Jun";
     }
     else if (mon == 07) {
         mon = "Month--" + "Jul";
     }
     else if (mon == 08) {
         mon = "Month--" + "Aug";
     }
     else if (mon == 09) {
         mon = "Month--" + "Sep";
     }
     else if (mon == 10) {
         mon = "Month--" + "Oct";
     }
     else if (mon == 11) {
         mon = "Month--" + "Nov";
     }
     else {
         mon = "Month--" + "Dec";
     }

     $("#monthtab #aa").append("<td>" + mon + "</td>");

     
    
//         $("#monthtab #mrow3").append("<td> " + b + "</td>");
//         $("#monthtab #mrow3").append("<td> " + br + "</td>");
//         $("#monthtab #mrow3").append("<td> " + ncnt[i] + "</td>");
//         $("#monthtab #mrow3").append("<td> " + ycnt[i] + "</td>");
//         $("#monthtab #mrow3").append("<td> " + pcnt[i] + "%" + "</td>");
//         $("#monthtab #mrow3").append("<td> " + ncnt1[i] + "</td>");
//         $("#monthtab #mrow3").append("<td> " + ycnt1[i] + "</td>");
//         $("#monthtab #mrow3").append("<td> " + pcnt1[i] + "%" + "</td>");
//         $("#monthtab #mrow3").append("<td> " + ncnt2[i] + "</td>");
//         $("#monthtab #mrow3").append("<td> " + ycnt2[i] + "</td>");
//         $("#monthtab #mrow3").append("<td> " + pcnt2[i] + "%" + "</td>");
//     

//     $("#monthtab #mrow4").append("<td><a href='#' onclick='allbranches(" + "\"" + a + "\"" + " , " + "\"" + b + "\"" + ", " + "\"" + c + "\"" + ")'>All</a> </td>");



          tbody = $("#monthtab");
          // if (tbody == null || tbody.length < 1) return;
          for (i = 0; i < shpmntcnt.length; i++) {
              var trow = $("<tr>");

              $("<td>")
              //  .addClass("tableCell")
                               .text(b)
              // .data("col", c)
                               .appendTo(trow);
              $("<td>")

                               .text(shpmntcnt[i].br)
              // .data("col", c)
                               .appendTo(trow);
              $("<td>")

                               .text(ncnt[i])
              // .data("col", c)
                               .appendTo(trow);
              $("<td>")

                               .text(ycnt[i])
              // .data("col", c)
                               .appendTo(trow);

              $("<td>")

                               .text(pcnt[i] + "%")
              // .data("col", c)
                               .appendTo(trow);


              $("<td>")

                               .text(ncnt1[i])
              // .data("col", c)
                               .appendTo(trow);
              $("<td>")

                               .text(ycnt1[i])
              // .data("col", c)
                               .appendTo(trow);
              $("<td>")

                               .text(pcnt1[i] + "%")
              // .data("col", c)
                               .appendTo(trow);
              $("<td>")

                               .text(ncnt2[i])
              // .data("col", c)
                               .appendTo(trow);
              $("<td>")

                               .text(ycnt2[i])
              // .data("col", c)
                               .appendTo(trow);

              $("<td>")

                               .text(pcnt2[i] + "%")
              // .data("col", c)
                               .appendTo(trow);
             
              trow.appendTo(tbody);
          }

          var trow1 = $("<tr>");
          $("<td>")

                               .text(b)
          // .data("col", c)
                               .appendTo(trow1);
          $("<td>")

                               .text("All")
          // .data("col", c)
                               .appendTo(trow1);
          trow1.appendTo(tbody);

 });


}
