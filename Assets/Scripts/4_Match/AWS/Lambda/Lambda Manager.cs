using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.Runtime;

public class LambdaManager
{
    protected static readonly RegionEndpoint region = RegionEndpoint.APNortheast2;

    protected static AmazonLambdaClient GetClient()
    {
        var credentials = AWSAuthManager.GetCredentials();
        return new AmazonLambdaClient(credentials, region);
    }
}