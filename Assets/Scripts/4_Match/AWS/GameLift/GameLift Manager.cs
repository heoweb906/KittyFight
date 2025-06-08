using Amazon;
using Amazon.GameLift;
using Amazon.Runtime;

public class GameLiftManager
{
    protected static readonly RegionEndpoint region = RegionEndpoint.APNortheast2;

    protected static AmazonGameLiftClient GetClient()
    {
        var credentials = AWSAuthManager.GetCredentials();
        return new AmazonGameLiftClient(credentials, region);
    }
}