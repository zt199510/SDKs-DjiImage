﻿using System.Text.RegularExpressions;

namespace SDKs.DjiImage
{
    static class Rdf
    {
        static readonly string rdf_Description = "<rdf:Description";
        static readonly string drone_dji_Version = "drone-dji:Version=[\"][\\d]+[.\\d]*[\"]";
        static readonly string drone_dji_GpsStatus = "drone-dji:GpsStatus=[\"][\\w]+[\"]";
        static readonly string drone_dji_GpsLatitude = "drone-dji:GpsLatitude=[\"][-+]{0,1}\\d+[.\\d+]*[\"]";
        static readonly string drone_dji_GpsLongitude = "drone-dji:GpsLongitude=[\"][-+]{0,1}\\d+[.\\d+]*[\"]";
        static readonly string drone_dji_GpsLongtitude = "drone-dji:GpsLongtitude=[\"][-+]{0,1}\\d+[.\\d+]*[\"]";
        static readonly string drone_dji_AbsoluteAltitude = "drone-dji:AbsoluteAltitude=[\"][-+]{0,1}\\d+[.\\d+]*[\"]";
        static readonly string drone_dji_RelativeAltitude = "drone-dji:RelativeAltitude=[\"][-+]{0,1}\\d+[.\\d+]*[\"]";
        static readonly string drone_dji_GimbalRollDegree = "drone-dji:GimbalRollDegree=[\"][-+]{0,1}\\d+[.\\d+]*[\"]";
        static readonly string drone_dji_GimbalYawDegree = "drone-dji:GimbalYawDegree=[\"][-+]{0,1}\\d+[.\\d+]*[\"]";
        static readonly string drone_dji_GimbalPitchDegree = "drone-dji:GimbalPitchDegree=[\"][-+]{0,1}\\d+[.\\d+]*[\"]";
        static readonly string drone_dji_FlightRollDegree = "drone-dji:FlightRollDegree=[\"][-+]{0,1}\\d+[.\\d+]*[\"]";
        static readonly string drone_dji_FlightYawDegree = "drone-dji:FlightYawDegree=[\"][-+]{0,1}\\d+[.\\d+]*[\"]";
        static readonly string drone_dji_FlightPitchDegree = "drone-dji:FlightPitchDegree=[\"][-+]{0,1}\\d+[.\\d+]*[\"]";
        static readonly string drone_dji_RtkFlag = "drone-dji:RtkFlag=[\"]\\d+[\"]";

        static string GetStr(Regex regex, System.Text.Encoding encoding, byte[] bytes, int sIndex, int ncount)
        {
            string str = encoding.GetString(bytes, sIndex, ncount);
            var match = regex.Match(str);
            int tmpIndex = match.Index;
            if (match.Success)
            {
                if (tmpIndex + 0xa00 < ncount)
                    return str.Substring(tmpIndex, 0xa00);

                return encoding.GetString(bytes, sIndex + tmpIndex, ncount >= 0xa00 ? 0xa00 : ncount);
            }

            int nextIndex = sIndex + ncount - 0x20;
            int nextcount = bytes.Length - nextIndex;
            if (nextcount <= 0x100)
                return string.Empty;

            if (nextcount > ncount)
                nextcount = ncount;

            return GetStr(regex, encoding, bytes, nextIndex, nextcount);
        }
        static string GetStr(Regex regex, System.Text.Encoding encoding, Stream stream, int sIndex, int ncount)
        {
            stream.Position = sIndex;

            byte[] bytes = new byte[ncount];
            stream.Read(bytes, 0, ncount);
            string str = encoding.GetString(bytes);
            var match = regex.Match(str);
            int tmpIndex = match.Index;
            if (match.Success)
            {
                int leftcount = ncount - tmpIndex;
                if (leftcount >= 0xa00)
                    return str.Substring(tmpIndex, 0xa00);

                int nc = (int)stream.Length - sIndex - tmpIndex;
                if (nc >= 0xa00)
                    return GetStr(regex, encoding, stream, sIndex + tmpIndex, 0xa00);

                return GetStr(regex, encoding, stream, sIndex + tmpIndex, nc);
            }

            int nextIndex = sIndex + ncount - 0x20;
            int nextcount = (int)stream.Length - nextIndex;
            if (nextcount <= 0x100)
                return string.Empty;

            if (nextcount > ncount)
                nextcount = ncount;

            return GetStr(regex, encoding, stream, nextIndex, nextcount);
        }
        internal static RdfDroneDji GetDroneDji(Stream stream)
        {
            var encoding = System.Text.Encoding.ASCII;
            int len = (int)stream.Length;
            string text = GetStr(new Regex(rdf_Description, RegexOptions.Multiline), encoding, stream, 0, len > 0x2800 ? 0x2800 : len);
            if (string.IsNullOrEmpty(text))
                return RdfDroneDji.Empty;

            return GetDroneDji(text);
        }
        internal static RdfDroneDji GetDroneDji(byte[] bytes)
        {
            var encoding = System.Text.Encoding.ASCII;
            string text = GetStr(new Regex(rdf_Description, RegexOptions.Multiline), encoding, bytes, 0, bytes.Length > 0x2800 ? 0x2800 : bytes.Length);
            if (string.IsNullOrEmpty(text))
                return RdfDroneDji.Empty;

            return GetDroneDji(text);
        }
        internal static RdfDroneDji GetDroneDji(string text)
        {
            var meta = new RdfDroneDji();
            var mc = Regex.Match(text, drone_dji_Version);
            if (mc.Success)
                meta.Version = mc.Value.Split('=')[1].Trim('"');

            mc = Regex.Match(text, drone_dji_GpsStatus);
            if (mc.Success)
                meta.GpsStatus = mc.Value.Split('=')[1].Trim('"');

            mc = Regex.Match(text, drone_dji_GpsLatitude);
            if (mc.Success)
                meta.GpsLatitude = decimal.Parse(mc.Value.Split('=')[1].Trim('"').Trim('+'));

            mc = Regex.Match(text, drone_dji_GpsLongitude);
            if (mc.Success)
                meta.GpsLongitude = decimal.Parse(mc.Value.Split('=')[1].Trim('"').Trim('+'));

            mc = Regex.Match(text, drone_dji_GpsLongtitude);
            if (mc.Success)
                meta.GpsLongitude = decimal.Parse(mc.Value.Split('=')[1].Trim('"').Trim('+'));

            mc = Regex.Match(text, drone_dji_AbsoluteAltitude);
            if (mc.Success)
                meta.AbsoluteAltitude = decimal.Parse(mc.Value.Split('=')[1].Trim('"').Trim('+'));

            mc = Regex.Match(text, drone_dji_RelativeAltitude);
            if (mc.Success)
                meta.RelativeAltitude = decimal.Parse(mc.Value.Split('=')[1].Trim('"').Trim('+'));

            mc = Regex.Match(text, drone_dji_GimbalRollDegree);
            if (mc.Success)
                meta.GimbalRollDegree = decimal.Parse(mc.Value.Split('=')[1].Trim('"').Trim('+'));

            mc = Regex.Match(text, drone_dji_GimbalYawDegree);
            if (mc.Success)
                meta.GimbalYawDegree = decimal.Parse(mc.Value.Split('=')[1].Trim('"').Trim('+'));

            mc = Regex.Match(text, drone_dji_GimbalPitchDegree);
            if (mc.Success)
                meta.GimbalPitchDegree = decimal.Parse(mc.Value.Split('=')[1].Trim('"').Trim('+'));

            mc = Regex.Match(text, drone_dji_FlightRollDegree);
            if (mc.Success)
                meta.FlightRollDegree = decimal.Parse(mc.Value.Split('=')[1].Trim('"').Trim('+'));
            mc = Regex.Match(text, drone_dji_FlightYawDegree);

            if (mc.Success)
                meta.FlightYawDegree = decimal.Parse(mc.Value.Split('=')[1].Trim('"').Trim('+'));

            mc = Regex.Match(text, drone_dji_FlightPitchDegree);
            if (mc.Success)
                meta.FlightPitchDegree = decimal.Parse(mc.Value.Split('=')[1].Trim('"').Trim('+'));

            mc = Regex.Match(text, drone_dji_RtkFlag);
            if (mc.Success)
                meta.RtkFlag = int.Parse(mc.Value.Split('=')[1].Trim('"'));

            return meta;
        }
    }
}