using System;
using System.Collections.Generic;
using Soheil.Common;
using Soheil.Core.DataServices;

namespace Soheil.Core
{
    public static class LoginInfo
    {
        public static UserDataService DataService { get; set; }
        public static int Id { get; set; }
        public static string Title { get; set; }
        public static List<Tuple<string, AccessType>> Access { get; set; }
		public static bool IsDebugMode { get; set; }

        public static string GetUsername(int userId)
        {
            if (DataService == null) return string.Empty;
            var user = DataService.GetSingle(userId);
            return user == null ? string.Empty : user.Username;
        }

	}
}
