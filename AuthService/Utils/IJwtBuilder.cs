﻿namespace AuthMicroservice.Utils;

public interface IJwtBuilder
{
    string GetToken(UserClaimsData userClaimsData);
    UserClaimsData ValidateToken(string token);
}
