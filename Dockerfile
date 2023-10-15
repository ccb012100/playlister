FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY Playlister/*.csproj ./Playlister/
COPY Playlister.Tests/*.csproj ./Playlister.Tests/
# Restore as distinct layers
RUN dotnet restore

# copy everything else and build app
COPY . ./
WORKDIR /source/Playlister
RUN dotnet publish -c release --no-restore -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app .
ENTRYPOINT ["dotnet", "Playlister.dll"]