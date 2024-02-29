namespace WindowsCoreAudio
{
    using System;

    using WindowsCoreAudio.API;

    public static class PropertyKeys
    {
        public static PropertyKey PKEY_AppUserModel_Background = new PropertyKey
        {
            FormatId = Guid.Parse("{86D40B4D-9069-443C-819A-2A54090DCCEC}"),
            PropertyId = new UIntPtr(4)
        };

        public static PropertyKey PKEY_AppUserModel_PackageFullName = new PropertyKey
        {
            FormatId = Guid.Parse("{9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3}"),
            PropertyId = new UIntPtr(21)
        };

        public static PropertyKey PKEY_AppUserModel_PackageInstallPath = new PropertyKey
        {
            FormatId = Guid.Parse("{9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3}"),
            PropertyId = new UIntPtr(15)
        };

        public static PropertyKey PKEY_AudioEndPoint_Interface = new PropertyKey
        {
            FormatId = Guid.Parse("{A45C254E-DF1C-4EFD-8020-67D146A850E0}"),
            PropertyId = new UIntPtr(2)
        };

        public static PropertyKey PKEY_AudioEndpoint_PhysicalSpeakers = new PropertyKey
        {
            FormatId = Guid.Parse("{0x1DA5D803, 0xD492, 0x4EDD, {0x8C, 0x23, 0xE0, 0xC0, 0xFF, 0xEE, 0x7F, 0x0E}}"),
            PropertyId = new UIntPtr(3)
        };

        public static PropertyKey PKEY_Device_DeviceDesc = new PropertyKey
        {
            FormatId = Guid.Parse("{0xA45C254E, 0xDF1C, 0x4EFD, {0x80, 0x20, 0x67, 0xD1, 0x46, 0xA8, 0x50, 0xE0}}"),
            PropertyId = new UIntPtr(2)
        };

        public static PropertyKey PKEY_Device_EnumeratorName = new PropertyKey
        {
            FormatId = Guid.Parse("{0xA45C254E, 0xDF1C, 0x4EFD, {0x80, 0x20, 0x67, 0xD1, 0x46, 0xA8, 0x50, 0xE0}}"),
            PropertyId = new UIntPtr(24)
        };

        public static PropertyKey PKEY_Device_FriendlyName = new PropertyKey
        {
            FormatId = Guid.Parse("{0xA45C254E, 0xDF1C, 0x4EFD, {0x80, 0x20, 0x67, 0xD1, 0x46, 0xA8, 0x50, 0xE0}}"),
            PropertyId = new UIntPtr(14)
        };

        public static PropertyKey PKEY_Device_IconPath = new PropertyKey
        {
            FormatId = Guid.Parse("{0x259ABFFC, 0x50A7, 0x47CE, {0xAF, 0x8, 0x68, 0xC9, 0xA7, 0xD7, 0x33, 0x66}}"),
            PropertyId = new UIntPtr(12)
        };

        public static PropertyKey PKEY_Device_InstanceId = new PropertyKey
        {
            FormatId = Guid.Parse("{0x78C34FC8, 0x104A, 0x4ACA, {0x9E, 0xA4, 0x52, 0x4D, 0x52, 0x99, 0x6E, 0x57}}"),
            PropertyId = new UIntPtr(2)
        };

        public static PropertyKey PKEY_DeviceInterface_FriendlyName = new PropertyKey
        {
            FormatId = Guid.Parse("{0x026E516E, 0xB814, 0x414B, {0x83, 0xCD, 0x85, 0x6D, 0x6F, 0xEF, 0x48, 0x22}}"),
            PropertyId = new UIntPtr(2)
        };

        public static PropertyKey PKEY_ItemNameDisplay = new PropertyKey
        {
            FormatId = Guid.Parse("{B725F130-47EF-101A-A5F1-02608C9EEBAC}"),
            PropertyId = new UIntPtr(10)
        };

        public static PropertyKey PKEY_Tile_SmallLogoPath = new PropertyKey
        {
            FormatId = Guid.Parse("{86D40B4D-9069-443C-819A-2A54090DCCEC}"),
            PropertyId = new UIntPtr(2)
        };
    }
}


//Name

//+ FormatId    { a45c254e - df1c - 4efd - 8020 - 67d146a850e0}
//System.Guid
//        PropertyId	0x0000018b00000002	System.UIntPtr

//+		FormatId	{b3f8fa53-0004-438e-9003-51a46e139bfc}	System.Guid
//        PropertyId	0x0000018b00000003	System.UIntPtr

//+		FormatId	{b3f8fa53-0004-438e-9003-51a46e139bfc}	System.Guid
//        PropertyId	0x0000018b00000004	System.UIntPtr

//+		FormatId	{1da5d803-d492-4edd-8c23-e0c0ffee7f0e}	System.Guid
//        PropertyId	0x0000018b00000005	System.UIntPtr


//namespace WindowsCoreAudio
//{
//    using System;
//    using System.Collections.Concurrent;
//    using System.Collections.Generic;
//    using System.Linq;

//    using WindowsCoreAudio.API;

//    public static class PropertyKeys
//    {
//        private static ConcurrentDictionary<string, PropertyKey> Items { get; }

//        public static List<string> Names
//        {
//            get
//            {
//                return new List<string>(Items.Keys);
//            }
//        }

//        public static List<PropertyKey> Keys
//        {
//            get
//            {
//                return new List<PropertyKey>(Items.Values);
//            }
//        }

//        static PropertyKeys()
//        {
//            Items = new ConcurrentDictionary<string, PropertyKey>();

//            Items["PKEY_AppUserModel_Background"] = new PropertyKey
//            (
//                Guid.Parse("{86D40B4D-9069-443C-819A-2A54090DCCEC}"),
//                new UIntPtr(4)
//            );

//            Items["PKEY_AppUserModel_PackageFullName"] = new PropertyKey
//            (
//                Guid.Parse("{9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3}"),
//                new UIntPtr(21)
//            );

//            Items["PKEY_AppUserModel_PackageInstallPath"] = new PropertyKey
//            (
//                Guid.Parse("{9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3}"),
//                new UIntPtr(15)
//            );

//            Items["PKEY_AudioEndPoint_Interface"] = new PropertyKey
//            (
//                Guid.Parse("{A45C254E-DF1C-4EFD-8020-67D146A850E0}"),
//                new UIntPtr(2)
//            );

//            Items["PKEY_AudioEndpoint_PhysicalSpeakers"] = new PropertyKey
//            (
//                Guid.Parse("{0x1DA5D803, 0xD492, 0x4EDD, {0x8C, 0x23, 0xE0, 0xC0, 0xFF, 0xEE, 0x7F, 0x0E}}"),
//                new UIntPtr(3)
//            );

//            Items["PKEY_Device_DeviceDesc"] = new PropertyKey
//            (
//                Guid.Parse("{0xA45C254E, 0xDF1C, 0x4EFD, {0x80, 0x20, 0x67, 0xD1, 0x46, 0xA8, 0x50, 0xE0}}"),
//                new UIntPtr(2)
//            );

//            Items["PKEY_Device_EnumeratorName"] = new PropertyKey
//            (
//                Guid.Parse("{0xA45C254E, 0xDF1C, 0x4EFD, {0x80, 0x20, 0x67, 0xD1, 0x46, 0xA8, 0x50, 0xE0}}"),
//                new UIntPtr(24)
//            );

//            Items["PKEY_Device_FriendlyName"] = new PropertyKey
//            (
//                Guid.Parse("{0xA45C254E, 0xDF1C, 0x4EFD, {0x80, 0x20, 0x67, 0xD1, 0x46, 0xA8, 0x50, 0xE0}}"),
//                new UIntPtr(14)
//            );

//            Items["PKEY_Device_IconPath"] = new PropertyKey
//            (
//                Guid.Parse("{0x259ABFFC, 0x50A7, 0x47CE, {0xAF, 0x8, 0x68, 0xC9, 0xA7, 0xD7, 0x33, 0x66}}"),
//                new UIntPtr(12)
//            );

//            Items["PKEY_Device_InstanceId"] = new PropertyKey
//            (
//                Guid.Parse("{0x78C34FC8, 0x104A, 0x4ACA, {0x9E, 0xA4, 0x52, 0x4D, 0x52, 0x99, 0x6E, 0x57}}"),
//                new UIntPtr(2)
//            );

//            Items["PKEY_DeviceInterface_FriendlyName"] = new PropertyKey
//            (
//                Guid.Parse("{0x026E516E, 0xB814, 0x414B, {0x83, 0xCD, 0x85, 0x6D, 0x6F, 0xEF, 0x48, 0x22}}"),
//                new UIntPtr(2)
//            );

//            Items["PKEY_ItemNameDisplay"] = new PropertyKey
//            (
//                Guid.Parse("{B725F130-47EF-101A-A5F1-02608C9EEBAC}"),
//                new UIntPtr(10)
//            );

//            Items["PKEY_Tile_SmallLogoPath"] = new PropertyKey
//            (
//                Guid.Parse("{86D40B4D-9069-443C-819A-2A54090DCCEC}"),
//                new UIntPtr(2)
//            );
//        }

//        public static PropertyKey? GetKey(string name)
//        {
//            if (Items.TryGetValue(name, out PropertyKey key))
//            {
//                return key;
//            }
//            return null;
//        }

//        public static string GetName(PropertyKey key)
//        {
//            if (Items.FirstOrDefault(x => x.Value == key).Key is string name)
//            {
//                return name;
//            }
//            return null;
//        }
//    }
//}
