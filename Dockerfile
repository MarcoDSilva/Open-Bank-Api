FROM mcr.microsoft.com/dotnet/sdk:6.0 AS Main
WORKDIR /app
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["/OpenBank.API/OpenBank.API.csproj", "OpenBank.API/"]
RUN dotnet restore "OpenBank.API/OpenBank.API.csproj"
COPY . .
WORKDIR "/src/OpenBank.API"
RUN dotnet build "OpenBank.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenBank.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM Main as Final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "SimpleBank.API.dll"]
