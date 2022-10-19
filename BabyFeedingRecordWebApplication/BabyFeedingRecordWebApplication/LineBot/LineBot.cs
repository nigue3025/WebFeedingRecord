using isRock.LineBot;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        public string ServerWebsite = string.Empty;
        public List<LineBotData> LineEvents = new List<LineBotData>();



        public virtual void decodeReceivedMessage(string str)
        {
            throw new NotImplementedException();
        }

        
        public virtual void sendMessage(string replytoken, string msg)
        {
            throw new NotImplementedException();
        }
    }

    public class IsRockLineBot : LineBot
    {
        ReceivedMessage receivedMessage;
        public override void decodeReceivedMessage(string rawstr)
        {
            receivedMessage = Utility.Parsing(rawstr);
            var replytokenlst = receivedMessage.events.Where(a=>a.message!=null).Select(a => a.replyToken).ToList();
            var replymsglst = receivedMessage.events.Where(a => a.message != null).Select(a => a.message.text).ToList();
            var uidlst = receivedMessage.events.Where(a => a.message != null).Select(a => a.source.userId).ToList();
            var gridlst=receivedMessage.events.Where(a => a.message != null).Select(a => a.source.groupId).ToList();
            var rmidlst = receivedMessage.events.Where(a => a.message != null).Select(a => a.source.roomId).ToList();
            LineEvents = LineBotData.createLineBotDataList(replytokenlst, replymsglst, uidlst,gridlst,rmidlst);
        }


        public override void sendMessage(string replytoken, string msg)
        {
            Utility.ReplyMessage(replytoken, msg, ChannelAccessToken);
        }


    }

}

