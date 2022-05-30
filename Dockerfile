FROM mcr.microsoft.com/dotnet/sdk:5.0-bionic AS builder

COPY . /app/

WORKDIR /app

ENV DOTNET_CLI_TELEMETRY_OPTOUT 1

RUN dotnet restore

RUN dotnet build --configuration Release --no-restore 

ENTRYPOINT exec dotnet test --configuration Release --no-build --verbosity normal --collect:"XPlat Code Coverage" -r /coverage
