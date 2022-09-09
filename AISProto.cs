//
// dkxce Marine AIS Packet Constructor and Parser
// AIS Implementation for C#
// https://github.com/dkxce/Marine_AIS_Packet
//

using System;
using System.Collections.Generic;
using System.Text;

namespace dkxce.AIS
{
    #region Enums
    /// <summary>
    ///     AIS Ship Type, TABLE 18 Rec. ITU-R M.13171-1,
    ///     https://api.vtexplorer.com/docs/ref-aistypes.html
    /// </summary>
    public enum AisShipType : uint
    {
        Default = 0,
        // 1..19 Reserved
        WIG_AllShips = 20, // Wing in ground
        WIG_HazardousCategoryA = 21,
        WIG_HazardousCategoryB = 22,
        WIG_HazardousCategoryC = 23,
        WIG_HazardousCategoryD = 24,
        // 25..29 Wing in ground (WIG), Reserved for future use
        Fishing = 30,
        Towing = 31,
        TowingBig = 32, // Towing: length exceeds 200m or breadth exceeds 25m
        DredgingOrUnderwater = 33, // Dredging or underwater ops
        Diving = 34,
        Military = 35,
        Sailing = 36,
        PleasureCraft = 37,
        // 38..39 Reserved
        HighSpeedCraft_All = 40, // HSC
        HighSpeedCraft_A = 41,
        HighSpeedCraft_B = 42,
        HighSpeedCraft_C = 43,
        HighSpeedCraft_D = 44,
        // 45..48 Reserved
        HighSpeedCraft_NoInfo = 49,
        PilotVessel = 50,
        SearchRescue = 51,
        Tug = 52,
        PortTender = 53,
        AntiPollutionEquipment = 54,
        LawEnforcement = 55,
        SpareLocalVessel = 56,
        SpareLocalVessel2 = 57,        
        MedicalTransport = 58,
        NoncombatantShipAccording2RRResolutionNo_18 = 59,
        Passenger_All = 60,
        Passenger_A = 61,
        Passenger_B = 62,
        Passenger_C = 63,
        Passenger_D = 64,
        // 65..68 Reserved
        Passenger_NoInfo = 69,
        Cargo_All = 70,
        Cargo_A = 71,
        Cargo_B = 72,
        Cargo_C = 73,
        Cargo_D = 74,
        // 75..78 Reserved
        Cargo_NoInfo = 79,
        Tanker_All = 80,
        Tanker_A = 81,
        Tanker_B = 82,
        Tanker_C = 83,
        Tanker_D = 84,
        // 85..88 Reserved
        Tanker_NoInfo = 89,
        OtherType_All = 90,
        OtherType_HazardousCategory_A = 91,
        OtherType_HazardousCategory_B = 92,
        OtherType_HazardousCategory_C = 93,
        OtherType_HazardousCategory_D = 94,
        // 95..98 Reserved
        OtherType_NoInfo = 99
    }

    /// <summary>
    ///     AIS Navigation Status, TABLE 15a Rec. ITU-R M.13171-1,
    ///     https://api.vtexplorer.com/docs/ref-navstat.html
    /// </summary>
    public enum AisNavigationStatus: uint
    {
        UnderWayUsingEngine = 0,
        AtAnchor = 1,
        NotUnderCommand = 2,
        RestrictedManoeuverability = 3,
        ConstrainedByHerDraught = 4,
        Moored = 5,
        Aground = 6,
        EngagedInFishing = 7,
        UnderWaySailing = 8,
        ReservedHSC = 9,
        ReservedWIG = 10, // Wing in ground
        Reserved11 = 11,
        Reserved12 = 12,
        Reserved13 = 13,
        AISSARTisActive = 14,
        NotDefined = 15
    }

    /// <summary>
    ///    AIS Maneuver Indicator, TABLE 15a Rec. ITU-R M.13171-1
    /// </summary>
    public enum AisManeuverIndicator: uint
    {
        NotAvailable = 0,
        NoSpecialManeuver = 1,
        SpecialManeuver = 2
    }

    /// <summary>
    ///     AIS Communication Status, TABLE 15b Rec. ITU-R M.13171-1
    /// </summary>
    public enum AisCommunicationStatus: uint
    {
        DirectUTC = 0,
        UndirectUTC = 1,
        BaseStationSync = 2,
        AnotherStationSync = 3
    }

    /// <summary>
    ///     AIS Position Fix Status, TABLE 16 Rec. ITU-R M.13171-1
    /// </summary>
    public enum AisPosFix : uint
    { 
        Undefined = 0,
        GPS = 1,
        Glonass = 2,
        GPS_Glonass = 3,
        LoranC = 4,
        Chayka = 5,
        IntergatedNavSys = 6,
        Surveyed = 7
    }

    /// <summary>
    ///     AIS Version Indicator, TABLE 17 Rec. ITU-R M.13171-1
    /// </summary>
    public enum AisVersion
    {
        AIS_0 = 0,
        AIS_1 = 1,
        AIS_2 = 2,
        AIS_3 = 3
    }
    #endregion Enums

    public interface AisISentense
    {
        byte[] ToAisUnpacked();

        string ToAisSentense();
        byte[] ToAisEnpacked();

        string ToAisFullPacket();
        byte[] ToAisFullDataMsg();
    }

    public interface AisIPositionReport
    {
        string ToAisPositionPacket();
        byte[] ToAisPositionDataMsg();
    }

    public abstract class AisSentense: AisISentense
    {
        /// <summary>
        ///     Packet Message Identifier, 6 bits (offset 0)
        /// </summary>
        public byte MessageID { get; protected set; } = 0;

        /// <summary>
        ///     Packet Repeat Indicator, 2 bits (offset 6)
        /// </summary>
        public byte Repeat = 0;

        /// <summary>
        ///     Maritime Mobile Service Identity, 30 Bits (offset 8)
        /// </summary>
        public uint MMSI;

        public virtual string ToString()
        {
            if (this.MessageID == 0)
                return "Empty_AisSentense";
            else
                return $"{this.MessageID}_AisSentense";
        }

        public virtual string ToAisSentense()
        {
            return this.ToString();
        }

        public virtual byte[] ToAisUnpacked()
        {
            return new byte[0];
        }

        public virtual byte[] ToAisEnpacked()
        {
            return AISTransCoder.EnpackAISBytes(this.ToAisUnpacked());
        }

        public virtual string ToAisFullPacket()
        {
            if (this.MessageID == 0) return String.Empty;

            string res = "!AIVDM,1,1,,A," + this.ToString() + ",0";
            res += "*" + AISTransCoder.Checksum(res);
            res = res + "\r\n";
            return res;
        }

        public virtual byte[] ToAisFullDataMsg()
        {
            if (this.MessageID == 0) return new byte[0];

            return Encoding.ASCII.GetBytes(ToAisFullPacket());
        }       
    }

    // TABLE 13 (p.40 Rec. ITU-R M.13171-1):
    //  * PACKET 1, TABLE 15a (p.43 Rec. ITU-R M.13171-1)
    //  * PACKET 2, TABLE 15a (p.43 Rec. ITU-R M.13171-1)
    //  * PACKET 3, TABLE 15a (p.43 Rec. ITU-R M.13171-1)
    /// <summary>
    ///     1 - Position Report Class A (Scheduled Position Report),
    ///     2 - Position Report Class A (Assigned Scheduled Position Report),
    ///     3 - Position Report Class A (Special Position Report);
    /// </summary>
    public class PositionReport123 : AisSentense, AisISentense, AisIPositionReport
    {
        private const ushort  TotalNumberOfBits  = 168;
        private static ushort TotalNumberOfBytes = TotalNumberOfBits / 8;

        public PositionReport123() { MessageID = 3; }

        // MessageID, 6 Bits (offset 0)
        // Repeat, 2 Bits (offset 6)
        // MMSI, 30 Bits (offset 8)
        
        /// <summary>
        ///     AIS Navigation Status, 4 Bits (offset 38)
        /// </summary>
        private AisNavigationStatus NavigationStatus = AisNavigationStatus.NotDefined;
        /// <summary>
        ///     Rate-of-Turn, 8 Bits (offset 42)
        /// </summary>
        private int ROT = 0;
        /// <summary>
        ///     Speed-over-Ground, kmph, 10 Bits (offset 50)
        /// </summary>
        public uint SOG = 0;
        /// <summary>
        ///     Position Accuracy, 1 Bits (offset 60)
        /// </summary>
        public bool Accuracy = false;
        /// <summary>
        ///     Longitude, degrees, 28 Bits (offset 61)
        /// </summary>
        public double Longitude = 0;
        /// <summary>
        ///     Latitude, degrees, 27 Bits (offset 89)
        /// </summary>
        public double Latitude = 0;
        /// <summary>
        ///     Course-Over-Ground, degrees, 12 Bits (offset 116)
        /// </summary>
        public double COG = 0;
        /// <summary>
        ///     True Heading, degrees, 9 Bits (offset 128)
        /// </summary>
        public ushort HDG = 0;
        /// <summary>
        ///      UTC timestamp, 6 Bits (offset 137)
        ///      0..60 - Not Available, 61 - Manual Mode, 62 - Dead Reckoning, 63 - Inoperative
        /// </summary>
        public uint TimeStamp = 60;
        /// <summary>
        ///     Maneuver Indicator, 2 Bits (offset 143)
        /// </summary>
        public AisManeuverIndicator ManeuverIndicator = AisManeuverIndicator.NoSpecialManeuver;
        /// <summary>
        ///     Radio status, 19 Bits (offset 149)
        /// </summary>
        public AisCommunicationStatus RadioStatus = AisCommunicationStatus.DirectUTC;

        public static PositionReport123 FromAIS(byte[] unpackedBytes)
        {
            if (unpackedBytes == null) return null;
            if (unpackedBytes.Length < TotalNumberOfBytes) return null;
            int stype = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 0, 6);
            if ((stype < 1) || (stype > 3)) return null;

            PositionReport123 res = new PositionReport123();
            res.MessageID = (byte)stype;
            res.Repeat = (byte)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 6, 2);
            res.MMSI = (uint)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 8, 30);
            res.NavigationStatus = (AisNavigationStatus)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 38, 4);
            res.ROT = AISTransCoder.GetBitsAsSignedInt(unpackedBytes, 42, 8);
            res.SOG = (uint)(AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 50, 10) / 10 * 1.852);
            res.Accuracy = (byte)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 60, 1) == 1 ? true : false;
            res.Longitude = AISTransCoder.GetBitsAsSignedInt(unpackedBytes, 61, 28) / 600000.0;
            res.Latitude = AISTransCoder.GetBitsAsSignedInt(unpackedBytes, 89, 27) / 600000.0;
            res.COG = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 116, 12) / 10.0;
            res.HDG = (ushort)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 128, 9);
            res.TimeStamp = (uint)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 137, 6);
            res.ManeuverIndicator = (AisManeuverIndicator)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 143, 2);
            res.RadioStatus = (AisCommunicationStatus)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 149, 19);

            return res;
        }

        public static PositionReport123 FromAIS(string ais)
        {
            byte[] unp = AISTransCoder.UnpackAISString(ais);
            return FromAIS(unp);
        }

        public override byte[] ToAisUnpacked()
        {
            int acc = Accuracy ? 1 : 0;

            byte[] unpackedBytes = new byte[TotalNumberOfBytes];
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 000, 006, (int)MessageID);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 006, 002, (int)Repeat);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 008, 030, (int)MMSI);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 038, 004, (int)NavigationStatus);
            AISTransCoder.SetBitsAsSignedInt(unpackedBytes,   042, 008, (int)ROT);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 050, 010, (int)(SOG / 1.852 * 10));
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 060, 001, (int)acc);
            AISTransCoder.SetBitsAsSignedInt(unpackedBytes,   061, 028, (int)(Longitude * 600000));
            AISTransCoder.SetBitsAsSignedInt(unpackedBytes,   089, 027, (int)(Latitude * 600000));
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 116, 012, (int)(COG * 10));
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 128, 009, (int)HDG);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 137, 006, (int)TimeStamp);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 143, 002, (int)ManeuverIndicator);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 145, 002, (int)0); // Reserved
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 147, 001, (int)0); // Spare
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 148, 001, (int)0); // RAIM
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 149, 019, (int)RadioStatus);
            return unpackedBytes;
        }

        public override string ToString()
        {
            return AISTransCoder.EnpackAISString(ToAisUnpacked());
        }

        public string ToAisPositionPacket()
        {
            string ln1 = "!AIVDM,1,1,,A," + this.ToString() + ",0";
            ln1 += "*" + AISTransCoder.Checksum(ln1);
            string res = ln1 + "\r\n";
            return res;
        }

        public byte[] ToAisPositionDataMsg()
        {
            return Encoding.ASCII.GetBytes(ToAisPositionPacket());
        }

        public string ToAisPositionPacket(ShipStaticAndVoyageRelatedReport5 ai)
        {
            string ln1 = "!AIVDM,1,1,,A," + this.ToString() + ",0";
            ln1 += "*" + AISTransCoder.Checksum(ln1);
            string ln2 = "!AIVDM,1,1,,A," + ai.ToString() + ",0";
            ln2 += "*" + AISTransCoder.Checksum(ln2);
            string res = ln1 + "\r\n" + ln2 + "\r\n";
            return res;
        }

        public byte[] ToAisPositionPacketAsBytes(ShipStaticAndVoyageRelatedReport5 ai)
        {
            return Encoding.ASCII.GetBytes(ToAisPositionPacket(ai));
        }
    }

    // TABLE 13 (p.40 Rec. ITU-R M.13171-1):
    //  * PACKET 4, TABLE 16 (p.44 Rec. ITU-R M.13171-1)
    //  * PACKET 11, TABLE 16 (p.44 Rec. ITU-R M.13171-1)
    /// <summary>
    ///     4 - Base station report,
    ///     11 - UTC/date response;
    /// </summary>
    public class BaseStationReport4: AisSentense, AisISentense, AisIPositionReport
    {
        private const ushort TotalNumberOfBits = 168;
        private static ushort TotalNumberOfBytes = TotalNumberOfBits / 8;

        public BaseStationReport4() { MessageID = 4; }
        public BaseStationReport4(bool utcResponse) { MessageID = utcResponse ? (byte)11 : (byte)4; }

        // MessageID, 6 Bits (offset 0)
        // Repeat, 2 Bits (offset 6)
        // MMSI, 30 Bits (offset 8)

        /// <summary>
        ///     DateTime UTC, 40 Bits (offset 38)
        /// </summary>
        public DateTime? UTC;
        /// <summary>
        ///     Position Accuracy, 1 Bits (offset 78)
        /// </summary>
        public bool Accuracy = false;
        /// <summary>
        ///     Longitude, degrees, 28 Bits (offset 79)
        /// </summary>
        public double Longitude = 0;
        /// <summary>
        ///     Latitude, degrees, 27 Bits (offset 107)
        /// </summary>
        public double Latitude = 0;
        /// <summary>
        ///     GPS Position Fix, 4 Bits (offset 134)
        /// </summary>
        public AisPosFix PosFix = AisPosFix.Undefined;
        /// <summary>
        ///     Radio status, 19 Bits (offset 149)
        /// </summary>
        private AisCommunicationStatus RadioStatus = AisCommunicationStatus.DirectUTC;

        public static BaseStationReport4 FromAIS(byte[] unpackedBytes)
        {
            if (unpackedBytes == null) return null;
            if (unpackedBytes.Length < TotalNumberOfBytes) return null;
            int stype = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 0, 6);
            if ((stype != 4) && (stype != 11)) return null;

            BaseStationReport4 res = new BaseStationReport4();
            res.MessageID = (byte)stype;
            res.Repeat = (byte)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 6, 2);
            res.MMSI = (uint)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 8, 30);

            int yy = (int)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 14, 38);
            int MM = (int)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 4, 52);
            int dd = (int)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 5, 56);
            int hh = (int)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 5, 61);
            int mm = (int)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 6, 66);
            int ss = (int)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 6, 72);
            if ((yy != 0) || (MM != 0) || (dd != 0) || (hh != 24) || (mm != 60) || (ss != 60))
            {
                if (yy == 0) yy = DateTime.UtcNow.Year;
                if (MM == 0) MM = DateTime.UtcNow.Month;
                if (dd == 0) dd = DateTime.UtcNow.Day;
                if (hh == 24) hh = DateTime.UtcNow.Hour;
                if (hh == 60) mm = DateTime.UtcNow.Minute;
                if (hh == 60) ss = DateTime.UtcNow.Second;
                res.UTC = new DateTime(yy, MM, dd, hh, mm, ss, DateTimeKind.Utc);
            };

            res.Accuracy = (byte)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 78, 1) == 1 ? true : false;
            res.Longitude = AISTransCoder.GetBitsAsSignedInt(unpackedBytes, 79, 28) / 600000.0;
            res.Latitude = AISTransCoder.GetBitsAsSignedInt(unpackedBytes, 107, 27) / 600000.0;
            res.PosFix = (AisPosFix)AISTransCoder.GetBitsAsSignedInt(unpackedBytes, 134, 4);
            res.RadioStatus = (AisCommunicationStatus)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 149, 19);

            return res;
        }

        public static BaseStationReport4 FromAIS(string ais)
        {
            byte[] unp = AISTransCoder.UnpackAISString(ais);
            return FromAIS(unp);
        }

        public override byte[] ToAisUnpacked()
        {
            int acc = Accuracy ? 1 : 0;

            byte[] unpackedBytes = new byte[TotalNumberOfBytes];
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 000, 006, (int)MessageID);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 006, 002, (int)Repeat);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 008, 030, (int)MMSI);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 038, 014, UTC == null ? (int)00 : (int)UTC.Value.Year);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 052, 004, UTC == null ? (int)00 : (int)UTC.Value.Month);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 056, 005, UTC == null ? (int)00 : (int)UTC.Value.Day);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 061, 005, UTC == null ? (int)24 : (int)UTC.Value.Hour);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 066, 006, UTC == null ? (int)60 : (int)UTC.Value.Minute);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 072, 006, UTC == null ? (int)60 : (int)UTC.Value.Second);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 078, 001, (int)acc);
            AISTransCoder.SetBitsAsSignedInt(unpackedBytes,   079, 028, (int)(Longitude * 600000));
            AISTransCoder.SetBitsAsSignedInt(unpackedBytes,   107, 027, (int)(Latitude * 600000));
            AISTransCoder.SetBitsAsSignedInt(unpackedBytes,   134, 004, (int)PosFix);
            AISTransCoder.SetBitsAsSignedInt(unpackedBytes,   138, 010, (int)0); // Spare
            AISTransCoder.SetBitsAsSignedInt(unpackedBytes,   148, 001, (int)0); // RAIM
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 149, 019, (int)RadioStatus);
            return unpackedBytes;
        }

        public override string ToString()
        {
            return AISTransCoder.EnpackAISString(ToAisUnpacked());
        }

        public string ToAisPositionPacket()
        {
            string ln1 = "!AIVDM,1,1,,A," + this.ToString() + ",0";
            ln1 += "*" + AISTransCoder.Checksum(ln1);
            string res = ln1 + "\r\n";
            return res;
        }

        public byte[] ToAisPositionDataMsg()
        {
            return Encoding.ASCII.GetBytes(ToAisPositionPacket());
        }
    }    

    // TABLE 13 (p.40 Rec. ITU-R M.13171-1):
    //  * PACKET 5, TABLE 17 (p.45 Rec. ITU-R M.13171-1)
    /// <summary>
    ///     5 - Static and voyage related data;
    /// </summary>
    public class ShipStaticAndVoyageRelatedReport5 : AisSentense
    {
        private const short   TotalNumberOfBits  = 424;
        private static ushort TotalNumberOfBytes = TotalNumberOfBits / 8;

        public ShipStaticAndVoyageRelatedReport5 () { MessageID = 5; }

        // MessageID, 6 Bits (offset 0)
        // Repeat, 2 Bits (offset 6)
        // MMSI, 30 Bits (offset 8)


        /// <summary>
        ///     AIS Version, 2 Bits (offset 38)
        /// </summary>
        public AisVersion AisVer { get; private set; } = AisVersion.AIS_0;
        /// <summary>
        ///     IMO Number, 30 Bits (offset 40)
        /// </summary>
        public uint IMOShipID;
        /// <summary>
        ///     7-symbols Callsign, 42 Bits (offset 70)
        /// </summary>
        public string CallSign;
        /// <summary>
        ///     20-symbols Vessel, 120 Bits (offset 112)
        /// </summary>
        public string VesselName;
        /// <summary>
        ///     Type of Ship/Cargo, 8 Bits (offset 232)
        /// </summary>
        public AisShipType ShipType = AisShipType.Default;
        /// <summary>
        ///     Ship Dimentions A-B-C-D, 30 Bits (offset 240)
        ///     FIGURE 17 (p.48 Rec. ITU-R M.13171-1)
        /// </summary>
        public int[] Dimensions = new int[4];
        /// <summary>
        ///     GPS Position Fix, 4 Bits (offset 270)
        /// </summary>
        public AisPosFix PosFix = AisPosFix.Undefined;
        /// <summary>
        ///     Estimated time of arrival, UTC, 20 Bits (offset 274)
        /// </summary>
        public DateTime? ETA = null;
        /// <summary>
        ///     Maximum present static draught, m, 8 Bits (offset 294)
        /// </summary>
        public double Draught = 0;
        /// <summary>
        ///     20-symbols Destination, 120 Bits (offset 302)
        /// </summary>
        public string Destination = "";
        /// <summary>
        ///     Data Terminal Ready, 1 Bit (offset 422)
        /// </summary>
        public bool DataTerminalReady = false;

        public static ShipStaticAndVoyageRelatedReport5 FromAIS(byte[] unpackedBytes)
        {
            if (unpackedBytes == null) return null;
            if (unpackedBytes.Length < TotalNumberOfBytes) return null;            
            int stype = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 0, 6);
            if (stype != 5) return null;

            ShipStaticAndVoyageRelatedReport5  res = new ShipStaticAndVoyageRelatedReport5 ();
            res.MessageID = (byte)stype;
            res.Repeat = (byte)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 6, 2);
            res.MMSI = (uint)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 8, 30);
            res.AisVer = (AisVersion)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 2, 38);
            res.IMOShipID = (uint)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 40, 30);
            res.CallSign = AISTransCoder.GetAisString(unpackedBytes, 70, 42);
            res.VesselName = AISTransCoder.GetAisString(unpackedBytes, 112, 120);
            res.ShipType = (AisShipType)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 232, 8);
            res.Dimensions[0] = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 240, 9);
            res.Dimensions[1] = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 249, 9);
            res.Dimensions[2] = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 258, 6);
            res.Dimensions[3] = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 264, 6);
            res.PosFix = (AisPosFix)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 270, 4);
            
            int MM = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 274, 4);
            int dd = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 278, 4);
            int hh = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 282, 6);
            int mm = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 288, 6);
            if((MM != 0) || (dd != 0) || (hh != 24) || (mm != 60))
            {
                if (MM == 0) MM = DateTime.UtcNow.Month;
                if (dd == 0) dd = DateTime.UtcNow.Day;
                if (hh == 0) hh = DateTime.UtcNow.Hour;
                if (mm == 0) mm = DateTime.UtcNow.Minute;
                res.ETA = new DateTime(MM < DateTime.UtcNow.Month ? DateTime.UtcNow.Year + DateTime.UtcNow.Year : DateTime.UtcNow.Year, MM, dd, hh, mm, 00, DateTimeKind.Utc);
            };
            res.Draught = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 294, 8) * 10.0;
            res.Destination = AISTransCoder.GetAisString(unpackedBytes, 302, 120);
            res.DataTerminalReady = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 422, 1) == 0;

            return res;
        }

        public static ShipStaticAndVoyageRelatedReport5  FromAIS(string ais)
        {
            byte[] unp = AISTransCoder.UnpackAISString(ais);
            return FromAIS(unp);
        }

        public override byte[] ToAisUnpacked()
        {
            string callsign = string.IsNullOrEmpty(CallSign) ? "UNKNOWN" : CallSign.ToUpper();
            if (callsign.Length > 7) callsign = callsign.Substring(0, 7);
            string vessel = string.IsNullOrEmpty(VesselName) ? "UNKNOWN" : VesselName.ToUpper();
            if (vessel.Length > 20) vessel = vessel.Substring(0, 20);
            string dest = string.IsNullOrEmpty(Destination) ? "UNKNOWN" : Destination.ToUpper();
            if (dest.Length > 20) dest = dest.Substring(0, 20);

            byte[] unpackedBytes = new byte[TotalNumberOfBytes]; // 54?
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 000, 006, (int)MessageID);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 006, 002, (int)Repeat);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 008, 030, (int)MMSI);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 038, 002, (int)AisVer); // AIS Version
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 040, 030, (int)IMOShipID);
            AISTransCoder.SetAisString(unpackedBytes,         070, 042, callsign);
            AISTransCoder.SetAisString(unpackedBytes,         112, 120, vessel);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 232, 008, (int)ShipType);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 240, 009, Dimensions == null || Dimensions.Length != 4 ? (int)0 : (int)Dimensions[0]); //A DIMENTIONS
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 249, 009, Dimensions == null || Dimensions.Length != 4 ? (int)0 : (int)Dimensions[1]); //B DIMENTIONS
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 258, 006, Dimensions == null || Dimensions.Length != 4 ? (int)0 : (int)Dimensions[2]); //C DIMENTIONS
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 264, 006, Dimensions == null || Dimensions.Length != 4 ? (int)0 : (int)Dimensions[3]); //D DIMENTIONS
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 270, 004, (int)PosFix);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 274, 004, ETA == null ? 00 : ETA.Value.Month);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 278, 005, ETA == null ? 00 : ETA.Value.Day);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 283, 005, ETA == null ? 24 : ETA.Value.Hour);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 288, 006, ETA == null ? 60 : ETA.Value.Minute);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 294, 008, (int)Draught); // Maximum present static draught
            AISTransCoder.SetAisString(unpackedBytes,         302, 120, dest);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 422, 001, DataTerminalReady ? 0 : 1); // DTE
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 423, 001, (int)0); // Spare
            return unpackedBytes;
        }

        public override string ToString()
        {
            return AISTransCoder.EnpackAISString(ToAisUnpacked());
        }        
    }

    // TABLE 13 (p.40 Rec. ITU-R M.13171-1):
    //  * PACKET 6, TABLE 19 (p.49 Rec. ITU-R M.13171-1)
    /// <summary>
    ///     6 - Addressed Binary Message;
    /// </summary>
    public class AddressedBinaryMessage6: AisSentense
    {
        private const short MaxNumberOfBits = 1008;
        private static ushort MaxNumberOfBytes = MaxNumberOfBits / 8;

        public AddressedBinaryMessage6() { MessageID = 6; }

        // MessageID, 6 Bits (offset 0)
        // Repeat, 2 Bits (offset 6)
        // MMSI, 30 Bits (offset 8)

        /// <summary>
        ///     Sequence Number, 2 Bits (offset 38)
        /// </summary>
        public uint SeqNum = 0;
        /// <summary>
        ///     Maritime Mobile Service Identity, 30 Bits (offset 40)
        /// </summary>
        public uint DestMMSI;
        /// <summary>
        ///     Retransmit flag, 1 Bit (offset 70)
        /// </summary>
        public bool Retransmit = false;
        /// <summary>
        ///     Binary Data, Max 117 Bytes or 936 Bits (offset 72)
        /// </summary>
        public byte[] Data = new byte[0];

        public static AddressedBinaryMessage6 FromAIS(byte[] unpackedBytes)
        {
            if (unpackedBytes == null) return null;
            if (unpackedBytes.Length > MaxNumberOfBytes) return null;
            int stype = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 0, 6);
            if (stype != 6) return null;

            AddressedBinaryMessage6 res = new AddressedBinaryMessage6();
            res.MessageID = (byte)stype;
            res.Repeat = (byte)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 6, 2);
            res.MMSI = (uint)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 8, 30);
            res.SeqNum = (uint)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 38, 2);
            res.DestMMSI = (uint)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 40, 30);
            res.Retransmit = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 70, 1) == 1;
            List<byte> bytes = new List<byte>();
            for (int i = 9; i < unpackedBytes.Length; i++) bytes.Add(unpackedBytes[i]);
            res.Data = bytes.ToArray();

            return res;
        }

        public static AddressedBinaryMessage6 FromAIS(string ais)
        {
            byte[] unp = AISTransCoder.UnpackAISString(ais);
            return FromAIS(unp);
        }        

        public override byte[] ToAisUnpacked()
        {
            int dLen = Math.Min(Data.Length, 117);
            byte[] unpackedBytes = new byte[9 + dLen];
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 000, 006, (int)MessageID);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 006, 002, (int)Repeat);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 008, 030, (int)MMSI);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 038, 002, (int)SeqNum);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 040, 030, (int)DestMMSI);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 070, 001, Retransmit ? 1 : 0);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 071, 001, 0); // Spare
            for (int i = 0; i < dLen; i++) unpackedBytes[9 + i] = Data[i];
            return unpackedBytes;
        }

        public override string ToString()
        {
            return AISTransCoder.EnpackAISString(ToAisUnpacked());
        }
    }

    // NO MESSAGE Acknowledgement 7, 13

    // TABLE 13 (p.40 Rec. ITU-R M.13171-1):
    //  * PACKET 8, TABLE 22 (p.52 Rec. ITU-R M.13171-1)
    /// <summary>
    ///     8 - Binary Broadcast Message;
    /// </summary>
    public class BinaryBroadcastMessage8: AisSentense
    {
        private const short MaxNumberOfBits = 1008;
        private static ushort MaxNumberOfBytes = MaxNumberOfBits / 8;

        public BinaryBroadcastMessage8() { MessageID = 8; }

        // MessageID, 6 Bits (offset 0)
        // Repeat, 2 Bits (offset 6)
        // MMSI, 30 Bits (offset 8)

        /// <summary>
        ///     Binary Data, Max 121 Bytes or 968 Bits (offset 40)
        /// </summary>
        public byte[] Data = new byte[0];

        public static BinaryBroadcastMessage8 FromAIS(byte[] unpackedBytes)
        {
            if (unpackedBytes == null) return null;
            if (unpackedBytes.Length > MaxNumberOfBytes) return null;
            int stype = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 0, 6);
            if (stype != 8) return null;

            BinaryBroadcastMessage8 res = new BinaryBroadcastMessage8();
            res.MessageID = (byte)stype;
            res.Repeat = (byte)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 6, 2);
            res.MMSI = (uint)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 8, 30);
            List<byte> bytes = new List<byte>();
            for (int i = 5; i < unpackedBytes.Length; i++) bytes.Add(unpackedBytes[i]);
            res.Data = bytes.ToArray();

            return res;
        }

        public static BinaryBroadcastMessage8 FromAIS(string ais)
        {
            byte[] unp = AISTransCoder.UnpackAISString(ais);
            return FromAIS(unp);
        }

        public override byte[] ToAisUnpacked()
        {
            int dLen = Math.Min(Data.Length, 121);
            byte[] unpackedBytes = new byte[5 + dLen];
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 000, 006, (int)MessageID);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 006, 002, (int)Repeat);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 008, 030, (int)MMSI);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 038, 002, (int)0); // Spare
            for (int i = 0; i < dLen; i++) unpackedBytes[5 + i] = Data[i];
            return unpackedBytes;
        }

        public override string ToString()
        {
            return AISTransCoder.EnpackAISString(ToAisUnpacked());
        }        
    }
   
    // TABLE 13 (p.40 Rec. ITU-R M.13171-1):
    //  * PACKET 9, TABLE 23 (p.53 Rec. ITU-R M.13171-1)
    /// <summary>
    ///     9 - Aircraft Position Report;
    /// </summary>
    public class AircraftPositionReport9 : AisSentense, AisISentense, AisIPositionReport
    {
        private const ushort TotalNumberOfBits = 168;
        private static ushort TotalNumberOfBytes = TotalNumberOfBits / 8;

        public AircraftPositionReport9() { MessageID = 9; }

        // MessageID, 6 Bits (offset 0)
        // Repeat, 2 Bits (offset 6)
        // MMSI, 30 Bits (offset 8)

        /// <summary>
        ///     Altitude, m, 12 Bits (offset 38)
        /// </summary>
        public uint Altitude = 4095;
        /// <summary>
        ///     Speed-over-Ground, kmph, 10 Bits (offset 50)
        /// </summary>
        public uint SOG = 0;
        /// <summary>
        ///     Position Accuracy, 1 Bits (offset 60)
        /// </summary>
        public bool Accuracy = false;
        /// <summary>
        ///     Longitude, degrees, 28 Bits (offset 61)
        /// </summary>
        public double Longitude = 0;
        /// <summary>
        ///     Latitude, degrees, 27 Bits (offset 89)
        /// </summary>
        public double Latitude = 0;
        /// <summary>
        ///     Course-Over-Ground, degrees, 12 Bits (offset 116)
        /// </summary>
        public double COG = 0;
        /// <summary>
        ///      UTC timestamp, 6 Bits (offset 128)
        ///      0..60 - Not Available, 61 - Manual Mode, 62 - Dead Reckoning, 63 - Inoperative
        /// </summary>
        public uint TimeStamp = 60;
        /// <summary>
        ///     Data Terminal Ready, 1 Bit (offset 142)
        /// </summary>
        public bool DataTerminalReady = false;        
        /// <summary>
        ///     Radio status, 19 Bits (offset 149)
        /// </summary>
        private AisCommunicationStatus RadioStatus = AisCommunicationStatus.DirectUTC;

        public static AircraftPositionReport9 FromAIS(byte[] unpackedBytes)
        {
            if (unpackedBytes == null) return null;
            if (unpackedBytes.Length < TotalNumberOfBytes) return null;
            int stype = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 0, 6);
            if ((stype != 13)) return null;

            AircraftPositionReport9 res = new AircraftPositionReport9();
            res.MessageID = (byte)stype;
            res.Repeat = (byte)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 6, 2);
            res.MMSI = (uint)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 8, 30);
            res.Altitude = (uint)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 38, 12);
            res.SOG = (uint)(AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 50, 10) / 10 * 1.852);
            res.Accuracy = (byte)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 60, 1) == 1 ? true : false;
            res.Longitude = AISTransCoder.GetBitsAsSignedInt(unpackedBytes, 61, 28) / 600000.0;
            res.Latitude = AISTransCoder.GetBitsAsSignedInt(unpackedBytes, 89, 27) / 600000.0;
            res.COG = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 116, 12) / 10.0;
            res.TimeStamp = (uint)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 128, 6);
            res.DataTerminalReady = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 142, 2) == 0;
            res.RadioStatus = (AisCommunicationStatus)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 149, 19);

            return res;
        }

        public static AircraftPositionReport9 FromAIS(string ais)
        {
            byte[] unp = AISTransCoder.UnpackAISString(ais);
            return FromAIS(unp);
        }

        public override byte[] ToAisUnpacked()
        {
            int acc = Accuracy ? 1 : 0;

            byte[] unpackedBytes = new byte[TotalNumberOfBytes];
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 000, 006, (int)MessageID);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 006, 002, (int)Repeat);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 008, 030, (int)MMSI);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 038, 012, (int)Altitude);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 050, 010, (int)(SOG / 1.852 * 10));
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 060, 001, (int)acc);
            AISTransCoder.SetBitsAsSignedInt(unpackedBytes, 061, 028, (int)(Longitude * 600000));
            AISTransCoder.SetBitsAsSignedInt(unpackedBytes, 089, 027, (int)(Latitude * 600000));
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 116, 012, (int)(COG * 10));
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 128, 006, (int)TimeStamp);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 142, 001, DataTerminalReady ? 0 : 1);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 143, 005, (int)0); // Spare
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 147, 001, (int)0); // Spare
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 148, 001, (int)0); // RAIM
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 149, 019, (int)RadioStatus);
            return unpackedBytes;
        }

        public override string ToString()
        {
            return AISTransCoder.EnpackAISString(ToAisUnpacked());
        }

        public string ToAisPositionPacket()
        {
            string ln1 = "!AIVDM,1,1,,A," + this.ToString() + ",0";
            ln1 += "*" + AISTransCoder.Checksum(ln1);
            string res = ln1 + "\r\n";
            return res;
        }

        public byte[] ToAisPositionDataMsg()
        {
            return Encoding.ASCII.GetBytes(ToAisPositionPacket());
        }      
    }

    // TABLE 13 (p.40 Rec. ITU-R M.13171-1):
    //  * PACKET 10, TABLE 24 (p.54 Rec. ITU-R M.13171-1)
    /// <summary>
    ///     10 - UTC and date inquiry;
    /// </summary>
    public class UTCandDateInquiryMessage10 : AisSentense
    {
        private const ushort TotalNumberOfBits = 72;
        private static ushort TotalNumberOfBytes = TotalNumberOfBits / 8;

        public UTCandDateInquiryMessage10() { MessageID = 10; }

        // MessageID, 6 Bits (offset 0)
        // Repeat, 2 Bits (offset 6)
        // MMSI, 30 Bits (offset 8)

        /// <summary>
        ///     Maritime Mobile Service Identity, 30 Bits (offset 40)
        /// </summary>
        public uint DestMMSI;

        public static UTCandDateInquiryMessage10 FromAIS(byte[] unpackedBytes)
        {
            if (unpackedBytes == null) return null;
            if (unpackedBytes.Length < TotalNumberOfBytes) return null;
            int stype = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 0, 6);
            if (stype != 10) return null;

            UTCandDateInquiryMessage10 res = new UTCandDateInquiryMessage10();
            res.MessageID = (byte)stype;
            res.Repeat = (byte)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 6, 2);
            res.MMSI = (uint)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 8, 30);
            res.DestMMSI = (uint)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 40, 30);
            List<byte> bytes = new List<byte>();
            
            return res;
        }

        public static UTCandDateInquiryMessage10 FromAIS(string ais)
        {
            byte[] unp = AISTransCoder.UnpackAISString(ais);
            return FromAIS(unp);
        }

        public override byte[] ToAisUnpacked()
        {
            byte[] unpackedBytes = new byte[TotalNumberOfBytes];
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 000, 006, (int)MessageID);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 006, 002, (int)Repeat);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 008, 030, (int)MMSI);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 038, 002, (int)0); // Spare
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 040, 030, (int)DestMMSI);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 070, 002, (int)0); // Spare
            return unpackedBytes;
        }

        public override string ToString()
        {
            return AISTransCoder.EnpackAISString(ToAisUnpacked());
        }
    }

    // TABLE 13 (p.40 Rec. ITU-R M.13171-1):
    //  * PACKET 4, TABLE 16 (p.44 Rec. ITU-R M.13171-1)
    //  * PACKET 11, TABLE 16 (p.44 Rec. ITU-R M.13171-1)
    /// <summary>
    ///     4 - Base station report,
    ///     11 - UTC/date response;
    /// </summary>
    public class UTCAndDateResponseReport11 : BaseStationReport4
    {
        public UTCAndDateResponseReport11() { MessageID = 11; }
    }

    // TABLE 13 (p.40 Rec. ITU-R M.13171-1):
    //  * PACKET 12, TABLE 25 (p.54 Rec. ITU-R M.13171-1)
    /// <summary>
    /// <summary>
    ///     12 - Addressed safety related message;
    /// </summary>
    public class AddressedSafetyMessage12 : AisSentense
    {
        private const short MaxNumberOfBits = 1008;
        private static ushort MaxNumberOfBytes = MaxNumberOfBits / 8;

        public AddressedSafetyMessage12() { MessageID = 12; }

        // MessageID, 6 Bits (offset 0)
        // Repeat, 2 Bits (offset 6)
        // MMSI, 30 Bits (offset 8)

        /// <summary>
        ///     Sequence Number, 2 Bits (offset 38)
        /// </summary>
        public uint SeqNum = 0;
        /// <summary>
        ///     Maritime Mobile Service Identity, 30 Bits (offset 40)
        /// </summary>
        public uint DestMMSI;
        /// <summary>
        ///     Retransmit flag, 1 Bit (offset 70)
        /// </summary>
        public bool Retransmit = false;
        /// <summary>
        ///     Binary Data, Max 156 ANSII Symbols or 936 Bits (offset 72)
        /// </summary>
        public string Message = "";

        public static AddressedSafetyMessage12 FromAIS(byte[] unpackedBytes)
        {
            if (unpackedBytes == null) return null;
            if (unpackedBytes.Length > MaxNumberOfBytes) return null;
            int stype = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 0, 6);
            if (stype != 12) return null;

            AddressedSafetyMessage12 res = new AddressedSafetyMessage12();
            res.MessageID = (byte)stype;
            res.Repeat = (byte)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 6, 2);
            res.MMSI = (uint)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 8, 30);
            res.SeqNum = (uint)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 38, 2);
            res.DestMMSI = (uint)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 40, 30);
            res.Retransmit = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 70, 1) == 1;

            int msgl = (unpackedBytes.Length - 9) * 8;
            res.Message = AISTransCoder.GetAisString(unpackedBytes, 72, msgl).Trim();

            return res;
        }

        public static AddressedSafetyMessage12 FromAIS(string ais)
        {
            byte[] unp = AISTransCoder.UnpackAISString(ais);
            return FromAIS(unp);
        }

        public override byte[] ToAisUnpacked()
        {
            string msg = string.IsNullOrEmpty(Message) ? "" : Message.ToUpper();
            if(msg.Length > 156) msg = msg.Substring(0, 156);
            while(msg.Length * 6 % 8 > 0) msg += " ";
            int sln = msg.Length * 6;
            int dln = sln / 8;

            byte[] unpackedBytes = new byte[9 + dln];
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 000, 006, (int)MessageID);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 006, 002, (int)Repeat);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 008, 030, (int)MMSI);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 038, 002, (int)SeqNum);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 040, 030, (int)DestMMSI);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 070, 001, Retransmit ? 1 : 0);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 071, 001, 0); // Spare
            AISTransCoder.SetAisString(unpackedBytes,         072, sln, msg);
            return unpackedBytes;
        }

        public override string ToString()
        {
            return AISTransCoder.EnpackAISString(ToAisUnpacked());
        }
    }

    // NO MESSAGE Acknowledgement 7, 13

    // TABLE 13 (p.40 Rec. ITU-R M.13171-1):
    //  * PACKET 14, TABLE 26 (p.55 Rec. ITU-R M.13171-1)
    /// <summary>
    ///     14 - Safety Related Broadcast Message;
    /// </summary>
    public class SafetyRelatedBroadcastMessage14: AisSentense
    {
        public string Message = "PING";

        public SafetyRelatedBroadcastMessage14() { MessageID = 14; }

        public SafetyRelatedBroadcastMessage14(string Message) { MessageID = 14; this.Message = Message; }

        // MessageID, 6 Bits (offset 0)
        // Repeat, 2 Bits (offset 6)
        // MMSI, 30 Bits (offset 8)

        /// <summary>
        ///     Safety related text, max 967 Bits (offset 40)
        /// </summary>
        public string Text = "PING";

        public override byte[] ToAisUnpacked()
        {
            string sftv = string.IsNullOrEmpty(Text) ? "PING" : Text.ToUpper();
            if (sftv.Length >= 967) sftv = sftv.Substring(0, 967);
            int btl = sftv.Length * 6;

            byte[] unpackedBytes = new byte[5 + (int)(sftv.Length / 8.0 * 6.0 + 1)];
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 000, 006, (int)MessageID);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 006, 002, (int)Repeat);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 008, 030, (int)MMSI); //MMSI
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 038, 002, (int)0); // Spare
            AISTransCoder.SetAisString(unpackedBytes,         040, btl, sftv);
            return unpackedBytes;
        }

        public override string ToString()
        {
            return AISTransCoder.EnpackAISString(ToAisUnpacked());
        }

        public string ToPacketFrame()
        {
            string s = this.ToString();
            s = "!AIVDM,1,1,,A," + s + ",0";
            s += "*" + AISTransCoder.Checksum(s);
            return s;
        }
    }

    // NO MESSAGE Interrogation 15
    // NO MESSAGE Assigned mode command 16
    // NO MESSAGE GNSS broadcast binary message 17    

    // TABLE 13 (p.40 Rec. ITU-R M.13171-1):
    //  * PACKET 18, TABLE 31 (p.59 Rec. ITU-R M.13171-1)
    /// <summary>
    ///     18 - Standard Class B equipment position report
    /// </summary>
    public class StandardEquipPositionReport18 : AisSentense, AisISentense, AisIPositionReport
    {
        private const ushort TotalNumberOfBits = 168;
        private static ushort TotalNumberOfBytes = TotalNumberOfBits / 8;

        public StandardEquipPositionReport18() { MessageID = 18; }

        // MessageID, 6 Bits (offset 0)
        // Repeat, 2 Bits (offset 6)
        // MMSI, 30 Bits (offset 8)

        /// <summary>
        ///     Speed-over-Ground, kmph, 10 Bits (offset 46)
        /// </summary>
        public uint SOG = 0;
        /// <summary>
        ///     Position Accuracy, 1 Bits (offset 56)
        /// </summary>
        public bool Accuracy = false;
        /// <summary>
        ///     Longitude, degrees, 28 Bits (offset 57)
        /// </summary>
        public double Longitude = 0;
        /// <summary>
        ///     Latitude, degrees, 27 Bits (offset 85)
        /// </summary>
        public double Latitude = 0;
        /// <summary>
        ///     Course-Over-Ground, degrees, 12 Bits (offset 112)
        /// </summary>
        public double COG = 0;
        /// <summary>
        ///     True Heading, degrees, 9 Bits (offset 124)
        /// </summary>
        public ushort HDG = 0;
        /// <summary>
        ///      UTC timestamp, 6 Bits (offset 133)
        ///      0..60 - Not Available, 61 - Manual Mode, 62 - Dead Reckoning, 63 - Inoperative
        /// </summary>
        public uint TimeStamp = 60;
        /// <summary>
        ///     Radio status, 19 Bits (offset 149)
        /// </summary>
        public AisCommunicationStatus RadioStatus = AisCommunicationStatus.DirectUTC;

        public static StandardEquipPositionReport18 FromAIS(byte[] unpackedBytes)
        {
            if (unpackedBytes == null) return null;
            if (unpackedBytes.Length < TotalNumberOfBytes) return null;
            int stype = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 0, 6);
            if (stype != 18) return null;

            StandardEquipPositionReport18 res = new StandardEquipPositionReport18();
            res.MessageID = (byte)stype;
            res.Repeat = (byte)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 6, 2);
            res.MMSI = (uint)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 8, 30);
            res.SOG = (uint)(AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 46, 10) / 10 * 1.852);
            res.Accuracy = (byte)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 56, 1) == 1 ? true : false;
            res.Longitude = AISTransCoder.GetBitsAsSignedInt(unpackedBytes, 57, 28) / 600000.0;
            res.Latitude = AISTransCoder.GetBitsAsSignedInt(unpackedBytes, 85, 27) / 600000.0;
            res.COG = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 112, 12) / 10.0;
            res.HDG = (ushort)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 124, 9);
            res.TimeStamp = (uint)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 133, 6);
            res.RadioStatus = (AisCommunicationStatus)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 149, 19);
            return res;
        }

        public static StandardEquipPositionReport18 FromAIS(string ais)
        {
            byte[] unp = AISTransCoder.UnpackAISString(ais);
            return FromAIS(unp);
        }       
        public override byte[] ToAisUnpacked()
        {
            int acc = Accuracy ? 1 : 0;

            byte[] unpackedBytes = new byte[TotalNumberOfBytes];
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 000, 006, (int)MessageID); // type
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 006, 002, (int)Repeat);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 008, 030, (int)MMSI);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 038, 008, (int)0); // RESERVED
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 046, 010, (int)(SOG / 1.852 * 10)); // speed            
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 056, 001, (int)acc);
            AISTransCoder.SetBitsAsSignedInt(unpackedBytes,   057, 028, (int)(Longitude * 600000));
            AISTransCoder.SetBitsAsSignedInt(unpackedBytes,   085, 027, (int)(Latitude * 600000));
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 112, 012, (int)(COG * 10.0));
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 124, 009, HDG);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 133, 006, (int)TimeStamp);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 139, 004, (int)0); // RESERVERD
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 143, 004, (int)0); // SPARE
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 147, 001, (int)0); // RAIM
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 148, 001, (int)0); // Comm Flag
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 149, 019, (int)RadioStatus);
            return unpackedBytes;
        }

        public override string ToString()
        {
            return AISTransCoder.EnpackAISString(ToAisUnpacked());
        }
       
        public string ToAisPositionPacket()
        {
            string ln1 = "!AIVDM,1,1,,A," + this.ToString() + ",0";
            ln1 += "*" + AISTransCoder.Checksum(ln1);
            string res = ln1 + "\r\n";
            return res;
        }

        public byte[] ToAisPositionDataMsg()
        {
            return Encoding.ASCII.GetBytes(ToAisPositionPacket());
        }
    }

    // TABLE 13 (p.40 Rec. ITU-R M.13171-1):
    //  * PACKET 19, TABLE 32 (p.61 Rec. ITU-R M.13171-1)
    /// <summary>
    ///     19 - Extended Class B equipment position report;
    /// </summary>
    public class ExtendedEquipPositionReport19 : AisSentense, AisISentense, AisIPositionReport
    {
        private const ushort  TotalNumberOfBits  = 312;
        private static ushort TotalNumberOfBytes = TotalNumberOfBits / 8;

        public ExtendedEquipPositionReport19 () { MessageID = 19; }

        // MessageID, 6 Bits (offset 0)
        // Repeat, 2 Bits (offset 6)
        // MMSI, 30 Bits (offset 8)

        /// <summary>
        ///     Speed-over-Ground, kmph, 10 Bits (offset 46)
        /// </summary>
        public uint SOG = 0;
        /// <summary>
        ///     Position Accuracy, 1 Bits (offset 56)
        /// </summary>
        public bool Accuracy = false;
        /// <summary>
        ///     Longitude, degrees, 28 Bits (offset 57)
        /// </summary>
        public double Longitude = 0;
        /// <summary>
        ///     Latitude, degrees, 27 Bits (offset 85)
        /// </summary>
        public double Latitude = 0;
        /// <summary>
        ///     Course-Over-Ground, degrees, 12 Bits (offset 112)
        /// </summary>
        public double COG = 0;
        /// <summary>
        ///     True Heading, degrees, 9 Bits (offset 124)
        /// </summary>
        public ushort HDG = 0;
        /// <summary>
        ///      UTC timestamp, 6 Bits (offset 133)
        ///      0..60 - Not Available, 61 - Manual Mode, 62 - Dead Reckoning, 63 - Inoperative
        /// </summary>
        public uint TimeStamp = 60;
        /// <summary>
        ///     20-symbols Vessel, 120 Bits (offset 143)
        /// </summary>
        public string VesselName;
        /// <summary>
        ///     Type of Ship/Cargo, 8 Bits (offset 263)
        /// </summary>
        public AisShipType ShipType = AisShipType.Default;
        /// <summary>
        ///     Ship Dimentions A-B-C-D, 30 Bits (offset 271)
        ///     FIGURE 17 (p.48 Rec. ITU-R M.13171-1)
        /// </summary>
        public int[] Dimensions = new int[4];
        /// <summary>
        ///     GPS Position Fix, 4 Bits (offset 301)
        /// </summary>
        public AisPosFix PosFix = AisPosFix.Undefined;
        /// <summary>
        ///     Data Terminal Ready, 1 Bit (offset 306)
        /// </summary>
        public bool DataTerminalReady = false;

        public static ExtendedEquipPositionReport19 FromAIS(byte[] unpackedBytes)
        {
            if (unpackedBytes == null) return null;
            if (unpackedBytes.Length < TotalNumberOfBytes) return null;
            int stype = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 0, 6);
            if (stype != 19) return null;

            ExtendedEquipPositionReport19 res = new ExtendedEquipPositionReport19 ();
            res.MessageID = (byte)stype;
            res.Repeat = (byte)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 6, 2);
            res.MMSI = (uint)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 8, 30);
            res.SOG = (uint)(AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 46, 10) / 10 * 1.852);
            res.Accuracy = (byte)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 56, 1) == 1 ? true : false;
            res.Longitude = AISTransCoder.GetBitsAsSignedInt(unpackedBytes, 57, 28) / 600000.0;
            res.Latitude = AISTransCoder.GetBitsAsSignedInt(unpackedBytes, 85, 27) / 600000.0;
            res.COG = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 112, 12) / 10.0;
            res.HDG = (ushort)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 124, 9);
            res.TimeStamp = (uint)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 133, 6);
            res.VesselName = AISTransCoder.GetAisString(unpackedBytes, 143, 120);
            res.ShipType = (AisShipType)AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 263, 8);
            res.Dimensions[0] = (int)AISTransCoder.GetBitsAsSignedInt(unpackedBytes, 271, 9); // A
            res.Dimensions[1] = (int)AISTransCoder.GetBitsAsSignedInt(unpackedBytes, 280, 9); // B
            res.Dimensions[2] = (int)AISTransCoder.GetBitsAsSignedInt(unpackedBytes, 289, 6); // C
            res.Dimensions[3] = (int)AISTransCoder.GetBitsAsSignedInt(unpackedBytes, 295, 6); // D
            res.PosFix = (AisPosFix)AISTransCoder.GetBitsAsSignedInt(unpackedBytes, 301, 4);
            res.DataTerminalReady = AISTransCoder.GetBitsAsUnsignedInt(unpackedBytes, 306, 1) == 0;
            return res;
        }

        public static ExtendedEquipPositionReport19 FromAIS(string ais)
        {
            byte[] unp = AISTransCoder.UnpackAISString(ais);
            return FromAIS(unp);
        }

        public override byte[] ToAisUnpacked()
        {
            string vessel = string.IsNullOrEmpty(VesselName) ? "UNKNOWN" : VesselName.ToUpper();
            if (vessel.Length > 20) vessel = vessel.Substring(0, 20);

            byte[] unpackedBytes = new byte[TotalNumberOfBytes];
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 000, 006, (int)MessageID); // type
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 006, 002, (int)Repeat);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 008, 030, (int)MMSI);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 038, 008, (int)0); // RESERVED
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 046, 010, (int)(SOG / 1.852 * 10)); // speed            
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 056, 001, Accuracy ? 1 : 0);
            AISTransCoder.SetBitsAsSignedInt(unpackedBytes,   057, 028, (int)(Longitude * 600000));
            AISTransCoder.SetBitsAsSignedInt(unpackedBytes,   085, 027, (int)(Latitude * 600000));
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 112, 012, (int)(COG * 10.0));
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 124, 009, HDG);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 133, 006, (int)TimeStamp);
            AISTransCoder.SetAisString(unpackedBytes,         143, 120, VesselName);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 263, 008, (int)ShipType);
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 271, 009, Dimensions == null || Dimensions.Length != 4 ? (int)0 : (int)Dimensions[0]); //A DIMENTIONS
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 280, 009, Dimensions == null || Dimensions.Length != 4 ? (int)0 : (int)Dimensions[1]); //B DIMENTIONS
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 289, 006, Dimensions == null || Dimensions.Length != 4 ? (int)0 : (int)Dimensions[2]); //C DIMENTIONS
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 295, 006, Dimensions == null || Dimensions.Length != 4 ? (int)0 : (int)Dimensions[3]); //D DIMENTIONS
            AISTransCoder.SetBitsAsUnsignedInt(unpackedBytes, 301, 004, (int)PosFix);
            return unpackedBytes;
        }

        public override string ToString()
        {            
            return AISTransCoder.EnpackAISString(ToAisUnpacked());
        }

        public string ToAisPositionPacket()
        {
            string ln0 = "!AIVDM,1,1,,A," + this.ToString() + ",0";
            ln0 += "*" + AISTransCoder.Checksum(ln0);
            string res = ln0 + "\r\n";
            return res;
        }

        public byte[] ToAisPositionDataMsg()
        {
            return Encoding.ASCII.GetBytes(ToAisPositionPacket());
        }
    }

    // NO MESSAGE Data link management message 20
    // NO MESSAGE Aids-to-navigation report 21
    // NO MESSAGE Channel management 22
        
    /// <summary>
    ///     AIS Trans Coder
    /// </summary>
    public class AISTransCoder
    {       
        public static string Checksum(string sentence)
        {
            int iFrom = 0;
            if (sentence.IndexOf('$') == 0) iFrom++;
            if (sentence.IndexOf('!') == 0) iFrom++;
            int iTo = sentence.Length;
            if (sentence.LastIndexOf('*') == (sentence.Length - 3))
                iTo = sentence.IndexOf('*');
            int checksum = Convert.ToByte(sentence[iFrom]);
            for (int i = iFrom + 1; i < iTo; i++)
                checksum ^= Convert.ToByte(sentence[i]);
            return checksum.ToString("X2");
        }

        public static byte[] UnpackAISString(string s)
        {
            return UnpackAISBytes(Encoding.UTF8.GetBytes(s));
        }

        private static byte[] UnpackAISBytes(byte[] data)
        {
            int outputLen = ((data.Length * 6) + 7) / 8;
            byte[] result = new byte[outputLen];

            int iSrcByte = 0;
            byte nextByte = ToSixBit(data[iSrcByte]);
            for (int iDstByte = 0; iDstByte < outputLen; ++iDstByte)
            {
                byte currByte = nextByte;
                if (iSrcByte < data.Length - 1)
                    nextByte = ToSixBit(data[++iSrcByte]);
                else
                    nextByte = 0;

                switch (iDstByte % 3)
                {
                    case 0:
                        result[iDstByte] = (byte)((currByte << 2) | (nextByte >> 4));
                        break;
                    case 1:
                        result[iDstByte] = (byte)((currByte << 4) | (nextByte >> 2));
                        break;
                    case 2:
                        result[iDstByte] = (byte)((currByte << 6) | (nextByte));
                        
                        if (iSrcByte < data.Length - 1)
                            nextByte = ToSixBit(data[++iSrcByte]);
                        else
                            nextByte = 0;
                        break;
                }
            }

            return result;
        }

        public static string EnpackAISString(byte[] ba)
        {
            return Encoding.UTF8.GetString(EnpackAISBytes(ba));
        }

        public static byte[] EnpackAISBytes(byte[] ba)
        {
            List<byte> res = new List<byte>();
            for (int i = 0; i < ba.Length; i++)
            {
                int val = 0;
                int val2 = 0;
                switch (i % 3)
                {
                    case 0:
                        val = (byte)((ba[i] >> 2) & 0x3F);
                        break;
                    case 1:
                        val = (byte)((ba[i - 1] & 0x03) << 4) | (byte)((ba[i] & 0xF0) >> 4);
                        break;
                    case 2:
                        val = (byte)((ba[i - 1] & 0x0F) << 2) | (byte)((ba[i] & 0xC0) >> 6);
                        val2 = (byte)((ba[i] & 0x3F)) + 48;
                        if (val2 > 87) val2 += 8;
                        break;
                };
                val += 48;
                if (val > 87) val += 8;
                res.Add((byte)val);
                if ((i % 3) == 2) res.Add((byte)val2);
            };
            return res.ToArray();
        }

        public static byte ToSixBit(byte b)
        {
            byte res = (byte)(b - 48);
            if (res > 39) res -= 8;
            return res;
        }

        public static string GetAisString(byte[] source, int start, int len)
        {
            string key = "@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_ !\"#$%&'()*+,-./0123456789:;<=>?";
            int l = key.Length;
            string val = "";
            for (int i = 0; i < len; i += 6)
            {
                byte c = (byte)(GetBitsAsSignedInt(source, start + i, 6) & 0x3F);
                val += key[c];
            };
            return val.Trim();
        }

        public static void SetAisString(byte[] source, int start, int len, string val)
        {
            if (val == null) val = "";
            string key = "@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_ !\"#$%&'()*+,-./0123456789:;<=>?;";
            int strlen = len / 6;
            if (val.Length > strlen) val = val.Substring(0, strlen);
            while (val.Length < strlen) val += " ";
            int s = 0;
            for (int i = 0; i < len; i += 6, s++)
            {
                int cc = key.IndexOf(val[s]);
                if (cc < 0) cc = key.IndexOf("?");
                byte c = (byte)cc;
                SetBitsAsSignedInt(source, start + i, 6, c);
            };
        }

        public static int GetBitsAsSignedInt(byte[] source, int start, int len)
        {
            int value = GetBitsAsUnsignedInt(source, start, len);
            if ((value & (1 << (len - 1))) != 0)
            {
                // perform 32 bit sign extension
                for (int i = len; i < 32; ++i)
                {
                    value |= (1 << i);
                }
            };
            return value;
        }

        public static void SetBitsAsSignedInt(byte[] source, int start, int len, int val)
        {
            int value = val;
            if (value < 0)
            {
                value = ~value;
                for (int i = len; i < 32; ++i)
                {
                    value |= (1 << i);
                };
            }
            SetBitsAsUnsignedInt(source, start, len, val);
        }

        public static int GetBitsAsUnsignedInt(byte[] source, int start, int len)
        {
            int result = 0;

            for (int i = start; i < (start + len); ++i)
            {
                int iByte = i / 8;
                int iBit = 7 - (i % 8);
                result = result << 1 | (((source[iByte] & (1 << iBit)) != 0) ? 1 : 0);
            };

            return result;
        }

        public static void SetBitsAsUnsignedInt(byte[] source, int start, int len, int val)
        {
            int bit = len - 1;
            for (int i = start; i < (start + len); ++i, --bit)
            {
                int iByte = i / 8;
                int iBit = 7 - (i % 8);
                byte mask = (byte)(0xFF - (byte)(1 << iBit));
                byte b = (byte)(((val >> bit) & 0x01) << iBit);
                source[iByte] = (byte)((source[iByte] & mask) | b);
            }
        }

        public static int Hash(string name)
        {
            string upname = name == null ? "" : name;
            int stophere = upname.IndexOf("-");
            if (stophere > 0) upname = upname.Substring(0, stophere);
            while (upname.Length < 9) upname += " ";

            int hash = 0x2017;
            int i = 0;
            while (i < 9)
            {
                hash ^= (int)(upname.Substring(i, 1))[0] << 16;
                hash ^= (int)(upname.Substring(i + 1, 1))[0] << 8;
                hash ^= (int)(upname.Substring(i + 2, 1))[0];
                i += 3;
            };
            return hash & 0x7FFFFF;
        }

        public static uint MMSI(string name)
        {
            string upname = name == null ? "" : name;
            while (upname.Length < 9) upname += " ";
            int hash = 2017;
            int i = 0;
            while (i < 9)
            {
                hash ^= (int)(upname.Substring(i, 1))[0] << 16;
                hash ^= (int)(upname.Substring(i + 1, 1))[0] << 8;
                hash ^= (int)(upname.Substring(i + 2, 1))[0];
                i += 3;
            };
            return (uint)(hash & 0xFFFFFF);
        }
    }
}
