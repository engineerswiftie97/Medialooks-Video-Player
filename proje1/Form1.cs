using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MPLATFORMLib;
using System.Runtime.InteropServices;

namespace proje1
{
    public partial class Form1 : Form
    {
        public Form1()
        {

            InitializeComponent();
        }
        MPlaylistClass myPlaylist = new MPlaylistClass();
        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
            //Set and enable a preview,bunları her projede yaz
            myPlaylist.PreviewWindowSet("", panel1.Handle.ToInt32());
            myPlaylist.PreviewEnable("", 1, 1);
            myPlaylist.PropsSet("loop", "false");
            //Start mFile object
            myPlaylist.ObjectStart(new object());
            //Fill video formats
            int nCount;
            int nIndex;
            string strFormat;
            M_VID_PROPS vidProps;
            comboBox1.Items.Clear();
            //Get video format count
            myPlaylist.FormatVideoGetCount(eMFormatType.eMFT_Convert, out nCount);
            comboBox1.Enabled = nCount > 0;
            if (nCount <= 0) return;
            for (int i = 0; i < nCount; i++)
            {
                //Get format by index
                myPlaylist.FormatVideoGetByIndex(eMFormatType.eMFT_Convert, i, out vidProps, out strFormat);
                comboBox1.Items.Add(strFormat);
            }
            //Check if there is selected format
            myPlaylist.FormatVideoGet(eMFormatType.eMFT_Convert, out vidProps, out nIndex, out strFormat);
            comboBox1.SelectedIndex = nIndex > 0 ? nIndex : 0;
            //Fill audio formats
            M_AUD_PROPS audProps;
            comboBox2.Items.Clear();
            //Get video format count
            myPlaylist.FormatAudioGetCount(eMFormatType.eMFT_Convert, out nCount);
            comboBox2.Enabled = nCount > 0;
            if (nCount <= 0) return;
            for (int i = 0; i < nCount; i++)
            {
                //Get format by index
                myPlaylist.FormatAudioGetByIndex(eMFormatType.eMFT_Convert, i, out audProps, out strFormat);
                comboBox2.Items.Add(strFormat);
            }
            //Check if there is selected format
            myPlaylist.FormatAudioGet(eMFormatType.eMFT_Convert, out audProps, out nIndex, out strFormat);
            comboBox2.SelectedIndex = nIndex > 0 ? nIndex : 0;
            //set audio volume
            trackBar1.Value = 50;
            double dblPos = (double)trackBar1.Value / trackBar1.Maximum;
            myPlaylist.PreviewAudioVolumeSet("", -1, -30 * (1 - dblPos));

        }
        private void button3_Click(object sender, EventArgs e)
        {
           

            int val1;
            double val2;
            double x;
            double y;
            double z;
            if (listBox1.Items.Count > 0 && listBox1.SelectedIndex >= 0)
            {

                string strPathOrCommand;
                MItem pItem;
                double dblPos;
                myPlaylist.PlaylistGetByIndex(listBox1.SelectedIndex, out dblPos, out strPathOrCommand, out pItem);

                pItem.FileInOutGet(out x, out y, out z);
               
                double _dbFilePos;
                //item ın position unu get ile alıp sonra ona set ediyoruz. böylece kaldıgı yerden devam edıyo
                pItem.FilePosGet(out _dbFilePos);
                pItem.FilePosSet(_dbFilePos, 0);
                myPlaylist.FilePlayStart();

                //myPlaylist.PlaylistPosSet(listBox1.SelectedIndex, 0, 0);
                
                myPlaylist.PlaylistGetCount(out val1, out val2);
                label5.Text = "Video Time: " + val2.ToString() + "s";
                
                trackBar2.Maximum = Convert.ToInt32(z);
               

            }


        }

        private void button4_Click(object sender, EventArgs e)
        {
            myPlaylist.FilePlayPause(0);

        }

        private void button5_Click(object sender, EventArgs e)
        {
            myPlaylist.FilePlayStop(0);
            myPlaylist.FilePosSet(0, 0);
            trackBar2.Value = 0;
            label4.Width = 10;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.FileNames.Length != 0)
            {
                foreach (string t in openFileDialog1.FileNames)
                {
                    int nIndex = -1;
                    MItem pFile;
                    myPlaylist.PlaylistAdd(null, t, "", ref nIndex, out pFile);
                }
                updateList(); // this method allows to keep listBox1 in actual state.
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                myPlaylist.PlaylistRemoveByIndex(listBox1.SelectedIndex, 0);
                updateList();
            }

        }
        private void updateList()
        {
            listBox1.Items.Clear();
            int nFiles;
            double dblDuration;
            myPlaylist.PlaylistGetCount(out nFiles, out dblDuration);
            for (int i = 0; i < nFiles; i++)
            {
                string strPathOrCommand;
                MItem pItem;
                double dblPos;
                myPlaylist.PlaylistGetByIndex(i, out dblPos, out strPathOrCommand, out pItem);
                strPathOrCommand = strPathOrCommand.Substring(strPathOrCommand.LastIndexOf('\\') + 1);
                listBox1.Items.Add(strPathOrCommand);
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            double dblPos = (double)trackBar1.Value / trackBar1.Maximum;
            myPlaylist.PreviewAudioVolumeSet("", -1, -30 * (1 - dblPos));


        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            M_VID_PROPS vidProps = new M_VID_PROPS();
            string strFormat;
            myPlaylist.FormatVideoGetByIndex(eMFormatType.eMFT_Convert, comboBox1.SelectedIndex, out vidProps, out strFormat);
            //Set new video format
            myPlaylist.FormatVideoSet(eMFormatType.eMFT_Convert, ref vidProps);


        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            M_AUD_PROPS audProps = new M_AUD_PROPS();
            string strFormat;
            myPlaylist.FormatAudioGetByIndex(eMFormatType.eMFT_Convert, comboBox2.SelectedIndex, out audProps, out strFormat);
            //Set new audio format
            myPlaylist.FormatAudioSet(eMFormatType.eMFT_Convert, ref audProps);

        }
        bool aa = false;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(aa)
            {
                return;
            }
            double a;
            double b;
            int c;
            int d;
            myPlaylist.PlaylistPosGet(out c, out d, out a, out b);
            trackBar2.Value = Convert.ToInt32(a);
            label4.Width = (trackBar2.Value * (trackBar2.Width - 20)) / trackBar2.Maximum;

           
            eMState eState;
            if (myPlaylist != null)//null exception için try-catch e alınmış.
            {
                myPlaylist.ObjectStateGet(out eState);
                try
                {
                    int nCount;
                    double dblListLen;
                    myPlaylist.PlaylistGetCount(out nCount, out dblListLen);

                    if (nCount > 0)
                    {
                        int nCurFile, nNextFile = 0;
                        double dblFilePos, dblListPos = 0;
                        myPlaylist.PlaylistPosGet(out nCurFile, out nNextFile, out dblFilePos, out dblListPos);

                        string strPath;
                        MItem pItem;
                        double dblListOffset;
                        myPlaylist.PlaylistGetByIndex(nCurFile, out dblListOffset, out strPath, out pItem);

                        double dblIn, dblOut, dblFileLen = 0;
                        pItem.FileInOutGet(out dblIn, out dblOut, out dblFileLen);
                        if (dblOut > dblIn)
                            dblFileLen = dblOut;
                      
                        
                        string strFile = strPath.Substring(strPath.LastIndexOf('\\') + 1);

                        label3.Text = Dbl2PosStr(dblFilePos) + "/" + Dbl2PosStr(dblFileLen) + " " +
                            eState.ToString() + " (" + (nCurFile + 1) + "/" + nCount + ") " +
                            Dbl2PosStr(dblListPos) + "/" + Dbl2PosStr(dblListLen) + " " +
                            "\r\n" + strFile + "\r\n" + strPath;

                        System.Runtime.InteropServices.Marshal.ReleaseComObject(pItem);
                    }

                    GC.Collect();
                }
                catch
                {

                    label4.Width = 0;
                    throw;
                }
                string Dbl2PosStr(double _dblPos)
                {
                    int nHour = (int)_dblPos / 3600;
                    int nMinutes = ((int)_dblPos % 3600) / 60;
                    int nSec = ((int)_dblPos % 60);
                    _dblPos -= (int)_dblPos;
                    int nMsec = (int)(_dblPos * 1000 + 0.5);


                    string strRes = nHour.ToString("00") + ":" + nMinutes.ToString("00") + ":" + nSec.ToString("00") + "." + nMsec.ToString("000");
                    return strRes;
                }
            }

        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            aa = true;
            int nCount;
            double dblListLen;
            myPlaylist.PlaylistGetCount(out nCount, out dblListLen);
            if (nCount > 0)
            {

                myPlaylist.FilePlayPause(0);

                label4.Width = (trackBar2.Value * (trackBar2.Width - 20)) / trackBar2.Maximum;

                double dblIn = 0;
                double dblOut = 0;
                double dblDuration = 0;
                string strPath;

                MItem pItem;
                double dblListOffset;
                myPlaylist.PlaylistGetByIndex(listBox1.SelectedIndex, out dblListOffset, out strPath, out pItem);

                pItem.FileInOutGet(out dblIn, out dblOut, out dblDuration);
                dblDuration = (dblOut > dblIn ? dblOut : dblDuration) - dblIn;
                double dblPos = totalDur + dblIn + dblDuration * (double)trackBar2.Value / (double)trackBar2.Maximum;
                myPlaylist.FilePosSet(dblPos, 0);
            
            }
            
        }

     

        private void trackBar2_MouseLeave(object sender, EventArgs e)
        {

        }
        double totalDur = 0;
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            double val1;
            string val2;
            MItem val3;

            myPlaylist.PlaylistGetByIndex(listBox1.SelectedIndex, out val1, out val2, out val3);
            myPlaylist.PlaylistPosSet(listBox1.SelectedIndex, 0, 0);
            int nCurFile, nNextFile = 0;
            double dblFilePos, dblListPos = 0;
            myPlaylist.PlaylistPosGet(out nCurFile, out nNextFile, out dblFilePos, out dblListPos);
            totalDur = dblListPos;
            myPlaylist.FilePlayStop(0);
            trackBar2.Value = 0;
            label4.Width = 0;
            double x;
            double y;
            double z;
            val3.FileInOutGet(out x, out y, out z);
            trackBar2.Maximum = Convert.ToInt32(z);
        }


        private void trackBar2_MouseUp(object sender, MouseEventArgs e)
        {

            myPlaylist.FilePlayStart();
            aa = false;


        }

        
    }
}

