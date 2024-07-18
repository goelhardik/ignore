FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin -Channel 6.0 -Runtime dotnet -InstallDir /usr/share/dotnet

COPY . /app/

WORKDIR /app

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

RUN dotnet restore

RUN dotnet build --configuration Release --no-restore

ENTRYPOINT ["dotnet", "test", "--configuration", "Release", "--no-build", "--verbosity", "normal", "--collect:\"XPlat Code Coverage\"", "--results-directory", "/coverage"]
