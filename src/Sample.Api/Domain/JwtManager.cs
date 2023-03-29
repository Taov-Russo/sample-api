using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Sample.Api.Infrastructure.Authorization;
using Sample.Api.Models;
using Sample.Api.Models.Configuration;

namespace Sample.Api.Domain;

public interface IJwtManager
{
    string CreateAuthToken(Guid userId);
}

public class JwtManager : IJwtManager
{
    private readonly JwtConfiguration configuration;

    public JwtManager(JwtConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public string CreateAuthToken(Guid userId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, userId.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

        var jwt = new JwtSecurityToken(
            configuration.ValidIssuer,
            configuration.ValidAudience,
            notBefore: DateTime.Now,
            claims: claimsIdentity.Claims,
            expires: GetExpirationDate(),
            signingCredentials: new SigningCredentials(getSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        var handler = new JwtSecurityTokenHandler();
        var encodedJwt = handler.WriteToken(jwt);
        return encodedJwt;
    }

    public DateTime GetExpirationDate()
    {
        return DateTime.Now.Add(configuration.TokenLifetime);
    }

    public void Configure(JwtBearerOptions options)
    {
        options.SecurityTokenValidators.Clear();
        options.SecurityTokenValidators.Add(new PartnerChangeJwtValidator());
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = configuration.ValidIssuer,
            ValidateAudience = true,
            ValidAudience = configuration.ValidAudience,
            ValidateLifetime = true,
            IssuerSigningKey = getSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true
        };
    }

    private SymmetricSecurityKey getSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.EncryptionKey));
    }
}