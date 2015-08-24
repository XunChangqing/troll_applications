﻿; -- Example1.iss --
; Demonstrates copying 3 files and creating an icon.

; SEE THE DOCUMENTATION FOR DETAILS ON CREATING .ISS SCRIPT FILES!
#define DistFolder '..\dist_folder' 
#define ApplicationName '山妖卫士'
#define ApplicationVersion GetFileVersion('..\dist_folder\troll_ui_app.exe')

[Setup]
AppName={#ApplicationName}
AppPublisher=马沙科技
AppVersion={#ApplicationVersion}

DefaultDirName={pf}\masatek\trollwiz
DefaultGroupName=山妖卫士
UninstallDisplayIcon={app}\troll_ui_app.exe
Compression=lzma2
SolidCompression=yes
;OutputDir=userdocs:Inno Setup Examples Output

[languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
;Name: "cs"; MessagesFile: "compiler:Languages\ChineseSimp.isl"

[Dirs]
Name: "{localappdata}\masatek\trollwiz"
[Files]
Source: "{#DistFolder}\troll_ui_app.exe"; DestDir: "{app}"
Source: "{#DistFolder}\troll_ui_app.exe.config"; DestDir: "{app}"
Source: "{#DistFolder}\troll_ui_app.pdb"; DestDir: "{app}"
Source: "{#DistFolder}\troll_ui_app.vshost.exe"; DestDir: "{app}"
Source: "{#DistFolder}\troll_ui_app.vshost.exe.config"; DestDir: "{app}"
Source: "{#DistFolder}\TrotiNet.dll"; DestDir: "{app}"
Source: "{#DistFolder}\TrotiNet.pdb"; DestDir: "{app}"
Source: "{#DistFolder}\TrotiNet.xml"; DestDir: "{app}"
Source: "{#DistFolder}\System.Data.SQLite.dll"; DestDir: "{app}"
Source: "{#DistFolder}\System.Data.SQLite.xml"; DestDir: "{app}"
Source: "{#DistFolder}\log4net.dll"; DestDir: "{app}"
Source: "{#DistFolder}\log4net.xml"; DestDir: "{app}"
Source: "{#DistFolder}\HtmlAgilityPack.dll"; DestDir: "{app}"
Source: "{#DistFolder}\HtmlAgilityPack.pdb"; DestDir: "{app}"
Source: "{#DistFolder}\HtmlAgilityPack.xml"; DestDir: "{app}"
Source: "{#DistFolder}\x64\SQLite.Interop.dll"; DestDir: "{app}\x64\SQLite.Interop.dll"
Source: "{#DistFolder}\x86\SQLite.Interop.dll"; DestDir: "{app}\x86\SQLite.Interop.dll"
Source: "{#DistFolder}\caffe.dll"; DestDir: "{app}"
Source: "{#DistFolder}\libiomp5md.dll"; DestDir: "{app}"
Source: "{#DistFolder}\data\deploy.prototxt"; DestDir: "{app}\data\deploy.prototxt"
Source: "{#DistFolder}\data\imagenet_mean.binaryproto"; DestDir: "{app}\data\imagenet_mean.binaryproto"
Source: "{#DistFolder}\data\model_256__iter_50000.caffemodel"; DestDir: "{app}\data\model_256__iter_50000.caffemodel"
Source: "{#DistFolder}\porn.db"; DestDir: "{app}"
;Source: "MyProg.chm"; DestDir: "{app}"
;Source: "Readme.txt"; DestDir: "{app}"; Flags: isreadme

;Source: "{#MyDistFolder}\dotNetFx40_Full_setup.exe"; DestDir: {tmp}; Flags: deleteafterinstall; Check: not IsRequiredDotNetDetected

[Icons]
Name: "{group}\山妖卫士"; Filename: "{app}\troll_ui_app.exe"; WorkingDir: "{app}"
Name: "{group}\卸载山妖卫士"; Filename: "{uninstallexe}"
Name: "{commondesktop}\山妖卫士"; Filename: "{app}\troll_ui_app.exe"; WorkingDir: "{app}"; Tasks: desktopicon 
;Name: "{commonprograms}\My Program"
;Name: "{commonstartup}\My Program"
;Name: "{Desktop}"; Filename: "{app}\troll_ui_app.exe";

[UninstallDelete]
Type: filesandordirs; Name: "{localappdata}\masatek\trollwiz"

[Tasks]
Name: desktopicon; Description: "创建桌面快捷方式"; GroupDescription: "Additional icons:";
;Name: desktopicon\common; Description: "For all users"; GroupDescription: "Additional icons:";
;Name: desktopicon\user; Description: "For the current user only"; GroupDescription: "Additional icons:";
;Name: quicklaunchicon; Description: "Create a &Quick Launch icon"; GroupDescription: "Additional icons:";
;Name: associate; Description: "&Associate files"; GroupDescription: "Other tasks:"; Flags: unchecked
Name: startexe; Description: "安装完成以后启动程序"

[Run]
;Filename: {tmp}\dotNetFx40_Full_setup.exe; Parameters: "/q:a /c:""install /l /q"""; Check: not IsRequiredDotNetDetected; StatusMsg: Microsoft Framework 4.0 is beïng installed. Please wait...

[Code]
function IsDotNetDetected(version: string; service: cardinal): boolean;
// Indicates whether the specified version and service pack of the .NET Framework is installed.
//
// version -- Specify one of these strings for the required .NET Framework version:
//    'v1.1.4322'     .NET Framework 1.1
//    'v2.0.50727'    .NET Framework 2.0
//    'v3.0'          .NET Framework 3.0
//    'v3.5'          .NET Framework 3.5
//    'v4\Client'     .NET Framework 4.0 Client Profile
//    'v4\Full'       .NET Framework 4.0 Full Installation
//    'v4.5'          .NET Framework 4.5
//
// service -- Specify any non-negative integer for the required service pack level:
//    0               No service packs required
//    1, 2, etc.      Service pack 1, 2, etc. required
var
    key: string;
    install, release, serviceCount: cardinal;
    check45, success: boolean;
begin
    // .NET 4.5 installs as update to .NET 4.0 Full
    if version = 'v4.5' then begin
        version := 'v4\Full';
        check45 := true;
    end else
        check45 := false;

    // installation key group for all .NET versions
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\' + version;

    // .NET 3.0 uses value InstallSuccess in subkey Setup
    if Pos('v3.0', version) = 1 then begin
        success := RegQueryDWordValue(HKLM, key + '\Setup', 'InstallSuccess', install);
    end else begin
        success := RegQueryDWordValue(HKLM, key, 'Install', install);
    end;

    // .NET 4.0/4.5 uses value Servicing instead of SP
    if Pos('v4', version) = 1 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Servicing', serviceCount);
    end else begin
        success := success and RegQueryDWordValue(HKLM, key, 'SP', serviceCount);
    end;

    // .NET 4.5 uses additional value Release
    if check45 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Release', release);
        success := success and (release >= 378389);
    end;

    result := success and (install = 1) and (serviceCount >= service);
end;


function InitializeSetup(): Boolean;
begin
    if not IsDotNetDetected('v4.5', 0) then begin
        MsgBox('MyApp requires Microsoft .NET Framework 4.0 Client Profile.'#13#13
            'Please use Windows Update to install this version,'#13
            'and then re-run the MyApp setup program.', mbInformation, MB_OK);
        result := false;
    end else
        result := true;
end;
