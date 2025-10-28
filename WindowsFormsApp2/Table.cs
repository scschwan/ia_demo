using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static WindowsFormsApp2.Image_Process;

namespace WindowsFormsApp2
{
    internal class Table
    {
        private ListView listViewProducts;
        private ListView listViewCounts;
        private readonly string[] infoOrder = { "I", "A", "PIN","NG" };
        private Font regularFont;
        private Font boldFont;

        private const int RowHeight = 25; // 각 행의 높이
        private const int HeaderHeight = 25; // 헤더의 높이
        private const int DesiredRowCount = 4; // 원하는 데이터 행 수

        public Table(ListView listView1, ListView listView2)
        {
            listViewProducts = listView1;
            listViewCounts = listView2;
            InitializeListViews();
        }

        private void InitializeListViews()
        {
            regularFont = new Font(listViewProducts.Font.FontFamily, listViewProducts.Font.Size + 2);
            boldFont = new Font(regularFont, FontStyle.Bold);

            Action<ListView> commonSetup = (lv) =>
            {
                lv.View = View.Details;
                lv.FullRowSelect = true;
                lv.GridLines = true;
                lv.OwnerDraw = true;
                lv.Font = regularFont;
                lv.BorderStyle = BorderStyle.FixedSingle; // BorderStyle을 FixedSingle로 변경
            };

            commonSetup(listViewProducts);
            commonSetup(listViewCounts);

            listViewProducts.Columns.Clear();
            listViewProducts.Columns.Add("Info", 50);
            listViewProducts.Columns.Add("X", 100);
            listViewProducts.Columns.Add("Y", 100);
            listViewProducts.Columns.Add("Deg", -2);

            listViewCounts.Columns.Clear();
            listViewCounts.Columns.Add("제품", 135);
            listViewCounts.Columns.Add("이송 현황", 135);

            listViewProducts.DrawColumnHeader += ListView_DrawColumnHeader;
            listViewProducts.DrawItem += ListView_DrawItem;
            listViewProducts.DrawSubItem += ListView_DrawSubItem;
            listViewCounts.DrawColumnHeader += ListView_DrawColumnHeader;
            listViewCounts.DrawItem += ListView_DrawItem;
            listViewCounts.DrawSubItem += ListView_DrawSubItem;
            listViewProducts.Resize += ListView_Resize;
            listViewCounts.Resize += ListView_Resize;

            // Paint 이벤트 핸들러 추가
            listViewProducts.Paint += ListView_Paint;
            listViewCounts.Paint += ListView_Paint;

            listViewCounts.Height = (DesiredRowCount * RowHeight) + HeaderHeight;
        }

        private void ListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.White, e.Bounds);
            e.Graphics.DrawRectangle(new Pen(Color.Black, 2), e.Bounds);
            TextRenderer.DrawText(e.Graphics, e.Header.Text, boldFont, e.Bounds, Color.Black,
                                  TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }

        private void ListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void ListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            Rectangle bounds = new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width, RowHeight);
            e.Graphics.DrawRectangle(Pens.Black, bounds);

            Font font = e.ColumnIndex == 0 || sender == listViewCounts ? boldFont : regularFont;

            if (e.Item.BackColor == Color.LightGray)
            {
                e.Graphics.FillRectangle(Brushes.LightGray, bounds);
            }

            TextRenderer.DrawText(e.Graphics, e.SubItem.Text, font, bounds, Color.Black,
                                  TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }

        private void ListView_Resize(object sender, EventArgs e)
        {
            ListView lv = (ListView)sender;
            if (lv == listViewCounts)
            {
                int columnWidth = lv.ClientSize.Width / 2;
                lv.Columns[0].Width = columnWidth;
                lv.Columns[1].Width = lv.ClientSize.Width - columnWidth;
            }
            else
            {
                int lastColumnWidth = lv.ClientSize.Width;
                for (int i = 0; i < lv.Columns.Count - 1; i++)
                {
                    lastColumnWidth -= lv.Columns[i].Width;
                }
                if (lastColumnWidth > 0)
                {
                    lv.Columns[lv.Columns.Count - 1].Width = lastColumnWidth;
                }
            }
        }

        // 새로운 Paint 이벤트 핸들러 추가
        private void ListView_Paint(object sender, PaintEventArgs e)
        {
            if (sender is ListView listView)
            {
                // 굵은 외곽선 그리기
                using (Pen pen = new Pen(Color.Black, 2))
                {
                    e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, listView.Width - 1, listView.Height - 1));
                }
            }
        }

        public void DisplaySortedProductInfo(List<Product> productInfoList)
        {
            listViewProducts.BeginUpdate();
            listViewProducts.Items.Clear();
            
            var sortedProducts = productInfoList
                .Where(p => p.Info != "Non Pick") // Non Pick 제외
                .GroupBy(p => p.Info)
                .OrderBy(g => Array.IndexOf(infoOrder, g.Key))
                .SelectMany(g => g.OrderBy(p => double.Parse(p.Y)).ThenBy(p => double.Parse(p.X)))
                .ToList();

            string currentInfo = "";
            foreach (var product in sortedProducts)
            {
                if (product.Info != currentInfo)
                {
                    if (currentInfo != "")
                    {
                        AddSeparatorItem(listViewProducts);
                    }
                    currentInfo = product.Info;
                }

                var item = new ListViewItem(new[] { product.Info, product.X, product.Y, product.Deg.ToString("F2") });
                listViewProducts.Items.Add(item);
            }

            listViewProducts.EndUpdate();
        }

        public void DisplayProductCountInfo(ProductManager productManager)
        {
            listViewCounts.BeginUpdate();
            listViewCounts.Items.Clear();

            var productTypes = new[] { "I", "A", "PIN", "NG" };

            foreach (var productType in productTypes)
            {
                var count = productManager.ProductCounts.FirstOrDefault(p => p.Type == productType)?.Count ?? 0;
                var item = new ListViewItem(new[] { productType, count.ToString() });
                listViewCounts.Items.Add(item);
            }

            listViewCounts.EndUpdate();
        }

        private void AddSeparatorItem(ListView listView)
        {
            var separatorItem = new ListViewItem(new[] { "", "", "", "" });
            separatorItem.BackColor = Color.LightGray;
            listView.Items.Add(separatorItem);
        }
    }
}