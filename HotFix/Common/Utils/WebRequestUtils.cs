using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix
{
    public class WebRequestUtils
    {
        private static bool hasInit;
        public static string enterRoomUrl = "horse_match/start";
        public static string myPhotoUrl = "cuser/my_photo";
        public static string listFrontUrl = "horse_user/list_front";
        public static string detailFront = "horse_user/detail_front";
        public static string listFront = "config/public/list_front";
        public static string walkHorse = "horse_user/walk_horse";
        public static string walkHorseEnd = "horse_user/walk_horse_end";
        public static string walkHorseEnd2 = "horse_user/walk_horse_end_rent";
        public static string pageFront = "horse_match_record/page_front";
        public static string milletDetailUrl = "horse_user/detail_millet";
        public static string feedHorseUrl = "horse_user/use_feed";
        public static string purchaseUrl = "horse_buy_order/create";
        public static string purchaseNewUrl = "share_buy_order/create";//购买马粟新接口
        public static string purchaseTypeUrl = "dict/public/list";
        public static string horseMatchFrontUrl = "horse_match/page_front";
        public static string horseMatchRecordUrl = "horse_match_record/page_front";
        public static string horseMatchCreateUrl = "horse_match/create";
        public static string horseMatchJoinUrl = "horse_match/join";
        public static string horseBreadSiteUrl = "horse_breed_site/list_front";
        public static string horseBreadSiteRecordUrl = "horse_breed_site_record/book_breed_site";//公马预约开房间
        public static string horseBreadPayResultsUrl = "horse_breed_site_record/detail_pay_results";
        public static string detailListFrontUrl = "horse_user/detail_list_front";
        public static string breedSiteConfigUrl = "horse_breed_site/breed_site_config";//繁育中房间状态
        public static string bookBreedSiteMaternalUrl = "horse_breed_site_record/book_breed_site_maternal";
        public static string horseGrowUpUrl = "horse_user/detail_front_grow_up";
        public static string horseBirthListUrl = "horse_task/list_front";
        public static string horseUserNameUrl = "horse_user/set_name";
        public static string checkUserUrl = "horse_user/check_user";
        public static string registSDUrl = "sumpay_register_record/sum_user_query";
        public static string checkPwdUrl = "user/check_trade_pwd";//检查是否设置密码，是返回app设置，否
        public static string detailByUser = "account/detailByUser";
        public static string checkRole = "user/check_role";
        public static string enterTheGame = "horse_user_log/enter_the_meta";
        public static string jourMyPage = "jour/my/page_shell";
        public static string milletAccount = "horse_user/millet_account";
        public static string milletTransfer = "transfer_order/millet_transfer";
        public static string getUserPhoto = "cuser/get_user_photo";//通过手机号查询用户名称
        public static string shareGetRecord = "share_get_record/page_front";//份额列表
        public static string shareBuyOrder = "share_buy_order/create";//购买马粟
        public static string payRecordCreate = "horse_pay_record/create";//氪金
        public static string getProperty = "horse_pay_config/detail_front";//获取提高属性所需费用
        public static string gameDetail = "horse_user/horse_match_config_note";//游戏说明
        public static string horseMatchDetail = "horse_match/detail_front";//游戏说明
        public static string myInvite = "cuser/my_invite";//分享
        public static string shareReward = "millet_task_config/share_reward";//分享得马粟

        public static string checkMatch = "horse_match/check_match";//检查是否可以开始游戏
        public static string getQuestion = "horse_question/get_question";//获取题目
        public static string questionRecord = "horse_question_record/answer";//回答问题
        public static string rentHorse = "rent_horse/create";//赛马租赁
        public static string changeRentHorse = "rent_horse/modify";//修改租赁价格
        public static string recycleHorse = "rent_horse/recycle";//回收赛马
        public static string rentHorseDetailFront = "rent_horse/detail_front";//详情查赛马租赁/{id}
        public static string rentHorseList = "rent_horse/page_front";//分页查赛马租赁
        public static string rentHorseMy = "rent_horse/my";//我的出租
        public static string myRentHorse = "rent_horse/rent_horse_my"; //我租赁的马匹
        public static string rentHorseRecord = "rent_horse_record/create";//租赁马匹
        public static string myHistoryRecord = "rent_horse/history_record";//历史订单
        public static string OutLineConnectAgain = "horse_user/detail_user";//断线重连

        public static string frontUrl;
        public static string myToken = "";
        public static void InitUrl(string front,string token) { 
        
            if(hasInit)return;
            frontUrl = front;
            enterRoomUrl = front + enterRoomUrl;
            myPhotoUrl = front + myPhotoUrl;
            listFrontUrl = front + listFrontUrl;
            detailFront = front + detailFront;
            listFront = front + listFront;
            walkHorse = front + walkHorse;
            walkHorseEnd = front + walkHorseEnd;
            walkHorseEnd2 = front + walkHorseEnd2;
            pageFront = front + pageFront;
            milletDetailUrl = front + milletDetailUrl;
            feedHorseUrl=front + feedHorseUrl;
            purchaseUrl=front + purchaseUrl;
            purchaseNewUrl = front + purchaseNewUrl;
            purchaseTypeUrl =front + purchaseTypeUrl;
            horseMatchFrontUrl = front + horseMatchFrontUrl;
            horseMatchRecordUrl = front + horseMatchRecordUrl;
            horseMatchCreateUrl = front + horseMatchCreateUrl;
            horseMatchJoinUrl = front + horseMatchJoinUrl;
            horseBreadSiteUrl = front + horseBreadSiteUrl;
            horseBreadSiteRecordUrl = front + horseBreadSiteRecordUrl;
            horseBreadPayResultsUrl = front + horseBreadPayResultsUrl;
            detailListFrontUrl = front + detailListFrontUrl;
            breedSiteConfigUrl = front + breedSiteConfigUrl;
            bookBreedSiteMaternalUrl = front + bookBreedSiteMaternalUrl;
            horseGrowUpUrl=front + horseGrowUpUrl;
            horseBirthListUrl=front + horseBirthListUrl;
            horseUserNameUrl=front + horseUserNameUrl;
            checkUserUrl = front + checkUserUrl;
            registSDUrl=front + registSDUrl;
            checkPwdUrl = front + checkPwdUrl;
            detailByUser = front + detailByUser;
            checkRole = front + checkRole;
            enterTheGame = front + enterTheGame;
            milletAccount = front + milletAccount;
            milletTransfer = front + milletTransfer;
            jourMyPage = front + jourMyPage;
            getUserPhoto = front + getUserPhoto;
            shareGetRecord = front + shareGetRecord;
            shareBuyOrder = front + shareBuyOrder;
            payRecordCreate = front + payRecordCreate;
            getProperty = front + getProperty;
            gameDetail = front + gameDetail;
            horseMatchDetail = front + horseMatchDetail;
            myInvite = front + myInvite;
            checkMatch = front + checkMatch;
            shareReward = front + shareReward;
            getQuestion = front + getQuestion;
            questionRecord = front + questionRecord;
            rentHorse = front + rentHorse;
            changeRentHorse = front + changeRentHorse;
            recycleHorse = front + recycleHorse;
            rentHorseDetailFront = front + rentHorseDetailFront;
            rentHorseList = front + rentHorseList;
            rentHorseRecord = front + rentHorseRecord;
            rentHorseMy = front + rentHorseMy;
            myRentHorse = front + myRentHorse;
            myHistoryRecord = front + myHistoryRecord;
            OutLineConnectAgain = front + OutLineConnectAgain;
            myToken = token;
            hasInit = true;
        }

    }
}
