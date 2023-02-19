using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PageSwapper
{
    public partial class Form1 : Form
    {
        const int PAGE_NUM = 4;
        const int INS_NUM_PER_PAGE = 10;
        const int INS_NUM = 320;
        int circle = 0;
        int insConter = 0;
        int randomMark = 0;
        int randomMark1 = -1;
        int randomMark2 = -1;
        


        float missCounter = 0;
        int selectedAlgo = 0;
        int selectedIns = 0;
        //指令队列
        List<int> isExecute = new List<int>();

        public class Page
        {
            public List<TextBox> instructors;
            public RichTextBox pageNum;
            public int pageLife;
            public bool pageState;
            public int curNum;
            public List<int> ins;
            public Page()
            {
                pageLife = 0;
                curNum = -1;
                pageState = false;
                instructors = new List<TextBox>();

            }
            public void PageChosen(int pos)
            {
                for (int i = 0; i < INS_NUM_PER_PAGE; i++)
                {
                    instructors[i].Text = i.ToString();
                    instructors[i].BackColor = System.Drawing.Color.Aqua;
                }
                instructors[pos].BackColor = System.Drawing.Color.Tomato;
                pageLife = 0;
                pageState = true;
            }

            public void PageReset()
            {
                for (int i = 0; i < INS_NUM_PER_PAGE; i++)
                {
                    instructors[i].Text = "";
                    instructors[i].BackColor = System.Drawing.SystemColors.Control;
                }
                pageLife = 0;
                curNum = -1;
                pageState = false;
                pageNum.Text = "";

            }
        }
        List<Page> pages = new List<Page>();

        public int chooseIns()
        {
            //随机选择一条指令执行
            Random rd = new Random();
            int idx = rd.Next(isExecute.Count);
            randomMark = idx;
            int rev = isExecute[idx];
            isExecute.Remove(rev);

            return rev;
        }
   
       
        public int chooseIns2()
        {
            isExecute.Remove(isExecute[0]);
            //顺序执行
            return insConter;
        }
        //public int chooseIns3()
        //{
        //    //混合执行
        //    //执行了偶数个指令时随机选择一条
        //    if (insConter % 2 == 0)
        //    {
        //        return chooseIns();
        //    }
        //    else  //奇数时选择下一条指令执行
        //    {
        //        if (randomMark >= isExecute.Count)
        //            randomMark = 0;
        //        int rev = isExecute[randomMark];
        //        isExecute.Remove(rev);
        //        return rev;
        //    }
        //}
        public int chooseIns3()
        {
            if (insConter % 6 == 0)  //随机选取 0 - 319，标记为randomMark
            {
                Random rd = new Random();
                int idx = rd.Next(isExecute.Count);
                randomMark = idx;
                int rev = isExecute[idx];
                isExecute.Remove(rev);

                return rev;
            }
            else if (insConter % 6 == 1)  //顺序执行
            {
                if (randomMark >= isExecute.Count)
                    randomMark = 0;
                int rev = isExecute[randomMark];
                isExecute.Remove(rev);
                return rev;
            }
            else if (insConter % 6 == 2)  //随机选取 0 - randomMark，标记为randomMark1
            {
                Random rd1 = new Random();
                int idx = rd1.Next(randomMark);
                randomMark1 = idx;

                int rev = isExecute[idx];

                Console.WriteLine(randomMark);
                Console.WriteLine(randomMark1);
                Console.WriteLine(rev);
                Console.WriteLine("-----------");

                isExecute.Remove(rev);
                return rev;
            }
            else if (insConter % 6 == 3)  //顺序执行
            {
                if (randomMark1 >= isExecute.Count)
                    randomMark1 = 0;
                int rev = isExecute[randomMark1];
                isExecute.Remove(rev);
                return rev;
            }
            else if (insConter % 6 == 4)  //随机选取 0 - randomMark1，标记为randomMark2
            {
                Random rd = new Random();
                int idx = rd.Next(randomMark1, isExecute.Count);
                randomMark2 = idx;
                int rev = isExecute[idx];
                isExecute.Remove(rev);
                return rev;
            }
            else  //奇数时选择下一条指令执行
            {
                if (randomMark2 >= isExecute.Count)
                    randomMark2 = isExecute.Count - 1;
                int rev = isExecute[randomMark2];
                isExecute.Remove(rev);
                return rev;
            }
        }

        public int FIFO(int pageNum)
        {
            for (int i = 0; i < PAGE_NUM; i++)
            {
                if (pages[i].pageNum.Text == "第" + pageNum.ToString() + "页")
                {
                    return i;
                }
            }

            return circle++ % PAGE_NUM;
        }

        public int LRU(int pageNum)
        {
            for (int i = 0; i < PAGE_NUM; i++)
            {
                if (pages[i].pageNum.Text == "第" + pageNum.ToString() + "页")
                {
                    return i;
                }
            }


            int longestPage = 0;
            int longestLife = 0;
            //Console.WriteLine("----------------");
            for (int i = 0; i < PAGE_NUM; i++)
            {
                if (pages[i].pageLife > longestLife)
                {
                    longestPage = i;
                    longestLife = pages[i].pageLife;
                }
              //  Console.WriteLine(pages[i].pageLife);
            }
            //Console.WriteLine("++++++++++++++");
           // Console.WriteLine(longestPage);
            return longestPage;
        }

        public void Swapper()
        {
            bool isPageMiss = false;
            //int i = chooseIns();  //选中的指令
            int i;
            if (selectedIns == 0)
            {
                i = chooseIns();
            }
            else if (selectedIns == 1)
            {
                i = chooseIns2();
            }
            else
            {
                i = chooseIns3();
            }

            insConter++;
            int np = i / INS_NUM_PER_PAGE;  //选中的页面
            //int p = choosePage(np);  //内存中的页（有4个）
            int p;
            if (selectedAlgo == 0)
            {
                p = FIFO(np);
            }
            else
            {
                p = LRU(np);
            }
            int pagePos = i % 10;       //指令在页中的位置
            pages[p].pageNum.Text = "第" + np.ToString() + "页";
            pages[p].PageChosen(pagePos);

            int prePage = pages[p].curNum;
            if (prePage != np)
            {
                pages[p].pageLife = 0;
                missCounter++;
                isPageMiss = true;
            }
            pages[p].curNum = np;
            LogSwapper(i, isPageMiss, prePage, np);

        }
        public void UpdateMiss()
        {
            this.richTextBox6.Text = "缺页数：" + missCounter.ToString();
            this.richTextBox7.Text = "缺页率：" + (Math.Ceiling(((missCounter) / insConter) * 100)).ToString() + "%";
            if (insConter == 0)
            {
                this.richTextBox7.Text = "缺页率：0%";
            }
        }

        public void LogSwapper(int ins, bool isMiss, int pageOut, int pageIn)
        {
            string str0, str1, str2, str3, str4;
            str0 = insConter.ToString();
            str1 = ins.ToString();
            str2 = isMiss ? "是" : "否";
            str3 = pageOut.ToString();
            str4 = pageIn.ToString();
            if (pageOut == -1)
            {
                str3 = "-";
            }
            if (pageIn == pageOut)
            {
                str3 = str4 = "-";
            }
            this.LogBox.Text += "\n";
            this.LogBox.Text += "    " +
                             str0 + (new string(' ', 9 - str0.Length)) +
                             str1 + (new string(' ', 7 - str1.Length)) +
                             str2 + (new string(' ', 7 - str2.Length)) +
                             str3 + (new string(' ', 9 - str3.Length)) +
                             str4;
        }


        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < INS_NUM; i++)
            {
                isExecute.Add(i);
            }
            for (int i = 0; i < PAGE_NUM; i++)
            {
                pages.Add(new Page());
            }

            this.comboBox1.Items.Add("FIFO");
            this.comboBox1.Items.Add("LRU");

            this.comboBox2.Items.Add("随机执行");
            this.comboBox2.Items.Add("顺序执行");
            this.comboBox2.Items.Add("混合执行");
            pages[0].pageNum = this.richTextBox1;
            pages[1].pageNum = this.richTextBox2;
            pages[2].pageNum = this.richTextBox3;
            pages[3].pageNum = this.richTextBox4;

            pages[0].instructors.Add(this.textBox1);
            pages[0].instructors.Add(this.textBox2);
            pages[0].instructors.Add(this.textBox3);
            pages[0].instructors.Add(this.textBox4);
            pages[0].instructors.Add(this.textBox5);
            pages[0].instructors.Add(this.textBox6);
            pages[0].instructors.Add(this.textBox7);
            pages[0].instructors.Add(this.textBox8);
            pages[0].instructors.Add(this.textBox9);
            pages[0].instructors.Add(this.textBox10);

            pages[1].instructors.Add(this.textBox11);
            pages[1].instructors.Add(this.textBox12);
            pages[1].instructors.Add(this.textBox13);
            pages[1].instructors.Add(this.textBox14);
            pages[1].instructors.Add(this.textBox15);
            pages[1].instructors.Add(this.textBox16);
            pages[1].instructors.Add(this.textBox17);
            pages[1].instructors.Add(this.textBox18);
            pages[1].instructors.Add(this.textBox19);
            pages[1].instructors.Add(this.textBox20);

            pages[2].instructors.Add(this.textBox21);
            pages[2].instructors.Add(this.textBox22);
            pages[2].instructors.Add(this.textBox23);
            pages[2].instructors.Add(this.textBox24);
            pages[2].instructors.Add(this.textBox25);
            pages[2].instructors.Add(this.textBox26);
            pages[2].instructors.Add(this.textBox27);
            pages[2].instructors.Add(this.textBox28);
            pages[2].instructors.Add(this.textBox29);
            pages[2].instructors.Add(this.textBox30);

            pages[3].instructors.Add(this.textBox31);
            pages[3].instructors.Add(this.textBox32);
            pages[3].instructors.Add(this.textBox33);
            pages[3].instructors.Add(this.textBox34);
            pages[3].instructors.Add(this.textBox35);
            pages[3].instructors.Add(this.textBox36);
            pages[3].instructors.Add(this.textBox37);
            pages[3].instructors.Add(this.textBox38);
            pages[3].instructors.Add(this.textBox39);
            pages[3].instructors.Add(this.textBox40);
        }


        private void allStepTick(object sender, EventArgs e)
        {
            if (isExecute.Count == 0)
            {
                return;
            }
            for (int i = 0; i < PAGE_NUM; i++)
            {
                pages[i].pageLife++;
            }
            Swapper();
            UpdateMiss();
            if (isExecute.Count == 0)
            {
                allStepTimer.Stop();
            }
        }

        private void oneStepTick(object sender, EventArgs e)
        {
            if (isExecute.Count == 0)
            {
                return;
            }
            for (int i = 0; i < PAGE_NUM; i++)
            {
                pages[i].pageLife++;
            }
            Swapper();
            UpdateMiss();
            oneStepTimer.Stop();
        }
        private void oneStepClicked(object sender, EventArgs e)
        {
            oneStepTimer.Enabled = true;
            oneStepTimer.Interval = 100;
            if (allStepTimer.Enabled == false)
                this.oneStepTimer.Start();
        }


        private void allStepClicked(object sender, EventArgs e)
        {
            allStepTimer.Enabled = true;
            allStepTimer.Interval = 100;
            if (allStepTimer.Enabled == false)
                this.allStepTimer.Start();

        }

        private void resetClicked(object sender, EventArgs e)
        {
            oneStepTimer.Stop();
            allStepTimer.Stop();
            circle = 0;
            insConter = 0;
            randomMark = 0;
            isExecute.Clear();
            missCounter = 0;
            for (int i = 0; i < INS_NUM; i++)
            {
                isExecute.Add(i);
            }
            for (int i = 0; i < PAGE_NUM; i++)
            {
                pages[i].PageReset();
            }
            LogBox.Text = "  执行序号   地址   缺页   换出页   换入页";
            UpdateMiss();
        }


        private void selectAlgo(object sender, EventArgs e)
        {
            selectedAlgo = comboBox1.SelectedIndex;
        }

        private void selectPatten(object sender, EventArgs e)
        {
            selectedIns = comboBox2.SelectedIndex;
        }
    }

}
