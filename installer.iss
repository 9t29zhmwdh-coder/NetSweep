; Inno Setup script for NetSweep.
; MyAppVersion is passed in from CI via /DMyAppVersion=<version> (see .github/workflows/installer.yml).
; Not signed: unsigned installers trigger a Windows SmartScreen "Unknown Publisher" prompt on first run,
; which is expected for this project (see CHANGELOG / project decision, 2026-07-10).

#define MyAppName "NetSweep"
#define MyAppPublisher "novoSYS Informatik GmbH"
#define MyAppExeName "NetSweep.exe"
#define MyAppURL "https://github.com/9t29zhmwdh-coder/NetSweep"

[Setup]
AppId={{B6E1B6C0-6E2A-4E4A-9C7E-2E7B7E5B9B4A}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputBaseFilename=NetSweep-Setup
OutputDir=installer-output
Compression=lzma
SolidCompression=yes
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
UninstallDisplayIcon={app}\{#MyAppExeName}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"

[Files]
Source: "publish\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall skipifsilent
