using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

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
        [Display(Name ="更新時間")]
        public DateTime FeedingDate { get; set; }

        [Display(Name = "餵食時間")]
        //[DataType(DataType.Time)]
        public DateTime FeedingTime { get; set; }

        [Display(Name = "母奶")]
        public int MotherMilkVolume { get; set; }

        [Display(Name = "配方")]
        public int FormularMilkVolume { get; set; }

        [Display(Name ="副食品")]
        public int BabyFoodVolume { get; set; }
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
                timeStr=timeStr.Trim();
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

        enum eMilkType { eMmilk,eFmilk, eBabyfood } //對應foodTypeCharAry
        static char[] foodTypeCharAry = new char[] { '母', '配', '副' }; //對應eMilkType

        static bool containFoodType(string msgStr,out List<char> foodTypeLst)
        {
            bool containFood = false;
            foodTypeLst = new List<char>();
            foreach (char msgchr in msgStr)
            {
                if (foodTypeCharAry.Contains(msgchr))
                {
                    containFood = true;
                    foodTypeLst.Add(msgchr);
                }
            }
            return containFood;
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
                int[] milktypeVolume = new int[Enum.GetNames(typeof(eMilkType)).Length] ;
                milktypeVolume = milktypeVolume.Select(a => a = 0).ToArray();
                List<char> containedfoodTypeList;
                if(containFoodType(msgStr,out containedfoodTypeList))
                {
                    var AllMilk = msgStr.Split(foodTypeCharAry);
                    var foodTypeCharLst = foodTypeCharAry.ToList();
                    foreach (var foodElmnt in containedfoodTypeList)
                    {
                        int currFoodTypeIndex = foodTypeCharLst.IndexOf(foodElmnt);
                        int.TryParse(AllMilk[containedfoodTypeList.IndexOf(foodElmnt)],out milktypeVolume[currFoodTypeIndex]);
                    }
                    feedingRecord.FeedingDate = DateTime.Now;
                    feedingRecord.FeedingTime = getTime(timeStr);
                    feedingRecord.FormularMilkVolume = milktypeVolume[(int)eMilkType.eFmilk];
                    feedingRecord.MotherMilkVolume = milktypeVolume[(int)eMilkType.eMmilk];
                    feedingRecord.BabyFoodVolume = milktypeVolume[(int)eMilkType.eBabyfood];
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
