FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY . .
RUN dotnet restore

WORKDIR /app/AccountApi

RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/AccountApi/out .

ENTRYPOINT ["dotnet", "Deeproxio.AccountApi.dll"]
