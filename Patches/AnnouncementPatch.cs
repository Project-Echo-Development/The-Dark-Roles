using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.Data.Player;
using AmongUs.Data;
using Assets.InnerNet;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace DarkRoles.Patches
{
    public class ModNews
    {
        public int Number;
        public int BeforeNumber;
        public string Title;
        public string SubTitle;
        public string ShortTitle;
        public string Text;
        public string Date;

        public Announcement ToAnnouncement()
        {
            var result = new Announcement
            {
                Number = Number,
                Title = Title,
                SubTitle = SubTitle,
                ShortTitle = ShortTitle,
                Text = Text,
                Language = (uint)DataManager.Settings.Language.CurrentLanguage,
                Date = Date,
                Id = "ModNews"
            };

            return result;
        }
    }
    [HarmonyPatch]
    public class ModNewsHistory
    {
        public static List<ModNews> AllModNews = new();

        // When creating new news, you can not delete old news 
        public static void Init()
        {
            if (TranslationController.Instance.currentLanguage.languageID == SupportedLangs.English)
            {
                {
                    {
                        // The Dark Roles 1.0.0 Release
                        var news = new ModNews
                        {
                            Number = 100000,
                            Title = "Here at last!",
                            SubTitle = "\r★★ Finally released! ★★",
                            ShortTitle = "The Dark Roles 1.0.0",
                            Text = "<size=150%>Welcome to The Dark Roles v1.0.0 by Project Echo Development!</size>\n\n<size=125%>Support for Among Us v17.1s and up!</size>\n"
                                + "\n【Base】\n - Base on TOH\r\n"
                                + "\n【Added】\n - An indoor pool\r\n"
                                + "\n【Changes】\n - Mod is refreshed\n\r"
                                + "\n【Message】\n - Apologies for such a long wait. In the process of development, I lost motivation to work on the project multiple times. At last, I am proud to finally release The Dark Roles!\r",
                            Date = "2026-01-12T00:00:00Z"
                        };
                        AllModNews.Add(news);
                    }
                }
            }

            [HarmonyPatch(typeof(PlayerAnnouncementData), nameof(PlayerAnnouncementData.SetAnnouncements)), HarmonyPrefix]
            static bool SetModAnnouncements(PlayerAnnouncementData __instance, [HarmonyArgument(0)] ref Il2CppReferenceArray<Announcement> aRange)
            {
                if (!AllModNews.Any())
                {
                    Init();
                    AllModNews.Sort((a1, a2) => { return DateTime.Compare(DateTime.Parse(a2.Date), DateTime.Parse(a1.Date)); });
                }

                List<Announcement> FinalAllNews = new();
                AllModNews.Do(n => FinalAllNews.Add(n.ToAnnouncement()));
                foreach (var news in aRange)
                {
                    if (!AllModNews.Any(x => x.Number == news.Number))
                        FinalAllNews.Add(news);
                }
                FinalAllNews.Sort((a1, a2) => { return DateTime.Compare(DateTime.Parse(a2.Date), DateTime.Parse(a1.Date)); });

                aRange = new(FinalAllNews.Count);
                for (int i = 0; i < FinalAllNews.Count; i++)
                    aRange[i] = FinalAllNews[i];

                return true;
            }
        }
    }
}
