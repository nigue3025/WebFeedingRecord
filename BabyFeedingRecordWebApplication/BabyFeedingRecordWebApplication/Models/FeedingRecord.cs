using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BabyFeedingRecordWebApplication.Models
{
    //public class FeedingRecordComponentSet
    //{
    //    public IEnumerable<FeedingRecord> Models;
    //    public int PageSize = 30;
        
    //    public FeedingRecordComponentSet(IEnumerable<FeedingRecord>model,int pageSize=30)
    //    {
    //        this.Models = model;
    //        this.PageSize = pageSize;   
    //    }

    //    public int ElementNo 
    //    {
    //        get => Models.Count();
    //    }

    //    public int PageNo
    //    {
    //        get => Models.Count() / PageSize;
    //    }

    //    //JsonSerializer 
  
    //}

    public class FeedingRecord
    {
        public int Id { get; set; }
        [Display(Name ="日期")]
        [DataType(DataType.Date)]
        
        public DateTime FeedingDate { get; set; }

        [Display(Name = "時間")]
        [DataType(DataType.Time)]
        public DateTime FeedingTime { get; set; }

        [Display(Name = "母奶")]
        public int MotherMilkVolume { get; set; }

        [Display(Name = "配方")]
        public int FormularMilkVolume { get; set; }

        public string? Memo { get; set; }



        static bool JudgeLen(string msgStr)
        {
            if (msgStr.Split(msgSplitChar).Length < 2)
            {

                return false;
            }
            return true;
        }
        static DateTime getTime(string timeStr)
        {
            int hr, min;
            try
            {
                hr = int.Parse(timeStr.Substring(0, 2));
                min = int.Parse(timeStr.Substring(2, 2));
            }
            catch
            {
                hr = 0;
                min = 0;
            }
            return new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                hr,
                min,
                0);
        }
        static char[] msgSplitChar = new char[] { ',', '，'};
        static string getMemo(string[] strAry)
        {
            if (strAry.Length < 2)
                return string.Empty;

            var strLst = strAry.ToList();
            strLst.RemoveRange(0, 2);
            return string.Join(',',strLst);
        }

        public static bool CreateFeedingRecord(string msgStr,out FeedingRecord feedingRecord,out string outMsg)
        {
            feedingRecord = new FeedingRecord();
            outMsg = "格式錯誤";

            try
            {
                if (!JudgeLen(msgStr))
                    return false;
                var msgStrAry = msgStr.Split(msgSplitChar);
               
                var timeStr = msgStrAry[0];
                 msgStr = msgStrAry[1];

            
                feedingRecord.MotherMilkVolume = 0;
                feedingRecord.FormularMilkVolume = 0;

                int MMilk = 0;
                int FMilk = 0;
                if (msgStr.Contains("配") || msgStr.Contains("母"))
                {
                    var AllMilk = msgStr.Split(new char[] { '母', '配' });
                    if (AllMilk.Length > 2)
                    {
                        int.TryParse(AllMilk[0], out MMilk);
                        int.TryParse(AllMilk[1], out FMilk);
                    }
                    else
                    {
                        if (msgStr.Contains("母"))
                            int.TryParse(AllMilk[0], out MMilk);
                        else
                            int.TryParse(AllMilk[0], out FMilk);
                    }

                    feedingRecord.FeedingDate = DateTime.Now;
                    feedingRecord.FeedingTime = getTime(timeStr);
                    feedingRecord.FormularMilkVolume = FMilk;
                    feedingRecord.MotherMilkVolume = MMilk;
                    feedingRecord.Memo = getMemo(msgStrAry);


                   
                  
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            outMsg = "";
            return true;
            
        }



    }
}
