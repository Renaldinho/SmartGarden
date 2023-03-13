FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Program/Program.csproj", "Program/"]
RUN dotnet restore "Program/Program.csproj"
COPY . .
WORKDIR "/src/Program"
RUN dotnet build "Program.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Program.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Program.dll"]
