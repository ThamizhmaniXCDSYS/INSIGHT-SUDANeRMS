/*
FusionCharts JavaScript Library
Copyright FusionCharts Technologies LLP
License Information at <http://www.fusioncharts.com/license>

@author FusionCharts Technologies LLP
@version fusioncharts/3.2.3-sr3.5322

Third-party attributions:
SWFObject v2.2 <http://code.google.com/p/swfobject/>
JSON v2 <http://www.JSON.org/js.html>
Firebug Lite 1.3.0 <http://getfirebug.com/firebuglite>
jQuery 1.7.1 <http://jquery.com/>
*/
(function () {
    if (typeof window.FusionCharts === "undefined") {
        var a = {}, i = a.modules = {}, h = ["swfUrl", "id", "width", "height", "debugMode", "registerWithJS", "bgColor", "scaleMode", "lang", "detectFlashVersion", "autoInstallRedirect"], j = Object.prototype.toString, f = /msie/i.test(navigator.userAgent) && !window.opera, b = !!document.createElementNS && !!document.createElementNS("http://www.w3.org/2000/svg", "svg").createSVGRect, e = function (a, d) {
            var b, f; if (d instanceof Array) for (b = 0; b < d.length; b += 1) typeof d[b] !== "object" ? a[b] =
d[b] : (typeof a[b] !== "object" && (a[b] = d[b] instanceof Array ? [] : {}), e(a[b], d[b])); else for (b in d) typeof d[b] === "object" ? (f = j.call(d[b]), f === "[object Object]" ? (typeof a[b] !== "object" && (a[b] = {}), e(a[b], d[b])) : f === "[object Array]" ? (a[b] instanceof Array || (a[b] = []), e(a[b], d[b])) : a[b] = d[b]) : a[b] = d[b]; return a
        }; a.extend = function (a, d, b, f) { var g; if (b && a.prototype) a = a.prototype; if (f === !0) e(a, d); else for (g in d) a[g] = d[g]; return a }; a.uniqueId = function () { return "chartobject-" + (a.uniqueId.lastId += 1) }; a.uniqueId.lastId =
0; a.policies = { options: { swfSrcPath: ["swfSrcPath", void 0], product: ["product", "v3"], insertMode: ["insertMode", "replace"], safeMode: ["safeMode", !0], overlayButton: ["overlayButton", void 0], containerBackgroundColor: ["backgroundColor", void 0], chartType: ["type", void 0] }, attributes: { lang: ["lang", "EN"], "class": ["className", "FusionCharts"], id: ["id", void 0] }, width: ["width", "400"], height: ["height", "300"], src: ["swfUrl", ""] }; a.parsePolicies = function (c, d, b) {
    var f, g, m; for (g in d) if (a.policies[g] instanceof Array) m = b[d[g][0]],
c[g] = m === void 0 ? d[g][1] : m; else for (f in typeof c[g] !== "object" && (c[g] = {}), d[g]) m = b[d[g][f][0]], c[g][f] = m === void 0 ? d[g][f][1] : m
}; a.core = function (c) {
    if (!(this instanceof a.core)) { if (arguments.length === 1 && c instanceof Array && c[0] === "private") { if (i[c[1]]) return; i[c[1]] = {}; return typeof c[2] === "function" ? c[2].call(a, i[c[1]]) : a } if (arguments.length === 1 && typeof c === "string") return a.core.items[c]; a.raiseError(this, "25081840", "run", "", new SyntaxError('Use the "new" keyword while creating a new FusionCharts object')) } var d =
{}, o; this.__state = {}; if (arguments.length === 1 && typeof arguments[0] === "object") d = arguments[0]; else { for (o in h) d[h[o]] = arguments[o]; if (a.core.options.sensePreferredRenderer && d.swfUrl && d.swfUrl.match && !d.swfUrl.match(/[^a-z0-9]+/ig)) d.type = d.swfUrl } arguments.length > 1 && typeof arguments[arguments.length - 1] === "object" && (delete d[h[arguments.length - 1]], a.extend(d, arguments[arguments.length - 1])); this.id = typeof d.id === "undefined" ? this.id = a.uniqueId() : d.id; this.args = d; if (a.core.items[this.id] instanceof a.core) a.raiseWarning(this,
"06091847", "param", "", Error('A FusionChart oject with the specified id "' + this.id + '" already exists. Renaming it to ' + (this.id = a.uniqueId()))); if (d.type && d.type.toString) { if (!a.renderer.userSetDefault && (f || b)) d.renderer = d.renderer || "javascript"; d.swfUrl = (a.core.options.swfSrcPath || d.swfSrcPath || a.core.options.scriptBaseUri).replace(/\/\s*$/g, "") + "/" + d.type + ".swf" } a.parsePolicies(this, a.policies, d); this.attributes.id = this.id; this.resizeTo(d.width, d.height, !0); a.raiseEvent("BeforeInitialize", d, this);
    a.core.items[this.id] = this; a.raiseEvent("Initialized", d, this); return this
}; a.core.prototype = {}; a.core.prototype.constructor = a.core; a.extend(a.core, { id: "FusionCharts", version: [3, 2, 3, "sr3", 5322], items: {}, options: { sensePreferredRenderer: !0 }, getObjectReference: function (c) { return a.core.items[c].ref } }, !1); window.FusionCharts = a.core
    } 
})();
(function () {
    var a = FusionCharts(["private", "EventManager"]); if (a !== void 0) {
        window.FusionChartsEvents = { BeforeInitialize: "beforeinitialize", Initialized: "initialized", Loaded: "loaded", BeforeRender: "beforerender", Rendered: "rendered", DataLoadRequested: "dataloadrequested", DataLoadRequestCancelled: "dataloadrequestcancelled", DataLoadRequestCompleted: "dataloadrequestcompleted", BeforeDataUpdate: "beforedataupdate", DataUpdateCancelled: "dataupdatecancelled", DataUpdated: "dataupdated", DataLoadCancelled: "dataloadcancelled",
            DataLoaded: "dataloaded", DataLoadError: "dataloaderror", NoDataToDisplay: "nodatatodisplay", DataXMLInvalid: "dataxmlinvalid", InvalidDataError: "invaliddataerror", DrawComplete: "drawcomplete", Resized: "resized", BeforeDispose: "beforedispose", Disposed: "disposed"
        }; var i = function (a, b, e, c) { try { a[0].call(b, e, c || {}) } catch (d) { setTimeout(function () { throw d; }, 0) } }, h = function (f, b, e) {
            if (f instanceof Array) for (var c = 0, d; c < f.length; c += 1) {
                if (f[c][1] === b.sender || f[c][1] === void 0) d = f[c][1] === b.sender ? b.sender : a.core, i(f[c],
d, b, e); if (b.cancel === !0) break
            } 
        }, j = { listeners: {}, lastEventId: 0, addListener: function (f, b, e) {
            if (f instanceof Array) for (var c = 0; c < f.length; c += 1) j.addListener(f[c], b, e); else typeof f !== "string" ? a.raiseError(this, "03091549", "param", "::EventTarget.addListener", Error("Unspecified Event Type")) : typeof b !== "function" ? a.raiseError(this, "03091550", "param", "::EventTarget.addListener", Error("Invalid Event Listener")) : (f = f.toLowerCase(), j.listeners[f] instanceof Array || (j.listeners[f] = []), j.listeners[f].push([b,
e]))
        }, removeListener: function (f, b, e) {
            var c; if (f instanceof Array) for (c = 0; c < f.length; c += 1) j.removeListener(f[c], b, e); else if (typeof f !== "string") a.raiseError(this, "03091559", "param", "::EventTarget.removeListener", Error("Unspecified Event Type")); else if (typeof b !== "function") a.raiseError(this, "03091560", "param", "::EventTarget.removeListener", Error("Invalid Event Listener")); else if (f = f.toLowerCase(), f = j.listeners[f], f instanceof Array) for (c = 0; c < f.length; c += 1) f[c][0] === b && f[c][1] === e && (f.splice(c, 1),
c -= 1)
        }, triggerEvent: function (f, b, e) { if (typeof f !== "string") a.raiseError(this, "03091602", "param", "::EventTarget.dispatchEvent", Error("Invalid Event Type")); else return f = f.toLowerCase(), b = { eventType: f, eventId: j.lastEventId += 1, sender: b || Error("Orphan Event"), stopPropagation: function () { return (this.cancel = !0) === !1 } }, h(j.listeners[f], b, e), h(j.listeners["*"], b, e), !0 } 
        }; a.raiseEvent = function (a, b, e) { return j.triggerEvent(a, e, b) }; a.addEventListener = function (a, b) { return j.addListener(a, b) }; a.removeEventListener =
function (a, b) { return j.removeListener(a, b) }; a.extend(a.core, { addEventListener: a.addEventListener, removeEventListener: a.removeEventListener }, !1); a.extend(a.core, { addEventListener: function (a, b) { return j.addListener(a, b, this) }, removeEventListener: function (a, b) { return j.removeListener(a, b, this) } }, !0); a.addEventListener("BeforeDispose", function (a) { var b, e; for (b in j.listeners) for (e = 0; e < j.listeners[b].length; e += 1) j.listeners[b][e][1] === a.sender && j.listeners[b].splice(e, 1) })
    } 
})();
(function () {
    var a = FusionCharts(["private", "ErrorHandler"]); if (a !== void 0) {
        var i = { type: "TypeException", range: "ValueRangeException", impl: "NotImplementedException", param: "ParameterException", run: "RuntimeException", comp: "DesignTimeError", undefined: "UnspecifiedException" }, h = function (f, b, e, c, d, o) {
            var h = "#" + b + " " + (f ? f.id : "unknown-source") + c + " " + o + " >> "; d instanceof Error ? (d.name = i[e], d.module = "FusionCharts" + c, d.level = o, d.message = h + d.message, h = d.message, window.setTimeout(function () { throw d; }, 0)) : h += d; b =
{ id: b, nature: i[e], source: "FusionCharts" + c, message: h }; a.raiseEvent(o, b, f); if (typeof window["FC_" + o] === "function") window["FC_" + o](b)
        }; a.raiseError = function (a, b, e, c, d) { h(a, b, e, c, d, "Error") }; a.raiseWarning = function (a, b, e, c, d) { h(a, b, e, c, d, "Warning") }; var j = { outputHelpers: { text: function (a, b) { j.outputTo("#" + a.eventId + " [" + (a.sender.id || a.sender).toString() + '] fired "' + a.eventType + '" event. ' + (a.eventType === "error" || a.eventType === "warning" ? b.message : "")) }, event: function (a, b) { this.outputTo(a, b) }, verbose: function (a,
b) { j.outputTo(a.eventId, a.sender.id, a.eventType, b) } 
        }, outputHandler: function (f, b) { typeof j.outputTo !== "function" ? a.core.debugMode.outputFailed = !0 : (a.core.debugMode.outputFailed = !1, j.currentOutputHelper(f, b)) }, currentOutputHelper: void 0, outputTo: void 0, enabled: !1
        }; j.currentOutputHelper = j.outputHelpers.text; a.extend(a.core, { debugMode: { syncStateWithCharts: !0, outputFormat: function (a) {
            if (a && typeof a.toLowerCase === "function" && typeof j.outputHelpers[a = a.toLowerCase()] === "function") return j.currentOutputHelper =
j.outputHelpers[a], !0; return !1
        }, outputTo: function (f) { typeof f === "function" ? j.outputTo = f : f === null && (a.core.debugMode.enabled(!1), delete j.outputTo) }, enabled: function (f, b, e) {
            var c; if (typeof f === "object" && arguments.length === 1) c = f, f = c.state, b = c.outputTo, e = c.outputFormat; if (typeof f === "function") { if (typeof b === "string" && (arguments.length === 2 || c)) e = b; b = f; f = !0 } if (typeof f === "boolean" && f !== j.enabled) a.core[(j.enabled = f) ? "addEventListener" : "removeEventListener"]("*", j.outputHandler); if (typeof b === "function") j.outputTo =
b; a.core.debugMode.outputFormat(e); return j.enabled
        }, _enableFirebugLite: function () { window.console && window.console.firebug ? a.core.debugMode.enabled(console.log, "verbose") : a.loadScript("firebug-lite.js", function () { a.core.debugMode.enabled(console.log, "verbose") }, "{ startOpened: true }") } 
        }
        }, !1)
    } 
})();
(function () {
    var a = FusionCharts(["private", "modules.mantle.ajax"]); if (a) {
        var i = window, h = parseFloat(navigator.appVersion.split("MSIE")[1]), j = h >= 5.5 && h <= 7 ? !0 : !1, f = i.location.protocol === "file:", b = i.ActiveXObject, e = (!b || !f) && i.XMLHttpRequest, c = function () { var a; if (e) return c = function () { return new e }, c(); try { a = new b("Msxml2.XMLHTTP"), c = function () { return new b("Msxml2.XMLHTTP") } } catch (o) { try { a = new b("Microsoft.XMLHTTP"), c = function () { return new b("Microsoft.XMLHTTP") } } catch (f) { a = !1 } } return a }, i = a.ajax = function (a,
c) { this.onSuccess = a; this.onError = c }; i.prototype.headers = { "If-Modified-Since": "Sat, 29 Oct 1994 19:43:31 GMT", "X-Requested-With": "XMLHttpRequest", "X-Requested-By": "FusionCharts", Accept: "text/plain, */*" }; i.prototype.get = function (d, b) {
    var e = this, g = e.xmlhttp, m = e.headers, v = e.onError, h = e.onSuccess, i; if (!g || j) g = c(), e.xmlhttp = g; g.onreadystatechange = function () {
        try {
            g.readyState === 4 && h && (!g.status && f || g.status >= 200 && g.status < 300 || g.status === 304 || g.status === 1223 || g.status === 0 ? h(g.responseText, e, b, d) : v && v(Error("XmlHttprequest Error"),
e, b, d))
        } catch (a) { v && v(a, e, b, d) } 
    }; try { g.overrideMimeType && g.overrideMimeType("text/plain"); g.open("GET", d, !0); for (i in m) g.setRequestHeader(i, m[i]); g.send(null) } catch (q) { a.raiseError(a.core, "1110111515A", "run", "XmlHttprequest Error", q.message) } return g
}; i.prototype.abort = function () { var a = this.xmlhttp; return a && typeof a.abort === "function" && a.readyState && a.readyState !== 0 && a.abort() } 
    } 
})();
(function () {
    var a = FusionCharts(["private", "modules.mantle.runtime;1.1"]); if (a !== void 0) {
        var i = /(^|[\/\\])(fusioncharts\.js|fusioncharts\.debug\.js|fusioncharts\.core\.js|fusioncharts\.min\.js|fusioncharts\.packed\.js)([\?#].*)?$/ig; a.core.options.scriptBaseUri = function () {
            var d = document.getElementsByTagName("script"), c = d.length, b, g; for (g = 0; g < c; g += 1) if (b = d[g].getAttribute("src"), !(b === void 0 || b === null || b.match(i) === null)) return b.replace(i, "$1"); a.raiseError(FusionCharts, "1603111624", "run", ">GenericRuntime~scriptBaseUri",
"Unable to locate FusionCharts script source location (URL)."); return ""
        } (); var h = /[\\\"<>;&]/, j = /^[^\S]*?(sf|f|ht)(tp|tps):\/\//ig, f = FusionChartsEvents.ExternalResourceLoad = "externalresourceload", b = {}, e = {}, c = {}; a.isXSSSafe = function (a, d) { if (d && j.exec(a) !== null) return !1; return h.exec(a) === null }; a.loadScript = function (d, g, o, h) {
            if (!d) return !1; var p = g && g.success || g, j = g && g.failure, i, u = { type: "script", success: !1 }; i = a.core.options.scriptBaseUri + d; a.isXSSSafe(i, !0) || (i = typeof window.encodeURIComponent === "function" ?
window.encodeURIComponent(i) : window.escape(i)); u.path = a.core.options.scriptBaseUri; u.src = i; u.file = d; if (e[i] === !0 && h) return u.success = !0, u.notReloaded = !0, typeof g === "function" && (g(), a.raiseEvent(f, u, a.core)), !0; if (b[i] && h) return !1; b[i] = !0; g = document.createElement("script"); g.type = "text/javascript"; g.src = i; o && (g.innerHTML = o); if (typeof p === "function") e[i] = !1, clearTimeout(c[i]), g.onload = function () { e[i] = !0; u.success = !0; clearTimeout(c[i]); p(d, i); a.raiseEvent(f, u, a.core) }, g.onreadystatechange = function () {
    if (this.readyState ===
"complete" || this.readyState === "loaded") e[i] = !0, u.success = !0, clearTimeout(c[i]), p(d, i), a.raiseEvent(f, u, a.core)
}; document.getElementsByTagName("head")[0].appendChild(g); typeof j === "function" && (c[i] = setTimeout(function () { e[i] || (j(d, i), a.raiseEvent(f, u, a.core)) }, a.core.options.html5ResourceLoadTimeout || 3E4)); return !0
        }; var d = a.purgeDOM = function (a) { var c = a.attributes, g, b; if (c) for (g = c.length - 1; g >= 0; g -= 1) b = c[g].name, typeof a[b] === "function" && (a[b] = null); if (c = a.childNodes) { c = c.length; for (g = 0; g < c; g += 1) d(a.childNodes[g]) } },
o = function (a, d, c) { for (var g in a) { var b; if (a[g] instanceof Array) d[a[g][0]] = c[g]; else for (b in a[g]) d[a[g][b][0]] = c[g][b] } }, p = /[^\%\d]*$/ig, g = /^(FusionCharts|FusionWidgets|FusionMaps)/; a.extend(a.core, { dispose: function () { a.raiseEvent("BeforeDispose", {}, this); a.renderer.dispose(this); delete a.core.items[this.id]; a.raiseEvent("Disposed", {}, this); for (var d in this) delete this[d] }, clone: function (d, c) {
    var g = typeof d, b = {}, e = a.extend({}, this.args, !1, !1); o(a.policies, e, this); o(a.renderer.getRendererPolicy(this.options.renderer),
e, this); delete e.id; delete e.animate; delete e.stallLoad; b.link = e.link; e = a.extend({}, e, !1, !1); e.link = b.link; switch (g) { case "object": a.extend(e, d); break; case "boolean": c = d } return c ? e : new a.core(e)
}, isActive: function () { if (!this.ref || document.getElementById(this.id) !== this.ref || typeof this.ref.signature !== "function") return !1; try { return g.test(this.ref.signature()) } catch (a) { return !1 } }, resizeTo: function (d, c, g) {
    var b = { width: d, height: c }; if (typeof d === "object") b.width = d.width, b.height = d.height, g = c; if (b.width &&
typeof b.width.toString === "function") this.width = b.width.toString().replace(p, ""); if (b.height && typeof b.height.toString === "function") this.height = b.height.toString().replace(p, ""); g !== !0 && a.renderer.resize(this, b)
}, chartType: function (a) { var d = this.src, c; if (a) this.src = a, this.isActive() && this.render(); return (c = (c = d.substring(d.indexOf(".swf"), 0)) ? c : d).substring(c.lastIndexOf("/") + 1).toLowerCase() } 
}, !0); window.getChartFromId = function (d) {
    a.raiseWarning(this, "11133001041", "run", "GenericRuntime~getChartFromId()",
'Use of deprecated "getChartFromId()". Replace with "FusionCharts()" or FusionCharts.items[].'); return a.core.items[d] instanceof a.core ? a.core.items[d].ref : window.swfobject.getObjectById(d)
} 
    } 
})();
(function () {
    var a = FusionCharts(["private", "RendererManager"]); if (a !== void 0) {
        a.policies.options.containerElementId = ["renderAt", void 0]; a.policies.options.renderer = ["renderer", void 0]; a.normalizeCSSDimension = function (a, c, b) {
            var a = a === void 0 ? b.offsetWidth : a, c = c === void 0 ? b.offsetHeight : c, g; b.style.width = a = a.toString ? a.toString() : "0"; b.style.height = c = c.toString ? c.toString() : "0"; if (a.match(/^\s*\d*\.?\d*\%\s*$/) && !a.match(/^\s*0\%\s*$/) && b.offsetWidth === 0) for (; g = b.offsetParent; ) if (g.offsetWidth > 0) {
                a = (g.offsetWidth *
parseFloat(a.match(/\d*/)[0]) / 100).toString(); break
            } if (c.match(/^\s*\d*\.?\d*\%\s*$/) && !c.match(/^\s*0\%\s*$/) && b.offsetHeight <= 20) for (; g = b.offsetParent; ) if (g.offsetHeight > 0) { c = (g.offsetHeight * parseFloat(c.match(/\d*/)[0]) / 100).toString(); break } g = { width: a.replace ? a.replace(/^\s*(\d*\.?\d*)\s*$/ig, "$1px") : a, height: c.replace ? c.replace(/^\s*(\d*\.?\d*)\s*$/ig, "$1px") : c }; b.style.width = g.width; b.style.height = g.height; return g
        }; var i = function () { a.raiseError(this, "25081845", "run", "::RendererManager", Error("No active renderer")) },
h = { undefined: { render: i, remove: i, update: i, resize: i, config: i, policies: {}} }, j = {}, f = a.renderer = { register: function (d, c) { if (!d || typeof d.toString !== "function") throw "#03091436 ~renderer.register() Invalid value for renderer name."; d = d.toString().toLowerCase(); if (h[d] !== void 0) return a.raiseError(a.core, "03091438", "param", "::RendererManager>register", 'Duplicate renderer name specified in "name"'), !1; h[d] = c; return !0 }, userSetDefault: !1, setDefault: function (d) {
    if (!d || typeof d.toString !== "function") return a.raiseError(a.core,
"25081731", "param", "::RendererManager>setDefault", 'Invalid renderer name specified in "name"'), !1; if (h[d = d.toString().toLowerCase()] === void 0) return a.raiseError(a.core, "25081733", "range", "::RendererManager>setDefault", "The specified renderer does not exist."), !1; this.userSetDefault = !1; a.policies.options.renderer = ["renderer", d]; return !0
}, notifyRender: function (d) {
    var c = a.core.items[d && d.id]; (!c || d.success === !1) && a.raiseError(a.core.items[d.id], "25081850", "run", "::RendererManager", Error("There was an error rendering the chart. Enable FusionCharts JS debugMode for more information."));
    if (c.ref = d.ref) if (d.ref.FusionCharts = a.core.items[d.id], c.options.containerBackgroundColor) d.ref.style && (d.ref.style.backgroundColor = c.options.containerBackgroundColor); a.raiseEvent("internal.DOMElementCreated", d, c)
}, protectedMethods: { options: !0, attributes: !0, src: !0, ref: !0, constructor: !0, signature: !0, link: !0 }, getRenderer: function (a) { return h[a] }, getRendererPolicy: function (a) { a = h[a].policies; return typeof a === "object" ? a : {} }, currentRendererName: function () { return a.policies.options.renderer[1] }, update: function (a) {
    j[a.id].update.apply(a,
Array.prototype.slice.call(arguments, 1))
}, render: function (a) { j[a.id].render.apply(a, Array.prototype.slice.call(arguments, 1)) }, remove: function (a) { j[a.id].remove.apply(a, Array.prototype.slice.call(arguments, 1)) }, resize: function (a) { j[a.id].resize.apply(a, Array.prototype.slice.call(arguments, 1)) }, config: function (a) { j[a.id].config.apply(a, Array.prototype.slice.call(arguments, 1)) }, dispose: function (a) { j[a.id].dispose.apply(a, Array.prototype.slice.call(arguments, 1)) } 
}, b = function (d) {
    return function () {
        if (this.ref ===
void 0 || this.ref === null || typeof this.ref[d] !== "function") a.raiseError(this, "25081617", "run", "~" + d + "()", "ExternalInterface call failed. Check whether chart has been rendered."); else return this.ref[d].apply(this.ref, arguments)
    } 
}; a.addEventListener("BeforeInitialize", function (d) {
    var d = d.sender, c; if (typeof d.options.renderer === "string" && h[d.options.renderer.toLowerCase()] === void 0) d.options.renderer = a.policies.options.renderer[1]; d.options.renderer = d.options.renderer.toLowerCase(); j[d.id] = h[d.options.renderer];
    if (j[d.id].initialized !== !0 && typeof j[d.id].init === "function") j[d.id].init(), j[d.id].initialized = !0; a.parsePolicies(d, j[d.id].policies || {}, d.args); for (var b in j[d.id].prototype) d[b] = j[d.id].prototype[b]; for (c in j[d.id].events) d.addEventListener(c, j[d.id].events[c])
}); a.addEventListener("Loaded", function (c) {
    var e = c.sender, c = c.sender.ref; e instanceof a.core && delete e.__state.rendering; if (!(c === void 0 || c === null || typeof c.getExternalInterfaceMethods !== "function")) {
        var f; try {
            f = c.getExternalInterfaceMethods(),
f = typeof f === "string" ? f.split(",") : []
        } catch (g) { f = [], a.raiseError(e, "13111126041", "run", "RendererManager^Loaded", Error("Error while retrieving data from the chart-object." + (g.message && g.message.indexOf("NPObject") >= 0 ? " Possible cross-domain security restriction." : ""))) } for (c = 0; c < f.length; c += 1) e[f[c]] === void 0 && (e[f[c]] = b(f[c]))
    } 
}); var e = function (a, c) { if (typeof a[c] === "function") return function () { return a[c].apply(a, arguments) }; return a[c] }; a.addEventListener("loaded", function (c) {
    var b = c.sender; if (b.ref) {
        var f =
a.renderer.protectedMethods, g = a.renderer.getRenderer(b.options.renderer).protectedMethods, m; for (m in c.sender) if (g && !f[m] && !(g[m] || b.ref[m] !== void 0)) try { b.ref[m] = e(c.sender, m) } catch (h) { } 
    } 
}); var c = function (a, c) { var b = document.getElementById(a), g = c.getAttribute("id"); if (b === null) return !1; if (a === g) return !0; for (var g = c.getElementsByTagName("*"), e = 0; e < g.length; e += 1) if (g[e] === b) return !1; return !0 }; a.extend(a.core, { render: function (b) {
    var e, f; ((e = window[this.id]) && e.FusionCharts && e.FusionCharts === this || (e =
this.ref) && e.FusionCharts && e.FusionCharts === this) && a.renderer.dispose(this); window[this.id] !== void 0 && a.raiseError(this, "25081843", "comp", ".render", Error("#25081843:IECompatibility() Chart Id is same as a JavaScript variable name. Variable naming error. Please use unique name for chart JS variable, chart-id and container id.")); e = document.createElement(this.options.containerElementType || "span"); f = this.options.insertMode.toLowerCase() || "replace"; if (b === void 0) b = this.options.containerElementId; typeof b ===
"string" && (b = document.getElementById(b)); if (b === void 0 || b === null) return a.raiseError(this, "03091456", "run", ".render()", Error("Unable to find the container DOM element.")), this; if (c(this.id, b)) return a.raiseError(this, "05102109", "run", ".render()", Error("A duplicate object already exists with the specific Id: " + this.id)), this; e.setAttribute("id", this.id); if (f !== "append" && f !== "prepend") for (; b.hasChildNodes(); ) b.removeChild(b.firstChild); f === "prepend" && b.firstChild ? b.insertBefore(e, b.firstChild) : b.appendChild(e);
    this.options.containerElement = b; this.options.containerElementId = b.id; if (f = e.style) f.lineHeight = "100%", f.display = "inline-block", f.zoom = "1", f["*DISPLAY"] = "inline"; a.normalizeCSSDimension(this.width, this.height, e); this.__state.rendering = !0; a.raiseEvent("BeforeRender", { container: b, width: this.width, height: this.height }, this); a.renderer.render(this, e, a.renderer.notifyRender); return this
}, remove: function () { a.renderer.remove(this); return this }, configure: function (c, b) {
    var e; c && (typeof c === "string" ? (e = {}, e[c] =
b) : e = c, a.renderer.config(this, e))
} 
}, !0); a.extend(a.core, { setCurrentRenderer: function () { var a = f.setDefault.apply(f, arguments); f.userSetDefault = !0; return a }, getCurrentRenderer: function () { return f.currentRendererName.apply(f, arguments) }, render: function () {
    var c = ["swfUrl", "id", "width", "height", "renderAt", "dataSource", "dataFormat"], b = {}, e; if (arguments[0] instanceof a.core) return arguments[0].render(), arguments[0]; for (e = 0; e < arguments.length && e < c.length; e += 1) b[c[e]] = arguments[e]; typeof arguments[arguments.length -
1] === "object" && (delete b[c[e - 1]], a.extend(b, arguments[arguments.length - 1])); if (b.dataFormat === void 0) b.dataFormat = FusionChartsDataFormats.XMLURL; return (new a.core(b)).render()
} 
}, !1)
    } 
})();
(function () {
    var a = FusionCharts(["private", "DataHandlerManager"]); if (a !== void 0) {
        window.FusionChartsDataFormats = {}; var i = {}, h = {}, j = {}, f = /url$/i, b = function (c, b, e, f) {
            var g = !1, m = e.obj, h = e.format, e = e.silent; a.raiseEvent("DataLoadRequestCompleted", { source: "XmlHttpRequest", url: f, data: c, dataFormat: h, cancelDataLoad: function () { g = !0; b.abort(); this.cancelDataLoad = function () { return !1 }; return !0 }, xmlHttpRequestObject: b.xhr }, m); g !== !0 ? m.setChartData(c, h, e) : a.raiseEvent("DataLoadCancelled", { source: "XmlHttpRequest",
                url: f, dataFormat: h, xmlHttpRequestObject: b.xhr
            }, m)
        }, e = function (c, b, e, f) { e = e.obj; c = { source: "XmlHttpRequest", url: f, xmlHttpRequestObject: b.xhr, error: c, httpStatus: b.xhr && b.xhr.status ? b.xhr.status : -1 }; a.raiseEvent("DataLoadError", c, e); typeof window.FC_DataLoadError === "function" && window.FC_DataLoadError(e.id, c) }; a.policies.options.dataSource = ["dataSource", void 0]; a.policies.options.dataFormat = ["dataFormat", void 0]; a.policies.options.dataConfiguration = ["dataConfiguration", void 0]; a.policies.options.showDataLoadingMessage =
["showDataLoadingMessage", !0]; a.addDataHandler = function (c, b) {
    if (typeof c !== "string" || i[c.toLowerCase()] !== void 0) a.raiseError(a.core, "03091606", "param", "::DataManager.addDataHandler", Error("Invalid Data Handler Name")); else {
        var e = {}, f = c.toLowerCase(); i[f] = b; e["set" + c + "Url"] = function (a) { return this.setChartDataUrl(a, c) }; e["set" + c + "Data"] = function (a, b) { return this.setChartData(a, c, !1, b) }; e["get" + c + "Data"] = function () { return this.getChartData(c) }; window.FusionChartsDataFormats[c] = f; window.FusionChartsDataFormats[c +
"URL"] = f + "URL"; a.extend(a.core, e, !0)
    } 
}; a.addEventListener("BeforeInitialize", function (a) { a = a.sender; h[a.id] = ""; j[a.id] = {}; if (a.options.dataSource !== void 0 && typeof a.options.dataFormat === "string") a.__state.dataSetDuringConstruction = !0, a.setChartData(a.options.dataSource, a.options.dataFormat) }); a.addEventListener("BeforeDispose", function (a) { var b = a.sender; delete h[a.sender.id]; delete j[a.sender.id]; b && b.__state && b.__state.dhmXhrObj && b.__state.dhmXhrObj.abort() }); a.extend(a.core, { setChartDataUrl: function (c,
d, h) {
    if (d === void 0 || d === null || typeof d.toString !== "function") a.raiseError(a.core, "03091609", "param", ".setChartDataUrl", Error("Invalid Data Format")); else {
        var d = d.toString().toLowerCase(), i, g = this, m = g.options && g.options.renderer === "flash" && g.options.useLegacyXMLTransport || !1; f.test(d) ? i = d.slice(0, -3) : (i = d, d += "url"); a.raiseEvent("DataLoadRequested", { source: "XmlHttpRequest", url: c, dataFormat: i, cancelDataLoadRequest: function () {
            m = !0; a.raiseEvent("DataLoadRequestCancelled", { source: "XmlHttpRequest", url: c,
                dataFormat: i
            }, g); try { this.__state && this.__state.dhmXhrObj && this.__state.dhmXhrObj.abort() } catch (b) { } this.cancelDataLoadRequest = function () { return !1 }; return !0
        } 
        }, g); if (m) { if (this.__state && this.__state.dhmXhrObj) try { this.__state.dhmXhrObj.abort() } catch (j) { } } else { this.options.dataSource = c; if (!this.__state.dhmXhrObj) this.__state.dhmXhrObj = new a.ajax(b, e); this.__state.dhmXhrObj.get(typeof window.decodeURIComponent === "function" ? window.decodeURIComponent(c) : window.unescape(c), { obj: this, format: i, silent: h }) } 
    } 
},
    setChartData: function (b, d, e) {
        (d === void 0 || d === null || typeof d.toString !== "function") && a.raiseError(a.core, "03091610", "param", ".setChartData", Error("Invalid Data Format")); var d = d.toString().toLowerCase(), p; if (f.test(d)) this.setChartDataUrl(b, d, e); else {
            this.options.dataSource = b; p = d; this.options.dataFormat = d; var d = i[p], g = !1; if (typeof d === "undefined") a.raiseError(a.core, "03091611", "param", ".setChartData", Error("Data Format not recognized")); else if (d = d.encode(b, this, this.options.dataConfiguration) || {},
d.format = d.dataFormat = p, d.dataSource = b, d.cancelDataUpdate = function () { g = !0; this.cancelDataUpdate = function () { return !1 }; return !0 }, a.raiseEvent("BeforeDataUpdate", d, this), delete d.cancelDataUpdate, g === !0) a.raiseEvent("DataUpdateCancelled", d, this); else {
                h[this.id] = d.data || ""; j[this.id] = {}; if (e !== !0) this.options.safeMode === !0 && this.__state.rendering === !0 && !this.isActive() ? (this.__state.updatePending = d, a.raiseWarning(this, "23091255", "run", "::DataHandler~update", "Renderer update was postponed due to async loading.")) :
(delete this.__state.updatePending, a.renderer.update(this, d)); this.__state.dataReady = void 0; a.raiseEvent("DataUpdated", d, this)
            } 
        } 
    }, getChartData: function (b, d) {
        var c; var e; if (b === void 0 || typeof b.toString !== "function" || (e = i[b = b.toString().toLowerCase()]) === void 0) a.raiseError(this, "25081543", "param", "~getChartData()", Error('Unrecognized data-format specified in "format"')); else return c = typeof j[this.id][b] === "object" ? j[this.id][b] : j[this.id][b] = e.decode(h[this.id], this, this.options.dataConfiguration),
e = c, Boolean(d) === !0 ? e : e.data
    }, dataReady: function () { return this.__state.dataReady } 
}, !0); a.extend(a.core, { transcodeData: function (b, d, e, f, g) {
    if (!d || typeof d.toString !== "function" || !e || typeof e.toString !== "function" || i[e = e.toString().toLowerCase()] === void 0 || i[d = d.toString().toLowerCase()] === void 0) a.raiseError(this, "14090217", "param", "transcodeData()", Error("Unrecognized data-format specified during transcoding.")); else {
        b = i[d].encode(b, this, g); e = i[e].decode(b.data, this, g); if (!(e.error instanceof Error)) e.error =
b.error; return f ? e : e.data
    } 
} 
}, !1); a.addEventListener("Disposed", function (a) { delete j[a.sender.id] }); a.addEventListener("Loaded", function (b) { b = b.sender; b instanceof a.core && b.__state.updatePending !== void 0 && (a.renderer.update(b, b.__state.updatePending), delete b.__state.updatePending) }); a.addEventListener("NoDataToDisplay", function (a) { a.sender.__state.dataReady = !1 })
    } 
})();
var swfobject = window.swfobject = function () {
    function a() { if (!B) { try { var a = n.getElementsByTagName("body")[0].appendChild(n.createElement("span")); a.parentNode.removeChild(a) } catch (b) { return } B = !0; for (var a = E.length, c = 0; c < a; c++) E[c]() } } function i(a) { B ? a() : E[E.length] = a } function h(a) {
        if (typeof w.addEventListener != t) w.addEventListener("load", a, !1); else if (typeof n.addEventListener != t) n.addEventListener("load", a, !1); else if (typeof w.attachEvent != t) v(w, "onload", a); else if (typeof w.onload == "function") {
            var b =
w.onload; w.onload = function () { b(); a() } 
        } else w.onload = a
    } function j() { var a = n.getElementsByTagName("body")[0], b = n.createElement(u); b.setAttribute("type", y); var c = a.appendChild(b); if (c) { var g = 0; (function () { if (typeof c.GetVariable != t) { var d; try { d = c.GetVariable("$version") } catch (e) { } if (d) d = d.split(" ")[1].split(","), k.pv = [parseInt(d[0], 10), parseInt(d[1], 10), parseInt(d[2], 10)] } else if (g < 10) { g++; setTimeout(arguments.callee, 10); return } a.removeChild(b); c = null; f() })() } else f() } function f() {
        var a = z.length; if (a >
0) for (var g = 0; g < a; g++) {
            var f = z[g].id, h = z[g].callbackFn, i = { success: !1, id: f }; if (k.pv[0] > 0) {
                var j = m(f); if (j) if (l(z[g].swfVersion) && !(k.wk && k.wk < 312)) { if (q(f, !0), h) i.success = !0, i.ref = b(f), h(i) } else if (z[g].expressInstall && e()) {
                    i = {}; i.data = z[g].expressInstall; i.width = j.getAttribute("width") || "0"; i.height = j.getAttribute("height") || "0"; if (j.getAttribute("class")) i.styleclass = j.getAttribute("class"); if (j.getAttribute("align")) i.align = j.getAttribute("align"); for (var v = {}, j = j.getElementsByTagName("param"),
o = j.length, p = 0; p < o; p++) j[p].getAttribute("name").toLowerCase() != "movie" && (v[j[p].getAttribute("name")] = j[p].getAttribute("value")); c(i, v, f, h)
                } else d(j), h && h(i)
            } else if (q(f, !0), h) { if ((f = b(f)) && typeof f.SetVariable != t) i.success = !0, i.ref = f; h(i) } 
        } 
    } function b(a) { var b, c = null; if (!document.embeds || !(b = document.embeds[a])) if (!((b = m(a)) && b.nodeName == "OBJECT")) b = window[a]; if (!b) return c; typeof b.SetVariable != t ? c = b : (a = b.getElementsByTagName(u)[0]) && (c = a); return c } function e() {
        return !F && l("6.0.65") && (k.win ||
k.mac) && !(k.wk && k.wk < 312)
    } function c(a, b, c, g) {
        F = !0; I = g || null; K = { success: !1, id: c }; var d = m(c); if (d) {
            d.nodeName == "OBJECT" ? (D = o(d), G = null) : (D = d, G = c); a.id = L; if (typeof a.width == t || !/%$/.test(a.width) && parseInt(a.width, 10) < 310) a.width = "310"; if (typeof a.height == t || !/%$/.test(a.height) && parseInt(a.height, 10) < 137) a.height = "137"; n.title = n.title.slice(0, 47) + " - Flash Player Installation"; g = k.ie && k.win ? "ActiveX" : "PlugIn"; g = "MMredirectURL=" + w.location.toString().replace(/&/g, "%26") + "&MMplayerType=" + g + "&MMdoctitle=" +
n.title; typeof b.flashvars != t ? b.flashvars += "&" + g : b.flashvars = g; if (k.ie && k.win && d.readyState != 4) g = n.createElement("div"), c += "SWFObjectNew", g.setAttribute("id", c), d.parentNode.insertBefore(g, d), d.style.display = "none", function () { d.readyState == 4 ? d.parentNode.removeChild(d) : setTimeout(arguments.callee, 10) } (); p(a, b, c)
        } 
    } function d(a) {
        if (k.ie && k.win && a.readyState != 4) {
            var b = n.createElement("div"); a.parentNode.insertBefore(b, a); b.parentNode.replaceChild(o(a), b); a.style.display = "none"; (function () {
                a.readyState ==
4 ? a.parentNode.removeChild(a) : setTimeout(arguments.callee, 10)
            })()
        } else a.parentNode.replaceChild(o(a), a)
    } function o(a) { var b = n.createElement("div"); if (k.win && k.ie) b.innerHTML = a.innerHTML; else if (a = a.getElementsByTagName(u)[0]) if (a = a.childNodes) for (var c = a.length, g = 0; g < c; g++) !(a[g].nodeType == 1 && a[g].nodeName == "PARAM") && a[g].nodeType != 8 && b.appendChild(a[g].cloneNode(!0)); return b } function p(a, b, c) {
        var g, c = m(c); if (k.wk && k.wk < 312) return g; if (c) {
            if (typeof a.id == t) a.id = c.id; if (k.ie && k.win) {
                var d = "", e; for (e in a) if (a[e] !=
Object.prototype[e]) e.toLowerCase() == "data" ? b.movie = a[e] : e.toLowerCase() == "styleclass" ? d += ' class="' + a[e] + '"' : e.toLowerCase() != "classid" && (d += " " + e + '="' + a[e] + '"'); e = ""; for (var f in b) b[f] != Object.prototype[f] && (e += '<param name="' + f + '" value="' + b[f] + '" />'); c.outerHTML = '<object classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000"' + d + ">" + e + "</object>"; H[H.length] = a.id; g = m(a.id)
            } else {
                f = n.createElement(u); f.setAttribute("type", y); for (var h in a) a[h] != Object.prototype[h] && (h.toLowerCase() == "styleclass" ?
f.setAttribute("class", a[h]) : h.toLowerCase() != "classid" && f.setAttribute(h, a[h])); for (d in b) b[d] != Object.prototype[d] && d.toLowerCase() != "movie" && (a = f, e = d, h = b[d], g = n.createElement("param"), g.setAttribute("name", e), g.setAttribute("value", h), a.appendChild(g)); c.parentNode.replaceChild(f, c); g = f
            } 
        } return g
    } function g(a) {
        var b = m(a); if (b && b.nodeName == "OBJECT") k.ie && k.win ? (b.style.display = "none", function () {
            if (b.readyState == 4) { var c = m(a); if (c) { for (var g in c) typeof c[g] == "function" && (c[g] = null); c.parentNode.removeChild(c) } } else setTimeout(arguments.callee,
10)
        } ()) : b.parentNode.removeChild(b)
    } function m(a) { var b = null; try { b = n.getElementById(a) } catch (c) { } return b } function v(a, b, c) { a.attachEvent(b, c); C[C.length] = [a, b, c] } function l(a) { var b = k.pv, a = a.split("."); a[0] = parseInt(a[0], 10); a[1] = parseInt(a[1], 10) || 0; a[2] = parseInt(a[2], 10) || 0; return b[0] > a[0] || b[0] == a[0] && b[1] > a[1] || b[0] == a[0] && b[1] == a[1] && b[2] >= a[2] ? !0 : !1 } function s(a, b, c, g) {
        if (!k.ie || !k.mac) {
            var d = n.getElementsByTagName("head")[0]; if (d) {
                c = c && typeof c == "string" ? c : "screen"; g && (J = x = null); if (!x || J !=
c) g = n.createElement("style"), g.setAttribute("type", "text/css"), g.setAttribute("media", c), x = d.appendChild(g), k.ie && k.win && typeof n.styleSheets != t && n.styleSheets.length > 0 && (x = n.styleSheets[n.styleSheets.length - 1]), J = c; k.ie && k.win ? x && typeof x.addRule == u && x.addRule(a, b) : x && typeof n.createTextNode != t && x.appendChild(n.createTextNode(a + " {" + b + "}"))
            } 
        } 
    } function q(a, b) { if (M) { var c = b ? "visible" : "hidden"; B && m(a) ? m(a).style.visibility = c : s("#" + a, "visibility:" + c) } } function r(a) {
        return /[\\\"<>\.;]/.exec(a) != null &&
typeof encodeURIComponent != t ? encodeURIComponent(a) : a
    } var t = "undefined", u = "object", y = "application/x-shockwave-flash", L = "SWFObjectExprInst", w = window, n = document, A = navigator, N = !1, E = [function () { N ? j() : f() } ], z = [], H = [], C = [], D, G, I, K, B = !1, F = !1, x, J, M = !0, k = function () {
        var a = typeof n.getElementById != t && typeof n.getElementsByTagName != t && typeof n.createElement != t, b = A.userAgent.toLowerCase(), c = A.platform.toLowerCase(), g = c ? /win/.test(c) : /win/.test(b), c = c ? /mac/.test(c) : /mac/.test(b), b = /webkit/.test(b) ? parseFloat(b.replace(/^.*webkit\/(\d+(\.\d+)?).*$/,
"$1")) : !1, d = ! +"\u000b1", e = [0, 0, 0], f = null; if (typeof A.plugins != t && typeof A.plugins["Shockwave Flash"] == u) { if ((f = A.plugins["Shockwave Flash"].description) && !(typeof A.mimeTypes != t && A.mimeTypes[y] && !A.mimeTypes[y].enabledPlugin)) N = !0, d = !1, f = f.replace(/^.*\s+(\S+\s+\S+$)/, "$1"), e[0] = parseInt(f.replace(/^(.*)\..*$/, "$1"), 10), e[1] = parseInt(f.replace(/^.*\.(.*)\s.*$/, "$1"), 10), e[2] = /[a-zA-Z]/.test(f) ? parseInt(f.replace(/^.*[a-zA-Z]+(.*)$/, "$1"), 10) : 0 } else if (typeof w.ActiveXObject != t) try {
            var m = new ActiveXObject("ShockwaveFlash.ShockwaveFlash");
            if (m) { try { f = m.GetVariable("$version") } catch (h) { } f && (d = !0, f = f.split(" ")[1].split(","), e = [parseInt(f[0], 10), parseInt(f[1], 10), parseInt(f[2], 10)]) } 
        } catch (i) { } return { w3: a, pv: e, wk: b, ie: d, win: g, mac: c}
    } (); (function () {
        k.w3 && ((typeof n.readyState != t && n.readyState == "complete" || typeof n.readyState == t && (n.getElementsByTagName("body")[0] || n.body)) && a(), B || (typeof n.addEventListener != t && n.addEventListener("DOMContentLoaded", a, !1), k.ie && k.win && (n.attachEvent("onreadystatechange", function () {
            n.readyState == "complete" &&
(n.detachEvent("onreadystatechange", arguments.callee), a())
        }), w == top && function () { if (!B) { try { n.documentElement.doScroll("left") } catch (b) { setTimeout(arguments.callee, 0); return } a() } } ()), k.wk && function () { B || (/loaded|complete/.test(n.readyState) ? a() : setTimeout(arguments.callee, 0)) } (), h(a)))
    })(); (function () {
        k.ie && k.win && window.attachEvent("onunload", function () {
            for (var a = C.length, b = 0; b < a; b++) C[b][0].detachEvent(C[b][1], C[b][2]); a = H.length; for (b = 0; b < a; b++) g(H[b]); for (var c in k) k[c] = null; k = null; for (var d in swfobject) swfobject[d] =
null; swfobject = null
        })
    })(); return { FusionChartsModified: !0, registerObject: function (a, b, c, g) { if (k.w3 && a && b) { var d = {}; d.id = a; d.swfVersion = b; d.expressInstall = c; d.callbackFn = g; z[z.length] = d; q(a, !1) } else g && g({ success: !1, id: a }) }, getObjectById: function (a) { if (k.w3) return b(a) }, embedSWF: function (a, b, g, d, f, m, h, j, v, o) {
        var s = { success: !1, id: b }; k.w3 && !(k.wk && k.wk < 312) && a && b && g && d && f ? (q(b, !1), i(function () {
            g += ""; d += ""; var i = {}; if (v && typeof v === u) for (var k in v) i[k] = v[k]; i.data = a; i.width = g; i.height = d; k = {}; if (j && typeof j ===
u) for (var n in j) k[n] = j[n]; if (h && typeof h === u) for (var r in h) typeof k.flashvars != t ? k.flashvars += "&" + r + "=" + h[r] : k.flashvars = r + "=" + h[r]; if (l(f)) n = p(i, k, b), i.id == b && q(b, !0), s.success = !0, s.ref = n; else if (m && e()) { i.data = m; c(i, k, b, o); return } else q(b, !0); o && o(s)
        })) : o && o(s)
    }, switchOffAutoHideShow: function () { M = !1 }, ua: k, getFlashPlayerVersion: function () { return { major: k.pv[0], minor: k.pv[1], release: k.pv[2]} }, hasFlashPlayerVersion: l, createSWF: function (a, b, c) { if (k.w3) return p(a, b, c) }, showExpressInstall: function (a,
b, g, d) { k.w3 && e() && c(a, b, g, d) }, removeSWF: function (a) { k.w3 && g(a) }, createCSS: function (a, b, c, g) { k.w3 && s(a, b, c, g) }, addDomLoadEvent: i, addLoadEvent: h, getQueryParamValue: function (a) { var b = n.location.search || n.location.hash; if (b) { /\?/.test(b) && (b = b.split("?")[1]); if (a == null) return r(b); for (var b = b.split("&"), c = 0; c < b.length; c++) if (b[c].substring(0, b[c].indexOf("=")) == a) return r(b[c].substring(b[c].indexOf("=") + 1)) } return "" }, expressInstallCallback: function () {
    if (F) {
        var a = m(L); if (a && D) {
            a.parentNode.replaceChild(D,
a); if (G && (q(G, !0), k.ie && k.win)) D.style.display = "block"; I && I(K)
        } F = !1
    } 
} 
    }
} ();
(function () {
    var a = FusionCharts(["private", "FlashRenderer"]); if (a !== void 0) {
        try { a.swfobject = window.swfobject, window.swfobject.createCSS("object.FusionCharts:focus, embed.FusionCharts:focus", "outline: none") } catch (i) { } a.core.options.requiredFlashPlayerVersion = "8"; a.core.options.flashInstallerUrl = "http://get.adobe.com/flashplayer/"; a.core.options.installRedirectMessage = "You need Adobe Flash Player 8 (or above) to view the charts on this page. It is a free, lightweight and safe installation from Adobe Systems Incorporated.\n\nWould you like to go to Adobe's website and install Flash Player?"; a.core.hasRequiredFlashVersion =
function (b) { if (typeof b === "undefined") b = a.core.options.requiredFlashPlayerVersion; return window.swfobject ? window.swfobject.hasFlashPlayerVersion(b) : void 0 }; var h = !1, j = function (b, e) {
    if (!(e && e.source === "XmlHttpRequest")) {
        var c = b.sender; if (c.ref && typeof c.ref.dataInvokedOnSWF === "function" && c.ref.dataInvokedOnSWF() && typeof c.ref.getXML === "function") a.raiseWarning(c, "08300116", "run", "::DataHandler~__fusioncharts_vars", "Data was set in UTF unsafe manner"), c.setChartData(window.unescape(b.sender.ref.getXML({ escaped: !0 })),
FusionChartsDataFormats.XML, !0), c.flashVars.dataXML = c.getChartData(FusionChartsDataFormats.XML), delete c.flashVars.dataURL; b.sender.removeEventListener("DataLoaded", j)
    } 
}; window.__fusioncharts_dimension = function () {
    var b = /.*?\%\s*?$/g; return function (e) {
        var c, d; c.__state.flashInvokedDimensionRequest = !0; return !((c = a.core(e)) instanceof a.core && c.ref && (d = c.ref.parentNode)) ? {} : { width: d.offsetWidth * (b.test(c.width) ? parseInt(c.width, 10) / 100 : 1), height: d.offsetHeight * (b.test(c.height) ? parseInt(c.height, 10) /
100 : 1)
        }
    } 
} (); window.__fusioncharts_vars = function (b, e) {
    var c = a.core.items[b]; if (!(c instanceof a.core)) return setTimeout(function () {
        var c; if (c = b !== void 0) {
            var e = window.swfobject.getObjectById(b), f, g, m; c = {}; var h; if (!e && typeof e.tagName !== "string") c = void 0; else {
                if ((f = e.parentNode) && f.tagName && f.tagName.toLowerCase() === "object" && f.parentNode) f = f.parentNode; if (f) {
                    c.renderAt = f; if (!(e.tagName !== "OBJECT" && e.getAttribute && (h = e.getAttribute("flashvars") || "")) && e.hasChildNodes && e.hasChildNodes()) {
                        m = e.childNodes;
                        f = 0; for (e = m.length; f < e; f += 1) if (m[f].tagName === "PARAM" && (g = m[f].getAttribute("name")) && g.toLowerCase() === "flashvars") h = m[f].getAttribute("value") || ""
                    } if (h && typeof h.toString === "function") { h = h.split(/\=|&/g); c.flashVars = {}; f = 0; for (e = h.length; f < e; f += 2) c.flashVars[h[f]] = h[f + 1] } 
                } else c = void 0
            } 
        } c || a.raiseError(a.core, "25081621", "run", "::FlashRenderer", "FusionCharts Flash object is accessing flashVars of non-existent object.")
    }, 0), !1; if (typeof e === "object") {
        if (c.ref && typeof c.ref.dataInvokedOnSWF === "function" &&
c.ref.dataInvokedOnSWF()) { if (e.dataURL !== void 0) c.addEventListener("DataLoaded", j); else if (e.dataXML !== void 0) e.dataXML = window.unescape(e.dataXML); c.__state.flashUpdatedFlashVars = !0 } else delete e.dataURL, delete e.dataXML; a.extend(c.flashVars, e); return !0
    } if (c.__state.dataSetDuringConstruction && c.flashVars.dataXML === void 0 && c.options.dataSource !== void 0 && typeof c.options.dataFormat === "string") c.flashVars.dataXML = c.options.dataSource; c.__state.flashInvokedFlashVarsRequest = !0; return c.flashVars
}; window.__fusioncharts_event =
function (b, e) { setTimeout(function () { a.raiseEvent(b.type, e, a.core.items[b.sender]) }, 0) }; var f = function (b) {
    b = b.sender; if (b.options.renderer === "flash") {
        if (b.width === void 0) b.width = a.renderer.policies.flashVars.chartWidth[1]; if (b.height === void 0) b.height = a.renderer.policies.flashVars.chartHeight[1]; if (b.flashVars.DOMId === void 0) b.flashVars.DOMId = b.id; a.extend(b.flashVars, { registerWithJS: "1", chartWidth: b.width, chartHeight: b.height, InvalidXMLText: "Invalid data." }); if (Boolean(b.options.autoInstallRedirect) ===
!0 && !window.swfobject.hasFlashPlayerVersion(a.core.options.requiredFlashPlayerVersion.toString()) && h === !1 && (h = !0, window.confirm(a.core.options.installRedirectMessage))) window.location.href = a.core.options.flashInstallerUrl; if (b.options.dataFormat === void 0 && b.options.dataSource === void 0) b.options.dataFormat = FusionChartsDataFormats.XMLURL, b.options.dataSource = "Data.xml"
    } 
}; a.renderer.register("flash", { dataFormat: "xml", init: function () { a.addEventListener("BeforeInitialize", f) }, policies: { params: { scaleMode: ["scaleMode",
"noScale"], scale: ["scaleMode", "noScale"], wMode: ["wMode", "opaque"], menu: ["menu", void 0], bgColor: ["bgColor", void 0], allowScriptAccess: ["allowScriptAccess", "always"], quality: ["quality", "best"], swLiveConnect: ["swLiveConnect", void 0], base: ["base", void 0], align: ["align", void 0], salign: ["sAlign", void 0]
}, flashVars: { lang: ["lang", "EN"], debugMode: ["debugMode", void 0], scaleMode: ["scaleMode", "noScale"], animation: ["animate", void 0] }, options: { autoInstallRedirect: ["autoInstallRedirect", !1], useLegacyXMLTransport: ["_useLegacyXMLTransport",
!1]
}
}, render: function (b, e) {
    Boolean(this.flashVars.animation) === !0 && delete this.flashVars.animation; this.src || a.raiseError(this, "03102348", "run", "::FlashRenderer.render", 'Could not find a valid "src" attribute. swfUrl or chart type missing.'); var c = {}, d = this.flashVars.dataXML, f = this.flashVars.dataURL; a.extend(c, this.flashVars); if (this.flashVars.stallLoad === !0) {
        if (this.options.dataFormat === FusionChartsDataFormats.XML) d = this.options.dataSource; if (this.options.dataFormat === FusionChartsDataFormats.XMLURL) f =
this.options.dataSource
    } if (a.core.debugMode.enabled() && a.core.debugMode.syncStateWithCharts && c.debugMode === void 0 && this.options.safeMode) c.debugMode = "1"; this.__state.lastRenderedSrc = this.src; c.dataXML = (typeof window.encodeURIComponent === "function" ? window.encodeURIComponent(d) : window.escape(d)) || ""; c.dataURL = a.isXSSSafe(f) ? f || "" : (typeof window.encodeURIComponent === "function" ? window.encodeURIComponent(f) : window.escape(f)) || ""; if (!window.swfobject || !window.swfobject.embedSWF || !window.swfobject.FusionChartsModified) window.swfobject =
a.swfobject; window.swfobject && window.swfobject.embedSWF ? window.swfobject.embedSWF(this.src, b.id, this.width, this.height, a.core.options.requiredFlashPlayerVersion, void 0, c, this.params, this.attributes, e) : a.raiseError(this, "1113061611", "run", "FlashRenderer~render", Error("Could not find swfobject library or embedSWF API"))
}, update: function (a) {
    var e = this.ref, c = a.data; this.flashVars.dataXML = c; a.error === void 0 ? this.isActive() && typeof e.setDataXML === "function" ? this.src !== this.__state.lastRenderedSrc ? this.render() :
e.setDataXML(c, !1) : (delete this.flashVars.dataURL, delete this.flashVars.animation) : this.isActive() && typeof e.showChartMessage === "function" ? e.showChartMessage("InvalidXMLText") : (this.flashVars.dataXML = "<Invalid" + a.format.toUpperCase() + ">", delete this.flashVars.dataURL, delete this.flashVars.animation)
}, resize: function () {
    this.flashVars.chartWidth = this.width; this.flashVars.chartHeight = this.height; if (this.ref !== void 0) this.ref.width = this.width, this.ref.height = this.height, typeof this.ref.resize === "function" &&
this.ref.resize(this.ref.offsetWidth, this.ref.offsetHeight)
}, config: function (b) { a.extend(this.flashVars, b) }, dispose: function () { var a; window.swfobject.removeSWF(this.id); (a = this.ref) && a.parentNode && a.parentNode.removeChild(a) }, protectedMethods: { flashVars: !0, params: !0, setDataXML: !0, setDataURL: !0, hasRendered: !0, getXML: !0, getDataAsCSV: !0, print: !0, exportChart: !0 }, events: { Loaded: function (a) { a.sender.flashVars.animation = "0" }, DataLoadRequested: function (b, e) {
    var c = b.sender, d = e.url, f = !1; if (e.dataFormat === FusionChartsDataFormats.XML &&
(window.location.protocol === "file:" && Boolean(c.options.safeMode) || Boolean(c.options.useLegacyXMLTransport))) c.ref ? c.ref.setDataURL ? c.ref.setDataURL(d, !1) : a.raiseError(this, "0109112330", "run", ">FlashRenderer^DataLoadRequested", Error("Unable to fetch URL due to security restriction on Flash Player. Update global security settings.")) : c.flashVars.dataURL = d, b.stopPropagation(), f = !0, e.cancelDataLoadRequest(), c.addEventListener("DataLoaded", j); if (c.ref && c.showChartMessage) delete c.flashVars.stallLoad, c.options.showDataLoadingMessage &&
c.ref.showChartMessage("XMLLoadingText"); else if (!f) c.flashVars.stallLoad = !0
}, DataLoadRequestCancelled: function (a) { a = a.sender; a.ref && typeof a.showChartMessage === "function" && a.ref.showChartMessage(); delete a.flashVars.stallLoad }, DataLoadError: function (a, e) { var c = a.sender; c.ref && typeof c.ref.showChartMessage === "function" && e.source === "XmlHttpRequest" ? c.ref.showChartMessage("LoadDataErrorText") : (delete c.flashVars.dataURL, c.flashVars.dataXML = "<JSON parsing error>", delete c.flashVars.stallLoad) }, DataLoadRequestCompleted: function (a,
e) { e.source === "XmlHttpRequest" && delete a.sender.flashVars.stallLoad } 
}, prototype: { getSWFHTML: function () {
    var a = document.createElement("span"), e = document.createElement("span"), c = "RnVzaW9uQ2hhcnRz" + (new Date).getTime(); a.appendChild(e); e.setAttribute("id", c); a.style.display = "none"; document.getElementsByTagName("body")[0].appendChild(a); window.swfobject.embedSWF(this.src, c, this.width, this.height, "8.0.0", void 0, this.flashVars, this.params, this.attrs); e = a.innerHTML.replace(c, this.id); window.swfobject.removeSWF(c);
    a.parentNode.removeChild(a); return e
}, setTransparent: function (a) { typeof a !== "boolean" && a !== null && (a = !0); this.params.wMode = a === null ? "window" : a === !0 ? "transparent" : "opaque" }, registerObject: function () { }, addVariable: function () { a.raiseWarning(this, "1012141919", "run", "FlashRenderer~addVariable()", 'Use of deprecated "addVariable()". Replace with "configure()".'); a.core.prototype.configure.apply(this, arguments) }, setDataXML: function (b) {
    a.raiseWarning(this, "11033001081", "run", "GenericRuntime~setDataXML()", 'Use of deprecated "setDataXML()". Replace with "setXMLData()".');
    b === void 0 || b === null || typeof b.toString !== "function" ? a.raiseError(this, "25081627", "param", "~setDataXML", 'Invalid data type for parameter "xml"') : this.ref === void 0 || this.ref === null || typeof this.ref.setDataXML !== "function" ? this.setChartData(b.toString(), FusionChartsDataFormats.XML) : this.ref.setDataXML(b.toString())
}, setDataURL: function (b) {
    a.raiseWarning(this, "11033001082", "run", "GenericRuntime~setDataURL()", 'Use of deprecated "setDataURL()". Replace with "setXMLUrl()".'); b === void 0 || b === null || typeof b.toString !==
"function" ? a.raiseError(this, "25081724", "param", "~setDataURL", 'Invalid data type for parameter "url"') : this.ref === void 0 || this.ref === null || typeof this.ref.setDataURL !== "function" ? this.setChartData(b.toString(), FusionChartsDataFormats.XMLURL) : this.ref.setDataURL(b.toString())
} 
}
}); a.renderer.setDefault("flash")
    } 
})();
(function () {
    var a; a = FusionCharts(["private", "modules.renderer.highcharts"]); if (a !== void 0) {
        a.core.options.jQuerySourceFileName = "jquery.min.js"; var i = function () { }, h = a.hcLib = { cmdQueue: [], moduleCmdQueue: { jquery: [], base: [], charts: [], powercharts: [], widgets: [], maps: []} }, j = h.chartAPI = function () { }, f = h.moduleDependencies = {}, b = h.moduleMeta = { jquery: "jquery.min.js", base: "FusionCharts.HC.js", charts: "FusionCharts.HC.Charts.js", powercharts: "FusionCharts.HC.PowerCharts.js", widgets: "FusionCharts.HC.Widgets.js", maps: "FusionCharts.HC.Maps.js" },
e = {}; h.getDependentModuleName = function (a) { var b, c, d = []; for (b in f) if ((c = f[b][a]) !== void 0) d[c] = b; return d }; var c = h.hasModule = function (b) { var c, d; if (b instanceof Array) { c = 0; for (d = b.length; c < d; c += 1) if (!Boolean(a.modules["modules.renderer.highcharts-" + b]) || b === "jquery" && !Boolean(window.jQuery)) return !1; return !0 } if (b === "jquery") return Boolean(window.jQuery); return Boolean(a.modules["modules.renderer.highcharts-" + b]) }, d = h.loadModule = function (g, d, f, h) {
    g instanceof Array || (g = [g]); var i = g.length, j = 0, o = function () {
        if (j >=
i) d && d(); else { var p = g[j], u = b[p], y; j += 1; if (p) if (c(p)) { o(); return } else { if (e[p]) { a.raiseError(h || a.core, "1112201445A", "run", "JavaScriptRenderer~loadModule() ", "required resources are absent and also blocked from loading."); f && f(p); return } } else f && f(p); y = p === "jquery" ? a.core.options.jQuerySourceFileName : a.core.options["html5" + p + "Src"]; a.loadScript(y == void 0 ? u : y, { success: function () { c(p) ? o() : f && f(p) }, failure: f && function () { f(p) } }, void 0, !0) } 
    }; o()
}, o = h.executeWaitingCommands = function (a) {
    for (var b; b = a.shift(); ) typeof b ===
"object" && i[b.cmd].apply(b.obj, b.args)
}, p = function () { var a = function () { }; a.prototype = { LoadDataErrorText: "Error in loading data.", XMLLoadingText: "Retrieving data. Please wait", InvalidXMLText: "Invalid data.", ChartNoDataText: "No data to display.", ReadingDataText: "Reading data. Please wait", ChartNotSupported: "Chart type not supported.", LoadingText: "Loading chart. Please wait", RenderChartErrorText: "Unable to render chart." }; return a.prototype.constructor = a } (); i.dataFormat = "json"; i.policies = { jsVars: {}, options: { showLoadingMessage: ["showLoadingMessage",
!0]
}
}; i.init = function () { window.jQuery ? c("base") ? i.ready = !0 : d("base", function () { i.ready = !0; o(h.cmdQueue) }, void 0, a.core) : d("jquery", function () { jQuery.noConflict(); if (window.$ === void 0) window.$ = jQuery; i.init() }, void 0, a.core) }; i.render = function (a) {
    var b = this.jsVars.msgStore; if (this.options.showLoadingMessage) a.innerHTML = '<small style="display: inline-block; *zoom:1; *display:inline; width: 100%; font-family: Verdana; font-size: 10px; color: #666666; text-align: center; padding-top: ' + (parseInt(a.style.height,
10) / 2 - 5) + 'px">' + b.LoadingText + "</small>"; h.cmdQueue.push({ cmd: "render", obj: this, args: arguments })
}; i.update = function () { h.cmdQueue.push({ cmd: "update", obj: this, args: arguments }) }; i.resize = function () { h.cmdQueue.push({ cmd: "resize", obj: this, args: arguments }) }; i.dispose = function () { var a = h.cmdQueue, b, c; b = 0; for (c = a.length; b < c; b += 1) a[b].obj === this && (a.splice(b, 1), c -= 1, b -= 1) }; i.config = function () { h.cmdQueue.push({ cmd: "config", obj: this, args: arguments }) }; i.load = function () { h.cmdQueue.push({ cmd: "load", obj: this, args: arguments }) };
        i.protectedMethods = {}; i.events = { BeforeInitialize: function (a) { var a = a.sender, b = a.jsVars, c = this.chartType(); b.fcObj = a; b.msgStore = b.msgStore || new p; b.cfgStore = b.cfgStore || {}; b.drawCount = 0; j[c] || i.load.call(a) }, DataLoadRequested: function (a) {
            var a = a.sender, b = a.jsVars; delete b.loadError; a.ref && a.options.showDataLoadingMessage ? b.hcObj && !b.hasNativeMessage && b.hcObj.showLoading ? b.hcObj.showLoading(b.msgStore.XMLLoadingText) : a.ref.showChartMessage ? a.ref.showChartMessage("XMLLoadingText") : b.stallLoad = !0 : b.stallLoad =
!0
        }, DataLoadRequestCompleted: function (a) { delete a.sender.id.stallLoad }, DataLoadError: function (a) { var a = a.sender, b = a.jsVars; delete b.stallLoad; b.loadError = !0; a.ref && typeof a.ref.showChartMessage === "function" && a.ref.showChartMessage("LoadDataErrorText") } 
        }; a.extend(i.prototype, { getSWFHTML: function () { a.raiseWarning(this, "11090611381", "run", "JavaScriptRenderer~getSWFHTML()", "getSWFHTML() is not supported for JavaScript charts.") }, addVariable: function () {
            a.raiseWarning(this, "11090611381", "run", "JavaScriptRenderer~addVariable()",
'Use of deprecated "addVariable()". Replace with "configure()".'); a.core.prototype.configure.apply(this, arguments)
        }, getXML: function () { a.raiseWarning(this, "11171116291", "run", "JavaScriptRenderer~getXML()", 'Use of deprecated "getXML()". Replace with "getXMLData()".'); return this.getXMLData.apply(this, arguments) }, setDataXML: function () {
            a.raiseWarning(this, "11171116292", "run", "JavaScriptRenderer~setDataXML()", 'Use of deprecated "setDataXML()". Replace with "setXMLData()".'); return this.setXMLData.apply(this,
arguments)
        }, setDataURL: function () { a.raiseWarning(this, "11171116293", "run", "JavaScriptRenderer~setDataURL()", 'Use of deprecated "SetDataURL()". Replace with "setXMLUrl()".'); return this.setXMLUrl.apply(this, arguments) }, hasRendered: function () { return this.jsVars.hcObj && this.jsVars.hcObj.hasRendered }, setTransparent: function (a) { var b; if (b = this.jsVars) typeof a !== "boolean" && a !== null && (a = !0), b.transparent = a === null ? !1 : a === !0 ? !0 : !1 } 
        }); a.extend(a.core, { _fallbackJSChartWhenNoFlash: function () {
            window.swfobject.hasFlashPlayerVersion(a.core.options.requiredFlashPlayerVersion) ||
a.renderer.setDefault("javascript")
        }, _enableJSChartsForSelectedBrowsers: function (b) { b === void 0 || b === null || a.renderer.setDefault(RegExp(b).test(navigator.userAgent) ? "javascript" : "flash") }, _doNotLoadExternalScript: function (a) { var c, d; for (c in a) d = c.toLowerCase(), b[d] && (e[d] = Boolean(a[c])) }, _preloadJSChartModule: function () { throw "NotImplemented()"; } 
        }); a.renderer.register("javascript", i); window.swfobject && window.swfobject.hasFlashPlayerVersion && !window.swfobject.hasFlashPlayerVersion(a.core.options.requiredFlashPlayerVersion) &&
(a.raiseWarning(a.core, "1204111846", "run", "JSRenderer", "Switched to JavaScript as default rendering due to absence of required Flash Player."), a.renderer.setDefault("javascript"))
    } 
})();
(function () {
    var a = FusionCharts(["private", "XMLDataHandler"]); if (a !== void 0) {
        var i = function (a) { return { data: a, error: void 0} }; a.addDataHandler("XML", { encode: i, decode: i }); var h = a._interactiveCharts = { selectscatter: [!0, !1], dragcolumn2d: [!0, !0], dragarea: [!0, !0], dragline: [!0, !0], dragnode: [!0, !0] }, j = function (a) { if (a === !1 && typeof this.constructor.prototype.getXMLData === "function") return this.constructor.prototype.getXMLData.apply(this); return this.ref.getXMLData() }; a.addEventListener("Loaded", function (a) {
            a =
a.sender; if (a.chartType && h[a.chartType()] && h[a.chartType()][0] && a.options && a.options.renderer === "flash") a.getXMLData = j
        })
    } 
})(); var JSON; JSON || (JSON = {});
(function () {
    function a(a) { return a < 10 ? "0" + a : a } function i(a) { f.lastIndex = 0; return f.test(a) ? '"' + a.replace(f, function (a) { var b = c[a]; return typeof b === "string" ? b : "\\u" + ("0000" + a.charCodeAt(0).toString(16)).slice(-4) }) + '"' : '"' + a + '"' } function h(a, c) {
        var g, f, j, l, s = b, q, r = c[a]; r && typeof r === "object" && typeof r.toJSON === "function" && (r = r.toJSON(a)); typeof d === "function" && (r = d.call(c, a, r)); switch (typeof r) {
            case "string": return i(r); case "number": return isFinite(r) ? String(r) : "null"; case "boolean": case "null": return String(r);
            case "object": if (!r) return "null"; b += e; q = []; if (Object.prototype.toString.apply(r) === "[object Array]") { l = r.length; for (g = 0; g < l; g += 1) q[g] = h(g, r) || "null"; j = q.length === 0 ? "[]" : b ? "[\n" + b + q.join(",\n" + b) + "\n" + s + "]" : "[" + q.join(",") + "]"; b = s; return j } if (d && typeof d === "object") { l = d.length; for (g = 0; g < l; g += 1) typeof d[g] === "string" && (f = d[g], (j = h(f, r)) && q.push(i(f) + (b ? ": " : ":") + j)) } else for (f in r) Object.prototype.hasOwnProperty.call(r, f) && (j = h(f, r)) && q.push(i(f) + (b ? ": " : ":") + j); j = q.length === 0 ? "{}" : b ? "{\n" + b + q.join(",\n" +
b) + "\n" + s + "}" : "{" + q.join(",") + "}"; b = s; return j
        } 
    } if (typeof Date.prototype.toJSON !== "function") Date.prototype.toJSON = function () { return isFinite(this.valueOf()) ? this.getUTCFullYear() + "-" + a(this.getUTCMonth() + 1) + "-" + a(this.getUTCDate()) + "T" + a(this.getUTCHours()) + ":" + a(this.getUTCMinutes()) + ":" + a(this.getUTCSeconds()) + "Z" : null }, String.prototype.toJSON = Number.prototype.toJSON = Boolean.prototype.toJSON = function () { return this.valueOf() }; var j = /[\u0000\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,
f = /[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g, b, e, c = { "\u0008": "\\b", "\t": "\\t", "\n": "\\n", "\u000c": "\\f", "\r": "\\r", '"': '\\"', "\\": "\\\\" }, d; if (typeof JSON.stringify !== "function") JSON.stringify = function (a, c, g) {
    var f; e = b = ""; if (typeof g === "number") for (f = 0; f < g; f += 1) e += " "; else typeof g === "string" && (e = g); if ((d = c) && typeof c !== "function" && (typeof c !== "object" || typeof c.length !== "number")) throw Error("JSON.stringify"); return h("",
{ "": a })
}; if (typeof JSON.parse !== "function") JSON.parse = function (a, b) {
    function c(a, d) { var e, f, h = a[d]; if (h && typeof h === "object") for (e in h) Object.prototype.hasOwnProperty.call(h, e) && (f = c(h, e), f !== void 0 ? h[e] = f : delete h[e]); return b.call(a, d, h) } var d, a = String(a); j.lastIndex = 0; j.test(a) && (a = a.replace(j, function (a) { return "\\u" + ("0000" + a.charCodeAt(0).toString(16)).slice(-4) })); if (/^[\],:{}\s]*$/.test(a.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, "@").replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g,
"]").replace(/(?:^|:|,)(?:\s*\[)+/g, ""))) return d = eval("(" + a + ")"), typeof b === "function" ? c({ "": d }, "") : d; throw new SyntaxError("JSON.parse");
} 
})();
(function () {
    var a = FusionCharts(["private", "JSON_DataHandler"]); if (a !== void 0) {
        window.JSON === void 0 && a.raiseError(this, "1113062012", "run", "JSONDataHandler", Error("Could not find library support for JSON parsing.")); a.core.options.allowIESafeXMLParsing = ["_allowIESafeXMLParsing", !0]; var i = function (a) { if (a === null || a === void 0 || typeof a.toString !== "function") return ""; return a = a.toString().replace(/&/g, "&amp;").replace(/\'/g, "&#39;").replace(/\"/g, "&quot;").replace(/</g, "&lt;").replace(/>/g, "&gt;") }, h = function () {
            var f =
{ arr: { set: !0, trendlines: !0, vtrendlines: !0, line: { trendlines: !0, vtrendlines: !0 }, data: !0, dataset: !0, lineset: !0, categories: !0, category: !0, linkeddata: !0, application: !0, definition: !0, axis: !0, connectors: !0, connector: { connectors: !0 }, trendset: !0, row: { rows: !0 }, column: { columns: !0 }, label: { labels: !0 }, color: { colorrange: !0 }, dial: { dials: !0 }, pointer: { pointers: !0 }, point: { trendpoints: !0 }, process: { processes: !0 }, task: { tasks: !0 }, milestone: { milestones: !0 }, datacolumn: { datatable: !0 }, text: { datacolumn: !0 }, alert: { alerts: !0 },
    groups: { annotations: !0 }, items: { groups: !0}
}, tag: { chart: "linkedchart", map: "linkedmap", graph: "linkedgraph", set: "data", vline: { chart: "data", graph: "data", dataset: "data", categories: "category", linkedchart: "data", linkedgraph: "data" }, apply: { application: "application" }, style: { definition: "definition" }, annotationgroup: { annotations: "groups" }, annotation: { groups: "items"} }, attr: { vline: { vline: "true"} }, ins: { chart: !0, map: !0, graph: !0 }, dsv: { dataset: "data", categories: "category" }, text: { target: "target", value: "value" }, group: { styles: { definition: !0,
    application: !0
}, chart: { value: !0, target: !0}
}
}, b = { append: function (a, b, e, h) { f.arr[e] && (f.arr[e] === !0 || f.arr[e][h] === !0) ? (b[e] instanceof Array || (b[e] = []), b[e].push(a)) : b[e] = a }, child: function (c, d, e, h) {
    var g, i, j, l; for (g = 0; g < d.length; g += 1) switch (i = d[g].nodeName.toLowerCase(), d[g].nodeType) {
        case 1: j = b.attr(d[g].attributes); l = f.ins[i]; l === !0 && (l = j, j = {}, j[i] = l); l = f.attr[i]; typeof l === "object" && a.extend(j, l); if (l = f.tag[i]) typeof l === "object" && typeof l[e] === "string" && (i = l[e]), typeof l === "string" && (i = l); d[g].childNodes.length &&
((l = f.group[e]) && l[i] ? b.child(c, d[g].childNodes, i, h) : b.child(j, d[g].childNodes, i, h)); l = f.group[e]; (!l || !l[i]) && b.append(j, c, i, e); break; case 3: if (l = f.text[e]) i = l, j = d[g].data, b.append(j, c, i, e); l = f.dsv[e]; if (typeof l === "string" && h.chart && parseInt(h.chart.compactdatamode, 10)) i = l, j = d[g].data, c[i] = c[i] ? c[i] + j : j
    } 
}, attr: function (a) { var b, e = {}; if (!a || !a.length) return e; for (b = 0; b < a.length; b += 1) e[a[b].nodeName.toLowerCase()] = a[b].nodeValue; return e } 
}, e = function (c) {
    var d = {}, f, h, g; if (typeof c !== "object" && typeof c.toString !==
"function") return e.errorObject = new TypeError("xml2json.parse()"), d; c = c.toString().replace(/<\!--[\s\S]*?--\>/g, "").replace(/<\?xml[\s\S]*?\?>/ig, "").replace(/&(?!([^;\n\r]+?;))/g, "&amp;$1"); c = c.replace(/^\s\s*/, ""); f = /\s/; for (var i = c.length; f.test(c.charAt(i -= 1)); ); c = c.slice(0, i + 1); if (!c) return d; window.DOMParser ? f = (new window.DOMParser).parseFromString(c, "text/xml") : document.body && a.core.options.allowIESafeXMLParsing ? (i = document.createElement("xml"), i.innerHTML = c, document.body.appendChild(i), f =
i.XMLDocument, document.body.removeChild(i)) : (f = new ActiveXObject("Microsoft.XMLDOM"), f.async = "false", f.loadXML(c)); if (!(f.childNodes.length === 1 && (h = f.childNodes[0]) && h.nodeName && (g = h.nodeName.toLowerCase()) && (g === "chart" || g === "map" || g === "graph"))) return e.errorObject = new TypeError("xml2json.parse()"), d; d[g] = b.attr(h.attributes); h.childNodes && b.child(d, h.childNodes, g, d); delete e.errorObject; return d
}; return function (a) { delete e.errorObject; return { data: e(a), error: e.errorObject} } 
        } (), j = function () {
            var a =
{ items: { explode: { data: "set", groups: { annotations: "annotationgroup" }, items: { groups: "annotation"} }, text: { chart: { target: "target", value: "value"} }, dsv: { dataset: { data: "dataset" }, categories: { category: "categories"} }, attr: { chart: { chart: "chart", graph: "chart" }, graph: { graph: "graph", chart: "graph" }, map: { map: "map" }, linkedchart: { chart: "chart", graph: "graph", map: "map"} }, group: { styles: { definition: "style", application: "apply" }, map: { data: "entity", entitydef: "entity" }, markers: { definition: "marker", application: "marker", shapes: "shape",
    connectors: "connector"
}
}
}, qualify: function (a, b, d) { return typeof this.items[a][d] === "object" ? this.items[a][d][b] : this.items[a][d] } 
}, b = function (e, c, d, h) {
    var j = "", g = "", m = "", v = "", l, s, q; c && typeof c.toLowerCase === "function" && (c = c.toLowerCase()); if (d === void 0 && e[c]) for (l in e[c]) if (s = l.toLowerCase(), s === "compactdatamode") h.applyDSV = e[c][l] == 1; if (e instanceof Array) for (l = 0; l < e.length; l += 1) v += typeof e[l] === "string" ? i(e[l]) : b(e[l], c, d, h); else {
        for (l in e) s = l.toLowerCase(), e[l] instanceof Array && (q = a.qualify("group",
s, c)) ? g += "<" + s + ">" + b(e[l], q, c, h) + "</" + s + ">" : typeof e[l] === "object" ? (q = a.qualify("attr", s, c)) ? (m = b(e[l], q, c, h).replace(/\/\>/ig, ""), c = s) : g += b(e[l], s, c, h) : h.applyDSV && (q = a.qualify("dsv", s, c)) ? g += e[l] : (q = a.qualify("text", s, c)) ? g += "<" + q + ">" + e[l] + "</" + q + ">" : s === "vline" && Boolean(e[l]) ? c = "vline" : j += " " + s + '="' + i(e[l]).toString().replace(/\"/ig, "&quot;") + '"'; if (q = a.qualify("explode", d, c)) c = q; v = (m !== "" ? m : "<" + c) + j + (g !== "" ? ">" + g + "</" + c + ">" : " />")
    } return v
}; return function (a) {
    delete b.errorObject; if (a && typeof a ===
"string") try { a = JSON.parse(a) } catch (c) { b.errorObject = c } return { data: b(a, a && a.graph ? "graph" : a && a.map ? "map" : "chart", void 0, {}), error: b.errorObject}
} 
        } (); a.addDataHandler("JSON", { encode: j, decode: h })
    } 
})();
(function () {
    var a = FusionCharts(["private", "CSVDataHandler"]); if (a !== void 0) {
        var i = function (a) { this._data = []; this.columnCount = this.rowCount = 0; this.configure(a) }; i.prototype.set = function (a, i, f) { var b; if (this.rowCount <= a) { for (b = this.rowCount; b <= a; b += 1) this._data[b] = []; this.rowCount = a + 1 } if (this.columnCount <= i) this.columnCount = i + 1; this._data[a][i] = f }; i.prototype.configure = function (a) {
            this.delimiter = this._decodePseudoCode(a.delimiter, ","); this.qualifier = this._decodePseudoCode(a.qualifier, '"'); this.eolCharacter =
this._decodePseudoCode(a.eolCharacter, "\r\n")
        }; i.prototype._decodePseudoCode = function (a, i) { if (a === void 0 || a === null || !a.toString) return i; return a.replace("{tab}", "\t").replace("{quot}", '"').replace("{apos}", "'") }; i.prototype.toString = function () { var a, i, f = ""; for (a = 0; a < this.rowCount; a += 1) i = this.qualifier + this._data[a].join(this.qualifier + this.delimiter + this.qualifier) + this.qualifier, f += i === '""' ? this.eolCharacter : i + this.eolCharacter; this.rowCount > 0 && (f = f.slice(0, f.length - 2)); return f }; a.addDataHandler("CSV",
{ encode: function (h, i) { a.raiseError(i, "0604111215A", "run", "::CSVDataHandler.encode()", "FusionCharts CSV data-handler only supports encoding of data."); throw "FeatureNotSupportedException()"; }, decode: function (h) {
    var h = a.core.transcodeData(h, "xml", "json") || {}, j, f, b, e, c, d; if (typeof h.chart !== "object") h.chart = {}; j = new i({ separator: h.chart.exportdataseparator, qualifier: h.chart.exportdataqualifier }); if (h.dataset && h.categories && h.categories[0] && h.categories[0].category) for (f = 0; f < h.dataset.length; f += 1) {
        j.set(0,
f + 1, h.dataset[f].seriesname); e = b = 0; for (c = h.categories[0].category.length; b < c; b += 1) h.categories[0].category[b].vline || (j.set(e + 1, 0, h.categories[0].category[b].label || h.categories[0].category[b].name), d = parseFloat(h.dataset[f] && h.dataset[f].data && h.dataset[f].data[e] ? h.dataset[f].data[e].value : ""), d = isNaN(d) ? "" : d, j.set(e + 1, f + 1, d), e += 1)
    } else if (h.data instanceof Array) {
        j.set(0, 1, h.chart && h.chart.yaxisname ? h.chart.yaxisname : "Value"); c = h.data.length; for (f = 0; f < c; f += 1) h.data[f].vline || (j.set(f + 1, 0, h.data[f].label ||
h.data[f].name), d = parseFloat(h.data[f].value ? h.data[f].value : ""), d = isNaN(d) ? "" : d, j.set(f + 1, 1, d))
    } j.rowCount > 0 && j.set(0, 0, h.chart && h.chart.xaxisname ? h.chart.xaxisname : "Label"); return { data: j.toString(), error: void 0}
} 
}); a.core.addEventListener("Loaded", function (a) { a = a.sender; if (a.options.renderer === "javascript") a.getDataAsCSV = a.ref.getDataAsCSV = a.getCSVData })
    } 
})();
(function () {
    var a = FusionCharts(["private", "DynamicChartAttributes"]); a !== void 0 && a.extend(a.core, { setChartAttribute: function (a, h) { if (typeof a === "string") { var j = a, a = {}; a[j] = h } else if (a === null || typeof a !== "object") return; var j = 0, f = this.getChartData(FusionChartsDataFormats.JSON), b, e = f.chart || f.graph || {}; for (b in a) j += 1, a[b] === null ? delete e[b.toLowerCase()] : e[b.toLowerCase()] = a[b]; if (j > 0) { if (typeof e.animation === "undefined") e.animation = "0"; this.setChartData(f, FusionChartsDataFormats.JSON) } }, getChartAttribute: function (i) {
        var h =
(h = this.getChartData(FusionChartsDataFormats.JSON)).chart || h.graph; if (arguments.length === 0 || i === void 0 || h === void 0) return h; var j, f; if (typeof i === "string") j = h[i.toString().toLowerCase()]; else if (i instanceof Array) { j = {}; for (f = 0; f < i.length; f += 1) j[i[f]] = h[i[f].toString().toLowerCase()] } else a.raiseError(this, "25081429", "param", "~getChartAttribute()", 'Unexpected value of "attribute"'); return j
    } 
    }, !0)
})();
(function () {
    var a = FusionCharts(["private", "api.LinkManager"]); if (a !== void 0) {
        a.policies.link = ["link", void 0]; var i = window.FusionChartsDOMInsertModes = { REPLACE: "replace", APPEND: "append", PREPEND: "prepend" }, h = {}, j = function (b, e) { this.items = {}; this.root = b; this.parent = e; e instanceof a.core ? this.level = this.parent.link.level + 1 : (h[b.id] = [{}], this.level = 0) }, f = function (a, e) {
            return (a.options.containerElement === e.options.containerElement || a.options.containerElementId === e.options.containerElementId) && a.options.insertMode ===
i.REPLACE
        }; j.prototype.configuration = function () { return h[this.root.id][this.level] || (h[this.root.id][this.level] = {}) }; a.extend(a.core, { configureLink: function (b, e) {
            var c; if (b instanceof Array) { for (c = 0; c < b.length; c += 1) typeof h[this.link.root.id][c] !== "object" && (h[this.link.root.id][c] = {}), a.extend(h[this.link.root.id][c], b[c]); h[this.link.root.id].splice(b.length) } else if (typeof b === "object") {
                if (typeof e !== "number") e = this.link.level; h[this.link.root.id][e] === void 0 && (h[this.link.root.id][e] = {}); a.extend(h[this.link.root.id][e],
b)
            } else a.raiseError(this, "25081731", "param", "~configureLink()", "Unable to update link configuration from set parameters")
        } 
        }, !0); a.addEventListener("BeforeInitialize", function (b) { if (b.sender.link instanceof j) { if (b.sender.link.parent instanceof a.core) b.sender.link.parent.link.items[b.sender.id] = b.sender } else b.sender.link = new j(b.sender) }); a.addEventListener("LinkedChartInvoked", function (b, e) {
            var c = b.sender, d = c.clone({ dataSource: e.data, dataFormat: e.linkType, link: new j(c.link.root, c) }, !0); c.args &&
parseInt(c.args.animate, 10) !== 0 && delete d.animate; a.extend(d, c.link.configuration()); a.raiseEvent("BeforeLinkedItemOpen", { level: c.link.level }, c.link.root); a.core.items[d.id] instanceof a.core && a.core.items[d.id].dispose(); d = new a.core(d); if (!f(d, c) && (!c.options.overlayButton || !c.options.overlayButton.message)) { if (typeof c.options.overlayButton !== "object") c.options.overlayButton = {}; c.options.overlayButton.message = "Close" } d.render(); a.raiseEvent("LinkedItemOpened", { level: c.link.level, item: d }, c.link.root)
        });
        a.addEventListener("OverlayButtonClick", function (b, e) { if (e.id === "LinkManager") { var c = b.sender, d = c.link.level - 1, h = c.link.parent, i = c.link.root; a.raiseEvent("BeforeLinkedItemClose", { level: d, item: c }, i); setTimeout(function () { a.core.items[c.id] && c.dispose(); a.raiseEvent("LinkedItemClosed", { level: d }, i) }, 0); !h.isActive() && f(c, h) && h.render() } }); a.addEventListener("Loaded", function (b) {
            if ((b = b.sender) && b.link !== void 0 && b.link.root !== b && b.link.parent instanceof a.core) if (b.ref && typeof b.ref.drawOverlayButton ===
"function") { var e = a.extend({ show: !0, id: "LinkManager" }, b.link.parent.options.overlayButton); a.extend(e, b.link.parent.link.configuration().overlayButton || {}); b.ref.drawOverlayButton(e) } else a.raiseWarning(b, "04091602", "run", "::LinkManager^Loaded", "Unable to draw overlay button on object. -" + b.id)
        }); a.addEventListener("BeforeDispose", function (b) { var e = b.sender; e && e.link instanceof j && (e.link.parent instanceof a.core && delete e.link.parent.link.items[b.sender.id], delete h[e.id]) }); FusionChartsEvents.LinkedItemOpened =
"linkeditemopened"; FusionChartsEvents.BeforeLinkedItemOpen = "beforelinkeditemopen"; FusionChartsEvents.LinkedItemClosed = "linkeditemclosed"; FusionChartsEvents.BeforeLinkedItemClose = "beforelinkeditemclose"
    } 
})();
(function () {
    var a = FusionCharts(["private", "PrintManager"]); if (a !== void 0) {
        var i = { enabled: !1, invokeCSS: !0, processPollInterval: 2E3, message: "Chart is being prepared for print.", useExCanvas: !1, bypass: !1 }, h = { getCanvasElementOf: function (b, c, d) {
            if (b.__fusioncharts__canvascreated !== !0) {
                var e = document.createElement("canvas"), f = a.core.items[b.id].attributes["class"]; i.useExCanvas && G_vmlCanvasManager && G_vmlCanvasManager.initElement(e); e.setAttribute("class", f); e.__fusioncharts__reference = b.id; b.parentNode.insertBefore(e,
b.nextSibling); b.__fusioncharts__canvascreated = !0
            } b.nextSibling.setAttribute("width", c || b.offsetWidth || 2); b.nextSibling.setAttribute("height", d || b.offsetHeight || 2); return b.nextSibling
        }, removeCanvasElementOf: function (a) {
            var b = a.ref && a.ref.parentNode ? a.ref.parentNode : a.options.containerElement || window.getElementById(a.options.containerElementId); if (b) {
                var c = b.getElementsByTagName("canvas"), d, e; e = 0; for (d = c.length; e < d; e += 1) if (c[e].__fusioncharts__reference === a.id && (b.removeChild(c[e]), a.ref)) a.ref.__fusioncharts__canvascreated =
!1
            } 
        }, rle2rgba: function (a, b, c) { typeof c !== "string" && (c = "FFFFFF"); var a = a.split(/[;,_]/), d, e, f, h, i, j = 0; for (e = 0; e < a.length; e += 2) { a[e] === "" && (a[e] = c); a[e] = ("000000" + a[e]).substr(-6); f = parseInt("0x" + a[e].substring(0, 2), 16); h = parseInt("0x" + a[e].substring(2, 4), 16); i = parseInt("0x" + a[e].substring(4, 6), 16); for (d = 0; d < a[e + 1]; d += 1) b[j] = f, b[j + 1] = h, b[j + 2] = i, b[j + 3] = 255, j += 4 } return b }, rle2array: function (a, b) {
            typeof b !== "string" && (b = "FFFFFF"); var c = a.split(";"), d, e; for (d in c) {
                c[d] = c[d].split(/[_,]/); for (e = 0; e < c[d].length; e +=
2) c[d][e] = c[d][e] === "" ? b : ("000000" + c[d][e]).substr(-6)
            } return c
        }, drawText: function (b, c, d, e) { b = b.getContext("2d"); d = d || 2; e = e || 2; b.clearRect(0, 0, d, e); b.textBaseline = "middle"; b.textAlign = "center"; b.font = "8pt verdana"; b.fillStyle = "#776666"; typeof b.fillText === "function" ? b.fillText(c, d / 2, e / 2) : typeof b.mozDrawText === "function" ? (b.translate(d / 2, e / 2), b.mozDrawText(c)) : a.raiseWarning(a.core, "25081803", "run", "::PrintManager>lib.drawText", "Canvas text drawing is not supported in browser"); return !0 }, appendCSS: function (a) {
            var b =
document.createElement("style"); b.setAttribute("type", "text/css"); typeof b.styleSheet === "undefined" ? b.appendChild(document.createTextNode(a)) : b.styleSheet.cssText = a; return document.getElementsByTagName("head")[0].appendChild(b)
        } 
        }; h.drawRLE = function (a, b, c, d, e) {
            c = c || 2; d = d || 2; a.setAttribute("width", c); a.setAttribute("height", d); a = a.getContext("2d"); if (typeof a.putImageData === "function" && typeof a.createImageData === "function") c = a.createImageData(c, d), h.rle2rgba(b, c.data, e), a.putImageData(c, 0, 0); else for (e in c =
h.rle2array(b, e), d = e = b = 0, c) for (d = b = 0; d < c[e].length; d += 2) a.fillStyle = "#" + c[e][d], a.fillRect(b, e, c[e][d + 1], 1), b += parseInt(c[e][d + 1], 10); return !0
        }; var j = { styles: { print: "canvas.FusionCharts{display:none;}@media print{object.FusionCharts{display:none;}canvas.FusionCharts{display:block;}}", error: "canvas.FusionCharts{display:none;}", normal: "" }, cssNode: void 0 }, f = {}, b = {}, e = 0, c; j.invoke = function (a) {
            typeof this.styles[a] !== "undefined" && (a = this.styles[a]); if (typeof a !== "undefined") this.cssNode !== void 0 && this.cssNode.parentNode !==
void 0 && this.cssNode.parentNode.removeChild(this.cssNode), j.cssNode = h.appendCSS(a)
        }; var d = function (b) {
            var d = b.sender.ref, o, l; if (d === void 0 || typeof d.prepareImageDataStream !== "function" || d.prepareImageDataStream() === !1) c(b.sender); else {
                f[b.sender.id] || (f[b.sender.id] = d, e += 1, e === 1 && a.raiseEvent("PrintReadyStateChange", { ready: !1, bypass: i.bypass }, b.sender)); try { o = d.offsetWidth, l = d.offsetHeight, h.drawText(h.getCanvasElementOf(d, o, l), i.message, o, l) } catch (p) {
                    j.invoke("error"), a.raiseError(b.sender, "25081807",
"run", "::PrintManager>onDrawComplete", "There was an error while showing message to user via canvas.")
                } 
            } 
        }, o = function (b, c) { try { h.drawRLE(h.getCanvasElementOf(b.sender.ref, c.width, c.height), c.stream, c.width, c.height, c.bgColor) === !0 && f[b.sender.id] && (delete f[b.sender.id], e -= 1, e === 0 && a.raiseEvent("PrintReadyStateChange", { ready: !0, bypass: i.bypass }, b.sender)) } catch (d) { j.invoke("error"), a.raiseError(b.sender, "25081810", "run", "::PrintManager>onImageStreamReady", "There was an error while drawing canvas.") } },
p = function (a) { h.removeCanvasElementOf(a.sender) }; c = function (c) { var e; if (c instanceof a.core) b[c.id] = c; else for (e in b) d({ sender: b[e] }, {}), delete b[e] }; a.extend(a.core, { printManager: { configure: function (b) { a.extend(i, b || {}) }, isReady: function () { if (i.bypass) return !0; if (e > 0 || !i.enabled) return !1; var b, c; for (b in a.core.items) if ((c = a.core.items[b].ref) !== void 0 && c.hasRendered && c.hasRendered() === !1) return !1; return !0 }, enabled: function (b) {
    if (b === void 0) return i.enabled; if (a.renderer.currentRendererName() !==
"flash" || typeof document.createElement("canvas").getContext !== "function") return i.bypass = !0, a.raiseEvent("PrintReadyStateChange", { ready: !0, bypass: i.bypass }), a.raiseWarning(a.core, "25081816", "run", ".printManager.enabled", "printManager is not compatible with your browser"), i.enabled; i.bypass = !1; var e = b ? "addEventListener" : "removeEventListener"; a.core[e]("ImageStreamReady", o); a.core[e]("DrawComplete", d); a.core[e]("BeforeDispose", p); if (b === !0) {
        var f; i.invokeCSS === !0 && j.invoke("print"); for (f in a.core.items) c(a.core.items[f]),
c()
    } else { var l; j.invoke("error"); for (l in a.core.items) h.removeCanvasElementOf(a.core.items[l]); i.bypass || a.raiseEvent("PrintReadyStateChange", { ready: !1, bypass: i.bypass }); j.invoke("normal") } return i.enabled = b
}, managedPrint: function (b) {
    i.bypass ? window.print() : a.core.printManager.isReady() ? typeof b === "object" && b.ready !== !0 || (a.removeEventListener("PrintReadyStateChange", a.core.printManager.managedPrint), window.print()) : a.core.printManager.enabled(!0) !== !0 ? window.print() : a.addEventListener("PrintReadyStateChange",
a.core.printManager.managedPrint)
} 
}
}, !1); FusionChartsEvents.PrintReadyStateChange = "printreadystatechange"
    } 
})();
