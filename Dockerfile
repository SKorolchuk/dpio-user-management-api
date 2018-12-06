FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY . .
RUN dotnet restore

WORKDIR /app/AccountApi

RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/AccountApi/out .

HEALTHCHECK CMD curl --fail http://localhost:80/ready || exit

ENTRYPOINT ["dotnet", "Deeproxio.AccountApi.dll"]
