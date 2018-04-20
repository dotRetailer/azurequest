using RestSharp;

namespace AzureQuest.Common.SecureRequest
{
    public static class RequestClient
    {
        public const string TokenUrl = "https://azurequest-tmp.eu.auth0.com/oauth/token";

        public static AuthenticationRequestData GetAuthRequestData()
        {
            var datajson = System.Environment.GetEnvironmentVariable("AzureQuestAPIAuth", System.EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(datajson)) { throw new System.MissingMemberException("Environment Variable: AzureQuestAPIAuth"); }
            var result = datajson.JsonDeserialize<AuthenticationRequestData>();
            if(result == null || string.IsNullOrEmpty(result.client_id)){ throw new System.FormatException($"Could not de-serialize into AuthenticationRequestData: {datajson}"); }
            return result;
        }

        public static AccessToken GetAccessToken()
        {
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", GetAuthRequestData().JsonSerialize(), ParameterType.RequestBody);
            IRestResponse response = new RestClient(TokenUrl).Execute(request);
            return response.Content.JsonDeserialize<AccessToken>();
        }

        public static OperationResult Request(string url, Method method, object data = null)
        {
            var token = GetAccessToken();
            var request = new RestRequest(method);
            request.AddHeader("authorization", $"{token.token_type} {token.access_token}");
            if (data != null) { request.AddJsonBody(data); }
            var response = new RestClient(url).Execute(request);
            return new OperationResult(true, response.Content);
        }
    }
}
