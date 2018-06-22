﻿using System;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.SecretOptions;
using Microsoft.Extensions.Options;
using Npgsql;
using Dapper;
using System.Linq;

namespace api.mapserv.utah.gov.Services
{
    public class PostgreApiKeyRepository : IApiKeyRepository
    {
        private readonly string ConnectionString;
        private const string apiKeyByKey = @"SELECT key,
                   account_id, 
                   whitelisted, 
                   status, 
                   deleted, 
                   configuration, 
                   regex_pattern, 
                   is_machine_name
                FROM
                    public.apikeys
                WHERE 
                    lower(key) = @key";

        public PostgreApiKeyRepository(IOptions<DbConfiguration> dbOptions)
        {
            ConnectionString = $"Host=db;Username=postgres;Password={dbOptions.Value.DbPassword};Database=webapi";
        }

        public async Task<ApiKey> GetKey(string key)
        {
            key = key.ToLowerInvariant();

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                var items = await conn.QueryAsync<ApiKey>(apiKeyByKey, new { key });

                return items.FirstOrDefault();
            }

            return null;
        }
    }
}