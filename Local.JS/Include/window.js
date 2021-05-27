/// DEFINE AUTHOR "Creeper Lv"
/// DEFINE NAME "WindowCompatibility"
/// DEFINE VERSION 0.1.0.0
/// USING JS "io.js"
/// IFNDEF WINDOWOBJ
/// DEFINE WINDOWOBJ
var window = new Object();
/// ENDIF
window.alert = alert;
function prompt(text, value) {
    alert("=INPUT====");
    alert("=" + text);
    alert("=Default Value:" + value);
    var a = Console.ReadLine();
    if (a == "") return value;
    else return a;
};

window.prompt = prompt;