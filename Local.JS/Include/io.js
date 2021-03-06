/// DEFINE NAME "DotNetIO"
/// DEFINE AUTHOR "Creeper Lv"
/// DEFINE VERSION 0.1.0.0
/// EXPOSETYPE File System.IO.File
/// EXPOSETYPE Directory System.IO.Directory
/// EXPOSETYPE FileInfo System.IO.FileInfo
/// EXPOSETYPE DirectoryInfo System.IO.DirectoryInfo
/// EXPOSETYPE Stream System.IO.Stream
/// EXPOSETYPE FileStream System.IO.FileStream
/// EXPOSETYPE TextWriter System.IO.TextWriter
/// EXPOSETYPE TextReader System.IO.TextReader

/// IFNDEF SYSTEM
/// DEFFINE SYSTEM
var System = new Object();
/// ENDIF
/// IFNDEF System_IO
/// DEFINE System_IO
System.IO = new Object();
/// ENDIF
System.IO.File = File;
System.IO.Directory = Directory;