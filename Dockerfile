FROM mcr.microsoft.com/dotnet/sdk:3.1-bionic AS builder

RUN useradd -m dotnet
USER dotnet

COPY --chown=dotnet . /app/
WORKDIR /app

ENV DOTNET_CLI_TELEMETRY_OPTOUT 1

RUN dotnet restore

RUN dotnet build --configuration Release --no-restore 

RUN dotnet test --configuration Release --no-build --verbosity normal
