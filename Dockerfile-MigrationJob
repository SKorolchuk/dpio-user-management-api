FROM microsoft/dotnet:2.2-sdk
WORKDIR /migration

# Copy csproj and restore as distinct layers
COPY . ./
COPY Deeproxio.Persistence.Identity/ ./Deeproxio.Persistence.Identity/

WORKDIR /migration/

RUN dotnet restore

WORKDIR /migration/Deeproxio.Persistence.Identity

RUN dotnet restore
RUN dotnet build