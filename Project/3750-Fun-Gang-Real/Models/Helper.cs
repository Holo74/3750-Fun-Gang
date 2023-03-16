namespace Assignment_1.Models
{
    public static class Helper
    {
        public static T? ReturnFirstSelected<T>(List<T> list)
        {
            if (list.Count > 0)
            {
                return list[0];
            }
            return default(T);
        }
    }
}