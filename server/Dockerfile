# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0.204-alpine3.19 AS build-env
WORKDIR /app
COPY . .
RUN dotnet publish src/GameServer/GameServer.csproj \
    -c Release \
    -o bin \
    --sc false \
    -p:DebugType=none \
    -p:PublishSingleFile=true

FROM mcr.microsoft.com/dotnet/runtime:8.0.4-alpine3.19
WORKDIR /app
COPY --from=build-env /app/bin/GameServer .
RUN ln -s /record Records
ENTRYPOINT ["./GameServer"]
EXPOSE 14514
ENV JUDGE=1
