using NuGet.Protocol;
using System.ComponentModel.DataAnnotations;

namespace BabyFeedingRecordWebApplication.Models
{
    public class FeedingStatisticsListBuilder
    {
        SortedDictionary<DateTime, List<FeedingRecord>> statDict;
        public FeedingStatisticsListBuilder()
        {
            statDict = new SortedDictionary<DateTime, List<FeedingRecord>>();
        }

        DateTime ToDateNoTime(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
        }

        public void Add(List<FeedingRecord> feedingRecords)
        {
            foreach (var feedingrecord in feedingRecords)
                Add(feedingrecord);
        }

        public void Add(FeedingRecord feedingRecord)
        {
            var DateOnly = ToDateNoTime(feedingRecord.FeedingTime);
            if (!statDict.ContainsKey(DateOnly))
                statDict[DateOnly] = new List<FeedingRecord>();
            statDict[DateOnly].Add(feedingRecord);
        }

        void getMilkSum(List<FeedingRecord> feedingRecords,out int MMilk,out int FMilk,out int totalMilk)
        {
            MMilk = 0;
            FMilk = 0;
            totalMilk = 0;
            foreach (var record in feedingRecords)
            {
                MMilk += record.MotherMilkVolume;
                FMilk += record.FormularMilkVolume;
            }
            totalMilk = MMilk + FMilk;
        }

        double getTimeIntervalAvg(List<FeedingRecord> feedingRecords)
        {
            double avg = 0;
            var dtLst=feedingRecords.Select(a => a.FeedingTime).ToList();
            dtLst.Sort();
            if (dtLst.Count > 1)
            {
                for (int i = 1; i < dtLst.Count; i++)
                    avg = avg + (dtLst[i] - dtLst[i - 1]).TotalHours;
                avg = Math.Round(avg / ((double)(dtLst.Count - 1)), 2);
            }
            return avg;
        }   

        public List<FeedingStatistics> generateFeedingStatistics()
        {
            List<FeedingStatistics> feedingstatisticList = new List<FeedingStatistics>();
            var revStatDict = statDict.Reverse();
            foreach(var stat in revStatDict)
            {
                int motherMilkSum, formularMilkSum, MilkSum;

                getMilkSum(stat.Value, out motherMilkSum, out formularMilkSum, out MilkSum);
                double intervalAvg = getTimeIntervalAvg(stat.Value);
                FeedingStatistics feedingStatistics = new FeedingStatistics() { FeedingTime = stat.Key,MotherMilkTotal=motherMilkSum, FormularMilkTotal=formularMilkSum,Total=MilkSum ,TimeIntervalAvg=intervalAvg};
                feedingstatisticList.Add(feedingStatistics);

            }
            return feedingstatisticList;
        }

    }
    public class FeedingStatistics
    {
        public int Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime FeedingTime { get; set; }

        public int MotherMilkTotal { get; set; }
        public int FormularMilkTotal { get; set; }

        public int Total { get; set; }
        public double TimeIntervalAvg{get;set;}

    }
}
