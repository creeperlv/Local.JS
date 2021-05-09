/// DEFINE NAME "STDIO"
/// DEFINE AUTHOR "Creeper Lv"
/// DEFINE VERSION 0.0.1.0
/// IFNDEF STDIO
/// DEFINE STDIO
/// ENDIF
/// EXPOSETYPE __stdio__ Local.JS.JSIO.stdio
function getc() {
    return __stdio__.getc();
}
function gets() {
    return __stdio__.gets();
}
function scanf(format) {
    return __stdio__.scanf(format);
}
function print(content) {
    __stdio__.print(content);
}
function printf() {
    var param = new Array();
    for (var i = 1; i < arguments.length; i++) {
        param.push(arguments[i]);
    }
    __stdio__.printf(arguments[0], param);
}
function flushIn() {
    __stdio__.ClearInputBuffer();
}
function fflush() {
    __stdio__.fflush();
}
function printfn() {
    var param = new Array();
    for (var i = 1; i < arguments.length; i++) {
        param.push(arguments[i]);
    }
    __stdio__.printfn(arguments[0], param);
}
__stdio__.SetIn(Console.In);
__stdio__.SetOut(Console.Out);