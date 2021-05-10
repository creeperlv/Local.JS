/// DEFINE NAME "STDIO"
/// DEFINE AUTHOR "Creeper Lv"
/// DEFINE VERSION 0.0.1.0
/// IFNDEF STDIO
/// DEFINE STDIO
/// ENDIF
/// EXPOSETYPE __stdio__ Local.JS.JSIO.stdio

// This file aims on providing a library similar to `stdio.h` in C.
// However, not all functions in `stdio.h` will be implemented here.

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
// Clear input buffer
function flushIn() {
    __stdio__.ClearInputBuffer();
}
// Flush output.
function fflush() {
    __stdio__.fflush();
}
// This function provides .Net style printf which uses string.Format() to perform formatted string output.
// parameter: string:Format ... obj args
function printfn() {
    var param = new Array();
    for (var i = 1; i < arguments.length; i++) {
        param.push(arguments[i]);
    }
    __stdio__.printfn(arguments[0], param);
}
__stdio__.SetIn(Console.In);
__stdio__.SetOut(Console.Out);