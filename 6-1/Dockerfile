# Usamos la imagen oficial del SDK de .NET 8 para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos el csproj y restauramos dependencias
# Nota: Como correremos esto DENTRO de la carpeta 6-1, solo copiamos el archivo local
COPY 6-1.csproj .
RUN dotnet restore

# Copiamos todo el resto del código
COPY . .

# Publicamos la aplicación en modo Release
RUN dotnet publish -c Release -o /app/publish

# --- Etapa Final (Imagen ligera para correr la app) ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 8080

# Copiamos los archivos compilados desde la etapa anterior
COPY --from=build /app/publish .

# Arrancamos la API
ENTRYPOINT ["dotnet", "6-1.dll"]