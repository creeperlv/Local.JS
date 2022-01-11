/// DEFINE AUTHOR "Creeper Lv"
/// DEFINE NAME "TerminalGUI"
/// DEFINE VERSION 0.1.0.0
/// USING DLL Local.JS.Extension.TerminalGUI.dll
/// USING DLL Terminal.Gui.dll
/// EXPOSETYPE TGUI Local.JS.Extension.TerminalGUI.GUICS
/// EXPOSETYPE Dim Terminal.Gui.Dim
/// EXPOSETYPE Pos Terminal.Gui.Pos
/// EXPOSETYPE Window Terminal.Gui.Window
var TerminalGUIInited = false;
function InitGUI() {
    TGUI.Init();
    TerminalGUIInited = true;
}