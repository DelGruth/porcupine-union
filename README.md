1. Requirements to run
.net 9 SDK
Blazor

2. Clone git

3. Run below in terminal while in solution dir: 
dotnet tool install --local dotnet-ef;
dotnet tool install --local csharpier;
dotnet restore; 
dotnet build; 

-launch entry Api with Web Ui below, or do so manually 
dotnet run --project UserAccessSystem.Internal.Api/UserAccessSystem.Internal.Api.csproj;

------------
New terminal needed for Web host
dotnet run --project UserAccesSystem.External.Web/UserAccesSystem.External.Web.csproj;


Ui: http://localhost:5226/



Entry point Backend -> UAS.Internal.Api
Entry point Ui -> UAS.External.Web
