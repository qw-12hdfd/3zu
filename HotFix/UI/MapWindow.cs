using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix
{
    internal class MapWindow:Window
    {
        private Button siyangchang;
        private Button fanzhichang;
        private Button saimachang;
        private Button close;
        private Camera camera;
        private RawImage rImage;
        public override void Awake(object param1 = null, object param2 = null, object param3 = null)
        {
            siyangchang = m_Transform.Find("MapBack/siyangchang").GetComponent<Button>();
            fanzhichang = m_Transform.Find("MapBack/fanzhichang").GetComponent<Button>();
            saimachang = m_Transform.Find("MapBack/saimachang").GetComponent<Button>();
            close = m_Transform.Find("MapBack/Close").GetComponent<Button>();
            camera = m_Transform.Find("Camera").GetComponent<Camera>();
            rImage = m_Transform.Find("MapBack/RawImage").GetComponent<RawImage>();
            RenderTexture showTest = new RenderTexture(512, 512, 24, RenderTextureFormat.ARGB32);
            camera.targetTexture = showTest;
            rImage.texture = showTest;
            AddButtonClickListener(siyangchang, GoToSiYangChang);
            AddButtonClickListener(fanzhichang, GoToFanZhiChang);
            AddButtonClickListener(saimachang, GoToSaiMaChang);
            AddButtonClickListener(close, () => { UIManager.instance.CloseWnd(this); });
        }

        private void GoToSaiMaChang()
        {
            RFrameWork.instance.OpenCommonConfirm("提示", "您是否要传送到赛马场？", () => { PlayerController.GoToPosition("saimachang"); UIManager.instance.CloseWnd(this);
                MainWindow.GetDownHorse(false);//关闭上马按钮
                if ( UserInfoManager.maCaoTransform != null)
                {
                    MainWindow.MountHorse(false, false);
                    MainWindow.PutFood(false, 0, UserInfoManager.mountHorseID);
                    var ani = UserInfoManager.maCaoTransform.Find("HorsePos").GetChild(0).GetComponent<Animator>();
                    if (!ani.GetBool("Eat") && UserInfoManager.maCaoTransform.Find("fodders").GetChild(0).gameObject.active && !UserInfoManager.playerCtrl.mount)
                        ani.SetBool("Eat", true);
                    UserInfoManager.maCaoTransform = null;
                }
            }, () => { });
        }

        private void GoToFanZhiChang()
        {
            RFrameWork.instance.OpenCommonConfirm("提示", "您是否要传送到繁育场？", () => { PlayerController.GoToPosition("fanzhichang"); UIManager.instance.CloseWnd(this);
                MainWindow.GetDownHorse(false);//关闭上马按钮
                if (UserInfoManager.maCaoTransform != null)
                {
                    MainWindow.MountHorse(false, false);
                    MainWindow.PutFood(false, 0, UserInfoManager.mountHorseID);
                    var ani = UserInfoManager.maCaoTransform.Find("HorsePos").GetChild(0).GetComponent<Animator>();
                    if (!ani.GetBool("Eat") && UserInfoManager.maCaoTransform.Find("fodders").GetChild(0).gameObject.active && !UserInfoManager.playerCtrl.mount)
                        ani.SetBool("Eat", true);
                    UserInfoManager.maCaoTransform = null;
                }
            }, () => { });
        }

        private void GoToSiYangChang()
        {
            RFrameWork.instance.OpenCommonConfirm("提示", "您是否要传送到饲养场？", () => { PlayerController.GoToPosition("siyangchang"); UIManager.instance.CloseWnd(this);
                MainWindow.GetDownHorse(false);//关闭上马按钮
                if (UserInfoManager.maCaoTransform != null)
                {
                    MainWindow.MountHorse(false, false);
                    MainWindow.PutFood(false, 0, UserInfoManager.mountHorseID);
                    var ani = UserInfoManager.maCaoTransform.Find("HorsePos").GetChild(0).GetComponent<Animator>();
                    if (!ani.GetBool("Eat") && UserInfoManager.maCaoTransform.Find("fodders").GetChild(0).gameObject.active && !UserInfoManager.playerCtrl.mount)
                        ani.SetBool("Eat", true);
                    UserInfoManager.maCaoTransform = null;
                }
            }, ()=> { });
        }
    }
}
