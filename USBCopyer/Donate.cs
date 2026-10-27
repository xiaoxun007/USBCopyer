using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace USBCopyer
{
    public partial class donateForm : Form
    {
        public donateForm()
        {
            InitializeComponent();
            Icon = Program.ico;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
                try
                {
                    // 隐藏性定制：移除跳转作者代码库的外部链接
                }
                catch (Exception ex)
                {
                    MessageBox.Show("打开失败：" + ex.ToString());
                }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                // 隐藏性定制：移除跳转作者博客的外部链接
            }
            catch (Exception ex)
            {
                MessageBox.Show("打开失败：" + ex.ToString());
            }
        }
    }
}
