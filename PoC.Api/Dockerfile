FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal AS base
WORKDIR /app
EXPOSE 5100

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src
COPY ["PoC.Api.csproj", "./"]
RUN dotnet restore "./PoC.Api.csproj"

COPY . .
WORKDIR "/src/."
RUN dotnet build "PoC.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PoC.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PoC.Api.dll"]