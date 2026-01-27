# Fase base (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Fase build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiamos el csproj desde la raíz
COPY ["ApiBootcampCLT.csproj", "./"]
RUN dotnet restore "./ApiBootcampCLT.csproj"

# Copiamos todo el resto
COPY . .
RUN dotnet build "./ApiBootcampCLT.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Fase publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ApiBootcampCLT.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Fase final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiBootcampCLT.dll"]
