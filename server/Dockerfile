# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0.204-alpine3.19 AS build-env
WORKDIR /usr/local/src/app
COPY . .
RUN dotnet publish src/GameServer -c Release -o bin --sc false \
    -p:DebugType=none -p:PublishSingleFile=true

FROM mcr.microsoft.com/dotnet/runtime:8.0.4-alpine3.19
COPY --from=build-env /usr/local/src/app/bin/GameServer /usr/local/bin/GameServer
ENTRYPOINT ["/usr/local/bin/GameServer"]
EXPOSE 14514
