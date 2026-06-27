FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080


RUN apk add --no-cache icu-data icu-libs 
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["LibraryManager.sln", "./"]
COPY ["LibraryManager.API/LibraryManager.API.csproj", "LibraryManager.API/"]
COPY ["LibraryManager.Application/LibraryManager.Application.csproj", "LibraryManager.Application/"]
COPY ["LibraryManager.Application.Tests/LibraryManager.Application.Tests.csproj", "LibraryManager.Application.Tests/"]
COPY ["LibraryManager.Domain/LibraryManager.Domain.csproj", "LibraryManager.Domain/"]
COPY ["LibraryManger.Persistence/LibraryManger.Persistence.csproj", "LibraryManger.Persistence/"]

RUN dotnet restore

COPY . .
WORKDIR "/src/LibraryManager.API"
RUN dotnet build "LibraryManager.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LibraryManager.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LibraryManager.API.dll"]