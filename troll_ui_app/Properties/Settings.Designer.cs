﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.18444
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace troll_ui_app.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://domain.shanyaows.com/domain_names/update_logs.json")]
        public string domainUpdateLogsUrl {
            get {
                return ((string)(this["domainUpdateLogsUrl"]));
            }
            set {
                this["domainUpdateLogsUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("data/porn.model")]
        public string modelFile {
            get {
                return ((string)(this["modelFile"]));
            }
            set {
                this["modelFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("8090")]
        public int bindPort {
            get {
                return ((int)(this["bindPort"]));
            }
            set {
                this["bindPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        public int maxNumDomainList {
            get {
                return ((int)(this["maxNumDomainList"]));
            }
            set {
                this["maxNumDomainList"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("60")]
        public int maxHistoryDays {
            get {
                return ((int)(this["maxHistoryDays"]));
            }
            set {
                this["maxHistoryDays"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://domain.shanyaows.com/domain_names/submit_tmp_domain_name.json")]
        public string submitTmpDomainUrl {
            get {
                return ((string)(this["submitTmpDomainUrl"]));
            }
            set {
                this["submitTmpDomainUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("30")]
        public int thdPornPicsOfDomain {
            get {
                return ((int)(this["thdPornPicsOfDomain"]));
            }
            set {
                this["thdPornPicsOfDomain"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("porn.db")]
        public string pornDbFileName {
            get {
                return ((string)(this["pornDbFileName"]));
            }
            set {
                this["pornDbFileName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("images")]
        public string imagesDir {
            get {
                return ((string)(this["imagesDir"]));
            }
            set {
                this["imagesDir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool firstTime {
            get {
                return ((bool)(this["firstTime"]));
            }
            set {
                this["firstTime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("224")]
        public int minWidth {
            get {
                return ((int)(this["minWidth"]));
            }
            set {
                this["minWidth"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("224")]
        public int minHeight {
            get {
                return ((int)(this["minHeight"]));
            }
            set {
                this["minHeight"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool upgraded {
            get {
                return ((bool)(this["upgraded"]));
            }
            set {
                this["upgraded"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string email {
            get {
                return ((string)(this["email"]));
            }
            set {
                this["email"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("updates")]
        public string updateDir {
            get {
                return ((string)(this["updateDir"]));
            }
            set {
                this["updateDir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://download.shanyaows.com/trollwiz-vers.xml")]
        public string updateXmlUrl {
            get {
                return ((string)(this["updateXmlUrl"]));
            }
            set {
                this["updateXmlUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string openid {
            get {
                return ((string)(this["openid"]));
            }
            set {
                this["openid"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://domain.shanyaows.com/wechat_apis/create_scene.json")]
        public string createSceneUrl {
            get {
                return ((string)(this["createSceneUrl"]));
            }
            set {
                this["createSceneUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://domain.shanyaows.com/wechat_apis/scene_info.json")]
        public string getSceneUrl {
            get {
                return ((string)(this["getSceneUrl"]));
            }
            set {
                this["getSceneUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://domain.shanyaows.com/wechat_apis/access_token.json")]
        public string getAccessTokenUrl {
            get {
                return ((string)(this["getAccessTokenUrl"]));
            }
            set {
                this["getAccessTokenUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token={0}")]
        public string wechatCreateQrcodeUrl {
            get {
                return ((string)(this["wechatCreateQrcodeUrl"]));
            }
            set {
                this["wechatCreateQrcodeUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket={0}")]
        public string wechatGetQrcodeUrl {
            get {
                return ((string)(this["wechatGetQrcodeUrl"]));
            }
            set {
                this["wechatGetQrcodeUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}")]
        public string wechatSendTemplateMessage {
            get {
                return ((string)(this["wechatSendTemplateMessage"]));
            }
            set {
                this["wechatSendTemplateMessage"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MNdOlj_naI2e_BZuCgWqY03oDoC8OI3pltEZ5oe_cr0")]
        public string wechatTemplateUninstall {
            get {
                return ((string)(this["wechatTemplateUninstall"]));
            }
            set {
                this["wechatTemplateUninstall"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("EZPCL_OUoG7FbBGG3CBmJG75UJL6Xx9r9vBgPFE23pw")]
        public string wechatTemplatePornDetected {
            get {
                return ((string)(this["wechatTemplatePornDetected"]));
            }
            set {
                this["wechatTemplatePornDetected"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("HgM5dmYw2gAvp6lYwO_CLXK2kiu2z68oG0tOxSvOA4o")]
        public string wechatTemplatePornScanned {
            get {
                return ((string)(this["wechatTemplatePornScanned"]));
            }
            set {
                this["wechatTemplatePornScanned"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("8")]
        public int thdPornPicsOfBlackDomain {
            get {
                return ((int)(this["thdPornPicsOfBlackDomain"]));
            }
            set {
                this["thdPornPicsOfBlackDomain"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string guid {
            get {
                return ((string)(this["guid"]));
            }
            set {
                this["guid"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://domain.shanyaows.com/trollwiz_users/register.json")]
        public string registerTrollwizUserUrl {
            get {
                return ((string)(this["registerTrollwizUserUrl"]));
            }
            set {
                this["registerTrollwizUserUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.0.0.0")]
        public string skipVersion {
            get {
                return ((string)(this["skipVersion"]));
            }
            set {
                this["skipVersion"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://www.shanyaows.com/faq.html")]
        public string helpUrl {
            get {
                return ((string)(this["helpUrl"]));
            }
            set {
                this["helpUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool isProtected {
            get {
                return ((bool)(this["isProtected"]));
            }
            set {
                this["isProtected"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool IsNetworkImageTurnOn {
            get {
                return ((bool)(this["IsNetworkImageTurnOn"]));
            }
            set {
                this["IsNetworkImageTurnOn"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool IsPornWebsiteProtectionTurnOn {
            get {
                return ((bool)(this["IsPornWebsiteProtectionTurnOn"]));
            }
            set {
                this["IsPornWebsiteProtectionTurnOn"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool IsLocalActiveImageTurnOn {
            get {
                return ((bool)(this["IsLocalActiveImageTurnOn"]));
            }
            set {
                this["IsLocalActiveImageTurnOn"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool IsLocalActiveVideoTurnOn {
            get {
                return ((bool)(this["IsLocalActiveVideoTurnOn"]));
            }
            set {
                this["IsLocalActiveVideoTurnOn"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://domain.shanyaows.com/wechat_apis/get_user_info.json")]
        public string getUserInfoUrl {
            get {
                return ((string)(this["getUserInfoUrl"]));
            }
            set {
                this["getUserInfoUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://domain.shanyaows.com/wechat_apis/get_auth_info.json")]
        public string getAuthInfoUrl {
            get {
                return ((string)(this["getAuthInfoUrl"]));
            }
            set {
                this["getAuthInfoUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string userNickname {
            get {
                return ((string)(this["userNickname"]));
            }
            set {
                this["userNickname"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string userHeadimgurl {
            get {
                return ((string)(this["userHeadimgurl"]));
            }
            set {
                this["userHeadimgurl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://domain.shanyaows.com/wechat_apis/error_report.json")]
        public string errorReportUrl {
            get {
                return ((string)(this["errorReportUrl"]));
            }
            set {
                this["errorReportUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool IsStrongNetworkImageFilter {
            get {
                return ((bool)(this["IsStrongNetworkImageFilter"]));
            }
            set {
                this["IsStrongNetworkImageFilter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("oE6T4TWYxeI85_rlUuF4-sZww2vNiPLrRaL7k_b5lWE")]
        public string wechatTemplateAuthRequest {
            get {
                return ((string)(this["wechatTemplateAuthRequest"]));
            }
            set {
                this["wechatTemplateAuthRequest"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxe9c247fd9d468fa6&redi" +
            "rect_uri=http%3A%2F%2Fdomain.shanyaows.com%2Fwechat_apis%2Fauth&response_type=co" +
            "de&scope=snsapi_userinfo&state=weixin#wechat_redirect")]
        public string wechatAuthUrl {
            get {
                return ((string)(this["wechatAuthUrl"]));
            }
            set {
                this["wechatAuthUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://www.shanyaows.com/faq.html#whywechatbinding")]
        public string whyWechatBindingUrl {
            get {
                return ((string)(this["whyWechatBindingUrl"]));
            }
            set {
                this["whyWechatBindingUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://www.shanyaows.com/forbidden.html")]
        public string forbiddenRedirectUrl {
            get {
                return ((string)(this["forbiddenRedirectUrl"]));
            }
            set {
                this["forbiddenRedirectUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.DateTime lastFastLocalScanDateTime {
            get {
                return ((global::System.DateTime)(this["lastFastLocalScanDateTime"]));
            }
            set {
                this["lastFastLocalScanDateTime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool isFastLocalScanIncremental {
            get {
                return ((bool)(this["isFastLocalScanIncremental"]));
            }
            set {
                this["isFastLocalScanIncremental"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool isAllLocalScanIncremental {
            get {
                return ((bool)(this["isAllLocalScanIncremental"]));
            }
            set {
                this["isAllLocalScanIncremental"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.DateTime lastAllLocalScanDateTime {
            get {
                return ((global::System.DateTime)(this["lastAllLocalScanDateTime"]));
            }
            set {
                this["lastAllLocalScanDateTime"] = value;
            }
        }
    }
}
