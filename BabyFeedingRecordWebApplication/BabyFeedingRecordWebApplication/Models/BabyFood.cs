using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace BabyFeedingRecordWebApplication.Models
{
    public class BabyFood
    {
        public int Id { get; set; }
        [Display(Name = "BabyFood")]
        public string? Name { get; set; }

        public List<DateOnly>? WholeDates;
        public List<DateOnly>? ConsecutiveDates;

        public static List<DateOnly> getLongestConsecutiveDates(List<DateOnly> dtlst)
        {

            List<DateOnly> consecutiveDateLst = new List<DateOnly>();
            List<DateOnly> tempDateLst = new List<DateOnly>();
            if (dtlst.Count == 0)
                return consecutiveDateLst;
            dtlst.Sort();
      
            consecutiveDateLst.Add(dtlst[0]);
            tempDateLst.Add(dtlst[0]);
            for (int i = 1; i < dtlst.Count; i++)
            {
                if ((dtlst[i].DayNumber - dtlst[i - 1].DayNumber) == 1)
                    tempDateLst.Add(dtlst[i]);
                else
                {
                    if (tempDateLst.Count > consecutiveDateLst.Count)
                        consecutiveDateLst = tempDateLst.ToList();
                    tempDateLst.Clear();
                }

            }
            if (tempDateLst.Count > consecutiveDateLst.Count)
                consecutiveDateLst = tempDateLst;

            return consecutiveDateLst;
        }

    }
}
