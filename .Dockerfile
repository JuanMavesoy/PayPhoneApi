# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln .
COPY PayPhoneApi/*.csproj ./PayPhoneApi/
COPY Application/*.csproj ./Application/
COPY Domain/*.csproj ./Domain/
COPY Infraestructure/*.csproj ./Infraestructure/

RUN dotnet restore

COPY . .
RUN dotnet publish PayPhoneApi/PayPhoneApi.csproj -c Release -o out

# Etapa de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "PayPhoneApi.dll"]
