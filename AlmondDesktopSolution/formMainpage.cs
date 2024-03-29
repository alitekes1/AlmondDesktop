﻿using Almond;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace formsApp
{
    public partial class FormMainpage : Form
    {
        public FormMainpage()
        {
            InitializeComponent();
        }
        flashCardClass flashCard = new flashCardClass();
        public int i = 0;
        public int j = 0;
        private void veriEkleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formAdd addform = new formAdd();
            addform.Show();
        }

        private void veriSilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formDelete deleteform = new formDelete();
            deleteform.ShowDialog();
        }
        private void verileriGüncelleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formUpdate updateform = new formUpdate();
            updateform.ShowDialog();
        }
        private void gösterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formAllData alldataShowDialog = new formAllData();
            alldataShowDialog.Show();
            MessageBox.Show("Bu sayfayı verileri daha iyi görebilmek için TAM EKRAN yapabilirsiniz.", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void listeSilToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            formDeleteList listdeleteform = new formDeleteList();
            listdeleteform.ShowDialog();
        }
        private void listeOluşturToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            formCreateList formCreateList2 = new formCreateList();
            formCreateList2.ShowDialog();
        }
        private void tümListelerimToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formMyList formMylistt = new formMyList();
            formMylistt.Show();
            MessageBox.Show("Bu sayfayı verileri daha iyi görebilmek için TAM EKRAN yapabilirsiniz.", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        private void isChooseList()//listenin seçilip seçilmediğini kontrol eder.
        {
            if (comboboxlisteMain.SelectedItem == null)
            {
                //liste seçili değilse eğer butonlara tıklanılmasını engeller.
                btnpuan1.Enabled = false;
                btnpuan2.Enabled = false;
                butonpuan3.Enabled = false;
                btnpuan4.Enabled = false;
                btnpuan5.Enabled = false;
                btnshowanswerMain.Enabled = false;
                btnshowanswerMain.Text = "Liste Seçiniz!";
            }
            else
            {//liste seçildiğinde butonlar tıklanılabilir hale gelir.
                btnshowanswerMain.Text = "Cevabı Göster";
                btnpuan1.Enabled = true;
                btnpuan2.Enabled = true;
                butonpuan3.Enabled = true;
                btnpuan4.Enabled = true;
                btnpuan5.Enabled = true;
                btnshowanswerMain.Enabled = true;
            }
        }
        private void FormMainpage_Load(object sender, EventArgs e)
        {
            isChooseList();
            flashCard.comboboxCreate(comboboxlisteMain);
        }
        private void comboboxlisteMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            datatableforPDF();
            labelsoru.Visible = true;
            i = 0;
            j = 0;
            SqlCommand komut = new SqlCommand("select idnumber,quesiton,answer,listName,puan,soruNo from data_table where listName=@list and answer is not null order by puan asc", flashCard.baglanti());//seçilen listeye göre data gride verileri aktarır ve puan artan şekilde sıralar
            komut.Parameters.AddWithValue("@list", comboboxlisteMain.Text);
            SqlDataAdapter da = new SqlDataAdapter(komut);
            DataSet ds = new DataSet();
            da.Fill(ds);
            datagridMain.DataSource = ds.Tables[0];
            flashCard.baglanti().Close();
            showCard();
            isChooseList();
        }

        private void datatableforPDF()
        {
            SqlCommand komut1 = new SqlCommand("select quesiton,answer,puan from data_table where listName=@parameter and answer is not null order by puan asc", flashCard.baglanti());//seçilen listeye göre data gride verileri aktarır ve puan artan şekilde sıralar
            komut1.Parameters.AddWithValue("@parameter", comboboxlisteMain.Text);
            SqlDataAdapter da1 = new SqlDataAdapter(komut1);
            DataSet ds1 = new DataSet();
            da1.Fill(ds1);
            datagridPDF.DataSource = ds1.Tables[0];
            datagridPDF.Columns[0].HeaderText = "Soru";
            datagridPDF.Columns[1].HeaderText = "Cevap";
            datagridPDF.Columns[2].HeaderText = "Puan";
            flashCard.baglanti().Close();
        }

        private void btnShowDialoganswerMain_Click(object sender, EventArgs e)
        {
            labelcevap.Visible = true;
        }
        private void showCard()
        {

            labelcevap.Visible = false;
            ///soruyu ekrana yazdırma
            if (i < datagridMain.Rows.Count - 1)//datagridin son satırına gelene kadar git ve hücreyi labela aktar
            {
                labelsoru.Text = datagridMain.Rows[i].Cells[1].Value.ToString();
                labelSoruNo.Text = datagridMain.Rows[i].Cells[5].Value.ToString();
                i++;
            }
            /// cevabı ekrana yazırma 
            if (j < datagridMain.Rows.Count - 1)//datagridin son satırına kadar git ve hücreyi labela aktar
            {
                labelcevap.Text = datagridMain.Rows[j].Cells[2].Value.ToString();
                j++;
            }

            if (j == datagridMain.Rows.Count - 1)//listedeki tüm kartlar gözüktüyse
            {
                btnpuan1.Enabled = false;
                btnpuan2.Enabled = false;
                btnpuan3.Enabled = false;
                btnpuan4.Enabled = false;
                btnpuan5.Enabled = false;
                labelsoru.Visible = false;
                btnshowanswerMain.Enabled = false;
                MessageBox.Show("Seçili listeye ait tüm kartları gözden geçirdiniz.Başka listelerle devam etmek istiyorsanız tekrar liste seçiniz.", "Liste Sonu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                comboboxlisteMain.Text = "";
            }
        }
        private void puanUpdate(Label label, int artis)//puan artırma fonksiyonu
        {
            SqlCommand komut = new SqlCommand("update data_table set puan+=@p2 where soruNo=@p1", flashCard.baglanti());
            komut.Parameters.AddWithValue("@p1", label.Text);
            komut.Parameters.AddWithValue("@p2", artis);
            komut.ExecuteNonQuery();
            flashCard.baglanti().Close();
        }
        private void btnpuan1_Click(object sender, EventArgs e)
        {
            int artis = 1;
            puanUpdate(labelSoruNo, artis);
            showCard();
        }
        private void btnpuan2_Click(object sender, EventArgs e)
        {
            int artis = 2;
            puanUpdate(labelSoruNo, artis);
            showCard();
        }
        private void butonpuan3_Click(object sender, EventArgs e)
        {
            int artis = 3;
            puanUpdate(labelSoruNo, artis);
            showCard();
        }
        private void btnpuan4_Click(object sender, EventArgs e)
        {
            int artis = 4;
            puanUpdate(labelSoruNo, artis);
            showCard();

        }
        private void btnpuan5_Click(object sender, EventArgs e)
        {
            int artis = 5;
            puanUpdate(labelSoruNo, artis);
            showCard();
        }
        private void enÇokToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formPuanTable formMyMistakes1 = new formPuanTable();
            formMyMistakes1.Show();
        }
        private void çıkışYapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void comboboxlisteMain_MouseClick(object sender, MouseEventArgs e)
        {
            flashCard.comboboxRefresh(comboboxlisteMain);//comboboxa tıkalayınca refresh eder
        }

        private void bilgiAlmakİçinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hakkımda hakkimda = new hakkımda();
            hakkimda.ShowDialog();
        }
        private void projeÖzetiVeÇıkarılanDerslerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Çok Yakında...", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void pDFToolStripMenuItem1_Click(object sender, EventArgs e)//PDF İŞLEMLERİ (bu kısım hazır olarak alınmıştır).
        {
            SaveFileDialog save = new SaveFileDialog();
            save.OverwritePrompt = false;
            save.Title = "PDF Dosyaları";
            save.DefaultExt = "pdf";
            save.Filter = "PDF Dosyaları (*.pdf)|*.pdf|Tüm Dosyalar(*.*)|*.*";
            if (save.ShowDialog() == DialogResult.OK)
            {
                PdfPTable pdfTable = new PdfPTable(datagridPDF.ColumnCount);
                pdfTable.DefaultCell.Padding = 3; // hücre duvarı ve veri arasında mesafe
                pdfTable.WidthPercentage = 95; // hücre genişliği
                pdfTable.HorizontalAlignment = Element.ALIGN_MIDDLE; // yazı hizalaması
                pdfTable.DefaultCell.BorderWidth = 1; // kenarlık kalınlığı
                                                      //pdfTable.DefaultCell.VerticalAlignment= Element.ALIGN_MIDDLE;
                foreach (DataGridViewColumn column in datagridPDF.Columns)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText));
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(240, 240, 240);// hücre arka plan rengi
                    cell.HorizontalAlignment = Element.ALIGN_CENTER; // Başlıkları ortala
                    pdfTable.AddCell(cell);
                }


                try
                {
                    foreach (DataGridViewRow row in datagridPDF.Rows)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            pdfTable.AddCell(cell.Value.ToString());
                        }
                    }

                }
                catch (NullReferenceException)
                {
                }
                using (FileStream stream = new FileStream(save.FileName + ".pdf", FileMode.Create))
                {
                    Document pdfDoc = new Document(PageSize.A2, 10f, 10f, 10f, 0f);// sayfa boyutu.
                    PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();
                    pdfDoc.Add(new Paragraph(" ", FontFactory.GetFont(FontFactory.HELVETICA, 30)));
                    pdfDoc.Add(pdfTable);
                    pdfDoc.Close();
                    stream.Close();
                }
            }
        }


        //kısayol işlemleri
        private void FormMainpage_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.NumPad1)
            {
                btnpuan1.PerformClick();
            }
            if (e.KeyCode == Keys.NumPad2)
            {
                btnpuan2.PerformClick();
            }
            if (e.KeyCode == Keys.NumPad3)
            {
                butonpuan3.PerformClick();
            }
            if (e.KeyCode == Keys.NumPad4)
            {
                btnpuan4.PerformClick();
            }
            if (e.KeyCode == Keys.NumPad5)
            {
                btnpuan5.PerformClick();
            }
            if (e.KeyCode == Keys.Enter)
            {
                btnshowanswerMain.PerformClick();
            }
            // form kısayolları
            if (e.Control && e.KeyCode == Keys.A)
            {
                formAdd form = new formAdd();
                form.Show();
            }
            if (e.Control && e.KeyCode == Keys.S)
            {
                formAllData form = new formAllData();
                form.ShowDialog();
            }
            if (e.Control && e.KeyCode == Keys.D)
            {
                formDelete form = new formDelete();
                form.ShowDialog();
            }
            if (e.Control && e.KeyCode == Keys.U)
            {
                formUpdate form = new formUpdate();
                form.ShowDialog();
            }
            if (e.Control && e.KeyCode == Keys.P)
            {
                formPuanTable form = new formPuanTable();
                form.ShowDialog();
            }
            if (e.Control && e.KeyCode == Keys.E)
            {
                Application.Exit();
            }

        }
        private void kısayollarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formShortCuts formShortCuts = new formShortCuts();
            formShortCuts.ShowDialog();
        }

        private void indirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comboboxlisteMain.SelectedItem == null)
            {
                MessageBox.Show("PDF halinde almak istediğiniz listeyi anasayfadan seçiniz.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                comboboxlisteMain.Focus();
            }
        }

        private void labelsoru_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
