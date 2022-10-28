using NuGet.Protocol;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace BabyFeedingRecordWebApplication.Models
{
    public class FeedingStatisticsListBuilder
    {
        public SortedDictionary<DateTime, List<FeedingRecord>> statDict;
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
        double getPercentage(int theMilk,double totalMilk)
        {
            return ((double) theMilk)/ totalMilk *100;
        }
        

        public List<FeedingStatistics> generateFeedingStatistics()
        {
            List<FeedingStatistics> feedingstatisticList = new List<FeedingStatistics>();
            var revStatDict = statDict.Reverse();
            foreach(var stat in revStatDict)
            {
                int motherMilkSum, formularMilkSum, MilkSum;
                double MMilkPercentage, FMilkPercentage;
                getMilkSum(stat.Value, out motherMilkSum, out formularMilkSum, out MilkSum);
                double intervalAvg = getTimeIntervalAvg(stat.Value);
                FeedingStatistics feedingStatistics = new FeedingStatistics() { 
                    FeedingTime = stat.Key,
                    MotherMilkTotal=motherMilkSum, 
                    FormularMilkTotal=formularMilkSum,
                    Total=MilkSum ,
                    TimeIntervalAvg=intervalAvg,
                    MotherMilkPercentage=getPercentage(motherMilkSum,MilkSum),
                    FormularMilkPercentage=getPercentage(formularMilkSum,MilkSum),
                    feedingRecords=stat.Value.ToList()
                };
                feedingstatisticList.Add(feedingStatistics);

            }
            return feedingstatisticList;
        }

    }
    public class FeedingStatistics
    {
        public List<FeedingRecord>? feedingRecords;

        public int Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime FeedingTime { get; set; }

        public int MotherMilkTotal { get; set; }
        public int FormularMilkTotal { get; set; }

        public double MotherMilkPercentage { get; set; }
        public double FormularMilkPercentage { get; set; }
        public int Total { get; set; }
        public double TimeIntervalAvg{get;set;}

        public string MMilkTotalWithPcntg
        {
            get
            {
                return $"{this.MotherMilkTotal} ({this.MotherMilkPercentage.ToString("0.00")}%)";
            }
        }

        public string FMilkTOtalWithPcntg
        {
            get
            {
                return $"{this.FormularMilkTotal} ({this.FormularMilkPercentage.ToString("0.00")}%)";
            }
        }

    }


   

}
