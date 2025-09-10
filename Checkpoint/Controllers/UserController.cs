﻿using Checkpoint.Model;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Checkpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private static ConnectionMultiplexer redis;

        [HttpGet]
        public async Task<IActionResult> Get()
        {

            string key = "getUsers";

            try
            {
                redis = ConnectionMultiplexer.Connect("localhost:6379");
                IDatabase db = redis.GetDatabase();
                await db.KeyExpireAsync(key, TimeSpan.FromMinutes(15));
                string userValue = await db.StringGetAsync(key);

                if (!string.IsNullOrEmpty(userValue))
                {
                    return Ok(userValue);
                }

                using var connection = new MySqlConnection("Server=localhost;Database=mysql; User=root;Password=123;");
                await connection.OpenAsync();
                string query = @"select * from users; ";
                var users = await connection.QueryAsync<User>(query);
                string userJson = JsonConvert.SerializeObject(users);
                await db.StringSetAsync(key, userJson);

                return Ok(users);
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
