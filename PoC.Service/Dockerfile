FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src
COPY ["PoC.Service.csproj", "./"]
RUN dotnet restore "./PoC.Service.csproj"

COPY ./ ./
WORKDIR "/src/."
RUN dotnet build "PoC.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PoC.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PoC.Service.dll"]