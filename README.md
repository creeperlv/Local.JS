# Local.JS

[![.NET](https://github.com/creeperlv/Local.JS/actions/workflows/dotnet.yml/badge.svg)](https://github.com/creeperlv/Local.JS/actions/workflows/dotnet.yml)

![Logo of Local.JS](/Icon.png)

This is a small tool to run JavaScript programs locally without web browsers. Currently, it can be used to build up a simple web browser.

## Get Started

There are currently no releases or compiled binaries, you need to manually run `dotnet build` to obtain executable. In order to perform cross-compile, you may need to specify the [runtime identifier](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog), e.g.: to build Local.JS targeting x64 Linux, you need to use `-r linux-x64`.

The Local.JS have pre-expoesd a lot of .NET types, objects to help you build up you own things, such as `Console`, `Files`, `Directory`.

To run your JavaScript code, just simply type `./Local.JS <your-code-file>` in your terminal.

## Pre-Processor

Local.JS supports macro like C/C++, however, uses different way to mark macros in order to not breaking the compatibility with ECMAScript.

For example:
```
/// DEFINE KEY
/// ifdef KEY
/// endif
/// ifndef KEY
/// endif
/// USING JS "A Java Script Code File.js"
/// USING DLL "An-Externel-DotNet-Lib.dll"
```

**Note:** The Console Application of Local.JS will automatically perform pre-process before executing the file, it can be switched off by `--NOPREPROCESS`.

**Note:** If Local.JS is compiled in Debug configuration, `DEBUG` flag will be automatically defined. In Release configuration, it will define `RELEASE`.

## Extensions

There are a few extensions to help developing JavaScript program as backend (or, server-side).

They are:

-Local.JS.Extension.BatchedMultiTask - A thread manager that limits the number parallel tasks.

-Local.JS.Extension.IndexedFile - A simple index to files, help organizing files.

-Local.JS.Extension.HttpServer - A simple web server that uses HttpListener as its backend. This server uses delegate-callback and `Local.JS.Extension.BatchedMultiTask` to help balancing the load of the server.

## Samples

Currently, there is only one sample that is a web server, named `SimpleWebServer.js` inside `Samples` folder, it uses some Extensions of Local.JS.

## Licensing

Almost all files under this repo are licensed under the MIT License as `LICENSE` file says.
