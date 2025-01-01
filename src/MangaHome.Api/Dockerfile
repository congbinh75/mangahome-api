FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

RUN mkdir /logs

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/MangaHome.Api/MangaHome.Api.csproj", "src/MangaHome.Api/"]
RUN dotnet restore "src/MangaHome.Api/MangaHome.Api.csproj"
COPY . .
WORKDIR "/src/src/MangaHome.Api"
RUN dotnet build "MangaHome.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "MangaHome.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MangaHome.Api.dll"]
