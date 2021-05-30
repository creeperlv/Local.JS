/// DEFINE AUTHOR "Creeper Lv"
/// DEFINE NAME "WindowCompatibility"
/// DEFINE VERSION 0.1.0.0
/// USING JS "io.js"
/// IFNDEF WINDOWOBJ
/// DEFINE WINDOWOBJ
var window = new Object();
/// ENDIF
alert = function __alert(text) {
    var b0 = typeof (TerminalGUIInited) == "undefined";
    if (b0 == true) {
        Console.WriteLine(text);
    } else {
        if (TerminalGUIInited == false) {
            Console.WriteLine(text);
        } else {
            //Will work...

        }
    }

}
window.alert = alert;
function prompt(text, value) {
    var b0 = typeof (TerminalGUIInited) == "undefined";
    if (b0 == true) {

        alert("=INPUT====");
        alert("=" + text);
        alert("=Default Value:" + value);
        var a = Console.ReadLine();
        if (a == "") return value;
        else return a;
    } else {
        if (TerminalGUIInited == false) {

            alert("=INPUT====");
            alert("=" + text);
            alert("=Default Value:" + value);
            var a = Console.ReadLine();
            if (a == "") return value;
            else return a;
        } else {
            //Will work...
            return "";
        }
    }
};

window.prompt = prompt;