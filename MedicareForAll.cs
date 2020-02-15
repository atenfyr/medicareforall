using Terraria.ModLoader;

namespace MedicareForAll
{
	public class MedicareForAll : Mod
	{
        //public static string GithubUserName => "atenfyr";
        //public static string GithubProjectName => "medicareforall";

        public static class UsefulThings
        {
            public static string SecondsToHMS(int num, string zeroString = "0 seconds")
            {
                if (num < 1) return zeroString;

                string res = "";
                int hours = num / 3600;
                if (hours == 1) res += hours + " hour ";
                if (hours > 1) res += hours + " hours ";
                num %= 3600;
                int minutes = num / 60;
                if (minutes == 1) res += minutes + " minute ";
                if (minutes > 1) res += minutes + " minutes ";
                num %= 60;
                if (num == 1) res += num + " second ";
                if (num > 1) res += num + " seconds ";

                return res.TrimEnd();
            }
        }


        public MedicareForAll()
		{
            Properties = ModProperties.AutoLoadAll;
        }
    }
}