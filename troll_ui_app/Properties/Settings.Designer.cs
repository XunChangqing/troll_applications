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
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int lastid {
            get {
                return ((int)(this["lastid"]));
            }
            set {
                this["lastid"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://121.40.144.14:8080/domain_admin/domain_update_logs.json")]
        public string update_domain_url {
            get {
                return ((string)(this["update_domain_url"]));
            }
            set {
                this["update_domain_url"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("data/deploy.prototxt")]
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
        [global::System.Configuration.DefaultSettingValueAttribute("data/model_256__iter_50000.caffemodel")]
        public string trainedFile {
            get {
                return ((string)(this["trainedFile"]));
            }
            set {
                this["trainedFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("data/imagenet_mean.binaryproto")]
        public string meanFile {
            get {
                return ((string)(this["meanFile"]));
            }
            set {
                this["meanFile"] = value;
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
        [global::System.Configuration.DefaultSettingValueAttribute("15")]
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
        [global::System.Configuration.DefaultSettingValueAttribute("http://121.40.144.14:8080/domain_admin/tmp_domain_names.json")]
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
        [global::System.Configuration.DefaultSettingValueAttribute("50")]
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
    }
}
