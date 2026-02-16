# Create migration

# To create a migration for SQLite, use the following command:
```bash	
dotnet ef migrations add -c SqliteDbContext -p src/DSRS.Infrastructure/DSRS.Infrastructure.csproj -s src/DSRS.Gateway/DSRS.Gateway.csproj -o Persistence/Migrations/Sqlite
dotnet ef database update -c SqliteDbContext -p src/DSRS.Infrastructure/DSRS.Infrastructure.csproj -s src/DSRS.Gateway/DSRS.Gateway.csproj

```

# To create a migration for SQL Server, use the following command:
```bash
dotnet ef migrations add -c SqlServerDbContext -p src/DSRS.Infrastructure/DSRS.Infrastructure.csproj -s src/DSRS.Gateway/DSRS.Gateway.csproj -o Persistence/Migrations/SqlServer
```

# Then update the database to apply the migration for SQLite:
```bash
dotnet ef database update -c SqliteDbContext -p src/DSRS.Infrastructure/DSRS.Infrastructure.csproj -s src/DSRS.Gateway/DSRS.Gateway.csproj
```
# And for SQL Server:
```bash
dotnet ef database update -c SqlServerDbContext -p src/DSRS.Infrastructure/DSRS.Infrastructure.csproj -s src/DSRS.Gateway/DSRS.Gateway.csproj
```

# Remove migration
# To revert database changes, update database from previous migration or remove all migrations
dotnet ef database update {previous_migration_name}
or
dotnet ef database update 0

# To remove the last migration for SQLite, use the following command:
```bash
dotnet ef migrations remove -c SqliteDbContext -p src/DSRS.Infrastructure/DSRS.Infrastructure.csproj -s src/DSRS.Gateway/DSRS.Gateway.csproj -o Persistence/Migrations/Sqlite
```

# To remove the last migration for SQL Server, use the following command:
```bash
dotnet ef migrations remove -c SqlServerDbContext -p src/DSRS.Infrastructure/DSRS.Infrastructure.csproj -s src/DSRS.Gateway/DSRS.Gateway.csproj -o Persistence/Migrations/SqlServer
```