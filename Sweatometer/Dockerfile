FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS builder

RUN apt-get update && \
    apt-get install -y unzip libunwind8 gettext nodejs npm
ADD . /Sweatometer
WORKDIR /Sweatometer
RUN dotnet build
RUN dotnet publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0

COPY --from=builder /Sweatometer/bin/Debug/netcoreapp3.0/publish/ /Sweatometer/
WORKDIR /Sweatometer
ENTRYPOINT dotnet Sweatometer.dll
