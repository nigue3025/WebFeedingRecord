using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace BabyFeedingRecordWebApplication.Models
{
    public class MovingAvg
    {
        public List<FeedingStatistics> StatLst = new List<FeedingStatistics>();
        public static int[] MaNumLst = new int[] { 3, 5, 10, 15, 30 };


        public int Id { get; set; }


        public string DateInterval
        {
            get
            {
                return $"{StatLst[StatLst.Count - 1].feedingRecords[StatLst[StatLst.Count - 1].feedingRecords.Count - 1].FeedingDate.ToString("yyyy.MM.dd")}~" +
                    $"{StatLst[0].feedingRecords[0].FeedingDate.ToString("yyyy.MM.dd")}";
            }

        }

        [Display(Name = "n")]
        public int Mnum { get; set; }





        [Display(Name = "母奶n日均")]
        public double MA_MMilk
        {
            get
            {
                return StatLst.Average(a => (double)a.MotherMilkTotal);
            }
        }

        [Display(Name = "配方n日均")]
        public double MA_FMilk
        {
            get
            {
                return StatLst.Average(a => (double)a.FormularMilkTotal);
            }
        }

        [Display(Name = "總和n日均")]
        public double MA_Total
        {
            get
            {
                return StatLst.Average(a => (double)a.Total);
            }
        }

        [Display(Name = "配方比率(%)")]
        public double MA_FMpcntg
        {
            get
            {
                return (MA_FMilk / MA_Total) * 100;
            }
        }

        [Display(Name = "母奶比率(%)")]
        public double MA_MMpcntg
        {
            get
            {
                return (MA_MMilk / MA_Total) * 100;
            }
        }




        public static List<MovingAvg> getMovingAvg(int mnum, List<FeedingStatistics> statlst)
        {
            List<MovingAvg> mavglst = new List<MovingAvg>();
            for (int i = 0; i < statlst.Count; i++)
            {
                var mag = new MovingAvg() { Mnum = mnum };
                for (int j = i; j < i + mnum; j++)
                {
                    if (j > statlst.Count - 1)
                        break;
                    mag.StatLst.Add(statlst[j]);
                }

                if (mag.StatLst.Count > 0)
                    mavglst.Add(mag);
            }
            return mavglst;
        }

    }
}
