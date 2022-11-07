using isRock.LineBot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Cryptography;
using System.Text;

namespace BabyFeedingRecordWebApplication.LineBot
{

    public class LineBotData
    {
        public string replyToken;
        public string ReceivedMessage;
        public string uid;
        public string groupid;
        public string roomid;


        public static List<LineBotData> createLineBotDataList(List<string> tokens, List<string> msgs, List<string> uids,List<string> groupids,List<string>roomids)
        {
            List<LineBotData> lineBotDatallst = new List<LineBotData>();
            for (int i = 0; i < tokens.Count; i++)
                lineBotDatallst.Add(new LineBotData()
                {
                    replyToken = tokens[i],
                    ReceivedMessage = msgs[i],
                    uid = uids[i],
                    groupid = groupids[i],
                    roomid = roomids[i]
                });
            return lineBotDatallst;
        }
    }


    public abstract class LineBot
    {
        public string ChannelAccessToken = string.Empty;
        public string ChannelSecrete = string.Empty;
        public string ServerWebsite = string.Empty;
        public List<LineBotData> LineEvents = new List<LineBotData>();
        public List<LineBotData> UnapprovedLineEvents = new List<LineBotData>();
        public List<string> permittedUids = new List<string>();
        public List<string> permittedGids = new List<string>();


        public virtual void decodeReceivedMessage(string str)
        {
            throw new NotImplementedException();
        }
        public virtual bool validateSignature(IHeaderDictionary hd, string bodyStr)
        {
            throw new NotImplementedException();
        }
        
        public virtual bool validateSignature(string channelSecrete)
        {
            throw new NotImplementedException();
        }

        public virtual void sendMessage(string replytoken, string msg)
        {
            throw new NotImplementedException();
        }

        protected bool CheckGroupValid(string gid)
        {
            
            return permittedGids.Contains(gid);
        }

        protected bool CheckUidValid(string uid)
        {
            return permittedUids.Contains(uid);
        }


    }

    public class IsRockLineBot : LineBot
    {
        ReceivedMessage receivedMessage;
        public override void decodeReceivedMessage(string rawstr)
        {
            receivedMessage = Utility.Parsing(rawstr);
            var receivedMessageEvents = receivedMessage.events.ToList();


            var unApporvedEvent = receivedMessageEvents.Where((a) => !checkValid(a)).ToList(); //過濾出未允許的帳號,尚未處理
            receivedMessageEvents = receivedMessageEvents.Where((a) => checkValid(a)).ToList();

            var replytokenlst = receivedMessageEvents.Where(a=>a.message!=null).Select(a => a.replyToken).ToList();
            var replymsglst = receivedMessageEvents.Where(a => a.message != null).Select(a => a.message.text).ToList();
            var uidlst = receivedMessageEvents.Where(a => a.message != null).Select(a => a.source.userId).ToList();
            var gridlst= receivedMessageEvents.Where(a => a.message != null).Select(a => a.source.groupId).ToList();
            var rmidlst = receivedMessageEvents.Where(a => a.message != null).Select(a => a.source.roomId).ToList();
            LineEvents = LineBotData.createLineBotDataList(replytokenlst, replymsglst, uidlst,gridlst,rmidlst);


            var ureplytokenlst = unApporvedEvent.Where(a => a.message != null).Select(a => a.replyToken).ToList();
            var ureplymsglst = unApporvedEvent.Where(a => a.message != null).Select(a => a.message.text).ToList();
            var uuidlst = unApporvedEvent.Where(a => a.message != null).Select(a => a.source.userId).ToList();
            var ugridlst = unApporvedEvent.Where(a => a.message != null).Select(a => a.source.groupId).ToList();
            var urmidlst = unApporvedEvent.Where(a => a.message != null).Select(a => a.source.roomId).ToList();
            UnapprovedLineEvents= LineBotData.createLineBotDataList(ureplytokenlst, ureplymsglst, uuidlst, ugridlst, urmidlst);
        }


        bool checkValid(Event anEvent)
        {
            if (anEvent.source.type == "group")
                return CheckGroupValid(anEvent.source.groupId);
            return CheckUidValid(anEvent.source.userId);
        }
     

        public override void sendMessage(string replytoken, string msg)
        {
            Utility.ReplyMessage(replytoken, msg, ChannelAccessToken);
        }

        public override bool validateSignature(IHeaderDictionary hd,string bodyStr)
        {
            if (hd.TryGetValue("X-Line-Signature", out var value))
            {
                string? xlineSignature = value.FirstOrDefault();
                if (Convert.ToBase64String(new HMACSHA256(Encoding.UTF8.GetBytes(ChannelSecrete)).ComputeHash(Encoding.UTF8.GetBytes(bodyStr))) == xlineSignature)
                {
                    return true;
                }
            }
            return false;
        }

    }

}

