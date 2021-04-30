/// DEFINE NAME SampleStaticServer
/// DEFINE VERSION 1.0.0.0
/// DEFINE AUTHOR Creeper Lv
/// USING JS Part2.js
/// USING DLL Local.JS.Extension.HttpServer.dll
/// EXPOSE TYPE FileInfo System.IO.FileInfo
function HttpHandler(context){
	alert("Request:"+context.Request.HttpMethod+">>"+context.Request.Url.AbsolutePath);
	var index=IndexedFile_Index.Get(context.Request.Url.AbsolutePath.substring(1));
	if(index==null){
		ServerCore.SendMessage(context,"<html><head><title>404 Not Found</title></head><body><p>404 Not Found</p></body></html>");
	}else{
		ServerCore.SendFile(context,index);
	}
}
function ExceptionHandler(e){
	error(e);
}
function FileDatabaseInit(){
	IndexedFile_Index.Init("./Files/TheArk.json");
	pass("IndexedFile Database Inited.");
}
function Command(){
	var item="";
	while(true){
		item = Console.ReadLine();
		var cmdL=CommandLineTool.Analyze(item);
		if(cmdL.RealParameter[0].EntireArgument.toUpperCase()=="EXIT"){
			return;
		}else if(cmdL.RealParameter[0].EntireArgument.toUpperCase().startsWith("REG")){
			try{
				var item1=cmdL.RealParameter[1].EntireArgument;// arg0
				var item2=cmdL.RealParameter[2].EntireArgument;// arg1
				if(item1==""){
					error("Wrong Argument.");
					continue;
				}
				if(item2==""){
					error("Wrong Argument.");
					continue;
				}
				IndexedFile_Index.StoreCpy(item1,item2);
				IndexedFile_Index.SaveIndeics();
			}catch(e){
				error("Something Wrong:"+e);
			}
			
		}else{
			warn("Unknown command.");
		}
	
	}
}
function Main(){
	alert("Local.JS.Extension.SimpleHttpServer Test");
	alert("Local.JS.Extension.IndexedFile Test");
	FileDatabaseInit();
	ServerCore.AddHandler("HttpHandler");
	ServerCore.SetExecutingEngine(Core.GetEngine());
	ServerCore.SetListeningAddress("http://127.0.0.1:8081/");
	ServerCore.ApplySettings();
	ServerCore.Start();
	pass("Server Started.");
}
Main();
Command();