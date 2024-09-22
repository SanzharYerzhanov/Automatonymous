FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["WebApplication5/WebApplication5.csproj", "WebApplication5/"]
RUN dotnet restore "WebApplication5/WebApplication5.csproj"
COPY . .
WORKDIR "/src/WebApplication5"
RUN dotnet build "WebApplication5.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApplication5.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApplication5.dll"]
