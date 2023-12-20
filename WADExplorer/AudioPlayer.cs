using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace WADExplorer
{
    public partial class AudioPlayer : UserControl
    {
        public SoundPlayer Player = new SoundPlayer();
        public Stream audioStream;

        public AudioPlayer()
        {
            InitializeComponent();
        }

        public void SetSoundFromStream(Stream sound)
        {
            Player = new SoundPlayer(sound);
            audioStream = sound;
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            Player.Play();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            Player.Stop();
        }

        private void ExportStreamButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog()
            {
                FileName = AudioDescription.Text,
                Filter = "Microsoft WAV|*.wav|All files|*.*"
            };

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                byte[] buffer = new byte[audioStream.Length];
                //audioStream.Read(buffer, 0, buffer.Length);

                using (var f = new BinaryReader(audioStream, Encoding.ASCII, true))
                    buffer = f.ReadBytes(buffer.Length);

                FileStream file = new FileStream(saveFile.FileName, FileMode.OpenOrCreate, FileAccess.Write);
                file.Write(buffer, 0, buffer.Length);
                file.Close();

                MessageBox.Show(String.Format("Successfully exported audio buffer to '{0}'!", saveFile.FileName), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
