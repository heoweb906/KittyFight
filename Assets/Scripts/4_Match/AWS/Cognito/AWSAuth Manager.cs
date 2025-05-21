using Amazon;
using Amazon.CognitoIdentity;
using Amazon.Runtime;

public static class AWSAuthManager
{
    private static CognitoAWSCredentials credentials;

    private const string IdentityPoolId = "ap-northeast-2:6e5e4460-7f1b-4a84-848e-d8309afcbe07";

    public static AWSCredentials GetCredentials()
    {
        if (credentials == null)
        {
            credentials = new CognitoAWSCredentials(
                IdentityPoolId,
                RegionEndpoint.APNortheast2
            );
        }

        return credentials;
    }
}
