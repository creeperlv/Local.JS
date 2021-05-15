/// DEFINE NAME "Culture"
/// DEFINE AUTHOR "Creeper Lv"
/// DEFINE VERSION 0.1.0.0
/// EXPOSETYPE __culture__ System.Globalization.CultureInfo
/// IFNDEF SYSTEM
/// DEFFINE SYSTEM
var System = new Object();
/// ENDIF
/// IFNDEF System_Globalization
/// DEFINE System_Globalization
System.Globalization = new Object();
/// ENDIF
System.Globalization.CultureInfo = __culture__;
var CurrentCulture = __culture__.CurrentCulture;