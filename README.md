1. Requirements to run<br/>
.net 9 SDK<br/>
Blazor<br/>
<br/>
2. Clone git <br/>
<br/>
3. Run below in terminal while in solution dir:
<br/>dotnet tool install --local dotnet-ef;
<br/>dotnet tool install --local csharpier; <br/>
dotnet restore; <br/>
dotnet build; <br/>
<br/>
4. launch entry Api with Web Ui below, or do so manually <br/>
dotnet run --project UserAccessSystem.Internal.Api/UserAccessSystem.Internal.Api.csproj;
<br/>
------------<br/>
New terminal needed for Web host<br/>
dotnet run --project UserAccesSystem.External.Web/UserAccesSystem.External.Web.csproj;<br/>
<br/>

Ui: http://localhost:5226/<br/>

<br/>

Entry point Backend -> UAS.Internal.Api<br/>
Entry point Ui -> UAS.External.Web
