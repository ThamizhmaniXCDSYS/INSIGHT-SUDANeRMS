// frameMagic.js; standalone JS to fix the iframe-content positioning issue which normally results in what seems to be a blank 2nd page being shown
//
// version: 201208301100
//
// author: frank -futtta- goossens
// License: CreativeCommons Attribution-Share Alike (http://creativecommons.org/licenses/by-sa/3.0/) or LGPL (http://www.gnu.org/licenses/lgpl-3.0.txt)
// more info: http://blog.futtta.be/2010/03/09/
//
// usage: 
//      * include this script in the head of your document and you're done
//      * optionally set fM_conf as a comma-seperated list if iframe-id's to be magically fixed, the script will then leave other iframes be
// 	  e.g.: var fM_conf="iFrame1,iFrame3";
//
// bugs & issues:
//	* useless if you don't have a long iframe in which first a long iframe-content-page is loaded and then a short one

window.onload = function () {
    if (typeof fM_conf === "undefined") { conf = ""; }
    else { conf = fM_conf; }

    ifr = document.getElementsByTagName('iframe');
    for (i = 0; i < ifr.length; i++) {
        iframe = ifr[i];
        if (!iframe.id) { iframe.id = "iframe-" + i };
        iid = iframe.id;
        if ((conf === "") || (conf.indexOf(iid) >= 0)) {
            if (iframe.addEventListener) {
                iframe.addEventListener("load",
					function (id) {
					    return function () { frameMagic(id) };
					} (iid),
					false);
            } else if (iframe.attachEvent) {
                iframe.attachEvent("onload",
					function (iid) {
					    return function () { frameMagic(id) };
					} (iid)
				);
            }
        }
    }
}

function frameMagic(el) {
    document.getElementById(el).scrollIntoView();
    window.scrollBy(0, -50)
}

