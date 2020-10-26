using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WindowsShade.Models
{

//var hardwareInfo = HardwareClass.GetHardwareTable();
//foreach (var h in hardwareInfo)
//{
//    if (h.DeviceName == "通用即插即用监视器")
//    {
//        h.SetEnabled(false);
//    }
//}
    /// <summary>
    /// 禁用设备
    /// zgke@sina.com
    /// qq:116149
    /// </summary>
    public class HardwareClass
    {
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, uint Enumerator, IntPtr HwndParent, DIGCF Flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiGetDeviceRegistryProperty(IntPtr DeviceInfoSet, SP_DEVINFO_DATA DeviceInfoData, SPDRP Property, uint PropertyRegDataType, StringBuilder PropertyBuffer, uint PropertyBufferSize, IntPtr RequiredSize);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetupDiSetClassInstallParams(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, IntPtr ClassInstallParams, int ClassInstallParamsSize);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern Boolean SetupDiCallClassInstaller(DIF InstallFunction, IntPtr DeviceInfoSet, IntPtr DeviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern int SetupDiDestroyDeviceInfoList(IntPtr lpInfoSet);

        public enum DIGCF
        {
            DIGCF_DEFAULT = 0x1,
            DIGCF_PRESENT = 0x2,
            DIGCF_ALLCLASSES = 0x4,
            DIGCF_PROFILE = 0x8,
            DIGCF_DEVICEINTERFACE = 0x10
        }

        public enum SPDRP
        {
            SPDRP_DEVICEDESC = 0,
            SPDRP_HARDWAREID = 0x1,
            SPDRP_COMPATIBLEIDS = 0x2,
            SPDRP_UNUSED0 = 0x3,
            SPDRP_SERVICE = 0x4,
            SPDRP_UNUSED1 = 0x5,
            SPDRP_UNUSED2 = 0x6,
            SPDRP_CLASS = 0x7,
            SPDRP_CLASSGUID = 0x8,
            SPDRP_DRIVER = 0x9,
            SPDRP_CONFIGFLAGS = 0xA,
            SPDRP_MFG = 0xB,
            SPDRP_FRIENDLYNAME = 0xC,
            SPDRP_LOCATION_INFORMATION = 0xD,
            SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = 0xE,
            SPDRP_CAPABILITIES = 0xF,
            SPDRP_UI_NUMBER = 0x10,
            SPDRP_UPPERFILTERS = 0x11,
            SPDRP_LOWERFILTERS = 0x12,
            SPDRP_BUSTYPEGUID = 0x13,
            SPDRP_LEGACYBUSTYPE = 0x14,
            SPDRP_BUSNUMBER = 0x15,
            SPDRP_ENUMERATOR_NAME = 0x16,
            SPDRP_SECURITY = 0x17,
            SPDRP_SECURITY_SDS = 0x18,
            SPDRP_DEVTYPE = 0x19,
            SPDRP_EXCLUSIVE = 0x1A,
            SPDRP_CHARACTERISTICS = 0x1B,
            SPDRP_ADDRESS = 0x1C,
            SPDRP_UI_NUMBER_DESC_FORMAT = 0x1E,
            SPDRP_MAXIMUM_PROPERTY = 0x1F
        }

        public enum DICS
        {
            DICS_ENABLE = 0x1,
            DICS_DISABLE = 0x2,
            DICS_PROPCHANGE = 0x3,
            DICS_START = 0x4,
            DICS_STOP = 0x5,
            DICS_FLAG_GLOBAL = DICS_ENABLE,
            DICS_FLAG_CONFIGSPECIFIC = DICS_DISABLE,
            DICS_FLAG_CONFIGGENERAL = DICS_START
        }

        public enum DIF
        {
            DIF_SELECTDEVICE = 0x1,
            DIF_INSTALLDEVICE = 0x2,
            DIF_ASSIGNRESOURCES = 0x3,
            DIF_PROPERTIES = 0x4,
            DIF_REMOVE = 0x5,
            DIF_FIRSTTIMESETUP = 0x6,
            DIF_FOUNDDEVICE = 0x7,
            DIF_SELECTCLASSDRIVERS = 0x8,
            DIF_VALIDATECLASSDRIVERS = 0x9,
            DIF_INSTALLCLASSDRIVERS = 0xA,
            DIF_CALCDISKSPACE = 0xB,
            DIF_DESTROYPRIVATEDATA = 0xC,
            DIF_VALIDATEDRIVER = 0xD,
            DIF_MOVEDEVICE = 0xE,
            DIF_DETECT = 0xF,
            DIF_INSTALLWIZARD = 0x10,
            DIF_DESTROYWIZARDDATA = 0x11,
            DIF_PROPERTYCHANGE = 0x12,
            DIF_ENABLECLASS = 0x13,
            DIF_DETECTVERIFY = 0x14,
            DIF_INSTALLDEVICEFILES = 0x15,
            DIF_UNREMOVE = 0x16,
            DIF_SELECTBESTCOMPATDRV = 0x17,
            DIF_ALLOW_INSTALL = 0x18,
            DIF_REGISTERDEVICE = 0x19,
            DIF_NEWDEVICEWIZARD_PRESELECT = 0x1A,
            DIF_NEWDEVICEWIZARD_SELECT = 0x1B,
            DIF_NEWDEVICEWIZARD_PREANALYZE = 0x1C,
            DIF_NEWDEVICEWIZARD_POSTANALYZE = 0x1D,
            DIF_NEWDEVICEWIZARD_FINISHINSTALL = 0x1E,
            DIF_UNUSED1 = 0x1F,
            DIF_INSTALLINTERFACES = 0x20,
            DIF_DETECTCANCEL = 0x21,
            DIF_REGISTER_COINSTALLERS = 0x22,
            DIF_ADDPROPERTYPAGE_ADVANCED = 0x23,
            DIF_ADDPROPERTYPAGE_BASIC = 0x24,
            DIF_RESERVED1 = 0x25,
            DIF_TROUBLESHOOTER = 0x26,
            DIF_POWERMESSAGEWAKE = 0x27
        }



        [StructLayout(LayoutKind.Sequential)]
        public class SP_DEVINFO_DATA
        {
            public int cbSize;
            public Guid classGuid;
            public int devInst;
            public ulong reserved;
        };

        [StructLayout(LayoutKind.Sequential)]
        public class SP_PROPCHANGE_PARAMS
        {
            public SP_CLASSINSTALL_HEADER ClassInstallHeader = new SP_CLASSINSTALL_HEADER();
            public int StateChange;
            public int Scope;
            public int HwProfile;
        };

        [StructLayout(LayoutKind.Sequential)]
        public class SP_CLASSINSTALL_HEADER
        {
            public int cbSize;
            public int InstallFunction;
        };


        /// <summary>
        /// 获取设备的名称和ID
        /// </summary>
        /// <returns></returns>
        public static IList<HardwareInfo> GetHardwareTable()
        {
            IList<HardwareInfo> _ReturnList = new List<HardwareInfo>();
            Guid _NewGuid = Guid.Empty;
            IntPtr _MainIntPtr = SetupDiGetClassDevs(ref _NewGuid, 0, IntPtr.Zero, DIGCF.DIGCF_ALLCLASSES | DIGCF.DIGCF_PRESENT);
            if (_MainIntPtr.ToInt32() == -1) return _ReturnList;
            SP_DEVINFO_DATA _DevinfoData = new SP_DEVINFO_DATA();
            _DevinfoData.classGuid = System.Guid.Empty;
            _DevinfoData.cbSize = 28;
            _DevinfoData.devInst = 0;
            _DevinfoData.reserved = 0;
            StringBuilder _DeviceName = new StringBuilder("");
            _DeviceName.Capacity = 1000;
            uint i = 0;
            while (SetupDiEnumDeviceInfo(_MainIntPtr, i, _DevinfoData))
            {
                SetupDiGetDeviceRegistryProperty(_MainIntPtr, _DevinfoData, SPDRP.SPDRP_DEVICEDESC, 0, _DeviceName, (uint)_DeviceName.Capacity, IntPtr.Zero);

                _ReturnList.Add(new HardwareInfo(_DeviceName.ToString(), _DevinfoData.classGuid, _DevinfoData.cbSize, _DevinfoData.devInst, _DevinfoData.reserved));

                i++;
            }
            SetupDiDestroyDeviceInfoList(_MainIntPtr);
            return _ReturnList;
        }

        public class HardwareInfo
        {
            private string m_DeviceName = string.Empty;
            private Guid m_ClassGuid = Guid.Empty;
            private int m_Size = 0;
            private int m_DevInst = 0;
            private ulong m_Reserved = 0;

            /// <summary>
            /// 设备名称
            /// </summary>
            public string DeviceName { get { return m_DeviceName; } }

            /// <summary>
            /// 设备GUID
            /// </summary>
            public Guid ClassGuid { get { return m_ClassGuid; } }

            public HardwareInfo(string p_DeviceName, Guid p_ClassGuid, int p_Size, int p_DevInst, ulong p_Reserved)
            {
                m_ClassGuid = p_ClassGuid;
                m_DeviceName = p_DeviceName;
                m_DevInst = p_DevInst;
                m_Reserved = p_Reserved;
                m_Size = p_Size;
            }

            /// <summary>
            /// 设置状态
            /// </summary>
            public void SetEnabled(bool p_Enabled)
            {
                Guid _NewGuid = Guid.Empty;
                IntPtr _MainIntPtr = SetupDiGetClassDevs(ref _NewGuid, 0, IntPtr.Zero, DIGCF.DIGCF_ALLCLASSES | DIGCF.DIGCF_PRESENT);
                if (_MainIntPtr.ToInt32() == -1) return;
                SP_DEVINFO_DATA _DevinfoData = new SP_DEVINFO_DATA();
                _DevinfoData.classGuid = System.Guid.Empty;
                _DevinfoData.cbSize = 28;
                _DevinfoData.devInst = 0;
                _DevinfoData.reserved = 0;
                StringBuilder _DeviceName = new StringBuilder("");
                _DeviceName.Capacity = 1000;

                uint i = 0;
                while (SetupDiEnumDeviceInfo(_MainIntPtr, i, _DevinfoData))
                {
                    SetupDiGetDeviceRegistryProperty(_MainIntPtr, _DevinfoData, SPDRP.SPDRP_DEVICEDESC, 0, _DeviceName, (uint)_DeviceName.Capacity, IntPtr.Zero);

                    if (_DevinfoData.classGuid.ToString() == this.ClassGuid.ToString())
                    {
                        SetHardwareEnabled(_MainIntPtr, p_Enabled, _DevinfoData);
                        break;
                    }
                    i++;
                }
                SetupDiDestroyDeviceInfoList(_MainIntPtr);
            }

            private void SetHardwareEnabled(IntPtr m_MainIntPtr, bool p_Enabled, SP_DEVINFO_DATA p_DevInfoData)
            {
                SP_PROPCHANGE_PARAMS _HardwareParams = new SP_PROPCHANGE_PARAMS();
                _HardwareParams.ClassInstallHeader.cbSize = Marshal.SizeOf(typeof(SP_CLASSINSTALL_HEADER));
                _HardwareParams.ClassInstallHeader.InstallFunction = (int)DIF.DIF_PROPERTYCHANGE;
                _HardwareParams.StateChange = (int)DICS.DICS_DISABLE;
                _HardwareParams.Scope = (int)DICS.DICS_FLAG_CONFIGSPECIFIC;

                if (p_Enabled)
                {
                    _HardwareParams.StateChange = (int)DICS.DICS_ENABLE;
                    _HardwareParams.Scope = (int)DICS.DICS_FLAG_GLOBAL;

                    IntPtr _HardwareParamsIntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(_HardwareParams));
                    Marshal.StructureToPtr(_HardwareParams, _HardwareParamsIntPtr, true);
                    IntPtr _DevInfoDataIntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(p_DevInfoData));

                    if (SetupDiSetClassInstallParams(m_MainIntPtr, _DevInfoDataIntPtr, _HardwareParamsIntPtr, Marshal.SizeOf(typeof(SP_PROPCHANGE_PARAMS))))
                    {
                        SetupDiCallClassInstaller(DIF.DIF_PROPERTYCHANGE, m_MainIntPtr, _DevInfoDataIntPtr);
                    }
                    _HardwareParams.ClassInstallHeader.cbSize = Marshal.SizeOf(typeof(SP_CLASSINSTALL_HEADER));
                    _HardwareParams.ClassInstallHeader.InstallFunction = (int)DIF.DIF_PROPERTYCHANGE;
                    _HardwareParams.StateChange = (int)DICS.DICS_ENABLE;
                    _HardwareParams.Scope = (int)DICS.DICS_FLAG_CONFIGSPECIFIC;
                    _HardwareParams.HwProfile = 0;
                }

                IntPtr _SetHardwareParamsIntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(_HardwareParams));
                Marshal.StructureToPtr(_HardwareParams, _SetHardwareParamsIntPtr, true);

                IntPtr _SetDevInfoDataIntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(p_DevInfoData));
                Marshal.StructureToPtr(p_DevInfoData, _SetDevInfoDataIntPtr, true);

                SetupDiSetClassInstallParams(m_MainIntPtr, _SetDevInfoDataIntPtr, _SetHardwareParamsIntPtr, Marshal.SizeOf(typeof(SP_PROPCHANGE_PARAMS)));
                SetupDiCallClassInstaller(DIF.DIF_PROPERTYCHANGE, m_MainIntPtr, _SetDevInfoDataIntPtr);
            }
        }
    }
}
