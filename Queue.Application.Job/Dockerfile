﻿FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Queue.Application.Job.csproj", "Queue.Application.Job/"]
RUN dotnet restore "Queue.Application.Job/Queue.Application.Job.csproj"
WORKDIR "/src/Queue.Application.Job"
COPY . .
RUN dotnet build "Queue.Application.Job.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Queue.Application.Job.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Queue.Application.Job.dll"]
