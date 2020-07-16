FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine
WORKDIR /migration

# Copy csproj and restore as distinct layers
COPY . ./
COPY Deeproxio.Persistence.Identity/ ./Deeproxio.Persistence.Identity/

WORKDIR /migration/

RUN dotnet restore

WORKDIR /migration/Deeproxio.Persistence.Identity.Migration

RUN dotnet restore
RUN dotnet build