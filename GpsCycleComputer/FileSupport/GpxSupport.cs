﻿using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using GpsCycleComputer;
using GpsUtils;
using System.Xml;

namespace GpsSample.FileSupport
{
    class GpxSupport : IFileSupport
    {
        CultureInfo IC = CultureInfo.InvariantCulture;
#if false
        public bool Load(string filename, ref Form1.WayPointInfo WayPoints,
            int vector_size, ref float[] dataLat, ref float[] dataLong, ref int[] dataT, out int data_size)
        {
            int Counter = 0;
            bool Status = false;
            DateTime StartTime = DateTime.Now;

            data_size = 0;
            WayPoints.WayPointCount = 0;

            // set "." as decimal separator for reading the map info
            NumberFormatInfo number_info = new NumberFormatInfo();
            number_info.NumberDecimalSeparator = ".";

            Cursor.Current = Cursors.WaitCursor;
            try
            {
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                string line = "";
                double last_lat = 0.0;
                double last_long = 0.0;
                string name = "";

                while ((line = sr.ReadLine()) != null)      //null: EndOfStream
                {
                    line = line.Trim();

                    // skip blank lines
                    if (line == "") { continue; }

                    if (line.IndexOf("<trkpt ") >= 0)
                    {
                        last_lat = 0.0;
                        last_long = 0.0;
                    }

                    // line contains coordinates - extract
                    if ((line.IndexOf("lat=\"") >= 0) || (line.IndexOf("lon=\"") >= 0))
                    {
                        string[] words = line.Split(new Char[] { ' ' });
                        for (int i = 0; i < words.Length; i++)
                        {
                            if (words[i].IndexOf("lon=\"") >= 0)
                            {
                                words[i] = words[i].Replace("></trkpt>", "");
                                words[i] = words[i].Replace("/>", "");
                                words[i] = words[i].Replace("lon=\"", "");
                                words[i] = words[i].Replace("\"", "");
                                words[i] = words[i].Replace(">", "");
                                last_long = Convert.ToDouble(words[i].Trim(), number_info);
                            }
                            else if (words[i].IndexOf("lat=\"") >= 0)
                            {
                                words[i] = words[i].Replace("></trkpt>", "");
                                words[i] = words[i].Replace("/>", "");
                                words[i] = words[i].Replace("lat=\"", "");
                                words[i] = words[i].Replace("\"", "");
                                words[i] = words[i].Replace(">", "");
                                last_lat = Convert.ToDouble(words[i].Trim(), number_info);
                            }
                        }
                    }

                // fix if file contains coordinates only (i.e. no time info), but on a separate lines
                /*    else if ((line.IndexOf("</trkpt>") == 0) && (last_lat != 0.0) && (last_long != 0.0))
                    {
                        // check if we need to decimate arrays
                        if (Counter >= vector_size)
                        {
                            for (int i = 0; i < vector_size / 2; i++)
                            {
                                dataLat[i] = dataLat[i * 2];
                                dataLong[i] = dataLong[i * 2];
                                dataT[i] = dataT[i * 2];
                            }
                            Counter /= 2;
                        }

                        dataLat[Counter] = (float)last_lat;
                        dataLong[Counter] = (float)last_long;
                        dataT[Counter] = 0; // no time in this file

                        Counter++;

                        last_lat = 0.0; last_long = 0.0;
                    }*/
                    // time follows lat/long, so make sure these are loaded
                    if ((line.IndexOf("<time>") == 0) && (line.IndexOf("</time>") >= 0) && (line.Length == 33)
                             && (last_lat != 0.0) && (last_long != 0.0))
                    {
                        line = line.Replace("<time>", "");
                        line = line.Replace("</time>", "");
                        line = line.Trim();

                        // read time
                        DateTime tm = new DateTime(Convert.ToInt16(line.Substring(0, 4)),
                                                   Convert.ToInt16(line.Substring(5, 2)),
                                                   Convert.ToInt16(line.Substring(8, 2)),
                                                   Convert.ToInt16(line.Substring(11, 2)),
                                                   Convert.ToInt16(line.Substring(14, 2)),
                                                   Convert.ToInt16(line.Substring(17, 2)));

                        if (Counter == 0) // the first point loaded
                        {
                            StartTime = tm;
                        }

                        TimeSpan run_time = tm - StartTime;
                        double double_total_sec = run_time.TotalSeconds; if (double_total_sec < 0.0) { double_total_sec = 0.0; }
                        dataT[Counter] = (Int32)double_total_sec;
                    }

                    // fix if file contains coordinates only (i.e. no time info), all on one line
                    if (((line.IndexOf("</trkpt>") >= 0) || (line.IndexOf("/>") >= 0)) && (last_lat != 0.0) && (last_long != 0.0))
                    {
                        // check if we need to decimate arrays
                        if (Counter >= vector_size)
                        {
                            for (int i = 0; i < vector_size / 2; i++)
                            {
                                dataLat[i] = dataLat[i * 2];
                                dataLong[i] = dataLong[i * 2];
                                dataT[i] = dataT[i * 2];
                            }
                            Counter /= 2;
                        }

                        dataLat[Counter] = (float)last_lat;
                        dataLong[Counter] = (float)last_long;
                        dataT[Counter] = 0; // no time in this file

                        Counter++;

                        last_lat = 0.0; last_long = 0.0;
                    }


                    // read names of Waypoints
                    else if ((line.IndexOf("<name>") >= 0))
                    {
                        name = line.Replace("<name>", "");
                        name = name.Replace("</name>", "");
                    }
                    // read names of Waypoints
                    else if ((line.IndexOf("</wpt>") >= 0) && (last_lat != 0.0) && (last_long != 0.0) && (name.Length > 0))
                    {
                        if (WayPoints.WayPointCount < WayPoints.WayPointDataSize - 1)
                        {
                            WayPoints.lat[WayPoints.WayPointCount] = (float)last_lat;
                            WayPoints.lon[WayPoints.WayPointCount] = (float)last_long;
                            WayPoints.name[WayPoints.WayPointCount] = name;
                            WayPoints.WayPointCount++;
                            name = "";
                        }
                        last_lat = 0.0;
                        last_long = 0.0;
                    }
                }
                sr.Close();
                fs.Close();

                data_size = Counter;
                Status = true;
            }
            catch (Exception e)
            {
                Utils.log.Error(" LoadGpx ", e);
            }
            Cursor.Current = Cursors.Default;
            return Status;
        }
#endif
#if tru
        public bool Load(string filename, ref Form1.WayPointInfo WayPoints,
            int vector_size, ref float[] dataLat, ref float[] dataLong, ref int[] dataT, out int data_size)
        {
            int Counter = 0;
            bool Status = false;
            DateTime StartTime = DateTime.Now;

            data_size = 0;
            WayPoints.WayPointCount = 0;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = true;
                XmlReader reader = XmlReader.Create(filename, settings);
                
                reader.MoveToContent();
                while (reader.Read())
                {
                    //Console.WriteLine("<<" + reader.NodeType + "   " + reader.Name + "   " + reader.Value);
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.Name)
                        {
                            case "trkpt":
                                if (Counter >= vector_size)     // check if we need to decimate arrays
                                {
                                    for (int i = 0; i < vector_size / 2; i++)
                                    {
                                        dataLat[i] = dataLat[i * 2];
                                        dataLong[i] = dataLong[i * 2];
                                        dataT[i] = dataT[i * 2];
                                    }
                                    Counter = vector_size / 2;
                                }
                                dataLat[Counter] = (float)Convert.ToDouble(reader.GetAttribute("lat"), IC);
                                dataLong[Counter] = (float)Convert.ToDouble(reader.GetAttribute("lon"), IC);
                                //Console.WriteLine("KB:lat" + reader.GetAttribute("lat") + "lon" + reader.GetAttribute("lon"));
                                while (reader.Read())       //read subtree
                                {
                                    //Console.WriteLine("<<<<" + reader.NodeType + "   " + reader.Name);

                                    if (reader.NodeType == XmlNodeType.Element)
                                    {
                                        switch (reader.Name)
                                        {
                                            case "ele":
                                                reader.Read();
                                                Console.WriteLine("elevation=" + reader.Value);
                                                //Console.WriteLine("elvation=" + reader.ReadString());
                                                break;
                                            case "time":
                                                reader.Read();
                                                Console.WriteLine("time=" + Convert.ToDateTime(reader.Value, IC));
                                                //reader.ReadEndElement();
                                                break;
                                            case "speed":
                                                reader.Read();
                                                Console.WriteLine("speed=" + Convert.ToDouble(reader.Value, IC).ToString());
                                                //reader.ReadEndElement();
                                                break;
                                        }
                                        //reader.Read();
                                        //if (reader.NodeType != XmlNodeType.EndElement)
                                        //    Console.WriteLine("fehlendes EndTag");
                                    }
                                    else if (reader.Name == "trkpt" && reader.NodeType == XmlNodeType.EndElement)
                                    {
                                        Counter++;
                                        break;
                                    }
                                }
                                break;


                            case "wpt":
                                WayPoints.lat[WayPoints.WayPointCount] = (float)Convert.ToDouble(reader.GetAttribute("lat"), IC);
                                WayPoints.lon[WayPoints.WayPointCount] = (float)Convert.ToDouble(reader.GetAttribute("lon"), IC);
                                while (reader.Read())       //read subtree
                                {
                                    if (reader.NodeType == XmlNodeType.Element)
                                    {
                                        switch (reader.Name)
                                        {
                                            case "name":
                                                reader.Read();
                                                WayPoints.name[WayPoints.WayPointCount] = reader.Value;
                                                break;
                                        }
                                    }
                                    else if (reader.Name == "wpt" && reader.NodeType == XmlNodeType.EndElement)
                                    {
                                        WayPoints.WayPointCount++;
                                        break;
                                    }
                                }
                                break;

                        }
                    }
                }
                reader.Close();
                data_size = Counter;
                Status = true;
            }
            catch (Exception e)
            {
                Utils.log.Error(" LoadGpx ", e);
            }
            Cursor.Current = Cursors.Default;
            return Status;
        }

#else
        public bool Load(string filename, ref Form1.WayPointInfo WayPoints,
            int vector_size, ref float[] dataLat, ref float[] dataLong, ref int[] dataT, out int data_size)
        {
            bool Status = false;
            DateTime StartTime = DateTime.MinValue;

            data_size = 0;
            WayPoints.WayPointCount = 0;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                //XmlParserContext xmlpc = new XmlParserContext(null, null, "", XmlSpace.Default, System.Text.Encoding.UTF8);    does not work
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = true;
                StreamReader sr = new StreamReader(filename, System.Text.Encoding.UTF8);        //use StreamReader to overwrite encoding ISO-8859-1, which is not supported by .NETCF (no speed drawback)
                XmlReader reader = XmlReader.Create(sr, settings);
                //reader.MoveToContent();
                reader.ReadToFollowing("gpx");
                reader.Read();
                while (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "trk")
                    {
                        reader.Read();
                        while (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name == "trkseg")
                            {
                                reader.Read();
                                while (reader.NodeType == XmlNodeType.Element)
                                {
                                    if (reader.Name == "trkpt")
                                    {
                                        if (data_size >= vector_size)     // check if we need to decimate arrays
                                        {
                                            for (int i = 0; i < vector_size / 2; i++)
                                            {
                                                dataLat[i] = dataLat[i * 2];
                                                dataLong[i] = dataLong[i * 2];
                                                dataT[i] = dataT[i * 2];
                                            }
                                            data_size = vector_size / 2;
                                        }
                                        dataLat[data_size] = (float)Convert.ToDouble(reader.GetAttribute("lat"), IC);
                                        dataLong[data_size] = (float)Convert.ToDouble(reader.GetAttribute("lon"), IC);
                                        dataT[data_size] = 0;           //clear data in case there is no <time> field
                                        reader.Read();
                                        while (reader.NodeType == XmlNodeType.Element)       //read subtree
                                        {
                                            switch (reader.Name)
                                            {
                                                case "ele":
                                                    reader.Skip();
                                                    break;
                                                case "time":
                                                    if (StartTime == DateTime.MinValue)
                                                    {
                                                        StartTime = reader.ReadElementContentAsDateTime();
                                                    }
                                                    else
                                                    {
                                                        TimeSpan tspan = reader.ReadElementContentAsDateTime() - StartTime;
                                                        dataT[data_size] = (int)tspan.TotalSeconds;
                                                    }
                                                    break;
                                                case "speed":
                                                    reader.Skip();
                                                    break;
                                                default:
                                                    reader.Skip();
                                                    break;
                                            }
                                        }
                                        reader.ReadEndElement();
                                        data_size++;
                                    }
                                    else
                                        reader.Skip();
                                }
                                reader.ReadEndElement();
                            }
                            else if (reader.Name == "name")
                            {
                                string trkname = reader.ReadElementString();       //prepared for later use
                            }
                            else
                                reader.Skip();
                        }
                        reader.ReadEndElement();
                    }
                    else if (reader.Name == "wpt")
                    {
                        WayPoints.lat[WayPoints.WayPointCount] = (float)Convert.ToDouble(reader.GetAttribute("lat"), IC);
                        WayPoints.lon[WayPoints.WayPointCount] = (float)Convert.ToDouble(reader.GetAttribute("lon"), IC);
                        reader.Read();
                        while (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name == "name")
                            {
                                WayPoints.name[WayPoints.WayPointCount] = reader.ReadElementString();
                                WayPoints.WayPointCount++;
                            }
                            else if (reader.Name == "desc")
                            {
                                string wp_desc = reader.ReadElementString();        //prepared for later use
                            }
                            else
                                reader.Skip();
                        }
                        reader.ReadEndElement();
                    }
                    else
                        reader.Skip();
                }
                //reader.ReadEndElement();
                reader.Close();
                Status = true;
            }
            catch (Exception e)
            {
                Utils.log.Error(" LoadGpx ", e);
            }
            Cursor.Current = Cursors.Default;
            return Status;
        }

#endif



        public void Write(String gpx_file, int CheckPointCount,
            Form1.CheckPointInfo[] CheckPoints,
            CheckBox checkGpxRte, CheckBox checkGpxSpeedMs, CheckBox checkGPXtrkseg,
            float[] PlotLat, float[] PlotLong, int PlotCount,
            short[] PlotS, int[] PlotT, short[] PlotZ, short[] PlotH, DateTime StartTime,
            NumericUpDown numericGpxTimeShift,
            string dist_unit, string speed_unit, string alt_unit, string exstop_info,
                string dist, string speed_cur, string speed_avg, string speed_max, string run_time_label, string last_sample_time, string altitude, string battery
            )
        {
            FileStream fs = null;
            StreamWriter wr = null;
            try
            {
                fs = new FileStream(gpx_file, FileMode.Create);
                wr = new StreamWriter(fs);

                // write GPX header
                wr.WriteLine("<?xml version=\"1.0\"?>");
                wr.WriteLine("<gpx version=\"1.1\"");
                wr.WriteLine(" creator=\"GPSCycleComputer\"");
                wr.WriteLine(" xsi:schemaLocation=\"http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/1/1/gpx.xsd http://www.garmin.com/xmlschemas/TrackPointExtension/v1 http://www.garmin.com/xmlschemas/TrackPointExtensionv1.xsd\"");
                wr.WriteLine(" xmlns=\"http://www.topografix.com/GPX/1/1\"");
                wr.WriteLine(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"");
                wr.WriteLine(" xmlns:gpxtpx=\"http://www.garmin.com/xmlschemas/TrackPointExtension/v1\">");
                /*
                wr.WriteLine("<?xml version=\"1.0\"?>");
                wr.WriteLine("<gpx");
                wr.WriteLine("version=\"1.0\"");
                wr.WriteLine(" creator=\"GPSCycleComputer\"");
                wr.WriteLine(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"");
                wr.WriteLine(" xmlns=\"http://www.topografix.com/GPX/1/0\"");
                wr.WriteLine(" xsi:schemaLocation=\"http://www.topografix.com/GPX/1/0 http://www.topografix.com/GPX/1/0/gpx.xsd\">");
                wr.WriteLine();
                 */
                if (CheckPointCount != 0)
                {
                    for (int chk = 0; chk < CheckPointCount; chk++)
                    {
                        // need to replace chars not supported by XML
                        string chk_name = CheckPoints[chk].name.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                        string chk_link = null;
                        int link_idx = chk_name.IndexOf('\x02');
                        if (link_idx != -1)                     //audio file is present
                        {
                            chk_link = chk_name.Substring(link_idx + 1);
                            if (link_idx == 0)
                                chk_name = chk_name.Remove(0, 1);
                            else
                                chk_name = chk_name.Remove(link_idx, chk_name.Length - link_idx);
                        }

                        // GPS Track Analyser expects a newline between wpt and name
                        wr.WriteLine("<wpt lat=\"" + CheckPoints[chk].lat.ToString("0.##########", IC)
                                    + "\" lon=\"" + CheckPoints[chk].lon.ToString("0.##########", IC)
                                    + "\" >");
                        wr.WriteLine("<name>" + chk_name + "</name>");
                        if (link_idx != -1)
                            wr.WriteLine("<link href=\"" + chk_link + "\" />");
                        wr.WriteLine("</wpt>");
                    }
                }

                wr.WriteLine(checkGpxRte.Checked ? "<rte>" : "<trk>");
                wr.WriteLine("<name>" + StartTime + "</name>");
                // GPS Track Analyser expects a newline between desc and Data
                wr.WriteLine("<desc>");
                wr.WriteLine("<![CDATA[" + dist + " " + dist_unit + " " + run_time_label + " " + exstop_info
                               + " " + speed_cur + " " + speed_avg + " " + speed_max + " " + speed_unit
                               + " battery " + battery
                               + "]]>");
                wr.WriteLine("</desc>");

                if (checkGpxRte.Checked == false) { wr.WriteLine("<trkseg>"); }

                int lastTime = 0;
                // here write coordinates
                for (int i = 0; i < PlotCount; i++)
                {
                    if (checkGpxRte.Checked)
                    {
                        wr.WriteLine("<rtept lat=\"" + PlotLat[i].ToString("0.##########", IC) +
                                     "\" lon=\"" + PlotLong[i].ToString("0.##########", IC) + "\">");
                    }
                    else
                    {
                        if (checkGPXtrkseg.Checked && PlotT[i] > lastTime + 10)     //if gap > 10s, separate track in different segments
                        {
                            wr.WriteLine("</trkseg>");
                            wr.WriteLine("<trkseg>");
                        }
                        lastTime = PlotT[i];
                        wr.WriteLine("<trkpt lat=\"" + PlotLat[i].ToString("0.##########", IC) +
                                     "\" lon=\"" + PlotLong[i].ToString("0.##########", IC) + "\">");
                    }
                    if (PlotZ[i] != Int16.MinValue)     //ignore invalid value
                    {
                        wr.WriteLine("<ele>" + PlotZ[i] + "</ele>");
                    }
                    TimeSpan run_time = new TimeSpan(Decimal.ToInt32(numericGpxTimeShift.Value), 0, PlotT[i]);
                    string utcTime_str = (StartTime.ToUniversalTime() + run_time).ToString("s") + "Z";
                    wr.WriteLine("<time>" + utcTime_str + "</time>");

                    /*if (PlotS[i] != Int16.MinValue)     //ignore invalid value        //speed is invalid in gpx v1.1
                    {
                        if (checkGpxSpeedMs.Checked) // speed in m/s instead of km/h
                        {
                            wr.WriteLine("<speed>" + (PlotS[i] * (0.1 / 3.6)).ToString("0.##########", IC) + "</speed>");
                        }
                        else
                        {
                            wr.WriteLine("<speed>" + (PlotS[i] * 0.1).ToString("0.##########", IC) + "</speed>");
                        }
                    }*/
                    if (PlotH[i] != 0)              //heart rate
                    {
                        wr.WriteLine("<extensions><gpxtpx:TrackPointExtension><gpxtpx:hr>{0}</gpxtpx:hr></gpxtpx:TrackPointExtension></extensions>", PlotH[i]);
                    }
                    wr.WriteLine(checkGpxRte.Checked ? "</rtept>" : "</trkpt>");
                }
                // write end of the GPX file
                if (checkGpxRte.Checked == false) { wr.WriteLine("</trkseg>"); }
                if (checkGpxRte.Checked) { wr.WriteLine("</rte>"); } else { wr.WriteLine("</trk>"); }

                wr.WriteLine("</gpx>");
            }
            catch (Exception ee)
            {
                Utils.log.Error(" buttonSaveGPX_Click ", ee);
            }
            finally
            {
                if (wr != null) wr.Close();
                if (fs != null) fs.Close();
            }
        }
    }
}