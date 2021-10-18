using AgoraIO.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace TeleMedicine_BE.Utils
{
    public interface IAgoraProvider
    {
        string GenerateToken(string channel, dynamic uId, uint expiredTime);
    }
    public class AgoraProvider : IAgoraProvider
    {
        public string GenerateToken(string channel, dynamic uId, uint expiredTime)
        {
            var uid = uId.ValueKind == JsonValueKind.Number ? uId.GetUInt64().ToString() : uId.GetString();

            var tokenBuilder = new AccessToken("834dec7fc5144086a2fe803cb3e51fff", "94b0a8175ae1499b8bfdc994e2271eb0", channel, uid);

            tokenBuilder.addPrivilege(Privileges.kJoinChannel, expiredTime);

            tokenBuilder.addPrivilege(Privileges.kPublishAudioStream, expiredTime);

            tokenBuilder.addPrivilege(Privileges.kPublishVideoStream, expiredTime);

            tokenBuilder.addPrivilege(Privileges.kPublishDataStream, expiredTime);

            tokenBuilder.addPrivilege(Privileges.kRtmLogin, expiredTime);

            return tokenBuilder.build();
        }
    }
}
