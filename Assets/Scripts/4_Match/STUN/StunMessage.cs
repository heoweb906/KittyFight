using System;
using System.Net;

public class StunResult
{
    public IPEndPoint PublicEndPoint;
}

public class StunMessage
{
    /// <summary>
    /// STUN Binding Request 메시지 생성
    /// </summary>
    public static byte[] BuildRequest()
    {
        byte[] stunRequest = new byte[20];
        // Message Type: Binding Request (0x0001)
        stunRequest[0] = 0x00;
        stunRequest[1] = 0x01;
        // Message Length: 0 (no attributes)
        stunRequest[2] = 0x00;
        stunRequest[3] = 0x00;
        // Transaction ID: 16 bytes 랜덤
        Array.Copy(Guid.NewGuid().ToByteArray(), 0, stunRequest, 4, 16);
        return stunRequest;
    }

    /// <summary>
    /// STUN 응답에서 공인 IP와 포트를 추출
    /// </summary>
    public static IPEndPoint ParseResponse(byte[] data)
    {
        if (data.Length < 20)
            return null;

        int index = 20;

        while (index + 4 <= data.Length)
        {
            // Attribute Header: Type (2 bytes) + Length (2 bytes)
            ushort attrType = (ushort)((data[index] << 8) | data[index + 1]);
            ushort attrLength = (ushort)((data[index + 2] << 8) | data[index + 3]);

            // 최소 길이 보장
            if (index + 4 + attrLength > data.Length)
                break;

            // Check for XOR-MAPPED-ADDRESS (0x0020) or MAPPED-ADDRESS (0x0001)
            if (attrType == 0x0020 || attrType == 0x0001)
            {
                byte family = data[index + 5];
                if (family == 0x01) // IPv4
                {
                    ushort port = (ushort)((data[index + 6] << 8) | data[index + 7]);
                    byte[] ipBytes = new byte[4];

                    if (attrType == 0x0020)
                    {
                        port ^= 0x2112;
                        for (int i = 0; i < 4; i++)
                            ipBytes[i] = (byte)(data[index + 8 + i] ^ 0x21);
                    }
                    else if (attrType == 0x0001)
                    {
                        Array.Copy(data, index + 8, ipBytes, 0, 4);
                    }

                    return new IPEndPoint(new IPAddress(ipBytes), port);
                }
            }

            // 다음 속성으로 이동 (속성은 4바이트 헤더 + 본문)
            index += 4 + attrLength;
        }

        return null;
    }
}