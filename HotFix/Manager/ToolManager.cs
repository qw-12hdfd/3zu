
using HotFix.Common;
using LitJson;
using MalbersAnimations.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HotFix
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class ToolManager
    {
        public static void SetStateOfShare(string info)
        {
            JsonData jsonData = JsonMapper.ToObject(info);
            string result = (string)jsonData["result"];
            if (result.Equals("succeed"))
            {
                RFrameWork.instance.OpenCommonConfirm("提示", "分享成功", () => {
                    WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.shareReward, ShareRewardFunc, true, "{}", RFrameWork.instance.token);
                }, null);
            }
            else
            {

                RFrameWork.instance.OpenCommonConfirm("提示", "分享失败:" + result, () => { }, null);
            }
        }

        private static void ShareRewardFunc(string obj)
        {
            WebRequestManager.instance.AsyncLoadUnityWebRequest(WebRequestUtils.milletAccount, MainWindow.RefreshNumData, true, "{}", RFrameWork.instance.token);
        }

        public static string privacyUrl;
        /// <summary>
        /// 退出游戏
        /// </summary>
        public static void ExitGame()
        {
            Debug.Log("ExitGame 退出游戏");
            JsonData data = new JsonData();
            MsgBase close = new MsgBase("Close", data.ToJson());
            UserInfoManager.Init();
            RFrameWork.instance.QuitTheGame();
            NetManager.instance.Send(close);
            NetManager.instance.Close();
        }
        /// <summary>
        /// 退出游戏
        /// </summary>
        public static void StartExitGame()
        {
            Debug.Log("StartExitGame 退出游戏");
            JsonData data = new JsonData();
            MsgBase close = new MsgBase("Close", data.ToJson());
            RFrameWork.instance.QuitTheGame();
            NetManager.instance.Send(close);
            NetManager.instance.Close();
        }
        public static void SetFrontUrl(string frontUrl)
        {
            Debug.Log("FrontUrl:"+frontUrl);
            if(frontUrl.Equals("dev"))
            {
                privacyUrl = "http://front.dev.hzyuzhouyuan.com/sysConfigDetail?key=horse_buy_textarea&title=马术元宇宙购买协议";
                System.Uri uri = new System.Uri(privacyUrl);
                privacyUrl = uri.AbsoluteUri;
            }else if (frontUrl.Equals("test"))
            {
                privacyUrl = "http://front.test.hzyuzhouyuan.com/sysConfigDetail?key=horse_buy_textarea&title=马术元宇宙购买协议";
                System.Uri uri = new System.Uri(privacyUrl);
                privacyUrl = uri.AbsoluteUri;
            }
            else
            {
                privacyUrl = "http://front.hzyuzhouyuan.com/sysConfigDetail?key=horse_buy_textarea&title=马术元宇宙购买协议"; 
                System.Uri uri = new System.Uri(privacyUrl);
                privacyUrl = uri.AbsoluteUri;
            }
            Debug.Log("privacyUrl:" + privacyUrl);
        }
        /// <summary>                        
        /// thi Transform root给Transform添加扩展方法，扩展方法为在根节点查找name的子物体
        /// 查找子物体
        /// </summary>
        /// <param name="root"></需要查找物体的根节点>
        /// <param name="name"></需要查找子物体的名称>
        /// <returns></returns>
        public static Transform FindRecursively(this Transform root, string name)
        {

            if (root.name == name)
            {
                return root;
            }
            //遍历根节点下的所有子物体
            foreach (Transform child in root)
            {
                //递归查找子物体
                Transform t = FindRecursively(child, name);
                if (t != null)
                {
                    return t;
                }
            }
            return null;
        }

        /// <summary>
        /// 查找root身上的monobehaviour组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T FindBehaviour<T>(this Transform root, string name) where T : MonoBehaviour
        {
            Transform child = FindRecursively(root, name);

            if (child == null)
            {
                return null;
            }

            T temp = child.GetComponent<T>();
            if (temp == null)
            {
                Debug.LogError(name + " is not has component ");
            }

            return temp;
        }


        /// <summary>
        /// 字符串转整型;
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int Str2Int(string str)
        {
            if (IsNumber(str))
            {
                return string.IsNullOrEmpty(str) == true ? 0 : Convert.ToInt32(str);
            }
            return 0;

        }
        /// <summary>
        /// 常量A是否全是数字
        /// </summary>
        public static string A = "^[0-9]+$";
        /// <summary>
        /// 利用正则表达式来判断字符串是否全是数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static bool IsNumber(string str)
        {
            Regex regex = new Regex(A);
            return regex.IsMatch(str) == true ? true : false;
        }
        /// <summary>
        /// 字符串转成float;
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static float Str2Float(string str)
        {
            return string.IsNullOrEmpty(str) == true ? 0 : Convert.ToSingle(str);
        }
        /// <summary>
        /// 字符串转成bool;
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool Str2Boolean(string str)
        {
            if (str == "1" || str.ToLower() == "true" || str.ToLower() == "yes")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 设置预制体包括全部子级的layer
        /// </summary>
        /// <param name="root"></param>
        /// <param name="layer"></param>
        public static void SetChildrensLayer(Transform root,int layer)
        {
            Transform[] all = root.GetComponentsInChildren<Transform>();
            foreach (var item in all)
            {
                item.gameObject.layer = layer;
            }
        }

        private static IEnumerator ShareCutPic(string id, string photo, string nickname, string inviteCode, string inviteUrl, string inviteUrlPhoto,string milletQuantity)
        {
            if (GameMapManager.instance.CurrentMapName == ConStr.MAINSCENE)
                UIManager.instance.CloseWnd(FilesName.MAINPANEL);
            yield return new WaitForEndOfFrame();
            if (GameMapManager.instance.CurrentMapName == ConStr.MAINSCENE)
                UIManager.instance.PopUpWnd(FilesName.MAINPANEL,false,false);
            Texture2D screenShot = ScreenCapture.CaptureScreenshotAsTexture();
            screenShot.Apply();
            object[] obj = new object[] { id, photo, nickname, inviteCode, inviteUrl, inviteUrlPhoto , milletQuantity };
            UIManager.instance.PopUpWnd(FilesName.SHAREPANEL, true, false, screenShot,obj);
        }

        internal static void ShareMsgToApp(string id, string photo, string nickname, string inviteCode, string inviteUrl, string inviteUrlPhoto,string milletQuantity)
        {
            IEnumeratorTool.instance.StartCoroutineNew(ShareCutPic(id,photo,nickname,inviteCode,inviteUrl,inviteUrlPhoto, milletQuantity));
        }
    }
}
