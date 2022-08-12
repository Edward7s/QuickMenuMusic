using System.IO;
using System.Net;
using System.Linq;
namespace QuickMenuMusic
{ 
    internal class Config
    {
        public static Config Instance { get; private set; }
        public string Path { get; private set; }
        public Config()
        {
            Instance = this;
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "//Nocturnal"))
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "//Nocturnal");

            if (!Directory.Exists(Directory.GetCurrentDirectory() + "//Nocturnal//MenuMusic"))
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "//Nocturnal//MenuMusic");

            if (Directory.GetFiles(Directory.GetCurrentDirectory() + "//Nocturnal//MenuMusic").Length != 0)
            {
                Path = Directory.GetFiles(Directory.GetCurrentDirectory() + "//Nocturnal//MenuMusic").FirstOrDefault();
                return;
            }

            using (WebClient wc = new WebClient()) 
                wc.DownloadFile("https://github.com/Edward7s/QuickMenuMusic/raw/master/QuickMenuMusic/Music/Its%20to%20coold.mp3", Directory.GetCurrentDirectory() + "//Nocturnal//MenuMusic//SweaterWheather.mp3");

            Path = Directory.GetFiles(Directory.GetCurrentDirectory() + "//Nocturnal//MenuMusic").FirstOrDefault();

        }
    }
}