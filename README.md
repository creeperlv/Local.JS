# Local.JS

This is a small tool to run JavaScript programs locally without web browsers. Currently, it can be used to build up a simple web browser.

## Get Started

There are currently no releases or compiled binaries, you need to manually run `dotnet build` to obtain executable.

The Local.JS have pre-expoesd a lot of .NET types, objects to help you build up you own things, such as `Console`, `Files`, `Directory`.

To run your JavaScript code, just simply type `./Local.JS <your-code-file>` in your terminal.

## Extensions

There are a few extensions to help developing JavaScript program as backend (or, server-side).

They are:

-Local.JS.Extension.BatchedMultiTask - A thread manager that limits the number parallel tasks.

-Local.JS.Extension.IndexedFile - A simple index to files, help organizing files.

-Local.JS.Extension.HttpServer - A simple web server that uses HttpListener as its backend. This server uses delegate-callback and `Local.JS.Extension.BatchedMultiTask` to help balancing the load of the server.

## Samples

Currently, there is only one sample that is a web server, named `SimpleWebServer.js` inside `Samples` folder, it uses the Extensions of Local.JS.