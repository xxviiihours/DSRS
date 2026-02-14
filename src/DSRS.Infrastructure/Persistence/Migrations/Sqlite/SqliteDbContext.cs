using DSRS.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSRS.Infrastructure.Persistence.Migrations.Sqlite;

public class SqliteDbContext(DbContextOptions<SqliteDbContext> options,
    IDateTime dateTimeService) : AppDbContext(options, dateTimeService)
{
}
