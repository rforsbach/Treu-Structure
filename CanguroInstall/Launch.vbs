Set WshShell = CreateObject("WScript.Shell")
WshShell.Run """" & Property("CustomActionData") & """",7,False
Set WshShell = Nothing