; -- Example1.iss --
; Demonstrates copying 3 files and creating an icon.

; SEE THE DOCUMENTATION FOR DETAILS ON CREATING .ISS SCRIPT FILES!
#define DistFolder '..\dist_folder' 
#define ApplicationName '山妖卫士'
#define ApplicationVersion GetFileVersion('..\dist_folder\troll_ui_app.exe')

[Setup]
AppName={#ApplicationName}
AppPublisher=马沙科技
AppVersion={#ApplicationVersion}

PrivilegesRequired=admin
DefaultDirName={pf}\masatek\trollwiz
DefaultGroupName=山妖卫士
UninstallDisplayIcon={app}\troll_ui_app.exe
Compression=lzma2
SolidCompression=yes
;OutputDir=userdocs:Inno Setup Examples Output
AppMutex=masa_troll_guard_mutex
SetupMutex=TrollwizSetupsMutex,Global\TrollwizSetupsMutex
MinVersion=6.1.7600
;OnlyBelowVersion=10.0.10240
SetupIconFile=shanyaows.ico
;WizardImageBackColor=#bbggrr #221100
WizardImageFile=left.bmp
WizardSmallImageFile=small.bmp
OutputBaseFilename=trollwiz-{#ApplicationVersion}
OutputManifestFile=trollwiz-{#ApplicationVersion}.manifest
DisableReadyPage=yes
DisableReadyMemo=yes
DisableWelcomePage=yes
DisableDirPage=auto
DisableProgramGroupPage=auto

[languages]
;Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "cs"; MessagesFile: "compiler:Languages\ChineseSimp.isl"

[Registry]
Root: HKCU; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "trollwiz"; ValueData: """{app}\troll_ui_app.exe -notvisible"""; Flags: uninsdeletevalue; Tasks: autostart

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
Source: "{#DistFolder}\Titanium.Web.Proxy.dll"; DestDir: "{app}"
Source: "{#DistFolder}\System.Data.SQLite.dll"; DestDir: "{app}"
Source: "{#DistFolder}\System.Data.SQLite.xml"; DestDir: "{app}"
Source: "{#DistFolder}\log4net.dll"; DestDir: "{app}"
Source: "{#DistFolder}\log4net.xml"; DestDir: "{app}"
Source: "{#DistFolder}\HtmlAgilityPack.dll"; DestDir: "{app}"
Source: "{#DistFolder}\HtmlAgilityPack.pdb"; DestDir: "{app}"
Source: "{#DistFolder}\HtmlAgilityPack.xml"; DestDir: "{app}"
Source: "{#DistFolder}\Newtonsoft.Json.dll"; DestDir: "{app}"
Source: "{#DistFolder}\Newtonsoft.Json.xml"; DestDir: "{app}"
Source: "{#DistFolder}\QRCoder.dll"; DestDir: "{app}"
Source: "{#DistFolder}\QRCoder.pdb"; DestDir: "{app}"
Source: "{#DistFolder}\x64\SQLite.Interop.dll"; DestDir: "{app}\x64\"
Source: "{#DistFolder}\x86\SQLite.Interop.dll"; DestDir: "{app}\x86\"
Source: "{#DistFolder}\trollwiz-masatek.dll"; DestDir: "{app}"
Source: "{#DistFolder}\libiomp5md.dll"; DestDir: "{app}"
Source: "{#DistFolder}\msvcp120.dll"; DestDir: "{app}"
Source: "{#DistFolder}\msvcr120.dll"; DestDir: "{app}"
Source: "{#DistFolder}\data\porn.model"; DestDir: "{app}\data\"
Source: "{#DistFolder}\porn.db"; DestDir: "{app}"
Source: "{#DistFolder}\avdevice-56.dll"; DestDir: "{app}"
Source: "{#DistFolder}\avfilter-5.dll"; DestDir: "{app}"
Source: "{#DistFolder}\avcodec-56.dll"; DestDir: "{app}"
Source: "{#DistFolder}\avformat-56.dll"; DestDir: "{app}"
Source: "{#DistFolder}\avutil-54.dll"; DestDir: "{app}"
Source: "{#DistFolder}\postproc-53.dll"; DestDir: "{app}"
Source: "{#DistFolder}\swresample-1.dll"; DestDir: "{app}"
Source: "{#DistFolder}\swscale-3.dll"; DestDir: "{app}"
Source: "{#DistFolder}\ffmpegwrapper.dll"; DestDir: "{app}"
;Source: "{#DistFolder}\testfile.txt"; DestDir: "{app}"
;Source: "MyProg.chm"; DestDir: "{app}"
;Source: "Readme.txt"; DestDir: "{app}"; Flags: isreadme

;require .net framework 4.5
;[Files]
;Source: "{#DistFolder}\dotnetfx45_full_x86_x64.exe"; DestDir: {tmp}; Flags: deleteafterinstall; Check: not IsRequiredDotNetDetected
;[Run]
;Filename: {tmp}\dotnetfx45_full_x86_x64.exe; Parameters: "/q:a /c:""install /l /q"""; Check: not IsRequiredDotNetDetected; StatusMsg: Microsoft Framework 4.5 is beïng installed. Please wait...

[Files]
Source: "{#DistFolder}\dotNetFx45_Full_setup.exe"; DestDir: {tmp}; Flags: deleteafterinstall; Check: not IsRequiredDotNetDetected
[Run]
Filename: {tmp}\dotNetFx45_Full_setup.exe; Parameters: "/q:a /c:""install /l /q"""; Check: not IsRequiredDotNetDetected; StatusMsg: Microsoft Framework 4.5 is beïng installed. Please wait...

[Icons]
Name: "{group}\山妖卫士"; Filename: "{app}\troll_ui_app.exe"; WorkingDir: "{app}"; Tasks: startupicon
Name: "{group}\卸载山妖卫士"; Filename: "{uninstallexe}"; Tasks: startupicon
;Name: "{Desktop}"; Filename: "{app}\troll_ui_app.exe";
Name: "{commondesktop}\山妖卫士"; Filename: "{app}\troll_ui_app.exe"; WorkingDir: "{app}"; Tasks: desktopicon 
;Name: "{commonprograms}\My Program"
;Name: "{commonstartup}\My Program"


[UninstallDelete]
Type: filesandordirs; Name: "{localappdata}\masatek\trollwiz"
Type: filesandordirs; Name: "{localappdata}\masatek\troll_ui_app*"

[Tasks]
Name: desktopicon; Description: "创建桌面快捷方式"; GroupDescription: "附加图标：";
Name: startupicon; Description: "创建开始菜单快捷方式"; GroupDescription: "附加图标："
Name: autostart; Description: "开机启动"; GroupDescription: "启动项"
;Name: desktopicon\common; Description: "For all users"; GroupDescription: "Additional icons:";
;Name: desktopicon\user; Description: "For the current user only"; GroupDescription: "Additional icons:";
;Name: quicklaunchicon; Description: "Create a &Quick Launch icon"; GroupDescription: "Additional icons:";
;Name: associate; Description: "&Associate files"; GroupDescription: "Other tasks:"; Flags: unchecked
;Name: startexe; Description: "安装完成以后启动程序"

[Run]
Filename: "{app}\troll_ui_app.exe"; Description: "启动应用程序"; Flags: postinstall nowait

;[UninstallRun]
;Filename: {app}\troll_ui_app.exe; Parameters: "-uninstall"

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

function IsRequiredDotNetDetected(): Boolean;  
begin
    result := IsDotNetDetected('v4.5', 0);
end;

//function InitializeSetup(): Boolean;
//begin
//    if not IsDotNetDetected('v4.5', 0) then begin
//        MsgBox('MyApp requires Microsoft .NET Framework 4.0 Client Profile.'#13#13
//            'Please use Windows Update to install this version,'#13
//            'and then re-run the MyApp setup program.', mbInformation, MB_OK);
//        result := false;
//    end else
//        result := true;
//end;
