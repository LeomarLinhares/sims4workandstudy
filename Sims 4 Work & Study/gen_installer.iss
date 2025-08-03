[Setup]
AppName=Sims 4 Work & Study
AppVersion=0.2
DefaultDirName={pf}\Sims4WorkAndStudy
DefaultGroupName=Sims4WorkAndStudy
OutputDir=installer_output
OutputBaseFilename=Installer

[Files]
Source: "C:\Users\leoma\source\repos\Sims 4 Work & Study\Sims 4 Work & Study\bin\Release\net8.0-windows\publish\win-x86\*"; DestDir: "{app}"; Flags: recursesubdirs

[Icons]
Name: "{group}\Sims 4 Work & Study"; Filename: "{app}\Sims4WorkStudy.exe"
