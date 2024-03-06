FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["trb-officer-backend.csproj", "./"]
RUN dotnet restore "trb-officer-backend.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "trb-officer-backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "trb-officer-backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "trb-officer-backend.dll"]
