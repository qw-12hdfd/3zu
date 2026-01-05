using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using Application = UnityEngine.Application;
using Text = UnityEngine.UI.Text;

namespace HotFix
{
    internal class ShareWindow:Window
    {
        public RawImage image;
        public GameObject logoImage;
        public RawImage icon;
        public RawImage invitePhoto;
        public GameObject erweima;
        public GameObject inviteObject1;
        public GameObject inviteObject2;
        public GameObject object3;
        public Text name;
        public Text feedNum;
        public Text inviteNum;
        public Text inviteNum2;
        public Text inviteCode;
        public Text des;
        public Button copyNum;
        public Button copuCode;
        public Button returnBtn;
        public Button weChat;
        public Button circle;
        public Button saveImage;
        public GameObject blackPanel;
        string idStr;//id,photo,nickname,inviteCode,inviteUrl,inviteUrlPhoto
        string photoStr;
        string nicknameStr;
        string inviteCodeStr;
        string inviteUrlStr;
        string inviteUrlPhotoStr;
        string milletQuantityStr;

        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            FindAllComponent();
            AddAllBtnsListener();
        }

        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            image.texture = param1 as Texture2D;
            object[] obj = param2 as object[];
            idStr = obj[0].ToString();
            photoStr = obj[1].ToString();
            nicknameStr = obj[2].ToString();
            inviteCodeStr = obj[3].ToString();
            inviteUrlStr = obj[4].ToString();
            inviteUrlPhotoStr = obj[5].ToString();
            milletQuantityStr = obj[6].ToString();
            UpdateUI();
        }

        private void UpdateUI()
        {
            WebRequestManager.instance.AsyncLoadUnityTexture(inviteUrlPhotoStr, (texture) =>
            {
                invitePhoto.texture = texture;
            });
            WebRequestManager.instance.AsyncLoadUnityTexture(photoStr, (texture) =>
            {
                icon.texture = texture;
            });
            name.text = nicknameStr;
            if (float.Parse(milletQuantityStr) > 0)
            {
                feedNum.text = "+" + (int)float.Parse(milletQuantityStr);
            }
            else
            {
                object3.SetActive(false);
            }
            des.text = UserInfoManager.rankStr; // "我在赛马比赛中获得了「第" + rankStr + "名」，你也来试试吧～";
            inviteNum.text = inviteCodeStr;
            inviteNum2.text = inviteCodeStr;
            inviteCode.text = inviteUrlStr;
        }

        private void FindAllComponent()
        {
            image = m_Transform.Find("Back/Bg/Image").GetComponent<RawImage>();
            logoImage = m_Transform.Find("Back/Bg/Image/Tips").gameObject;
            blackPanel = m_Transform.Find("Back/BlackPanel").gameObject;
            erweima = m_Transform.Find("Back/Bg/QRCode").gameObject;
            inviteObject1 = m_Transform.Find("Back/InviteCodeBack").gameObject;
            inviteObject2 = m_Transform.Find("Back/InviteCodeLinkBack").gameObject;
            object3 = m_Transform.Find("Back/Bg/Back").gameObject;
            icon = m_Transform.Find("Back/Bg/Icon").GetComponent<RawImage>();
            icon = m_Transform.Find("Back/Bg/Icon").GetComponent<RawImage>();
            invitePhoto = m_Transform.Find("Back/Bg/QRCode").GetComponent<RawImage>();
            name = m_Transform.Find("Back/Bg/Icon/Name").GetComponent<Text>();
            feedNum = m_Transform.Find("Back/Bg/Back/Image/Num").GetComponent<Text>();
            inviteNum = m_Transform.Find("Back/InviteCodeBack/Num").GetComponent<Text>();
            inviteNum2 = m_Transform.Find("Back/Bg/QRCode/InviteCodeNum").GetComponent<Text>();
            inviteCode = m_Transform.Find("Back/InviteCodeLinkBack/Num").GetComponent<Text>();
            des = m_Transform.Find("Back/Bg/Icon/Des").GetComponent<Text>();
            copyNum = m_Transform.Find("Back/InviteCodeBack/Text (Legacy)").GetComponent<Button>();
            copuCode = m_Transform.Find("Back/InviteCodeLinkBack/Text (Legacy)").GetComponent<Button>();
            returnBtn = m_Transform.Find("Back/ReturnBtn").GetComponent<Button>();
            weChat = m_Transform.Find("Back/Btns/WeChat").GetComponent<Button>();
            circle = m_Transform.Find("Back/Btns/CircleOfFriends").GetComponent<Button>();
            saveImage = m_Transform.Find("Back/Btns/SaveImage").GetComponent<Button>();
        }

        private void AddAllBtnsListener()
        {
            AddButtonClickListener(copyNum, CopyNumFunc);
            AddButtonClickListener(copuCode, CopyCodeFunc);
            AddButtonClickListener(returnBtn, ReturnFunc);
            AddButtonClickListener(weChat, WechatFunc);
            AddButtonClickListener(circle, CircleFunc);
            AddButtonClickListener(saveImage, SaveFunc);
        }

        private void CopyNumFunc()
        {
            //复制邀请码 TODO
            //RFrameWork.instance.CopyStr(inviteCodeStr);
        }

        private void CopyCodeFunc()
        {
            //复制邀请链接 TODO
            //RFrameWork.instance.CopyStr(inviteUrlStr);
        }

        private void ReturnFunc()
        {
            //关闭界面 TODO
            UIManager.instance.CloseWnd(this);
        }

        private void WechatFunc()
        {
            //微信分享 TODO
            DateTime time = DateTime.Now;
            IEnumeratorTool.instance.StartCoroutineNew(ShareWX(time));
        }

        private IEnumerator ShareWX(DateTime time)
        {
            erweima.SetActive(true);
            inviteObject1.SetActive(false);
            inviteObject2.SetActive(false);
            object3.SetActive(false);
            blackPanel.SetActive(true);
            yield return new WaitForEndOfFrame();
            Texture2D screenShot = ScreenCapture.CaptureScreenshotAsTexture();
            screenShot.Apply();
            byte[] bytes = screenShot.EncodeToPNG();
            Texture2D.Destroy(screenShot);
            string path = RFrameWork.instance.photoPath + "/YuanNianWX_" + ((time.ToUniversalTime().Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0).Ticks) / 10000000) * 1000 + ".png";
            yield return new WaitForSeconds(1);
            File.WriteAllBytes(path, bytes);
            Debug.Log("保存图片路径为" + path);
            yield return new WaitForEndOfFrame();
            blackPanel.SetActive(false);
            inviteObject1.SetActive(true);
            inviteObject2.SetActive(true);
            object3.SetActive(float.Parse(milletQuantityStr) > 0);
            erweima.SetActive(false);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            //RFrameWork.instance.CallShareWX(path);
        }

        private void CircleFunc()
        {
            //朋友圈分享 TODO
            DateTime time = DateTime.Now;
            IEnumeratorTool.instance.StartCoroutineNew(SharePYQ(time));
        }

        private IEnumerator SharePYQ(DateTime time)
        {
            erweima.SetActive(true);
            inviteObject1.SetActive(false);
            inviteObject2.SetActive(false);
            object3.SetActive(false);
            blackPanel.SetActive(true);
            yield return new WaitForEndOfFrame();
            Texture2D screenShot = ScreenCapture.CaptureScreenshotAsTexture();
            screenShot.Apply();
            byte[] bytes = screenShot.EncodeToPNG();
            Texture2D.Destroy(screenShot);
            string path = RFrameWork.instance.photoPath + "/YuanNianPYQ_" + ((time.ToUniversalTime().Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0).Ticks) / 10000000) * 1000 + ".png";
            yield return new WaitForSeconds(1);
            File.WriteAllBytes(path, bytes);
            Debug.Log("保存图片路径为" + path);
            yield return new WaitForEndOfFrame();
            blackPanel.SetActive(false);
            inviteObject1.SetActive(true);
            inviteObject2.SetActive(true);
            object3.SetActive(float.Parse(milletQuantityStr) > 0);
            erweima.SetActive(false);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            //RFrameWork.instance.CallSharePYQ(path);
        }

        private void SaveFunc()
        {
            DateTime time = DateTime.Now;
            //保存图片 TODO
            IEnumeratorTool.instance.StartCoroutineNew(ShareCutPic(time));
        }

        public IEnumerator ShareCutPic(DateTime time)
        {
            erweima.SetActive(true);
            inviteObject1.SetActive(false);
            inviteObject2.SetActive(false);
            object3.SetActive(false);
            blackPanel.SetActive(true);
            yield return new WaitForEndOfFrame();
            Texture2D screenShot = ScreenCapture.CaptureScreenshotAsTexture();
            screenShot.Apply();
            byte[] bytes = screenShot.EncodeToPNG(); 
            Texture2D.Destroy(screenShot);
            string path = RFrameWork.instance.photoPath + "/YuanNian_" + ((time.ToUniversalTime().Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0).Ticks) / 10000000) * 1000 + ".png";
            yield return new WaitForSeconds(1);
            File.WriteAllBytes(path, bytes);
            Debug.Log("保存图片路径为" + path);
            yield return new WaitForEndOfFrame();
            blackPanel.SetActive(false);
            inviteObject1.SetActive(true);
            object3.SetActive(float.Parse(milletQuantityStr) > 0);
            erweima.SetActive(false);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            //RFrameWork.instance.SaveImgPath(path);
        }
    }
}
