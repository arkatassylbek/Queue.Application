﻿FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Queue.Application.Receiver.csproj", "Queue.Application.Receiver/"]
RUN dotnet restore "Queue.Application.Receiver/Queue.Application.Receiver.csproj"
WORKDIR "/src/Queue.Application.Receiver"
COPY . .
RUN dotnet build "Queue.Application.Receiver.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Queue.Application.Receiver.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Queue.Application.Receiver.dll"]
