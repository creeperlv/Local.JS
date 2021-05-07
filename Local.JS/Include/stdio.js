/// DEFINE NAME "STDIO"
/// DEFINE AUTHOR "Creeper Lv"
/// DEFINE VERSION 0.0.1.0
/// IFNDEF STDIO
/// DEFINE STDIO
/// ENDIF
function getc() {
    return Local.JS.JSIO.stdio.getc();
}
function gets() {
    return Local.JS.JSIO.stdio.gets();
}
function scanf(format) {
    return Local.JS.JSIO.stdio.scanf(format);
}
function printf() {
    var param = new Object[];
    for (var i = 1; i < arguments.length; i++) {
        param.push(arguments[i]);
    }
    Local.JS.JSIO.stdio.printf(argments[0], param);
}
Local.JS.JSIO.stdio.SetIn(System.Console.In);
Local.JS.JSIO.stdio.SetOut(System.Console.Out);