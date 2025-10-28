using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public class ImagePopupForm : Form
    {
        private PictureBox popupPictureBox;

        public ImagePopupForm(System.Drawing.Image image)
        {
            // 폼 설정
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "원본 이미지";

            // 이미지 크기에 맞춰 폼 크기 설정
            this.ClientSize = image.Size;

            // PictureBox 생성 및 설정
            popupPictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                Image = image,
                SizeMode = PictureBoxSizeMode.AutoSize
            };

            // ESC 키로 닫기 가능하도록 설정
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                    this.Close();
            };

            // 이미지 클릭시 폼 닫기
            popupPictureBox.Click += (s, e) => this.Close();

            // PictureBox를 폼에 추가
            this.Controls.Add(popupPictureBox);
        }
    }
}
