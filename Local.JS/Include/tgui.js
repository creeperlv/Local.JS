/// DEFINE AUTHOR "Creeper Lv"
/// DEFINE NAME "TerminalGUI"
/// DEFINE VERSION 0.1.0.0
/// USING DLL Local.JS.Extension.TerminalGUI.dll
/// EXPOSETYPE Index Local.JS.Extension.TerminalGUI.GUICS
var TerminalGUIInited = false;
function InitGUI() {
    GUICS.Init();
    TerminalGUIInited = true;
}