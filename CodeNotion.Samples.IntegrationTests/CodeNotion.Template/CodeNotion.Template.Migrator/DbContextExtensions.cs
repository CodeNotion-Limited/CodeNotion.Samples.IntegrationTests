﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace CodeNotion.Template.Migrator;

public static class DbContextExtensions
{
    public static List<T> RawSqlQuery<T>(this DbContext context, string query, Func<DbDataReader, T> map)
    {
        using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = query;
        command.CommandType = CommandType.Text;

        context.Database.OpenConnection();

        using var result = command.ExecuteReader();
        var entities = new List<T>();

        while (result.Read())
        {
            entities.Add(map(result));
        }

        return entities;
    }
}