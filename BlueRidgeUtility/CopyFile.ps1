Param(
[parameter(Mandatory=$true)]
$UserName,
[parameter(Mandatory=$true)]
$Password,
[parameter(Mandatory=$true)]
$Source,
[parameter(Mandatory=$true)]
$Target,
[parameter(Mandatory=$true)]
$RemoteServer
)

$SecurePassword = ConvertTo-SecureString $Password -AsPlainText -Force
$cred = New-Object System.Management.Automation.PSCredential $UserName, $SecurePassword
$Session = New-PSSession -ComputerName $RemoteServer -Credential $cred
copy-item -path $Source  -destination $Target -force -ToSession $Session

 $SecurePassword = ConvertTo-SecureString Abhishek.Kumar$!@ -AsPlainText -Force
$cred = New-Object System.Management.Automation.PSCredential dev.corp.blueridgeglobal.com\abhishek.kumar,$SecurePassword
 $Session = New-PSSession -ComputerName 54.82.6.91 -Credential $cred
Copy-Item "S:\SCP_Ticket_Backup\SCP-10361\2018-08-31\ClarityEvents_20180831040416.dat" -Destination "D:\SCP_Ticket_Backup\" -FromSession  $Session

http://technico.qnownow.com/the-winrm-client-cannot-process-the-request-if-the-authentication-scheme-is-different/