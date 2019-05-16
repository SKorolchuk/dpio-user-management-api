# Deeproxio Account REST API

AccountApi service is used for Deeproxio platform authentication and authorization processes.
Also the service responds for managing dpio user data.

Technologies: `ASP.NET Core`, `Entity Framework Core`, `ASP.NET Core Identity`, `AutoMapper`, `Npgsql`, `JWT`

    Service requires access to `Postgres` database.

## Docker images

- Service image preparation

  ```bash
  # cd in root git repository
  docker build -t dpio-accountapi:latest -f Dockerfile .
  ```

- Service image standalone usage

  ```bash
  # run command after image build
  docker run -d --name dpio-accountapi --restart always -p 4300:80 --link local-postgres -e ConnectionStrings__IdentityDbContext=Host=local-postgres;Database=dpioaccountdb;Username=postgres;Password=12345678 -e ENVIRONMENT=Production dpio-accountapi:latest
  ```

  P.S. `local-postgres` is `Postgres` DB container deployed in the same docker environment. Replace to any correct name if it's need. Also replace `Username` and `Password` to correct DB user settings.

- Migration image preparation

  ```bash
  # cd in root git repository
  docker build -t dpio-accountapi-db-migration:latest -f Dockerfile-MigrationJob .
  ```

- Migration image standalone usage
  ```bash
  # run interactive container session after image build
  docker run -it --rm --name dpio-accountapi-migration --link test-postgres-server -e ConnectionStrings__IdentityDbContext=Host=test-postgres-server;Database=dpio-user-db;Username=admin;Password=12345678 -e ENVIRONMENT=Production dpio-accountapi-db-migration:latest dotnet ef database update
  ```
  P.S. `test-postgres-server` is `Postgres` DB container deployed in the same docker environment. Replace to any correct name if it's need. Also replace `Username` and `Password` to correct DB user settings.
