using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Dynamic;
using WAPIWrapperCSharp;

namespace WindCore
{
    public class ExpandoJSONConverter : JavaScriptConverter
    {
        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new System.Collections.ObjectModel.ReadOnlyCollection<Type>(new Type[] { typeof(System.Dynamic.ExpandoObject) });
            }
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var result = new Dictionary<string, object>();
            var dictionary = obj as IDictionary<string, object>;
            foreach (var item in dictionary)
            {
                result.Add(item.Key, item.Value);
            }

            return result;
        }
    }
    public class WindAPIService : WindAPI
    {
        public WindAPIService windAPI;
        public WindAPIService()
        {
            this.windAPI = this;
        }

        public string DataDrive(string argxs)//params
        // string windFuncName, string windCodes, string startTime, string endTime, string options = ""
        {
            string[] args = argxs.Split('|');
            string windFuncName = args[0];
            WindData wd;
            // System.Reflection.MethodInfo methodInfo = this.windAPI.GetType().GetMethod(windFuncName);
            // if (methodInfo == null)
            //     throw new ArgumentException("The specified property does not have a public accessor.");
            ///*
            if(windFuncName == "edb" && args.Length <= 5)
            {
            //*/
                System.Reflection.MethodInfo methodInfo = this.windAPI.GetType().GetMethod(windFuncName);
                if (methodInfo == null)
                    throw new ArgumentException("The specified property does not have a public accessor.");
                Func<string, string, string, string, WindData> windFunc = (Func<string, string, string, string, WindData>)Delegate.CreateDelegate(typeof(Func<string, string, string, string, WindData>), windAPI, methodInfo);
                string windCodes = args[1];
                string startTime = args[2];
                string endTime = args[3];
                string options = "Days=Alldays";
                wd = windFunc(windCodes, startTime, endTime, options);
            ///*
            }
            else if(windFuncName == "wsd" && args.Length <= 6)
            {
                Type[] types = new Type[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) };
                System.Reflection.MethodInfo methodInfo = this.windAPI.GetType().GetMethod(windFuncName, types);
                // System.Reflection.MethodInfo methodInfo = this.windAPI.GetType().BaseType.GetMethods()
                // .Where(x => x.Name.ToLower() == windFuncName.ToLower())
                // .FirstOrDefault();
                // .Single()
                if (methodInfo == null)
                    throw new ArgumentException("The specified property does not have a public accessor.");
                Func<string, string, string, string, string, WindData> windFunc = (Func<string, string, string, string, string, WindData>)Delegate.CreateDelegate(typeof(Func<string, string, string, string, string, WindData>), windAPI, methodInfo);
                string windCodes = args[1];
                string fields = args[2];
                string startTime = args[3];
                string endTime = args[4];
                string options = "Days=Alldays";
                wd = windFunc(windCodes, fields, startTime, endTime, options);
            }
            else
                throw new ArgumentException("The specified property does not have a public accessor.");
            //*/
            // Func<string, string, string, WindData> windFunc = (Func<string, string, string, WindData>)Delegate.CreateDelegate(typeof(Func<string, string, string, WindData>), windAPI, methodInfo);
            // WindData wd = windFunc(windCodes, startTime, endTime);

            return WindDataToSerialize(wd, windFuncName);
        }

        static string WindDataToSerialize(WindData wd, string windFuncName)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            serializer.RegisterConverters(new JavaScriptConverter[] { new ExpandoJSONConverter() });

            //请求出错，获取错误信息
            if (wd.errorCode != 0)
            {
                return serializer.Serialize(wd.GetErrorMsg());
            }

            // OutputWindData(wd, "wsd");
            // OutputWindData(wd, "edb");
            // Console.WriteLine(serializer.Serialize(wd.data));

            object[,] odata = (object[,])wd.getDataByFunc(windFuncName, false);
            int nTimeLength = wd.timeList.Length;
            int nCodeLength = wd.codeList.Length;
            int nFieldLength = wd.fieldList.Length;
            int nDataLength = wd.GetDataLength();
            // Console.WriteLine(string.Format("nTimeLength={0} nCodeLength={1} nFieldLength={2}", nTimeLength, nCodeLength, nFieldLength));

            // for (int i = 0; i < odata.GetLength(0);i++ )
            // {
            //     for (int j = 0; j < odata.GetLength(1); j++)
            //     {
            //          Console.WriteLine("维度 {0} {1} 的值为 {2}", i, j, odata[i, j]);
            //     }
            // }
            // Console.WriteLine(nDataLength);

            dynamic[] rows = new ExpandoObject[nDataLength];
            int indexTime = 0;
            int indexCode = 0;
            int indexField = 0;
            for (int i = 0; i < nDataLength; i++)
            {
                indexTime = (int)Math.Ceiling((float)(i+1)/(float)(nCodeLength*nFieldLength)) - 1;
                indexCode = (i)%nCodeLength;
                indexField = (i)%nFieldLength;
                // MAC_IND_API Urban;
                // Urban.END_DATE = wd.timeList[i].ToString("yyyy-MM-dd");
                // Urban.DATA_FREQ_PAR = 5;
                // Urban.MAC_IDX_PAR = 1001;
                // Urban.PRC_UNIT = "万人";
                // Urban.IDX_VAL = Convert.ToSingle(odata[i, 0].ToString());
                // Console.WriteLine(Urban.ToString());

                dynamic o = new ExpandoObject();
                o.time = wd.timeList[indexTime].ToString("yyyy-MM-dd");
                o.code = wd.codeList[indexCode];
                o.field = wd.fieldList[indexField];
                o.value = odata[indexTime, indexCode*nFieldLength+indexField];
                // o.value = Double.IsNaN((double)odata[indexTime, indexCode]) ? 0 : odata[indexTime, indexCode];
                rows[i] = o;
                // Console.WriteLine(string.Format("{0} {1} {2} {3}", o.time, o.code, o.field, o.value));
            }
            String serialization = serializer.Serialize(rows);
            return serialization;
        }

    }
}
