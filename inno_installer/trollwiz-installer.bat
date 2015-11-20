"D:\masawork\SignTool\signtool" sign /f "D:\masawork\SignTool\new_masatek.pfx" /p MasaTekcs417 /t http://timestamp.wosign.com/timestamp  "D:\\gitwork\\troll_applications\\dist_folder\\troll_ui_app.exe"

"C:\Program Files (x86)\Inno Setup 5\iscc.exe" /Q /DON_LINE=1 /Swosign="D:\masawork\SignTool\SignTool.exe sign /f D:\masawork\SignTool\new_masatek.pfx /p MasaTekcs417 /t http://timestamp.wosign.com/timestamp $f" troll_installer.iss
"C:\Program Files (x86)\Inno Setup 5\iscc.exe" /Q /DOFF_LINE=1 /Swosign="D:\masawork\SignTool\SignTool.exe sign /f D:\masawork\SignTool\new_masatek.pfx /p MasaTekcs417 /t http://timestamp.wosign.com/timestamp $f" troll_installer.iss

pause

