namespace FurnitureStoreBE.Common
{
    public static class UserSession
    {
        private static string _userId;

        // Method to set the userId
        public static void SetUserId(string userId)
        {
            _userId = userId;
        }

        // Method to get the userId
        public static string GetUserId()
        {
            return _userId;
        }
    }

}
