using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ManyPasswords.ViewModel
{
    public partial class PasswordViewModel : ViewModelBase
    {
        private static readonly Lazy<PasswordViewModel> lazy = new Lazy<PasswordViewModel>(() => new PasswordViewModel());
        public static PasswordViewModel Instance { get { return lazy.Value; } }

        // 内置账号模板集合
        public ObservableCollection<Models.BuildinItem> vBuildinItems = null;

        // 所有的账号集合
        public List<Models.PasswordItem> vAllPasswords = null;

        // 按照首字母分组的账号集合
        private ObservableCollection<Models.PasswordsGroup> _vManyPasswords = null;
        public ObservableCollection<Models.PasswordsGroup> vManyPasswords
        {
            get { return _vManyPasswords; }
            set { Set("vManyPasswords", ref _vManyPasswords, value); }
        }

        // 我的收藏
        private ObservableCollection<Models.PasswordItem> _vFavoritePasswords = null;
        public ObservableCollection<Models.PasswordItem> vFavoritePasswords
        {
            get { return _vFavoritePasswords; }
            set { Set("vFavoritePasswords", ref _vFavoritePasswords, value); }
        }

        // 搜索建议列表
        private List<Models.PasswordItem> _vSearchSuggestions = null;
        public List<Models.PasswordItem> vSearchSuggestions
        {
            get { return _vSearchSuggestions; }
            set { Set("vSearchSuggestions", ref _vSearchSuggestions, value); }
        }

        // 当前选中查看的密码
        private Models.PasswordItem _CurrentPassword = null;
        public Models.PasswordItem CurrentPassword
        {
            get { return _CurrentPassword; }
            set { Set("CurrentPassword", ref _CurrentPassword, value); }
        }

        // 当前编辑的密码
        private Models.PasswordItem _EditingTempPassword = null;
        public Models.PasswordItem EditingTempPassword
        {
            get { return _EditingTempPassword; }
            set { Set("EditingTempPassword", ref _EditingTempPassword, value); }
        }

        // 是否启用了Windows Hello锁定功能
        private bool _bLockEnabled = true;
        public bool bLockEnabled
        {
            get { return _bLockEnabled; }
            set { Set("bLockEnabled", ref _bLockEnabled, value); }
        }

        // 锁定App
        private bool _bAppLocked = true;
        public bool bAppLocked
        {
            get { return _bAppLocked; }
            set { Set("bAppLocked", ref _bAppLocked, value); }
        }

        // 应用程序的主题
        private ElementTheme _eAppTheme = ElementTheme.Light;
        public ElementTheme eAppTheme
        {
            get { return _eAppTheme; }
            set
            {
                Set("eAppTheme", ref _eAppTheme, value);
                lImageOpacity = (bTranslucentDark == true && value == ElementTheme.Dark) ? 0.7 : 1.0;
            }
        }

        // 夜间模式降低图片透明度
        private bool _bTranslucentDark = true;
        public bool bTranslucentDark
        {
            get { return _bTranslucentDark; }
            set
            {
                Set("bTranslucentDark", ref _bTranslucentDark, value);
                lImageOpacity = (value == true && eAppTheme == ElementTheme.Dark) ? 0.7 : 1.0;
            }
        }

        // 图片的透明度(夜间模式开启设置后变为0.7)
        private double _lImageOpacity = 1;
        public double lImageOpacity
        {
            get { return _lImageOpacity; }
            private set { Set("lImageOpacity", ref _lImageOpacity, value); }
        }

        // 侧边栏底部按钮Hover文案
        private string _sHoverTipsText = "";
        public string sHoverTipsText
        {
            get { return _sHoverTipsText; }
            set { Set("sHoverTipsText", ref _sHoverTipsText, value); }
        }

        // 应用的背景图片路径
        private string _sAppWallpaper = "ms-appx:///Assets/Background/Background.jpg";
        public string sAppWallpaper
        {
            get { return _sAppWallpaper; }
            set { Set("sAppWallpaper", ref _sAppWallpaper, value); }
        }

        // 将PasswordPage的Frame导航到空白页
        public Action ActNavigateToBlank { get; set; } = null;

        // 用来弹窗的样式
        private Style _customDialogStyle = null;

        // 设置页面导入/导出提示文字
        private string _sSettingProcessingTip = string.Empty;
        public string sSettingProcessingTip
        {
            get { return _sSettingProcessingTip; }
            set { Set("sSettingProcessingTip", ref _sSettingProcessingTip, value); }
        }

        public PasswordViewModel()
        {
            try
            {
                LoadSettingContainer();
                InitManyPasswords();
            }
            catch { }
        }

        public void LoadSettingContainer()
        {
            try
            {
                // 读取设置是否启用了密码锁定
                try
                {
                    try
                    {
                        // 兼容旧版本
                        if (App.AppSettingContainer.Values["Password"]?.ToString() != null)
                        {
                            App.AppSettingContainer.Values["bAppLockEnabled"] = "True";
                            App.AppSettingContainer.Values["Password"] = null;
                        }
                    }
                    catch { }

                    if (App.AppSettingContainer.Values["bAppLockEnabled"]?.ToString() == "True")
                    {
                        bLockEnabled = true;
                        bAppLocked = true;
                    }
                    else
                    {
                        bLockEnabled = false;
                        bAppLocked = false;
                    }
                }
                catch { }

                // 读取设置的背景图片路径
                try
                {
                    if (App.AppSettingContainer?.Values["sAppWallpaper"] != null)
                    {
                        string settingWallpaperPath = App.AppSettingContainer?.Values["sAppWallpaper"].ToString();
                        if (File.Exists(settingWallpaperPath))
                        {
                            this.sAppWallpaper = settingWallpaperPath;
                        }
                    }
                }
                catch { }

                // 读取设置的应用程序主题
                try
                {
                    if (App.AppSettingContainer?.Values["Theme"] == null)
                    {
                        this.eAppTheme = ElementTheme.Light;
                    }
                    else if (App.AppSettingContainer?.Values["Theme"]?.ToString() == "Light")
                    {
                        this.eAppTheme = ElementTheme.Light;
                    }
                    else if (App.AppSettingContainer?.Values["Theme"]?.ToString() == "Dark")
                    {
                        this.eAppTheme = ElementTheme.Dark;
                    }
                    else
                    {
                        this.eAppTheme = ElementTheme.Light;
                    }
                }
                catch { }

                // 读取设置是否启用夜间模式降低透明度
                try
                {
                    if (App.AppSettingContainer?.Values["bTranslucentInDarkMode"] == null ||
                        App.AppSettingContainer?.Values["bTranslucentInDarkMode"].ToString() == "True")
                    {
                        bTranslucentDark = true;
                    }
                    else
                    {
                        bTranslucentDark = false;
                    }
                }
                catch { }
            }
            catch { }
        }

        public void InitManyPasswords()
        {
            try
            {
                // 读取用户文件
                LoadPasswordsFile();
            }
            catch { }
        }

        // 从文件读取所有密码，并转换为分组的集合
        public async void LoadPasswordsFile()
        {
            try
            {
                vAllPasswords = await PasswordHelper.LoadData();

                // 分组
                var orderedList = (from item in vAllPasswords
                                   group item by item.sFirstLetter into newItems
                                   select
                                   new Models.PasswordsGroup
                                   {
                                       Key = newItems.Key,
                                       vPasswords = new ObservableCollection<Models.PasswordItem>(newItems.ToList())
                                   }
                                  ).OrderBy(x => x.Key).ToList();
                vManyPasswords = new ObservableCollection<Models.PasswordsGroup>(orderedList);

                // 收藏
                vFavoritePasswords = new ObservableCollection<Models.PasswordItem>();
                for (int i = vAllPasswords.Count - 1; i >= 0; i--)
                {
                    if (vAllPasswords[i].bFavorite)
                    {
                        vFavoritePasswords.Add(vAllPasswords[i]);
                    }
                }
            }
            catch { }
        }

        public async Task<bool> SavePasswordsFile()
        {
            try
            {
                string msg = await PasswordHelper.SaveData(vAllPasswords);

                // 如果返回值不是空字符串，说明有异常，弹个窗提示一下
                if (!string.IsNullOrEmpty(msg))
                {
                    if (_customDialogStyle == null)
                    {
                        _customDialogStyle = (Style)Application.Current.Resources["CustomDialogStyle"];
                    }

                    ContentDialog dialog = new ContentDialog();

                    if (_customDialogStyle != null)
                    {
                        dialog.Style = _customDialogStyle;
                    }

                    dialog.Title = ":(  保存数据失败了";
                    dialog.IsPrimaryButtonEnabled = false;
                    dialog.IsSecondaryButtonEnabled = false;
                    dialog.CloseButtonText = "好吧";
                    dialog.DefaultButton = ContentDialogButton.Primary;
                    dialog.Content = msg;
                    dialog.RequestedTheme = this.eAppTheme;
                    _ = await dialog.ShowAsync();

                    return false;
                }
            }
            catch { }
            return true;
        }

        // 创建内置账号模板
        public void InitBuildInAccounts()
        {
            try
            {
                if (vBuildinItems == null || vBuildinItems.Count <= 0)
                {
                    vBuildinItems = new ObservableCollection<Models.BuildinItem>{
                new Models.BuildinItem("Apple",
                                       "苹果",
                                       "苹果公司（Apple Inc.）是美国的一家高科技公司，2007年由苹果电脑公司（Apple Computer, Inc.）更名而来，核心业务为电子科技产品，总部位于加利福尼亚州的库比蒂诺。（来自必应网典）",
                                       "https://www.apple.com/cn/"),
                new Models.BuildinItem("iQIYI",
                                       "爱奇艺",
                                       "爱奇艺原名奇艺，是一个百度旗下的独立视频网站，于2010年1月创立，由美国私募股权投资公司普罗维登斯资本投资、龚宇博士担任公司首席执行官。汤兴博士担任公司首席技术官，马东担任公司首席内容官。 爱奇艺的内容以综艺娱乐为主。（来自必应网典）",
                                       "http://www.iqiyi.com/"),
                new Models.BuildinItem("Baidu",
                                       "百度",
                                       "百度（Nasdaq：BIDU）是全球最大的中文搜索引擎，2000年1月由李彦宏、徐勇两人创立于北京中关村，百度致力于向人们提供“简单，可依赖”的信息获取方式。（来自必应网典）",
                                       "https://www.baidu.com/"),
                new Models.BuildinItem("bilibili",
                                       "哔哩哔哩",
                                       "bilibili是中国大陆一个ACG相关的弹幕视频分享网站，也被称为哔哩哔哩、B站，其前身为视频分享网站Mikufans。该网站相比其他国内的视频网站最大的特点是悬浮于视频上方的实时评论功能。（来自必应网典）",
                                       "http://www.bilibili.com/"),
                new Models.BuildinItem("douyin",
                                       "抖音",
                                       "抖音是一款可在智能手机上浏览的短视频社交应用程序，由中国大陆字节跳动公司所创办运营。用户可录制15秒至1分钟3分钟或者更长的视频，也能上传视频、照片等。自2016年9月于今日头条孵化上线，定位为适合中国大陆年轻人的音乐短视频社区，应用为垂直音乐的UGC短视频，2017年以来获得用户规模快速增长。另外抖音上也有电商平台。（来自维基百科）",
                                       "https://www.douyin.com"),
                new Models.BuildinItem("DouYu",
                                       "斗鱼TV",
                                       "斗鱼TV是一家弹幕式直播分享网站，是国内直播分享网站中的一员。斗鱼TV的前身为ACFUN生放送直播，于2014年1月1日起正式更名为斗鱼TV。斗鱼TV以游戏直播为主，涵盖了体育、综艺、娱乐等多种直播内容。（来自必应网典）",
                                       "https://www.douyu.com/"),
                new Models.BuildinItem("Facebook",
                                       "Facebook",
                                       "Facebook（原本称作thefacebook）是一家位于门洛帕克 (加利福尼亚州)的在线社交网络服务网站。其名称的灵感来自美国高中提供给学生包含照片和联系数据的通信录（或称花名册）昵称“face book”。（来自必应网典）",
                                       "https://www.facebook.com/"),
                new Models.BuildinItem("GitHub",
                                       "GitHub",
                                       "GitHub是一个通过Git进行版本控制的软件源代码托管服务，由GitHub公司（曾称Logical Awesome）的开发者Chris Wanstrath、PJ Hyett和Tom Preston-Werner使用Ruby on Rails编写而成。（来自必应网典）",
                                       "https://github.com/"),
                new Models.BuildinItem("Google",
                                       "谷歌",
                                       "Google公司，是一家美国的跨国科技企业，业务范围涵盖互联网搜索、云计算、广告技术等领域，开发并提供大量基于互联网的产品与服务，其主要利润来自于AdWords等广告服务。（来自必应网典）",
                                       "https://www.google.com.hk/"),
                new Models.BuildinItem("Huya",
                                       "虎牙直播",
                                       "虎牙直播是以游戏直播为主的弹幕式互动直播平台，累计注册用户2亿，提供热门游戏直播、电竞赛事直播与游戏赛事直播，手游直播等。包含英雄联盟lol，王者荣耀，绝地求生，和平精英等游戏直播，lol、dota2、dnf等热门游戏直播以及单机游戏、手游等游戏直播。（来自官网）",
                                       "https://www.huya.com/"),
                new Models.BuildinItem("Instagram",
                                       "Instagram",
                                       "Instnstagram是一个免费提供在线图片及短视频分享的社交应用，于2010年10月发布。Instagram的名称取自“即时”（英语：instant）与“电报”（英语：telegram）两个单词的结合。（来自必应网典）",
                                       "https://www.instagram.com/"),
                new Models.BuildinItem("ITHome",
                                       "IT之家",
                                       "软媒的前身，是青岛乐购信息技术有限公司和北京掌秀无线互动娱乐有限公司，新软媒公司将北京和青岛的公司业务融为一体并重新业务整合，为互联网、PC电脑用户、手机用户、平板电脑用户提供我们所擅长的产品和服务。（来自必应网典）",
                                       "https://www.ithome.com/"),
                new Models.BuildinItem("JD",
                                       "京东",
                                       "京东是中国最大的自营式电商企业，京东创始人刘强东担任京东集团CEO。2014年5月22日，京东在纳斯达克挂牌，是仅次于阿里巴巴、腾讯、百度的中国第四大互联网上市公司 。（来自必应网典）",
                                       "https://www.jd.com/"),
                new Models.BuildinItem("Line",
                                       "Line",
                                       "Line由韩国互联网集团NHN的日本子公司NHN Japan推出。虽然是一个起步较晚的通讯应用，2011年6月才正式推向市场，但全球注册用户超过4亿。 即时通讯应用Line于2016年7月15日在东京上市，同时也在美国时间2016年7月14日在纽约上市。（来自必应网典）",
                                       " http://line.naver.jp/"),
                new Models.BuildinItem("LinkedIn",
                                       "领英",
                                       "LinkedIn是全球最大的职业社交网站，是一家面向商业客户的社交网络（SNS），中文名为领英。LinkedIn成立于2002年12月并于2003年启动，于2011年5月20日在美上市，总部位于美国加利福尼亚州山景城。（来自必应网典）",
                                       "http://www.linkedin.com/"),
                new Models.BuildinItem("Microsoft",
                                       "微软",
                                       "微软 (Microsoft)，是一家总部位于美国的跨国电脑科技公司，是世界PC机软件开发的先导，由比尔•盖茨与保罗•艾伦创始于1975年，公司总部设立在华盛顿州的雷德蒙德市。（来自必应网典）",
                                       "https://www.microsoft.com/zh-cn"),
                new Models.BuildinItem("Naver",
                                       "Naver",
                                       "NAVER（네이버）是Naver公司旗下韩国著名入口／搜索引擎网站，其Logo为一顶草帽，于1999年6月正式投入使用。它使用独有的搜索引擎，并且在韩文搜寻服务中独占鳌头。（来自必应网典）",
                                       "https://www.naver.com/"),
                new Models.BuildinItem("NetEase",
                                       "网易",
                                       "网易（NASDAQ：NTES）是一家中国的互联网技术公司。目前提供网络游戏、门户网站、移动新闻客户端、移动财经客户端、电子邮件、电子商务、搜索引擎、博客、相册、社交平台、互联网教育等服务。（来自必应网典）",
                                       "http://www.163.com/"),
                new Models.BuildinItem("paypal",
                                       "PayPal",
                                       "PayPal是倍受全球亿万用户追捧的国际贸易支付工具，即时支付，即时到账，全中文操作界面，能通过中国的本地银行轻松提现，解决外贸收款难题，助您成功开展海外业务，决胜全球。（来自百度百科）",
                                       "https://www.paypal.com"),
                new Models.BuildinItem("Pinterest",
                                       "Pinterest",
                                       "Pinterest，是一个网络与手机的应用程序，可以让用户利用其平台作为个人创意及项目工作所需的视觉探索工具，同时也有人把它视为一个图片分享类的社交网站，用户可以按主题分类添加和管理自己的图片收藏，并与好友分享。（来自必应网典）",
                                       "https://www.pinterest.com/"),
                new Models.BuildinItem("pixiv",
                                       "pixiv",
                                       "Pixiv是一个主要由日本艺术家所组成的虚拟社群，主体为由pixiv股份有限公司所运营的为插画艺术特化的社交网络服务网站。Pixiv目的是提供一个能让艺术家发表他们的插图，并透过评级系统反应其他用户意见之处。（来自必应网典）",
                                       "https://www.pixiv.net/"),
                new Models.BuildinItem("PSN",
                                       "PlayStation",
                                       "PlayStation Network是2006年11月11日SONY PS3发售的同时开始正式提供PS3的网络服务。（来自百度百科）",
                                       "https://www.playstation.com/"),
                new Models.BuildinItem("Reddit",
                                       "Reddit",
                                       "Reddit是个社交新闻站点，口号：提前于新闻发声，来自互联网的声音。其拥有者是Condé Nast Digital公司（Advance Magazine Publishers Inc的子公司）。用户（也叫redditors）能够浏览并且可以提交因特网上内容的链接或发布自己的原创或有关用户提交文本的帖子。其他的用户可对发布的链接进行高分或低分的投票，得分突出的链接会被放到首页。另外，用户可对发布的链接进行评论以及回复其他评论者，这样就形成了一个在线社区。Reddit用户可以创造他们自己的论题部分，对发布链接和评论的人来说，既像Reddit用户提交的非正式的，也像社团的正式的。（来自百度百科）",
                                       "https://www.reddit.com/"),
                new Models.BuildinItem("SnapChat",
                                       "SnapChat",
                                       "Snapchat是一款由斯坦福大学学生开发的图片分享（英语：Photo sharing）软件应用。利用该应用程序，用户可以拍照、录制影片、写文字和图画，并发送到自己在该应用上的好友列表。（来自必应网典）",
                                       "https://www.snapchat.com"),
                new Models.BuildinItem("Spotify",
                                       "Spotify",
                                       "Spotify是一个起源于瑞典的音乐流服务，是全球最大的流音乐服务商，提供包括Sony Music、EMI、Warner Music Group和Universal四大唱片公司及众多独立厂牌所授权、由数字版权管理（DRM）保护的音乐，使用用户在2016年6月已经达到1亿以上。(来自必应网典)",
                                       "https://www.spotify.com/"),
                new Models.BuildinItem("Steam",
                                       "Steam",
                                       "Steam平台是Valve公司聘请的BitTorrent协议(BT下载)发明者Bram·Cohen亲自开发设计的全球最大的综合性数字游戏软件发行平台。玩家可以在该平台购买游戏、软件、下载、讨论、上传、分享。（来自必应网典）",
                                       "http://store.steampowered.com/"),
                new Models.BuildinItem("Taobao",
                                       "淘宝",
                                       "淘宝网是亚太地区较大的网络零售、商圈，由阿里巴巴集团在2003年5月创立。目前已经成为世界范围的电子商务交易平台之一。（来自必应网典）",
                                       "https://www.taobao.com/"),
                new Models.BuildinItem("Tencent",
                                       "腾讯QQ",
                                       "腾讯公司成立于1998年11月， 是目前中国最大的互联网综合服务提供商之一，也是中国服务用户最多的互联网企业之一。成立10多年以来，腾讯一直秉承一切以用户价值为依归的经营理念，始终处于稳健、高速发展的状态。（来自必应网典）",
                                       "http://www.qq.com/"),
                new Models.BuildinItem("Twitter",
                                       "Twitter",
                                       "Twitter是一个社交网络（Social Network Service）及微博客服务的网站，是全球互联网上访问量最大的十个网站之一。是微博客的典型应用。它允许用户将自己的最新动态和想法以短信形式发送给手机和个性化网站群，而不仅仅是发送给个人。（来自必应网典）",
                                       "https://twitter.com/"),
                new Models.BuildinItem("twitch",
                                       "twitch",
                                       "Twitch是游戏软件影音流平台，2011年6月从Justin.tv分区成立。提供平台供游戏玩家进行游戏过程的实况，或供游戏赛事的转播。也提供聊天室，让观众间进行简单的交互。（来自必应网典）",
                                       "https://www.twitch.tv/"),
                new Models.BuildinItem("WeChat",
                                       "微信",
                                       "微信（官方英文：WeChat）是腾讯公司于2011年1月21日推出的一款即时通信软件， 市场研究公司On Device调查显示，微信在中国大陆的市场渗透率达93%。截至2015年6月，微信于全球拥有超过约6亿活跃用户。（来自必应网典）",
                                       "http://weixin.qq.com/"),
                new Models.BuildinItem("Weibo",
                                       "微博",
                                       "微博 是一个由新浪网推出，提供微型博客服务类的社交网站。3月27日新浪微博正式更名为“微博”拿掉“新浪”两个字之后的“微博”在架构上成为独立公司，与新浪网一起构成新浪公司的重要两级，于2014年4月17日在美国纳斯达克正式挂牌上市。（来自必应网典）",
                                       "http://www.weibo.com/"),
                new Models.BuildinItem("Whatsapp",
                                       "WhatsApp",
                                       "WhatsApp Messenger，简称WhatsApp ，是一款用于智能手机的跨平台加密即时通信客户端专有软件。2014年2月被Facebook以约193亿美元收购。到2016年2月为止，WhatsApp的用户群超过10亿，使其成为当时最流行的消息应用程序。（来自必应网典）",
                                       "https://www.whatsapp.com/"),
                new Models.BuildinItem("Yahoo",
                                       "雅虎",
                                       "雅虎公司（英语：Yahoo! Inc.，NASDAQ：YHOO）是美国一间跨国发展的信息技术公司，主要业务为门户网站经营及广告服务，并提供一系列产品包括：搜索引擎、电子邮箱、社交以及新闻聚合等服务。（来自必应网典）",
                                       "https://www.yahoo.com/"),
                new Models.BuildinItem("Alipay",
                                       "支付宝",
                                       "支付宝，是蚂蚁金服旗下的第三方支付平台，2004年12月由中国阿里巴巴集团于杭州创办，原本隶属于阿里巴巴集团，现在为阿里巴巴集团的关联公司，隶属于浙江蚂蚁金服。（来自必应网典）",
                                       "https://www.alipay.com/"),
                new Models.BuildinItem("Zhihu",
                                       "知乎",
                                       "知乎，中文互联网最大的知识社交平台，吉祥物是刘看山。一个人大脑中从未分享过的知识、经验、见解和判断力，总是另一群人非常想知道的东西。知乎的使命是把人们大脑里的知识经验和见解搬上互联网，让彼此更好地连接。（来自必应网典）",
                                       "https://www.zhihu.com/"),
                new Models.BuildinItem("BattleNet",
                                       "战网",
                                       "战网（Battle.net）是暴雪娱乐为旗下游戏提供的多人在线游戏服务，自1996年11月30日推出暗黑破坏神后运营。战网启动器于2017年3月24日更名为暴雪游戏平台（Blizzard App）。（来自必应网典）",
                                       "http://www.battlenet.com.cn/zh/")
                };
                }
            }
            catch { }
        }

        // 更换应用的背景图片
        public async void ChangeWallPaper()
        {
            try
            {
                //创建和自定义 FileOpenPicker
                var picker = new Windows.Storage.Pickers.FileOpenPicker
                {
                    ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                    SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
                };
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".png");

                //选取单个文件
                StorageFile file = await picker.PickSingleFileAsync();

                if (file != null)
                {
                    StorageFolder applicationFolder = ApplicationData.Current.LocalFolder;
                    StorageFolder folder = await applicationFolder.CreateFolderAsync("Pic", CreationCollisionOption.OpenIfExists);
                    try
                    {
                        StorageFile saveFile = await file.CopyAsync(folder, "app_wallpaper_" + DateTime.Now.Ticks + file.FileType, NameCollisionOption.ReplaceExisting);
                        try
                        {
                            if (this.sAppWallpaper.Contains("app_wallpaper_") &&
                                this.sAppWallpaper != saveFile.Path &&
                                File.Exists(this.sAppWallpaper))
                            {
                                File.Delete(this.sAppWallpaper);
                            }
                        }
                        catch { }
                        App.AppSettingContainer.Values["sAppWallpaper"] = saveFile.Path;
                        this.sAppWallpaper = saveFile.Path;
                    }
                    catch { }
                }
            }
            catch { }
        }

        // 添加账号密码
        public async void AddPassword(Models.PasswordItem add)
        {
            try
            {
                //add.sName = FilterEmoji(add.sName);
                add.sFirstLetter = GetFirstLetter(add.sName);
                vAllPasswords.Add(add);

                bool result = await SavePasswordsFile();
                if (!result)
                {
                    if (vAllPasswords.Contains(add))
                    {
                        vAllPasswords.Remove(add);
                    }
                    return;
                }

                Models.PasswordsGroup addingGroup = null;
                foreach (var group in vManyPasswords)
                {
                    if (group.Key == add.sFirstLetter)
                    {
                        addingGroup = group;
                        break;
                    }
                }
                if (addingGroup == null)
                {
                    addingGroup = new Models.PasswordsGroup();
                    addingGroup.Key = add.sFirstLetter;
                    addingGroup.vPasswords = new ObservableCollection<Models.PasswordItem>();
                    vManyPasswords.Add(addingGroup);
                }
                addingGroup.vPasswords.Add(add);

                var newOrderedList = vManyPasswords.OrderBy(x => x.Key).ToList();
                vManyPasswords.Clear();
                vManyPasswords = null;
                vManyPasswords = new ObservableCollection<Models.PasswordsGroup>(newOrderedList);

                ActNavigateToBlank?.Invoke();

                if (add.bFavorite)
                {
                    AddFavorite(add, false);
                }
            }
            catch { }
        }

        // 删除账号密码
        public async void RemovePassword(Models.PasswordItem remove)
        {
            try
            {
                if (vAllPasswords != null && vAllPasswords.Contains(remove))
                {
                    vAllPasswords.Remove(remove);

                    bool result = await SavePasswordsFile();
                    if (!result)
                    {
                        vAllPasswords.Add(remove);
                        return;
                    }

                    try
                    {
                        if (File.Exists(remove.sPicture) &&
                            !remove.sPicture.Contains("Assets") &&
                            !remove.sPicture.Contains("ms-appx"))
                        {
                            File.Delete(remove.sPicture);
                        }
                    }
                    catch { }
                }
                Models.PasswordsGroup removingGroup = null;
                foreach (var group in vManyPasswords)
                {
                    if (group.Key == remove.sFirstLetter)
                    {
                        removingGroup = group;
                        break;
                    }
                }
                if (removingGroup != null && removingGroup.vPasswords.Contains(remove))
                {
                    removingGroup.vPasswords.Remove(remove);
                    if (removingGroup.vPasswords.Count <= 0 && vManyPasswords.Contains(removingGroup))
                    {
                        vManyPasswords.Remove(removingGroup);
                    }
                }

                if (remove.bFavorite)
                {
                    RemoveFavorite(remove, false);
                }
            }
            catch { }
        }

        // 添加收藏
        public async void AddFavorite(Models.PasswordItem add, bool needToSave)
        {
            try
            {
                if (this.vFavoritePasswords == null)
                {
                    this.vFavoritePasswords = new ObservableCollection<Models.PasswordItem>();
                }
                add.bFavorite = true;
                this.vFavoritePasswords.Insert(0, add);

                if (needToSave)
                {
                    _ = await SavePasswordsFile();
                }
            }
            catch { }
        }

        // 取消收藏
        public async void RemoveFavorite(Models.PasswordItem remove, bool needToSave)
        {
            try
            {
                if (this.vFavoritePasswords != null)
                {
                    remove.bFavorite = false;
                    if (this.vFavoritePasswords.Contains(remove))
                    {
                        this.vFavoritePasswords.Remove(remove);
                    }

                    if (needToSave)
                    {
                        _ = await SavePasswordsFile();
                    }
                }
            }
            catch { }
        }

        // 搜索
        public void SearchSuggestPasswords(string search)
        {
            try
            {
                vSearchSuggestions?.Clear();
                vSearchSuggestions = null;
                if (string.IsNullOrEmpty(search))
                {
                    return;
                }
                vSearchSuggestions = vAllPasswords.Where(p => (p.sName.StartsWith(search, StringComparison.CurrentCultureIgnoreCase) || p.sAccount.StartsWith(search, StringComparison.CurrentCultureIgnoreCase))).ToList();
            }
            catch { }
        }

        // 导出文件
        public async Task ExportPasswordsFile()
        {
            try
            {
                sSettingProcessingTip = "正在导出...";

                try
                {
                    string result = await StorageFileHelper.CreatePasswordsZipFile();
                    if (string.IsNullOrEmpty(result))
                    {
                        IStorageFolder destinationfolder = ApplicationData.Current.LocalCacheFolder;
                        await Windows.System.Launcher.LaunchFolderAsync(destinationfolder);

                        sSettingProcessingTip = "导出文件完成，请务必妥善保管~";
                    }
                    else
                    {
                        // 失败
                        sSettingProcessingTip = result;
                    }
                }
                catch (Exception e) { sSettingProcessingTip = "保存文件失败：" + e.Message; }
            }
            catch (Exception e)
            {
                sSettingProcessingTip = "保存文件失败：" + e.Message;
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(3));
                sSettingProcessingTip = string.Empty;
            }
        }

        // 导入文件
        public async Task ImportPasswordsFile()
        {
            try
            {
                sSettingProcessingTip = "正在导入...";

                try
                {
                    string result = await StorageFileHelper.ReadPasswordsZipFile();
                    if (string.IsNullOrEmpty(result))
                    {
                        // 压缩完成，复制到指定位置，然后删除原文件
                        //file.async
                        sSettingProcessingTip = "密码已经导入啦~";
                    }
                    else
                    {
                        // 失败，在设置的蒙层上显示错误信息
                        sSettingProcessingTip = result;
                    }
                }
                catch (Exception e)
                {
                    sSettingProcessingTip = "导入文件失败：" + e.Message;
                }
            }
            catch (Exception e)
            {
                sSettingProcessingTip = "导入文件失败：" + e.Message;
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(3));
                sSettingProcessingTip = string.Empty;
            }
        }

        // 取首字母
        private char GetFirstLetter(string name)
        {
            try
            {
                // 数字
                if (string.IsNullOrEmpty(name) || System.Text.RegularExpressions.Regex.IsMatch(name.Trim(), "^[0-9]"))
                {
                    return '#';
                }

                // 英文
                if (System.Text.RegularExpressions.Regex.IsMatch(name.Trim(), "^[a-zA-Z]"))
                {
                    return name.ToUpper()[0];
                }

                // 特殊
                switch (name[0])
                {
                    case 'ā':
                    case 'á':
                    case 'ǎ':
                    case 'à':
                        return 'A';
                    case 'ō':
                    case 'ó':
                    case 'ǒ':
                    case 'ò':
                        return 'O';
                    case 'ē':
                    case 'é':
                    case 'ě':
                    case 'è':
                    case 'ê':
                        return 'E';
                    case 'ī':
                    case 'í':
                    case 'ǐ':
                    case 'ì':
                        return 'I';
                    case 'ū':
                    case 'ú':
                    case 'ǔ':
                    case 'ù':
                        return 'U';
                    case 'ǖ':
                    case 'ǘ':
                    case 'ǚ':
                    case 'ǜ':
                    case 'ü':
                        return 'V';
                }

                try
                {
                    char firstLetter = Helpers.PinyinHelper.GetFirstSpell(name);
                    return firstLetter;
                }
                catch { }
            }
            catch { }
            return name.Length > 0 ? name[0] : '#';
        }

        // 过滤掉Emoji，避免出问题
        public string FilterEmoji(string str)
        {

            string origin = str;
            try
            {
                //关键代码
                foreach (var a in str)
                {
                    byte[] bts = Encoding.UTF32.GetBytes(a.ToString());
                    if (bts[0].ToString() == "253" && bts[1].ToString() == "255")
                    {
                        str = str.Replace(a.ToString(), "");
                    }
                }
            }
            catch (Exception)
            {
                str = "非法字符";//origin;
            }
            return str;
        }


        #region 应用的各种功能

        // 锁定应用程序
        public void LockApp()
        {
            try
            {
                this.bAppLocked = true;
            }
            catch { }
        }

        // 解锁应用程序
        public async void UnlockApp()
        {
            try
            {
                if (this.bLockEnabled == false)
                {
                    this.bAppLocked = false;
                    return;
                }

                switch (await Windows.Security.Credentials.UI.UserConsentVerifier.RequestVerificationAsync("验证您的身份"))
                {
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.Verified:
                        this.bAppLocked = false;
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.DeviceNotPresent:
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.NotConfiguredForUser:
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.DisabledByPolicy:
                        await new Windows.UI.Popups.MessageDialog("当前识别设备未配置或被系统策略禁用，请尝试使用密码解锁").ShowAsync();
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.DeviceBusy:
                        await new Windows.UI.Popups.MessageDialog("当前识别设备不可用，请尝试使用密码解锁").ShowAsync();
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.RetriesExhausted:
                        await new Windows.UI.Popups.MessageDialog("验证失败，请尝试使用密码解锁").ShowAsync();
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.Canceled:
                        break;
                    default:
                        break;
                }
            }
            catch { }
        }

        // 开关WindowsHello
        public async void SetWindowsHelloEnable(bool on)
        {
            try
            {
                if (this.bLockEnabled != on)
                {
                    this.bLockEnabled = !this.bLockEnabled;
                    this.bLockEnabled = !this.bLockEnabled;
                    switch (await Windows.Security.Credentials.UI.UserConsentVerifier.RequestVerificationAsync("验证您的身份"))
                    {
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.Verified:
                            this.bLockEnabled = on;
                            App.AppSettingContainer.Values["bAppLockEnabled"] = this.bLockEnabled ? "True" : "False";
                            break;
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.DeviceNotPresent:
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.NotConfiguredForUser:
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.DisabledByPolicy:
                            await new Windows.UI.Popups.MessageDialog("当前识别设备未配置或被系统策略禁用").ShowAsync();
                            break;
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.DeviceBusy:
                            await new Windows.UI.Popups.MessageDialog("当前识别设备不可用").ShowAsync();
                            break;
                        case Windows.Security.Credentials.UI.UserConsentVerificationResult.RetriesExhausted:
                            await new Windows.UI.Popups.MessageDialog("验证失败").ShowAsync();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch { }
        }

        #endregion
    }
}
