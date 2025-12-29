using HotFix.Common;
using HotFix.Common.Utils;
using LitJson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace HotFix
{
    public class PurchaseManager
    {
        public static void SetStateOfPayment(string info)
        {
            JsonData jsonData = JsonMapper.ToObject(info);
            string result = (string)jsonData["result"];
            if (result.Equals("succeed"))
            {
                RFrameWork.instance.OpenCommonConfirm("提示", "支付成功", () => {
                    JsonData data = new JsonData();
                    data["payOrderCode"] = UserInfoManager.payOrderCode;
                    string jsonText = data.ToJson();
                    Debug.Log(UserInfoManager.payOrderCode + "   支付code");
                    UIManager.instance.CloseWnd(FilesName.PURCHASEPANEL);

                    switch (UserInfoManager.payType)
                    {
                        case PayType.HorseFeed:
                            Debug.Log("购买马粟成功");
                             UIManager.instance.CloseWnd(FilesName.PURCHASEHORSEMILLETPANEL);
                            //WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletDetailUrl,WebRequestFuncitons.GetMilletDetailOnly, true, "{}", RFrameWork.instance.token);
                            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount, MainWindow.RefreshNumData, true, "{}", RFrameWork.instance.token);
                            break;
                        case PayType.BreedRoom:
                            UIManager.instance.CloseAllWnd();
                            UIManager.instance.PopUpWnd(FilesName.MAINPANEL, false, false);
                            Debug.Log("购买房间成功");
                            //UIManager.instance.CloseWnd(FilesName.ORDEROPENHOUSEPANEL);
                            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.horseBreadPayResultsUrl, WebRequestFuncitons.HorseBreedSuccess, true, jsonText, RFrameWork.instance.token);
                            break;
                        case PayType.BreedFeed:
                            Debug.Log("购买配种马粟成功");
                             UIManager.instance.CloseWnd(FilesName.PURCHASEBREEDCHARGEPANEL);
                            //WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletDetailUrl, WebRequestFuncitons.GetMilletDetailOnly, true, "{}", RFrameWork.instance.token);
                            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount, MainWindow.RefreshNumData, true, "{}", RFrameWork.instance.token);
                            break;
                        default:
                            break;
                    }
                }, null);
            }
            else
            {
                 
                RFrameWork.instance.OpenCommonConfirm("提示", "支付失败:"+ result, () => { }, null);
            }
        }

        public static void CallWeChatPay(string appId, string merchantId, string prepayId, string wechatPackage, string nonceStr, string timeStamp, string sign)
        {
        Debug.Log("AndroidManager CallWeChatPay:" + appId + "  " + merchantId + "  " + prepayId); 
        //RFrameWork.instance.CallWeChatPay(appId, merchantId, prepayId, wechatPackage, nonceStr, timeStamp, sign);

        }
        public static void CallALiPay(string signOrder)
        {
            Debug.Log("AndroidManager CallALiPay:" + signOrder);
            //RFrameWork.instance.CallALiPay(signOrder);

        }
        public static void CallH5Pay(string url)
        {
            Debug.Log("AndroidManager CallH5Pay:" + url);
            //RFrameWork.instance.CallH5Pay(url);

        }
    }
}
