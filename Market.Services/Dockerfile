# Usa la imagen de base de .NET SDK para construir la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia el archivo de proyecto y restaura las dependencias
COPY ./MARKETPRODUCT_API/MARKETPRODUCT_API.csproj ./
RUN dotnet restore

# Copia todos los archivos y compila la aplicación
COPY ./MARKETPRODUCT_API/. ./
RUN dotnet publish -c Release -o out

# Usa la imagen de runtime de .NET para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out ./

# Expone el puerto en el que la aplicación escucha
EXPOSE 5080

# Establece la entrada de la aplicación
ENTRYPOINT ["dotnet", "MARKETPRODUCT_API.dll"]
