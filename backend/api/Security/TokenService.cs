using api.Helpers;
using Externalities.QueryModels;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Newtonsoft.Json;
using Serilog;

namespace api.Security;

public class TokenService
{
    public string IssueToken(Journalist journalist)
    {
        try
        {
            IJwtAlgorithm algorithm = new HMACSHA512Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            var jwtEncoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            var token = jwtEncoder.Encode(journalist, Environment.GetEnvironmentVariable(ENV_VAR_KEYS.JWT.ToString()));
            return token;
        }
        catch (Exception e)
        {
            Log.Error(e, "Problem issuing JWT");
            throw new InvalidOperationException("Could not create JWT");
        }
    }

    public Dictionary<string, string> ValidateJwt(string jwt)
    {
        try
        {
            IJsonSerializer serializer = new JsonNetSerializer();
            var provider = new UtcDateTimeProvider();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, new HMACSHA512Algorithm());
            var claims = decoder.Decode(jwt, Environment.GetEnvironmentVariable(ENV_VAR_KEYS.JWT.ToString()));
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(claims);
        }
        catch (Exception e)
        {
            Log.Error(e, "Problem validating JWT");
            throw new InvalidOperationException("Invalid JWT");
        }
    }
}