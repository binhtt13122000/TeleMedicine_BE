using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.Utils
{
    public enum SortTypeEnum
    {
        desc,
        asc
    }

    public class Constants
    {

        public static readonly int PAGE_SIZE = 50;

        public static readonly int MAXIMUM_PAGE_SIZE = 250;

        public static readonly string EXPIRES_IN_DAY = "86400";

        public static class Role
        {
            public const string ADMIN = "CUSTOMER";
        }

        public static class PrefixPolicy
        {
            public const string REQUIRED_ROLE = "RequiredRole";
        }

        public static class TokenClaims
        {
            public const string ROLE = "role";
            public const string UID = "uid";
            public const string EMAIL = "email";
        }

        public static class HeaderClaims
        {
            public const string FIREBASE_AUTH = "FirebaseAuth";
        }

        public static class AccountStatus
        {
            public const string ACTIVE = "ACTIVE";
        }

        public static class Status
        {
            public const string ACTIVE = "Active";

            public const string DISABLED = "Disabled";
        }

        public static class AppSetting
        {
            public const string FirebaseBucket = "testapp-f2b7c.appspot.com";
            public const string FirebaseApiKey = "AIzaSyB7aKvG36BtxQtGAs5Mqtt5SA-kv0Naqzc";
            public const string FirebaseAuthEmail = "usertest123@gmail.com";
            public const string FirebaseAuthPassword = "abc1235";

        }

        public static class Notification
        {
            public const int REQUEST_VERIFY = 1;
            public const int REQUEST_HEALTHCHECK = 2;
            public const int CANCEL_HEALTHCHECK = 3;
            public const int FINISH_HEATHCHECK = 4;
            public const int UPDATE_HEATHCHECK = 5;
        }
    }
}
