FROM microsoft/dotnet:3.1-sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY . .
RUN dotnet restore

WORKDIR /app/Deeproxio.UserManagement.API

RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:3.1-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/Deeproxio.UserManagement.API/out .

EXPOSE 80

ENTRYPOINT ["dotnet", "Deeproxio.UserManagement.API.dll"]
