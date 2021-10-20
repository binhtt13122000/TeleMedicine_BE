#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TeleMedicine_BE/TeleMedicine_BE.csproj", "TeleMedicine_BE/"]
COPY ["BusinessLogic/BusinessLogic.csproj", "BusinessLogic/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["AgoraIO/AgoraIO.csproj", "AgoraIO/"]
RUN dotnet restore "TeleMedicine_BE/TeleMedicine_BE.csproj"
COPY . .
WORKDIR "/src/TeleMedicine_BE"
RUN dotnet build "TeleMedicine_BE.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TeleMedicine_BE.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TeleMedicine_BE.dll"]