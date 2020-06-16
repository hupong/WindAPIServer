using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Dynamic;
using WindCore;
using SocketService;

namespace WindAPIServer
{

    // struct MAC_IND_API
    // {
    //    public string END_DATE;
    //    public int DATA_FREQ_PAR;
    //    public int MAC_IDX_PAR;
    //    public string PRC_UNIT;
    //    public float IDX_VAL;
    //    public override string ToString()
    //    {
    //         if(!string.IsNullOrEmpty(PRC_UNIT) && !string.IsNullOrEmpty(IDX_VAL.ToString()))
    //             return string.Format("{0} {1} {2} {3} {4}", END_DATE, DATA_FREQ_PAR, MAC_IDX_PAR, PRC_UNIT, IDX_VAL);
    //         else return "";
    //         // JavaScriptSerializer serializer = new JavaScriptSerializer();
    //         // serializer.MaxJsonLength = Int32.MaxValue;
    //         // String serialization = serializer.Serialize(this);
    //         // return serialization;
    //    }
    // }
    // public interface DataAPIService {}
    // public class WindDataAPIService : WindDataAPI //, DataAPIService
    // {

    // }

    class DataService
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start……");
            // Console.ReadLine();


            // string apiData = "";
            //wset取沪深300指数成分
            //WindData wd = w.wset("IndexConstituent", "date=20141215;windcode=000300.SH");
            //OutputWindData(wd, "wset");
            // WindData wd = w.wsd("600000.SH,600004.SH", "open", "2014-10-16", "2014-12-16", "");
            // OutputWindData(wd, "wsd");
            // apiData = DataService(windAPIService, "wss", "600267.SH,600276.SH,002038.SZ", "oper_rev,opprofit,net_profit_is", "unit=1;rptDate=20161231;rpttYPE=1", "");
            // Console.WriteLine(apiData);
            // apiData = DataService(windAPIService, "edb", "M0024135,M5206729,M0024136", "2018-10-01", "2018-11-09", "");
            // Console.WriteLine(apiData);
            WindAPIService windAPIService = CreateDataAPIService();
            windAPIService.start();
            SocketServer socketServer = CreateSocketService();
            socketServer.StartService(windAPIService.DataDrive);

            windAPIService.stop();
            Console.WriteLine("End......");
            Console.ReadLine();
        }
        public static WindAPIService CreateDataAPIService()
        {
            WindAPIService windAPIService = new WindAPIService();
            return windAPIService;
        }
        public static SocketServer CreateSocketService()
        {
            SocketServer socketServer = new SocketServer();
            return socketServer;
        }
    }
}
