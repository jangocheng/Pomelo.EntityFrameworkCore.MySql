// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using MySql.Data.MySqlClient;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    public static class MySqlDbContextOptionsExtensions
    {
        public static DbContextOptionsBuilder UseMySql(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            [NotNull] string connectionString,
            [CanBeNull] Action<MySqlDbContextOptionsBuilder> mySqlOptionsAction = null)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));
            Check.NotEmpty(connectionString, nameof(connectionString));

            var csb = new MySqlConnectionStringBuilder(connectionString)
            {
	            AllowUserVariables = true,
	            UseAffectedRows = false
            };
            connectionString = csb.ConnectionString;
            var extension = GetOrCreateExtension(optionsBuilder);
            extension.ConnectionString = connectionString;
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            mySqlOptionsAction?.Invoke(new MySqlDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        public static DbContextOptionsBuilder UseMySql(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            [NotNull] DbConnection connection,
            [CanBeNull] Action<MySqlDbContextOptionsBuilder> mySqlOptionsAction = null)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));
            Check.NotNull(connection, nameof(connection));

            var csb = new MySqlConnectionStringBuilder(connection.ConnectionString);
            if (csb.AllowUserVariables != true || csb.UseAffectedRows != false)
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    csb.AllowUserVariables = true;
                    csb.UseAffectedRows = false;
                    connection.ConnectionString = csb.ConnectionString;
                }
                else
                {
                    throw new InvalidOperationException("The connection string in Pomelo.EntityFrameworkCore.MySql must contains \"UseAffectedRows=false\" and \"AllowUserVariables=true\"");
                }
            };
            
            var extension = GetOrCreateExtension(optionsBuilder);
            extension.Connection = connection;
            extension.ConnectionString = csb.ConnectionString;
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            mySqlOptionsAction?.Invoke(new MySqlDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        public static DbContextOptionsBuilder<TContext> UseMySql<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            [NotNull] string connectionString,
            [CanBeNull] Action<MySqlDbContextOptionsBuilder> mySqlOptionsAction = null)
            where TContext : DbContext
        {
            return (DbContextOptionsBuilder<TContext>)UseMySql(
                (DbContextOptionsBuilder)optionsBuilder, new MySqlConnection(connectionString), mySqlOptionsAction);
        }

        public static DbContextOptionsBuilder<TContext> UseMySql<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            [NotNull] DbConnection connection,
            [CanBeNull] Action<MySqlDbContextOptionsBuilder> mySqlOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UseMySql(
                (DbContextOptionsBuilder)optionsBuilder, connection, mySqlOptionsAction);

        private static MySqlOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
        {
            var existing = optionsBuilder.Options.FindExtension<MySqlOptionsExtension>();
            return existing != null
                ? new MySqlOptionsExtension(existing)
                : new MySqlOptionsExtension();
        }
    }
}
