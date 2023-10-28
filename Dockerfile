FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["AdminCnsole_Backend/AdminCnsole_Backend.csproj", "AdminCnsole_Backend/"]
RUN dotnet restore "AdminCnsole_Backend/AdminCnsole_Backend.csproj"
COPY . .
WORKDIR "/src/AdminCnsole_Backend"
RUN dotnet build "AdminCnsole_Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AdminCnsole_Backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AdminCnsole_Backend.dll"]
