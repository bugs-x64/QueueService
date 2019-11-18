# QueueService
Client-server application for managing client queues in the office. This is an example application for registering office visitors in a queue for service. Server methods have a delay in execution to simulate real data processing.
# Technologies
- WEB-server: ASP.NET Core 2.2, Entity Framework Core 2.2, SignalR Core 1.1(for v3.0.0 require ASP.NET Core 3.0), MSSQL Express 2019
- Client,Admin app: WinForms, .NET Framework 4.6.1, SignalR Core 3.0.0
# Instruction for build and use
## 1. Configurating API Server
1) Install Visual Studio 2017 or later
2) Use Visual Studio Installer for install ASP.NET Core 2.2
3) Change synthetic delay time in file "appsettings.json"
```	json
    "SyntheticDelayMilliseconds": 0
```
### 1.1 Create Database:
1) In the "appsettings.json" file, specify the connection string to the server. Without changing the name of the connection string
``` json
"ConnectionStrings": {
    "qsdb": "Server=(localhost)\\sqlexpress;Database=qsdb;Trusted_Connection=True;"
  }
```
2) Run following commands in the package manager console (Microsoft.EntityFrameworkCore.Tools is required):
``` powershell
Add-Migration InitialCreate
Update-Database
```
3) Run **ClientsTests.Authentication** and **EmployeesTests.Authentication** tests from QueueServcieTests project. 
It is necessary to verify the correctness of the settings for connecting the server to the database
4) Run this SQL query:
``` sql
USE [qsdb]
GO
SET IDENTITY_INSERT [dbo].[employees] ON
INSERT INTO [dbo].[employees]
           ([id],[fio])
     VALUES
           (0,'default')
GO
SET IDENTITY_INSERT [dbo].[employees] OFF
```
5) Perform all other tests from QueueServcieTests project

## 2. Configurating Client app
1. Install .NET Framework 4.6.1
2. Use settings.xml for change url address of API Server. Example: 
``` xml
<?xml version="1.0" encoding="utf-8" ?>
<root>
  <server address="https://qservice124.azurewebsites.net"></server>
</root>
```
## 3. Configurating Admin app
1. Install .NET Framework 4.6.1
2. Use settings.xml for change url address of API Server. Example: 
``` xml
<?xml version="1.0" encoding="utf-8" ?>
<root>
  <server address="https://qservice124.azurewebsites.net"></server>
</root>
```
