FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY TaskTrack.Api/TaskTrack.Api.csproj TaskTrack.Api/
COPY TaskTrack.Application/TaskTrack.Application.csproj TaskTrack.Application/
COPY TaskTrack.Domain/TaskTrack.Domain.csproj TaskTrack.Domain/
COPY TaskTrack.Infrastructure/TaskTrack.Infrastructure.csproj TaskTrack.Infrastructure/

RUN dotnet restore TaskTrack.Api/TaskTrack.Api.csproj

COPY . .
RUN dotnet publish TaskTrack.Api/TaskTrack.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TaskTrack.Api.dll"]