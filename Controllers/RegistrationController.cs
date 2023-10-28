using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AdminCnsole_Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AdminCnsole_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RegistrationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("registration")]
        public string registration(Registration registration)
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("AdminDbConnection").ToString());
            SqlCommand cmd =
                new SqlCommand(
                    "INSERT INTO Registration(UserName,Password,IsActive) VALUES ('" + registration.UserName + "','" +
                    registration.Password + "','" + registration.IsActive + "')", con);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            if (i > 0)
            {
                return "Data inserted";
            }
            else
            {
                return "error";
            }
            
        }


        [HttpPost]
        [Route("login")]
        public IActionResult login(Registration registration)
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("AdminDbConnection").ToString());
    
            // 打开数据库连接
            con.Open();

            // 构造查询字符串，使用参数化查询以避免 SQL 注入
            string query = "SELECT * FROM Registration WHERE UserName = @UserName AND Password = @Password AND IsActive = 1";
    
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@UserName", registration.UserName);
            cmd.Parameters.AddWithValue("@Password", registration.Password);

            // 执行查询
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                // 用户有效，创建令牌并返回
                var token = GenerateToken(registration.UserName);
                con.Close(); // 记得关闭连接
                return Ok(new { token });
            }
            else
            {
                con.Close(); // 记得关闭连接
                return Unauthorized(); // 用户无效，返回 401 Unauthorized
            }
        }
        private string GenerateToken(string username)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"]; // Retrieve the secret key from configuration
            var key = Encoding.ASCII.GetBytes(secretKey); // Convert the secret key to bytes

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username)
                    // Add more claims as needed, e.g., roles, permissions
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Token expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}




















